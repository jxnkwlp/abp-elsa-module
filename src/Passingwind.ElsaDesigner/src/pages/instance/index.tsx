import { WorkflowPersistenceBehavior, WorkflowStatus } from '@/services/enums';
import { enumToStatus } from '@/services/utils';
import { getWorkflowInstanceList } from '@/services/WorkflowInstance';
import { ModalForm } from '@ant-design/pro-form';
import { PageContainer } from '@ant-design/pro-layout';
import type { ActionType, ProColumnType } from '@ant-design/pro-table';
import ProTable from '@ant-design/pro-table';
import { Button, message, Popconfirm } from 'antd';
import React, { useRef, useState } from 'react';
import { Link, useHistory } from 'umi';

const Index: React.FC = () => {
    const actionRef = useRef<ActionType>();

    const history = useHistory();

    const [editModalVisible, setEditModalVisible] = useState<boolean>(false);
    const [editModalTitle, setEditModalTitle] = useState<string>('');
    const [editModalData, setEditModalData] = useState<API.WorkflowDefinition>();
    const [editModalDataId, setEditModalDataId] = useState<string>();

    // const [searchKey, setSearchKey] = useState<string>();

    const columns: ProColumnType<API.WorkflowDefinition>[] = [
        {
            dataIndex: 'name',
            title: 'Name',
            render: (_, record) => {
                return <Link to={`/definition/${record.id}`}>{_}</Link>;
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
            title: 'Action',
            valueType: 'option',
            width: 80,
            align: 'center',
            render: (text, record, _, action) => [
                <a
                    key="edit"
                    onClick={() => {
                        history.push('/instance/' + record.id, { id: record.id });
                    }}
                >
                    Details
                </a>,
            ],
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
