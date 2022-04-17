import React from 'react';
import { PageContainer } from '@ant-design/pro-layout';
import { Alert } from 'antd';

const Index: React.FC = () => {
    return (
        <PageContainer>
            <Alert message="Hello" />
        </PageContainer>
    );
};

export default Index;
