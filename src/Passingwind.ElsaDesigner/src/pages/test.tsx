import MonacoEditor from '@/components/MonacoEditor';
import { getAbpApplicationConfiguration } from '@/services/AbpApplicationConfiguration';
import { PageContainer, ProForm, ProFormText } from '@ant-design/pro-components';
import Editor from '@monaco-editor/react';
import { Button, Card } from 'antd';
import { useEffect, useState } from 'react';
import { useIntl } from 'umi';
import MonacorEditorInput from './designer/form-input/monacor-editor-input';

const Test: React.FC = () => {
    const intl = useIntl();
    const [value, setValue] = useState('');

    const [form] = ProForm.useForm();

    useEffect(() => {
        //
        // registerNodeTypes();
        // //
        // const graph = new Graph({
        //     // container: containerEleRef.current,
        //     container: document.getElementById('graphContainer')!,
        //     // height: props.height ?? 600,
        //     // width: '100vm',
        //     autoResize: true,
        //     grid: {
        //         size: 10,
        //         visible: true,
        //     },
        // });
        // const data = {
        //     // 节点
        //     nodes: [
        //         {
        //             shape: 'elsa-node',
        //             id: 'node1', // String，可选，节点的唯一标识
        //             x: 40, // Number，必选，节点位置的 x 值
        //             y: 40, // Number，必选，节点位置的 y 值
        //             width: 80, // Number，可选，节点大小的 width 值
        //             height: 40, // Number，可选，节点大小的 height 值
        //             label: 'hello', // String，节点标签
        //         },
        //         {
        //             id: 'node2', // String，节点的唯一标识
        //             x: 160, // Number，必选，节点位置的 x 值
        //             y: 180, // Number，必选，节点位置的 y 值
        //             width: 80, // Number，可选，节点大小的 width 值
        //             height: 40, // Number，可选，节点大小的 height 值
        //             label: 'world', // String，节点标签
        //         },
        //     ],
        //     // 边
        //     edges: [
        //         {
        //             source: 'node1', // String，必须，起始节点 id
        //             target: 'node2', // String，必须，目标节点 id
        //         },
        //     ],
        // };
        // graph.fromJSON(data);
    }, []);

    let controller = new AbortController();
    const { signal } = controller;

    signal.addEventListener('abort', () => {
        console.log('aborted!');
    });

    return (
        <PageContainer>
            <Card>
                {/* <Button
                    onClick={() => {
                        controller = new AbortController();
                        const { signal } = controller;
                        getAbpApplicationConfiguration({
                            signal: signal,
                        });
                    }}
                >
                    AAA
                </Button>

                <Button
                    onClick={() => {
                        controller.abort();
                    }}
                >
                    BBB
                </Button> */}

                <Button
                    onClick={() => {
                        // setValue(new Date().getTime().toString());
                        form.setFieldsValue({
                            a: '1' + new Date().getTime().toString(),
                            b: '2' + new Date().getTime().toString(),
                        });
                    }}
                >
                    BBB
                </Button>
                {/* <div style={{ height: 100 }}>
                    <MonacorEditor value={value} />
                </div> */}
                <ProForm
                    form={form}
                    onFinish={(v) => {
                        console.log(v);
                    }}
                >
                    <ProForm.Item name="a">
                        <MonacorEditorInput />
                        {/* <MonacoEditor height={100} /> */}
                    </ProForm.Item>
                    <ProFormText name="b" />
                </ProForm>
            </Card>
        </PageContainer>
    );
};

export default Test;
