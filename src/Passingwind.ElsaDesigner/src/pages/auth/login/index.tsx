import Footer from '@/components/Footer';
import { LockOutlined, UserOutlined } from '@ant-design/icons';
import { LoginForm, ProFormText } from '@ant-design/pro-form';
import { Button, message, Spin, Tabs } from 'antd';
import React, { useEffect, useState } from 'react';
import { FormattedMessage, history, SelectLang, useIntl, useModel } from 'umi';
import { getLogin, loginLogin } from '@/services/Login';
import type { API } from '@/services/typings';
import styles from './index.less';

const Login: React.FC = () => {
    const intl = useIntl();
    const { refresh } = useModel('@@initialState');

    const [type, setType] = useState<string>('account');
    const [loading, setLoading] = useState<boolean>(false);
    const [loginInfo, setLoginInfo] = useState<API.AccountResult>();

    const fetchUserInfo = async () => {
        await refresh();
    };

    const handleSubmit = async (values: any) => {
        const msg = await loginLogin(values);
        if (msg.result === 1) {
            await fetchUserInfo();
            if (!history) return;
            history.push('/');
            return;
        } else {
            const defaultLoginFailureMessage = intl.formatMessage({
                id: 'pages.login.failure',
                defaultMessage: '登录失败，请重试！',
            });
            message.error(msg.description ?? defaultLoginFailureMessage);
        }
    };

    const handleExternalProviderLogin = async (provider: string) => {
        // await loginExternalLogin({ provider: provider }, { redirect: 'manual' });
        window.location.href = '/api/account/login/external?provider=' + provider;
        message.loading(intl.formatMessage({ id: 'common.dict.loading' }));
    };

    useEffect(() => {
        if (loginInfo && loginInfo.enableLocalLogin == false) {
        }
    }, [loginInfo]);

    const loadLoginInfo = async () => {
        setLoading(true);
        const result = await getLogin();
        if (result) setLoginInfo(result);
        setLoading(false);
    };

    useEffect(() => {
        loadLoginInfo();
    }, [0]);

    return (
        <div className={styles.container}>
            <div className={styles.lang} data-lang>
                {SelectLang && <SelectLang />}
            </div>
            <div className={styles.content}>
                <Spin spinning={loading}>
                    <LoginForm
                        logo={<img alt="logo" src="/logo.svg" />}
                        title="Workflow"
                        subTitle={intl.formatMessage({ id: 'pages.layouts.userLayout.title' })}
                        initialValues={{
                            autoLogin: true,
                        }}
                        onFinish={async (values) => {
                            await handleSubmit(values);
                        }}
                        submitter={type == 'sso' ? false : {}}
                    >
                        <Tabs activeKey={type} onChange={setType}>
                            <Tabs.TabPane
                                key="account"
                                tab={intl.formatMessage({
                                    id: 'pages.login.accountLogin.tab',
                                    defaultMessage: '账户密码登录',
                                })}
                            />
                            {loginInfo?.externalProviders?.length && (
                                <Tabs.TabPane
                                    key={'sso'}
                                    tab={intl.formatMessage({
                                        id: 'pages.login.ssoLogin.tab',
                                        defaultMessage: 'SSO登录',
                                    })}
                                />
                            )}
                        </Tabs>

                        {type === 'account' && (
                            <>
                                <ProFormText
                                    name="userNameOrEmailAddress"
                                    fieldProps={{
                                        size: 'large',
                                        prefix: <UserOutlined className={styles.prefixIcon} />,
                                    }}
                                    placeholder={intl.formatMessage({
                                        id: 'pages.login.username.placeholder',
                                        defaultMessage: '',
                                    })}
                                    rules={[
                                        {
                                            required: true,
                                            message: (
                                                <FormattedMessage
                                                    id="pages.login.username.required"
                                                    defaultMessage="请输入用户名!"
                                                />
                                            ),
                                        },
                                    ]}
                                />
                                <ProFormText.Password
                                    name="password"
                                    fieldProps={{
                                        size: 'large',
                                        prefix: <LockOutlined className={styles.prefixIcon} />,
                                    }}
                                    placeholder={intl.formatMessage({
                                        id: 'pages.login.password.placeholder',
                                        defaultMessage: '',
                                    })}
                                    rules={[
                                        {
                                            required: true,
                                            message: (
                                                <FormattedMessage
                                                    id="pages.login.password.required"
                                                    defaultMessage="请输入密码！"
                                                />
                                            ),
                                        },
                                    ]}
                                />
                            </>
                        )}
                        {type == 'sso' && (
                            <div>
                                {loginInfo?.externalProviders?.map((x) => {
                                    return (
                                        <>
                                            <Button
                                                type={
                                                    loginInfo?.externalProviders?.indexOf(x) == 0
                                                        ? 'primary'
                                                        : 'default'
                                                }
                                                size={
                                                    loginInfo?.externalProviders?.indexOf(x) == 0
                                                        ? 'large'
                                                        : 'middle'
                                                }
                                                block
                                                onClick={async () => {
                                                    await handleExternalProviderLogin(
                                                        x.authenticationScheme!,
                                                    );
                                                }}
                                            >
                                                {x.displayName}
                                            </Button>
                                        </>
                                    );
                                })}
                            </div>
                        )}
                    </LoginForm>
                </Spin>
            </div>
            <Footer />
        </div>
    );
};

export default Login;
