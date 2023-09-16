import {
    createWorkflowGroup,
    deleteWorkflowGroup,
    getWorkflowGroupList,
    updateWorkflowGroup,
} from '@/services/WorkflowGroup';
import type { API } from '@/services/typings';
import type { ActionType, ProColumnType } from '@ant-design/pro-components';
import {
    ModalForm,
    PageContainer,
    ProFormText,
    ProFormTextArea,
    ProTable,
} from '@ant-design/pro-components';
import { Button, Popconfirm, message } from 'antd';
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
    const tableActionRef = useRef<ActionType>();
    const intl = useIntl();
    const access = useAccess();

    const [editModalVisible, setEditModalVisible] = useState<boolean>(false);
    const [editModalTitle, setEditModalTitle] = useState<string>('');
    const [editModalData, setEditModalData] = useState<API.WorkflowGroup>();
    const [editModalDataId, setEditModalDataId] = useState<string>();

    const columns: ProColumnType<API.WorkflowGroup>[] = [
        {
            dataIndex: 'filter',
            title: intl.formatMessage({ id: 'page.workflowGroup.field.name' }),
            hideInTable: true,
        },
        {
            dataIndex: 'name',
            title: intl.formatMessage({ id: 'page.workflowGroup.field.name' }),
            search: false,
            copyable: true,
            width: '30%',
        },
        {
            dataIndex: 'description',
            title: intl.formatMessage({ id: 'page.workflowGroup.field.description' }),
            search: false,
        },
        {
            title: intl.formatMessage({ id: 'common.dict.table-action' }),
            valueType: 'option',
            width: 110,
            align: 'center',
            fixed: 'right',
            render: (text, record, _, action) => [
                access['ElsaWorkflow.WorkflowGroups.Update'] && (
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
                access['ElsaWorkflow.WorkflowGroups.Delete'] && (
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
            <ProTable<API.WorkflowGroup>
                columns={columns}
                actionRef={tableActionRef}
                search={{ labelWidth: 140 }}
                scroll={{ x: 800 }}
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
                    label={intl.formatMessage({ id: 'page.workflowGroup.field.name' })}
                />
                <ProFormTextArea
                    rules={[{ required: false }]}
                    name="description"
                    label={intl.formatMessage({ id: 'page.workflowGroup.field.description' })}
                    fieldProps={{
                        autoSize: { minRows: 2, maxRows: 5 },
                    }}
                />
            </ModalForm>
        </PageContainer>
    );
};

export default Index;
