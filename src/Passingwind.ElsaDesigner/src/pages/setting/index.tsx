import React, { useEffect, useRef, useState } from 'react';
import { PageContainer } from '@ant-design/pro-layout';
import { Card, Alert, Typography, Form, message, Divider, Button, Input } from 'antd';
import { useIntl, FormattedMessage } from 'umi';
import styles from './Welcome.less';
import {
    ProCard,
    ProForm,
    ProFormDigit,
    ProFormInstance,
    ProFormSwitch,
    ProFormText,
} from '@ant-design/pro-components';
import { getEmailSettings, updateEmailSettings } from '@/services/EmailSettings';

const Index: React.FC = () => {
    const intl = useIntl();
    const form = useRef<ProFormInstance>();

    const [tab, setTab] = useState('email');

    const [formData, setFormData] = useState<any>();

    const onTabChange = async (tab: string) => {
        setTab(tab);
        form?.current?.resetFields();

        if (tab == 'email') {
            const result = await getEmailSettings();
            if (result) {
                setFormData(result);
                form?.current?.setFieldsValue(result);
            }
        }
    };

    const handleSubmit = async (data: any) => {
        if (tab == 'email') {
            const result = await updateEmailSettings({ ...formData, ...data });
            if (result?.response?.ok) {
                message.success(intl.formatMessage({ id: 'common.dict.save.success' }));
            }
        }
    };

    const handleOnSendEmailTest = (address: string) => {
        message.info('TODO');
    };

    useEffect(() => {
        onTabChange(tab);
    }, [0]);

    return (
        <PageContainer>
            <Card
                tabList={[{ tab: 'Email (SMTP)', key: 'email' }]}
                activeTabKey={tab}
                onTabChange={onTabChange}
            >
                <ProForm
                    formRef={form}
                    layout="horizontal"
                    labelCol={{ span: 3 }}
                    labelWrap
                    wrapperCol={{ span: 9 }}
                    onFinish={async (values) => {
                        await handleSubmit(values);
                    }}
                >
                    {tab == 'email' && (
                        <>
                            <ProFormText
                                label={intl.formatMessage({ id: 'page.settings.email.smtpHost' })}
                                name="smtpHost"
                                rules={[{ required: true }, { max: 128 }]}
                            />
                            <ProFormDigit
                                label={intl.formatMessage({ id: 'page.settings.email.smtpPort' })}
                                name="smtpPort"
                                width="sm"
                                rules={[{ required: true }]}
                            />
                            <ProFormText
                                label={intl.formatMessage({
                                    id: 'page.settings.email.smtpUserName',
                                })}
                                name="smtpUserName"
                                rules={[{ max: 128 }]}
                            />
                            <ProFormText.Password
                                label={intl.formatMessage({
                                    id: 'page.settings.email.smtpPassword',
                                })}
                                name="smtpPassword"
                                rules={[{ max: 128 }]}
                            />
                            <ProFormText
                                label={intl.formatMessage({ id: 'page.settings.email.smtpDomain' })}
                                name="smtpDomain"
                            />
                            <ProFormSwitch
                                label={intl.formatMessage({
                                    id: 'page.settings.email.smtpEnableSsl',
                                })}
                                name="smtpEnableSsl"
                            />
                            <ProFormSwitch
                                label={intl.formatMessage({
                                    id: 'page.settings.email.smtpUseDefaultCredentials',
                                })}
                                name="smtpUseDefaultCredentials"
                            />
                            <ProFormText
                                label={intl.formatMessage({
                                    id: 'page.settings.email.defaultFromAddress',
                                })}
                                name="defaultFromAddress"
                                rules={[{ required: true }, { max: 128 }]}
                            />
                            <ProFormText
                                label={intl.formatMessage({
                                    id: 'page.settings.email.defaultFromDisplayName',
                                })}
                                name="defaultFromDisplayName"
                                rules={[{ required: true }, { max: 128 }]}
                            />
                            <Divider />
                            <ProForm.Item
                                label={intl.formatMessage({
                                    id: 'page.settings.email.testToAddress',
                                })}
                            >
                                <Input.Search
                                    placeholder=""
                                    allowClear
                                    enterButton={intl.formatMessage({
                                        id: 'page.settings.email.testSend',
                                    })}
                                    onSearch={handleOnSendEmailTest}
                                />
                            </ProForm.Item>
                        </>
                    )}
                </ProForm>
            </Card>
        </PageContainer>
    );
};

export default Index;
