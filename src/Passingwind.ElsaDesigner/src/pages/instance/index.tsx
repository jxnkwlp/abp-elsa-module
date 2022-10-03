import { WorkflowStatus } from '@/services/enums';
import type { API } from '@/services/typings';
import { getWorkflowDefinitionList } from '@/services/WorkflowDefinition';
import {
    deleteWorkflowInstance,
    getWorkflowInstanceList,
    workflowInstanceCancel,
    workflowInstanceRetry,
} from '@/services/WorkflowInstance';
import { PageContainer } from '@ant-design/pro-layout';
import type { ActionType, ProColumnType } from '@ant-design/pro-table';
import ProTable from '@ant-design/pro-table';
import { message, Modal } from 'antd';
import React, { useRef } from 'react';
import { Link, useIntl } from 'umi';
import { workflowStatusEnum } from './status';

const Index: React.FC = () => {
    const actionRef = useRef<ActionType>();
    const intl = useIntl();

    const columns: ProColumnType<API.WorkflowInstance>[] = [
        {
            dataIndex: 'workflowDefinitionId',
            title: intl.formatMessage({ id: 'page.instance.field.definition' }),
            hideInTable: true,
            valueType: 'select',
            request: async (p) => {
                const list = await getWorkflowDefinitionList({
                    filter: p.keyWords ?? '',
                    maxResultCount: 100,
                });
                return (list.items ?? []).map((x) => {
                    return {
                        label: `${x.displayName}(${x.name})`,
                        value: x.id,
                    };
                });
            },
            fieldProps: { showSearch: true },
        },
        {
            dataIndex: 'name',
            title: intl.formatMessage({ id: 'page.instance.field.name' }),
            copyable: true,
            render: (_, record) => {
                return (
                    <Link
                        to={{
                            pathname: `/instances/${record.id}`,
                        }}
                    >
                        {_}
                    </Link>
                );
            },
        },
        {
            dataIndex: 'version',
            title: intl.formatMessage({ id: 'page.instance.field.version' }),
            valueType: 'digit',
            width: 80,
        },
        {
            dataIndex: 'workflowStatus',
            title: intl.formatMessage({ id: 'page.instance.field.workflowStatus' }),
            valueEnum: workflowStatusEnum,
            // valueEnum: getWorkflowStatusEnum(),
        },
        {
            dataIndex: 'creationTime',
            title: intl.formatMessage({ id: 'common.dict.creationTime' }),
            valueType: 'dateTime',
            search: false,
        },
        {
            dataIndex: 'finishedTime',
            title: intl.formatMessage({ id: 'page.instance.field.finishedTime' }),
            valueType: 'dateTime',
            search: false,
        },
        {
            dataIndex: 'lastExecutedTime',
            title: intl.formatMessage({ id: 'page.instance.field.lastExecutedTime' }),
            valueType: 'dateTime',
            search: false,
        },
        {
            dataIndex: 'faultedTime',
            title: intl.formatMessage({ id: 'page.instance.field.faultedTime' }),
            valueType: 'dateTime',
            search: false,
        },
        {
            dataIndex: 'correlationId',
            title: intl.formatMessage({ id: 'page.instance.field.correlationId' }),
            width: 150,
            ellipsis: true,
            copyable: true,
        },
        {
            title: intl.formatMessage({ id: 'common.dict.table-action' }),
            valueType: 'option',
            width: 170,
            align: 'center',
            render: (text, record, _, action) => {
                const menus = [];

                if (
                    record.workflowStatus == WorkflowStatus.Idle ||
                    record.workflowStatus == WorkflowStatus.Running ||
                    record.workflowStatus == WorkflowStatus.Suspended
                ) {
                    menus.push(
                        <a
                            key="cancel"
                            onClick={() => {
                                Modal.confirm({
                                    title: intl.formatMessage({
                                        id: 'page.instance.cancel.confirm.title',
                                    }),
                                    content: intl.formatMessage({
                                        id: 'page.instance.cancel.confirm.content',
                                    }),
                                    onOk: async () => {
                                        const result = await workflowInstanceCancel(record.id!);
                                        if (result?.response?.ok) {
                                            message.success(
                                                intl.formatMessage({
                                                    id: 'page.instance.cancel.confirm.success',
                                                }),
                                            );
                                            action?.reload();
                                        }
                                    },
                                });
                            }}
                        >
                            {intl.formatMessage({ id: 'page.instance.cancel' })}
                        </a>,
                    );
                }

                if (record.workflowStatus == WorkflowStatus.Faulted) {
                    menus.push(
                        <a
                            key="retry"
                            onClick={() => {
                                Modal.confirm({
                                    title: intl.formatMessage({
                                        id: 'page.instance.retry.confirm.title',
                                    }),
                                    content: intl.formatMessage({
                                        id: 'page.instance.retry.confirm.content',
                                    }),
                                    onOk: async () => {
                                        const result = await workflowInstanceRetry(record.id!, {});
                                        if (result?.response?.ok) {
                                            message.success(
                                                intl.formatMessage({
                                                    id: 'page.instance.retry.confirm.success',
                                                }),
                                            );
                                            action?.reload();
                                        }
                                    },
                                });
                            }}
                        >
                            {intl.formatMessage({ id: 'page.instance.retry' })}
                        </a>,
                    );
                }

                menus.push(
                    <a
                        key="delete"
                        onClick={() => {
                            Modal.confirm({
                                title: intl.formatMessage({
                                    id: 'common.dict.delete.confirm',
                                }),
                                content: intl.formatMessage({
                                    id: 'page.instance.delete.confirm.content',
                                }),
                                onOk: async () => {
                                    const result = await deleteWorkflowInstance(record.id!);
                                    if (result?.response?.ok) {
                                        message.success(
                                            intl.formatMessage({
                                                id: 'common.dict.delete.success',
                                            }),
                                        );
                                        action?.reload();
                                    }
                                },
                            });
                        }}
                    >
                        {intl.formatMessage({ id: 'common.dict.delete' })}
                    </a>,
                );

                return menus;
            },
        },
    ];

    return (
        <PageContainer>
            <ProTable<API.WorkflowInstance>
                columns={columns}
                actionRef={actionRef}
                search={{ labelWidth: 120 }}
                rowKey="id"
                request={async (params) => {
                    const { current, pageSize } = params;
                    delete params.current;
                    delete params.pageSize;
                    const skipCount = (current! - 1) * pageSize!;
                    const result = await getWorkflowInstanceList({
                        ...params,
                        skipCount,
                        maxResultCount: pageSize,
                    });
                    if (result)
                        return {
                            success: true,
                            data: result.items,
                            total: result.totalCount,
                        };
                    else {
                        return {
                            success: false,
                        };
                    }
                }}
            />
        </PageContainer>
    );
};

export default Index;
