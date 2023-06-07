import { getDesignerRuntimeSelectListItems } from '@/services/Designer';
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
import { useIntl } from 'umi';
import CaseEditorInput from './form-input/case-editor-builder-input';
import MonacorEditorInput from './form-input/monacor-editor-input';
import { getEditorLanguage, getPropertySyntaxes } from './service';
import type { NodeTypeProperty } from './type';

const defaultInputSpan = 20;
const defaultSyntaxSpan = 4;

const NodePropFormItem = (
    name: string,
    inputPath: NamePath,
    value: any,
    uiEditor: string,
    propItem: NodeTypeProperty,
    inputProps: any,
    optionList: any,
) => {
    if (uiEditor == 'Default') {
        switch (propItem.uiHint) {
            case 'check-list':
                return (
                    <ProFormCheckbox.Group
                        {...inputProps}
                        name={inputPath}
                        options={optionList}
                        rules={[{ required: propItem.isRequired }]}
                    />
                );
            case 'checkbox':
                return (
                    <ProFormSwitch
                        {...inputProps}
                        name={inputPath}
                        rules={[{ required: propItem.isRequired }]}
                    />
                );
            case 'dropdown':
                return (
                    <ProFormSelect
                        {...inputProps}
                        name={inputPath}
                        options={optionList}
                        rules={[{ required: propItem.isRequired }]}
                        fieldProps={{ showSearch: true }}
                    />
                );
            case 'multi-text':
                return (
                    <ProFormSelect
                        {...inputProps}
                        name={inputPath}
                        options={optionList}
                        fieldProps={{ mode: 'tags' }}
                        rules={[{ required: propItem.isRequired }]}
                    />
                );
            case 'multi-line':
                return (
                    <ProFormTextArea
                        {...inputProps}
                        name={inputPath}
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
                            name={inputPath}
                            required={propItem.isRequired}
                            rules={[{ required: propItem.isRequired }]}
                        >
                            <MonacorEditorInput
                                id={name}
                                height={150}
                                language={getEditorLanguage(uiEditor)}
                            />
                        </ProForm.Item>
                    </Col>
                );
            case 'switch-case-builder':
                return (
                    <Col span={defaultInputSpan}>
                        <ProForm.Item
                            label={inputProps.label}
                            name={inputPath}
                            required={propItem.isRequired}
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
                        name={inputPath}
                        rules={[{ required: propItem.isRequired }]}
                    />
                );
        }
    } else if (uiEditor == 'Switch') {
        return (
            <Col span={defaultInputSpan}>
                <ProForm.Item
                    label={inputProps.label}
                    name={inputPath}
                    extra={inputProps.extra}
                    required={propItem.isRequired}
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
                name={inputPath}
                extra={inputProps.extra}
                required={propItem.isRequired}
                rules={[{ required: propItem.isRequired }]}
            >
                <MonacorEditorInput id={name} height={150} language={getEditorLanguage(uiEditor)} />
            </ProForm.Item>
        </Col>
    );
};

type NodePropFormProps = {
    workflowDefinitionId?: string;
    activityId: string;
    inProperties: NodeTypeProperty[];
    outProperties: NodeTypeProperty[];
    // initialValues: object | undefined;
    // setFieldsValue: (value: any) => void;
    // setFieldValue: (key: NamePath, value: any) => void;
    // getFieldValue: (key: NamePath) => any;
    storageProviders: Record<string, any>;
};

const NodePropForm: React.FC<NodePropFormProps> = (props) => {
    const { activityId, inProperties: properties, storageProviders } = props;

    const intl = useIntl();

    const [tabKey, setTabKey] = React.useState<string>('properties');

    useEffect(() => {
        if (properties.length > 0) {
            setTabKey('properties');
        } else {
            setTabKey('common');
        }
    }, [props, properties]);

    // useEffect(() => {
    //     console.log(props);
    // }, [0]);

    const renderProperties = () => {
        const formItems: React.ReactNode[] = [];

        (properties ?? []).forEach(async (propItem) => {
            // get field static options
            const fieldOptionItems = Array.isArray(propItem.options)
                ? propItem.options.map((o: any) => {
                      return { label: o.text, value: o.value };
                  })
                : Array.isArray(propItem.options?.items)
                ? propItem.options?.items.map((o: any) => {
                      return { label: o.text, value: o.value };
                  })
                : [];

            const isDynamicOptionItems = propItem.options?.runtimeSelectListProviderType;

            const inputProps = {
                // key: propItem.name,
                label: propItem.label,
                // name: ['props', propItem.name, ],
                readOnly: propItem.isReadOnly,
                colProps: { span: defaultInputSpan },
                // initialValue: propItem.defaultValue,
                required: propItem.isRequired,
                // placeholder: propItem.hint,
                extra: propItem.hint,
                request: null,
                fieldProps: null,
            };
            if (isDynamicOptionItems) {
                // provider request configure if 'runtimeSelectListProviderType' set.
                //@ts-ignore
                inputProps.request = async () => {
                    if (!isDynamicOptionItems) return fieldOptionItems;
                    //
                    const result = await getDesignerRuntimeSelectListItems({
                        providerTypeName: propItem.options?.runtimeSelectListProviderType,
                        context: propItem.options?.context,
                    });

                    return (result?.selectList?.items ?? []).map((x) => {
                        return {
                            label: x.text,
                            value: x.value,
                        };
                    });
                };
                //@ts-ignore
                inputProps.fieldProps = { showSearch: true };
            }

            const propSyntax = getPropertySyntaxes(propItem);
            let syntaxeList = (propSyntax.supports ?? []).map((x) => {
                return { label: x, key: x };
            });
            if (propSyntax.editor) {
                syntaxeList = [{ label: propSyntax.editor, key: propSyntax.editor ?? 'Literal' }];
            } else if (propItem.defaultSyntax != 'Switch') {
                syntaxeList = [
                    {
                        label: intl.formatMessage({
                            id: 'common.dict.default',
                        }),
                        key: 'Default',
                    },
                    ...syntaxeList,
                ];
            } else {
                // no-op
            }

            formItems.push(
                <ProFormGroup key={propItem.name}>
                    {propSyntax.editor ? (
                        NodePropFormItem(
                            `${activityId.replaceAll('-', '_')}_${propItem.name}_${
                                propSyntax.default ?? 'Literal'
                            }`,
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
                            fieldOptionItems,
                        )
                    ) : (
                        <>
                            <ProFormDependency name={['name', ['props', propItem.name, 'syntax']]}>
                                {({ props }) => {
                                    const syntax = props?.[propItem.name]?.syntax;
                                    return NodePropFormItem(
                                        `${activityId.replaceAll('-', '_')}_${
                                            propItem.name
                                        }_${syntax}`,
                                        ['props', propItem.name, 'expressions', syntax],
                                        propItem.defaultValue,
                                        syntax,
                                        propItem,
                                        inputProps,
                                        fieldOptionItems,
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
                </ProFormGroup>,
            );
        });

        return formItems;
    };

    const getTabItems = () => {
        const items = [];

        if (properties.length > 0) {
            items.push({
                key: 'properties',
                label: intl.formatMessage({ id: 'page.designer.settings.properties' }),
                children: <Row>{renderProperties()}</Row>,
            });
        }

        items.push(
            {
                key: 'common',
                label: intl.formatMessage({ id: 'page.designer.settings.common' }),
                children: (
                    <Row>
                        <ProFormText
                            label={intl.formatMessage({ id: 'page.designer.settings.field.name' })}
                            name="name"
                            rules={[
                                { required: true },
                                { max: 32 },
                                { pattern: /^\w+$/, message: 'Invalid characters' },
                            ]}
                            placeholder={intl.formatMessage({
                                id: 'page.designer.settings.field.name.tips',
                            })}
                        />
                        <ProFormText
                            label={intl.formatMessage({
                                id: 'page.designer.settings.field.displayName',
                            })}
                            name="displayName"
                            rules={[{ required: true }, { max: 64 }]}
                            placeholder={intl.formatMessage({
                                id: 'page.designer.settings.field.displayName.tips',
                            })}
                        />
                        <ProFormTextArea
                            label={intl.formatMessage({
                                id: 'page.designer.settings.field.description',
                            })}
                            name="description"
                            rules={[{ max: 128 }]}
                            fieldProps={{ autoSize: { minRows: 2, maxRows: 10 } }}
                            placeholder={intl.formatMessage({
                                id: 'page.designer.settings.field.description.tips',
                            })}
                        />
                    </Row>
                ),
            },
            {
                key: 'activityOutput',
                label: intl.formatMessage({
                    id: 'page.designer.settings.storage',
                }),
                children: (
                    <Row>
                        <ProFormSwitch
                            name="loadWorkflowContext"
                            label={intl.formatMessage({
                                id: 'page.designer.settings.field.loadWorkflowContext',
                            })}
                            extra={intl.formatMessage({
                                id: 'page.designer.settings.field.loadWorkflowContext.tips',
                            })}
                        />
                        <ProFormSwitch
                            name="saveWorkflowContext"
                            label={intl.formatMessage({
                                id: 'page.designer.settings.field.saveWorkflowContext',
                            })}
                            extra={intl.formatMessage({
                                id: 'page.designer.settings.field.saveWorkflowContext.tips',
                            })}
                        />
                        <ProFormSwitch
                            name="persistWorkflow"
                            label={intl.formatMessage({
                                id: 'page.designer.settings.field.persistWorkflow',
                            })}
                            extra={intl.formatMessage({
                                id: 'page.designer.settings.field.persistWorkflow.tips',
                            })}
                        />
                        {/* Output */}
                        {(props.outProperties ?? []).length > 0 ? (
                            <>
                                <Divider style={{ marginTop: 0 }}>
                                    {intl.formatMessage({
                                        id: 'page.designer.settings.field.propertyStorageProviders.output',
                                    })}
                                </Divider>
                                {(props.outProperties ?? []).map((item) => {
                                    return (
                                        <ProFormSelect
                                            key={'propertyStorageProviders_' + item.name}
                                            name={['propertyStorageProviders', item.name]}
                                            label={item.label ?? item.name}
                                            disabled={item.disableWorkflowProviderSelection}
                                            options={[
                                                {
                                                    label: intl.formatMessage({
                                                        id: 'common.dict.default',
                                                    }),
                                                    value: '',
                                                },
                                            ].concat(
                                                storageProviders.map((x: any) => {
                                                    return {
                                                        label: x.displayName,
                                                        value: x.name,
                                                    };
                                                }),
                                            )}
                                        />
                                    );
                                })}
                            </>
                        ) : null}
                        {/* Input */}
                        {(props.inProperties ?? []).length > 0 ? (
                            <>
                                <Divider style={{ marginTop: 0 }}>
                                    {' '}
                                    {intl.formatMessage({
                                        id: 'page.designer.settings.field.propertyStorageProviders.input',
                                    })}
                                </Divider>
                                {(props.inProperties ?? []).map((item) => {
                                    return (
                                        <ProFormSelect
                                            key={'propertyStorageProviders_' + item.name}
                                            name={['propertyStorageProviders', item.name]}
                                            label={item.label}
                                            disabled={item.disableWorkflowProviderSelection}
                                            options={[
                                                {
                                                    label: intl.formatMessage({
                                                        id: 'common.dict.default',
                                                    }),
                                                    value: '',
                                                },
                                            ].concat(
                                                storageProviders.map((x: any) => {
                                                    return {
                                                        label: x.displayName,
                                                        value: x.name,
                                                    };
                                                }),
                                            )}
                                        />
                                    );
                                })}
                            </>
                        ) : null}
                    </Row>
                ),
            },
        );

        return items;
    };

    return (
        <>
            <Tabs
                activeKey={tabKey}
                onChange={setTabKey}
                type="line"
                style={{ width: '100%', flex: 1 }}
                items={getTabItems()}
            />
        </>
    );
};

export default NodePropForm;
