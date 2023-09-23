import { getWorkflowInstanceStatusCountStatistics } from '@/services/WorkflowInstance';
import type { API } from '@/services/typings';
import { Pie } from '@ant-design/charts';
import { useAsyncEffect } from 'ahooks';
import { Card } from 'antd';
import React from 'react';
import { useIntl } from 'umi';

export const StatisticsStatusCount: React.FC<{ id: string }> = (props) => {
    const intl = useIntl();

    const [id, setId] = React.useState<string>(props.id);
    const [loading, setLoading] = React.useState<boolean>(false);

    const [statisticsStatusCountData, setStatisticsStatusCountData] =
        React.useState<API.WorkflowInstanceStatusCountStatisticsResult>();

    const load = async () => {
        setLoading(true);

        const statusCountList = await getWorkflowInstanceStatusCountStatistics({
            workflowDefinitionId: id,
        });
        setStatisticsStatusCountData(statusCountList);

        setLoading(false);
    };

    useAsyncEffect(async () => {
        await load();
    }, [0]);

    return (
        <Card
            style={{ marginBottom: 16 }}
            title={intl.formatMessage({
                id: 'page.instance.statistics.statusCount',
            })}
            loading={loading}
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
    );
};
