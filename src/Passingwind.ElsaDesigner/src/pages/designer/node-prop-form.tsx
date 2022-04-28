import ProForm, {
    ModalForm,
    ProFormCheckbox,
    ProFormDependency,
    ProFormGroup,
    ProFormList,
    ProFormSelect,
    ProFormSwitch,
    ProFormText,
    ProFormTextArea,
} from '@ant-design/pro-form';
import { Button, Card, Col, Form, Row, Tabs } from 'antd';
import type { NamePath } from 'antd/lib/form/interface';
import React, { useEffect } from 'react';
import MonacoEditor from 'react-monaco-editor';
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

    const renderItemInput = (
        inputName: NamePath,
        value: any,
        syntax: string,
        propItem: NodeTypeProperty,
        inputProps: any,
        options: any,
    ) => {
        if (syntax == 'Default') {
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
                case 'code-editor':
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
        } else if (syntax == 'Switch') {
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
                        language={getEditorLanguage(syntax)}
                        getJavaScriptLibs={async () => {
                            if (syntax.toLocaleLowerCase() == 'javascript') {
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

            const { defaultSyntax, syntaxes } = getPropertySyntaxes(propItem);
            let syntaxeList = ['Default', ...syntaxes];
            if (propItem.defaultSyntax == 'Switch') {
                syntaxeList = [...syntaxes];
            }

            return (
                <ProFormGroup key={propItem.name}>
                    <ProFormDependency name={['name', ['props', propItem.name, 'syntax']]}>
                        {({ props }) => {
                            // console.log(props);
                            const syntax = props?.[propItem.name]?.syntax ?? defaultSyntax;
                            // return (
                            //     <ProFormText
                            //         {...inputProps}
                            //         colProps={{ span: 20 }}
                            //         name={['props', propItem.name, 'expressions', syntax]}
                            //     />
                            // );
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

                    {/* syntax */}
                    <ProFormSelect
                        name={['props', propItem.name, 'syntax']}
                        options={syntaxeList.map((item) => {
                            return {
                                label: item,
                                value: item,
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

    return (
        <>
            <Tabs defaultActiveKey={tabKey} type="line" style={{ width: '100%', flex: 1 }}>
                {properties.length > 0 && (
                    <Tabs.TabPane tab="Properties" key="1">
                        <Row>{renderFormItems()}</Row>
                    </Tabs.TabPane>
                )}

                <Tabs.TabPane tab="Common" key="2">
                    <Row>
                        <ProFormText
                            label="Name"
                            name="name"
                            rules={[{ required: true }, { max: 32 }]}
                        />
                        <ProFormText
                            label="Display Name"
                            name="displayName"
                            rules={[{ required: true }, { max: 32 }]}
                        />
                        <ProFormTextArea
                            label="Description"
                            name="description"
                            rules={[{ max: 128 }]}
                            fieldProps={{ autoSize: { minRows: 2, maxRows: 10 } }}
                        />
                    </Row>
                </Tabs.TabPane>
            </Tabs>

            {/*  */}
        </>
    );
};

export default NodePropForm;
