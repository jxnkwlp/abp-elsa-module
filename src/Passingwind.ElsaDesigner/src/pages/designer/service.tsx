import { getDesignerActivityTypes, getDesignerScriptTypeDefinition } from '@/services/Designer';
import { randString } from '@/services/utils';
import type { Edge, Node } from '@antv/x6';
import type { PortManager } from '@antv/x6/lib/model/port';
import { uuid } from '@antv/x6/es/util/string/uuid';
import type { IGraphData, NodeTypeGroup, NodeTypeProperty } from './type';

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

export const getGraphOptions = () => {
    return {};
};

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

export const getPropertySyntaxes = (property: NodeTypeProperty) => {
    let syntaxes = property.supportedSyntaxes ?? [];
    let defaultSyntax: string = property.defaultSyntax ?? '';
    if (defaultSyntax && syntaxes.length == 0) {
        syntaxes.push(defaultSyntax);
    } else if (!defaultSyntax && syntaxes.length == 0) {
        syntaxes.push('Literal');
    }
    if (property.uiHint != 'switch-case-builder' && syntaxes.indexOf('Literal') == -1) {
        syntaxes = ['Literal', ...syntaxes];
    }
    // if (!defaultSyntax && syntaxes.length > 0) {
    //     defaultSyntax = syntaxes[0];
    // }
    if (property.options?.syntax) {
        syntaxes = [property.options?.syntax ?? 'Literal'];
    }
    // end
    if (syntaxes.length == 0) {
        syntaxes.push('Literal');
    }
    if (syntaxes.length > 0) defaultSyntax = syntaxes[0];
    //

    return { defaultSyntax, syntaxes };
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
    console.log(buttomPorts);
    node.removePorts(buttomPorts, { silent: true });
    // add out port
    const outcomes: string[] = node.getProp('outcomes') ?? [];
    console.log(outcomes);
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
    // console.log('count: ', outcomes.length, outEdges.length);
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
        console.log('current: ', currentEdgeNames);
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
    // console.log('remove :', beRemoved);
    return beRemoved;
};

/**
 *  统一创建节点配置信息
 */
export const createNodeConfig = (config: any) => {
    const { id, label, name, type, displayName } = config;
    const nodeId = id || uuid();
    const nodeLabel = label ?? type;
    const node = {
        shape: 'activity',
        ...config,
        id: nodeId,
        label: nodeLabel,
        displayName: displayName ?? nodeLabel,
        name: name ?? randString(type),
    };

    return node;
};

/**
 *  统一创建边配置信息
 */
export const createEdgeConfig = (config: any) => {
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
    console.log(data);
    const edges: API.ActivityConnectionCreate[] = data.edges.map((item: any) => {
        return {
            sourceId: item.source.cell,
            targetId: item.target.cell,
            outcome: item.name ?? item.outcome ?? 'Done',
        } as API.ActivityConnectionCreate;
    });

    const activities = data.nodes.map((item: Node.Metadata) => {
        let outcomes = item.outcomes ?? []; //edges.filter((x) => x.sourceId == item.id).map((x) => x.outcome);
        if (!outcomes.length) {
            outcomes = edges.filter((x) => x.sourceId == item.id).map((x) => x.outcome);
        }
        // if (!outcomes || !outcomes.length) {
        //     outcomes = ['Done'];
        // }
        return {
            activityId: item.id as string,
            type: item.type,
            name: item.name ?? randString(item.type),
            displayName: item.label ?? item.displayName,
            description: item.description,
            properties: item.properties,
            arrtibutes: {
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
    // console.log('convert input: ', activities, connections);

    const nodes: Node.Metadata = [];

    activities.forEach(async (item) => {
        const n = createNodeConfig({
            shape: 'activity',
            id: item.activityId?.toString(),
            // @ts-ignore
            x: parseInt(item.arrtibutes?.x ?? 0),
            // @ts-ignore
            y: parseInt(item.arrtibutes?.y ?? 0),
            outcomes: item.arrtibutes?.outcomes ?? [],
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
        }) as Node.Metadata;
        nodes.push(n);
    });

    const edges = connections.map((item) => {
        const sourceId = item.sourceId;
        const targetId = item.targetId;

        return createEdgeConfig({
            id: '', // be generated
            label: item.outcome,
            outcome: item.outcome,
            source: {
                cell: sourceId,
                port: item.outcome ?? 'Done',
            },
            target: {
                cell: targetId,
                port: 'In',
            },
        });
    });

    return { nodes: nodes, edges: edges };
};

export const getEditorScriptDefinitonsContent = async (id: string) => {
    const result = await getDesignerScriptTypeDefinition(id);
    return await result?.response?.text();
};
