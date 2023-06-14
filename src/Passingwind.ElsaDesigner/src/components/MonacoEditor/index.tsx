import Editor from '@monaco-editor/react';
import type * as monaco from 'monaco-editor';
import { useEffect } from 'react';

// monaco
type Monaco = typeof monaco;

type MonacoEditorProps = {
    path?: string;
    value?: string;
    onChange?: (value: string) => void;
    language?: string;
    minimap?: boolean;
    width?: number;
    height?: number;
    border?: boolean;
    options?: monaco.editor.IStandaloneEditorConstructionOptions;
    onMount?: (editor: monaco.editor.IStandaloneCodeEditor, monaco: Monaco) => void;
    onValidate?: (markers: monaco.editor.IMarker[]) => void;
};

const MonacoEditor: React.FC<MonacoEditorProps> = (props) => {
    const {
        width,
        height,
        path,
        value,
        language,
        minimap,
        onChange: onValueChange,
        options,
        border,
        onMount,
        onValidate,
    } = props;

    const handleValueChanged = (value: string) => {
        onValueChange?.(value);
    };

    const handleOnValidate = (markers: monaco.editor.IMarker[]) => {
        onValidate?.(markers);
    };

    return (
        <div className={border ? 'monaco-editor-container' : ''} style={{ height: '100%' }}>
            <Editor
                width={width}
                height={height}
                path={path}
                defaultLanguage={language}
                value={value}
                options={{
                    minimap: { enabled: minimap ?? true, autohide: true },
                    wordWrap: 'bounded',
                    wordWrapColumn: 1024,
                    automaticLayout: true,
                    autoIndent: 'full',
                    autoClosingBrackets: 'languageDefined',
                    foldingStrategy: 'auto',
                    formatOnPaste: true,
                    foldingImportsByDefault: true,
                    ...options,
                }}
                onChange={(v) => {
                    handleValueChanged(v as string);
                }}
                onMount={(e, m) => {
                    console.debug('editor mount: ', e.getModel()?.id);
                    onMount?.(e, m);
                }}
                onValidate={handleOnValidate}
            />
        </div>
    );
};

export default MonacoEditor;
