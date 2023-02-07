import MonacoEditor from '@/components/MonacoEditor';
import { ExpandOutlined } from '@ant-design/icons';
import type * as monaco from 'monaco-editor';
import { Modal, Tag } from 'antd';
import React, { useEffect } from 'react';
import './monacor-editor-input.less';

type MonacorEditorInputProps = {
    id?: string;
    value?: string;
    onChange?: (value: string) => void;
    language?: string;
    width?: number;
    height?: number;
    options?: monaco.editor.IStandaloneEditorConstructionOptions;
};

const MonacorEditorInput: React.FC<MonacorEditorInputProps> = (props) => {
    const { options } = props;

    const [language] = React.useState(props.language || 'plaintext');
    const [value, setValue] = React.useState(props.value || '');
    const [isFullscreen, setIsFullscreen] = React.useState(false);

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
                value={value}
                language={language}
                minimap={false}
                options={options}
                onChange={(value) => {
                    handleValueChanged(value);
                }}
            />
            <div className="fullscreen-toggle">
                <a onClick={() => handleToggleFullScreen()} title="Toggle fullscreen">
                    <ExpandOutlined />
                </a>
            </div>

            <Modal
                title={
                    <>
                        Edit <Tag>{language}</Tag>
                    </>
                }
                open={isFullscreen}
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
                        value={value}
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
