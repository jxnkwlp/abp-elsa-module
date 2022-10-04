import React, { useEffect, useState } from 'react';
import { PageContainer } from '@ant-design/pro-layout';
import { Alert, Card } from 'antd';
import { getWorkflowInstanceStatusDateCountStatistics } from '@/services/WorkflowInstance';
import { Line } from '@ant-design/charts';
import moment from 'moment';
import { useIntl } from 'umi';

const Index: React.FC = () => {
    const intl = useIntl();

    const [statusDateCountStatistics, setStatusDateCountStatistics] = useState<any>([]);

    useEffect(() => {
        const load = async () => {
            const result = await getWorkflowInstanceStatusDateCountStatistics({});
            const list = (result?.items ?? []).map((x) => {
                return {
                    date: moment(x.date),
                    count: x.finishedCount,
                    type: 'Finished',
                };
            });
            list.push(
                ...(result?.items ?? []).map((x) => {
                    return {
                        date: moment(x.date),
                        count: x.failedCount,
                        type: 'Failed',
                    };
                }),
            );
            setStatusDateCountStatistics(list);
        };

        load();
    }, []);

    return (
        <PageContainer>
            <Card
                title={intl.formatMessage({
                    id: 'page.dashboard.statistics.7datecount.title',
                })}
            >
                <div style={{ height: 300 }}>
                    <Line
                        data={statusDateCountStatistics}
                        xField="date"
                        yField="count"
                        seriesField="type"
                        smooth
                        padding="auto"
                    />
                </div>
            </Card>
        </PageContainer>
    );
};

export default Index;
