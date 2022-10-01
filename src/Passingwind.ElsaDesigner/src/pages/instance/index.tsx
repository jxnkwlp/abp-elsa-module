import { WorkflowStatus } from '@/services/enums';
import { API } from '@/services/typings';
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
import React, { useRef, useState } from 'react';
import { Link, useHistory } from 'umi';
import { workflowStatusEnum } from './status';

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
            dataIndex: 'workflowDefinitionId',
            title: 'Definition',
            hideInTable: true,
            valueType: 'select',
            request: async (p) => {
                console.log(p);
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
            title: 'Name',
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
            title: 'Version',
            valueType: 'digit',
        },
        {
            dataIndex: 'workflowStatus',
            title: 'Status',
            valueEnum: workflowStatusEnum,
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
            title: 'Correlation Id',
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
