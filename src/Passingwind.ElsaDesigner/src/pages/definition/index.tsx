import type { GlobalAPI } from '@/services/global';
import type { API } from '@/services/typings';
import { formatTableSorter, getTableQueryConfig, saveTableQueryConfig } from '@/services/utils';
import {
    deleteWorkflowDefinition,
    getWorkflowDefinitionList,
    getWorkflowDefinitionVersion,
    updateWorkflowDefinitionDefinition,
    workflowDefinitionDispatch,
    workflowDefinitionPublish,
    workflowDefinitionUnPublish,
} from '@/services/WorkflowDefinition';
import type { ProFormInstance } from '@ant-design/pro-components';
import { ModalForm, ProFormSelect, ProFormText, ProFormTextArea } from '@ant-design/pro-form';
import { PageContainer } from '@ant-design/pro-layout';
import type { ActionType, ProColumnType } from '@ant-design/pro-table';
import ProTable, { TableDropdown } from '@ant-design/pro-table';
import { Button, Form, message, Modal } from 'antd';
import React, { useEffect, useRef, useState } from 'react';
import { formatMessage, Link, useHistory, useIntl } from 'umi';
import EditFormItems from './edit-form-items';

const handleEdit = async (id: string, data: any) => {
    const response = await updateWorkflowDefinitionDefinition(id, data);
    if (response) {
        message.success(formatMessage({ id: 'common.dict.modified.success' }));
        return true;
    }
    return false;
};

const handleDelete = async (id: string) => {
    const response = await deleteWorkflowDefinition(id);
    if (response?.response?.ok) {
        message.success(formatMessage({ id: 'common.dict.deleted.success' }));
        return true;
    }
    return false;
};

