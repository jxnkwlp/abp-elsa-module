import {
    getWorkflowDefinitionVariables,
    updateWorkflowDefinitionVariables,
} from '@/services/WorkflowDefinition';
import { WorkflowPersistenceBehavior } from '@/services/enums';
import type { API } from '@/services/typings';
import { ProDescriptions } from '@ant-design/pro-components';
import { useAsyncEffect } from 'ahooks';
import { Button, Card, message } from 'antd';
import React from 'react';
import { useAccess, useIntl } from 'umi';
import VariableForm from './variableForm';

export const Basic: React.FC<{ id: string; data: API.WorkflowDefinition }> = (props) => {
    const intl = useIntl();
    const access = useAccess();

    const [id, setId] = React.useState<string>(props.id);
    const [loading, setLoading] = React.useState<boolean>(false);

    const [data, setData] = React.useState<API.WorkflowDefinition>(props.data);

    const [variableEditModalVisible, setVariableEditModalVisible] = React.useState<boolean>();
    const [variableData, setVariableData] = React.useState<any>();

    const loadVariable = async () => {
        const loading = message.loading(intl.formatMessage({ id: 'common.dict.loading' }));
        const result = await getWorkflowDefinitionVariables(id!);
        loading();
        setVariableData(result.variables);
        setVariableEditModalVisible(true);
    };

    const handleUpdateVariable = async (values: any) => {
        const loading = message.loading(intl.formatMessage({ id: 'common.dict.loading' }));
        const result = await updateWorkflowDefinitionVariables(id!, { variables: values });
        loading();
        message.success(intl.formatMessage({ id: 'common.dict.save.success' }));
        return result != null;
    };

    useAsyncEffect(async () => {}, [0]);

    return (
        <>
            <Card
                title={intl.formatMessage({ id: 'common.dict.general' })}
                style={{ marginBottom: 16 }}
                loading={loading}
                extra={
                    access['ElsaWorkflow.Definitions.CreateOrUpdateOrPublish'] ? (
                        <>
                            <Button
                                key="variables"
                                type="link"
                                disabled={!data}
                                onClick={() => {
                                    loadVariable();
                                }}
                            >
                                {intl.formatMessage({ id: 'page.definition.edit.variables' })}
                            </Button>
                        </>
                    ) : (
                        <></>
                    )
                }
            >
                <ProDescriptions
                    dataSource={data}
                    columns={[
                        {
                            title: intl.formatMessage({ id: 'page.definition.field.name' }),
                            dataIndex: 'name',
                            copyable: true,
                            ellipsis: true,
                        },
                        {
                            title: intl.formatMessage({
                                id: 'page.definition.field.displayName',
                            }),
                            dataIndex: 'displayName',
                            copyable: true,
                            ellipsis: true,
                        },
                        {
                            title: intl.formatMessage({ id: 'page.definition.field.description' }),
                            dataIndex: 'description',
                            ellipsis: true,
                        },
                        {
                            title: intl.formatMessage({ id: 'page.definition.field.tag' }),
                            dataIndex: 'tag',
                        },
                        {
                            title: intl.formatMessage({ id: 'page.definition.field.channel' }),
                            dataIndex: 'channel',
                        },
                        {
                            title: intl.formatMessage({
                                id: 'page.definition.field.persistenceBehavior',
                            }),
                            dataIndex: 'persistenceBehavior',
                            valueEnum: WorkflowPersistenceBehavior,
                        },
                        {
                            title: intl.formatMessage({ id: 'page.definition.field.isSingleton' }),
                            dataIndex: 'isSingleton',
                            valueEnum: {
                                true: 'Y',
                                false: 'N',
                            },
                        },
                        {
                            title: intl.formatMessage({ id: 'common.dict.creationTime' }),
                            dataIndex: 'creationTime',
                            valueType: 'dateTime',
                        },
                        {
                            title: intl.formatMessage({
                                id: 'page.definition.field.lastModificationTime',
                            }),
                            dataIndex: 'lastModificationTime',
                            valueType: 'dateTime',
                        },
                    ]}
                />
            </Card>

            {/* variable */}
            <VariableForm
                data={variableData}
                visible={variableEditModalVisible}
                onVisibleChange={setVariableEditModalVisible}
                onSubmit={async (value: any) => {
                    console.debug('update variables', value);
                    return await handleUpdateVariable(value);
                }}
            />
        </>
    );
};
