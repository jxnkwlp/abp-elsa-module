import TransferFormInput from '@/components/TransferFormInput';
import { getAllRoleList } from '@/services/Role';
import type { API } from '@/services/typings';
import { getUserList } from '@/services/User';
import { getWorkflowDefinitionList } from '@/services/WorkflowDefinition';
import {
    createWorkflowGroup,
    deleteWorkflowGroup,
    getWorkflowGroup,
    getWorkflowGroupList,
    updateWorkflowGroup,
    workflowGroupSetUsers,
    workflowGroupSetWorkflowflows,
} from '@/services/WorkflowGroup';
import type { ActionType, ProColumnType } from '@ant-design/pro-components';
import {
    ModalForm,
    PageContainer,
    ProForm,
    ProFormSelect,
    ProFormText,
    ProFormTextArea,
    ProTable,
    TableDropdown,
} from '@ant-design/pro-components';
import { Button, message, Modal } from 'antd';
import React, { useRef, useState } from 'react';
import { formatMessage, useAccess, useIntl } from 'umi';

const handleAdd = async (data: any) => {
    const response = await createWorkflowGroup(data);
    if (response) {
        message.success(formatMessage({ id: 'common.dict.created.success' }));
        return true;
    }
    return false;
};

const handleEdit = async (id: string, data: any) => {
    const response = await updateWorkflowGroup(id, data);
    if (response) {
        message.success(formatMessage({ id: 'common.dict.modified.success' }));
        return true;
    }
    return false;
};

const handleDelete = async (id: string) => {
    const response = await deleteWorkflowGroup(id);
    if (response?.response?.ok) {
        message.success(formatMessage({ id: 'common.dict.deleted.success' }));
        return true;
    }
    return false;
};

