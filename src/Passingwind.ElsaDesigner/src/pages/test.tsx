import React, { useEffect } from 'react';
import { PageContainer } from '@ant-design/pro-layout';
import { Card, Alert, Typography, Form } from 'antd';
import { useIntl, FormattedMessage } from 'umi';
import MonacorEditorInput from './designer/form-input/monacor-editor-input';
import { Graph } from '@antv/x6';
import { registerNodeTypes } from './designer/node';
import MonacoEditor from 'react-monaco-editor';
import metadata from 'monaco-editor/esm/metadata';
import * as monaco from 'monaco-editor/esm/vs/editor/editor.api';

const Test: React.FC = () => {
    const intl = useIntl();
    const ref = React.createRef<MonacoEditor>();
    useEffect(() => {
        // //
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

        console.log(metadata.languages);
    }, []);

    return (
        <PageContainer>
            <Card>
                {/* <Form initialValues={{ v: new Date().getTime().toString() }}>
                    <Form.Item name="v">
                    <MonacorEditorInput height={500} language="csharp" />
                    </Form.Item>
                </Form> */}
                {/* <div id="graphContainer" style={{ height: '500px' }}></div> */}
                <MonacoEditor
                    ref={ref}
                    language="csharp"
                    height={500}
                    options={{
                        wordWrap: 'bounded',
                        wordWrapColumn: 1024,
                        automaticLayout: true,
                        autoIndent: 'full',
                        tabSize: 2,
                        autoClosingBrackets: 'languageDefined',
                        foldingStrategy: 'auto',
                        suggestOnTriggerCharacters: true,
                    }}
                    editorDidMount={(editor, monaco) => {
                        // monaco.languages.typescript.typescriptDefaults.addExtraLib();
                        // monaco.editor.createModel(
                        //     'function add(int x,int y);',
                        //     'csharp',
                        //     monaco.Uri.parse('http://aaa.com/1.cs'),
                        // );
                        monaco.languages.registerCompletionItemProvider('csharp', {
                            triggerCharacters: ['.', ''],
                            provideCompletionItems: async (model, pository, contex, token) => {
                                let suggestions = [];

                                suggestions.push({
                                    label: 'add',
                                    kind: monaco.languages.CompletionItemKind.Function,
                                    insertText: 'add',
                                    documentation: '',
                                });

                                return { suggestions: suggestions };
                            },
                        });
                    }}
                    editorWillUnmount={(e) => {}}
                />
            </Card>
        </PageContainer>
    );
};

export default Test;
