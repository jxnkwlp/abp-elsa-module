import MonacoEditor from '@/components/MonacoEditor';
import { ExpandOutlined } from '@ant-design/icons';
import { Modal, Spin, Tag } from 'antd';
import type * as monaco from 'monaco-editor';
import React, { useEffect, useState } from 'react';
import './monacor-editor-input.less';

type MonacorEditorInputProps = {
    id?: string;
    value?: string;
    onChange?: (value: string) => void;
    language?: string;
    // width?: number;
    height?: number;
    options?: monaco.editor.IStandaloneEditorConstructionOptions;
    showFullScreen?: false;
};

const MonacorEditorInput: React.FC<MonacorEditorInputProps> = (props) => {
    const { options, showFullScreen } = props;

    const [language, setLanguage] = React.useState('plaintext');
    const [value, setValue] = React.useState<string>();

    const [isFullscreen, setIsFullscreen] = React.useState(false);
    const [showEditor, setShowEditor] = React.useState(true);
    const [name] = useState(props.id);

    // const [name2] = useState(randString());

    const [currentSize] = React.useState<{ w?: number; h: number }>({ h: 150 });

    const handleValueChanged = (v: string) => {
        setValue(v);
        props?.onChange?.(v);
    };

    const handleToggleFullScreen = () => {
        if (isFullscreen) {
            setIsFullscreen(false);
        } else {
            setIsFullscreen(true);
        }
    };

    useEffect(() => {
        // update
        setValue(props?.value || '');
    }, [props.value]);

    useEffect(() => {
        // update
        console.debug('language => ', props.language);
        setLanguage(props.language ?? 'plaintext');
    }, [props.language]);

    useEffect(() => {
        console.debug('init editor with id:', name, ', language:', language);
    }, [name, language]);

    useEffect(() => {
        if (isFullscreen) {
            setShowEditor(false);
        } else {
            // HACK
            window.setTimeout(() => {
                setShowEditor(true);
            }, 500);
        }
    }, [isFullscreen]);

    return (
        <div
            className="monaco-editor-container"
            id={props.id}
            style={{ height: props.height ?? currentSize?.h }}
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
            {showEditor ? (
                <MonacoEditor
                    value={value}
                    path={name}
                    language={language}
                    minimap={false}
                    options={options}
                    onChange={(value) => {
                        handleValueChanged(value);
                    }}
                />
            ) : (
                <Spin spinning>
                    <div style={{ height: 140 }} />
                </Spin>
            )}
            {(showFullScreen ?? true) && (
                <div className="fullscreen-toggle">
                    <a onClick={() => handleToggleFullScreen()} title="Toggle fullscreen">
                        <ExpandOutlined />
                    </a>
                </div>
            )}
            <Modal
                title={
                    <>
                        Edit <Tag>{language}</Tag>
                    </>
                }
                open={isFullscreen}
                onCancel={() => setIsFullscreen(false)}
                onOk={() => setIsFullscreen(false)}
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
                        value={value}
                        path={name}
                        language={language}
                        minimap={true}
                        options={options}
                        onChange={(value) => {
                            handleValueChanged(value);
                        }}
                    />
                </div>
            </Modal>
        </div>
    );
};

export default MonacorEditorInput;
