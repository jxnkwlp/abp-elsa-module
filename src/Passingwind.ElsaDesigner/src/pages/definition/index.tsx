import {
    deleteWorkflowDefinition,
    getWorkflowDefinitionList,
    updateWorkflowDefinitionDefinition,
    workflowDefinitionPublish,
    workflowDefinitionUnPublish,
} from '@/services/WorkflowDefinition';
import { ModalForm } from '@ant-design/pro-form';
import { PageContainer } from '@ant-design/pro-layout';
import type { ActionType, ProColumnType } from '@ant-design/pro-table';
import { TableDropdown } from '@ant-design/pro-table';
import ProTable from '@ant-design/pro-table';
import { Button, message, Modal, Popconfirm } from 'antd';
import React, { useRef, useState } from 'react';
import { useHistory } from 'umi';
import EditFormItems from './edit-form-items';

const handleEdit = async (id: string, data: any) => {
    const response = await updateWorkflowDefinitionDefinition(id, data);
    if (response) {
        message.success('Successfully modified');
        return true;
    }
    return false;
};

const handleDelete = async (id: string) => {
    const response = await deleteWorkflowDefinition(id);
    if (response?.response?.ok) {
        message.success('Delete successful');
        return true;
    }
    return false;
};

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
            search: false,
        },
        {
            dataIndex: 'displayName',
            title: 'Display Name',
            search: false,
        },
        {
            dataIndex: 'description',
            title: 'Description',
            search: false,
        },
        {
            dataIndex: 'latestVersion',
            title: 'Latest Version',
            search: false,
        },
        {
            dataIndex: 'publishedVersion',
            title: 'Published Version',
            search: false,
        },
        // {
        //     dataIndex: 'isSingleton',
        //     title: 'Singleton',
        //     search: false,
        //     valueEnum: {
        //         true: { text: 'Yes' },
        //         false: { text: 'No' },
        //     },
        // },
        {
            title: 'Modification Time',
            dataIndex: 'creationTime',
            valueType: 'dateTime',
            renderText: (_, record) => {
                return record.lastModificationTime ?? record.creationTime;
            },
        },
        {
            title: 'Action',
            valueType: 'option',
            width: 200,
            align: 'center',
            render: (text, record, _, action) => [
                <a
                    key="edit"
                    onClick={() => {
                        setEditModalData(record);
                        setEditModalDataId(record.id);
                        setEditModalVisible(true);
                        setEditModalTitle(`编辑 ${record.name}`);
                    }}
                >
                    Edit
                </a>,
                <a
                    key="designer"
                    onClick={() => {
                        history.push('/designer?id=' + record.id);
                    }}
                >
                    Designer
                </a>,
                <TableDropdown
                    key="actionGroup"
                    onSelect={async (key) => {
                        if (key == 'delete') {
                            Modal.confirm({
                                title: 'Confirm delete?',
                                onOk: async () => {
                                    if (await handleDelete(record.id!)) {
                                        action?.reload();
                                    }
                                },
                            });
                        } else if (key == 'publish') {
                            if (await workflowDefinitionPublish(record.id!)) {
                                action?.reload();
                            }
                        } else if (key == 'unpublish') {
                            if (await workflowDefinitionUnPublish(record.id!)) {
                                action?.reload();
                            }
                        }
                    }}
                    menus={[
                        {
                            key: 'publish',
                            name: 'Publish',
                            disabled: record.publishedVersion != null,
                        },
                        {
                            key: 'unpublish',
                            name: 'Unpublish',
                            disabled: record.publishedVersion == null,
                        },
                        { key: 'delete', name: 'Delete' },
                    ]}
                />,
                // <Popconfirm
                //     key="delete"
                //     title="Confirm?"
                //     onConfirm={async () => {
                //         if (await handleDelete(record.id!)) {
                //             action?.reload();
                //         }
                //     }}
                // >
                //     <a>Delete</a>
                // </Popconfirm>,
            ],
        },
    ];

    return (
        <PageContainer>
            <ProTable<API.WorkflowDefinition>
                columns={columns}
                actionRef={actionRef}
                rowKey="id"
                toolBarRender={() => [
                    <Button
                        key="add"
                        type="primary"
                        onClick={() => {
                            history.push('/designer');
                        }}
                    >
                        添加
                    </Button>,
                ]}
                search={false}
                request={async (params) => {
                    const { current, pageSize } = params;
                    delete params.current;
                    delete params.pageSize;
                    const skipCount = (current! - 1) * pageSize!;
                    const result = await getWorkflowDefinitionList({
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

            <ModalForm
                layout="horizontal"
                preserve={false}
                labelCol={{ span: 5 }}
                width={600}
                labelWrap
                title={editModalTitle}
                visible={editModalVisible}
                modalProps={{ destroyOnClose: true, maskClosable: false }}
                onVisibleChange={setEditModalVisible}
                initialValues={editModalData}
                onFinish={async (value) => {
                    const success = await handleEdit(editModalDataId!, value);

                    if (success) {
                        setEditModalVisible(false);
                        actionRef.current?.reload();
                    }
                }}
            >
                <EditFormItems />
            </ModalForm>
        </PageContainer>
    );
};

export default Index;
