import { randString } from '@/services/utils';
import {
    CompressOutlined,
    CopyOutlined,
    DeleteOutlined,
    OneToOneOutlined,
    RedoOutlined,
    UndoOutlined,
    ZoomInOutlined,
    ZoomOutOutlined,
} from '@ant-design/icons';
import type { Edge, Graph, Node } from '@antv/x6';
import { Addon } from '@antv/x6';
import { Toolbar } from '@antv/x6-react-components';
import '@antv/x6-react-components/es/menu/style/index.css';
import '@antv/x6-react-components/es/toolbar/style/index.css';
import type { Dnd } from '@antv/x6/lib/addon';
import { message, Modal } from 'antd';
import React, { useEffect, useImperativeHandle } from 'react';
import { useIntl } from 'umi';
import './flow.less';
import { registerNodeTypes } from './node';
import NodeTypesPanel from './node-types';
import {
    compareOutputEdges,
    createDefaultGraph,
    createNodeConfig,
    getNodeTypeRawData,
    graphCreateEdge,
    graphValidateConnection,
    graphValidateEdge,
    graphValidateMagnet,
    toggleNodePortVisible,
    updateEdgeStatus,
} from './service';
import type { IGraphData, NodeStatus, NodeUpdateData, ToolBarGroupData } from './type';

export type FlowActionType = {
    getGraphData: () => Promise<IGraphData>;
    updateNodeProperties: (id: string, value: NodeUpdateData) => void;
    updateNodeOutPorts: (id: string, outNames: string[]) => void;
    //
    setNodeStatus: (id: string, status: NodeStatus) => void;
    setAllNodeStatus: (status: NodeStatus) => void;
    setEdgeStyle: (id: string, status: NodeStatus) => void;
    setAllEdgesStyle: (status: NodeStatus) => void;
    //
    setEdgeName: (id: string, name: string) => void;
    //
    // setNodeIncomingEdgesStyle: (id: string, status: NodeStatus) => void;
    // setNodeOutgoingEdgesStyle: (id: string, status: NodeStatus) => void;
    setNodeOutgoingEdgeStyle: (id: string, edgeId: string, status: NodeStatus) => void;
};

type IFlowProps = {
    height?: number;
    actionRef?: React.Ref<FlowActionType | undefined>;
    graphData?: IGraphData;
    readonly?: boolean;
    showNodeTypes?: boolean;
    showMiniMap?: boolean;
    showToolbar?: boolean;
    toolbars?: ToolBarGroupData[];
    //node
    onNodeClick?: (nodeConfig: Node.Properties, node: Node) => void;
    onNodeDoubleClick?: (nodeConfig: Node.Properties, node: Node) => void;
    onEdgeClick?: (graph: Graph, edge: Edge<Edge.Properties>) => void;
    onEdgeDoubleClick?: (graph: Graph, edge: Edge<Edge.Properties>) => void;
    //
    onGraphInitial?: (graph: Graph) => void;
    onDataUpdate?: (graph: Graph) => void;
    //
    onBlankClick?: () => void;
};

