import React from 'react';
import { PageContainer } from '@ant-design/pro-layout';
import { Card, Alert, Typography, Form } from 'antd';
import { useIntl, FormattedMessage } from 'umi';
import MonacorEditorInput from './designer/form-input/monacor-editor-input';

const Test: React.FC = () => {
    const intl = useIntl();

    return (
        <PageContainer>
            <Card>
                <Form initialValues={{ v: new Date().getTime().toString() }}>
                    <Form.Item name="v">
                        <MonacorEditorInput height={500} />
                    </Form.Item>
                </Form>
            </Card>
        </PageContainer>
    );
};

export default Test;
