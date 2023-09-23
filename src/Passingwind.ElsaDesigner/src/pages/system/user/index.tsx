import type { API } from '@/services/typings';
import {
    createUser,
    deleteUser,
    getUserAssignableRoles,
    getUserList,
    getUserRoles,
    updateUser,
    updateUserRoles,
} from '@/services/User';
import { formatUserName } from '@/services/utils';
import { CheckCircleTwoTone, CloseCircleOutlined } from '@ant-design/icons';
import type { ActionType, ProColumnType } from '@ant-design/pro-components';
import {
    ModalForm,
    PageContainer,
    ProFormCheckbox,
    ProFormSelect,
    ProFormSwitch,
    ProFormText,
    ProTable,
} from '@ant-design/pro-components';
import { Button, message, Popconfirm, Space, Tag } from 'antd';
import moment from 'moment';
import React, { useRef, useState } from 'react';
import { formatMessage, useAccess, useIntl } from 'umi';

const handleAdd = async (data: any) => {
    const response = await createUser(data);
    if (response) {
        message.success(formatMessage({ id: 'common.dict.created.success' }));
        return true;
    }
    return false;
};

const handleEdit = async (id: string, data: any) => {
    const response = await updateUser(id, data);
    if (response) {
        message.success(formatMessage({ id: 'common.dict.modified.success' }));
        return true;
    }
    return false;
};

