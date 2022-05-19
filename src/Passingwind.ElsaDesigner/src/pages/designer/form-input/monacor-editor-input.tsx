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
        if (props.getJavaScriptLibs) {
            const libs = await props.getJavaScriptLibs();
            if (libs?.length > 0) {
                monaco.languages.typescript.javascriptDefaults.setExtraLibs(libs ?? []);

                libs.forEach((x) => {
                    const oldModel = monaco.editor.getModel(monaco.Uri.parse(x.filePath));

                    if (oldModel) oldModel.dispose();

                    monaco.editor.createModel(
                        x.content,
                        'typescript',
                        monaco.Uri.parse(x.filePath),
                    );
                });
            }
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
                editorDidMount={(e) => {
                    e.setValue(value?.toString());
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
                        editorDidMount={(e) => {
                            e.setValue(value?.toString());
                        }}
                    />
                </div>
            </Modal>
        </div>
    );
};

export default MonacorEditorInput;
