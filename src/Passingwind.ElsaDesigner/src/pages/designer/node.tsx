import type { Cell } from '@antv/x6';
import { Edge, Graph, Node } from '@antv/x6';
import React from 'react';
import '@antv/x6-react-shape';
import Icon, {
    ApiOutlined,
    CheckCircleOutlined,
    CloseCircleOutlined,
    ReloadOutlined,
    SettingOutlined,
} from '@ant-design/icons';
import type { NodeData } from './type';
import { Tooltip } from 'antd';

// Custome node
export class ElsaNode extends React.Component<{ node?: Node }> {
    shouldComponentUpdate() {
        const { node } = this.props;
        if (node) {
            if (node.hasChanged('data')) {
                return true;
            }
        }
        return false;
    }

    render() {
        const { node } = this.props;
        const data = node?.getData() as NodeData;
        const prop = node?.getProp();
        const { label = '', icon } = prop as NodeData;
        const { status = 'default' } = data as NodeData;

        return (
            <div className={`node ${status}`}>
                <span className="icon">{icon ? { ...icon } : <ApiOutlined />}</span>
                <span className="label">{label}</span>
                <span className="status">
                    {status == 'success' && <CheckCircleOutlined />}
                    {status == 'failed' && <CloseCircleOutlined />}
                    {status == 'running' && <ReloadOutlined spin />}
                </span>
            </div>
        );
    }
}

// Node port attributes
export const nodePortAttr = {
    circle: {
        r: 4,
        magnet: true,
        stroke: '#C2C8D5',
        strokeWidth: 1,
        fill: '#fff',
        style: {
            visibility: 'hidden',
            transition: 'all 1s',
        },
    },
};

// Default config for node
export const nodeDefaultConfig: Cell.Config = {
    inherit: 'react-shape',
    width: 180,
    height: 36,
    component: <ElsaNode />,
    ports: {
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
    },
};

// Default config for edge
export const edgeDefaultConfig: Cell.Config = {
    zIndex: 0,
    attrs: {
        line: {
            stroke: '#C2C8D5',
            strokeWidth: 1,
            targetMarker: {
                name: 'block',
                width: 12,
                height: 8,
            },
        },
    },
};

export const nodeShapeName = 'elsa-node';
export const edgeShapeName = 'elsa-edge';

export const registerNodeTypes = () => {
    // default
    Node.config(nodeDefaultConfig);
    Edge.config(edgeDefaultConfig);
    // register node & edge
    Graph.registerNode(
        nodeShapeName,
        {
            inherit: 'react-shape',
            ...nodeDefaultConfig,
        },
        true,
    );
    Graph.registerEdge(
        edgeShapeName,
        {
            inherit: 'edge',
            ...edgeDefaultConfig,
        },
        true,
    );
};