const handleDelete = async (id: string) => {
    const response = await deleteUser(id);
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

    const [editModalVisible, setEditModalVisible] = useState<boolean>(false);
    const [editModalTitle, setEditModalTitle] = useState<string>('');
    const [editModalData, setEditModalData] = useState<API.IdentityUser>();
    const [editModalDataId, setEditModalDataId] = useState<string>();

    const [editRolesModalVisible, setEditRolesModalVisible] = useState<boolean>(false);

    const columns: ProColumnType<API.IdentityUser>[] = [
        {
            dataIndex: 'filter',
            title: intl.formatMessage({ id: 'page.user.filter' }),
            hideInTable: true,
        },
        {
            dataIndex: 'userName',
            title: intl.formatMessage({ id: 'page.user.field.userName' }),
            search: false,
            copyable: true,
        },
        {
            dataIndex: 'email',
            title: intl.formatMessage({ id: 'page.user.field.email' }),
            search: false,
            copyable: true,
        },
        {
            dataIndex: 'name',
            title: intl.formatMessage({ id: 'page.user.field.name' }),
            search: false,
            copyable: true,
            renderText: (_, record) => {
                return formatUserName(record);
            },
        },
        {
            dataIndex: 'phoneNumber',
            title: intl.formatMessage({ id: 'page.user.field.phoneNumber' }),
            search: false,
            copyable: true,
        },
        {
            dataIndex: 'isActive',
            title: intl.formatMessage({ id: 'page.user.field.isActive' }),
            search: false,
            align: 'center',
            width: 100,
            valueEnum: {
                true: { text: <CheckCircleTwoTone /> },
                false: { text: <CloseCircleOutlined /> },
            },
        },
        {
            dataIndex: 'lockoutEnabled',
            title: intl.formatMessage({ id: 'page.user.field.lockoutEnabled' }),
            search: false,
            align: 'center',
            width: 100,
            // valueEnum: {
            //     true: { text: 'Y' },
            //     false: { text: 'N' },
            // },
            renderText: (_, record) => {
                if (_)
                    return (
                        <Space direction="vertical">
                            <span>
                                <CheckCircleTwoTone />
                            </span>
                            {record.lockoutEnd && <Tag color='warning'>{moment(record.lockoutEnd).format()}</Tag>}
                        </Space>
                    );
                else return <CloseCircleOutlined />;
            },
        },
        {
            dataIndex: 'creationTime',
            title: intl.formatMessage({ id: 'common.dict.lastModificationTime' }),
            valueType: 'dateTime',
            search: false,
            renderText: (_, record) => {
                return record.lastModificationTime ?? record.creationTime;
            },
        },
        {
            title: intl.formatMessage({ id: 'common.dict.table-action' }),
            valueType: 'option',
            align: 'center',
            width: 160,
            render: (text, record, _, action) => [
                access['AbpIdentity.Users.Update'] && (
                    <a
                        key="edit"
                        onClick={() => {
                            setEditModalData(record);
                            setEditModalDataId(record.id);
                            setEditModalVisible(true);
                            setEditModalTitle(
                                `${intl.formatMessage({ id: 'common.dict.edit' })} - ${
                                    record.userName
                                }`,
                            );
                        }}
                    >
                        {intl.formatMessage({ id: 'common.dict.edit' })}
                    </a>
                ),
                access['AbpIdentity.Users.Update'] && (
                    <a
                        key="roles"
                        onClick={async () => {
                            setEditModalData(record);
                            setEditModalDataId(record.id);
                            setEditRolesModalVisible(true);
                        }}
                    >
                        {intl.formatMessage({ id: 'page.user.role' })}
                    </a>
                ),
                access['AbpIdentity.Users.Delete'] && (
                    <Popconfirm
                        key="delete"
                        title={intl.formatMessage({ id: 'common.dict.delete.confirm' })}
                        onConfirm={async () => {
                            if (await handleDelete(record.id)) {
                                action?.reload();
                            }
                        }}
                    >
                        <a>{intl.formatMessage({ id: 'common.dict.delete' })}</a>
                    </Popconfirm>
                ),
            ],
        },
    ];

    return (
        <PageContainer>
            <ProTable<API.IdentityUser>
                columns={columns}
                actionRef={actionRef}
                search={{ labelWidth: 140 }}
                rowKey="id"
                toolBarRender={() => [
                    access['AbpIdentity.Users.Create'] && (
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
                    const result = await getUserList({
                        ...params,
                        sorting: 'creationTime desc',
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
                title={editModalTitle}
                width="600px"
                visible={editModalVisible}
                modalProps={{ destroyOnClose: true, maskClosable: false }}
                preserve={false}
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
                }}
                layout="horizontal"
                labelWrap
                labelCol={{ span: 5 }}
            >
                <ProFormText
                    rules={[{ required: true }, { max: 32 }]}
                    name="userName"
                    label={intl.formatMessage({ id: 'page.user.field.userName' })}
                    disabled={!!editModalDataId}
                />
                <ProFormText
                    rules={[
                        { required: true },
                        { max: 128 },
                        {
                            pattern: /\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*/,
                            message: 'invalid email address',
                        },
                    ]}
                    name="email"
                    label={intl.formatMessage({ id: 'page.user.field.email' })}
                />
                <ProFormText
                    rules={[{ required: false }, { max: 32 }]}
                    name="surname"
                    label={intl.formatMessage({ id: 'page.user.field.surname' })}
                />
                <ProFormText
                    rules={[{ required: false }, { max: 32 }]}
                    name="name"
                    label={intl.formatMessage({ id: 'page.user.field.name' })}
                />
                <ProFormText
                    rules={[{ required: false }, { max: 32 }]}
                    name="phoneNumber"
                    label={intl.formatMessage({ id: 'page.user.field.phoneNumber' })}
                />
                <ProFormSwitch
                    name="isActive"
                    label={intl.formatMessage({ id: 'page.user.field.isActive' })}
                />
                <ProFormSwitch
                    name="lockoutEnabled"
                    label={intl.formatMessage({ id: 'page.user.field.lockoutEnabled' })}
                />
                <ProFormText
                    rules={[{ required: !editModalDataId }, { max: 32 }, { min: 6 }]}
                    name="password"
                    label={intl.formatMessage({ id: 'page.user.field.password' })}
                />
                {!editModalDataId && (
                    <ProFormSelect
                        label={intl.formatMessage({ id: 'page.user.role' })}
                        name="roleNames"
                        mode="multiple"
                        showSearch
                        request={async (params) => {
                            const result = await getUserAssignableRoles({});
                            return (result?.items ?? []).map((x) => {
                                return {
                                    label: x.name,
                                    value: x.name,
                                };
                            });
                        }}
                    />
                )}
            </ModalForm>
            {/*  */}
            <ModalForm
                visible={editRolesModalVisible}
                onVisibleChange={setEditRolesModalVisible}
                title={`${intl.formatMessage({ id: 'page.user.role.update' })} - ${
                    editModalData?.userName
                }`}
                modalProps={{ destroyOnClose: true, maskClosable: false }}
                preserve={false}
                onFinish={async (values) => {
                    const result = await updateUserRoles(editModalDataId!, {
                        roleNames: values.roleNames as string[],
                    });

                    if (result?.response?.ok) {
                        message.success(intl.formatMessage({ id: 'common.dict.modified.success' }));
                    }

                    return result?.response?.ok;
                }}
                request={async () => {
                    const result = await getUserRoles(editModalDataId!);
                    return {
                        roleNames: (result.items ?? []).map((x) => {
                            return x.name;
                        }),
                    };
                }}
            >
                <ProFormCheckbox.Group
                    label=""
                    name="roleNames"
                    request={async () => {
                        const result = await getUserAssignableRoles({});
                        return (result?.items ?? []).map((x) => {
                            return {
                                label: x.name,
                                value: x.name,
                            };
                        });
                    }}
                />
            </ModalForm>
        </PageContainer>
    );
};

export default Index;
