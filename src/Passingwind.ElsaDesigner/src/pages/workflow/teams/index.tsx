import TransferFormInput from '@/components/TransferFormInput';
import { getAllRoleList } from '@/services/Role';
import type { API } from '@/services/typings';
import { getUserList } from '@/services/User';
import { formatUserName } from '@/services/utils';
import { getWorkflowDefinitionList } from '@/services/WorkflowDefinition';
import {
    createWorkflowTeam,
    deleteWorkflowTeam,
    deleteWorkflowTeamRoleScope,
    getWorkflowTeam,
    getWorkflowTeamList,
    getWorkflowTeamRoleScopes,
    updateWorkflowTeam,
    workflowTeamSetRoleScope,
    workflowTeamSetUsers,
} from '@/services/WorkflowTeam';
import type { ActionType, ProColumnType } from '@ant-design/pro-components';
import {
    ModalForm,
    PageContainer,
    ProForm,
    ProFormSelect,
    ProFormText,
    ProFormTextArea,
    ProTable
} from '@ant-design/pro-components';
import { Button, message, Modal, Popconfirm, Space, Table, Tabs, Tag } from 'antd';
import React, { useRef, useState } from 'react';
import { formatMessage, useAccess, useIntl } from 'umi';

const handleAdd = async (data: any) => {
    const response = await createWorkflowTeam(data);
    if (response) {
        message.success(formatMessage({ id: 'common.dict.created.success' }));
        return true;
    }
    return false;
};

const handleEdit = async (id: string, data: any) => {
    const response = await updateWorkflowTeam(id, data);
    if (response) {
        message.success(formatMessage({ id: 'common.dict.modified.success' }));
        return true;
    }
    return false;
};

