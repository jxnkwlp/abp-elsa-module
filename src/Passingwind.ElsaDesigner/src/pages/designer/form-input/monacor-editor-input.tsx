import { ExpandOutlined } from '@ant-design/icons';
import * as monaco from 'monaco-editor/esm/vs/editor/editor.api';
import React, { useEffect } from 'react';
import MonacoEditor from 'react-monaco-editor';

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

    const [isFullscreen, setIsFullscreen] = React.useState(false);

    const [originSize, setOriginSize] = React.useState<{ w?: number; h: number }>();
    const [currentSize, setCurrentSize] = React.useState<{ w?: number; h: number }>();

    const handleToggleFullScreen = () => {
        if (isFullscreen) {
            setIsFullscreen(false);
            setCurrentSize(originSize);
        } else {
            setIsFullscreen(true);
            setCurrentSize({ w: document.body.clientWidth, h: document.body.clientHeight });
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
        // console.log(props);
        ref.current?.editor?.setValue(props.value ?? '');
    }, [props, ref]);

    useEffect(() => {
        setOriginSize({ w: props.width, h: props.height ?? 150 });
        setCurrentSize({ w: props.width, h: props.height ?? 150 });
    }, [0]);

    useEffect(() => {
        if (props.language == 'javascript') {
            updateEditorScriptExtraLibs();
            // const libs = (props.javaScriptLibs ?? []).map((x) => {
            //     return { content: x.content, filePath: x.filePath };
            // });
            // monaco.languages.registerCompletionItemProvider(props.language ?? 'typescript', {
            //     triggerCharacters: [':'],
            //     provideCompletionItems: (model, position) => {
            //         const word = model.getWordUntilPosition(position);
            //         const range = {
            //             startLineNumber: position.lineNumber,
            //             endLineNumber: position.lineNumber,
            //             startColumn: word.startColumn,
            //             endColumn: word.endColumn,
            //         };
            //         return {
            //             suggestions: [
            //                 {
            //                     label: '"lodash"',
            //                     kind: monaco.languages.CompletionItemKind.Function,
            //                     documentation: 'The Lodash library exported as Node.js modules.',
            //                     insertText: '"lodash": "*"',
            //                     range: range,
            //                 },
            //             ],
            //         };
            //     },
            // });
        }
    }, [props.language]);

    return (
        <div
            className={
                isFullscreen ? 'monaco-editor-container fullscreen' : 'monaco-editor-container'
            }
            id={props.id}
            style={{ width: currentSize?.w, height: currentSize?.h }}
        >
            <MonacoEditor
                ref={ref}
                language={props.language}
                defaultValue={props.value}
                onChange={(v) => {
                    props?.onChange?.(v);
                }}
                options={{
                    minimap: { enabled: isFullscreen },
                    wordWrap: 'bounded',
                    wordWrapColumn: 1024,
                    automaticLayout: true,
                    autoIndent: 'full',
                    ...props?.options,
                }}
                editorDidMount={(e) => {
                    e.setValue(props.value ?? '');
                }}
            />
            <div className="fullscreen-toggle">
                <a onClick={() => handleToggleFullScreen()} title="Toggle fullscreen">
                    <ExpandOutlined />
                </a>
            </div>
        </div>
    );
};

export default MonacorEditorInput;
