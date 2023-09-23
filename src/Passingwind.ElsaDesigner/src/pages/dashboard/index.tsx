import { PageContainer } from '@ant-design/pro-layout';
import { Alert } from 'antd';
import React from 'react';
import { Access, useAccess, useIntl } from 'umi';
import CountStatistics from './count-statistics';
import DateCountStatistics from './date-count-statistics';

const Index: React.FC = () => {
    const intl = useIntl();
    const access = useAccess();

    return (
        <PageContainer>
            <Access
                accessible={access['ElsaWorkflow.Instances.Statistics']}
                fallback={
                    <Alert
                        type="error"
                        message={intl.formatMessage(
                            { id: 'common.noaccess' },
                            { permission: 'ElsaWorkflow.Instances.Statistics' },
                        )}
                    />
                }
            >
                <CountStatistics />
                <DateCountStatistics />
            </Access>
        </PageContainer>
    );
};

export default Index;
