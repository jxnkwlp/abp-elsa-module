import { getDesignerActivityTypes, getDesignerScriptTypeDefinition } from '@/services/Designer';
import { API } from '@/services/typings';
import { randString } from '@/services/utils';
import { CellView, Edge, Graph, Node, Shape } from '@antv/x6';
import type { PortManager } from '@antv/x6/lib/model/port';
import { uuid } from '@antv/x6/lib/util/string/uuid';
import { message } from 'antd';
import type { IGraphData, NodePropertySyntax, NodeTypeGroup, NodeTypeProperty } from './type';

// export const getTestData = () => {
//     const id1 = genrateId();
//     const id2 = genrateId();
//     const id3 = genrateId();
//     return {
//         // 节点
//         nodes: [
//             {
//                 id: id1, // String，可选，节点的唯一标识
//                 x: 18, // Number，必选，节点位置的 x 值
//                 y: 10, // Number，必选，节点位置的 y 值
//                 // width: 80, // Number，可选，节点大小的 width 值
//                 // height: 40, // Number，可选，节点大小的 height 值
//                 label: 'activity', // String，节点标签
//                 shape: 'activity',
//                 type: 'activity',
//                 name: 'activity',
//             },
//             {
//                 id: id2, // String，节点的唯一标识
//                 x: 300, // Number，必选，节点位置的 x 值
//                 y: 180, // Number，必选，节点位置的 y 值
//                 // width: 80, // Number，可选，节点大小的 width 值
//                 // height: 40, // Number，可选，节点大小的 height 值
//                 label: 'event', // String，节点标签
//                 shape: 'event',
//             },
//             {
//                 id: id3, // String，节点的唯一标识
//                 x: 0, // Number，必选，节点位置的 x 值
//                 y: 300, // Number，必选，节点位置的 y 值
//                 // width: 80, // Number，可选，节点大小的 width 值
//                 // height: 40, // Number，可选，节点大小的 height 值
//                 label: 'gateway', // String，节点标签
//                 shape: 'gateway',
//             },
//         ],
//         // 边
//         edges: [
//             {
//                 source: id1, // String，必须，起始节点 id
//                 target: id2, // String，必须，目标节点 id
//             },
//         ],
//     };
// };

export const toggleNodePortVisible = (node: Node, visible: boolean) => {
    node.getPorts().forEach((item) => {
        node.setPortProp(item.id!, 'attrs/circle', {
            style: { visibility: visible ? 'visible' : 'hidden' },
        });
    });
};

export const getNodeTypeRawData = async (force: boolean = false) => {
    const cache = localStorage.getItem('nodeType')
        ? (JSON.parse(
              localStorage.getItem('nodeType') ?? '{}',
          ) as API.ActivityTypeDescriptorListResult)
        : null;

    if (!cache || force) {
        const result = await getDesignerActivityTypes();
        localStorage.setItem('nodeType', JSON.stringify(result));
        return result;
    } else {
        return cache;
    }
};

// 侧边节点类型数据
export const getNodeTypeData = async (): Promise<NodeTypeGroup[]> => {
    const result = await getNodeTypeRawData(true);

    if (result?.items?.length == 0) {
        return [];
    }

    const nodeData: NodeTypeGroup[] = (result?.categories ?? []).map((x) => {
        return { id: x, name: x, children: [] };
    });

    result?.items?.forEach((item) => {
        const g = nodeData.find((x) => x.id === item.category);
        g?.children?.push({
            label: item.displayName,
            // inherit: 'rect',
            // width: 100,
            // height: 60,
            // attrs: {
            //     body: {
            //         rx: 6,
            //         ry: 6,
            //         stroke: '#5F95FF',
            //         fill: '#EFF4FF',
            //         strokeWidth: 1,
            //     },
            //     // img: {
            //     //     x: 6,
            //     //     y: 6,
            //     //     width: 16,
            //     //     height: 16,
            //     //     'xlink:href':
            //     //         'https://gw.alipayobjects.com/mdn/rms_43231b/afts/img/A*pwLpRr7QPGwAAAAAAAAAAAAAARQnAQ',
            //     // },
            //     label: {
            //         fontSize: 12,
            //         fill: '#262626',
            //     },
            // },
            // typeDescriptor: item,
            type: item.type,
            outcomes: item.outcomes ?? [],
        });
    });
    return nodeData;
};

