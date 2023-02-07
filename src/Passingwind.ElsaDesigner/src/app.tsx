/* eslint-disable @typescript-eslint/dot-notation */
import Footer from '@/components/Footer';
import RightContent from '@/components/RightContent';
import { BookOutlined, LinkOutlined } from '@ant-design/icons';
import type { Settings as LayoutSettings } from '@ant-design/pro-components';
import { PageLoading, SettingDrawer } from '@ant-design/pro-components';
import { loader } from '@monaco-editor/react';
import { message, notification } from 'antd';
import Cookies from 'js-cookie';
import moment from 'moment';
import * as monaco from 'monaco-editor';
import type { RequestConfig, RunTimeLayoutConfig } from 'umi';
import { formatMessage, history, Link } from 'umi';
import defaultSettings from '../config/defaultSettings';
import { getAbpApplicationConfiguration } from './services/AbpApplicationConfiguration';
import type { API } from './services/typings';

const isDev = process.env.NODE_ENV === 'development';
const loginPath = '/auth/login';

// Update moment default format
moment.defaultFormat = 'YYYY-MM-DD HH:mm:ss';
moment.defaultFormatUtc = 'YYYY-MM-DDTHH:mm:ss[Z]';

// see: https://github.com/suren-atoyan/monaco-react#loader-config
loader.config({ monaco });

/** 获取用户信息比较慢的时候会展示一个 loading */
export const initialStateConfig = {
    loading: <PageLoading />,
};

/**
 * @see  https://umijs.org/zh-CN/plugins/plugin-initial-state
 * */
export async function getInitialState(): Promise<{
    settings?: Partial<LayoutSettings>;
    currentUser?: API.CurrentUser;
    loading?: boolean;
    fetchUserInfo?: () => Promise<API.CurrentUser | undefined>;
}> {
    const loadData = async () => {
        const result = await getAbpApplicationConfiguration();
        return result?.currentUser;
    };

    // if (isDev) {
    //     // mock user
    //     return {
    //         settings: defaultSettings,
    //         currentUser: {
    //             isAuthenticated: true,
    //             name: 'admin',
    //             surName: 'admin',
    //             userName: 'admin',
    //         },
    //         fetchUserInfo: loadData,
    //     };
    // }

    const user = await loadData();

    if (!user?.isAuthenticated) {
        history.replace('/auth/login');
        return {
            settings: defaultSettings,
            currentUser: user,
            loading: true,
            fetchUserInfo: loadData,
        };
    }

    return {
        settings: defaultSettings,
        currentUser: user,
        fetchUserInfo: loadData,
    };
}

// ProLayout 支持的api https://procomponents.ant.design/components/layout
export const layout: RunTimeLayoutConfig = ({ initialState, setInitialState }) => {
    return {
        rightContentRender: () => <RightContent />,
        disableContentMargin: false,
        footerRender: () => <Footer />,
        onPageChange: () => {
            const { location } = history;
            if (isDev) console.log(location);
            // 如果没有登录，重定向到 login
            if (!initialState?.currentUser && location.pathname !== loginPath) {
                history.push(loginPath);
            }
        },
        links: isDev
            ? [
                  <Link key="openapi" to="/umi/plugin/openapi" target="_blank">
                      <LinkOutlined />
                      <span>OpenAPI 文档</span>
                  </Link>,
                  <Link to="/~docs" key="docs">
                      <BookOutlined />
                      <span>业务组件文档</span>
                  </Link>,
              ]
            : [],
        menuHeaderRender: undefined,
        // 自定义 403 页面
        // unAccessible: <div>unAccessible</div>,
        // 增加一个 loading 的状态
        childrenRender: (children, props) => {
            // if (initialState?.loading) return <PageLoading />;
            return (
                <>
                    {children}
                    {isDev && !props.location?.pathname?.includes('/login') && (
                        <SettingDrawer
                            disableUrlParams
                            enableDarkTheme
                            settings={initialState?.settings}
                            onSettingChange={(settings) => {
                                setInitialState((preInitialState) => ({
                                    ...preInitialState,
                                    settings,
                                }));
                            }}
                        />
                    )}
                </>
            );
        },
        ...initialState?.settings,
    };
};

// const httpRequestCodeMessage: Record<number, string> = {
//     200: '服务器成功返回请求的数据。',
//     201: '新建或修改数据成功。',
//     202: '一个请求已经进入后台排队（异步任务）。',
//     204: '删除数据成功。',
//     400: '发出的请求有错误。',
//     401: '未授权，请先登录',
//     403: '无权访问',
//     404: '请求的资源不存在',
//     406: '请求的格式不可得。',
//     410: '请求的资源被永久删除，且不会再得到的。',
//     422: '当创建一个对象时，发生一个验证错误。',
//     500: '服务器发生错误，请检查服务器。',
//     501: '网关错误。',
//     502: '网关错误。',
//     503: '服务不可用，服务器暂时过载或维护。',
//     504: '网关超时。',
// };

const httpRequestCodeMessageDefaults: Record<number, string> = {
    200: 'The server successfully returned the requested data. ',
    201: 'Create or modify data successfully. ',
    202: 'A request has been queued in the background (asynchronous task). ',
    204: 'Delete data successfully. ',
    400: 'There was an error in the request issued. ',
    401: 'Unauthorized, please log in first. ',
    403: 'No access. ',
    404: 'The requested resource does not exist. ',
    406: 'The requested format is not available. ',
    410: 'The requested resource has been permanently deleted and will no longer be available. ',
    422: 'A validation error occurred while creating an object. ',
    500: 'An error occurred on the server, please check the server. ',
    501: 'Bad gateway. ',
    502: 'Bad gateway. ',
    503: 'The service is unavailable, the server is temporarily overloaded or under maintenance. ',
    504: 'Gateway timed out. ',
};

export const request: RequestConfig = {
    errorHandler: (error) => {
        const { data, response } = error;
        if (response && response.status >= 300) {
            const errorMessage = formatMessage({
                id: `common.http.response.statusCode.${response.status}`,
                defaultMessage: httpRequestCodeMessageDefaults[response.status],
            });

            let errorText = errorMessage || response.statusText;
            const { status } = response;

            if ((status == 403 || status == 400) && data?.error?.message)
                errorText = data?.error?.message;

            if (!errorText && status == 400) {
                errorText = 'Please refresh and try again';
            }

            message.error(
                errorText ?? formatMessage({ id: 'common.http.response.error.requestFailed' }),
            );
        }
        // else if (error.message == 'cancel' || error.message == 'aborted') {
        //     console.debug('canceled');
        // }
        else if (error.toString().indexOf('aborted') >= 0) {
            console.debug('aborted.', error);
        } else if (!response) {
            notification.error({
                description: formatMessage({ id: 'common.http.response.error.networkError1' }),
                message: formatMessage({ id: 'common.http.response.error.networkError2' }),
            });
        }

        // 如果状态码非 20X，则返回 null
        return null;
    },
    requestInterceptors: [
        (url, options) => {
            if (options.headers) {
                options.headers['RequestVerificationToken'] = Cookies.get('XSRF-TOKEN');

                const locale = window.localStorage.getItem('umi_locale');
                if (locale) options.headers['Accept-Language'] = locale;
            }
            return { url, options };
        },
    ],
};
