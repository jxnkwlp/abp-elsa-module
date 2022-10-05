import { getPermissions, updatePermissions } from '@/services/Permissions';
import { createRole, deleteRole, getRoleList, updateRole } from '@/services/Role';
import type { API } from '@/services/typings';
import type { ActionType, ProColumnType } from '@ant-design/pro-components';
import {
    ModalForm,
    PageContainer,
    ProForm,
    ProFormSwitch,
    ProFormText,
    ProTable,
} from '@ant-design/pro-components';
import { Button, Checkbox, Col, Collapse, message, Popconfirm, Row } from 'antd';
import React, { useRef, useState } from 'react';
import { formatMessage, useIntl } from 'umi';

const handleAdd = async (data: any) => {
    const response = await createRole(data);
    if (response) {
        message.success(formatMessage({ id: 'common.dict.created.success' }));
        return true;
    }
    return false;
};

const handleEdit = async (id: string, data: any) => {
    const response = await updateRole(id, data);
    if (response) {
        message.success(formatMessage({ id: 'common.dict.modified.success' }));
        return true;
    }
    return false;
};

const handleDelete = async (id: string) => {
    const response = await deleteRole(id);
    if (response?.response?.ok) {
        message.success(formatMessage({ id: 'common.dict.deleted.success' }));
        return true;
    }
    return false;
};

