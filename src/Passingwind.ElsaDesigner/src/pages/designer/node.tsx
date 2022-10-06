import type { Cell } from '@antv/x6';
import { Edge, Graph, Node } from '@antv/x6';
import React from 'react';
import '@antv/x6-react-shape';
import Icon, {
    CheckCircleOutlined,
    CloseCircleOutlined,
    ReloadOutlined,
    SettingOutlined,
} from '@ant-design/icons';
import type { NodeData } from './type';

// 节点内容
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
        const { displayName = '', icon } = prop as NodeData;
        const { status = 'default' } = data as NodeData;

        return (
            <div className={`node ${status}`}>
                <span className="icon">{icon ? { ...icon } : <SettingOutlined />}</span>
                <span className="label">{displayName}</span>
                <span className="status">
                    {status == 'success' && <CheckCircleOutlined />}
                    {status == 'failed' && <CloseCircleOutlined />}
                    {status == 'running' && <ReloadOutlined spin />}
                </span>
            </div>
        );
    }
}

// 节点端口属性
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

// 节点默认配置
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

// 线默认配置
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
    // 默认设置
    Node.config(nodeDefaultConfig);
    Edge.config(edgeDefaultConfig);
    // 注册
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
