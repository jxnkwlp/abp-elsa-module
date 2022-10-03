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
import { message } from 'antd';
import React, { useCallback, useEffect, useImperativeHandle, useRef } from 'react';
import { useIntl } from 'umi';
import './flow.less';
import { registerNodeTypes } from './node';
import NodeTypesPanel from './node-types';
import {
    compareOutputEdges,
    createGraph,
    createNodeConfig,
    getNodeTypeRawData,
    graphCreateEdge,
    graphValidateConnection,
    graphValidateEdge,
    graphValidateMagnet,
    toggleNodePortVisible,
} from './service';
import type { IGraphData, NodeTypeStyleName, NodeUpdateData, ToolBarGroupData } from './type';
import { NodeTypeStyleNames } from './type';

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
    onEdgeClick?: (edge: Edge.Properties) => void;
    onEdgeDoubleClick?: (edge: Edge.Properties) => void;
    //
    onGraphInitial?: (graph: Graph) => void;
    onDataUpdate?: (graph: Graph) => void;
    //
    onBlankClick?: () => void;
};

export type FlowActionType = {
    getGraphData: () => Promise<IGraphData>;
    updateNodeProperties: (id: string, value: NodeUpdateData) => void;
    updateNodeOutPorts: (id: string, outNames: string[]) => void;
    setNodeStyle: (id: string, style: NodeTypeStyleName) => void;
    setAllNodesStyle: (style: NodeTypeStyleName) => void;
    setEdgeStyle: (id: string, style: NodeTypeStyleName) => void;
    setAllEdgesStyle: (style: NodeTypeStyleName) => void;
    setNodeIncomingEdgesStyle: (id: string, style: NodeTypeStyleName) => void;
    setNodeOutgoingEdgesStyle: (id: string, style: NodeTypeStyleName) => void;
    setNodeOutgoingEdgeStyle: (id: string, name: string, style: NodeTypeStyleName) => void;
};

