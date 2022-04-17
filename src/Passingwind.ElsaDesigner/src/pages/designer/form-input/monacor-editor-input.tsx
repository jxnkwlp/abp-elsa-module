import React, { useEffect } from 'react';
import MonacoEditor from 'react-monaco-editor';
import * as monaco from 'monaco-editor/esm/vs/editor/editor.api';

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
        <div className="monaco-editor-container" id={props.id}>
            <MonacoEditor
                ref={ref}
                width={props.width}
                height={props.height}
                language={props.language}
                defaultValue={props.value}
                onChange={(v) => {
                    props?.onChange?.(v);
                }}
                options={{
                    minimap: { enabled: false },
                    wordWrap: 'bounded',
                    automaticLayout: true,
                    autoIndent: 'full',
                    ...props?.options,
                }}
                editorDidMount={(e) => {
                    e.setValue(props.value ?? '');
                }}
            />
        </div>
    );
};

export default MonacorEditorInput;
