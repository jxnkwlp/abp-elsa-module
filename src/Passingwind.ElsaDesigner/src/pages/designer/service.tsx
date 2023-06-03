import {
    designerCSharpLanguageCodeAnalysis,
    designerCSharpLanguageCodeFormatter,
    designerCSharpLanguageCompletionProvider,
    designerCSharpLanguageHoverProvider,
    designerCSharpLanguageSignatureProvider,
    getDesignerActivityTypes,
    getDesignerJavaScriptTypeDefinition,
} from '@/services/Designer';
import type { API } from '@/services/typings';
import { randString } from '@/services/utils';
import { CloseCircleOutlined, ForkOutlined, MailOutlined, UserOutlined } from '@ant-design/icons';
import type { Edge, Node } from '@antv/x6';
import { Graph, Shape } from '@antv/x6';
import type { PortManager } from '@antv/x6/lib/model/port';
import { uuid } from '@antv/x6/lib/util/string/uuid';
import { message } from 'antd';
import { formatMessage } from 'umi';
import { edgeDefaultConfig, edgeShapeName, nodeShapeName } from './node';
import type {
    IGraphData,
    NodePropertySyntax,
    NodePropertySyntaxNames,
    NodeStatus,
    NodeTypeGroup,
    NodeTypeProperty,
} from './type';

export const getNodeIconByType = (type: string) => {
    const dict = {
        CurrentUser: <UserOutlined />,
        Fork: <ForkOutlined />,
        SendEmail: <MailOutlined />,
        Fault: <CloseCircleOutlined />,
        // TODO
    };
    return dict[type] ?? null;
};

export const updateEdgeStatus = (edge: Edge, status: NodeStatus) => {
    if (status == 'failed') {
        edge.attr('line/strokeDasharray', '');
        edge.attr('line/stroke', '#f00');
    } else if (status == 'success') {
        edge.attr('line/strokeDasharray', '');
        edge.attr('line/stroke', '#52c41a');
    } else if (status == 'running') {
        edge.attr('line/strokeDasharray', 5);
        edge.attr('line/style/animation', 'running-line 30s infinite linear');
    } else if (status == 'inactive') {
        edge.attr('line/strokeDasharray', 5);
    }
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
            description: item.description,
            type: item.type,
            outcomes: item.outcomes ?? [],
        });
    });
    return nodeData;
};

export const getEditorLanguage = (syntax: string): string => {
    switch (syntax) {
        case 'Json':
            return 'json';
        case 'JavaScript':
            return 'javascript';
        case 'C#':
            return 'csharp';
        case 'Liquid':
            return 'liquid';
        case 'SQL':
            return 'sql';
        case 'Literal':
        default:
            return 'plaintext';
    }
};

export const propertyExpressionSyntaxeCompatibleKeys = {
    javaScript: 'JavaScript',
    literal: 'Literal',
    json: 'Json',
    liquid: 'Liquid',
    sQL: 'SQL',
    'c#': 'C#',
};

/**
 *  获取属性值语法配置
 */
export const getPropertySyntaxes = (property: NodeTypeProperty): NodePropertySyntax => {
    let syntaxes: string[] = (property.supportedSyntaxes ?? []).map((x) => {
        return x;
    });
    // set default 'Literal'
    let defaultSyntax: NodePropertySyntaxNames | string = property.defaultSyntax ?? 'Literal';
    let editorSyntax = '';
    //
    if (defaultSyntax && syntaxes.length == 0) {
        syntaxes.push(defaultSyntax);
    }

    if (defaultSyntax == 'Switch' && syntaxes.indexOf('Json') == -1) {
        syntaxes.push('Json');
    }

    if (property.options?.syntax) {
        // syntaxes = [property.options?.syntax];
        syntaxes = [];
        defaultSyntax = 'Literal';
        editorSyntax = property.options?.syntax;
    }

    if (
        property.uiHint != 'check-list' &&
        property.uiHint != 'radio-list' &&
        property.uiHint != 'code-editor' &&
        property.uiHint != 'dictionary' &&
        property.uiHint != 'dynamic-list' &&
        property.uiHint != 'multi-text' &&
        property.uiHint != 'switch-case-builder' &&
        syntaxes.indexOf('Literal') == -1
    ) {
        syntaxes = ['Literal', ...syntaxes];
    }

    // push 'Variable'
    // syntaxes.push('Variable');

    // push C#
    if (syntaxes.indexOf('JavaScript') >= 0 && syntaxes.indexOf('C#') == -1) {
        syntaxes.push('C#');
    }

    // end
    if (syntaxes.length > 0) defaultSyntax = syntaxes[0];

    return {
        supports: syntaxes,
        default: defaultSyntax,
        editor: editorSyntax,
    } as NodePropertySyntax;
};

