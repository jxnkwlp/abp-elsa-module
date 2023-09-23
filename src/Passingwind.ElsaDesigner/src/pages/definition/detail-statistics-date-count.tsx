import { getWorkflowInstanceStatusDateCountStatistics } from '@/services/WorkflowInstance';
import { Line } from '@ant-design/charts';
import { useAsyncEffect } from 'ahooks';
import { Card } from 'antd';
import moment from 'moment';
import React from 'react';
import { useIntl } from 'umi';

export const StatisticsDateCount: React.FC<{ id: string }> = (props) => {
    const intl = useIntl();

    const [id, setId] = React.useState<string>(props.id);
    const [loading, setLoading] = React.useState<boolean>(false);

    const [statisticsDateCountData, setStatisticsDateCountData] = React.useState<any>([]);

    const load = async () => {
        setLoading(true);
        const tz = moment().utcOffset() / 60;
        const dateCountList = await getWorkflowInstanceStatusDateCountStatistics({
            workflowDefinitionId: id,
            tz: tz,
            datePeriod: 15,
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

        setLoading(false);
    };

    useAsyncEffect(async () => {
        await load();
    }, [0]);

    return (
        <Card
            style={{ marginBottom: 16 }}
            title={intl.formatMessage({
                id: 'page.instance.statistics.dateCount',
            })}
            loading={loading}
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
    );
};
