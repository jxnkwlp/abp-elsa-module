import {
    AlipayCircleOutlined,
    LockOutlined,
    MobileOutlined,
    TaobaoCircleOutlined,
    UserOutlined,
    WeiboCircleOutlined,
} from '@ant-design/icons';
import { Alert, message, Tabs } from 'antd';
import React, { useState } from 'react';
import { ProFormCaptcha, ProFormCheckbox, ProFormText, LoginForm } from '@ant-design/pro-form';
import { useIntl, history, FormattedMessage, SelectLang, useModel } from 'umi';
import Footer from '@/components/Footer';

import styles from './index.less';
import { loginLogin } from '@/services/Login';
import type { API } from '@/services/typings';

const LoginMessage: React.FC<{
    content: string;
}> = ({ content }) => (
    <Alert
        style={{
            marginBottom: 24,
        }}
        message={content}
        type="error"
        showIcon
    />
);

const Login: React.FC = () => {
    const [userLoginState, setUserLoginState] = useState<API.LoginResult>({});
    const [type, setType] = useState<string>('account');
    const { initialState, setInitialState } = useModel('@@initialState');

    const intl = useIntl();

    const fetchUserInfo = async () => {
        const userInfo = await initialState?.fetchData?.();
        if (userInfo) {
            await setInitialState((s) => ({
                ...s,
                currentUser: userInfo,
            }));
        }
    };

    const handleSubmit = async (values) => {
        try {
            // 登录
            const msg = await loginLogin(values as API.UserLoginInfo);
            if (msg.result === 1) {
                const defaultLoginSuccessMessage = intl.formatMessage({
                    id: 'pages.login.success',
                    defaultMessage: '登录成功！',
                });
                message.success(defaultLoginSuccessMessage);
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
            console.log(msg);
        } catch (error) {
            const defaultLoginFailureMessage = intl.formatMessage({
                id: 'pages.login.failure',
                defaultMessage: '登录失败，请重试！',
            });
            message.error(defaultLoginFailureMessage);
        }
    };
    const { status, type: loginType } = userLoginState;

    return (
        <div className={styles.container}>
            <div className={styles.lang} data-lang>
                {SelectLang && <SelectLang />}
            </div>
            <div className={styles.content}>
                <LoginForm
                    logo={<img alt="logo" src="/logo.svg" />}
                    title="Elsa Demo"
                    subTitle={intl.formatMessage({ id: 'pages.layouts.userLayout.title' })}
                    initialValues={{
                        autoLogin: true,
                    }}
                    onFinish={async (values) => {
                        await handleSubmit(values as API.LoginParams);
                    }}
                >
                    <Tabs activeKey={type} onChange={setType}>
                        <Tabs.TabPane
                            key="account"
                            tab={intl.formatMessage({
                                id: 'pages.login.accountLogin.tab',
                                defaultMessage: '账户密码登录',
                            })}
                        />
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
                </LoginForm>
            </div>
            <Footer />
        </div>
    );
};

export default Login;
