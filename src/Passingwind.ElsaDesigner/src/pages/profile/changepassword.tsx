import { profileChangePassword } from '@/services/Profile';
import { PageContainer } from '@ant-design/pro-layout';
import { Button, Card, Form, Input, Modal, Spin } from 'antd';
import React, { useState } from 'react';
import { history, useIntl, useModel } from 'umi';

const handleUpdate = async (data: any) => {
    const response = await profileChangePassword(data);
    if (response?.response?.ok) {
        return true;
    }
    return false;
};

const Index: React.FC = () => {
    const [loading, setLoading] = useState<boolean>(false);
    const intl = useIntl();

    const [form] = Form.useForm();

    const { initialState, setInitialState } = useModel('@@initialState');

    return (
        <PageContainer>
            <Card title={intl.formatMessage({ id: 'page.change-password.title' })}>
                <Spin spinning={loading}>
                    <Form
                        form={form}
                        name="basic"
                        labelCol={{ span: 4 }}
                        wrapperCol={{ span: 8 }}
                        layout="vertical"
                        onFinish={async (values) => {
                            setLoading(true);
                            if (await handleUpdate(values)) {
                                Modal.info({
                                    title: intl.formatMessage({ id: 'common.dict.info.tips' }),
                                    content: intl.formatMessage({
                                        id: 'page.change-password.submit.success',
                                    }),
                                    onOk: async () => {
                                        await setInitialState((s) => ({
                                            ...s,
                                            currentUser: undefined,
                                        }));
                                        history.replace('/auth/login');
                                    },
                                });
                            }
                            setLoading(false);
                        }}
                    >
                        <Form.Item
                            label={intl.formatMessage({
                                id: 'page.change-password.field.currentPassword',
                            })}
                            name="currentPassword"
                            rules={[{ required: true }, { min: 6 }, { max: 32 }]}
                        >
                            <Input.Password maxLength={32} />
                        </Form.Item>
                        <Form.Item
                            label={intl.formatMessage({
                                id: 'page.change-password.field.newPassword',
                            })}
                            name="newPassword"
                            rules={[{ required: true }, { min: 6 }, { max: 32 }]}
                        >
                            <Input.Password maxLength={32} />
                        </Form.Item>
                        <Form.Item
                            label={intl.formatMessage({
                                id: 'page.change-password.field.confirmPassword',
                            })}
                            name="confirmNewPassword"
                            dependencies={['newPassword']}
                            rules={[
                                { required: true },
                                { min: 6 },
                                { max: 32 },
                                ({ getFieldValue }) => ({
                                    validator(_, value) {
                                        if (!value || getFieldValue('newPassword') === value) {
                                            return Promise.resolve();
                                        }
                                        return Promise.reject(
                                            new Error(
                                                intl.formatMessage({
                                                    id: 'page.change-password.field.2passwordnotmatch',
                                                }),
                                            ),
                                        );
                                    },
                                }),
                            ]}
                        >
                            <Input.Password maxLength={32} />
                        </Form.Item>

                        <Form.Item label="">
                            <Button type="primary" htmlType="submit">
                                {intl.formatMessage({
                                    id: 'page.change-password.submit',
                                })}
                            </Button>
                        </Form.Item>
                    </Form>
                </Spin>
            </Card>
        </PageContainer>
    );
};

export default Index;