// 更新节点输出端口
export const setNodeOutPorts = (node: Node<Node.Properties>) => {
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

export const getNodeOutcomes = (node: Node<Node.Properties>) => {
    return (node.getProp('outcomes') ?? []).map((x: { toString: () => any }) =>
        x.toString(),
    ) as string[];
};

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
    const outcomes: string[] = (node.getProp('outcomes') ?? []).map((x: { toString: () => any }) =>
        x.toString(),
    );
    const currentEdgeNames: string[] = (outEdges ?? [])
        .map((x) => x.getProp('name') ?? '')
        .filter((x) => x);
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
    _node: Node,
    outcomes: string[],
    outEdges: Edge<Edge.Properties>[],
): string[] => {
    const currentEdgeNames: string[] = (outEdges ?? []).map((x) => x.getProp('name') ?? '');
    const beRemoved = currentEdgeNames.filter((x) => outcomes.indexOf(x) == -1);
    return beRemoved;
};

/**
 *  统一创建节点配置信息
 *  节点属性只设置 id/label/name/icon/outcomes/type/x/y, 其他统一保存到 data
 */
export const createNodeConfig = (config: Node.Metadata): Node.Metadata => {
    const { id, label, name, type } = config;
    const nodeId = id || uuid();
    const nodeLabel = label ?? type;
    const nodeConfig = {
        id: nodeId,
        label: nodeLabel,
        name: name ?? randString(type),
        icon: getNodeIconByType(type),
        type: type,
    };
    return {
        // ...nodeDefaultConfig,
        shape: nodeShapeName,
        ...config,
        ...nodeConfig,
        data: {
            ...config.data,
            ...nodeConfig,
        },
    };
};

/**
 *  统一创建边配置信息
 *  属性只设置 id/name/outcome
 */
