import { getWorkflowDefinitionAssignableGroups } from '@/services/WorkflowDefinition';
import { WorkflowContextFidelity, WorkflowPersistenceBehavior } from '@/services/enums';
import { ProFormSelect, ProFormSwitch, ProFormText, ProFormTextArea } from '@ant-design/pro-form';
import { Tabs } from 'antd';
import { useIntl } from 'umi';

const EditFormItems: React.FC = () => {
    const intl = useIntl();

    return (
        <>
            <Tabs
                defaultActiveKey="1"
                type="line"
                items={[
                    {
                        key: 'basic',
                        label: intl.formatMessage({ id: 'page.definition.edit.basic' }),
                        children: (
                            <>
                                <ProFormText
                                    label={intl.formatMessage({ id: 'page.definition.field.name' })}
                                    name="name"
                                    rules={[
                                        { required: true },
                                        { max: 64 },
                                        {
                                            pattern: /^[\u4e00-\u9fa5A-Za-z0-9\-\_]*$/,
                                            message: 'Invalid characters',
                                        },
                                    ]}
                                    extra={intl.formatMessage({
                                        id: 'page.definition.field.name.tips',
                                    })}
                                />
                                <ProFormText
                                    label={intl.formatMessage({
                                        id: 'page.definition.field.displayName',
                                    })}
                                    name="displayName"
                                    rules={[{ required: true }, { max: 128 }]}
                                    extra={intl.formatMessage({
                                        id: 'page.definition.field.displayName.tips',
                                    })}
                                />
                                <ProFormSelect
                                    label={intl.formatMessage({
                                        id: 'page.definition.field.groupName',
                                    })}
                                    name="groupId"
                                    rules={[{ required: false }]}
                                    request={async () => {
                                        const result =
                                            await getWorkflowDefinitionAssignableGroups();
                                        return (result?.items ?? []).map((x) => {
                                            return {
                                                label: x.name,
                                                value: x.id,
                                            };
                                        });
                                    }}
                                    // fieldProps={{ showSearch: true }}
                                />
                                <ProFormTextArea
                                    label={intl.formatMessage({
                                        id: 'page.definition.field.description',
                                    })}
                                    name="description"
                                    fieldProps={{ rows: 2, autoSize: { minRows: 2, maxRows: 5 } }}
                                    rules={[{ max: 256 }]}
                                />
                            </>
                        ),
                    },
                    {
                        key: 'Workflow Context',
                        label: intl.formatMessage({ id: 'page.definition.edit.workflowContext' }),
                        children: (
                            <>
                                <ProFormText
                                    label={intl.formatMessage({
                                        id: 'page.definition.field.contextOptions.type',
                                    })}
                                    name={['contextOptions', 'type']}
                                    rules={[{ required: false }, { max: 128 }]}
                                    extra={intl.formatMessage({
                                        id: 'page.definition.field.contextOptions.type.placeholder',
                                    })}
                                />
                                <ProFormSelect
                                    label={intl.formatMessage({
                                        id: 'page.definition.field.contextOptions.contextFidelity',
                                    })}
                                    name={['contextOptions', 'contextFidelity']}
                                    request={async () => {
                                        return Object.keys(WorkflowContextFidelity)
                                            .filter((x) => parseInt(x) >= 0)
                                            .map((key) => {
                                                return {
                                                    value: parseInt(key),
                                                    label: WorkflowContextFidelity[key],
                                                };
                                            });
                                    }}
                                    extra={intl.formatMessage({
                                        id: 'page.definition.field.contextOptions.contextFidelity.placeholder',
                                    })}
                                />
                            </>
                        ),
                    },
                    {
                        key: 'advanced',
                        label: intl.formatMessage({ id: 'page.definition.edit.advanced' }),
                        children: (
                            <>
                                <ProFormText
                                    label={intl.formatMessage({ id: 'page.definition.field.tag' })}
                                    name="tag"
                                    rules={[{ required: false }, { max: 64 }]}
                                />
                                <ProFormText
                                    label={intl.formatMessage({
                                        id: 'page.definition.field.channel',
                                    })}
                                    name="channel"
                                    rules={[{ required: false }, { max: 64 }]}
                                />
                                <ProFormSelect
                                    label={intl.formatMessage({
                                        id: 'page.definition.field.persistenceBehavior',
                                    })}
                                    name="persistenceBehavior"
                                    request={async () => {
                                        return Object.keys(WorkflowPersistenceBehavior)
                                            .filter((x) => parseInt(x) >= 0)
                                            .map((key) => {
                                                return {
                                                    value: parseInt(key),
                                                    label: WorkflowPersistenceBehavior[key],
                                                };
                                            });
                                    }}
                                    initialValue={WorkflowPersistenceBehavior.WorkflowBurst}
                                />
                                <ProFormSwitch
                                    label={intl.formatMessage({
                                        id: 'page.definition.field.isSingleton',
                                    })}
                                    name="isSingleton"
                                />
                                <ProFormSwitch
                                    label={intl.formatMessage({
                                        id: 'page.definition.field.deleteCompletedInstances',
                                    })}
                                    name="deleteCompletedInstances"
                                />
                            </>
                        ),
                    },
                ]}
            />
        </>
    );
};

export default EditFormItems;