const handleDelete = async (id: string) => {
    const response = await deleteWorkflowTeam(id);
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

    const [editModalData, setEditModalData] = useState<API.WorkflowTeam>();
    const [editModalDataId, setEditModalDataId] = useState<string>();

    const [settingsModalVisible, setSettingsModalVisible] = useState<boolean>(false);
    const [settingsTabKey, setSettingsTabKey] = useState('users');

    const [roleScopeModalVisible, setRoleScopeModalVisible] = useState(false);
    const [roleScopeList, setRoleScopeList] = useState<API.WorkflowTeamRoleScope[]>();
    const [editRoleScopeData, setEditRoleScopeData] =
        useState<{ roleName: string; workflowIds: string[] }>();

    const loadRoleScopes = async (id: string) => {
        const result = await getWorkflowTeamRoleScopes(id);
        setRoleScopeList(result?.items ?? []);
    };

    const handleShowSettings = async (record: API.WorkflowTeam) => {
        const loading = message.loading(intl.formatMessage({ id: 'common.dict.loading' }));
        setEditModalDataId(record.id);
        setEditModalData(record);

        const result = await getWorkflowTeam(record.id);
        await loadRoleScopes(record.id);

        if (result) {
            setEditModalData(result);
            setSettingsModalVisible(true);
        }
        loading();
    };

    const handleDeleteRoleScopes = async (roleName: string) => {
        const result = await deleteWorkflowTeamRoleScope(editModalDataId!, roleName);
        if (result?.response?.ok) {
            message.success(formatMessage({ id: 'common.dict.delete.success' }));
            await loadRoleScopes(editModalDataId!);
        }
    };

    const handleCreateOrUpdateRoleScope = async (roleName: string, workflowIds: string[]) => {
        const result = await workflowTeamSetRoleScope(editModalDataId!, {
            roleName: roleName,
            items: workflowIds?.map((x) => {
                return { providerName: 'Workflow', providerValue: x };
            }),
        });

        if (result) {
            await loadRoleScopes(editModalDataId!);
            message.success(formatMessage({ id: 'common.dict.save.success' }));
            return true;
        }

        return false;
    };

    const handleUpdateUsers = async (userIds: string[]) => {
        const result = await workflowTeamSetUsers(editModalDataId!, { userIds: userIds });
        if (result) {
            message.success(formatMessage({ id: 'common.dict.save.success' }));
            return true;
        }
        return false;
    };

    const loadList = async (record: API.WorkflowTeam) => {
        const loading = message.loading(intl.formatMessage({ id: 'common.dict.loading' }));
        setEditModalDataId(record.id);
        const result = await getWorkflowTeam(record.id);
        loading();
        setEditModalData(result);
        setEditModalVisible(true);
        setEditModalTitle(`${intl.formatMessage({ id: 'common.dict.edit' })} - ${record.name}`);
    };

    const columns: ProColumnType<API.WorkflowTeam>[] = [
        {
            dataIndex: 'name',
            title: intl.formatMessage({ id: 'page.workflowTeam.field.name' }),
            copyable: true,
            width: '30%',
        },
        {
            dataIndex: 'description',
            title: intl.formatMessage({ id: 'page.workflowTeam.field.description' }),
        },
        {
            title: intl.formatMessage({ id: 'common.dict.table-action' }),
            valueType: 'option',
            align: 'center',
            width: 150,
            render: (text, record, _, action) => [
                access['ElsaWorkflow.WorkflowTeams.Update'] && (
                    <a
                        key="edit"
                        onClick={() => {
                            loadList(record);
                        }}
                    >
                        {intl.formatMessage({ id: 'common.dict.edit' })}
                    </a>
                ),
                access['ElsaWorkflow.WorkflowTeams.ManagePermissions'] && (
                    <a
                        key="manage"
                        onClick={() => {
                            handleShowSettings(record);
                        }}
                    >
                        {intl.formatMessage({ id: 'page.workflowTeam.manage' })}
                    </a>
                ),
                access['ElsaWorkflow.WorkflowTeams.Delete'] && (
                    <a
                        key="delete"
                        onClick={() => {
                            Modal.confirm({
                                title: intl.formatMessage({ id: 'common.dict.confirm' }),
                                content: intl.formatMessage({ id: 'common.dict.delete.confirm' }),
                                onOk: async () => {
                                    if (await handleDelete(record.id)) {
                                        action?.reload();
                                    }
                                },
                            });
                        }}
                    >
                        {intl.formatMessage({ id: 'common.dict.delete' })}
                    </a>
                ),
            ],
        },
    ];

    return (
        <PageContainer>
            <ProTable<API.WorkflowTeam>
                columns={columns}
                actionRef={actionRef}
                search={false}
                rowKey="id"
                toolBarRender={() => [
                    access['ElsaWorkflow.WorkflowTeams.Create'] && (
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
                    const result = await getWorkflowTeamList({
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
                    label={intl.formatMessage({ id: 'page.workflowTeam.field.name' })}
                />

                <ProFormSelect
                    label={intl.formatMessage({ id: 'page.workflowTeam.field.users' })}
                    name="userIds"
                    rules={[{ required: true }]}
                    request={async (q) => {
                        const result = await getUserList({
                            filter: q.keyWords ?? '',
                            sorting: 'name',
                            maxResultCount: 20,
                        });
                        return (result?.items ?? []).map((x) => {
                            return {
                                label: `${x.email}(${formatUserName(x)})`,
                                value: x.id,
                            };
                        });
                    }}
                    fieldProps={{ showSearch: true, mode: 'multiple' }}
                />
                <ProFormTextArea
                    rules={[]}
                    name="description"
                    label={intl.formatMessage({ id: 'page.workflowTeam.field.description' })}
                />
            </ModalForm>
            {/* settings */}
            <Modal
                open={settingsModalVisible}
                onCancel={() => setSettingsModalVisible(false)}
                destroyOnClose
                maskClosable={false}
                footer={false}
                title={`${intl.formatMessage({ id: 'page.workflowTeam.manage' })} - ${
                    editModalData?.name
                }`}
                width={660}
            >
                <Tabs
                    items={[
                        {
                            key: 'users',
                            label: intl.formatMessage({
                                id: 'page.workflowTeam.users',
                            }),
                            children: (
                                <ProForm<{ userIds: string[] }>
                                    initialValues={{ userIds: editModalData?.userIds ?? [] }}
                                    preserve={false}
                                    onFinish={(values) => {
                                        return handleUpdateUsers(values.userIds);
                                    }}
                                    submitter={{
                                        render: (props, doms) => {
                                            return (
                                                <div style={{ textAlign: 'right' }}>{doms[1]}</div>
                                            );
                                        },
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
                                </ProForm>
                            ),
                        },
                        {
                            key: 'roles',
                            label: intl.formatMessage({
                                id: 'page.workflowTeam.roleScopes',
                            }),
                            children: (
                                <Table
                                    rowKey="id"
                                    size="small"
                                    dataSource={roleScopeList}
                                    pagination={false}
                                    columns={[
                                        {
                                            dataIndex: 'roleName',
                                            title: intl.formatMessage({
                                                id: 'page.workflowTeam.field.role',
                                            }),
                                            render: (value, record) => (
                                                <Space>
                                                    <span>{value}</span>
                                                    <Tag>{record.values?.length ?? 0}</Tag>
                                                </Space>
                                            ),
                                        },
                                        {
                                            title: intl.formatMessage({
                                                id: 'common.dict.table-action',
                                            }),
                                            align: 'center',
                                            width: 120,
                                            render: (text, record) => (
                                                <Space>
                                                    <a
                                                        key="edit"
                                                        onClick={() => {
                                                            setEditRoleScopeData({
                                                                roleName: record.roleName!,
                                                                workflowIds: (
                                                                    record.values ?? []
                                                                ).map((x) => {
                                                                    return x.providerValue;
                                                                }),
                                                            });
                                                            setRoleScopeModalVisible(true);
                                                        }}
                                                    >
                                                        {intl.formatMessage({
                                                            id: 'common.dict.edit',
                                                        })}
                                                    </a>
                                                    <Popconfirm
                                                        title={intl.formatMessage({
                                                            id: 'common.dict.delete.confirm',
                                                        })}
                                                        onConfirm={async () => {
                                                            return await handleDeleteRoleScopes(
                                                                record.roleName!,
                                                            );
                                                        }}
                                                    >
                                                        <a key="delete" onClick={() => {}}>
                                                            {intl.formatMessage({
                                                                id: 'common.dict.delete',
                                                            })}
                                                        </a>
                                                    </Popconfirm>
                                                </Space>
                                            ),
                                        },
                                    ]}
                                />
                            ),
                        },
                    ]}
                    onChange={setSettingsTabKey}
                    tabBarExtraContent={
                        settingsTabKey == 'roles' && (
                            <Button
                                type="primary"
                                onClick={() => {
                                    setEditRoleScopeData(undefined);
                                    setRoleScopeModalVisible(true);
                                }}
                            >
                                {intl.formatMessage({ id: 'common.dict.create' })}
                            </Button>
                        )
                    }
                />
                {/*  */}
            </Modal>
            {/* role scope form */}
            <ModalForm<{ roleName: string; workflowIds: string[] }>
                visible={roleScopeModalVisible}
                onVisibleChange={setRoleScopeModalVisible}
                title={intl.formatMessage({ id: 'page.workflowTeam.roleScopes' })}
                width={660}
                modalProps={{ destroyOnClose: true, maskClosable: false }}
                initialValues={editRoleScopeData}
                onFinish={async (value) => {
                    return await handleCreateOrUpdateRoleScope(value.roleName, value.workflowIds);
                }}
            >
                <ProFormSelect
                    label={intl.formatMessage({ id: 'page.workflowTeam.field.role' })}
                    name="roleName"
                    rules={[{ required: true }]}
                    request={async () => {
                        const result = await getAllRoleList({});
                        return (result?.items ?? []).map((x) => {
                            return {
                                label: x.name,
                                value: x.name,
                            };
                        });
                    }}
                />
                {/* <ProFormSelect
                    name="workflowTeamIds"
                    label={intl.formatMessage({ id: 'page.workflowTeam.field.workflowTeam' })}
                    mode="multiple"
                    options={[]}
                /> */}
                <ProForm.Item
                    name="workflowIds"
                    label={intl.formatMessage({ id: 'page.workflowTeam.field.workflow' })}
                    rules={[{ required: true }]}
                >
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