const Flow: React.FC<IFlowProps> = (props: IFlowProps) => {
    const { actionRef, graphData } = props;

    const graphElementRef = React.useRef<HTMLDivElement>(null);
    const graphRef = React.useRef<Graph>();

    const intl = useIntl();

    const [toolbarItemData, setToolbarItemData] = React.useState<ToolBarGroupData[]>([]);

    const [dnd, setDnd] = React.useState<Dnd | undefined>();

    useImperativeHandle(actionRef, () => ({
        getGraphData: async () => {
            const cells = graphRef.current?.toJSON().cells;
            const edges = cells?.filter((x) => x.shape == 'edge' || x.shape == 'elsa-edge');
            const nodes = cells?.filter((x) => x.shape != 'edge' && x.shape != 'elsa-edge');
            return { nodes, edges } as IGraphData;
        },
        updateNodeProperties: (id, values) => {
            const node = graphRef.current?.getNodes().find((x) => x.id == id);
            if (!node) {
                message.error(`node id '${id}' not found`);
                return;
            }
            if (node) {
                //
                // node.attr('label', { text: values.displayName });
                //
                // node.prop('id', values.name);
                node.prop('name', values.name);
                node.prop('label', values.label);
                //
                node.prop('outcomes', values.outcomes);
                //
                node.prop('properties', values.properties);

                // save data
                node.replaceData(values);

                //
                const beRemoveEdges = compareOutputEdges(
                    node,
                    values.outcomes,
                    graphRef.current?.getOutgoingEdges(node) ?? [],
                );
                beRemoveEdges.forEach((item) => {
                    graphRef.current?.removeEdge(item);
                });
            }
        },
        updateNodeOutPorts: (id, values) => {
            const node = graphRef.current?.getNodes().find((x) => x.id == id);
            if (!node) {
                message.error(`node id '${id}' not found`);
                return;
            }
            const outcomes = (values ?? []).map((x) => x.toString());
            node.prop('outcomes', outcomes);
            // node.addPort({ group: 'bottom' });
            // setNodeOutPorts(node, outcomes);
        },
        //
        setAllNodeStatus: (status) => {
            const allNodes = graphRef.current?.getNodes() ?? [];
            allNodes.forEach((node) => {
                node?.updateData({ status: status });
            });
        },
        setNodeStatus: (id, status) => {
            const node = graphRef.current?.getNodes().find((x) => x.id == id);
            node?.updateData({ status: status });
        },
        setAllEdgesStyle: (status) => {
            const allEdges = graphRef.current?.getEdges() ?? [];
            allEdges.forEach((edge) => {
                edge?.updateData({ status: status });
                updateEdgeStatus(edge, status);
            });
        },
        setEdgeStyle: (id, status) => {
            const edge = graphRef.current?.getEdges().find((x) => x.id == id);
            if (edge) {
                edge?.updateData({ status: status });
                updateEdgeStatus(edge!, status);
            }
        },
        setEdgeName: (id, name) => {
            const edge = graphRef.current?.getEdges().find((x) => x.id == id);
            if (edge) {
                // edge.updateAttrs({ label: name, name: name });
                edge.setProp('name', name);
                edge.setLabels(name);
                edge.setProp('outcome', name);
                edge?.updateData({ label: name, name: name, outcome: name });
            }
        },
        //
        // setNodeIncomingEdgesStyle: (id, status) => {
        //     const node = graphRef.current?.getNodes().find((x) => x.id == id);
        //     if (node) {
        //         const edges = graphRef.current?.getIncomingEdges(node);
        //         edges?.forEach((edge) => {
        //             const view = edge.findView(graphRef.current!);
        //             view?.removeClass(NodeTypeStyleNames);
        //             view?.addClass(style);
        //         });
        //     }
        // },
        // setNodeOutgoingEdgesStyle: (id, style) => {
        //     const node = graphRef.current?.getNodes().find((x) => x.id == id);
        //     if (node) {
        //         const edges = graphRef.current?.getOutgoingEdges(node);
        //         edges?.forEach((edge) => {
        //             const view = edge.findView(graphRef.current!);
        //             view?.removeClass(NodeTypeStyleNames);
        //             view?.addClass(style);
        //         });
        //     }
        // },
        setNodeOutgoingEdgeStyle: (id, edgeId, status) => {
            const node = graphRef.current?.getNodes().find((x) => x.id == id);
            if (node) {
                const edges = graphRef.current?.getOutgoingEdges(node);
                edges?.forEach((edge) => {
                    if (edge.getProp('outcome') == edgeId || edge.id == edgeId) {
                        edge?.updateData({ status: status });
                        updateEdgeStatus(edge!, status);
                    }
                });
            }
        },
    }));

    const handleToolbarClick = (graph: Graph, name: string, value?: string) => {
        switch (name) {
            case 'zoomIn':
                {
                    const currentValue = graph?.zoom() ?? 1;
                    if (
                        graph?.options?.mousewheel?.maxScale &&
                        currentValue >= graph?.options?.mousewheel?.maxScale
                    ) {
                        return;
                    }
                    graph?.zoom(0.1, { absolute: false });
                }
                break;
            case 'zoomOut':
                {
                    const currentValue = graph?.zoom() ?? 1;
                    if (
                        graph?.options?.mousewheel?.minScale &&
                        currentValue <= graph?.options?.mousewheel?.minScale
                    ) {
                        return;
                    }
                    graph?.zoom(-0.1, { absolute: false });
                }
                break;
            case 'redo':
                {
                    if (graph?.canRedo()) {
                        graph?.redo();
                    } else {
                        message.info('Nothing to redo');
                    }
                }
                break;
            case 'undo':
                {
                    if (graph?.canUndo()) {
                        graph?.undo();
                    } else {
                        message.info('Nothing to undo');
                    }
                }
                break;
            case 'center':
                {
                    graph?.zoom(1, { absolute: true });
                    graph?.centerContent({ padding: 50 });
                }
                break;
            case 'compress':
                {
                    graph?.scaleContentToFit();
                    graph?.centerContent({ padding: 50 });
                }
                break;
            case 'delete':
                {
                    const cells = graph?.getSelectedCells() ?? [];
                    if (cells.length > 0) {
                        graph.removeCells(cells);
                    }
                }
                break;
            case 'copy':
                {
                    const cells = graph?.getSelectedCells() ?? [];
                    //
                    if (cells?.length == 0) {
                        message.info('No node selected.');
                        return;
                    }
                    graph.cleanSelection();
                    graph?.copy(cells, { deep: false });
                    //
                    if (!graph?.isClipboardEmpty()) {
                        const c2 = graph?.paste({
                            offset: 80,
                            nodeProps: {
                                name: randString(),
                            },
                        });
                        if (c2) {
                            graph.select(c2);
                        }
                    }
                }
                break;
        }
    };

    const loadGraphData = (graph: Graph, data: IGraphData) => {
        try {
            props?.onDataUpdate?.(graph);
            graph.fromJSON(data);
            // @ts-ignore
            graph.centerContent({ padding: 50 });
        } catch (error) {
            console.error(error);
        }
    };

    const handleOnDrag = async (type: string, e: any) => {
        // const node = graphInstance?.createNode({
        //     ...flowNodes.activity,
        //     id: genrateId().toString(),
        //     label: type,
        // });

        if (!graphRef.current) {
            return;
        }

        const allNodeTypes = await getNodeTypeRawData();
        const nodeType = allNodeTypes?.items?.find((x) => x.type == type);
        if (!nodeType) {
            message.error(`Node type ${type} not exists.`);
            return;
        }

        const nodeConfig = createNodeConfig({
            type: type,
            id: '',
            outcomes: nodeType.outcomes ?? [],
            displayName: nodeType.displayName,
            label: nodeType.displayName,
            typeDescriptor: type,
        });

        const node = graphRef.current?.createNode(nodeConfig);

        if (node) dnd?.start(node!, e);
    };

    useEffect(() => {
        if (graphData && graphRef.current) {
            console.debug('graph load: ', graphData);
            loadGraphData(graphRef.current, graphData);
        }
    }, [graphData, graphRef.current]);

    useEffect(() => {
        const configeGraph = (graph: Graph) => {
            registerNodeTypes();

            const readonly = props.readonly ?? false;

            // 是否可以新增边
            graph.options.connecting.validateMagnet = (e) => graphValidateMagnet(graph, e);
            // 边是否有效
            graph.options.connecting.validateConnection = (e) => graphValidateConnection(graph, e);
            // 边是否生效
            graph.options.connecting.validateEdge = (e) => graphValidateEdge(graph, e);
            // 创建边
            graph.options.connecting.createEdge = (e) => graphCreateEdge(graph, e);

            if (readonly) {
                graph.disableClipboard();
                graph.disableHistory();
                graph.disableSnapline();
                // graph.disableSelection();
                graph.disableRubberband();
                // @ts-ignore
                graph.options.interacting.nodeMovable = false;
            }

            const dnd = new Addon.Dnd({
                target: graph!,
                scaled: false,
                animation: true,
                // getDragNode: (node) => node.clone({ keepId: true }),
                // getDropNode: (node) => {
                //     const node2 = node.clone({ keepId: true });
                //     // updateNodePorts(node2);
                //     return node2;
                // },
            });
            setDnd(dnd);

            graph.on('node:click', ({ node }) => {
                console.debug('node', node);
                console.debug('node data', node.getData());
                console.debug('node prop', node.prop());
                node.toFront();
                props?.onNodeClick?.(node.getProp() as Node.Properties, node);
            });

            graph.on('node:dblclick', ({ node }) => {
                props.onNodeDoubleClick?.(node.getProp() as Node.Properties, node);
            });

            graph.on('edge:click', ({ edge }) => {
                console.debug('edge', edge);
                console.debug('edge data', edge.getData());
                console.debug('edge prop', edge.prop());
                edge.toFront();
                props.onEdgeClick?.(graph, edge);
            });

            graph.on('edge:dblclick', ({ edge }) => {
                props.onEdgeDoubleClick?.(graph, edge);
            });

            graph.on('node:mouseenter', ({ node }) => {
                if (!readonly) {
                    node.addTools([
                        {
                            name: 'button-remove',
                            args: { x: '100%', y: 0, offset: { x: -5, y: 5 } },
                        },
                    ]);

                    toggleNodePortVisible(node, true);
                }
            });

            graph.on('node:mouseleave', ({ node }) => {
                node.removeTools();
                toggleNodePortVisible(node, false);
            });

            graph.on('edge:mouseenter', ({ edge }) => {
                if (!readonly) {
                    edge.toFront();
                    edge.addTools([
                        {
                            name: 'button-remove',
                            args: { distance: -40 },
                        },
                        {
                            name: 'target-arrowhead',
                            args: {
                                attrs: {
                                    d: 'M -14 -8 2 0 -14 8 Z',
                                    'stroke-width': 0,
                                    fill: '#1890ff',
                                },
                            },
                        },
                    ]);
                }
            });

            graph.on('edge:mouseleave', ({ edge }) => {
                edge.removeTools();
                edge.setZIndex(-1);
            });

            graph.on('blank:click', ({}) => {
                (graph.getCells() ?? []).forEach((item) => {
                    if (item.isNode()) {
                        toggleNodePortVisible(item, false);
                    }
                    if (item.isEdge()) {
                        item.setZIndex(-1);
                        item.removeTools();
                    }
                });

                // call
                props?.onBlankClick?.();
            });
        };

        // create
        const graph = createDefaultGraph(graphElementRef.current);
        graphRef.current = graph;

        // update
        configeGraph(graph);

        const destroy = () => {
            graph.dispose();
        };

        return () => {
            destroy();
        };
    }, [0]);

    useEffect(() => {
        if (!props.toolbars) {
            const toolbarItems: ToolBarGroupData[] = [
                {
                    group: '1',
                    items: [
                        {
                            name: 'zoomIn',
                            tooltip: intl.formatMessage({ id: 'page.designer.toolbar.zoomIn' }),
                            icon: <ZoomInOutlined />,
                            onClick: (g) => {
                                handleToolbarClick(g, 'zoomIn');
                            },
                        },
                        {
                            name: 'zoomOut',
                            tooltip: intl.formatMessage({ id: 'page.designer.toolbar.zoomOut' }),
                            icon: <ZoomOutOutlined />,
                            onClick: (g) => {
                                handleToolbarClick(g, 'zoomOut');
                            },
                        },
                    ],
                },
                {
                    group: '2',
                    items: [
                        {
                            name: 'undo',
                            tooltip: intl.formatMessage({ id: 'page.designer.toolbar.undo' }),
                            icon: <UndoOutlined />,
                            onClick: (g) => {
                                handleToolbarClick(g, 'undo');
                            },
                        },
                        {
                            name: 'redo',
                            tooltip: intl.formatMessage({ id: 'page.designer.toolbar.redo' }),
                            icon: <RedoOutlined />,
                            onClick: (g) => {
                                handleToolbarClick(g, 'redo');
                            },
                        },
                    ],
                },
                {
                    group: '3',
                    items: [
                        {
                            name: 'center',
                            tooltip: intl.formatMessage({ id: 'page.designer.toolbar.center' }),
                            icon: <OneToOneOutlined />,
                            onClick: (g) => {
                                handleToolbarClick(g, 'center');
                            },
                        },
                        {
                            name: 'compress',
                            tooltip: intl.formatMessage({ id: 'page.designer.toolbar.autoSize' }),
                            icon: <CompressOutlined />,
                            onClick: (g) => {
                                handleToolbarClick(g, 'compress');
                            },
                        },
                    ],
                },
                {
                    group: '4',
                    items: [
                        {
                            name: 'copy',
                            tooltip: intl.formatMessage({ id: 'page.designer.toolbar.copy' }),
                            icon: <CopyOutlined />,
                            onClick: (g) => {
                                handleToolbarClick(g, 'copy');
                            },
                        },
                        {
                            name: 'delete',
                            tooltip: intl.formatMessage({ id: 'page.designer.toolbar.delete' }),
                            icon: <DeleteOutlined />,
                            onClick: (g) => {
                                handleToolbarClick(g, 'delete');
                            },
                        },
                    ],
                },
            ];
            setToolbarItemData(toolbarItems);
        } else {
            setToolbarItemData(props.toolbars ?? []);
        }
    }, [0]);

    return (
        <div className="flow-container">
            {(props.showNodeTypes ?? true) && <NodeTypesPanel key="types" onDrag={handleOnDrag} />}
            <div
                ref={graphElementRef}
                id="graphContainer"
                className="graph-container"
                style={{
                    marginTop: props.showToolbar ?? true ? 40 : 0,
                    height: props.showToolbar ?? true ? 'calc(100% - 40px)' : '100%',
                    overflow: 'hidden',
                }}
            />
            {(props.showMiniMap ?? true) && <div id="minimap" className="minimap-container" />}
            {(props.showToolbar ?? true) && (
                <div
                    id="toolbar"
                    className="toolbar-container"
                    style={{ left: props.showNodeTypes ?? true ? 260 : 0 }}
                >
                    <Toolbar hoverEffect size="big">
                        {toolbarItemData.map((item, index) => {
                            return (
                                <Toolbar.Group key={item.group}>
                                    {item.items.map((item2, index2) => {
                                        return (
                                            <Toolbar.Item
                                                key={item2.name}
                                                name={item2.name}
                                                tooltip={item2.tooltip}
                                                icon={item2.icon}
                                                onClick={() => {
                                                    item2.onClick?.(graphRef.current!);
                                                }}
                                            />
                                        );
                                    })}
                                </Toolbar.Group>
                            );
                        })}
                    </Toolbar>
                </div>
            )}
        </div>
    );
};

export default Flow;
