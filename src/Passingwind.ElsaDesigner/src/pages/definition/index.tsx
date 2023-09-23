import {
    deleteWorkflowDefinition,
    deleteWorkflowDefinitionOwner,
    getWorkflowDefinitionAssignableGroups,
    getWorkflowDefinitionIam,
    getWorkflowDefinitionList,
    getWorkflowDefinitionVersion,
    updateWorkflowDefinitionDefinition,
    workflowDefinitionAddOwner,
    workflowDefinitionDispatch,
    workflowDefinitionExport2 as workflowDefinitionExport,
    workflowDefinitionImport,
    workflowDefinitionPublish,
    workflowDefinitionUnPublish,
} from '@/services/WorkflowDefinition';
import type { GlobalAPI } from '@/services/global';
import type { API } from '@/services/typings';
import {
    formatTableSorter,
    formatUserName,
    getTableQueryConfig,
    saveTableQueryConfig,
    showDownloadFile,
} from '@/services/utils';
import {
    CheckCircleTwoTone,
    CloseCircleOutlined,
    DownloadOutlined,
    UploadOutlined,
} from '@ant-design/icons';
import type { ProFormInstance } from '@ant-design/pro-components';
import { ProForm } from '@ant-design/pro-components';
import { ModalForm, ProFormSelect, ProFormText, ProFormTextArea } from '@ant-design/pro-form';
import { PageContainer } from '@ant-design/pro-layout';
import type { ActionType, ProColumnType } from '@ant-design/pro-table';
import ProTable, { TableDropdown } from '@ant-design/pro-table';
import {
    Alert,
    Button,
    Checkbox,
    Drawer,
    Form,
    Modal,
    Popconfirm,
    Table,
    Tabs,
    Tag,
    Tooltip,
    Typography,
    Upload,
    message,
} from 'antd';
import moment from 'moment';
import React, { useEffect, useRef, useState } from 'react';
import { Link, formatMessage, useAccess, useHistory, useIntl } from 'umi';
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
    const access = useAccess();

    const searchFormRef = useRef<ProFormInstance>();
    const tableActionRef = useRef<ActionType>();
    const [tableFilterCollapsed, setTableFilterCollapsed] = useState<boolean>(true);
    const [tableQueryConfig, setTableQueryConfig] = useState<GlobalAPI.TableQueryConfig>();
    const [tableSelectedRowKeys, setTableSelectedRowKeys] = useState<React.Key[]>();
    const [tableSelectedRows, setTableSelectedRows] = useState<API.WorkflowDefinition[]>([]);

    const history = useHistory();

    const [actionRow, setActionRow] = useState<API.WorkflowDefinition>();

    const [editModalVisible, setEditModalVisible] = useState<boolean>(false);
    const [editModalTitle, setEditModalTitle] = useState<string>('');
    const [editModalData, setEditModalData] = useState<API.WorkflowDefinition>();
    const [editModalDataId, setEditModalDataId] = useState<string>();

    const [dispatchFormVisible, setDispatchFormVisible] = useState<boolean>(false);
    const [dispatchId, setDispatchId] = useState<string>();

    const [iampModalVisible, setIampModalVisible] = useState(false);
    const [iamData, setIamData] = useState<API.WorkflowDefinitionIamResult>();

    const [exporting, setExporting] = useState<boolean>();

    const [importing, setImporting] = useState(false);
    const [importModalVisible, setImportModalVisible] = useState<boolean>(false);
    const [importOverwrite, setImportOverwrite] = useState(false);
    const [importResult, setImportResult] = useState<API.WorkflowDefinitionImportResult>();
    const [importResultTableVisible, setImportResultTableVisible] = useState(false);

    const [editForm] = ProForm.useForm();

    const loadIamData = async (id: string) => {
        const loading = message.loading(intl.formatMessage({ id: 'common.dict.loading' }));
        const result = await getWorkflowDefinitionIam(id);
        loading();
        if (result) {
            setIamData(result);
            return true;
        }
        return false;
    };

    const handleShowIam = async (record: API.WorkflowDefinition) => {
        setEditModalData(record);
        setEditModalDataId(record.id);

        const result = await loadIamData(record.id);

        if (result) {
            setIampModalVisible(true);
        }
    };

    const handleDeleteOwner = async (userId: string) => {
        const result = await deleteWorkflowDefinitionOwner(editModalDataId!, userId);
        if (result?.response?.ok) {
            message.success(intl.formatMessage({ id: 'common.dict.deleted.success' }));
            return true;
        }
        return false;
    };

    const handleAddOwner = async (userId: string) => {
        const result = await workflowDefinitionAddOwner(editModalDataId!, { userId: userId });
        if (result?.response?.ok) {
            message.success(intl.formatMessage({ id: 'common.dict.save.success' }));
            return true;
        }
        return false;
    };

    const handleExport = async () => {
        const loading = message.loading(
            intl.formatMessage({ id: 'page.definition.export.loading' }),
        );
        setExporting(true);
        const res = await workflowDefinitionExport(
            {
                ids: tableSelectedRowKeys as string[],
            },
            { responseType: 'arrayBuffer' },
        );
        if (res.response.ok) {
            const fileName =
                intl.formatMessage({ id: 'page.definition' }) +
                '-' +
                moment().format('YYYYMMDDHHmm') +
                '.zip';
            showDownloadFile(fileName, res.data, 'application/stream');
        }
        loading();
        setExporting(false);
    };

    const handleImport = async (
        file: any,
        filename: string | undefined,
        onSuccess: any,
        onError: any,
    ) => {
        setImporting(true);
        const formData = new FormData();
        formData.append('file', file);
        formData.append('overwrite', importOverwrite as unknown as string);
        const result = await workflowDefinitionImport(
            formData as unknown as API.WorkflowDefinitionImportRequest,
        );

        setImporting(false);
        setImportResult(result);

        // @ts-nocheck
        const firstError = (result?.results ?? []).find((x) => x.hasError);
        if (firstError) {
            onError?.({
                name: firstError.fileName!,
                message: firstError.errorMessage!,
            });
        } else {
            onSuccess?.({});
        }

        // check result
        // (result?.results ?? []).forEach((item) => {
        //     if (item.hasError) {
        //         message.error(
        //             intl.formatMessage(
        //                 {
        //                     id: 'page.definition.import.result.failed',
        //                 },
        //                 item.workflow,
        //             ) + item.errorMessage,
        //             5,
        //         );
        //     } else {
        //         message.success(
        //             intl.formatMessage(
        //                 { id: 'page.definition.import.result.success' },
        //                 { ...item.workflow },
        //             ),
        //         );
        //     }
        // });
    };

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
            sorter: true,
            sortOrder: tableQueryConfig?.sort?.name ?? null,
            width: 250,
            fixed: 'left',
            render: (_, record) => (
                <Typography.Text copyable={{ text: record.name }} ellipsis>
                    <Link to={`/definitions/${record.id}`}>{_}</Link>
                </Typography.Text>
            ),
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
            dataIndex: 'groupName',
            title: intl.formatMessage({ id: 'page.definition.field.groupName' }),
            search: false,
            width: 160,
        },
        {
            dataIndex: 'groupId',
            title: intl.formatMessage({ id: 'page.definition.field.groupName' }),
            hideInTable: true,
            request: async () => {
                const result = await getWorkflowDefinitionAssignableGroups();
                return (result?.items ?? []).map((x) => {
                    return {
                        label: x.name,
                        value: x.id,
                    };
                });
            },
        },
        {
            dataIndex: 'description',
            title: intl.formatMessage({ id: 'page.definition.field.description' }),
            search: false,
            width: 160,
        },
        {
            dataIndex: 'channel',
            title: intl.formatMessage({ id: 'page.definition.field.channel' }),
            hideInTable: true,
        },
        {
            dataIndex: 'tag',
            title: intl.formatMessage({ id: 'page.definition.field.tag' }),
            hideInTable: true,
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
            valueEnum: {
                true: { text: <CheckCircleTwoTone /> },
                false: { text: <CloseCircleOutlined /> },
            },
            sorter: true,
            sortOrder: tableQueryConfig?.sort?.isSingleton ?? null,
            width: 120,
            align: 'center',
            request: async () => {
                return [
                    {
                        label: 'Y',
                        value: 'true',
                    },
                    {
                        label: 'N',
                        value: 'false',
                    },
                ];
            },
        },
        {
            dataIndex: 'deleteCompletedInstances',
            title: intl.formatMessage({ id: 'page.definition.field.deleteCompletedInstances' }),
            hideInTable: true,
            valueEnum: {
                true: { text: 'Y' },
                false: { text: 'N' },
            },
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
            width: 150,
            align: 'center',
            fixed: 'right',
            render: (text, record, _, action) => [
                <Link
                    key="view"
                    to={{
                        pathname: `/definitions/${record.id}`,
                    }}
                >
                    {intl.formatMessage({ id: 'page.definition.view' })}
                </Link>,
                access['ElsaWorkflow.Definitions.CreateOrUpdateOrPublish'] ? (
                    <Link key="designer" to={'/designer?id=' + record.id}>
                        {intl.formatMessage({
                            id: 'page.definition.designer',
                        })}
                    </Link>
                ) : (
                    <></>
                ),
                <TableDropdown
                    key="actionGroup"
                    onSelect={async (key) => {
                        if (key == 'edit') {
                            setEditModalData(
                                Object.assign({}, record, {
                                    variablesString: JSON.stringify(
                                        record.variables ?? {},
                                        null,
                                        2,
                                    ),
                                }),
                            );
                            setEditModalDataId(record.id);
                            setEditModalTitle(
                                `${intl.formatMessage({ id: 'common.dict.edit' })} - ${
                                    record.name
                                }`,
                            );
                            setEditModalVisible(true);
                            editForm.setFieldsValue(
                                Object.assign({}, record, {
                                    variablesString: JSON.stringify(
                                        record.variables ?? {},
                                        null,
                                        2,
                                    ),
                                }),
                            );
                        } else if (key == 'delete') {
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
                        } else if (key == 'iam') {
                            handleShowIam(record);
                        }
                    }}
                    menus={[
                        {
                            key: 'edit',
                            name: intl.formatMessage({ id: 'page.definition.settings' }),
                            disabled: !access['ElsaWorkflow.Definitions.CreateOrUpdateOrPublish'],
                        },
                        {
                            key: 'iam',
                            name: intl.formatMessage({ id: 'page.definition.iam' }),
                            disabled: !access['ElsaWorkflow.Definitions.ManagePermissions'],
                        },
                        {
                            type: 'divider' as const,
                        },
                        {
                            key: 'dispatch',
                            name: intl.formatMessage({ id: 'page.definition.dispatch' }),
                            disabled:
                                !record.publishedVersion ||
                                !access['ElsaWorkflow.Definitions.Dispatch'],
                        },
                        {
                            type: 'divider' as const,
                        },
                        {
                            key: 'publish',
                            name: intl.formatMessage({ id: 'page.definition.publish' }),
                            disabled:
                                record.publishedVersion != null ||
                                !access['ElsaWorkflow.Definitions.CreateOrUpdateOrPublish'],
                        },
                        {
                            key: 'unpublish',
                            name: intl.formatMessage({ id: 'page.definition.unpublish' }),
                            disabled:
                                record.publishedVersion == null ||
                                !access['ElsaWorkflow.Definitions.CreateOrUpdateOrPublish'],
                        },
                        {
                            type: 'divider' as const,
                        },
                        {
                            key: 'copyable',
                            name: intl.formatMessage({ id: 'page.definition.copyable' }),
                            disabled: !access['ElsaWorkflow.Definitions.CreateOrUpdateOrPublish'],
                        },
                        {
                            type: 'divider' as const,
                        },
                        {
                            key: 'delete',
                            name: intl.formatMessage({ id: 'common.dict.delete' }),
                            danger: true,
                            disabled: !access['ElsaWorkflow.Definitions.Delete'],
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
                search={{ labelWidth: 140 }}
                scroll={{ x: 1300 }}
                rowKey="id"
                toolBarRender={() => [
                    access['ElsaWorkflow.Definitions.Export'] && (
                        <Button
                            key="export"
                            type="default"
                            loading={exporting}
                            onClick={() => {
                                handleExport();
                            }}
                            icon={<DownloadOutlined />}
                        >
                            {intl.formatMessage({ id: 'common.dict.export' })}
                        </Button>
                    ),
                    access['ElsaWorkflow.Definitions.Import'] && (
                        <Button
                            key="import"
                            type="default"
                            onClick={() => {
                                setImportModalVisible(true);
                            }}
                        >
                            {intl.formatMessage({ id: 'common.dict.import' })}
                        </Button>
                    ),
                    access['ElsaWorkflow.Definitions.CreateOrUpdateOrPublish'] && (
                        <Button
                            key="add"
                            type="primary"
                            onClick={() => {
                                history.push('/designer');
                            }}
                        >
                            {intl.formatMessage({ id: 'common.dict.create' })}
                        </Button>
                    ),
                ]}
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
                }}
                beforeSearchSubmit={(params) => {
                    // update filter
                    setTableQueryConfig({ ...tableQueryConfig, filter: params });
                    return params;
                }}
                request={async (params) => {
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

            {/* Edit */}
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
                    const success = await handleEdit(editModalDataId!, {
                        ...editModalData,
                        ...value,
                        groupId: value.groupId,
                    });

                    if (success) {
                        setEditModalVisible(false);
                        tableActionRef.current?.reload();
                    }
                }}
            >
                <EditFormItems />
            </ModalForm>

            {/* dispatch */}
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
            {/* IAM */}
            <Modal
                title={`${intl.formatMessage({ id: 'page.definition.iam' })} - ${
                    editModalData?.name
                }`}
                width={600}
                open={iampModalVisible}
                onCancel={() => setIampModalVisible(false)}
                maskClosable={false}
                destroyOnClose
                footer={false}
            >
                <Tabs
                    defaultActiveKey="owners"
                    items={[
                        {
                            key: 'owners',
                            label: intl.formatMessage({ id: 'page.definition.iam.owners' }),
                            children: (
                                <Table
                                    columns={[
                                        {
                                            dataIndex: 'userName',
                                            title: intl.formatMessage({
                                                id: 'page.user.field.userName',
                                            }),
                                        },
                                        {
                                            dataIndex: 'email',
                                            title: intl.formatMessage({
                                                id: 'page.user.field.email',
                                            }),
                                        },
                                        {
                                            dataIndex: 'name',
                                            title: intl.formatMessage({
                                                id: 'page.user.field.name',
                                            }),
                                            render: (value, record) => formatUserName(record),
                                        },
                                        {
                                            dataIndex: '_actions',
                                            width: 100,
                                            align: 'center',
                                            title: intl.formatMessage({
                                                id: 'common.dict.table-action',
                                            }),
                                            render: (value, record) => [
                                                <Popconfirm
                                                    key="delete"
                                                    title={intl.formatMessage({
                                                        id: 'common.dict.delete.confirm',
                                                    })}
                                                    onConfirm={async () => {
                                                        if (await handleDeleteOwner(record.id!)) {
                                                            await loadIamData(editModalDataId!);
                                                        }
                                                    }}
                                                >
                                                    <a>
                                                        {intl.formatMessage({
                                                            id: 'common.dict.delete',
                                                        })}
                                                    </a>
                                                </Popconfirm>,
                                            ],
                                        },
                                    ]}
                                    rowKey="id"
                                    size="small"
                                    dataSource={iamData?.owners ?? []}
                                    pagination={false}
                                />
                            ),
                        },
                        {
                            key: 'teams',
                            label: intl.formatMessage({ id: 'page.definition.iam.teams' }),
                            children: (
                                <Table
                                    columns={[
                                        {
                                            dataIndex: 'name',
                                            title: intl.formatMessage({
                                                id: 'page.workflowTeam.field.name',
                                            }),
                                        },
                                        {
                                            dataIndex: 'description',
                                            title: intl.formatMessage({
                                                id: 'page.workflowTeam.field.description',
                                            }),
                                        },
                                    ]}
                                    rowKey="id"
                                    size="small"
                                    dataSource={iamData?.teams ?? []}
                                    pagination={false}
                                />
                            ),
                        },
                    ]}
                />
            </Modal>

            <Drawer
                open={importModalVisible}
                onClose={() => setImportModalVisible(false)}
                maskClosable={false}
                width={500}
                title={intl.formatMessage({
                    id: 'page.definition.import',
                })}
                footer={false}
            >
                {importResult && !importing && (
                    <Alert
                        style={{ marginBottom: '10px' }}
                        showIcon
                        type={importResult?.failedCount > 0 ? 'warning' : 'success'}
                        message={intl.formatMessage(
                            { id: 'page.definition.import.result.summary' },
                            {
                                success: importResult?.successCount,
                                failed: importResult?.failedCount,
                            },
                        )}
                        action={
                            <Button
                                size="small"
                                onClick={() => {
                                    setImportResultTableVisible(true);
                                }}
                            >
                                {intl.formatMessage({
                                    id: 'page.definition.import.result.summary.table',
                                })}
                            </Button>
                        }
                    />
                )}
                <Form disabled={importing}>
                    <Form.Item
                        name="overwrite"
                        label=""
                        extra={intl.formatMessage({
                            id: 'page.definition.import.overwrite.tips',
                        })}
                    >
                        <Checkbox
                            onChange={(e) => {
                                setImportOverwrite(e.target.checked);
                            }}
                        >
                            {intl.formatMessage({
                                id: 'page.definition.import.overwrite',
                            })}
                        </Checkbox>
                    </Form.Item>
                    <Form.Item>
                        <Upload
                            name="file"
                            accept=".zip,.json"
                            listType="picture"
                            maxCount={1}
                            multiple={false}
                            withCredentials={true}
                            customRequest={async ({ file, filename, onSuccess, onError }) => {
                                await handleImport(file, filename, onSuccess, onError);
                            }}
                        >
                            <Button icon={<UploadOutlined />}>
                                {intl.formatMessage({
                                    id: 'common.dict.uploadButton',
                                })}
                            </Button>
                        </Upload>
                    </Form.Item>
                </Form>
            </Drawer>

            {/* import results */}
            <Modal
                open={importResultTableVisible}
                onCancel={() => setImportResultTableVisible(false)}
                width={680}
                maskClosable={false}
                footer={false}
                title={intl.formatMessage({ id: 'page.definition.import.result.table.tableTitle' })}
            >
                <Table
                    rowKey="fileName"
                    columns={[
                        {
                            dataIndex: 'fileName',
                            title: intl.formatMessage({
                                id: 'page.definition.import.result.table.fileName',
                            }),
                            ellipsis: true,
                        },
                        {
                            dataIndex: 'hasError',
                            title: intl.formatMessage({
                                id: 'page.definition.import.result.table.retult',
                            }),
                            render: (txt, record) => {
                                return txt ? (
                                    <Tooltip title={record.errorMessage}>
                                        <Tag color="error">
                                            {intl.formatMessage({
                                                id: 'page.definition.import.result.table.retult.error',
                                            })}
                                        </Tag>
                                    </Tooltip>
                                ) : (
                                    <Tag color="success">
                                        {intl.formatMessage({
                                            id: 'page.definition.import.result.table.retult.success',
                                        })}
                                    </Tag>
                                );
                            },
                        },
                        {
                            dataIndex: ['workflow', 'name'],
                            title: intl.formatMessage({ id: 'page.definition.field.name' }),
                            ellipsis: true,
                        },
                        {
                            dataIndex: ['workflow', 'displayName'],
                            title: intl.formatMessage({ id: 'page.definition.field.displayName' }),
                            ellipsis: true,
                        },
                        {
                            dataIndex: ['workflow', 'publishedVersion'],
                            title: intl.formatMessage({
                                id: 'page.definition.field.publishedVersion',
                            }),
                            width: 120,
                        },
                    ]}
                    dataSource={importResult?.results ?? []}
                    pagination={false}
                />
            </Modal>
        </PageContainer>
    );
};

export default Index;
