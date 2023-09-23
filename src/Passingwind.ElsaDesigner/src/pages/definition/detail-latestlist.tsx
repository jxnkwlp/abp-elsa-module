import { getWorkflowInstanceList } from '@/services/WorkflowInstance';
import { WorkflowInstanceStatus } from '@/services/enums';
import { DownOutlined } from '@ant-design/icons';
import { ProTable } from '@ant-design/pro-components';
import { useAsyncEffect } from 'ahooks';
import { Card, Dropdown, Space } from 'antd';
import React from 'react';
import { Link, useIntl } from 'umi';
import { workflowStatusEnum } from '../instance/status';

export const LatestList: React.FC<{ id: string }> = (props) => {
    const intl = useIntl();

    const [id, setId] = React.useState<string>(props.id);
    const [loading, setLoading] = React.useState<boolean>(false);

    const [topList, setTopList] = React.useState<any>();
    const [topListGroup, setTopListGroup] = React.useState<string>('time');

    const load = async (category: string) => {
        setLoading(true);
        setTopListGroup(category);

        let status = null;
        let sorting: string = 'id desc';
        if (category == 'error') status = WorkflowInstanceStatus.Faulted;
        else if (category == 'running') status = WorkflowInstanceStatus.Running;
        else if (category == 'suspended') status = WorkflowInstanceStatus.Suspended;
        else if (category == 'duration') {
            sorting = 'finishedDuration desc';
        }

        const list = await getWorkflowInstanceList({
            workflowDefinitionId: id,
            workflowStatus: status,
            sorting: sorting,
            skipCount: 0,
            maxResultCount: 10,
        });

        setTopList(list?.items ?? []);
        setLoading(false);
    };

    useAsyncEffect(async () => {
        await load('time');
    }, [0]);

    return (
        <Card
            style={{ marginBottom: 16 }}
            title={intl.formatMessage(
                {
                    id: 'page.instance.list.latestOfCount',
                },
                { number: 10 },
            )}
            extra={
                <Dropdown
                    menu={{
                        items: [
                            {
                                label: intl.formatMessage({
                                    id: 'page.instance.list.groupBy.time',
                                }),
                                key: 'time',
                            },
                            {
                                label: intl.formatMessage({
                                    id: 'page.instance.list.groupBy.duration',
                                }),
                                key: 'duration',
                            },
                            {
                                label: intl.formatMessage({
                                    id: 'page.instance.list.groupBy.error',
                                }),
                                key: 'error',
                            },
                            {
                                label: intl.formatMessage({
                                    id: 'page.instance.list.groupBy.running',
                                }),
                                key: 'running',
                            },
                            {
                                label: intl.formatMessage({
                                    id: 'page.instance.list.groupBy.suspended',
                                }),
                                key: 'suspended',
                            },
                        ],
                        onClick: (item) => {
                            load(item.key);
                        },
                    }}
                >
                    <Space>
                        {intl.formatMessage({ id: 'page.instance.list.groupBy' })}
                        <a onClick={(e) => e.preventDefault()}>
                            <Space>
                                {intl.formatMessage({
                                    id: 'page.instance.list.groupBy.' + topListGroup,
                                })}
                                <DownOutlined />
                            </Space>
                        </a>
                    </Space>
                </Dropdown>
            }
        >
            <ProTable
                loading={loading}
                search={false}
                columns={[
                    {
                        dataIndex: 'name',
                        title: intl.formatMessage({ id: 'page.instance.field.name' }),
                        renderText: (text, record) => (
                            <Link to={`/instances/${record.id}`}>{text}</Link>
                        ),
                    },
                    {
                        dataIndex: 'version',
                        title: intl.formatMessage({
                            id: 'page.instance.field.version',
                        }),
                        valueType: 'digit',
                        width: 100,
                    },
                    {
                        dataIndex: 'workflowStatus',
                        title: intl.formatMessage({
                            id: 'page.instance.field.workflowStatus',
                        }),
                        valueEnum: workflowStatusEnum,
                        width: 120,
                    },
                    {
                        dataIndex: 'creationTime',
                        title: intl.formatMessage({ id: 'common.dict.creationTime' }),
                        valueType: 'dateTime',
                        width: 150,
                    },
                    {
                        dataIndex: 'finishedDuration',
                        title: intl.formatMessage({
                            id: 'page.instance.field.finishedDuration',
                        }),
                        valueType: 'time',
                        width: 120,
                    },
                ]}
                rowKey="id"
                dataSource={topList}
                pagination={false}
                toolBarRender={false}
            />
        </Card>
    );
};
