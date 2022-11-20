import { WorkflowInstanceStatus } from '@/services/enums';
import type { GlobalAPI } from '@/services/global';
import type { API } from '@/services/typings';
import { formatTableSorter, getTableQueryConfig, saveTableQueryConfig } from '@/services/utils';
import { getWorkflowDefinitionList } from '@/services/WorkflowDefinition';
import {
    deleteWorkflowInstance,
    getWorkflowInstanceList,
    workflowInstanceCancel,
    workflowInstanceRetry,
} from '@/services/WorkflowInstance';
import { ProFormInstance, TableDropdown } from '@ant-design/pro-components';
import { PageContainer } from '@ant-design/pro-layout';
import type { ActionType, ProColumnType } from '@ant-design/pro-table';
import ProTable from '@ant-design/pro-table';
import { message, Modal, Space } from 'antd';
import React, { useEffect, useRef, useState } from 'react';
import { Link, useIntl } from 'umi';
import { workflowStatusEnum } from './status';

const Index: React.FC = () => {
    const intl = useIntl();

    const searchFormRef = useRef<ProFormInstance>();
    const tableActionRef = useRef<ActionType>();
    const [tableFilterCollapsed, setTableFilterCollapsed] = useState<boolean>(true);
    const [tableQueryConfig, setTableQueryConfig] = useState<GlobalAPI.TableQueryConfig>();

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
            valueType: 'select',
            request: async (p) => {
                const list = await getWorkflowDefinitionList({
                    filter: p.keyWords ?? '',
                    maxResultCount: 100,
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
            copyable: true,
            render: (_, record) => {
                return (
                    <Link
                        to={{
                            pathname: `/instances/${record.id}`,
                        }}
                    >
                        {_}
                    </Link>
                );
            },
            sorter: true,
            sortOrder: tableQueryConfig?.sort?.name ?? null,
            fixed: 'left',
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
            render: (text, record, _, action) => {
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
                                    name: intl.formatMessage({ id: 'page.instance.retry' }),
                                    disabled:
                                        record.workflowStatus != WorkflowInstanceStatus.Idle &&
                                        record.workflowStatus != WorkflowInstanceStatus.Running &&
                                        record.workflowStatus != WorkflowInstanceStatus.Suspended,
                                },
                                {
                                    key: 'retry',
                                    name: intl.formatMessage({ id: 'page.instance.cancel' }),
                                    disabled:
                                        record.workflowStatus != WorkflowInstanceStatus.Faulted,
                                },
                                {
                                    type: 'divider',
                                },
                                {
                                    key: 'delete',
                                    name: intl.formatMessage({ id: 'common.dict.delete' }),
                                    danger: true,
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
                actionRef={tableActionRef}
                formRef={searchFormRef}
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
