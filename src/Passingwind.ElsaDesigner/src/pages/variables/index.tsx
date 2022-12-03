import {
    createGlobalVariable,
    deleteGlobalVariable,
    getGlobalVariableList,
    updateGlobalVariable,
} from '@/services/GlobalVariable';
import type { API } from '@/services/typings';
import type { ActionType, ProColumnType } from '@ant-design/pro-components';
import {
    ModalForm,
    PageContainer,
    ProFormSwitch,
    ProFormText,
    ProFormTextArea,
    ProTable,
} from '@ant-design/pro-components';
import { Button, message, Popconfirm } from 'antd';
import React, { useRef, useState } from 'react';
import { formatMessage, useIntl } from 'umi';

const handleAdd = async (data: any) => {
    const response = await createGlobalVariable(data);
    if (response) {
        message.success(formatMessage({ id: 'common.dict.created.success' }));
        return true;
    }
    return false;
};

const handleEdit = async (id: string, data: any) => {
    const response = await updateGlobalVariable(id, data);
    if (response) {
        message.success(formatMessage({ id: 'common.dict.modified.success' }));
        return true;
    }
    return false;
};

const handleDelete = async (id: string) => {
    const response = await deleteGlobalVariable(id);
    if (response?.response?.ok) {
        message.success(formatMessage({ id: 'common.dict.deleted.success' }));
        return true;
    }
    return false;
};

const Index: React.FC = () => {
    const tableActionRef = useRef<ActionType>();
    const intl = useIntl();

    const [editModalVisible, setEditModalVisible] = useState<boolean>(false);
    const [editModalTitle, setEditModalTitle] = useState<string>('');
    const [editModalData, setEditModalData] = useState<API.IdentityUser>();
    const [editModalDataId, setEditModalDataId] = useState<string>();

    const columns: ProColumnType<API.GlobalVariable>[] = [
        {
            dataIndex: 'filter',
            title: intl.formatMessage({ id: 'page.variable.field.key' }),
            hideInTable: true,
        },
        {
            dataIndex: 'key',
            title: intl.formatMessage({ id: 'page.variable.field.key' }),
            search: false,
            copyable: true,
            width: 200,
        },
        {
            dataIndex: 'isSecret',
            title: intl.formatMessage({ id: 'page.variable.field.isSecret' }),
            search: false,
            align: 'center',
            renderText: (_) => {
                return _ ? 'Y' : 'N';
            },
            width: 100,
        },
        {
            dataIndex: 'value',
            title: intl.formatMessage({ id: 'page.variable.field.value' }),
            search: false,
            ellipsis: true,
            width: 500,
        },
        {
            title: intl.formatMessage({ id: 'common.dict.table-action' }),
            valueType: 'option',
            width: 110,
            align: 'center',
            fixed: 'right',
            render: (text, record, _, action) => [
                <a
                    key="edit"
                    onClick={() => {
                        setEditModalData(record);
                        setEditModalDataId(record.id);
                        setEditModalVisible(true);
                        setEditModalTitle(
                            `${intl.formatMessage({ id: 'common.dict.edit' })} - ${record.key}`,
                        );
                    }}
                >
                    {intl.formatMessage({ id: 'common.dict.edit' })}
                </a>,
                <Popconfirm
                    key="delete"
                    title={intl.formatMessage({ id: 'common.dict.delete.confirm' })}
                    onConfirm={async () => {
                        if (await handleDelete(record.id!)) {
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
            <ProTable<API.IdentityUser>
                columns={columns}
                actionRef={tableActionRef}
                search={{ labelWidth: 140 }}
                scroll={{ x: 800 }}
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
                    const result = await getGlobalVariableList({
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
                width={650}
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
                        tableActionRef.current?.reload();
                    }
                }}
                layout="horizontal"
                labelWrap
                labelCol={{ span: 4 }}
            >
                <ProFormText
                    rules={[
                        { required: true },
                        { max: 64 },
                        {
                            pattern: /^[\u4e00-\u9fa5A-Za-z0-9\-\_]*$/,
                            message: 'invalid character',
                        },
                    ]}
                    name="key"
                    label={intl.formatMessage({ id: 'page.variable.field.key' })}
                    disabled={!!editModalDataId}
                />
                <ProFormSwitch
                    rules={[{ required: false }]}
                    name="isSecret"
                    label={intl.formatMessage({ id: 'page.variable.field.isSecret' })}
                    extra={intl.formatMessage({ id: 'page.variable.field.isSecret.tooltip' })}
                />
                <ProFormTextArea
                    rules={[{ required: false }]}
                    name="value"
                    label={intl.formatMessage({ id: 'page.variable.field.value' })}
                    fieldProps={{
                        autoSize: { minRows: 2, maxRows: 20 },
                    }}
                />
                {/* <ProForm.Item
                    name="value"
                    label={intl.formatMessage({ id: 'page.variable.field.value' })}
                >
                    <MonacorEditorInput height={150} />
                </ProForm.Item> */}
            </ModalForm>
        </PageContainer>
    );
};

export default Index;
