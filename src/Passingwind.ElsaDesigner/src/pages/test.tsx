import React, { useEffect } from 'react';
import { PageContainer } from '@ant-design/pro-layout';
import { Card, Alert, Typography, Form } from 'antd';
import { useIntl, FormattedMessage } from 'umi';
import MonacorEditorInput from './designer/form-input/monacor-editor-input';
import { Graph } from '@antv/x6';
import { registerNodeTypes } from './designer/node';

const Test: React.FC = () => {
    const intl = useIntl();
    useEffect(() => {
        //
        registerNodeTypes();
        //
        const graph = new Graph({
            // container: containerEleRef.current,
            container: document.getElementById('graphContainer')!,
            // height: props.height ?? 600,
            // width: '100vm',
            autoResize: true,
            grid: {
                size: 10,
                visible: true,
            },
        });

        const data = {
            // 节点
            nodes: [
                {
                    shape: 'elsa-node',
                    id: 'node1', // String，可选，节点的唯一标识
                    x: 40, // Number，必选，节点位置的 x 值
                    y: 40, // Number，必选，节点位置的 y 值
                    width: 80, // Number，可选，节点大小的 width 值
                    height: 40, // Number，可选，节点大小的 height 值
                    label: 'hello', // String，节点标签
                },
                {
                    id: 'node2', // String，节点的唯一标识
                    x: 160, // Number，必选，节点位置的 x 值
                    y: 180, // Number，必选，节点位置的 y 值
                    width: 80, // Number，可选，节点大小的 width 值
                    height: 40, // Number，可选，节点大小的 height 值
                    label: 'world', // String，节点标签
                },
            ],
            // 边
            edges: [
                {
                    source: 'node1', // String，必须，起始节点 id
                    target: 'node2', // String，必须，目标节点 id
                },
            ],
        };

        graph.fromJSON(data);
    }, []);

    return (
        <PageContainer>
            <Card>
                {/* <Form initialValues={{ v: new Date().getTime().toString() }}>
                    <Form.Item name="v">
                        <MonacorEditorInput height={500} />
                    </Form.Item>
                </Form> */}
                <div id="graphContainer" style={{ height: '500px' }}></div>
            </Card>
        </PageContainer>
    );
};

export default Test;
