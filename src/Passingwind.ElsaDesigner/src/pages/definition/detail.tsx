import {
    getWorkflowDefinition,
    getWorkflowDefinitionDefinition,
    getWorkflowDefinitionVariables,
    getWorkflowDefinitionVersion,
    getWorkflowDefinitionVersions,
    updateWorkflowDefinitionVariables,
} from '@/services/WorkflowDefinition';
import {
    getWorkflowInstanceFaultsByWorkflowDefinition,
    getWorkflowInstanceList,
    getWorkflowInstanceStatusCountStatistics,
    getWorkflowInstanceStatusDateCountStatistics,
} from '@/services/WorkflowInstance';
import { WorkflowInstanceStatus, WorkflowPersistenceBehavior } from '@/services/enums';
import type { API } from '@/services/typings';
import { Line, Pie } from '@ant-design/charts';
import { DownOutlined } from '@ant-design/icons';
import { ProDescriptions, ProTable } from '@ant-design/pro-components';
import { PageContainer } from '@ant-design/pro-layout';
import { Alert, Button, Card, Col, Dropdown, Modal, Row, Space, Tag, message } from 'antd';
import moment from 'moment';
import React, { useEffect, useRef } from 'react';
import { Access, Link, useAccess, useHistory, useIntl, useParams } from 'umi';
import type { FlowActionType } from '../designer/flow';
import Flow from '../designer/flow';
import { conventToGraphData } from '../designer/service';
import type { IGraphData } from '../designer/type';
import { workflowStatusEnum } from '../instance/status';
import './detail.less';
import VariableForm from './variableForm';

