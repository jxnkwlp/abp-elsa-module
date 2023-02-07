import MonacoEditor from '@/components/MonacoEditor';
import { WorkflowInstanceStatus } from '@/services/enums';
import type { API } from '@/services/typings';
import { getWorkflowDefinitionVersion } from '@/services/WorkflowDefinition';
import {
    getWorkflowInstance,
    getWorkflowInstanceExecutionLogs,
    getWorkflowInstanceLogSummary,
} from '@/services/WorkflowInstance';
import {
    ClockCircleOutlined,
    FieldTimeOutlined,
    FunctionOutlined,
    StockOutlined,
} from '@ant-design/icons';
import { ProCard } from '@ant-design/pro-card';
import { ProDescriptions } from '@ant-design/pro-descriptions';
import { PageContainer } from '@ant-design/pro-layout';
import {
    Alert,
    Button,
    Card,
    Col,
    Empty,
    Modal,
    Row,
    Space,
    Tabs,
    Tag,
    Timeline,
    Tooltip,
} from 'antd';
import moment from 'moment';
import React, { useEffect, useRef, useState } from 'react';
import { useHistory, useIntl, useParams } from 'umi';
import type { FlowActionType } from '../designer/flow';
import Flow from '../designer/flow';
import { conventToGraphData } from '../designer/service';
import type { IGraphData } from '../designer/type';
import './detail.less';
import { workflowStatusEnum } from './status';
import type { WorkflowInstanceActivitySummaryInfo } from './types';

const dataRender = (value: string) => {
    return (
        <MonacoEditor
            value={value}
            options={{
                minimap: { enabled: false },
                readOnly: true,
                scrollBeyondLastLine: false,
            }}
            language="json"
        />
    );
};

