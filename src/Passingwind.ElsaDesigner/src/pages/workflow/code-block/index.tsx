import {
    createGlobalCode,
    deleteGlobalCode,
    getGlobalCodeList,
    updateGlobalCode,
} from '@/services/GlobalCode';
import type { API } from '@/services/typings';
import type { ActionType, ProColumnType } from '@ant-design/pro-components';
import { PageContainer, ProTable } from '@ant-design/pro-components';
import { Button, Popconfirm, message } from 'antd';
import React, { useRef, useState } from 'react';
import { formatMessage, history, useAccess, useIntl } from 'umi';

const handleAdd = async (data: any) => {
    const response = await createGlobalCode(data);
    if (response) {
        message.success(formatMessage({ id: 'common.dict.created.success' }));
        return true;
    }
    return false;
};

const handleEdit = async (id: string, data: any) => {
    const response = await updateGlobalCode(id, data);
    if (response) {
        message.success(formatMessage({ id: 'common.dict.modified.success' }));
        return true;
    }
    return false;
};

const handleDelete = async (id: string) => {
    const response = await deleteGlobalCode(id);
    if (response?.response?.ok) {
        message.success(formatMessage({ id: 'common.dict.deleted.success' }));
        return true;
    }
    return false;
};

const Index: React.FC = () => {
    const tableActionRef = useRef<ActionType>();
    const intl = useIntl();
    const access = useAccess();

    const [editModalVisible, setEditModalVisible] = useState<boolean>(false);
    const [editModalTitle, setEditModalTitle] = useState<string>('');
    const [editModalData, setEditModalData] = useState<API.GlobalCode>();
    const [editModalDataId, setEditModalDataId] = useState<string>();

    const columns: ProColumnType<API.GlobalCode>[] = [
        {
            dataIndex: 'filter',
            title: intl.formatMessage({ id: 'page.globalCode.field.name' }),
            hideInTable: true,
        },
        {
            dataIndex: 'name',
            title: intl.formatMessage({ id: 'page.globalCode.field.name' }),
            search: false,
            copyable: true,
            width: '30%',
        },
        {
            dataIndex: 'description',
            title: intl.formatMessage({ id: 'page.globalCode.field.description' }),
            search: false,
        },
        {
            dataIndex: 'languageDescription',
            title: intl.formatMessage({ id: 'page.globalCode.field.language' }),
            search: false,
        },
        {
            dataIndex: 'typeDescription',
            title: intl.formatMessage({ id: 'page.globalCode.field.type' }),
            search: false,
        },
        {
            dataIndex: 'latestVersion',
            title: intl.formatMessage({ id: 'page.globalCode.field.latestVersion' }),
            search: false,
        },
        {
            dataIndex: 'publishedVersion',
            title: intl.formatMessage({ id: 'page.globalCode.field.latestVersion' }),
            search: false,
        },
        {
            title: intl.formatMessage({ id: 'common.dict.table-action' }),
            valueType: 'option',
            width: 110,
            align: 'center',
            fixed: 'right',
            render: (text, record, _, action) => [
                access['ElsaWorkflow.GlobalCodes.Update'] && (
                    <a
                        key="edit"
                        onClick={() => {
                            history.push('/workflows/code-block/edit/' + record.id);
                        }}
                    >
                        {intl.formatMessage({ id: 'common.dict.edit' })}
                    </a>
                ),
                access['ElsaWorkflow.GlobalCodes.Delete'] && (
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
                    </Popconfirm>
                ),
            ],
        },
    ];

    return (
        <PageContainer>
            <ProTable<API.GlobalCode>
                columns={columns}
                actionRef={tableActionRef}
                search={{ labelWidth: 140 }}
                scroll={{ x: 800 }}
                rowKey="id"
                toolBarRender={() => [
                    access['ElsaWorkflow.GlobalCodes.Create'] && (
                        <Button
                            key="add"
                            type="primary"
                            onClick={() => {
                                history.push('/workflows/code-block/edit');
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
                    const result = await getGlobalCodeList({
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
            {/*
            <ModalForm
                title={editModalTitle}
                width={520}
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
                    rules={[{ required: true }, { max: 64 }]}
                    name="name"
                    label={intl.formatMessage({ id: 'page.globalCode.field.name' })}
                />
                <ProFormTextArea
                    rules={[{ required: false }]}
                    name="description"
                    label={intl.formatMessage({ id: 'page.globalCode.field.description' })}
                    fieldProps={{
                        autoSize: { minRows: 2, maxRows: 5 },
                    }}
                />
            </ModalForm> */}
        </PageContainer>
    );
};

export default Index;
