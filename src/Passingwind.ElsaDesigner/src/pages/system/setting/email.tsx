import React, { useEffect, useRef, useState } from 'react';
import { message, Button, Col, Row, Space, Alert, Spin } from 'antd';
import { Access, useAccess, useIntl } from 'umi';
import type { ProFormInstance } from '@ant-design/pro-components';
import {
    ModalForm,
    ProForm,
    ProFormDigit,
    ProFormSwitch,
    ProFormText,
    ProFormTextArea,
} from '@ant-design/pro-components';
import {
    emailSettingsSendTestEmail,
    getEmailSettings,
    updateEmailSettings,
} from '@/services/EmailSettings';

const Email: React.FC = () => {
    const intl = useIntl();
    const form = useRef<ProFormInstance>();
    const access = useAccess();

    const [loading, setLoading] = useState<boolean>(false);
    const [formData, setFormData] = useState<any>();

    const [testModalVisible, setTestModalVisible] = useState<boolean>();

    const handleSubmit = async (data: any) => {
        const result = await updateEmailSettings({ ...formData, ...data });
        if (result?.response?.ok) {
            message.success(intl.formatMessage({ id: 'common.dict.save.success' }));
        }
    };

    const handleSendTestEmail = async (values: any) => {
        const result = await emailSettingsSendTestEmail({ ...values });
        if (result?.response?.ok) {
            message.success(intl.formatMessage({ id: 'page.settings.email.test.success' }));
        }
    };

    const load = async () => {
        setLoading(true);
        const result = await getEmailSettings();
        setLoading(false);
        if (result) {
            setFormData(result);
            form?.current?.setFieldsValue(result);
        }
    };

    useEffect(() => {
        if (access['SettingManagement.Emailing']) load();
    }, [0]);

    return (
        <>
            <Access
                accessible={access['SettingManagement.Emailing']}
                fallback={
                    <Alert type="error" message={intl.formatMessage({ id: 'common.noaccess' })} />
                }
            >
                <Spin spinning={loading}>
                    <ProForm
                        formRef={form}
                        layout="horizontal"
                        labelWrap
                        labelCol={{ span: 3 }}
                        wrapperCol={{ sm: 24, md: 18, lg: 12, xl: 6, xxl: 4 }}
                        submitter={{
                            render: (props, doms) => {
                                return (
                                    <Row>
                                        <Col offset={3}>
                                            <Space>
                                                {doms}
                                                {access['SettingManagement.Emailing.Test'] && (
                                                    <Button
                                                        type="default"
                                                        onClick={() => {
                                                            setTestModalVisible(true);
                                                        }}
                                                    >
                                                        {intl.formatMessage({
                                                            id: 'page.settings.email.test',
                                                        })}
                                                    </Button>
                                                )}
                                            </Space>
                                        </Col>
                                    </Row>
                                );
                            },
                        }}
                        onFinish={async (values) => {
                            return await handleSubmit(values);
                        }}
                    >
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
                    </ProForm>
                    <ModalForm
                        modalProps={{ destroyOnClose: true, maskClosable: false }}
                        preserve={false}
                        layout="horizontal"
                        labelWrap
                        labelCol={{ span: 4 }}
                        title={intl.formatMessage({
                            id: 'page.settings.email.test',
                        })}
                        visible={testModalVisible}
                        onVisibleChange={setTestModalVisible}
                        submitter={{
                            searchConfig: {
                                submitText: intl.formatMessage({
                                    id: 'page.settings.email.testSend',
                                }),
                            },
                        }}
                        onFinish={async (values) => {
                            return await handleSendTestEmail(values);
                        }}
                    >
                        <ProFormText
                            label={intl.formatMessage({
                                id: 'page.settings.email.test.form.senderEmailAddress',
                            })}
                            name="senderEmailAddress"
                            rules={[
                                { required: true },
                                { max: 128 },
                                {
                                    pattern: /\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*/,
                                    message: 'invalid address',
                                },
                            ]}
                        />
                        <ProFormText
                            label={intl.formatMessage({
                                id: 'page.settings.email.test.form.targetEmailAddress',
                            })}
                            name="targetEmailAddress"
                            rules={[
                                { required: true },
                                { max: 128 },
                                {
                                    pattern: /\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*/,
                                    message: 'invalid address',
                                },
                            ]}
                        />
                        <ProFormText
                            label={intl.formatMessage({
                                id: 'page.settings.email.test.form.subject',
                            })}
                            name="subject"
                            rules={[{ required: true }, { max: 128 }]}
                        />
                        <ProFormTextArea
                            label={intl.formatMessage({
                                id: 'page.settings.email.test.form.body',
                            })}
                            name="body"
                            rules={[{ required: true }, { max: 256 }]}
                        />
                    </ModalForm>
                </Spin>
            </Access>
        </>
    );
};

export default Email;
