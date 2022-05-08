import { Edge, Graph } from '@antv/x6';

export const nodeAttribites = {
    activity: {
        normal: {
            body: {
                rx: 6,
                ry: 6,
                stroke: '#91d5ff',
                fill: '#e6f7ff',
                strokeWidth: 1,
            },
            // img: {
            //     x: 6,
            //     y: 6,
            //     width: 16,
            //     height: 16,
            //     'xlink:href':
            //         'https://gw.alipayobjects.com/mdn/rms_43231b/afts/img/A*pwLpRr7QPGwAAAAAAAAAAAAAARQnAQ',
            // },
            label: {
                fontSize: 12,
                fill: '#262626',
            },
        },
    },
};

export const nodePortAttr = {
    circle: {
        r: 6,
        magnet: true,
        stroke: '#5F95FF',
        strokeWidth: 1,
        fill: '#fff',
        style: {
            visibility: 'hidden',
            transition: 'all 1s',
        },
    },
};

export const nodePorts = {
    groups: {
        top: {
            position: 'top',
            attrs: nodePortAttr,
            tooltip: '',
            label: {
                position: {
                    name: 'top',
                },
            },
        },
        bottom: {
            position: 'bottom',
            attrs: nodePortAttr,
            tooltip: '',
            label: {
                position: {
                    name: 'bottom',
                },
            },
        },
        left: {
            position: 'left',
            attrs: nodePortAttr,
            tooltip: '',
            label: {
                position: {
                    name: 'left',
                },
            },
        },
        right: {
            position: 'right',
            attrs: nodePortAttr,
            tooltip: '',
            label: {
                position: {
                    name: 'right',
                },
            },
        },
    },
    items: [
        { id: 'top', group: 'top' },
        { id: 'bottom', group: 'bottom' },
        { id: 'left', group: 'left' },
        { id: 'right', group: 'right' },
    ],
};

export const flowNodes = {
    event: {
        inherit: 'circle',
        width: 60,
        height: 60,
        ports: nodePorts,
        attrs: {
            body: {
                strokeWidth: 2,
                stroke: '#5F95FF',
                fill: '#FFF',
            },
        },
    },
    activity: {
        inherit: 'rect',
        width: 100,
        height: 60,
        ports: nodePorts,
        markup: [
            {
                tagName: 'rect',
                selector: 'body',
            },
            {
                tagName: 'image',
                selector: 'img',
            },
            {
                tagName: 'text',
                selector: 'label',
            },
        ],
        attrs: nodeAttribites.activity.normal,
    },
    gateway: {
        inherit: 'polygon',
        width: 60,
        height: 60,
        ports: nodePorts,
        attrs: {
            body: {
                refPoints: '0,10 10,0 20,10 10,20',
                strokeWidth: 2,
                stroke: '#5F95FF',
                fill: '#EFF4FF',
            },
            label: {
                text: '+',
                fontSize: 40,
                fill: '#5F95FF',
            },
        },
    },
};

export const registerNodeTypes = () => {
    Graph.registerNode('event', flowNodes.event, true);

    Graph.registerNode('activity', flowNodes.activity, true);

    Graph.registerNode('gateway', flowNodes.gateway, true);

    Graph.registerEdge(
        'bpmn-edge',
        {
            inherit: 'edge',
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
        },
        true,
    );

    // Edge.define('bpmn-edge', {});

    // 默认设置
    Edge.config({
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
    });
};
