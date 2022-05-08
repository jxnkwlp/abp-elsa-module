import type { Settings as LayoutSettings } from '@ant-design/pro-layout';
import { SettingDrawer } from '@ant-design/pro-layout';
import { PageLoading } from '@ant-design/pro-layout';
import type { RequestConfig, RunTimeLayoutConfig } from 'umi';
import { history, Link } from 'umi';
import RightContent from '@/components/RightContent';
import Footer from '@/components/Footer';
import { currentUser as queryCurrentUser } from './services/ant-design-pro/api';
import { BookOutlined, LinkOutlined } from '@ant-design/icons';
import defaultSettings from '../config/defaultSettings';
import { ConfigProvider, message, notification } from 'antd';
import enUS from 'antd/lib/locale/en_US';

const isDev = process.env.NODE_ENV === 'development';
const loginPath = '/user/login';

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
    // fetchUserInfo?: () => Promise<API.CurrentUser | undefined>;
}> {
    return {
        settings: defaultSettings,
    };
}

// ProLayout 支持的api https://procomponents.ant.design/components/layout
export const layout: RunTimeLayoutConfig = ({ initialState, setInitialState }) => {
    return {
        rightContentRender: () => <RightContent />,
        disableContentMargin: false,
        waterMarkProps: {
            content: initialState?.currentUser?.name,
        },
        footerRender: () => <Footer />,
        // onPageChange: () => {
        //     const { location } = history;
        //     // 如果没有登录，重定向到 login
        //     if (!initialState?.currentUser && location.pathname !== loginPath) {
        //         history.push(loginPath);
        //     }
        // },
        menuHeaderRender: undefined,
        // 自定义 403 页面
        // unAccessible: <div>unAccessible</div>,
        // 增加一个 loading 的状态
        childrenRender: (children, props) => {
            // if (initialState?.loading) return <PageLoading />;
            return (
                <>
                    <ConfigProvider locale={enUS}>{children}</ConfigProvider>
                    {isDev && !props.location?.pathname?.includes('/login') && (
                        <SettingDrawer
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

const httpRequestCodeMessage: Record<number, string> = {
    200: '服务器成功返回请求的数据。',
    201: '新建或修改数据成功。',
    202: '一个请求已经进入后台排队（异步任务）。',
    204: '删除数据成功。',
    400: '发出的请求有错误。',
    401: '未授权，请先登录',
    403: '无权访问',
    404: '请求的资源不存在',
    406: '请求的格式不可得。',
    410: '请求的资源被永久删除，且不会再得到的。',
    422: '当创建一个对象时，发生一个验证错误。',
    500: '服务器发生错误，请检查服务器。',
    501: '网关错误。',
    502: '网关错误。',
    503: '服务不可用，服务器暂时过载或维护。',
    504: '网关超时。',
};

export const request: RequestConfig = {
    errorHandler: (error) => {
        const { data, response } = error;
        if (response && response.status) {
            let errorText = httpRequestCodeMessage[response.status] || response.statusText;
            const { status } = response;

            if ((status == 403 || status == 400) && data?.error?.message)
                errorText = data?.error?.message;

            message.error(errorText ?? '请求失败');
        } else if (!response) {
            notification.error({
                description: '您的网络发生异常，无法连接服务器',
                message: '网络异常',
            });
        }

        // 如果状态码非 20X，则返回 null
        return null;
    },
};