const Flow: React.FC<IFlowProps> = (props: IFlowProps) => {
    const { actionRef, graphData } = props;
    const intl = useIntl();

    const graphInstance = useRef<Graph>();

    const [toolbarItemData, setToolbarItemData] = React.useState<ToolBarGroupData[]>([]);

    const [dnd, setDnd] = React.useState<Dnd | undefined>();

    useImperativeHandle(actionRef, () => ({
        getGraphData: async () => {
            const cells = graphInstance.current?.toJSON().cells;
            const edges = cells?.filter((x) => x.shape == 'edge' || x.shape == 'bpmn-edge');
            const nodes = cells?.filter((x) => x.shape != 'edge' && x.shape != 'bpmn-edge');
            return { nodes, edges } as IGraphData;
        },
        updateNodeProperties: (id, values) => {
            const node = graphInstance.current?.getNodes().find((x) => x.id == id);
            if (!node) {
                message.error(`node id '${id}' not found`);
                return;
            }
            if (node) {
                //
                node.attr('label', { text: values.displayName });
                //
                node.prop('name', values.name);
                node.prop('displayName', values.displayName);
                node.prop('description', values.description);
                //
                node.prop('outcomes', values.outcomes);
                //
                node.prop('properties', values.properties);
                node.updateData(values.attribtues);

                //
                const beRemoveEdges = compareOutputEdges(
                    node,
                    values.outcomes,
                    graphInstance.current?.getOutgoingEdges(node) ?? [],
                );
                beRemoveEdges.forEach((item) => {
                    graphInstance.current?.removeEdge(item);
                });
            }
        },
        updateNodeOutPorts: (id, values) => {
            const node = graphInstance.current?.getNodes().find((x) => x.id == id);
            if (!node) {
                message.error(`node id '${id}' not found`);
                return;
            }
            const outcomes = (values ?? []).map((x) => x.toString());
            node.prop('outcomes', outcomes);
            // node.addPort({ group: 'bottom' });
            // setNodeOutPorts(node, outcomes);
        },
        setAllNodesStyle: (style) => {
            const allNodes = graphInstance.current?.getNodes() ?? [];
            allNodes.forEach((node) => {
                const view = graphInstance.current?.findViewByCell(node);
                view?.removeClass(NodeTypeStyleNames);
                view?.addClass(style);
            });
        },
        setNodeStyle: (id, style) => {
            const node = graphInstance.current?.getNodes().find((x) => x.id == id);
            if (node) {
                const view = node.findView(graphInstance.current!);
                view?.removeClass(NodeTypeStyleNames);
                view?.addClass(style);
            }
        },
        setAllEdgesStyle: (style) => {
            const allEdges = graphInstance.current?.getEdges() ?? [];
            allEdges.forEach((edge) => {
                const view = graphInstance.current?.findViewByCell(edge);
                view?.removeClass(NodeTypeStyleNames);
                view?.addClass(style);
            });
        },
        setEdgeStyle: (id, style) => {
            const edge = graphInstance.current?.getEdges().find((x) => x.id == id);
            if (edge) {
                const view = edge.findView(graphInstance.current!);
                view?.removeClass(NodeTypeStyleNames);
                view?.addClass(style);
            }
        },
        setNodeIncomingEdgesStyle: (id, style) => {
            const node = graphInstance.current?.getNodes().find((x) => x.id == id);
            if (node) {
                const edges = graphInstance.current?.getIncomingEdges(node);
                edges?.forEach((edge) => {
                    const view = edge.findView(graphInstance.current!);
                    view?.removeClass(NodeTypeStyleNames);
                    view?.addClass(style);
                });
            }
        },
        setNodeOutgoingEdgesStyle: (id, style) => {
            const node = graphInstance.current?.getNodes().find((x) => x.id == id);
            if (node) {
                const edges = graphInstance.current?.getOutgoingEdges(node);
                edges?.forEach((edge) => {
                    const view = edge.findView(graphInstance.current!);
                    view?.removeClass(NodeTypeStyleNames);
                    view?.addClass(style);
                });
            }
        },
        setNodeOutgoingEdgeStyle: (id, name, style) => {
            const node = graphInstance.current?.getNodes().find((x) => x.id == id);
            if (node) {
                const edges = graphInstance.current?.getOutgoingEdges(node);
                edges?.forEach((edge) => {
                    // console.log(edge.getProp('outcome'));
                    if (edge.getProp('outcome') == name || edge.id == name) {
                        const view = edge.findView(graphInstance.current!);
                        view?.removeClass(NodeTypeStyleNames);
                        view?.addClass(style);
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

    // const handleOnNodeClick = useCallback((nodeConfig: Node.Properties, node: Node) => {
    //     props?.onNodeClick?.(nodeConfig, node);
    // }, []);

    const loadGraphData = (data: IGraphData) => {
        // console.log(graphInstance.current);
        if (!graphInstance?.current) {
            console.error('graph never initial.');
            // message.error('');
            return;
        }
        // @ts-ignore
        graphInstance?.current?.fromJSON(data);
        graphInstance?.current?.centerContent({ padding: 50 });
        props?.onDataUpdate?.(graphInstance.current!);
    };

    const handleOnDrag = async (type: string, e: any) => {
        // const node = graphInstance?.createNode({
        //     ...flowNodes.activity,
        //     id: genrateId().toString(),
        //     label: type,
        // });

        if (!graphInstance.current) {
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

        const node = graphInstance.current?.createNode(nodeConfig);

        if (node) dnd?.start(node!, e);
    };

    const initial = async () => {
        const readonly = props.readonly ?? false;

        let graph: Graph;
        if (!graphInstance.current) {
            console.debug('graph initial...');

            registerNodeTypes();

            // 创建 graph
            graph = createGraph();

            // 是否可以新增边
            graph.options.connecting.validateMagnet = (e) => graphValidateMagnet(graph, e);
            // 边是否有效
            graph.options.connecting.validateConnection = (e) => graphValidateConnection(graph, e);
            // 边是否生效
            graph.options.connecting.validateEdge = (e) => graphValidateEdge(graph, e);
            // 创建边
            graph.options.connecting.createEdge = (e) => graphCreateEdge(graph, e);

            // set instance
            graphInstance.current = graph;
            props?.onGraphInitial?.(graph);
        } else {
            graph = graphInstance.current!;
        }

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
            console.debug(node);
            node.toFront();
            // handleOnNodeClick(node.getProp() as Node.Properties, node);
            props?.onNodeClick?.(node.getProp() as Node.Properties, node);
        });

        graph.on('node:dblclick', ({ node }) => {
            props.onNodeDoubleClick?.(node.getProp() as Node.Properties, node);
        });

        graph.on('edge:click', ({ edge }) => {
            console.debug(edge);
            edge.toFront();
            props.onEdgeClick?.({ ...edge });
        });

        graph.on('edge:dblclick', ({ edge }) => {
            props.onEdgeDoubleClick?.({ ...edge });
        });

        graph.on('node:mouseenter', ({ node }) => {
            if (!readonly) {
                node.addTools([
                    {
                        name: 'button-remove',
                        args: { x: 98, y: 6 },
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

    useEffect(() => {
        if (graphData && graphInstance.current) {
            console.debug('graph load: ', graphData);
            loadGraphData(graphData);
        }
    }, [graphData, graphInstance.current]);

    useEffect(() => {
        initial();

        return () => {
            if (graphInstance) graphInstance.current?.dispose();
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

    // const height = props.height ?? 600;

    return (
        <div className="flow-container">
            {(props.showNodeTypes ?? true) && <NodeTypesPanel key="types" onDrag={handleOnDrag} />}
            <div
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
                                                    item2.onClick?.(graphInstance.current!);
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
