import { ExpandOutlined } from '@ant-design/icons';
import { Modal } from 'antd';
import * as monaco from 'monaco-editor/esm/vs/editor/editor.api';
import React, { useEffect } from 'react';
import MonacoEditor from 'react-monaco-editor';
import './monacor-editor-input.less';

type MonacorEditorInputProps = {
    id?: string;
    value?: string;
    onChange?: (value: string) => void;
    language?: string;
    width?: number;
    height?: number;
    options?: monaco.editor.IStandaloneEditorConstructionOptions;
    getJavaScriptLibs?: () => Promise<{ content: string; filePath: string }[]>;
    getCSharpLanguageProviderRequest?: (
        provider: 'completion' | 'hoverinfo',
        payload: any,
    ) =>
        | Promise<any>
        | Promise<{ suggestion: string; description?: string }[]>
        | Promise<{ description?: string }>;
};

const MonacorEditorInput: React.FC<MonacorEditorInputProps> = (props) => {
    const ref = React.createRef<MonacoEditor>();
    const ref2 = React.createRef<MonacoEditor>();

    const [value, setValue] = React.useState(props.value || '');
    const [isFullscreen, setIsFullscreen] = React.useState(false);

    const [currentSize, setCurrentSize] = React.useState<{ w?: number; h: number }>();

    const handleValueChanged = (v: string) => {
        props?.onChange?.(v);
        setValue(v);
    };

    const handleToggleFullScreen = () => {
        if (isFullscreen) {
            setIsFullscreen(false);
        } else {
            setIsFullscreen(true);
            ref2.current?.editor?.setValue(value);
        }
    };

    const updateEditorScriptExtraLibs = async () => {
        if (!props?.getJavaScriptLibs) return;

        const libs = await props.getJavaScriptLibs();
        if (libs?.length > 0) {
            monaco.languages.typescript.javascriptDefaults.setExtraLibs(libs ?? []);

            libs.forEach((x) => {
                const oldModel = monaco.editor.getModel(monaco.Uri.parse(x.filePath));

                if (oldModel) oldModel.dispose();

                monaco.editor.createModel(x.content, 'typescript', monaco.Uri.parse(x.filePath));
            });
        }
    };

    const registerCSharpLanguageProvider = (monacoInstance: typeof monaco) => {
        if (!props?.getCSharpLanguageProviderRequest) return;

        // // monaco.languages.registerCompletionItemProvider;
        // console.log(1);
        // monacoInstance.languages.registerCompletionItemProvider('csharp', {
        //     triggerCharacters: [' ', '.'],
        //     provideCompletionItems: async (model, position) => {
        //         console.log(model);
        //         const result = await props?.getCSharpLanguageProviderRequest?.('completion', {
        //             code: model.getValue(),
        //             position: model.getOffsetAt(position),
        //         });

        //         if (result) {
        //             const suggestions = result.map((x) => {
        //                 return {
        //                     label: {
        //                         label: x.suggestion,
        //                         description: x.description,
        //                     },
        //                     kind: monaco.languages.CompletionItemKind.Function,
        //                     insertText: x.Suggestion,
        //                 };
        //             });
        //             return { suggestions: suggestions };
        //         } else {
        //             return { suggestions: [] };
        //         }
        //     },
        // });

        // monacoInstance.languages.registerHoverProvider('csharp', {
        //     provideHover: async (model, position, token) => {
        //         const result = await props?.getCSharpLanguageProviderRequest?.('hoverinfo', {
        //             code: model.getValue(),
        //             position: model.getOffsetAt(position),
        //         });

        //         return { contents: [] };
        //     },
        // });
    };

    const onEditorMount = (
        editor: monaco.editor.IStandaloneCodeEditor,
        monacoInstance: typeof monaco,
    ) => {
        editor.setValue(value?.toString());

        if (props.language == 'csharp') {
            registerCSharpLanguageProvider(monacoInstance);
        }
    };

    useEffect(() => {
        // setOriginSize({ w: props.width, h: props.height ?? 150 });
        setCurrentSize({ w: props.width, h: props.height ?? 150 });
    }, [0]);

    useEffect(() => {
        if (props.language == 'javascript') {
            updateEditorScriptExtraLibs();
        }
        //  else if (props.language == 'csharp') {
        // registerCSharpLanguageProvider();
        // }
    }, [props.language]);

    return (
        <div
            className="monaco-editor-container"
            id={props.id}
            style={{ height: currentSize?.h }}
            onKeyDown={(e) => {
                // keyCode: 27
                if (e.code == 'Escape') {
                    if (isFullscreen) {
                        handleToggleFullScreen();
                    }
                    e.preventDefault();
                    e.stopPropagation();
                }
            }}
        >
            <MonacoEditor
                ref={ref}
                language={props.language}
                onChange={(v) => {
                    handleValueChanged(v);
                }}
                options={{
                    minimap: { enabled: isFullscreen },
                    wordWrap: 'bounded',
                    wordWrapColumn: 1024,
                    automaticLayout: true,
                    autoIndent: 'full',
                    tabSize: 2,
                    autoClosingBrackets: 'languageDefined',
                    foldingStrategy: 'auto',
                    ...props?.options,
                }}
                editorDidMount={(e, m) => {
                    onEditorMount(e, m);
                }}
            />
            <div className="fullscreen-toggle">
                <a onClick={() => handleToggleFullScreen()} title="Toggle fullscreen">
                    <ExpandOutlined />
                </a>
            </div>

            <Modal
                title="Edit"
                visible={isFullscreen}
                onCancel={() => setIsFullscreen(false)}
                onOk={() => {
                    setIsFullscreen(false);
                }}
                width="96%"
                style={{ top: 20 }}
                destroyOnClose
                maskClosable={false}
                closable={false}
                bodyStyle={{ padding: 0 }}
            >
                <div
                    style={{ height: document.body.clientHeight * 0.8 + 'px' }}
                    onKeyDown={(e) => {
                        if (e.code == 'Escape') {
                            e.preventDefault();
                            e.stopPropagation();
                        } else if (e.ctrlKey && e.code == 'KeyS') {
                            // setIsFullscreen(false);
                            // e.preventDefault();
                            // e.stopPropagation();
                        }
                    }}
                >
                    <MonacoEditor
                        ref={ref2}
                        language={props.language}
                        onChange={(v) => {
                            ref.current?.editor?.setValue(v);
                        }}
                        options={{
                            minimap: { enabled: isFullscreen },
                            wordWrap: 'bounded',
                            wordWrapColumn: 1024,
                            automaticLayout: true,
                            autoIndent: 'full',
                            tabSize: 2,
                            autoClosingBrackets: 'languageDefined',
                            foldingStrategy: 'auto',
                            ...props?.options,
                        }}
                        editorDidMount={(e, m) => {
                            onEditorMount(e, m);
                        }}
                    />
                </div>
            </Modal>
        </div>
    );
};

export default MonacorEditorInput;
