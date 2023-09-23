import {
    batchDeleteWorkflowInstance,
    deleteWorkflowInstance,
    getWorkflowInstanceAssignableDefinition,
    getWorkflowInstanceList,
    workflowInstanceBatchCancel,
    workflowInstanceBatchRetry,
    workflowInstanceCancel,
    workflowInstanceRetry,
} from '@/services/WorkflowInstance';
import { WorkflowInstanceStatus } from '@/services/enums';
import type { GlobalAPI } from '@/services/global';
import type { API } from '@/services/typings';
import { formatTableSorter, getTableQueryConfig, saveTableQueryConfig } from '@/services/utils';
import type { ProFormInstance } from '@ant-design/pro-components';
import { TableDropdown } from '@ant-design/pro-components';
import { PageContainer } from '@ant-design/pro-layout';
import type { ActionType, ProColumnType } from '@ant-design/pro-table';
import ProTable from '@ant-design/pro-table';
import { Button, Modal, Space, Typography, message } from 'antd';
import React, { useEffect, useRef, useState } from 'react';
import { Link, useAccess, useIntl } from 'umi';
import { workflowStatusEnum } from './status';

const Index: React.FC = () => {
    const intl = useIntl();
    const access = useAccess();

    const searchFormRef = useRef<ProFormInstance>();
    const tableActionRef = useRef<ActionType>();
    const [tableFilterCollapsed, setTableFilterCollapsed] = useState<boolean>(true);
    const [tableQueryConfig, setTableQueryConfig] = useState<GlobalAPI.TableQueryConfig>();
    const [tableSelectedRowKeys, setTableSelectedRowKeys] = useState<React.Key[]>();
    const [tableSelectedRows, setTableSelectedRows] = useState<API.WorkflowInstance[]>([]);

    useEffect(() => {
        const tableQueryConfig = getTableQueryConfig('workflow_instances') ?? {};
        setTableQueryConfig(tableQueryConfig);
        searchFormRef.current?.setFieldsValue(tableQueryConfig?.filter);
        if (Object.keys(tableQueryConfig?.filter ?? {}).length > 0) {
            setTableFilterCollapsed(false);
        }
    }, []);

    const columns: ProColumnType<API.WorkflowInstance>[] = [
        {
            dataIndex: 'workflowDefinitionId',
            title: intl.formatMessage({ id: 'page.instance.field.definition' }),
            hideInTable: true,
            hideInSearch: !access['ElsaWorkflow.Definitions'],
            valueType: 'select',
            request: async (p) => {
                const list = await getWorkflowInstanceAssignableDefinition({
                    filter: p.keyWords ?? '',
                    sorting: 'name',
                    maxResultCount: 50,
                });
                return (list.items ?? []).map((x) => {
                    return {
                        label: `${x.displayName}(${x.name})`,
                        value: x.id,
                    };
                });
            },
            fieldProps: { showSearch: true },
        },
        {
            dataIndex: 'name',
            title: intl.formatMessage({ id: 'page.instance.field.name' }),
            sorter: true,
            sortOrder: tableQueryConfig?.sort?.name ?? null,
            fixed: 'left',
            render: (_, record) => (
                <Typography.Text copyable={{ text: record.name }}>
                    <Link to={`/instances/${record.id}`}>{_}</Link>
                </Typography.Text>
            ),
        },
        {
            dataIndex: 'version',
            title: intl.formatMessage({ id: 'page.instance.field.version' }),
            valueType: 'digit',
            width: 80,
            sorter: true,
            sortOrder: tableQueryConfig?.sort?.version ?? null,
        },
        {
            dataIndex: 'workflowStatus',
            title: intl.formatMessage({ id: 'page.instance.field.workflowStatus' }),
            valueEnum: workflowStatusEnum,
            width: 100,
            sorter: true,
            sortOrder: tableQueryConfig?.sort?.workflowStatus ?? null,
        },
        {
            dataIndex: 'creationTime',
            title: intl.formatMessage({ id: 'common.dict.creationTime' }),
            valueType: 'dateTime',
            width: 150,
            sorter: true,
            sortOrder: tableQueryConfig?.sort?.creationTime ?? null,
            search: false,
        },
        {
            dataIndex: 'creationTimes',
            title: intl.formatMessage({ id: 'common.dict.creationTime' }),
            valueType: 'dateTimeRange',
            hideInTable: true,
        },
        {
            dataIndex: 'finishedTime',
            title: intl.formatMessage({ id: 'page.instance.field.finishedTime' }),
            valueType: 'dateTime',
            width: 150,
            sorter: true,
            sortOrder: tableQueryConfig?.sort?.finishedTime ?? null,
            search: false,
        },
        {
            dataIndex: 'finishedTimes',
            title: intl.formatMessage({ id: 'page.instance.field.finishedTime' }),
            valueType: 'dateTimeRange',
            hideInTable: true,
        },
        {
            dataIndex: 'lastExecutedTime',
            title: intl.formatMessage({ id: 'page.instance.field.lastExecutedTime' }),
            valueType: 'dateTime',
            width: 150,
            sorter: true,
            sortOrder: tableQueryConfig?.sort?.lastExecutedTime ?? null,
            search: false,
        },
        {
            dataIndex: 'lastExecutedTimes',
            title: intl.formatMessage({ id: 'page.instance.field.lastExecutedTime' }),
            valueType: 'dateTimeRange',
            hideInTable: true,
        },
        {
            dataIndex: 'faultedTime',
            title: intl.formatMessage({ id: 'page.instance.field.faultedTime' }),
            valueType: 'dateTime',
            width: 150,
            sorter: true,
            sortOrder: tableQueryConfig?.sort?.faultedTime ?? null,
            search: false,
        },
        {
            dataIndex: 'faultedTimes',
            title: intl.formatMessage({ id: 'page.instance.field.faultedTime' }),
            valueType: 'dateTimeRange',
            hideInTable: true,
        },
        //
        {
            dataIndex: 'correlationId',
            title: intl.formatMessage({ id: 'page.instance.field.correlationId' }),
            width: 150,
            ellipsis: true,
            copyable: true,
        },
        {
            title: intl.formatMessage({ id: 'common.dict.table-action' }),
            valueType: 'option',
            width: 80,
            align: 'center',
            fixed: 'right',
            render: (_text, record, _, action) => {
                return (
                    <Space>
                        <Link
                            key="view"
                            to={{
                                pathname: `/instances/${record.id}`,
                            }}
                        >
                            {intl.formatMessage({ id: 'page.instance.view' })}
                        </Link>
                        <TableDropdown
                            key="actionGroup"
                            onSelect={(key) => {
                                if (key == 'delete') {
                                    Modal.confirm({
                                        title: intl.formatMessage({
                                            id: 'common.dict.delete.confirm',
                                        }),
                                        content: intl.formatMessage({
                                            id: 'page.instance.delete.confirm.content',
                                        }),
                                        onOk: async () => {
                                            const result = await deleteWorkflowInstance(record.id!);
                                            if (result?.response?.ok) {
                                                message.success(
                                                    intl.formatMessage({
                                                        id: 'common.dict.delete.success',
                                                    }),
                                                );
                                                action?.reload();
                                            }
                                        },
                                    });
                                } else if (key == 'retry') {
                                    Modal.confirm({
                                        title: intl.formatMessage({
                                            id: 'page.instance.retry.confirm.title',
                                        }),
                                        content: intl.formatMessage({
                                            id: 'page.instance.retry.confirm.content',
                                        }),
                                        onOk: async () => {
                                            const result = await workflowInstanceRetry(
                                                record.id!,
                                                {},
                                            );
                                            if (result?.response?.ok) {
                                                message.success(
                                                    intl.formatMessage({
                                                        id: 'page.instance.retry.confirm.success',
                                                    }),
                                                );
                                                action?.reload();
                                            }
                                        },
                                    });
                                } else if (key == 'cancel') {
                                    Modal.confirm({
                                        title: intl.formatMessage({
                                            id: 'page.instance.cancel.confirm.title',
                                        }),
                                        content: intl.formatMessage({
                                            id: 'page.instance.cancel.confirm.content',
                                        }),
                                        onOk: async () => {
                                            const result = await workflowInstanceCancel(record.id!);
                                            if (result?.response?.ok) {
                                                message.success(
                                                    intl.formatMessage({
                                                        id: 'page.instance.cancel.confirm.success',
                                                    }),
                                                );
                                                action?.reload();
                                            }
                                        },
                                    });
                                }
                            }}
                            menus={[
                                {
                                    key: 'cancel',
                                    name: intl.formatMessage({ id: 'page.instance.cancel' }),
                                    disabled:
                                        (record.workflowStatus != WorkflowInstanceStatus.Idle &&
                                            record.workflowStatus !=
                                                WorkflowInstanceStatus.Running &&
                                            record.workflowStatus !=
                                                WorkflowInstanceStatus.Suspended) ||
                                        !access['ElsaWorkflow.Instances.Action'],
                                },
                                {
                                    key: 'retry',
                                    name: intl.formatMessage({ id: 'page.instance.retry' }),
                                    disabled:
                                        record.workflowStatus != WorkflowInstanceStatus.Faulted ||
                                        !access['ElsaWorkflow.Instances.Action'],
                                },
                                {
                                    type: 'divider' as const,
                                },
                                {
                                    key: 'delete',
                                    name: intl.formatMessage({ id: 'common.dict.delete' }),
                                    danger: true,
                                    disabled: !access['ElsaWorkflow.Instances.Delete'],
                                },
                            ]}
                        />
                    </Space>
                );
            },
        },
    ];

    return (
        <PageContainer>
            <ProTable<API.WorkflowInstance>
                columns={columns}
                dateFormatter={(value) => {
                    return value.utc().format();
                }}
                actionRef={tableActionRef}
                formRef={searchFormRef}
                rowSelection={{
                    selectedRowKeys: tableSelectedRowKeys,
                    onChange: (selectedRowKeys, selectedRows) => {
                        setTableSelectedRows(selectedRows);
                        setTableSelectedRowKeys(selectedRowKeys);
                    },
                }}
                tableAlertOptionRender={({ selectedRowKeys, _, onCleanSelected }) => {
                    // issue 'selectedRows' wil be lost the new values.
                    return (
                        <Space>
                            <Button
                                key="retry"
                                type="link"
                                disabled={
                                    !(
                                        selectedRowKeys.length > 0 &&
                                        tableSelectedRows.every(
                                            (x) =>
                                                x.workflowStatus == WorkflowInstanceStatus.Faulted,
                                        )
                                    ) || !access['ElsaWorkflow.Instances.Action']
                                }
                                onClick={() => {
                                    Modal.confirm({
                                        title: intl.formatMessage({
                                            id: 'page.instance.retry.confirm.title',
                                        }),
                                        content: intl.formatMessage({
                                            id: 'page.instance.retry.confirm.content',
                                        }),
                                        onOk: async () => {
                                            const result = await workflowInstanceBatchRetry({
                                                ids: selectedRowKeys.map((x) => x as string),
                                            });
                                            if (result?.response?.ok) {
                                                onCleanSelected();
                                                tableActionRef.current?.reload();
                                                message.success(
                                                    intl.formatMessage({
                                                        id: 'common.dict.success',
                                                    }),
                                                );
                                            }
                                        },
                                    });
                                }}
                            >
                                {intl.formatMessage({ id: 'page.instance.retry' })}
                            </Button>
                            <Button
                                key="cancel"
                                type="link"
                                disabled={
                                    !(
                                        selectedRowKeys.length > 0 &&
                                        tableSelectedRows.every(
                                            (x) =>
                                                x.workflowStatus ==
                                                    WorkflowInstanceStatus.Running ||
                                                x.workflowStatus == WorkflowInstanceStatus.Idle ||
                                                x.workflowStatus ==
                                                    WorkflowInstanceStatus.Suspended,
                                        )
                                    ) || !access['ElsaWorkflow.Instances.Action']
                                }
                                onClick={() => {
                                    Modal.confirm({
                                        title: intl.formatMessage({
                                            id: 'page.instance.cancel.confirm.title',
                                        }),
                                        content: intl.formatMessage({
                                            id: 'page.instance.cancel.confirm.content',
                                        }),
                                        onOk: async () => {
                                            const result = await workflowInstanceBatchCancel({
                                                ids: selectedRowKeys.map((x) => x as string),
                                            });
                                            if (result?.response?.ok) {
                                                onCleanSelected();
                                                tableActionRef.current?.reload();
                                                message.success(
                                                    intl.formatMessage({
                                                        id: 'common.dict.success',
                                                    }),
                                                );
                                            }
                                        },
                                    });
                                }}
                            >
                                {intl.formatMessage({ id: 'page.instance.cancel' })}
                            </Button>
                            <Button
                                key="delete"
                                danger
                                type="link"
                                onClick={() => {
                                    Modal.confirm({
                                        title: intl.formatMessage({
                                            id: 'common.dict.delete.confirm',
                                        }),
                                        content: intl.formatMessage({
                                            id: 'page.instance.delete.confirm.content',
                                        }),
                                        onOk: async () => {
                                            const result = await batchDeleteWorkflowInstance({
                                                ids: selectedRowKeys.map((x) => x as string),
                                            });
                                            if (result?.response?.ok) {
                                                onCleanSelected();
                                                tableActionRef.current?.reload();
                                                message.success(
                                                    intl.formatMessage({
                                                        id: 'common.dict.success',
                                                    }),
                                                );
                                            }
                                        },
                                    });
                                }}
                                disabled={!access['ElsaWorkflow.Instances.Delete']}
                            >
                                {intl.formatMessage({ id: 'common.dict.delete' })}
                            </Button>
                            <Button key="clear" type="link" onClick={() => onCleanSelected()}>
                                {intl.formatMessage({
                                    id: 'common.dict.table.clearSelected',
                                })}
                            </Button>
                        </Space>
                    );
                }}
                search={{
                    labelWidth: 140,
                    collapsed: tableFilterCollapsed,
                    onCollapse: setTableFilterCollapsed,
                }}
                scroll={{ x: 1600 }}
                rowKey="id"
                onReset={() => {
                    // clear filter & pagination & sorting
                    setTableQueryConfig({
                        sort: null,
                        filter: null,
                        pagination: undefined,
                    });
                    // clear selected
                    setTableSelectedRowKeys([]);
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
                    // clear selected
                    setTableSelectedRowKeys([]);
                }}
                beforeSearchSubmit={(params) => {
                    // update filter
                    setTableQueryConfig({ ...tableQueryConfig, filter: params });
                    return params;
                }}
                request={async (params) => {
                    // clear selected
                    setTableSelectedRowKeys([]);
                    //
                    const { current, pageSize } = params;
                    delete params.current;
                    delete params.pageSize;
                    const skipCount = (current! - 1) * pageSize!;

                    // clear some params
                    const queryConfig = { ...tableQueryConfig };
                    delete queryConfig?.filter?.current;
                    delete queryConfig?.filter?.pageSize;
                    delete queryConfig?.filter?._timestamp;

                    // save session
                    saveTableQueryConfig('workflow_instances', queryConfig);

                    // fetch
                    const result = await getWorkflowInstanceList({
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
        </PageContainer>
    );
};

export default Index;