export const getEditorLanguage = (syntax: string) => {
    switch (syntax) {
        case 'JavaScript':
            return 'javascript';
        case 'Json':
            return 'json';
        case 'Liquid':
            return 'handlebars';
        case 'Literal':
        default:
            return 'plaintext';
    }
};

/**
 *  获取属性值配置
 */
export const getPropertySyntaxes = (property: NodeTypeProperty): NodePropertySyntax => {
    let syntaxes = (property.supportedSyntaxes ?? []).map((x) => {
        return x;
    });
    let defaultSyntax: string | undefined = property.defaultSyntax ?? undefined;
    let editorSyntax = '';
    //
    if (defaultSyntax && syntaxes.length == 0) {
        syntaxes.push(defaultSyntax);
    }

    // if (property.uiHint != 'switch-case-builder' && syntaxes.indexOf('Literal') == -1) {
    //     syntaxes = ['Literal', ...syntaxes];
    // }
    if (defaultSyntax == 'Switch') {
        syntaxes.push('Json');
    }
    // if (!defaultSyntax && syntaxes.length > 0) {
    //     defaultSyntax = syntaxes[0];
    // }
    if (property.options?.syntax) {
        // syntaxes = [property.options?.syntax];
        syntaxes = [];
        defaultSyntax = 'Literal';
        editorSyntax = property.options?.syntax;
    }
    if (
        (property.uiHint === 'single-line' ||
            property.uiHint === 'multi-line' ||
            property.uiHint === 'dropdown' ||
            property.uiHint === 'checkbox') &&
        syntaxes.indexOf('Literal') == -1
    ) {
        syntaxes = ['Literal', ...syntaxes];
    }

    // end
    if (syntaxes.length > 0) defaultSyntax = syntaxes[0];

    return {
        supports: syntaxes,
        default: defaultSyntax,
        editor: editorSyntax,
    } as NodePropertySyntax;
};

// export const configNode = (node: Node.Metadata) => {
//     const nodeId = node.id || uuid();
//     node.id = nodeId.toString();
//     return node;
// };

//
// 1. 1个出口，允许多线 结构
// 2. 多个出口，1口1线 结构
//

export const getNodeOutcomes = () => {};

// 更新节点输出端口
export const setNodeOutPorts = (node: Node<Node.Properties>, outcomes: string[]) => {
    // remove out ports
    const buttomPorts = node.getPortsByGroup('bottom');

    node.removePorts(buttomPorts, { silent: true });

    // call diff
    // console.log(node.getParsedPorts());
    // const currentPortNames = buttomPorts.map((x) => x.name);
    // node.addPorts(
    //     outcomes.map((item) => {
    //         return {
    //             id: uuid(),
    //             group: 'bottom',
    //             tooltip: item,
    //             outcome: item,
    //             // attrs: {
    //             //     text: {
    //             //         text: item,
    //             //     },
    //             // },
    //         };
    //     }) as PortManager.PortMetadata[],
    // );
};

export const updateNodePorts = (node: Node<Node.Properties>) => {
    // if (currentPorts.length > 0) node.removePort(currentPorts[1]);
    // const currentPorts = node.getPorts();
    // remove out ports
    const buttomPorts = node.getPortsByGroup('bottom');
    node.removePorts(buttomPorts, { silent: true });
    // add out port
    const outcomes: string[] = node.getProp('outcomes') ?? [];
    node.addPorts(
        outcomes.map((item) => {
            return {
                id: uuid(),
                group: 'bottom',
                label: item,
                outcome: item,
            };
        }) as PortManager.PortMetadata[],
    );
};

