import { getWorkflowInstanceFaultsByWorkflowDefinition } from '@/services/WorkflowInstance';
import type { API } from '@/services/typings';
import { ProTable } from '@ant-design/pro-components';
import { useAsyncEffect } from 'ahooks';
import { Card } from 'antd';
import React from 'react';
import { Link, useIntl } from 'umi';

export const ErrorList: React.FC<{ id: string }> = (props) => {
    const intl = useIntl();

    const [id, setId] = React.useState<string>(props.id);
    const [loading, setLoading] = React.useState<boolean>(false);

    const [errorList, setErrorList] = React.useState<API.WorkflowInstanceFault[]>();

    const load = async () => {
        setLoading(true);
        const list = await getWorkflowInstanceFaultsByWorkflowDefinition(id!, {
            maxResultCount: 5,
        });
        setErrorList(list?.items ?? []);
        setLoading(false);
    };

    useAsyncEffect(async () => {
        await load();
    }, [0]);

    return (
        <Card
            style={{ marginBottom: 16 }}
            title={intl.formatMessage({
                id: 'page.instance.faults.latest',
            })}
            loading={loading}
        >
            <ProTable<API.WorkflowInstanceFault>
                size="small"
                search={false}
                columns={[
                    {
                        dataIndex: 'message',
                        title: intl.formatMessage({
                            id: 'page.instance.field.fault.message',
                        }),
                        ellipsis: true,
                        copyable: true,
                    },
                    {
                        dataIndex: 'creationTime',
                        title: intl.formatMessage({
                            id: 'common.dict.creationTime',
                        }),
                        valueType: 'dateTime',
                        width: 150,
                    },
                    {
                        dataIndex: 'action',
                        title: intl.formatMessage({
                            id: 'common.dict.table-action',
                        }),
                        width: 120,
                        align: 'center',
                        renderText: (_text, record) => (
                            <Link to={`/instances/${record.workflowInstanceId}`}>
                                {intl.formatMessage({
                                    id: 'page.definition.goInstanceDetail',
                                })}
                            </Link>
                        ),
                    },
                ]}
                rowKey="id"
                dataSource={errorList}
                pagination={false}
                toolBarRender={false}
            />
        </Card>
    );
};
