import { WorkflowStatus } from '@/services/enums';
import { getWorkflowDefinitionVersion } from '@/services/WorkflowDefinition';
import { getWorkflowInstance, getWorkflowInstanceExecutionLogs } from '@/services/WorkflowInstance';
import { ClockCircleOutlined } from '@ant-design/icons';
import { PageContainer } from '@ant-design/pro-layout';
import { Alert, Card, Col, Modal, Popover, Row, Tag, Timeline, Typography } from 'antd';
import moment from 'moment';
import React, { useEffect, useRef } from 'react';
import { useHistory, useLocation } from 'umi';
import type { FlowActionType } from '../designer/flow';
import Flow from '../designer/flow';
import { conventToGraphData } from '../designer/service';
import type { IGraphData } from '../designer/type';

import './detail.less';

const Index: React.FC = () => {
    const history = useHistory();
    const location = useLocation();

    const flowAction = useRef<FlowActionType>();

    const [id, setId] = React.useState<string>();
    const [title, setTitle] = React.useState<string>();
    const [data, setData] = React.useState<API.WorkflowInstance>();
    const [loading, setLoading] = React.useState<boolean>(false);
    const [definition, setDefinition] = React.useState<API.WorkflowDefinitionVersion>();
    const [graphData, setGraphData] = React.useState<IGraphData>();

    const [logs, setLogs] = React.useState<API.WorkflowExecutionLog[]>([]);

    const [tabKey, setTabKey] = React.useState<string>('logs');

    const [graphInit, setGraphInit] = React.useState<boolean>(false);

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

    const loadData = async (id: string) => {
        setLoading(true);
        const result = await getWorkflowInstance(id);

        await loadLogs(id);

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

        await loadWorkflowDefinition(result.definitionId!, result.version!);

        setLoading(false);
    };

    const updateGraphEdgeStatus = (targetNodeId: string) => {
        graphData?.edges
            ?.filter((x) => x.target?.cell == targetNodeId)
            .forEach((x) => {
                const nodeLogs = logs.filter((log) => log.activityId === x.source?.cell);
                if (nodeLogs.findIndex((log) => log.eventName == 'Executed') >= 0) {
                    flowAction.current?.setEdgeStyle(x.id!, 'success');
                } else if (nodeLogs.findIndex((log) => log.eventName == 'Executing') >= 0) {
                    flowAction.current?.setEdgeStyle(x.id!, 'processing');
                } else if (nodeLogs.findIndex((log) => log.eventName == 'Faulted') >= 0) {
                    flowAction.current?.setEdgeStyle(x.id!, 'error');
                }
            });
    };

    const updateGraphNodeStatus = (nodeId: string) => {
        const nodeLogs = logs.filter((log) => log.activityId === nodeId);
        if (nodeLogs.findIndex((log) => log.eventName == 'Executed') >= 0) {
            flowAction.current?.setNodeStyle(nodeId, 'success');
        } else if (nodeLogs.findIndex((log) => log.eventName == 'Executing') >= 0) {
            flowAction.current?.setNodeStyle(nodeId, 'processing');
        } else if (nodeLogs.findIndex((log) => log.eventName == 'Faulted') >= 0) {
            flowAction.current?.setNodeStyle(nodeId, 'error');
        }
        updateGraphEdgeStatus(nodeId);
    };

    useEffect(() => {
        if (logs && graphInit && graphData) {
            flowAction.current?.setAllNodesStyle('default');
            flowAction.current?.setAllEdgesStyle('default');
            //
            const activityLogs = {} as Record<string, API.WorkflowExecutionLog[]>;
            // group
            (logs ?? []).forEach((item) => {
                const item2 = activityLogs[item.activityId!];
                if (item2) {
                    item2.push(item);
                } else {
                    activityLogs[item.activityId!] = [item];
                }
            });
            // loop
            for (const key in activityLogs) {
                // const logs = activityLogs[key];
                updateGraphNodeStatus(key);
            }

            //
            if (data?.currentActivity?.activityId) {
                flowAction.current?.setNodeStyle(data.currentActivity.activityId!, 'processing');
                flowAction.current?.setNodeIncomingEdgesStyle(
                    data.currentActivity.activityId!,
                    'processing',
                );
            }

            //
            if (data?.fault?.faultedActivityId) {
                flowAction.current?.setNodeStyle(data.fault.faultedActivityId!, 'error');
            }

            if (data?.blockingActivities) {
                data.blockingActivities.forEach((item) => {
                    flowAction.current?.setNodeStyle(item.activityId!, 'processing');
                });
            }
        }
    }, [data, logs, graphInit, graphData]);

    useEffect(() => {
        const sid = location?.state?.id ?? '';
        if (!sid) {
            history.goBack();
            return;
        }

        setId(sid);

        loadData(sid);
    }, []);

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
            <Row gutter={16}>
                <Col span={14}>
                    <Card
                        title={[
                            'Graph ',
                            <Tag key="2">{data?.version}</Tag>,
                            <Tag key="3">{WorkflowStatus[data?.workflowStatus]}</Tag>,
                        ]}
                    >
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
                        />
                    </Card>
                </Col>
                <Col span={10}>
                    <Card
                        title=""
                        tabProps={{}}
                        activeTabKey={tabKey}
                        tabList={[
                            { key: 'logs', tab: 'Logs' },
                            { key: 'input', tab: 'Input' },
                            { key: 'fault', tab: 'Fault' },
                            { key: 'variables', tab: 'Variables' },
                            { key: 'data', tab: 'Data' },
                        ]}
                        onTabChange={(key) => {
                            setTabKey(key);
                        }}
                    >
                        {tabKey === 'logs' && (
                            <Timeline
                                mode="left"
                                pending={
                                    data?.workflowStatus == WorkflowStatus.Running ||
                                    data?.workflowStatus == WorkflowStatus.Suspended
                                }
                            >
                                {logs.map((item) => {
                                    return (
                                        <Timeline.Item
                                            key={item.id}
                                            color={
                                                item.eventName == 'Executing'
                                                    ? 'gray'
                                                    : item.eventName == 'Faulted'
                                                    ? 'red'
                                                    : 'green'
                                            }
                                            dot={
                                                item.eventName == 'Executing' ? (
                                                    <ClockCircleOutlined
                                                        style={{ fontSize: '12px' }}
                                                    />
                                                ) : null
                                            }
                                        >
                                            <Popover
                                                title="Data"
                                                placement="left"
                                                content={
                                                    <Typography.Paragraph
                                                        style={{
                                                            display: 'block',
                                                            maxWidth: 500,
                                                            overflow: 'hidden',
                                                            wordWrap: 'break-word',
                                                        }}
                                                        copyable
                                                    >
                                                        {JSON.stringify(item.data ?? {})}
                                                    </Typography.Paragraph>
                                                }
                                            >
                                                {item.eventName} <Tag>{item.activityType}</Tag>
                                                <Tag>
                                                    {moment(item.timestamp).format(
                                                        'YYYY-MM-DD HH:mm:ss',
                                                    )}
                                                </Tag>
                                                <p>{item.message}</p>
                                            </Popover>
                                        </Timeline.Item>
                                    );
                                })}
                            </Timeline>
                        )}
                        {tabKey === 'input' && (
                            <Typography.Paragraph copyable>
                                {JSON.stringify(data?.input ?? {})}
                            </Typography.Paragraph>
                        )}
                        {tabKey === 'fault' && (
                            <Typography.Paragraph copyable>
                                {JSON.stringify(data?.fault ?? {})}
                            </Typography.Paragraph>
                        )}
                        {tabKey === 'variables' && (
                            <Typography.Paragraph copyable>
                                {JSON.stringify(data?.variables ?? {})}
                            </Typography.Paragraph>
                        )}
                        {tabKey === 'data' && (
                            <Typography.Paragraph copyable>
                                {JSON.stringify(data?.activityData ?? {})}
                            </Typography.Paragraph>
                        )}
                    </Card>
                </Col>
            </Row>
        </PageContainer>
    );
};

export default Index;
