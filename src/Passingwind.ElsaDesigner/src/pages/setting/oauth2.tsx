import React, { useEffect, useRef, useState } from 'react';
import { message, Alert, Space, Col, Row, Spin } from 'antd';
import { Access, useAccess, useIntl } from 'umi';
import type { ProFormInstance } from '@ant-design/pro-components';
import { ProForm, ProFormSwitch, ProFormText } from '@ant-design/pro-components';
import { getOAuth2Settings, updateOAuth2Settings } from '@/services/OAuth2Settings';

const OAuth2: React.FC = () => {
    const intl = useIntl();
    const form = useRef<ProFormInstance>();
    const access = useAccess();

    const [loading, setLoading] = useState<boolean>(false);
    const [formData, setFormData] = useState<any>();

    const handleSubmit = async (data: any) => {
        const result = await updateOAuth2Settings({ ...formData, ...data });
        if (result?.response?.ok) {
            message.success(intl.formatMessage({ id: 'common.dict.save.success' }));
        }
    };

    const load = async () => {
        setLoading(true);
        const result = await getOAuth2Settings();
        if (result) {
            setFormData(result);
            form?.current?.setFieldsValue(result);
        }
        setLoading(false);
    };

    useEffect(() => {
        if (access['SettingManagement.OAuth2']) load();
    }, [0]);

    return (
        <>
            <Access
                accessible={true}
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
                                            <Space>{doms}</Space>
                                        </Col>
                                    </Row>
                                );
                            },
                        }}
                        onFinish={async (values) => {
                            return await handleSubmit(values);
                        }}
                    >
                        <ProFormSwitch
                            label={intl.formatMessage({ id: 'page.settings.oauth2.enabled' })}
                            name="enabled"
                        />
                        <ProFormText
                            label={intl.formatMessage({ id: 'page.settings.oauth2.displayName' })}
                            name="displayName"
                            rules={[{ required: true }, { max: 16 }]}
                        />
                        <ProFormText
                            label={intl.formatMessage({ id: 'page.settings.oauth2.authority' })}
                            name="authority"
                            rules={[{ required: true }, { max: 64 }]}
                        />
                        <ProFormText
                            label={intl.formatMessage({
                                id: 'page.settings.oauth2.metadataAddress',
                            })}
                            name="metadataAddress"
                            rules={[{ required: true }, { max: 128 }]}
                        />
                        <ProFormText
                            label={intl.formatMessage({ id: 'page.settings.oauth2.clientId' })}
                            name="clientId"
                            rules={[{ required: true }, { max: 64 }]}
                        />
                        <ProFormText.Password
                            label={intl.formatMessage({ id: 'page.settings.oauth2.clientSecret' })}
                            name="clientSecret"
                            rules={[{ max: 128 }]}
                        />
                        <ProFormText
                            label={intl.formatMessage({ id: 'page.settings.oauth2.scope' })}
                            name="scope"
                            rules={[{ max: 64 }]}
                        />
                    </ProForm>
                </Spin>
            </Access>
        </>
    );
};

export default OAuth2;