const Index: React.FC = () => {
    const intl = useIntl();

    const searchFormRef = useRef<ProFormInstance>();
    const tableActionRef = useRef<ActionType>();
    const [tableFilterCollapsed, setTableFilterCollapsed] = useState<boolean>(true);
    const [tableQueryConfig, setTableQueryConfig] = useState<GlobalAPI.TableQueryConfig>();

    const history = useHistory();

    const [actionRow, setActionRow] = useState<API.WorkflowDefinition>();

    const [editModalVisible, setEditModalVisible] = useState<boolean>(false);
    const [editModalTitle, setEditModalTitle] = useState<string>('');
    const [editModalData, setEditModalData] = useState<API.WorkflowDefinition>();
    const [editModalDataId, setEditModalDataId] = useState<string>();

    const [dispatchFormVisible, setDispatchFormVisible] = useState<boolean>(false);
    const [dispatchId, setDispatchId] = useState<string>();

    const [editForm] = Form.useForm();

    const columns: ProColumnType<API.WorkflowDefinition>[] = [
        {
            dataIndex: 'filter',
            title: intl.formatMessage({ id: 'page.definition.field.name' }),
            hideInTable: true,
        },
        {
            dataIndex: 'name',
            title: intl.formatMessage({ id: 'page.definition.field.name' }),
            search: false,
            copyable: true,
            sorter: true,
            sortOrder: tableQueryConfig?.sort?.name ?? null,
            width: 250,
            fixed: 'left',
            renderText: (_, record) => <Link to={`/definitions/${record.id}`}>{_}</Link>,
        },
        {
            dataIndex: 'displayName',
            title: intl.formatMessage({ id: 'page.definition.field.displayName' }),
            search: false,
            copyable: true,
            sorter: true,
            sortOrder: tableQueryConfig?.sort?.displayName ?? null,
            width: 250,
        },
        {
            dataIndex: 'description',
            title: intl.formatMessage({ id: 'page.definition.field.description' }),
            search: false,
            width: 160,
        },
        {
            dataIndex: 'latestVersion',
            title: intl.formatMessage({ id: 'page.definition.field.latestVersion' }),
            search: false,
            sorter: true,
            sortOrder: tableQueryConfig?.sort?.latestVersion ?? null,
            width: 160,
        },
        {
            dataIndex: 'publishedVersion',
            title: intl.formatMessage({ id: 'page.definition.field.publishedVersion' }),
            search: false,
            sorter: true,
            sortOrder: tableQueryConfig?.sort?.publishedVersion ?? null,
            width: 160,
        },
        {
            dataIndex: 'isSingleton',
            title: intl.formatMessage({ id: 'page.definition.field.isSingleton' }),
            search: false,
            valueEnum: {
                true: { text: 'Y' },
                false: { text: 'N' },
            },
            sorter: true,
            sortOrder: tableQueryConfig?.sort?.isSingleton ?? null,
            width: 120,
            align: 'center',
        },
        {
            dataIndex: 'lastModificationTime',
            title: intl.formatMessage({ id: 'common.dict.lastModificationTime' }),
            valueType: 'dateTime',
            search: false,
            renderText: (_, record) => _ ?? record.creationTime,
            sorter: true,
            sortOrder: tableQueryConfig?.sort?.lastModificationTime ?? null,
            width: 180,
        },
        {
            title: intl.formatMessage({ id: 'common.dict.table-action' }),
            valueType: 'option',
            width: 180,
            align: 'center',
            fixed: 'right',
            render: (text, record, _, action) => [
                <a
                    key="designer"
                    onClick={() => {
                        history.push('/designer?id=' + record.id);
                    }}
                >
                    {intl.formatMessage({
                        id: 'page.definition.designer',
                    })}
                </a>,
                <a
                    key="edit"
                    onClick={() => {
                        setEditModalData(
                            Object.assign({}, record, {
                                variablesString: JSON.stringify(record.variables ?? {}, null, 2),
                            }),
                        );
                        setEditModalDataId(record.id);
                        setEditModalTitle(
                            `${intl.formatMessage({ id: 'common.dict.edit' })} - ${record.name}`,
                        );
                        setEditModalVisible(true);
                        editForm.setFieldsValue(
                            Object.assign({}, record, {
                                variablesString: JSON.stringify(record.variables ?? {}, null, 2),
                            }),
                        );
                    }}
                >
                    {intl.formatMessage({ id: 'page.definition.settings' })}
                </a>,

                <TableDropdown
                    key="actionGroup"
                    onSelect={async (key) => {
                        if (key == 'delete') {
                            Modal.confirm({
                                title: intl.formatMessage({
                                    id: 'common.dict.delete.confirm',
                                }),
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
                        } else if (key == 'copyable') {
                            history.push(
                                `/designer?fromId=${record.id}&fromVersion=${record.latestVersion}`,
                            );
                        } else if (key == 'dispatch') {
                            setDispatchId(record.id);
                            setActionRow(record);
                            setDispatchFormVisible(true);
                        }
                    }}
                    menus={[
                        {
                            key: 'dispatch',
                            name: intl.formatMessage({ id: 'page.definition.dispatch' }),
                            disabled: !record.publishedVersion,
                        },
                        {
                            type: 'divider',
                        },
                        {
                            key: 'publish',
                            name: intl.formatMessage({ id: 'page.definition.publish' }),
                            disabled: record.publishedVersion != null,
                        },
                        {
                            key: 'unpublish',
                            name: intl.formatMessage({ id: 'page.definition.unpublish' }),
                            disabled: record.publishedVersion == null,
                        },
                        {
                            type: 'divider',
                        },
                        {
                            key: 'copyable',
                            name: intl.formatMessage({ id: 'page.definition.copyable' }),
                        },
                        {
                            type: 'divider' as const,
                        },
                        {
                            key: 'delete',
                            name: intl.formatMessage({ id: 'common.dict.delete' }),
                            danger: true,
                        },
                    ]}
                />,
            ],
        },
    ];

    useEffect(() => {
        const tableQueryConfig = getTableQueryConfig('workflow_definitions') ?? {};
        setTableQueryConfig(tableQueryConfig);
        searchFormRef.current?.setFieldsValue(tableQueryConfig?.filter);
        if (Object.keys(tableQueryConfig?.filter ?? {}).length > 0) {
            setTableFilterCollapsed(false);
        }
    }, []);

    return (
        <PageContainer>
            <ProTable<API.WorkflowDefinition>
                columns={columns}
                actionRef={tableActionRef}
                formRef={searchFormRef}
                search={{ labelWidth: 140 }}
                scroll={{ x: 1300 }}
                rowKey="id"
                toolBarRender={() => [
                    <Button
                        key="add"
                        type="primary"
                        onClick={() => {
                            history.push('/designer');
                        }}
                    >
                        {intl.formatMessage({ id: 'common.dict.create' })}
                    </Button>,
                ]}
                onReset={() => {
                    // clear filter & pagination & sorting
                    setTableQueryConfig({
                        sort: null,
                        filter: null,
                        pagination: undefined,
                    });
                }}
                pagination={tableQueryConfig?.pagination}
                onChange={(pagination, _, sorter) => {
                    // update pagination
                    let queryConfig: GlobalAPI.TableQueryConfig = {
                        ...tableQueryConfig,
                        pagination: {
                            current: pagination.current ?? 1,
                            pageSize: pagination.pageSize ?? 10,
                        },
                        sort: null,
                    };
                    // update sorter
                    if (sorter && sorter.column && sorter.field) {
                        queryConfig = {
                            ...queryConfig,
                            sort: { [sorter.field]: sorter.order ?? '' },
                        };
                    }

                    // update
                    setTableQueryConfig(queryConfig);
                }}
                request={async (params) => {
                    const { current, pageSize } = params;
                    delete params.current;
                    delete params.pageSize;
                    const skipCount = (current! - 1) * pageSize!;

                    // update filter
                    const queryConfig: GlobalAPI.TableQueryConfig = {
                        ...tableQueryConfig,
                        filter: { ...tableQueryConfig?.filter, ...params },
                    };

                    // save session
                    saveTableQueryConfig('workflow_definitions', queryConfig);

                    // fetch
                    const result = await getWorkflowDefinitionList({
                        ...queryConfig.filter,
                        skipCount,
                        maxResultCount: pageSize,
                        sorting: formatTableSorter(queryConfig?.sort ?? {}),
                    });
                    return {
                        success: !!result,
                        data: result?.items,
                        total: result?.totalCount,
                    };
                }}
            />

            {/*  */}
            <ModalForm
                form={editForm}
                title={editModalTitle}
                visible={editModalVisible}
                modalProps={{ destroyOnClose: true, maskClosable: false }}
                preserve={false}
                onVisibleChange={setEditModalVisible}
                initialValues={editModalData}
                labelCol={{ span: 5 }}
                width={650}
                labelWrap
                layout="horizontal"
                onValuesChange={(value) => {
                    if (value.displayName) {
                        editForm.setFieldsValue({
                            name: (value.displayName as string)
                                ?.replaceAll(' ', '-')
                                ?.toLowerCase(),
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
                        tableActionRef.current?.reload();
                    }
                }}
            >
                <EditFormItems />
            </ModalForm>

            {/*  */}
            <ModalForm
                layout="horizontal"
                modalProps={{ destroyOnClose: true, maskClosable: false }}
                preserve={false}
                labelCol={{ span: 5 }}
                width={600}
                labelWrap
                title={intl.formatMessage({
                    id: 'page.definition.dispatch',
                })}
                visible={dispatchFormVisible}
                onVisibleChange={setDispatchFormVisible}
                onFinish={async (value) => {
                    const result = await workflowDefinitionDispatch(dispatchId!, value);
                    if (result?.workflowInstanceId) {
                        message.success(
                            intl.formatMessage(
                                {
                                    id: 'page.definition.dispatch.success',
                                },
                                { id: result.workflowInstanceId },
                            ),
                        );
                        return true;
                    }
                    return false;
                }}
            >
                <ProFormSelect
                    label={intl.formatMessage({ id: 'page.definition.dispatch.activityId' })}
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
                <ProFormText
                    label={intl.formatMessage({ id: 'page.definition.dispatch.correlationId' })}
                    name="correlationId"
                    rules={[{ max: 36 }]}
                />
                <ProFormText
                    label={intl.formatMessage({ id: 'page.definition.dispatch.contextId' })}
                    name="contextId"
                    rules={[{ max: 36 }]}
                />
                <ProFormTextArea
                    label={intl.formatMessage({ id: 'page.definition.dispatch.input' })}
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
