import type { API } from '@/services/typings';
import {
    getWorkflowInstanceStatusCountStatistics,
    getWorkflowInstanceStatusDateCountStatistics,
} from '@/services/WorkflowInstance';
import { Line } from '@ant-design/charts';
import { StatisticCard } from '@ant-design/pro-components';
import { PageContainer } from '@ant-design/pro-layout';
import { Card } from 'antd';
import moment from 'moment';
import RcResizeObserver from 'rc-resize-observer';
import React, { useEffect, useState } from 'react';
import { useIntl } from 'umi';

const Index: React.FC = () => {
    const intl = useIntl();

    const [loading, setLoading] = useState<boolean>(false);
    const [responsive, setResponsive] = useState(false);

    const [statusDateCountStatistics, setStatusDateCountStatistics] = useState<any>([]);
    const [allStatusCountStatistics, setAllStatusStatisticsCount] =
        useState<API.WorkflowInstanceStatusCountStatisticsResult>();

    useEffect(() => {
        const load = async () => {
            const result = await getWorkflowInstanceStatusCountStatistics();
            setAllStatusStatisticsCount(result);
        };

        load();
    }, []);

    useEffect(() => {
        const load = async () => {
            setLoading(true);
            const tz = moment().utcOffset() / 60;
            const result = await getWorkflowInstanceStatusDateCountStatistics({ tz: tz });
            const list = (result?.items ?? []).map((x) => {
                return {
                    date: moment(x.date).format('MM-DD'),
                    count: x.finishedCount,
                    type: 'Finished',
                };
            });
            list.push(
                ...(result?.items ?? []).map((x) => {
                    return {
                        date: moment(x.date).format('MM-DD'),
                        count: x.failedCount,
                        type: 'Failed',
                    };
                }),
            );
            setStatusDateCountStatistics(list);

            setLoading(false);
        };

        load();
    }, []);

    return (
        <PageContainer>
            <RcResizeObserver
                key="resize-observer"
                onResize={(offset) => {
                    setResponsive(offset.width < 596);
                }}
            >
                <StatisticCard.Group
                    loading={loading}
                    direction={responsive ? 'column' : 'row'}
                    title={intl.formatMessage({ id: 'page.dashboard.statistics.status.title' })}
                >
                    <StatisticCard
                        statistic={{
                            title: intl.formatMessage({
                                id: 'page.dashboard.statistics.status.all',
                            }),
                            value: allStatusCountStatistics?.all ?? 0,
                        }}
                    />
                    <StatisticCard
                        statistic={{
                            title: intl.formatMessage({
                                id: 'page.dashboard.statistics.status.finished',
                            }),
                            value: allStatusCountStatistics?.finished ?? 0,
                            status: 'success',
                        }}
                    />
                    <StatisticCard
                        statistic={{
                            title: intl.formatMessage({
                                id: 'page.dashboard.statistics.status.faulted',
                            }),
                            value: allStatusCountStatistics?.faulted ?? 0,
                            status: 'error',
                        }}
                    />
                    <StatisticCard
                        statistic={{
                            title: intl.formatMessage({
                                id: 'page.dashboard.statistics.status.running',
                            }),
                            value: allStatusCountStatistics?.running ?? 0,
                            status: 'processing',
                        }}
                    />
                    <StatisticCard
                        statistic={{
                            title: intl.formatMessage({
                                id: 'page.dashboard.statistics.status.suspended',
                            }),
                            value: allStatusCountStatistics?.suspended ?? 0,
                            status: 'warning',
                        }}
                    />
                </StatisticCard.Group>
            </RcResizeObserver>

            <Card
                loading={loading}
                title={intl.formatMessage(
                    {
                        id: 'page.dashboard.statistics.datecount.title',
                    },
                    { d: 30 },
                )}
                style={{ marginTop: 15 }}
            >
                <div style={{ height: 380 }}>
                    <Line
                        data={statusDateCountStatistics}
                        xField="date"
                        yField="count"
                        // xAxis={{
                        //     type: 'time',
                        // }}
                        seriesField="type"
                        smooth
                        padding="auto"
                        color={['#52c41a', '#f00']}
                    />
                </div>
            </Card>
        </PageContainer>
    );
};

export default Index;
