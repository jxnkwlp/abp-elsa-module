import type { API } from '@/services/typings';
import { getWorkflowInstanceStatusCountStatistics } from '@/services/WorkflowInstance';
import { StatisticCard } from '@ant-design/pro-components';
import RcResizeObserver from 'rc-resize-observer';
import React, { useEffect, useState } from 'react';
import CountUp from 'react-countup';
import { useIntl } from 'umi';

const countUpFormatter = (value: number) => <CountUp end={value} separator="," />;

const CountStatistics: React.FC = () => {
    const intl = useIntl();

    const [loading, setLoading] = useState<boolean>(true);
    const [responsive, setResponsive] = useState(false);

    const [allStatusCountStatistics, setAllStatusStatisticsCount] =
        useState<API.WorkflowInstanceStatusCountStatisticsResult>();

    const load = async () => {
        setLoading(true);
        const result = await getWorkflowInstanceStatusCountStatistics({});
        setAllStatusStatisticsCount(result);
        setLoading(false);
    };

    useEffect(() => {
        load();
    }, []);

    return (
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
                        formatter: (value) => countUpFormatter(value as number),
                    }}
                />
                <StatisticCard
                    statistic={{
                        title: intl.formatMessage({
                            id: 'page.dashboard.statistics.status.finished',
                        }),
                        value: allStatusCountStatistics?.finished ?? 0,
                        status: 'success',
                        formatter: (value) => countUpFormatter(value as number),
                    }}
                />
                <StatisticCard
                    statistic={{
                        title: intl.formatMessage({
                            id: 'page.dashboard.statistics.status.faulted',
                        }),
                        value: allStatusCountStatistics?.faulted ?? 0,
                        status: 'error',
                        formatter: (value) => countUpFormatter(value as number),
                    }}
                />
                <StatisticCard
                    statistic={{
                        title: intl.formatMessage({
                            id: 'page.dashboard.statistics.status.running',
                        }),
                        value: allStatusCountStatistics?.running ?? 0,
                        status: 'processing',
                        formatter: (value) => countUpFormatter(value as number),
                    }}
                />
                <StatisticCard
                    statistic={{
                        title: intl.formatMessage({
                            id: 'page.dashboard.statistics.status.suspended',
                        }),
                        value: allStatusCountStatistics?.suspended ?? 0,
                        status: 'warning',
                        formatter: (value) => countUpFormatter(value as number),
                    }}
                />
            </StatisticCard.Group>
        </RcResizeObserver>
    );
};

export default CountStatistics;