// export const updateNodeOutcomes = (nodeConfig: Node.Metadata, node: Node) => {};

/**
 *  是否可以新增边
 */
export const checkCanCreateEdge = (node: Node, outEdges: Edge<Edge.Properties>[]) => {
    const currentEdgeNames: string[] = (outEdges ?? [])
        .map((x) => x.getProp('name') ?? '')
        .filter((x) => x);
    const outcomes: string[] = (node.getProp('outcomes') ?? []).map((x: { toString: () => any }) =>
        x.toString(),
    );
    return outcomes.length > currentEdgeNames.length;
};

/**
 *  获取下一条变的名称
 */
export const getNextEdgeName = (node: Node, outEdges: Edge<Edge.Properties>[]) => {
    const currentEdgeNames: string[] = (outEdges ?? [])
        .map((x) => x.getProp('name') ?? '')
        .filter((x) => x);
    const outcomes: string[] = (node.getProp('outcomes') ?? []).map((x: { toString: () => any }) =>
        x.toString(),
    );
    //
    if (process.env.NODE_ENV === 'development') {
        console.debug('all: ', outcomes);
        console.debug('current: ', currentEdgeNames);
    }
    const result = outcomes.filter((x) => currentEdgeNames.indexOf(x) == -1);
    return result.length > 0 ? result[0].toString() : null;
};

/**
 *  获取输出边的差异
 */
export const compareOutputEdges = (
    node: Node,
    outcomes: string[],
    outEdges: Edge<Edge.Properties>[],
): string[] => {
    const currentEdgeNames: string[] = (outEdges ?? []).map((x) => x.getProp('name') ?? '');
    const beRemoved = currentEdgeNames.filter((x) => outcomes.indexOf(x) == -1);
    return beRemoved;
};

/**
 *  统一创建节点配置信息
 */
export const createNodeConfig = (config: Node.Metadata): Node.Metadata => {
    const { id, label, name, type, displayName } = config;
    const nodeId = id || uuid();
    const nodeLabel = label ?? type;
    return {
        shape: 'activity',
        ...config,
        id: nodeId,
        label: nodeLabel,
        displayName: displayName ?? nodeLabel,
        name: name ?? randString(type),
    };
};

/**
 *  统一创建边配置信息
 */
export const createEdgeConfig = (config: Edge.Metadata): Edge.Metadata => {
    const { id, label, name } = config;
    const edgeId = id ?? uuid();
    const edgeName = label ?? name ?? 'Done';

    return {
        shape: 'bpmn-edge',
        attrs: {
            line: {
                stroke: '#A2B1C3',
                strokeWidth: 2,
                targetMarker: {
                    name: 'block',
                    width: 12,
                    height: 8,
                },
            },
        },
        zIndex: 0,
        ...config,
        id: edgeId,
        name: edgeName,
        label: edgeName,
        outcome: edgeName,
    };
};

/**
 *  转换为Server数据
 */
export const conventToServerData = (data: IGraphData) => {
    console.debug('graph data: ', data);
    const edges: API.ActivityConnectionCreate[] = data.edges.map((item) => {
        return {
            sourceId: item.source.cell,
            targetId: item.target.cell,
            outcome: item.name ?? item.outcome ?? 'Done',
            attributes: {
                sourcePort: item.source.port,
                targetPort: item.target.port,
            },
        } as API.ActivityConnectionCreate;
    });

    const activities = data.nodes.map((item) => {
        let outcomes = item.outcomes ?? []; //edges.filter((x) => x.sourceId == item.id).map((x) => x.outcome);
        if (!outcomes.length) {
            outcomes = edges.filter((x) => x.sourceId == item.id).map((x) => x.outcome);
        }
        // if (!outcomes || !outcomes.length) {
        //     outcomes = ['Done'];
        // }
        return {
            ...(item.data ?? {}),
            activityId: item.id as string,
            type: item.type,
            name: item.name ?? randString(item.type),
            displayName: item.label ?? item.displayName,
            description: item.description,
            properties: item.properties,
            attributes: {
                x: Math.round(item.position?.x ?? 0),
                y: Math.round(item.position?.y ?? 0),
                outcomes: outcomes,
            },
        } as API.ActivityCreateOrUpdate;
    });

    return { activities: activities, connections: edges };
};

