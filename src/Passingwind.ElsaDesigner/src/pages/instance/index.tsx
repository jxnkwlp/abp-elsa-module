import { WorkflowStatus } from '@/services/enums';
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
import React, { useRef, useState } from 'react';
import { Link, useHistory } from 'umi';

const Index: React.FC = () => {
    const actionRef = useRef<ActionType>();

    const history = useHistory();

    // const [editModalVisible, setEditModalVisible] = useState<boolean>(false);
    // const [editModalTitle, setEditModalTitle] = useState<string>('');
    // const [editModalData, setEditModalData] = useState<API.WorkflowInstance>();
    // const [editModalDataId, setEditModalDataId] = useState<string>();

    // const [searchKey, setSearchKey] = useState<string>();

    const columns: ProColumnType<API.WorkflowInstance>[] = [
        {
            dataIndex: 'name',
            title: 'Name',
            render: (_, record) => {
                return (
                    <Link
                        to={{
                            pathname: `/instances/${record.id}`,
                            state: {
                                id: record.id,
                            },
                        }}
                    >
                        {_}
                    </Link>
                );
            },
        },
        // {
        //     dataIndex: 'definitionId',
        //     title: 'Definition Id',
        // },
        {
            dataIndex: 'version',
            title: 'Version',
            valueType: 'digit',
        },
        {
            dataIndex: 'workflowStatus',
            title: 'Status',
            // valueEnum: enumToStatus(WorkflowStatus),
            valueEnum: {
                [WorkflowStatus.Idle]: {
                    text: WorkflowStatus[WorkflowStatus.Idle],
                    status: 'default',
                },
                [WorkflowStatus.Running]: {
                    text: WorkflowStatus[WorkflowStatus.Running],
                    status: 'processing',
                },
                [WorkflowStatus.Finished]: {
                    text: WorkflowStatus[WorkflowStatus.Finished],
                    status: 'success',
                },
                [WorkflowStatus.Suspended]: {
                    text: WorkflowStatus[WorkflowStatus.Suspended],
                    status: 'warning',
                },
                [WorkflowStatus.Faulted]: {
                    text: WorkflowStatus[WorkflowStatus.Faulted],
                    status: 'error',
                },
                [WorkflowStatus.Cancelled]: {
                    text: WorkflowStatus[WorkflowStatus.Cancelled],
                    status: 'default',
                },
            },
        },
        {
            title: 'Creation Time',
            dataIndex: 'creationTime',
            valueType: 'dateTime',
            search: false,
        },
        {
            title: 'Finished Time',
            dataIndex: 'finishedTime',
            valueType: 'dateTime',
            search: false,
        },
        {
            title: 'Last Executed Time',
            dataIndex: 'lastExecutedTime',
            valueType: 'dateTime',
            search: false,
        },
        {
            title: 'Faulted Time',
            dataIndex: 'faultedTime',
            valueType: 'dateTime',
            search: false,
        },
        {
            title: 'CorrelationId',
            dataIndex: 'correlationId',
        },
        {
            title: 'Action',
            valueType: 'option',
            width: 200,
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
                                    title: 'Are you sure to cancel this instance?',
                                    content:
                                        'This operation will cancel the instance and all the tasks in it.',
                                    onOk: async () => {
                                        const result = await workflowInstanceCancel(record.id!);
                                        if (result?.response?.ok) {
                                            message.success('Canceled successfully');
                                            action?.reload();
                                        }
                                    },
                                });
                            }}
                        >
                            <span>Cancel</span>
                        </a>,
                    );
                }

                if (record.workflowStatus == WorkflowStatus.Faulted) {
                    menus.push(
                        <a
                            key="retry"
                            onClick={() => {
                                Modal.confirm({
                                    title: 'Are you sure to retry this instance?',
                                    content: 'This operation will retry the instance.',
                                    onOk: async () => {
                                        const result = await workflowInstanceRetry(record.id!, {});
                                        if (result?.response?.ok) {
                                            message.success('Retried successfully');
                                            action?.reload();
                                        }
                                    },
                                });
                            }}
                        >
                            <span>Retry</span>
                        </a>,
                    );
                }

                menus.push(
                    <a
                        key="delete"
                        onClick={() => {
                            Modal.confirm({
                                title: 'Are you sure to delete this instance?',
                                content: 'This operation will delete the instance.',
                                onOk: async () => {
                                    const result = await deleteWorkflowInstance(record.id!);
                                    if (result?.response?.ok) {
                                        message.success('Deleted successfully');
                                        action?.reload();
                                    }
                                },
                            });
                        }}
                    >
                        <span>Delete</span>
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
