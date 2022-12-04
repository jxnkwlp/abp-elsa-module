import React, { useEffect, useRef, useState } from 'react';
import { PageContainer } from '@ant-design/pro-layout';
import { Card, Col, Row, Space } from 'antd';
import { useIntl } from 'umi';
import type { ProFormInstance } from '@ant-design/pro-components';
import { ProForm } from '@ant-design/pro-components';
import Email from './email';

const Index: React.FC = () => {
    const intl = useIntl();
    const form = useRef<ProFormInstance>();

    const [tab, setTab] = useState('email');

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
                tabList={[{ tab: 'Email', key: 'email' }]}
                activeTabKey={tab}
                onTabChange={onTabChange}
            >
                {tab == 'email' && <Email />}
            </Card>
        </PageContainer>
    );
};

export default Index;