const Index: React.FC = () => {
    const actionRef = useRef<ActionType>();
    const intl = useIntl();

    const [editModalVisible, setEditModalVisible] = useState<boolean>(false);
    const [editModalTitle, setEditModalTitle] = useState<string>('');
    const [editModalData, setEditModalData] = useState<API.IdentityRole>();
    const [editModalDataId, setEditModalDataId] = useState<string>();

    const [permissionModalVisible, setPermissionModalVisible] = useState<boolean>(false);
    const [permissionEditData, setPermissionEditData] = useState<API.GetPermissionListResult>();
    const [permissionData, setPermissioData] = useState<{ name: string; isGranted: boolean }[]>([]);

    const updateRolePermission = async (grantedNames: string[]) => {
        const data = [...permissionData];
        data.forEach((item) => {
            item.isGranted = grantedNames.indexOf(item.name) >= 0;
        });
        const result = await updatePermissions(
            {
                providerKey: editModalData.name,
                providerName: 'R',
            },
            {
                permissions: data,
            },
        );

        if (result?.response?.ok) {
            message.success(intl.formatMessage({ id: 'common.dict.modified.success' }));
            return true;
        }

        return false;
    };

    const loadRolePermission = async (name: string) => {
        const loading = message.loading(intl.formatMessage({ id: 'common.dict.loading' }));
        //
        const result = await getPermissions({ providerKey: name, providerName: 'R' });
        setPermissionEditData(result);
        //
        const permissions = (result?.groups ?? [])
            .map((x) => {
                return (x.permissions ?? []).map((p) => {
                    return {
                        name: p.name,
                        isGranted: p.isGranted,
                    };
                });
            })
            .flatMap((x) => x);
        setPermissioData(permissions);

        loading();
    };

    const columns: ProColumnType<API.IdentityRole>[] = [
        {
            valueType: 'index',
            title: '#',
            width: 50,
        },
        {
            dataIndex: 'filter',
            title: intl.formatMessage({ id: 'page.role.field.name' }),
            hideInTable: true,
            hideInSetting: true,
        },
        {
            dataIndex: 'name',
            title: intl.formatMessage({ id: 'page.role.field.name' }),
            copyable: true,
            hideInSearch: true,
        },
        {
            dataIndex: 'isDefault',
            title: intl.formatMessage({ id: 'page.role.field.isDefault' }),
            search: false,
            align: 'center',
            valueEnum: {
                true: { text: 'Y' },
                false: { text: 'N' },
            },
        },
        {
            dataIndex: 'isPublic',
            title: intl.formatMessage({ id: 'page.role.field.isPublic' }),
            search: false,
            align: 'center',
            valueEnum: {
                true: { text: 'Y' },
                false: { text: 'N' },
            },
        },
        {
            dataIndex: 'isStatic',
            title: intl.formatMessage({ id: 'page.role.field.isStatic' }),
            search: false,
            align: 'center',
            valueEnum: {
                true: { text: 'Y' },
                false: { text: 'N' },
            },
        },
        // {
        //     dataIndex: 'creationTime',
        //     title: intl.formatMessage({ id: 'common.dict.lastModificationTime' }),
        //     valueType: 'dateTime',
        //     search: false,
        //     renderText: (_, record) => {
        //         return record.lastModificationTime ?? record.creationTime;
        //     },
        // },
        {
            title: intl.formatMessage({ id: 'common.dict.table-action' }),
            valueType: 'option',
            align: 'center',
            width: 160,
            render: (text, record, _, action) => [
                !record.isStatic && (
                    <a
                        key="edit"
                        onClick={() => {
                            setEditModalData(record);
                            setEditModalDataId(record.id);
                            setEditModalVisible(true);
                            setEditModalTitle(
                                `${intl.formatMessage({ id: 'common.dict.edit' })} - ${
                                    record.name
                                }`,
                            );
                        }}
                    >
                        {intl.formatMessage({ id: 'common.dict.edit' })}
                    </a>
                ),
                <a
                    key="permission"
                    onClick={async () => {
                        setEditModalData(record);
                        setEditModalDataId(record.id);
                        await loadRolePermission(record.name);
                        setPermissionModalVisible(true);
                    }}
                >
                    {intl.formatMessage({ id: 'page.role.permissions' })}
                </a>,
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
                </Popconfirm>,
            ],
        },
    ];

    return (
        <PageContainer>
            <ProTable<API.IdentityRole>
                columns={columns}
                actionRef={actionRef}
                rowKey="id"
                toolBarRender={() => [
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
                    </Button>,
                ]}
                request={async (params) => {
                    const { current, pageSize } = params;
                    delete params.current;
                    delete params.pageSize;
                    const skipCount = (current! - 1) * pageSize!;
                    const result = await getRoleList({
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
                    return success;
                }}
                layout="horizontal"
                labelCol={{ span: 4 }}
            >
                <ProFormText
                    rules={[{ required: true }, { max: 16 }]}
                    name="name"
                    label={intl.formatMessage({ id: 'page.role.field.name' })}
                />
                <ProFormSwitch
                    name="isDefault"
                    label={intl.formatMessage({ id: 'page.role.field.isDefault' })}
                />
                <ProFormSwitch
                    name="isPublic"
                    label={intl.formatMessage({ id: 'page.role.field.isPublic' })}
                />
            </ModalForm>
            {/*  */}
            <ModalForm
                visible={permissionModalVisible}
                onVisibleChange={setPermissionModalVisible}
                title={`${intl.formatMessage({ id: 'page.role.permissions' })} - ${
                    editModalData?.name
                }`}
                width="600px"
                modalProps={{ destroyOnClose: true, maskClosable: false }}
                preserve={false}
                initialValues={{
                    name: permissionData
                        .filter((x) => x.isGranted)
                        .map((x) => {
                            return x.name;
                        }),
                }}
                onFinish={async (value) => {
                    const grantedNames: string[] = value.name ?? [];
                    return await updateRolePermission(grantedNames);
                }}
            >
                <ProForm.Item name="name">
                    <Checkbox.Group style={{ width: '100%' }}>
                        <Collapse>
                            {(permissionEditData?.groups ?? []).map((group) => {
                                return (
                                    <Collapse.Panel header={group.displayName} key={group.name}>
                                        <Row>
                                            {(group.permissions ?? []).map((p) => {
                                                return (
                                                    <Col
                                                        key={p.name}
                                                        span={20}
                                                        offset={p.parentName ? 1 : 0}
                                                    >
                                                        <Checkbox
                                                            style={{ width: '100%' }}
                                                            value={p.name}
                                                            checked={p.isGranted}
                                                        >
                                                            {p.displayName}
                                                        </Checkbox>
                                                    </Col>
                                                );
                                            })}
                                        </Row>
                                    </Collapse.Panel>
                                );
                            })}
                        </Collapse>
                    </Checkbox.Group>
                </ProForm.Item>
            </ModalForm>
        </PageContainer>
    );
};

export default Index;
