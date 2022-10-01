import { WorkflowStatus } from '@/services/enums';
import type { API } from '@/services/typings';
import { getWorkflowDefinitionVersion } from '@/services/WorkflowDefinition';
import {
    getWorkflowInstance,
    getWorkflowInstanceExecutionLogs,
    getWorkflowInstanceLogSummary,
} from '@/services/WorkflowInstance';
import { ClockCircleOutlined, FieldTimeOutlined, FunctionOutlined } from '@ant-design/icons';
import { ProCard } from '@ant-design/pro-card';
import { ProDescriptions } from '@ant-design/pro-descriptions';
import { PageContainer } from '@ant-design/pro-layout';
import { Alert, Card, Col, Modal, Row, Space, Tabs, Tag, Timeline } from 'antd';
import moment from 'moment';
import React, { useEffect, useRef, useState } from 'react';
import MonacoEditor from 'react-monaco-editor';
import { useHistory, useLocation, useParams } from 'umi';
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
                automaticLayout: true,
                scrollBeyondLastLine: false,
            }}
            language="json"
        />
    );
};

const Index: React.FC = () => {
    const history = useHistory();
    const location = useLocation();
    const params = useParams();

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
                label: 'General',
                children: (
                    <ProDescriptions
                        dataSource={selectActivityData}
                        column={1}
                        columns={[
                            {
                                title: 'Activity Id',
                                dataIndex: 'activityId',
                                copyable: true,
                            },
                            {
                                title: 'Name',
                                dataIndex: 'name',
                                copyable: true,
                            },
                            {
                                title: 'Display Name',
                                dataIndex: 'displayName',
                                copyable: true,
                            },
                            {
                                title: 'Outcomes',
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
                label: 'State',
                children: (
                    <div className="data-render-container">
                        {dataRender(JSON.stringify(selectActivityData?.stateData ?? {}, null, 2))}
                    </div>
                ),
            },
            {
                key: 'journalData',
                label: 'Journal Data',
                children: (
                    <div className="data-render-container">
                        {dataRender(JSON.stringify(selectActivityData?.journalData ?? {}, null, 2))}
                    </div>
                ),
            },
        ];

        console.log(selectActivityData);

        return list;
    };

    const loadWorkflowDefinition = async (definitionId: string, version: number) => {
        const result = await getWorkflowDefinitionVersion(definitionId, version);

        if (!result) {
            Modal.error({
                title: 'Error',
                content: 'The workflow defintion does not exist.',
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
                title: 'Error',
                content: 'The workflow instance does not exist.',
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
            console.log(item);
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
        console.log(list);

        list.forEach((item) => {
            if (item.isFaulted) {
                flowAction.current?.setNodeStyle(item.activityId!, 'error');
            } else if (item.isExecuted) {
                flowAction.current?.setNodeStyle(item.activityId!, 'success');
            } else if (item.isExecuting) {
                flowAction.current?.setNodeStyle(item.activityId!, 'processing');
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
                flowAction.current?.setNodeStyle(item.activityId!, 'processing');
            });
        }

        // error
        if (data?.fault?.faultedActivityId) {
            flowAction.current?.setNodeStyle(data.fault.faultedActivityId!, 'error');
        }
    };

    useEffect(() => {
        if (graphInit && data) {
            flowAction.current?.setAllNodesStyle('default');
            flowAction.current?.setAllEdgesStyle('default');

            updateGraphNodeStatus();
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
        <PageContainer title={title} loading={loading}>
            {data?.fault && (
                <Alert
                    showIcon
                    message={data?.fault.message}
                    type="error"
                    description=""
                    style={{ marginBottom: 10 }}
                />
            )}

            <ProCard title="General" style={{ marginBottom: 16 }}>
                <ProDescriptions
                    dataSource={data}
                    columns={[
                        { title: 'Name', dataIndex: 'name', copyable: true },
                        { title: 'Correlation Id', dataIndex: 'correlationId', copyable: true },
                        { title: 'Version', dataIndex: 'version', copyable: true },
                        {
                            title: 'Status',
                            dataIndex: 'workflowStatus',
                            valueEnum: workflowStatusEnum,
                        },
                        { title: 'Created', dataIndex: 'creationTime', valueType: 'dateTime' },
                        { title: 'Finished', dataIndex: 'finishedTime', valueType: 'dateTime' },
                        { title: 'Faulted', dataIndex: 'faultedTime', valueType: 'dateTime' },
                        {
                            title: 'Last Executed',
                            dataIndex: 'lastExecutedTime',
                            valueType: 'dateTime',
                        },
                    ]}
                />
            </ProCard>

            <Row gutter={16}>
                <Col span={14}>
                    <Card title="Graph">
                        <Flow
                            readonly
                            actionRef={flowAction}
                            showMiniMap={false}
                            showNodeTypes={false}
                            showToolbar={false}
                            graphData={graphData}
                            onDataUpdate={(g) => {
                                setGraphInit(true);
                            }}
                            onNodeClick={(c, node) => {
                                setSelectActivityId(node.id!);
                            }}
                            onBlankClick={() => {
                                setSelectActivityId('');
                                setTabKey('logs');
                            }}
                        />
                    </Card>
                </Col>
                <Col span={10}>
                    <Card
                        title=""
                        tabProps={{}}
                        activeTabKey={tabKey}
                        tabList={[
                            { key: 'activityState', tab: 'Activity State' },
                            { key: 'logs', tab: 'Timeline' },
                            { key: 'input', tab: 'Input' },
                            { key: 'fault', tab: 'Exception' },
                            { key: 'variables', tab: 'Variables' },
                            { key: 'data', tab: 'Data' },
                        ]}
                        onTabChange={(key) => {
                            setTabKey(key);
                        }}
                    >
                        {tabKey == 'activityState' && (
                            <div>
                                {!selectActivityId && (
                                    <Alert message="Please select an node first." />
                                )}
                                {selectActivityId && <Tabs items={getActivityTabItems()} />}
                            </div>
                        )}
                        {tabKey === 'logs' && (
                            <Timeline
                                mode="left"
                                pending={
                                    data?.workflowStatus == WorkflowStatus.Running ||
                                    data?.workflowStatus == WorkflowStatus.Suspended
                                }
                                reverse
                            >
                                {(activitiesSummary ?? []).map((item) => {
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
                                            // label={moment(item.startTime).format(
                                            //     'YYYY-MM-DD HH:mm:ss',
                                            // )}
                                        >
                                            <Space>
                                                <span style={{ fontSize: 16 }}>
                                                    {item.displayName}
                                                </span>
                                                <Tag>
                                                    <FieldTimeOutlined />{' '}
                                                    {moment(item.startTime).format(
                                                        'YYYY-MM-DD HH:mm:ss',
                                                    )}
                                                </Tag>
                                                <Tag>
                                                    <FunctionOutlined /> {item.activityType}
                                                </Tag>
                                            </Space>
                                            {/* {item.status}  */}
                                            {item.message && <p>{item.message}</p>}
                                        </Timeline.Item>
                                    );
                                })}
                            </Timeline>
                        )}
                        {tabKey === 'input' && (
                            <div className="data-render-container">
                                {dataRender(JSON.stringify(data?.input ?? {}, null, 2))}
                            </div>
                        )}
                        {tabKey === 'fault' && (
                            <div className="data-render-container">
                                {dataRender(JSON.stringify(data?.fault ?? {}, null, 2))}
                            </div>
                        )}
                        {tabKey === 'variables' && (
                            <div className="data-render-container">
                                {dataRender(JSON.stringify(data?.variables ?? {}, null, 2))}
                            </div>
                        )}
                        {tabKey === 'data' && (
                            <div className="data-render-container">
                                {dataRender(JSON.stringify(data?.activityData ?? {}, null, 2))}
                            </div>
                        )}
                    </Card>
                </Col>
            </Row>
        </PageContainer>
    );
};

export default Index;
