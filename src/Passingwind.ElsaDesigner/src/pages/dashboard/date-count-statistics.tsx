import { getWorkflowInstanceStatusDateCountStatistics } from '@/services/WorkflowInstance';
import { Line } from '@ant-design/charts';
import { Card } from 'antd';
import moment from 'moment';
import React, { useEffect, useState } from 'react';
import { useIntl } from 'umi';

const DateCountStatistics: React.FC = () => {
    const intl = useIntl();

    const [loading, setLoading] = useState<boolean>(true);

    const [statusDateCountStatistics, setStatusDateCountStatistics] = useState<any>([]);

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
    useEffect(() => {
        load();
    }, []);

    return (
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
    );
};

export default DateCountStatistics;