/**
 *  转换为画布数据
 */
export const conventToGraphData = async (
    activities: API.Activity[],
    connections: API.ActivityConnection[],
): Promise<IGraphData> => {
    const nodes: Node.Properties[] = [];

    activities.forEach(async (item) => {
        nodes.push(
            createNodeConfig({
                data: item,
                shape: 'activity', // TODO
                id: item.activityId,
                // @ts-ignore
                x: parseInt(item.attributes?.x ?? 0),
                // @ts-ignore
                y: parseInt(item.attributes?.y ?? 0),
                outcomes: item.attributes?.outcomes ?? [],
                label: item.displayName ?? item.type,
                //
                // data: {
                //     name: item.name,
                //     type: item.type,
                //     displayName: item.displayName,
                //     description: item.description,
                //     properties: item.properties,
                // },
                name: item.name,
                type: item.type,
                displayName: item.displayName,
                description: item.description,
                properties: item.properties,
            }) as Node.Properties,
        );
    });

    const edges: Edge.Metadata[] = [];

    connections.forEach((item) => {
        const sourceId = item.sourceId;
        const targetId = item.targetId;
        const attr = item.attributes ?? {};

        // check source
        if (
            sourceId &&
            nodes.findIndex((x) => x.id == sourceId) >= 0 &&
            targetId &&
            nodes.findIndex((x) => x.id == targetId) >= 0
        ) {
            edges.push(
                createEdgeConfig({
                    id: uuid(),
                    label: item.outcome,
                    outcome: item.outcome,
                    source: {
                        cell: sourceId,
                        port: attr?.sourcePort ?? 'bottom',
                    },
                    target: {
                        cell: targetId,
                        port: attr?.targetPort ?? 'top',
                    },
                } as Edge.Metadata),
            );
        } else {
            console.debug(`edge source id ${sourceId} or target id ${targetId} not found`);
        }
    });

    return { nodes: nodes, edges: edges };
};

export const getEditorScriptDefinitonsContent = async (id: string) => {
    const result = await getDesignerScriptTypeDefinition(id);
    return await result?.response?.text();
};

export const graphValidateMagnet = (graph: Graph, args: any) => {
    const { cell, view, magnet } = args;

    if (!magnet) return false;

    if (cell.isNode())
        if (!checkCanCreateEdge(cell, graph.getOutgoingEdges(cell) ?? [])) {
            message.error(`No more outcomes.`);
            return false;
        }

    //
    return true;
};

export const graphValidateConnection = (graph: Graph, args: any) => {
    const { targetMagnet } = args;

    if (!targetMagnet) {
        return false;
    }

    return true;
};

export const graphValidateEdge = (graph: Graph, args: any) => {
    const { edge, type } = args;

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
};

export const graphCreateEdge = (graph: Graph, args: any) => {
    const { sourceCell, sourceMagnet } = args;

    //
    if (sourceCell.isNode()) {
        const nextName = getNextEdgeName(sourceCell, graph.getOutgoingEdges(sourceCell) ?? []);
        if (nextName) {
            return new Shape.Edge(createEdgeConfig({ id: '', label: nextName }));
        } else {
            message.error(`No more outcomes.`);
            return null;
        }
    } else return null;
};

export const createGraph = (options?: any) => {
    return new Graph({
        container: document.getElementById('graphContainer')!,
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
            modifiers: 'ctrl',
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

        ...options,
    });
};

export const getGraphOptions = () => {
    return {};
};
