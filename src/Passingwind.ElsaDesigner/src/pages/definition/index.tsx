import {
    deleteWorkflowDefinition,
    getWorkflowDefinitionList,
    getWorkflowDefinitionVersion,
    updateWorkflowDefinitionDefinition,
    workflowDefinitionDispatch,
    workflowDefinitionPublish,
    workflowDefinitionUnPublish,
} from '@/services/WorkflowDefinition';
import { ModalForm, ProFormSelect, ProFormText, ProFormTextArea } from '@ant-design/pro-form';
import { PageContainer } from '@ant-design/pro-layout';
import type { ActionType, ProColumnType } from '@ant-design/pro-table';
import { TableDropdown } from '@ant-design/pro-table';
import ProTable from '@ant-design/pro-table';
import { Button, Form, message, Modal } from 'antd';
import React, { useRef, useState } from 'react';
import { useHistory } from 'umi';
import EditFormItems from './edit-form-items';
import { useForm } from 'antd/lib/form/Form';

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

    const [actionRow, setActionRow] = useState<API.WorkflowDefinition>();

    const [editModalVisible, setEditModalVisible] = useState<boolean>(false);
    const [editModalTitle, setEditModalTitle] = useState<string>('');
    const [editModalData, setEditModalData] = useState<API.WorkflowDefinition>();
    const [editModalDataId, setEditModalDataId] = useState<string>();

    const [dispatchFormVisible, setDispatchFormVisible] = useState<boolean>(false);
    const [dispatchId, setDispatchId] = useState<string>();

    // const [searchKey, setSearchKey] = useState<string>();

    const [editForm] = Form.useForm();

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
                    key="dispatch"
                    onClick={() => {
                        setDispatchId(record.id);
                        setActionRow(record);
                        setDispatchFormVisible(true);
                    }}
                >
                    Dispatch
                </a>,
                <a
                    key="edit"
                    onClick={() => {
                        setEditModalData(
                            Object.assign({}, record, {
                                variablesString: JSON.stringify(record.variables ?? {}),
                            }),
                        );
                        setEditModalDataId(record.id);
                        setEditModalVisible(true);
                        setEditModalTitle(`Edit ${record.name}`);
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

            {/*  */}
            <ModalForm
                layout="horizontal"
                form={editForm}
                preserve={false}
                labelCol={{ span: 5 }}
                width={650}
                labelWrap
                title={editModalTitle}
                visible={editModalVisible}
                modalProps={{ destroyOnClose: true, maskClosable: false }}
                onVisibleChange={setEditModalVisible}
                initialValues={editModalData}
                onValuesChange={(value) => {
                    if (value.displayName) {
                        editForm.setFieldsValue({
                            name: value.displayName?.replaceAll(' ', '-'),
                        });
                    }
                }}
                onFinish={async (value) => {
                    const success = await handleEdit(
                        editModalDataId!,
                        Object.assign(
                            {},
                            editModalData,
                            { variables: JSON.parse(value.variablesString ?? '{}') },
                            value,
                        ),
                    );

                    if (success) {
                        setEditModalVisible(false);
                        actionRef.current?.reload();
                    }
                }}
            >
                <EditFormItems />
            </ModalForm>

            {/*  */}
            <ModalForm
                layout="horizontal"
                preserve={false}
                labelCol={{ span: 5 }}
                width={600}
                labelWrap
                title={'Dispatch'}
                visible={dispatchFormVisible}
                modalProps={{ destroyOnClose: true, maskClosable: false }}
                onVisibleChange={setDispatchFormVisible}
                onFinish={async (value) => {
                    const result = await workflowDefinitionDispatch(dispatchId!, value);
                    if (result?.workflowInstanceId) {
                        message.success(
                            'Dispatch success. Instance Id: ' + result.workflowInstanceId,
                        );
                        return true;
                    }
                    return false;
                }}
            >
                <ProFormSelect
                    label="Activity"
                    name="activityId"
                    request={async () => {
                        const result = await getWorkflowDefinitionVersion(
                            dispatchId!,
                            actionRow?.publishedVersion ?? 1,
                        );
                        return (result.activities ?? []).map((x) => {
                            return {
                                label: `${x.displayName} (${x.name})`,
                                value: x.activityId,
                            };
                        });
                    }}
                />
                <ProFormText label="Correlation Id" name="correlationId" rules={[{ max: 36 }]} />
                <ProFormText label="Context Id" name="contextId" rules={[{ max: 36 }]} />
                <ProFormTextArea
                    label="Input"
                    name="input"
                    fieldProps={{
                        autoSize: {
                            minRows: 2,
                            maxRows: 10,
                        },
                    }}
                />
            </ModalForm>
        </PageContainer>
    );
};

export default Index;
