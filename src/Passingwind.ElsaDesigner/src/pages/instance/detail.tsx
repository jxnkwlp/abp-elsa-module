import { getWorkflowDefinitionVersion } from '@/services/WorkflowDefinition';
import { getWorkflowInstance, getWorkflowInstanceExecutionLogs } from '@/services/WorkflowInstance';
import { PageContainer } from '@ant-design/pro-layout';
import {
    Alert,
    Button,
    Card,
    Col,
    Modal,
    Popover,
    Result,
    Row,
    Tabs,
    Tag,
    Timeline,
    Typography,
} from 'antd';
import moment from 'moment';
import React, { useEffect } from 'react';
import { useHistory, useLocation } from 'umi';
import Flow from '../designer/flow';
import { conventToGraphData } from '../designer/service';
import type { IGraphData } from '../designer/type';

const Index: React.FC = () => {
    const history = useHistory();
    const location = useLocation();

    const [id, setId] = React.useState<string>();
    const [title, setTitle] = React.useState<string>();
    const [data, setData] = React.useState<API.WorkflowInstance>();
    const [loading, setLoading] = React.useState<boolean>(false);
    const [graphData, setGraphData] = React.useState<IGraphData>();

    const [logs, setLogs] = React.useState<API.WorkflowExecutionLog[]>([]);

    const [tabKey, setTabKey] = React.useState<string>('logs');

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
                    <Card title={['Graph ', <Tag key="2">{data?.version}</Tag>]}>
                        <Flow
                            readonly
                            showMiniMap={false}
                            showNodeTypes={false}
                            showToolbar={false}
                            graphData={graphData}
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
                            { key: 'fault', tab: 'Fault' },
                            { key: 'variables', tab: 'Variables' },
                            { key: 'data', tab: 'Data' },
                        ]}
                        onTabChange={(key) => {
                            setTabKey(key);
                        }}
                    >
                        {tabKey === 'logs' && (
                            <Timeline mode="left">
                                {logs.map((item) => {
                                    return (
                                        <Timeline.Item
                                            key={item.id}
                                            color={item.eventName == 'Faulted' ? 'red' : 'green'}
                                        >
                                            <Popover
                                                title="Data"
                                                placement="left"
                                                content={
                                                    <Typography.Paragraph
                                                        style={{
                                                            maxWidth: 500,
                                                            overflow: 'hidden',
                                                            wordWrap: 'break-word',
                                                        }}
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
                        {tabKey === 'fault' && (
                            <Typography.Paragraph>
                                <code>{JSON.stringify(data?.fault ?? {})}</code>
                            </Typography.Paragraph>
                        )}
                        {tabKey === 'variables' && (
                            <Typography.Paragraph>
                                <code>{JSON.stringify(data?.variables ?? {})}</code>
                            </Typography.Paragraph>
                        )}
                        {tabKey === 'data' && (
                            <Typography.Paragraph>
                                <code>{JSON.stringify(data?.activityData ?? {})}</code>
                            </Typography.Paragraph>
                        )}
                    </Card>
                </Col>
            </Row>
        </PageContainer>
    );
};

export default Index;