const Index: React.FC = () => {
    const history = useHistory();
    const params = useParams();
    const intl = useIntl();

    const flowAction = useRef<FlowActionType>();

    const [id, setId] = React.useState<string>();
    const [title, setTitle] = React.useState<string>();
    const [data, setData] = React.useState<API.WorkflowInstance>();
    const [loading, setLoading] = React.useState<boolean>(false);
    const [definition, setDefinition] = React.useState<API.WorkflowDefinitionVersion>();
    const [graphData, setGraphData] = React.useState<IGraphData>();
    const [logs, setLogs] = React.useState<API.WorkflowExecutionLog[]>([]);
    const [logSummary, setLogSummary] = useState<API.WorkflowInstanceExecutionLogSummary>();

    const [activitiesSummary, setActivitiesSummary] = useState<
        WorkflowInstanceActivitySummaryInfo[]
    >([]);

    const [tabKey, setTabKey] = React.useState<string>('logs');

    const [graphInit, setGraphInit] = React.useState<boolean>(false);

    const [selectActivityId, setSelectActivityId] = useState('');
    const [selectActivityData, setSelectActivityData] =
        useState<WorkflowInstanceActivitySummaryInfo>();

    const getActivityTabItems = () => {
        const list = [
            {
                key: 'general',
                label: intl.formatMessage({ id: 'page.instance.general' }),
                children: (
                    <ProDescriptions
                        dataSource={selectActivityData}
                        column={1}
                        columns={[
                            {
                                title: intl.formatMessage({ id: 'page.instance.activityId' }),
                                dataIndex: 'activityId',
                                copyable: true,
                            },
                            {
                                title: intl.formatMessage({ id: 'page.instance.activityType' }),
                                dataIndex: 'activityType',
                                copyable: true,
                            },
                            {
                                title: intl.formatMessage({ id: 'page.definition.field.name' }),
                                dataIndex: 'name',
                                copyable: true,
                            },
                            {
                                title: intl.formatMessage({
                                    id: 'page.definition.field.displayName',
                                }),
                                dataIndex: 'displayName',
                                copyable: true,
                            },
                            {
                                title: intl.formatMessage({ id: 'page.instance.outcomes' }),
                                dataIndex: 'outcomes',
                                renderText: (_) => {
                                    return (_ ?? []).length == 0
                                        ? '-'
                                        : (_ ?? []).map((x) => {
                                              return <Tag key={x}>{x}</Tag>;
                                          });
                                },
                            },
                        ]}
                    />
                ),
            },
            {
                key: 'stateData',
                label: intl.formatMessage({ id: 'page.instance.stateData' }),
                children: (
                    <div className="activity-data-render-container ">
                        {dataRender(JSON.stringify(selectActivityData?.stateData ?? {}, null, 2))}
                    </div>
                ),
            },
            {
                key: 'journalData',
                label: intl.formatMessage({ id: 'page.instance.journalData' }),
                children: (
                    <div className="activity-data-render-container ">
                        {dataRender(JSON.stringify(selectActivityData?.journalData ?? {}, null, 2))}
                    </div>
                ),
            },
        ];

        return list;
    };

    const loadWorkflowDefinition = async (definitionId: string, version: number) => {
        const result = await getWorkflowDefinitionVersion(definitionId, version);

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

        setDefinition(result);

        const data = await conventToGraphData(result.activities ?? [], result.connections ?? []);
        setGraphData(data);
    };

    const loadLogs = async (id: string) => {
        const result = await getWorkflowInstanceExecutionLogs(id);

        setLogs(result?.items ?? []);
    };

    const loadLogsSummary = async (id: string) => {
        const result = await getWorkflowInstanceLogSummary(id);

        setLogSummary(result);
    };

    const loadData = async (id: string) => {
        setLoading(true);
        const result = await getWorkflowInstance(id);

        if (result) {
            setData(result);
            setTitle(result?.name);
        } else {
            Modal.error({
                title: intl.formatMessage({ id: 'common.dict.error.tips' }),
                content: intl.formatMessage({ id: 'page.instance.notFound' }),
                onOk: () => {
                    history.goBack();
                },
            });
            return;
        }

        await loadLogs(id);
        await loadLogsSummary(id);
        await loadWorkflowDefinition(result.workflowDefinitionId!, result.version!);

        setLoading(false);
    };

    useEffect(() => {
        if (selectActivityId) {
            const item = activitiesSummary?.find((x) => x.activityId == selectActivityId);

            if (item) {
                setSelectActivityData(item ?? undefined);
                setTabKey('activityState');
            } else {
                setTabKey('logs');
                // message.warning('Unactivity');
            }
        }
    }, [selectActivityId]);

    const updateGraphNodeStatus = async () => {
        const list = (logSummary?.activities ?? []).map((x) => {
            const nodeDefintion = definition?.activities?.find((d) => d.activityId == x.activityId);
            return {
                ...x,
                name: nodeDefintion?.name ?? '',
                displayName: nodeDefintion?.displayName ?? '',
            } as WorkflowInstanceActivitySummaryInfo;
        });
        setActivitiesSummary(list);

        list.forEach((item) => {
            if (item.isFaulted) {
                flowAction.current?.setNodeStatus(item.activityId!, 'failed');
            } else if (item.isExecuted) {
                flowAction.current?.setNodeStatus(item.activityId!, 'success');
            } else if (item.isExecuting) {
                flowAction.current?.setNodeStatus(item.activityId!, 'running');
            }

            if ((item.outcomes ?? []).length > 0) {
                (item.outcomes ?? []).forEach((outcome) => {
                    const edge = definition?.connections?.find(
                        (x) => x.outcome == outcome && x.sourceId == item.activityId,
                    );
                    if (edge) {
                        flowAction.current?.setNodeOutgoingEdgeStyle(
                            item.activityId!,
                            outcome,
                            'success',
                        );
                    }
                });
            }
        });

        // processing
        if (data?.blockingActivities) {
            data.blockingActivities.forEach((item) => {
                flowAction.current?.setNodeStatus(item.activityId!, 'running');
            });
        }

        // error
        (data?.faults ?? []).forEach((item) => {
            if (item.faultedActivityId) {
                flowAction.current?.setNodeStatus(item.faultedActivityId!, 'failed');
            }
        });
    };

    const updateGraph = async () => {
        flowAction.current?.setAllNodeStatus('inactive');
        flowAction.current?.setAllEdgesStyle('inactive');

        await updateGraphNodeStatus();
    };

    const reload = async () => {
        setSelectActivityId('');
        setTabKey('logs');
        await loadData(id!);
        await updateGraph();
    };

    useEffect(() => {
        if (graphInit && data) {
            updateGraph();
        }
    }, [graphInit, data, definition, logs, logSummary]);

    useEffect(() => {
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
            loading={loading}
            extra={[
                <Button
                    key="reload"
                    type="primary"
                    loading={loading}
                    disabled={!id}
                    onClick={() => {
                        reload();
                    }}
                >
                    {intl.formatMessage({ id: 'page.instance.reload' })}
                </Button>,
            ]}
        >
            {(data?.faults?.length ?? 0) > 0 && (
                <Alert
                    showIcon
                    message={data!.faults![0].message}
                    type="error"
                    description=""
                    style={{ marginBottom: 10 }}
                />
            )}

            <ProCard
                title={intl.formatMessage({ id: 'page.instance.general' })}
                style={{ marginBottom: 16 }}
                collapsible
            >
                <ProDescriptions
                    dataSource={data}
                    columns={[
                        {
                            title: intl.formatMessage({ id: 'page.instance.field.name' }),
                            dataIndex: 'name',
                            copyable: true,
                        },
                        {
                            title: intl.formatMessage({ id: 'page.instance.field.correlationId' }),
                            dataIndex: 'correlationId',
                            copyable: true,
                        },
                        {
                            title: intl.formatMessage({ id: 'page.instance.field.version' }),
                            dataIndex: 'version',
                            copyable: true,
                        },
                        {
                            title: intl.formatMessage({ id: 'page.instance.field.workflowStatus' }),
                            dataIndex: 'workflowStatus',
                            valueEnum: workflowStatusEnum,
                        },
                        {
                            title: intl.formatMessage({ id: 'common.dict.creationTime' }),
                            dataIndex: 'creationTime',
                            valueType: 'dateTime',
                        },
                        {
                            title: intl.formatMessage({ id: 'page.instance.field.finishedTime' }),
                            dataIndex: 'finishedTime',
                            valueType: 'dateTime',
                        },
                        {
                            title: intl.formatMessage({ id: 'page.instance.field.faultedTime' }),
                            dataIndex: 'faultedTime',
                            valueType: 'dateTime',
                        },
                        {
                            title: intl.formatMessage({
                                id: 'page.instance.field.lastExecutedTime',
                            }),
                            dataIndex: 'lastExecutedTime',
                            valueType: 'dateTime',
                        },
                    ]}
                />
            </ProCard>

            <Row gutter={16}>
                <Col span={14} md={14} sm={24}>
                    <Card title={intl.formatMessage({ id: 'page.instance.graph' })}>
                        <Flow
                            readonly
                            actionRef={flowAction}
                            showMiniMap={false}
                            showNodeTypes={false}
                            showToolbar={false}
                            graphData={graphData}
                            onDataUpdate={() => {
                                setGraphInit(true);
                            }}
                            onNodeClick={(_c, node) => {
                                setSelectActivityId(node.id!);
                            }}
                            onBlankClick={() => {
                                setSelectActivityData(undefined);
                                setSelectActivityId('');
                                setTabKey('logs');
                            }}
                        />
                    </Card>
                </Col>
                <Col span={10} md={10} sm={24}>
                    <Card
                        title=""
                        activeTabKey={tabKey}
                        tabList={[
                            {
                                key: 'activityState',
                                tab: intl.formatMessage({
                                    id: 'page.instance.activityState',
                                }),
                            },
                            {
                                key: 'logs',
                                tab: intl.formatMessage({
                                    id: 'page.instance.timeline',
                                }),
                            },
                            {
                                key: 'input',
                                tab: intl.formatMessage({
                                    id: 'page.instance.input',
                                }),
                            },
                            {
                                key: 'fault',
                                tab: intl.formatMessage({
                                    id: 'page.instance.exception',
                                }),
                            },
                            {
                                key: 'variables',
                                tab: intl.formatMessage({
                                    id: 'page.instance.variables',
                                }),
                            },
                        ]}
                        onTabChange={(key) => {
                            setTabKey(key);
                        }}
                    >
                        {tabKey == 'activityState' && (
                            <div>
                                {!selectActivityId && (
                                    <Alert
                                        message={intl.formatMessage({
                                            id: 'page.instance.node.select.tips',
                                        })}
                                        showIcon
                                    />
                                )}
                                {selectActivityId && selectActivityData && (
                                    <Tabs items={getActivityTabItems()} />
                                )}
                            </div>
                        )}
                        {tabKey === 'logs' &&
                            ((activitiesSummary ?? []).length ? (
                                <Timeline
                                    mode="left"
                                    pending={
                                        data?.workflowStatus == WorkflowInstanceStatus.Running ||
                                        data?.workflowStatus == WorkflowInstanceStatus.Suspended
                                    }
                                    reverse
                                >
                                    {activitiesSummary.map((item) => {
                                        return (
                                            <Timeline.Item
                                                key={item.activityId}
                                                color={
                                                    item.isExecuting
                                                        ? 'gray'
                                                        : item.isFaulted
                                                        ? 'red'
                                                        : 'green'
                                                }
                                                dot={
                                                    item.isExecuting ? (
                                                        <ClockCircleOutlined
                                                            style={{ fontSize: '12px' }}
                                                        />
                                                    ) : null
                                                }
                                            >
                                                <Space>
                                                    <Tooltip
                                                        title={item.activityId}
                                                        placement="left"
                                                    >
                                                        <span style={{ fontSize: 16 }}>
                                                            {item.displayName}
                                                        </span>
                                                    </Tooltip>
                                                    <Tag>
                                                        <FieldTimeOutlined />{' '}
                                                        {moment(item.startTime).format()}
                                                    </Tag>
                                                    <Tag>
                                                        <FunctionOutlined /> {item.activityType}
                                                    </Tag>
                                                    <Tag>
                                                        <StockOutlined />{' '}
                                                        {/* {moment
                                                            .duration(
                                                                item.duration?.toFixed(0),
                                                                'milliseconds',
                                                            )
                                                            .humanize()} */}
                                                        {item.duration?.toFixed(0)} ms
                                                    </Tag>
                                                </Space>
                                                {/* {item.status}  */}
                                                {item.message && <p>{item.message}</p>}
                                            </Timeline.Item>
                                        );
                                    })}
                                </Timeline>
                            ) : (
                                <Empty />
                            ))}

                        {tabKey === 'input' &&
                            (data?.input ? (
                                <>
                                    <div className="data-render-container">
                                        {dataRender(JSON.stringify(data.input ?? {}, null, 2))}
                                    </div>
                                </>
                            ) : (
                                <Empty />
                            ))}
                        {tabKey === 'fault' &&
                            (data?.faults?.length ? (
                                <>
                                    <div className="data-render-container">
                                        {dataRender(JSON.stringify(data.faults ?? [], null, 2))}
                                    </div>
                                </>
                            ) : (
                                <Empty />
                            ))}
                        {tabKey === 'variables' &&
                            (Object.keys(data?.variables ?? {}).length ? (
                                <>
                                    <div className="data-render-container">
                                        {dataRender(JSON.stringify(data?.variables ?? {}, null, 2))}
                                    </div>
                                </>
                            ) : (
                                <Empty />
                            ))}
                    </Card>
                </Col>
            </Row>
        </PageContainer>
    );
};

export default Index;
