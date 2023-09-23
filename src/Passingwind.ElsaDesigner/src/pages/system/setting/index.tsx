import type { ProFormInstance } from '@ant-design/pro-components';
import { PageContainer } from '@ant-design/pro-layout';
import { Card } from 'antd';
import React, { useEffect, useRef, useState } from 'react';
import { useIntl } from 'umi';
import Email from './email';
import OAuth2 from './oauth2';

const Index: React.FC = () => {
    const intl = useIntl();
    const form = useRef<ProFormInstance>();

    const [tab, setTab] = useState('emailing');

    const onTabChange = async (tab: string) => {
        setTab(tab);
        form?.current?.resetFields();
    };

    useEffect(() => {
        onTabChange(tab);
    }, [0]);

    return (
        <PageContainer>
            <Card
                tabList={[
                    { tab: intl.formatMessage({ id: 'page.settings.email' }), key: 'emailing' },
                    { tab: intl.formatMessage({ id: 'page.settings.oauth2' }), key: 'oauth2' },
                ]}
                activeTabKey={tab}
                onTabChange={onTabChange}
            >
                {tab == 'emailing' && <Email />}
                {tab == 'oauth2' && <OAuth2 />}
            </Card>
        </PageContainer>
    );
};

export default Index;
