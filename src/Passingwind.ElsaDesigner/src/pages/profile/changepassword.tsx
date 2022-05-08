import { profileChangePassword } from '@/services/Profile';
import { PageContainer } from '@ant-design/pro-layout';
import { Button, Card, Form, Input, Modal, Spin } from 'antd';
import React, { useState } from 'react';
import { history, useModel } from 'umi';

const handleUpdate = async (data: any) => {
    const response = await profileChangePassword(data);
    if (response?.response?.ok) {
        // message.success('修改成功');
        return true;
    }
    return false;
};

const Index: React.FC = () => {
    const [loading, setLoading] = useState<boolean>(false);

    const [form] = Form.useForm();

    const { initialState, setInitialState } = useModel('@@initialState');

    return (
        <PageContainer>
            <Card title="Change Password">
                <Spin spinning={loading}>
                    <Form
                        form={form}
                        name="basic"
                        labelCol={{ span: 2 }}
                        wrapperCol={{ span: 10 }}
                        layout="horizontal"
                        onFinish={async (values) => {
                            setLoading(true);
                            if (await handleUpdate(values)) {
                                Modal.info({
                                    title: '提示',
                                    content: '修改密码成功，请重新登录',
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
                            label="当前密码"
                            name="currentPassword"
                            rules={[{ required: true }, { min: 6 }, { max: 32 }]}
                        >
                            <Input.Password maxLength={32} />
                        </Form.Item>
                        <Form.Item
                            label="新密码"
                            name="newPassword"
                            rules={[{ required: true }, { min: 6 }, { max: 32 }]}
                        >
                            <Input.Password maxLength={32} />
                        </Form.Item>
                        <Form.Item
                            label="重复密码"
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
                                        return Promise.reject(new Error('2次输入的密码不匹配'));
                                    },
                                }),
                            ]}
                        >
                            <Input.Password maxLength={32} />
                        </Form.Item>

                        <Form.Item>
                            <Button type="primary" htmlType="submit">
                                修改
                            </Button>
                        </Form.Item>
                    </Form>
                </Spin>
            </Card>
        </PageContainer>
    );
};

export default Index;
