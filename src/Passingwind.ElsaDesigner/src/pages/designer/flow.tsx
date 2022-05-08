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
import type { Edge, Node } from '@antv/x6';
import { Addon, Graph, Shape } from '@antv/x6';
import { Toolbar } from '@antv/x6-react-components';
import '@antv/x6-react-components/es/menu/style/index.css';
import '@antv/x6-react-components/es/toolbar/style/index.css';
import type { Dnd } from '@antv/x6/lib/addon';
import { message } from 'antd';
import React, { useEffect, useImperativeHandle } from 'react';
import './flow.less';
import { registerNodeTypes } from './node';
import NodeTypesPanel from './node-types';
import {
    checkCanCreateEdge,
    compareOutputEdges,
    createEdgeConfig,
    createNodeConfig,
    getGraphOptions,
    getNextEdgeName,
    getNodeTypeRawData,
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
};

const Flow: React.FC<IFlowProps> = (props: IFlowProps) => {
    const { actionRef, graphData } = props;

    const [graphInstance, setGraphInstance] = React.useState<Graph | undefined>();

    const [toolbarItemData, setToolbarItemData] = React.useState<ToolBarGroupData[]>([]);

    const [dnd, setDnd] = React.useState<Dnd | undefined>();

    useImperativeHandle(actionRef, () => ({
        getGraphData: async () => {
            const cells = graphInstance?.toJSON().cells;
            const edges = cells?.filter((x) => x.shape == 'edge' || x.shape == 'bpmn-edge');
            const nodes = cells?.filter((x) => x.shape != 'edge' && x.shape != 'bpmn-edge');
            return { nodes, edges } as IGraphData;
        },
        updateNodeProperties: (id, values) => {
            const node = graphInstance?.getNodes().find((x) => x.id == id);
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
                    graphInstance?.getOutgoingEdges(node) ?? [],
                );
                beRemoveEdges.forEach((item) => {
                    graphInstance?.removeEdge(item);
                });
            }
        },
        updateNodeOutPorts: (id, values) => {
            const node = graphInstance?.getNodes().find((x) => x.id == id);
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
            const allNodes = graphInstance?.getNodes() ?? [];
            allNodes.forEach((node) => {
                const view = graphInstance?.findViewByCell(node);
                view?.removeClass(NodeTypeStyleNames);
                view?.addClass(style);
            });
        },
        setNodeStyle: (id, style) => {
            const node = graphInstance?.getNodes().find((x) => x.id == id);
            if (node) {
                const view = node.findView(graphInstance!);
                view?.removeClass(NodeTypeStyleNames);
                view?.addClass(style);
            }
        },
        setAllEdgesStyle: (style) => {
            const allEdges = graphInstance?.getEdges() ?? [];
            allEdges.forEach((edge) => {
                const view = graphInstance?.findViewByCell(edge);
                view?.removeClass(NodeTypeStyleNames);
                view?.addClass(style);
            });
        },
        setEdgeStyle: (id, style) => {
            const edge = graphInstance?.getEdges().find((x) => x.id == id);
            if (edge) {
                const view = edge.findView(graphInstance!);
                view?.removeClass(NodeTypeStyleNames);
                view?.addClass(style);
            }
        },
        setNodeIncomingEdgesStyle: (id, style) => {
            const node = graphInstance?.getNodes().find((x) => x.id == id);
            if (node) {
                const edges = graphInstance?.getIncomingEdges(node);
                edges?.forEach((edge) => {
                    const view = edge.findView(graphInstance!);
                    view?.removeClass(NodeTypeStyleNames);
                    view?.addClass(style);
                });
            }
        },
        setNodeOutgoingEdgesStyle: (id, style) => {
            const node = graphInstance?.getNodes().find((x) => x.id == id);
            if (node) {
                const edges = graphInstance?.getOutgoingEdges(node);
                edges?.forEach((edge) => {
                    const view = edge.findView(graphInstance!);
                    view?.removeClass(NodeTypeStyleNames);
                    view?.addClass(style);
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

    const loadGraphData = (data: IGraphData) => {
        // @ts-ignore
        graphInstance?.fromJSON(data);
        graphInstance?.centerContent({ padding: 50 });
        props?.onDataUpdate?.(graphInstance!);
    };

    const handleOnDrag = async (type: string, e: any) => {
        // const node = graphInstance?.createNode({
        //     ...flowNodes.activity,
        //     id: genrateId().toString(),
        //     label: type,
        // });

        if (!graphInstance) {
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

        // graphInstance.addNode({ ...nodeConfig, x: 100, y: 100 });

        const node = graphInstance.createNode(nodeConfig);

        if (node) dnd?.start(node!, e);
    };

    const initial = async () => {
        const readonly = props.readonly ?? false;

        if (!graphInstance) {
            console.debug('graph initial...');

            registerNodeTypes();

            const graph = new Graph({
                container: document.getElementById('container')!,
                // height: props.height ?? 600,
                // width: '100vm',
                autoResize: true,
                grid: {
                    size: 10,
                    visible: true,
                },
                history: {
                    enabled: true,
                    // ignoreChange: true,
                    beforeAddCommand(event, args) {
                        // @ts-ignore
                        if (args?.key == 'tools') {
                            return false;
                        }
                        return true;
                    },
                },
                mousewheel: {
                    enabled: true,
                    zoomAtMousePosition: true,
                    modifiers: ['ctrl'],
                    minScale: 0.5,
                    maxScale: 1.5,
                },
                scroller: {
                    padding: 40,
                },
                panning: {
                    enabled: true,
                    // modifiers: 'ctrl',
                },
                minimap: {
                    enabled: true,
                    container: document.getElementById('minimap')!,
                    width: 230,
                    height: 180,
                },
                snapline: true,
                keyboard: true,
                selecting: {
                    enabled: true,
                    showNodeSelectionBox: false,
                },
                clipboard: {
                    enabled: true,
                    useLocalStorage: true,
                },
                translating: {
                    restrict: true,
                },
                connecting: {
                    router: {
                        name: 'manhattan',
                        args: {
                            step: 10,
                        },
                    },
                    connector: {
                        name: 'rounded',
                        args: {
                            radius: 10,
                        },
                    },
                    anchor: 'center',
                    connectionPoint: 'anchor',
                    allowBlank: false,
                    allowMulti: 'withPort',
                    snap: true,
                    highlight: true,
                    allowLoop: false,
                    allowEdge: false,
                    // 是否可以新增边
                    validateMagnet({ cell, view, magnet }) {
                        if (!magnet) return false;

                        if (cell.isNode())
                            if (!checkCanCreateEdge(cell, graph.getOutgoingEdges(cell) ?? [])) {
                                message.error(`No more outcomes.`);
                                return false;
                            }

                        //
                        return true;
                    },
                    // 边是否有效
                    validateConnection({ sourceCell, targetMagnet }) {
                        if (!targetMagnet) {
                            return false;
                        }

                        return true;
                    },
                    // 边是否生效
                    validateEdge({ edge, type }) {
                        console.log(edge, type);
                        if (edge.getProp('shape') != 'bpmn-edge') {
                            return false;
                        }
                        // if (edge.get) {
                        //     const nextName = getNextEdgeName(
                        //         sourceCell,
                        //         graph.getOutgoingEdges(sourceCell) ?? [],
                        //     );
                        //     if (!nextName) {
                        //         return false;
                        //     }
                        // }
                        return true;
                    },
                    // 创建边
                    createEdge: ({ sourceCell, sourceMagnet }) => {
                        // const portName = sourceMagnet.getAttribute('port') ?? 'Done';
                        // console.log(typeof sourceCell);
                        if (sourceCell.isNode()) {
                            const nextName = getNextEdgeName(
                                sourceCell,
                                graph.getOutgoingEdges(sourceCell) ?? [],
                            );
                            if (nextName) {
                                return new Shape.Edge(
                                    createEdgeConfig({ id: '', label: nextName }),
                                );
                            } else {
                                message.error(`No more outcomes.`);
                                return null;
                            }
                        } else return null;
                    },
                },
                highlighting: {
                    // 连线过程中，自动吸附到链接桩时被使用。
                    magnetAdsorbed: {
                        name: 'stroke',
                        args: {
                            attrs: {
                                fill: '#fff',
                                stroke: '#47C769',
                                'stroke-width': 2,
                            },
                        },
                    },
                    magnetAvailable: {
                        name: 'stroke',
                        args: {
                            attrs: {
                                fill: '#fff',
                                stroke: '#47C769',
                                'stroke-width': 1,
                            },
                        },
                    },
                },

                ...getGraphOptions(),
            });

            setGraphInstance(graph);
            props?.onGraphInitial?.(graph);

            if (readonly) {
                graph.disableClipboard();
                graph.disableHistory();
                graph.disableSnapline();
                graph.disableSelection();
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
                props.onNodeClick?.(node.getProp() as Node.Properties, node);
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
            });
        }
    };

    useEffect(() => {
        if (graphData && graphInstance) {
            console.debug('graph load: ', graphData);
            loadGraphData(graphData);
        }
    }, [graphData, graphInstance]);

    useEffect(() => {
        initial();

        return () => {
            if (graphInstance) graphInstance.dispose();
        };
    }, []);

    useEffect(() => {
        if (!props.toolbars) {
            const toolbarItems: ToolBarGroupData[] = [
                {
                    group: '1',
                    items: [
                        {
                            name: 'zoomIn',
                            tooltip: 'Zoom In',
                            icon: <ZoomInOutlined />,
                            onClick: (g) => {
                                handleToolbarClick(g, 'zoomIn');
                            },
                        },
                        {
                            name: 'zoomOut',
                            tooltip: 'Zoom Out',
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
                            tooltip: 'Undo',
                            icon: <UndoOutlined />,
                            onClick: (g) => {
                                handleToolbarClick(g, 'undo');
                            },
                        },
                        {
                            name: 'redo',
                            tooltip: 'Redo',
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
                            tooltip: 'Center',
                            icon: <OneToOneOutlined />,
                            onClick: (g) => {
                                handleToolbarClick(g, 'center');
                            },
                        },
                        {
                            name: 'compress',
                            tooltip: 'Auto Size',
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
                            tooltip: 'Copy Selected',
                            icon: <CopyOutlined />,
                            onClick: (g) => {
                                handleToolbarClick(g, 'copy');
                            },
                        },
                        {
                            name: 'delete',
                            tooltip: 'Delete Selected',
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
    }, []);

    const height = props.height ?? 600;

    return (
        <div className="flow-container" style={{ height: height + 'px' }}>
            {(props.showNodeTypes ?? true) && <NodeTypesPanel key="types" onDrag={handleOnDrag} />}
            <div
                id="container"
                className="graph-container"
                style={{
                    marginTop: props.showToolbar ?? true ? 40 : 0,
                    height: props.showToolbar ?? true ? height - 40 + 'px' : height + 'px',
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
                                                    item2.onClick?.(graphInstance!);
                                                }}
                                            />
                                        );
                                    })}
                                </Toolbar.Group>
                            );
                        })}
                        {/* <Toolbar.Group>
                            <Toolbar.Item
                                name="zoomIn"
                                tooltip="Zoom In "
                                icon={<ZoomInOutlined />}
                            />
                            <Toolbar.Item
                                name="zoomOut"
                                tooltip="Zoom Out"
                                icon={<ZoomOutOutlined />}
                            />
                        </Toolbar.Group>
                        <Toolbar.Group>
                            <Toolbar.Item
                                name="undo"
                                tooltip="Undo"
                                icon={<UndoOutlined />}
                                disabled={graphInstance?.canUndo() ?? true}
                            />
                            <Toolbar.Item
                                name="redo"
                                tooltip="Redo"
                                icon={<RedoOutlined />}
                                disabled={graphInstance?.canRedo() ?? true}
                            />
                        </Toolbar.Group>
                        <Toolbar.Group>
                            <Toolbar.Item
                                name="center"
                                tooltip="Center"
                                icon={<OneToOneOutlined />}
                            />
                            <Toolbar.Item
                                name="compress"
                                tooltip="Auto Size"
                                icon={<CompressOutlined />}
                            />
                        </Toolbar.Group> */}
                    </Toolbar>
                </div>
            )}
        </div>
    );
};

export default Flow;
