import { getWorkflowStorageProviders } from '@/services/Workflow';
import ProForm, {
    ProFormCheckbox,
    ProFormDependency,
    ProFormGroup,
    ProFormSelect,
    ProFormSwitch,
    ProFormText,
    ProFormTextArea,
} from '@ant-design/pro-form';
import { Col, Divider, Row, Tabs } from 'antd';
import type { NamePath } from 'antd/lib/form/interface';
import React, { useEffect, useMemo } from 'react';
import CaseEditorInput from './form-input/case-editor-builder-input';
import MonacorEditorInput from './form-input/monacor-editor-input';
import {
    getEditorLanguage,
    getEditorScriptDefinitonsContent,
    getPropertySyntaxes,
} from './service';
import type { NodeTypeProperty } from './type';

const defaultInputSpan = 20;
const defaultSyntaxSpan = 4;

type NodePropFormProps = {
    workflowDefinitionId?: string;
    properties: NodeTypeProperty[];
    // initialValues: object | undefined;
    setFieldsValue: (value: any) => void;
    setFieldValue: (key: NamePath, value: any) => void;
    getFieldValue: (key: NamePath) => any;
};

const NodePropForm: React.FC<NodePropFormProps> = (props) => {
    const { properties } = props;

    const [tabKey, setTabKey] = React.useState<string>('1');
    const [storageProviders, setStorageProviders] = React.useState<any[]>([]);

    useEffect(() => {
        if (properties.length > 0) {
            setTabKey('2');
        }
    }, []);

    const getScriptDefinitonsContent = async () => {
        if (!props.workflowDefinitionId) return '';
        const result = await getEditorScriptDefinitonsContent(props.workflowDefinitionId);
        return result;
    };

    const loadStorageProviders = async () => {
        const result = await getWorkflowStorageProviders();
        setStorageProviders(result?.items ?? []);
    };

    const renderItemInput = (
        inputName: NamePath,
        value: any,
        uiEditor: string,
        propItem: NodeTypeProperty,
        inputProps: any,
        options: any,
    ) => {
        if (uiEditor == 'Default') {
            switch (propItem.uiHint) {
                case 'check-list':
                    return (
                        <ProFormCheckbox.Group
                            {...inputProps}
                            name={inputName}
                            options={options}
                            extra={propItem.hint}
                            rules={[{ required: propItem.isRequired }]}
                        />
                    );
                case 'checkbox':
                    return (
                        <ProFormSwitch
                            {...inputProps}
                            name={inputName}
                            extra={propItem.hint}
                            rules={[{ required: propItem.isRequired }]}
                        />
                    );
                case 'dropdown':
                    return (
                        <ProFormSelect
                            {...inputProps}
                            name={inputName}
                            options={options}
                            rules={[{ required: propItem.isRequired }]}
                        />
                    );
                case 'multi-text':
                    return (
                        <ProFormSelect
                            {...inputProps}
                            name={inputName}
                            options={options}
                            fieldProps={{ mode: 'tags' }}
                            rules={[{ required: propItem.isRequired }]}
                        />
                    );
                case 'multi-line':
                    return (
                        <ProFormTextArea
                            {...inputProps}
                            name={inputName}
                            fieldProps={{
                                rows: 2,
                                autoSize: { minRows: 2, maxRows: 10 },
                                showCount: false,
                            }}
                            rules={[{ required: propItem.isRequired }]}
                        />
                    );
                case 'code-editor':
                    return (
                        <Col span={defaultInputSpan}>
                            <ProForm.Item
                                label={inputProps.label}
                                name={inputName}
                                required={propItem.isRequired}
                                extra={propItem.hint}
                                rules={[{ required: propItem.isRequired }]}
                            >
                                <MonacorEditorInput
                                    height={150}
                                    language={getEditorLanguage(uiEditor)}
                                    getJavaScriptLibs={async () => {
                                        if (uiEditor.toLocaleLowerCase() == 'javascript') {
                                            return [
                                                {
                                                    content: await getScriptDefinitonsContent(),
                                                    filePath: 'script.d.ts',
                                                },
                                            ];
                                        }
                                        return [];
                                    }}
                                />
                            </ProForm.Item>
                        </Col>
                    );
                case 'switch-case-builder':
                    return (
                        <Col span={defaultInputSpan}>
                            <ProForm.Item
                                label={inputProps.label}
                                name={inputName}
                                required={propItem.isRequired}
                                extra={propItem.hint}
                                rules={[{ required: propItem.isRequired }]}
                            >
                                <CaseEditorInput />
                            </ProForm.Item>
                        </Col>
                    );
                default:
                    return (
                        <ProFormText
                            {...inputProps}
                            name={inputName}
                            rules={[{ required: propItem.isRequired }]}
                        />
                    );
            }
        } else if (uiEditor == 'Switch') {
            return (
                <Col span={defaultInputSpan}>
                    <ProForm.Item
                        label={inputProps.label}
                        name={inputName}
                        required={propItem.isRequired}
                        extra={propItem.hint}
                        rules={[{ required: propItem.isRequired }]}
                    >
                        <CaseEditorInput />
                    </ProForm.Item>
                </Col>
            );
        }
        return (
            <Col span={defaultInputSpan}>
                <ProForm.Item
                    label={inputProps.label}
                    name={inputName}
                    required={propItem.isRequired}
                    extra={propItem.hint}
                    rules={[{ required: propItem.isRequired }]}
                >
                    <MonacorEditorInput
                        height={150}
                        language={getEditorLanguage(uiEditor)}
                        getJavaScriptLibs={async () => {
                            if (uiEditor.toLocaleLowerCase() == 'javascript') {
                                return [
                                    {
                                        content: await getScriptDefinitonsContent(),
                                        filePath: 'script.d.ts',
                                    },
                                ];
                            }
                            return [];
                        }}
                    />
                </ProForm.Item>
            </Col>
        );
    };

    const renderFormItems = () => {
        return (properties ?? []).map((propItem) => {
            const options = Array.isArray(propItem.options)
                ? propItem.options.map((o: any) => {
                      return { label: o.text, value: o.value };
                  })
                : Array.isArray(propItem.options?.items)
                ? propItem.options?.items.map((o: any) => {
                      return { label: o.text, value: o.value };
                  })
                : null;
            const inputProps = {
                // key: propItem.name,
                label: propItem.label,
                // name: ['props', propItem.name, ],
                readOnly: propItem.isReadOnly,
                placeholder: propItem.hint,
                colProps: { span: defaultInputSpan },
                // initialValue: propItem.defaultValue,
                required: propItem.isRequired,
            };

            const propSyntax = getPropertySyntaxes(propItem);
            let syntaxeList = (propSyntax.supports ?? []).map((x) => {
                return { label: x, key: x };
            });
            if (propSyntax.editor) {
                syntaxeList = [{ label: propSyntax.editor, key: propSyntax.editor ?? 'Literal' }];
            } else if (propItem.defaultSyntax != 'Switch') {
                syntaxeList = [{ label: 'Default', key: 'Default' }, ...syntaxeList];
            } else {
                // no-op
            }

            return (
                <ProFormGroup key={propItem.name}>
                    {propSyntax.editor ? (
                        renderItemInput(
                            [
                                'props',
                                propItem.name,
                                'expressions',
                                propSyntax.default ?? 'Literal',
                            ],
                            propItem.defaultValue,
                            propSyntax.editor,
                            propItem,
                            inputProps,
                            options,
                        )
                    ) : (
                        <>
                            <ProFormDependency name={['name', ['props', propItem.name, 'syntax']]}>
                                {({ props }) => {
                                    const syntax = props?.[propItem.name]?.syntax;
                                    return renderItemInput(
                                        ['props', propItem.name, 'expressions', syntax],
                                        propItem.defaultValue,
                                        syntax,
                                        propItem,
                                        inputProps,
                                        options,
                                    );
                                }}
                            </ProFormDependency>
                        </>
                    )}

                    {/* syntax */}
                    <ProFormSelect
                        name={['props', propItem.name, 'syntax']}
                        options={syntaxeList.map((item) => {
                            return {
                                label: item.label,
                                value: item.key,
                            };
                        })}
                        // initialValue={defaultSyntax}
                        allowClear={false}
                        colProps={{ span: defaultSyntaxSpan }}
                    />
                </ProFormGroup>
            );
        });
    };

    const renderFormItemMemo = useMemo(
        () => renderFormItems(),
        [props.properties, renderFormItems],
    );

    useEffect(() => {
        loadStorageProviders();
    }, []);

    return (
        <>
            <Tabs defaultActiveKey={tabKey} type="line" style={{ width: '100%', flex: 1 }}>
                {properties.length > 0 && (
                    <Tabs.TabPane tab="Properties" key="1">
                        <Row>{renderFormItemMemo}</Row>
                    </Tabs.TabPane>
                )}

                <Tabs.TabPane tab="Common" key="2">
                    <Row>
                        <ProFormText
                            label="Name"
                            name="name"
                            rules={[
                                { required: true },
                                { max: 32 },
                                { pattern: /^\w+$/, message: 'Invalid characters' },
                            ]}
                            placeholder="The technical name of the activity."
                        />
                        <ProFormText
                            label="Display Name"
                            name="displayName"
                            rules={[{ required: true }, { max: 32 }]}
                            placeholder="The friendly name of the activity."
                        />
                        <ProFormTextArea
                            label="Description"
                            name="description"
                            rules={[{ max: 128 }]}
                            fieldProps={{ autoSize: { minRows: 2, maxRows: 10 } }}
                            placeholder="A custom description for this activity."
                        />
                    </Row>
                </Tabs.TabPane>
                <Tabs.TabPane tab="Storage" key="3">
                    <Row>
                        <ProFormSwitch
                            name="loadWorkflowContext"
                            label="Load Workflow Context"
                            extra="When enabled, this will load the workflow context into memory before executing this activity."
                        />
                        <ProFormSwitch
                            name="saveWorkflowContext"
                            label="Save Workflow Context"
                            extra="When enabled, this will save the workflow context back into storage after executing this activity."
                        />
                        <ProFormSwitch
                            name="persistWorkflow"
                            label="Save Workflow Instance"
                            extra="When enabled, this will save the workflow instance back into storage right after executing this activity."
                        />
                    </Row>
                </Tabs.TabPane>
                <Tabs.TabPane tab="Activity Output" key="4">
                    <Row>
                        <ProFormSelect
                            name={['propertyStorageProviders', 'Output']}
                            label="Default Storage Provider"
                            extra="Configure the desired storage for each output property of this activity."
                            options={[{ label: 'Default', value: '' }].concat(
                                storageProviders.map((x: any) => {
                                    return { label: x.displayName, value: x.name };
                                }),
                            )}
                        />
                        <Divider style={{ marginTop: 0 }} />
                        {(props.properties ?? []).map((item) => {
                            return (
                                <ProFormSelect
                                    key={'propertyStorageProviders_' + item.name}
                                    name={['propertyStorageProviders', item.name]}
                                    label={item.name}
                                    options={[{ label: 'Default', value: '' }].concat(
                                        storageProviders.map((x: any) => {
                                            return { label: x.displayName, value: x.name };
                                        }),
                                    )}
                                />
                            );
                        })}
                    </Row>
                </Tabs.TabPane>
            </Tabs>

            {/*  */}
        </>
    );
};

export default NodePropForm;