const Index: React.FC = () => {
    const actionRef = useRef<ActionType>();
    const intl = useIntl();
    const access = useAccess();

    const [loading, setLoading] = useState<boolean>(false);

    const [editModalVisible, setEditModalVisible] = useState<boolean>(false);
    const [editModalTitle, setEditModalTitle] = useState<string>('');

    const [editModalData, setEditModalData] = useState<API.WorkflowGroup>();
    const [editModalDataId, setEditModalDataId] = useState<string>();

    const [usersModalVisible, setUsersModalVisible] = useState<boolean>(false);

    const [workflowsModalVisible, setWorkflowsModalVisible] = useState(false);

    const handleShowGroupWorkflow = async (record: API.WorkflowGroup) => {
        const loading = message.loading(intl.formatMessage({ id: 'common.dict.loading' }));
        setEditModalDataId(record.id);
        setEditModalData(record);

        const result = await getWorkflowGroup(record.id);
        if (result) {
            setEditModalData(result);
            setWorkflowsModalVisible(true);
        }
        loading();
    };

    const handleUpdateGroupWorkflows = async (workflowIds: string[]) => {
        const result = await workflowGroupSetWorkflowflows(editModalDataId!, {
            workflowIds: workflowIds,
        });
        if (result) {
            return true;
        }
        return false;
    };

    const handleShowGroupUsers = async (record: API.WorkflowGroup) => {
        const loading = message.loading(intl.formatMessage({ id: 'common.dict.loading' }));
        setEditModalDataId(record.id);
        setEditModalData(record);

        const result = await getWorkflowGroup(record.id);
        if (result) {
            setEditModalData(result);
            setUsersModalVisible(true);
        }
        loading();
    };

    const handleUpdateGroupUsers = async (userIds: string[]) => {
        const result = await workflowGroupSetUsers(editModalDataId!, { userIds: userIds });
        if (result) {
            return true;
        }
        return false;
    };

    const handleLoadGroup = async (record: API.WorkflowGroup) => {
        const loading = message.loading(intl.formatMessage({ id: 'common.dict.loading' }));
        setEditModalDataId(record.id);
        const result = await getWorkflowGroup(record.id);
        loading();
        setEditModalData(result);
        setEditModalVisible(true);
        setEditModalTitle(`${intl.formatMessage({ id: 'common.dict.edit' })} - ${record.name}`);
    };

    const columns: ProColumnType<API.WorkflowGroup>[] = [
        {
            dataIndex: 'name',
            title: intl.formatMessage({ id: 'page.workflowgroup.field.name' }),
            copyable: true,
        },
        {
            dataIndex: 'description',
            title: intl.formatMessage({ id: 'page.workflowgroup.field.description' }),
        },
        {
            dataIndex: 'roleName',
            title: intl.formatMessage({ id: 'page.workflowgroup.field.roleName' }),
        },
        {
            title: intl.formatMessage({ id: 'common.dict.table-action' }),
            valueType: 'option',
            align: 'center',
            width: 120,
            render: (text, record, _, action) => [
                access['ElsaWorkflow.WorkflowGroups.Update'] && (
                    <a
                        key="edit"
                        onClick={() => {
                            handleLoadGroup(record);
                        }}
                    >
                        {intl.formatMessage({ id: 'common.dict.edit' })}
                    </a>
                ),
                <TableDropdown
                    key="more"
                    menus={[
                        {
                            key: 'users',
                            name: intl.formatMessage({ id: 'page.workflowgroup.manageUser' }),
                            disabled: !access['ElsaWorkflow.WorkflowGroups.ManagePermissions'],
                        },
                        {
                            key: 'workflows',
                            name: intl.formatMessage({ id: 'page.workflowgroup.manageWorkflows' }),
                            disabled: !access['ElsaWorkflow.WorkflowGroups.ManagePermissions'],
                        },
                        {
                            type: 'divider',
                        },
                        {
                            key: 'delete',
                            name: intl.formatMessage({ id: 'common.dict.delete' }),
                            disabled: !access['ElsaWorkflow.WorkflowGroups.Delete'],
                            danger: true,
                        },
                    ]}
                    onSelect={async (key) => {
                        if (key == 'delete') {
                            Modal.confirm({
                                title: intl.formatMessage({ id: 'common.dict.confirm' }),
                                content: intl.formatMessage({ id: 'common.dict.delete.confirm' }),
                                onOk: async () => {
                                    if (await handleDelete(record.id)) {
                                        action?.reload();
                                    }
                                },
                            });
                        } else if (key == 'users') {
                            await handleShowGroupUsers(record);
                        } else if (key == 'workflows') {
                            await handleShowGroupWorkflow(record);
                        }
                    }}
                />,
            ],
        },
    ];

    return (
        <PageContainer>
            <ProTable<API.WorkflowGroup>
                columns={columns}
                actionRef={actionRef}
                search={false}
                rowKey="id"
                toolBarRender={() => [
                    access['ElsaWorkflow.WorkflowGroups.Create'] && (
                        <Button
                            key="add"
                            type="primary"
                            onClick={() => {
                                setEditModalData(undefined);
                                setEditModalDataId('');
                                setEditModalVisible(true);
                                setEditModalTitle(intl.formatMessage({ id: 'common.dict.create' }));
                            }}
                        >
                            {intl.formatMessage({ id: 'common.dict.create' })}
                        </Button>
                    ),
                ]}
                request={async (params) => {
                    const { current, pageSize } = params;
                    delete params.current;
                    delete params.pageSize;
                    const skipCount = (current! - 1) * pageSize!;
                    const result = await getWorkflowGroupList({
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
            {/* create & edit */}
            <ModalForm
                title={editModalTitle}
                width="600px"
                visible={editModalVisible}
                modalProps={{ destroyOnClose: true, maskClosable: false }}
                onVisibleChange={setEditModalVisible}
                initialValues={editModalData}
                onFinish={async (value) => {
                    let success = false;
                    // @ts-nocheck
                    const data = { ...value };
                    if (editModalDataId) {
                        success = await handleEdit(editModalDataId, data);
                    } else {
                        success = await handleAdd(data);
                    }

                    if (success) {
                        setEditModalVisible(false);
                        actionRef.current?.reload();
                    }
                    return success;
                }}
                layout="horizontal"
                labelCol={{ span: 4 }}
            >
                <ProFormText
                    rules={[{ required: true }, { max: 64 }]}
                    name="name"
                    label={intl.formatMessage({ id: 'page.workflowgroup.field.name' })}
                />
                <ProFormSelect
                    label={intl.formatMessage({ id: 'page.workflowgroup.field.role' })}
                    name="roleId"
                    rules={[{ required: true }]}
                    request={async () => {
                        const result = await getAllRoleList({});
                        return (result?.items ?? []).map((x) => {
                            return {
                                label: x.name,
                                value: x.id,
                            };
                        });
                    }}
                />
                <ProFormSelect
                    label={intl.formatMessage({ id: 'page.workflowgroup.field.users' })}
                    name="userIds"
                    rules={[{ required: true }]}
                    request={async (q) => {
                        const result = await getUserList({
                            filter: q.keyWords ?? '',
                            sorting: 'name',
                            maxResultCount: 100,
                        });
                        return (result?.items ?? []).map((x) => {
                            return {
                                label: `${x.email}(${x.surname} ${x.name})`,
                                value: x.id,
                            };
                        });
                    }}
                    fieldProps={{ showSearch: true, mode: 'multiple' }}
                />
                <ProFormSelect
                    label={intl.formatMessage({ id: 'page.workflowgroup.field.workflow' })}
                    name="workflowIds"
                    rules={[{ required: true }]}
                    request={async (q) => {
                        const result = await getWorkflowDefinitionList({
                            filter: q.keyWords ?? '',
                            sorting: 'name',
                            maxResultCount: 100,
                        });
                        return (result?.items ?? []).map((x) => {
                            return {
                                label: `${x.displayName}(${x.name})`,
                                value: x.id,
                            };
                        });
                    }}
                    fieldProps={{ showSearch: true, mode: 'multiple' }}
                />
                <ProFormTextArea
                    rules={[]}
                    name="description"
                    label={intl.formatMessage({ id: 'page.workflowgroup.field.description' })}
                />
            </ModalForm>
            {/* users */}
            <ModalForm
                visible={usersModalVisible}
                onVisibleChange={setUsersModalVisible}
                title={`${intl.formatMessage({ id: 'page.workflowgroup.manageUser' })} - ${
                    editModalData?.name
                }`}
                width={660}
                modalProps={{ destroyOnClose: true, maskClosable: false }}
                initialValues={editModalData}
                onFinish={async (value) => {
                    return await handleUpdateGroupUsers(value.userIds ?? []);
                }}
            >
                <ProForm.Item name="userIds">
                    <TransferFormInput<API.IdentityUser>
                        request={async () => {
                            // todo
                            const result = await getUserList({
                                maxResultCount: 1000,
                                sorting: 'username',
                            });
                            return result?.items ?? [];
                        }}
                        render={(item) => item.userName ?? item.id}
                        rowKey={(item) => item.id}
                        showSearch
                        height={300}
                        panelWidth={290}
                    />
                </ProForm.Item>
            </ModalForm>
            {/* workflows */}
            <ModalForm
                visible={workflowsModalVisible}
                onVisibleChange={setWorkflowsModalVisible}
                title={`${intl.formatMessage({ id: 'page.workflowgroup.manageWorkflows' })} - ${
                    editModalData?.name
                }`}
                width={660}
                modalProps={{ destroyOnClose: true, maskClosable: false }}
                initialValues={editModalData}
                onFinish={async (value) => {
                    return await handleUpdateGroupWorkflows(value.workflowIds ?? []);
                }}
            >
                <ProForm.Item name="workflowIds">
                    <TransferFormInput<API.WorkflowDefinition>
                        request={async () => {
                            // todo
                            const result = await getWorkflowDefinitionList({
                                maxResultCount: 1000,
                                sorting: 'name',
                            });
                            return result?.items ?? [];
                        }}
                        render={(item) => item.name ?? item.id}
                        rowKey={(item) => item.id}
                        showSearch
                        height={300}
                        panelWidth={290}
                    />
                </ProForm.Item>
            </ModalForm>
        </PageContainer>
    );
};

export default Index;