const Index: React.FC = () => {
    const history = useHistory();
    const params = useParams();
    const intl = useIntl();
    const access = useAccess();

    const flowAction = useRef<FlowActionType>();

    const [id, setId] = React.useState<string>();
    const [title, setTitle] = React.useState<string>();
    const [data, setData] = React.useState<API.WorkflowDefinition>();
    const [versionData, setVersionData] = React.useState<API.WorkflowDefinitionVersion>();
    const [loading, setLoading] = React.useState<boolean>(false);
    const [graphData, setGraphData] = React.useState<IGraphData>();
    const [versionList, setVersionList] = React.useState<API.WorkflowDefinitionVersionListItem[]>(
        [],
    );

    const [topListLoading, setTopListLoading] = React.useState(false);
    const [topList, setTopList] = React.useState<any>();
    const [topListGroup, setTopListGroup] = React.useState<string>('time');

    const [statisticsLoading, setStatisticsLoading] = React.useState(false);
    const [statisticsDateCountData, setStatisticsDateCountData] = React.useState<any>([]);
    const [statisticsStatusCountData, setStatisticsStatusCountData] =
        React.useState<API.WorkflowInstanceStatusCountStatisticsResult>();

    const [errorListLoading, setErrorListLoading] = React.useState(false);
    const [errorList, setErrorList] = React.useState<API.WorkflowInstanceFault[]>();

    const [variableEditModalVisible, setVariableEditModalVisible] = React.useState<boolean>();
    const [variableData, setVariableData] = React.useState<any>();

    const loadVersion = async (id: string, version?: number) => {
        let _data: API.WorkflowDefinitionVersion;
        if (version) _data = await getWorkflowDefinitionVersion(id, version);
        else _data = await getWorkflowDefinition(id);

        if (!_data) {
            Modal.error({
                title: intl.formatMessage({ id: 'common.dict.error.tips' }),
                content: intl.formatMessage({ id: 'page.definition.versionNotFound' }),
                onOk: () => {
                    history.goBack();
                },
            });
            return;
        }

        setVersionData(_data);

        const data = await conventToGraphData(_data.activities ?? [], _data.connections ?? []);
        setGraphData(data);
    };

    const loadData = async (id: string) => {
        setLoading(true);

        // defintion
        const result = await getWorkflowDefinitionDefinition(id);
        setData(result);
        // const result = await getWorkflowDefinitionVersion(definitionId, version);

        if (!result) {
            Modal.error({
                title: intl.formatMessage({ id: 'common.dict.error.tips' }),
                content: intl.formatMessage({ id: 'page.definition.notFound' }),
                onOk: () => {
                    history.goBack();
                },
            });
            return;
        }

        setTitle(result.displayName);

        // version
        await loadVersion(id, result.latestVersion!);

        // all version
        const versionList = await getWorkflowDefinitionVersions(id, { maxResultCount: 1000 });
        setVersionList(versionList?.items ?? []);
        //
        setLoading(false);
    };

    const handleSwitchVersion = async (version: number) => {
        setLoading(true);
        await loadVersion(id!, version);
        setLoading(false);
    };

    const loadVariable = async () => {
        const loading = message.loading(intl.formatMessage({ id: 'common.dict.loading' }));
        const result = await getWorkflowDefinitionVariables(id!);
        loading();
        setVariableData(result.variables);
        setVariableEditModalVisible(true);
    };

    const handleUpdateVariable = async (values: any) => {
        const loading = message.loading(intl.formatMessage({ id: 'common.dict.loading' }));
        const result = await updateWorkflowDefinitionVariables(id!, { variables: values });
        loading();
        message.success(intl.formatMessage({ id: 'common.dict.save.success' }));
        return result != null;
    };

    const loadTopList = async (category: string) => {
        setTopListLoading(true);
        setTopListGroup(category);
        let status = null;
        let sorting: string = 'id desc';
        if (category == 'error') status = WorkflowInstanceStatus.Faulted;
        else if (category == 'running') status = WorkflowInstanceStatus.Running;
        else if (category == 'suspended') status = WorkflowInstanceStatus.Suspended;
        else if (category == 'duration') {
            sorting = 'finishedDuration desc';
        }

        const list = await getWorkflowInstanceList({
            workflowDefinitionId: id,
            workflowStatus: status,
            sorting: sorting,
            skipCount: 0,
            maxResultCount: 10,
        });

        setTopListLoading(false);
        setTopList(list?.items ?? []);
    };

    const loadStatistics = async () => {
        setStatisticsLoading(true);
        const tz = moment().utcOffset() / 60;
        const dateCountList = await getWorkflowInstanceStatusDateCountStatistics({
            workflowDefinitionId: id,
            tz: tz,
            datePeriod: 7,
        });
        const list = (dateCountList?.items ?? []).map((x) => {
            return {
                date: moment(x.date).format('MM-DD'),
                count: x.finishedCount,
                type: 'Finished',
            };
        });
        list.push(
            ...(dateCountList?.items ?? []).map((x) => {
                return {
                    date: moment(x.date).format('MM-DD'),
                    count: x.failedCount,
                    type: 'Failed',
                };
            }),
        );

        setStatisticsDateCountData(list);

        const statusCountList = await getWorkflowInstanceStatusCountStatistics({
            workflowDefinitionId: id,
        });
        setStatisticsStatusCountData(statusCountList);
        setStatisticsLoading(false);
    };

    const loadErrors = async () => {
        setErrorListLoading(true);
        const list = await getWorkflowInstanceFaultsByWorkflowDefinition(id!, {
            maxResultCount: 5,
        });
        setErrorList(list?.items ?? []);
        setErrorListLoading(false);
    };

    useEffect(() => {
        if (!id) return;
        if (access['ElsaWorkflow.Instances.Statistics']) {
            loadTopList('time');
            loadStatistics();
            loadErrors();
        }
    }, [id]);

    useEffect(() => {
        // @ts-ignore
        const sid = params.id ?? '';
        if (!sid) {
            history.goBack();
            return;
        }

        setId(sid);
        loadData(sid);
    }, [0]);

    return (
        <PageContainer
            title={title}
            extra={
                <Space>
                    {access['ElsaWorkflow.Definitions.CreateOrUpdateOrPublish'] ? (
                        <>
                            <Button
                                key="designer"
                                type="primary"
                                disabled={!data}
                                onClick={() => history.push(`/designer?id=${data!.id}`)}
                            >
                                {intl.formatMessage({ id: 'page.definition.designer' })}
                            </Button>
                        </>
                    ) : (
                        <></>
                    )}
                </Space>
            }
        >
            <Card
                title={intl.formatMessage({ id: 'common.dict.general' })}
                style={{ marginBottom: 16 }}
                extra={
                    access['ElsaWorkflow.Definitions.CreateOrUpdateOrPublish'] ? (
                        <>
                            <Button
                                key="variables"
                                type="link"
                                disabled={!data}
                                onClick={() => {
                                    loadVariable();
                                }}
                            >
                                {intl.formatMessage({ id: 'page.definition.edit.variables' })}
                            </Button>
                        </>
                    ) : (
                        <></>
                    )
                }
            >
                <ProDescriptions
                    dataSource={data}
                    columns={[
                        {
                            title: intl.formatMessage({ id: 'page.definition.field.name' }),
                            dataIndex: 'name',
                            copyable: true,
                            ellipsis: true,
                        },
                        {
                            title: intl.formatMessage({
                                id: 'page.definition.field.displayName',
                            }),
                            dataIndex: 'displayName',
                            copyable: true,
                            ellipsis: true,
                        },
                        {
                            title: intl.formatMessage({ id: 'page.definition.field.description' }),
                            dataIndex: 'description',
                            ellipsis: true,
                        },
                        {
                            title: intl.formatMessage({ id: 'page.definition.field.tag' }),
                            dataIndex: 'tag',
                        },
                        {
                            title: intl.formatMessage({ id: 'page.definition.field.channel' }),
                            dataIndex: 'channel',
                        },
                        {
                            title: intl.formatMessage({
                                id: 'page.definition.field.persistenceBehavior',
                            }),
                            dataIndex: 'persistenceBehavior',
                            valueEnum: WorkflowPersistenceBehavior,
                        },
                        {
                            title: intl.formatMessage({ id: 'page.definition.field.isSingleton' }),
                            dataIndex: 'isSingleton',
                            valueEnum: {
                                true: 'Y',
                                false: 'N',
                            },
                        },
                        {
                            title: intl.formatMessage({ id: 'common.dict.creationTime' }),
                            dataIndex: 'creationTime',
                            valueType: 'dateTime',
                        },
                        {
                            title: intl.formatMessage({
                                id: 'page.definition.field.lastModificationTime',
                            }),
                            dataIndex: 'lastModificationTime',
                            valueType: 'dateTime',
                        },
                    ]}
                />
            </Card>

            <Access
                accessible={access['ElsaWorkflow.Instances.Statistics']}
                fallback={
                    <Alert
                        type="error"
                        message={intl.formatMessage(
                            { id: 'common.noaccess' },
                            { permission: 'ElsaWorkflow.Instances.Statistics' },
                        )}
                    />
                }
            >
                <Row gutter={12}>
                    <Col span={8}>
                        <Card
                            style={{ marginBottom: 16 }}
                            title={intl.formatMessage({
                                id: 'page.instance.statistics.dateCount',
                            })}
                            loading={statisticsLoading}
                        >
                            <div style={{ height: 235 }}>
                                <Line
                                    data={statisticsDateCountData}
                                    xField="date"
                                    yField="count"
                                    seriesField="type"
                                    smooth
                                    padding="auto"
                                    color={['#52c41a', '#f00']}
                                />
                            </div>
                        </Card>
                    </Col>
                    <Col span={8}>
                        <Card
                            style={{ marginBottom: 16 }}
                            title={intl.formatMessage({
                                id: 'page.instance.statistics.statusCount',
                            })}
                            loading={statisticsLoading}
                        >
                            <div style={{ height: 235 }}>
                                <Pie
                                    data={[
                                        {
                                            type: intl.formatMessage({
                                                id: 'page.dashboard.statistics.status.finished',
                                            }),
                                            value: statisticsStatusCountData?.finished ?? 0,
                                        },
                                        {
                                            type: intl.formatMessage({
                                                id: 'page.dashboard.statistics.status.faulted',
                                            }),
                                            value: statisticsStatusCountData?.faulted ?? 0,
                                        },
                                        {
                                            type: intl.formatMessage({
                                                id: 'page.dashboard.statistics.status.suspended',
                                            }),
                                            value: statisticsStatusCountData?.suspended ?? 0,
                                        },
                                    ]}
                                    color={['#2ca02c', '#d62728', '#000000']}
                                    angleField={'value'}
                                    colorField={'type'}
                                    appendPadding={0}
                                    radius={0.8}
                                    label={{
                                        type: 'inner',
                                        offset: '-30%',
                                        content: ({ percent }) => `${(percent * 100).toFixed(0)}%`,
                                        style: {
                                            fontSize: 14,
                                            textAlign: 'center',
                                        },
                                    }}
                                    interactions={[
                                        {
                                            type: 'element-selected',
                                        },
                                        {
                                            type: 'element-active',
                                        },
                                    ]}
                                />
                            </div>
                        </Card>
                    </Col>
                    <Col span={8}>
                        <Card
                            style={{ marginBottom: 16 }}
                            title={intl.formatMessage({
                                id: 'page.instance.faults.latest',
                            })}
                            loading={errorListLoading}
                        >
                            <ProTable<API.WorkflowInstanceFault>
                                size="small"
                                search={false}
                                columns={[
                                    {
                                        dataIndex: 'message',
                                        title: intl.formatMessage({
                                            id: 'page.instance.field.fault.message',
                                        }),
                                        ellipsis: true,
                                        copyable: true,
                                    },
                                    {
                                        dataIndex: 'creationTime',
                                        title: intl.formatMessage({
                                            id: 'common.dict.creationTime',
                                        }),
                                        valueType: 'dateTime',
                                        width: 150,
                                    },
                                    {
                                        dataIndex: 'action',
                                        title: intl.formatMessage({
                                            id: 'common.dict.table-action',
                                        }),
                                        width: 120,
                                        align: 'center',
                                        renderText: (_text, record) => (
                                            <Link to={`/instances/${record.workflowInstanceId}`}>
                                                {intl.formatMessage({
                                                    id: 'page.definition.goInstanceDetail',
                                                })}
                                            </Link>
                                        ),
                                    },
                                ]}
                                rowKey="id"
                                dataSource={errorList}
                                pagination={false}
                                toolBarRender={false}
                            />
                        </Card>
                    </Col>
                </Row>

                <Card
                    style={{ marginBottom: 16 }}
                    title={intl.formatMessage(
                        {
                            id: 'page.instance.list.latestOfCount',
                        },
                        { number: 10 },
                    )}
                    extra={
                        <Dropdown
                            menu={{
                                items: [
                                    {
                                        label: intl.formatMessage({
                                            id: 'page.instance.list.groupBy.time',
                                        }),
                                        key: 'time',
                                    },
                                    {
                                        label: intl.formatMessage({
                                            id: 'page.instance.list.groupBy.duration',
                                        }),
                                        key: 'duration',
                                    },
                                    {
                                        label: intl.formatMessage({
                                            id: 'page.instance.list.groupBy.error',
                                        }),
                                        key: 'error',
                                    },
                                    {
                                        label: intl.formatMessage({
                                            id: 'page.instance.list.groupBy.running',
                                        }),
                                        key: 'running',
                                    },
                                    {
                                        label: intl.formatMessage({
                                            id: 'page.instance.list.groupBy.suspended',
                                        }),
                                        key: 'suspended',
                                    },
                                ],
                                onClick: (item) => {
                                    loadTopList(item.key);
                                },
                            }}
                        >
                            <Space>
                                {intl.formatMessage({ id: 'page.instance.list.groupBy' })}
                                <a onClick={(e) => e.preventDefault()}>
                                    <Space>
                                        {intl.formatMessage({
                                            id: 'page.instance.list.groupBy.' + topListGroup,
                                        })}
                                        <DownOutlined />
                                    </Space>
                                </a>
                            </Space>
                        </Dropdown>
                    }
                >
                    <ProTable
                        loading={topListLoading}
                        search={false}
                        columns={[
                            {
                                dataIndex: 'name',
                                title: intl.formatMessage({ id: 'page.instance.field.name' }),
                                renderText: (text, record) => (
                                    <Link to={`/instances/${record.id}`}>{text}</Link>
                                ),
                            },
                            {
                                dataIndex: 'version',
                                title: intl.formatMessage({
                                    id: 'page.instance.field.version',
                                }),
                                valueType: 'digit',
                                width: 100,
                            },
                            {
                                dataIndex: 'workflowStatus',
                                title: intl.formatMessage({
                                    id: 'page.instance.field.workflowStatus',
                                }),
                                valueEnum: workflowStatusEnum,
                                width: 120,
                            },
                            {
                                dataIndex: 'creationTime',
                                title: intl.formatMessage({ id: 'common.dict.creationTime' }),
                                valueType: 'dateTime',
                                width: 150,
                            },
                            {
                                dataIndex: 'finishedDuration',
                                title: intl.formatMessage({
                                    id: 'page.instance.field.finishedDuration',
                                }),
                                valueType: 'time',
                                width: 120,
                            },
                        ]}
                        rowKey="id"
                        dataSource={topList}
                        pagination={false}
                        toolBarRender={false}
                    />
                </Card>
            </Access>

            <Card
                loading={loading}
                title={
                    <Space>
                        {intl.formatMessage({ id: 'page.definition.graph' })}
                        <Tag>{`version: ${versionData?.version ?? '-'}`}</Tag>
                    </Space>
                }
                extra={
                    <Dropdown
                        menu={{
                            items: versionList.map((x) => {
                                return { label: x.version!, key: x.version! };
                            }),
                            onClick: async (item) => {
                                await handleSwitchVersion(item.key as any as number);
                            },
                        }}
                    >
                        <a onClick={(e) => e.preventDefault()}>
                            <Space>
                                {intl.formatMessage({ id: 'page.definition.switchVersion' })}
                                <DownOutlined />
                            </Space>
                        </a>
                    </Dropdown>
                }
            >
                <Flow
                    readonly
                    actionRef={flowAction}
                    showMiniMap={false}
                    showNodeTypes={false}
                    showToolbar={false}
                    graphData={graphData}
                />
            </Card>

            {/* variable */}
            <VariableForm
                data={variableData}
                visible={variableEditModalVisible}
                onVisibleChange={setVariableEditModalVisible}
                onSubmit={async (value: any) => {
                    console.debug('update variables', value);
                    return await handleUpdateVariable(value);
                }}
            />
        </PageContainer>
    );
};

export default Index;