export const createEdgeConfig = (config: Edge.Metadata): Edge.Metadata => {
    const { id, label, name } = config;
    const edgeId = id ?? uuid();
    const edgeName = name ?? label ?? 'Done';

    return {
        ...edgeDefaultConfig,
        shape: edgeShapeName,
        ...config,
        id: edgeId,
        label: label,
        name: edgeName,
        outcome: edgeName,
        data: config,
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
            // sourceActivityId: item.source.cell,
            // targetActivityId: item.target.cell,
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
        const data = item.data;
        delete data.icon;
        //
        data.attributes = {
            ...item.position,
            outcomes: outcomes,
        };

        return {
            ...data,
            activityId: item.id as string,
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
                // shape: 'elsa-node', // TODO
                id: item.activityId,
                // @ts-ignore
                x: parseInt(item.attributes?.x ?? 0),
                // @ts-ignore
                y: parseInt(item.attributes?.y ?? 0),
                outcomes: item.attributes?.outcomes ?? [],
                label: item.displayName ?? item.type,
                name: item.name,
                type: item.type,
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

export const getJavascriptEditorDefinitonsContent = async (id: string) => {
    const result = await getDesignerJavaScriptTypeDefinition(id, { passError: true });
    return await result?.response?.text();
};

let cSharpEditorCodeAnalysisProviderRequestAbortController = new AbortController();
let cSharpEditorCompletionProviderRequestAbortController = new AbortController();
let cSharpEditorHoverProviderRequestAbortController = new AbortController();
let cSharpEditorSignatureProviderRequestAbortController = new AbortController();

export const getCSharpEditorLanguageProvider = async (
    id: string,
    provider: string,
    payload: any,
) => {
    if (provider == 'completion') {
        // cancel pre request if exists.
        if (
            cSharpEditorCompletionProviderRequestAbortController &&
            !cSharpEditorCompletionProviderRequestAbortController.signal.aborted
        ) {
            cSharpEditorCompletionProviderRequestAbortController.abort();
        }

        cSharpEditorCompletionProviderRequestAbortController = new AbortController();
        const { signal } = cSharpEditorCompletionProviderRequestAbortController;
        //
        const result = await designerCSharpLanguageCompletionProvider(id, payload, {
            signal: signal,
        });
        return result?.items ?? [];
    } else if (provider == 'hoverinfo') {
        // cancel pre request if exists.
        if (
            cSharpEditorHoverProviderRequestAbortController &&
            !cSharpEditorHoverProviderRequestAbortController.signal.aborted
        ) {
            cSharpEditorHoverProviderRequestAbortController.abort();
        }

        cSharpEditorHoverProviderRequestAbortController = new AbortController();
        const { signal } = cSharpEditorHoverProviderRequestAbortController;
        //
        const result = await designerCSharpLanguageHoverProvider(id, payload, {
            signal: signal,
        });
        return result ?? undefined;
    } else if (provider == 'signature') {
        // cancel pre request if exists.
        if (
            cSharpEditorSignatureProviderRequestAbortController &&
            !cSharpEditorSignatureProviderRequestAbortController.signal.aborted
        ) {
            cSharpEditorSignatureProviderRequestAbortController.abort();
        }
        cSharpEditorSignatureProviderRequestAbortController = new AbortController();
        const { signal } = cSharpEditorSignatureProviderRequestAbortController;
        const result = await designerCSharpLanguageSignatureProvider(id, payload, {
            signal: signal,
        });
        return result ?? undefined;
    } else if (provider == 'analysis') {
        // cancel pre request if exists.
        if (
            cSharpEditorCodeAnalysisProviderRequestAbortController &&
            !cSharpEditorCodeAnalysisProviderRequestAbortController.signal.aborted
        ) {
            cSharpEditorCodeAnalysisProviderRequestAbortController.abort();
        }

        cSharpEditorCodeAnalysisProviderRequestAbortController = new AbortController();
        const { signal } = cSharpEditorCodeAnalysisProviderRequestAbortController;
        //
        const result = await designerCSharpLanguageCodeAnalysis(id, payload, {
            signal: signal,
        });
        return result?.items ?? [];
    } else if (provider == 'format') {
        const result = await designerCSharpLanguageCodeFormatter(payload);
        return result;
    }

    return null;
};

export const graphValidateMagnet = (graph: Graph, args: any) => {
    const { cell, magnet } = args;

    if (!magnet) return false;

    if (cell.isNode())
        if (!checkCanCreateEdge(cell, graph.getOutgoingEdges(cell) ?? [])) {
            message.error(formatMessage({ id: 'page.designer.noMoreOutcomes' }));
            return false;
        }

    //
    return true;
};

export const graphValidateConnection = (_graph: Graph, args: any) => {
    const { targetMagnet } = args;

    if (!targetMagnet) {
        return false;
    }

    return true;
};

export const graphValidateEdge = (_graph: Graph, args: any) => {
    const { edge } = args;

    if (edge.getProp('shape') != 'elsa-edge') {
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
    const { sourceCell } = args;

    //
    if (sourceCell.isNode()) {
        const nextName = getNextEdgeName(sourceCell, graph.getOutgoingEdges(sourceCell) ?? []);
        if (nextName) {
            return new Shape.Edge(createEdgeConfig({ id: null, label: nextName }));
        } else {
            message.error(formatMessage({ id: 'page.designer.noMoreOutcomes' }));
            return null;
        }
    } else return null;
};

export const createDefaultGraph = (ele: HTMLElement | null, options: any = null) => {
    return new Graph({
        container: ele,
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
            enabled: false,
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
            multiple: true,
            rubberband: true,
            modifiers: 'ctrl',
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
            magnetAvailable: {
                name: 'stroke',
                args: {
                    padding: 4,
                    attrs: {
                        // strokeWidth: 1,
                        'stroke-width': 1,
                        stroke: '#47C769',
                        fill: '#ffffff',
                    },
                },
            },
            magnetAdsorbed: {
                name: 'stroke',
                args: {
                    attrs: {
                        // 'stroke-width': 2,
                        stroke: '#47C769',
                        fill: '#47C769',
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
