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
    options?: monaco.editor.IStandaloneEditorConstructionOptions;
    onMount?: (editor: monaco.editor.IStandaloneCodeEditor, monaco: Monaco) => void;
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
        onMount,
    } = props;

    const handleValueChanged = (value: string) => {
        onValueChange?.(value);
    };

    return (
        <Editor
            width={width}
            height={height}
            path={path}
            defaultLanguage={language}
            onChange={(v) => {
                handleValueChanged(v as string);
            }}
            value={value}
            options={{
                minimap: { enabled: minimap ?? true, autohide: true },
                wordWrap: 'bounded',
                wordWrapColumn: 1024,
                automaticLayout: true,
                autoIndent: 'full',
                tabSize: 2,
                autoClosingBrackets: 'languageDefined',
                foldingStrategy: 'auto',
                ...options,
            }}
            onMount={(e, m) => {
                onMount?.(e, m);
            }}
        />
    );
};

export default MonacoEditor;
