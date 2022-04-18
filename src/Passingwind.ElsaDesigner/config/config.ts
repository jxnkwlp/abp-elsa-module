// https://umijs.org/config/
import { defineConfig } from 'umi';
import { join } from 'path';

import defaultSettings from './defaultSettings';
import proxy from './proxy';
import routes from './routes';

import MonacoWebpackPlugin from 'monaco-editor-webpack-plugin';

const { REACT_APP_ENV } = process.env;

export default defineConfig({
    hash: true,
    antd: {},
    dva: {
        hmr: true,
    },
    layout: {
        // https://umijs.org/zh-CN/plugins/plugin-layout
        locale: false,
        siderWidth: 208,
        ...defaultSettings,
    },
    // https://umijs.org/zh-CN/plugins/plugin-locale
    locale: {
        // default zh-CN
        default: 'zh-CN',
        antd: true,
        // default true, when it is true, will use `navigator.language` overwrite default
        baseNavigator: true,
    },
    dynamicImport: {
        loading: '@ant-design/pro-layout/es/PageLoading',
    },
    targets: {
        ie: 11,
    },
    // umi routes: https://umijs.org/docs/routing
    routes,
    // Theme for antd: https://ant.design/docs/react/customize-theme-cn
    theme: {
        'root-entry-name': 'variable',
    },
    // esbuild is father build tools
    // https://umijs.org/plugins/plugin-esbuild
    esbuild: {
        target: 'es2020',
    },
    title: false,
    ignoreMomentLocale: true,
    proxy: proxy[REACT_APP_ENV || 'dev'],
    manifest: {
        basePath: '/',
    },
    // Fast Refresh 热更新
    fastRefresh: {},
    // openAPI: [
    //     {
    //         requestLibPath: "import { request } from 'umi'",
    //         // 或者使用在线的版本
    //         // schemaPath: "https://gw.alipayobjects.com/os/antfincdn/M%24jrzTTYJN/oneapi.json"
    //         schemaPath: join(__dirname, 'oneapi.json'),
    //         mock: false,
    //     },
    //     {
    //         requestLibPath: "import { request } from 'umi'",
    //         schemaPath: 'https://gw.alipayobjects.com/os/antfincdn/CA1dOm%2631B/openapi.json',
    //         projectName: 'swagger',
    //     },
    // ],
    nodeModulesTransform: { type: 'none' },
    mfsu: {},
    webpack5: {},
    exportStatic: {},
    // devtool: // false/'eval',
    chainWebpack: (memo) => {
        // 更多配置 https://github.com/Microsoft/monaco-editor-webpack-plugin#options
        memo.plugin('monaco-editor-webpack-plugin').use(MonacoWebpackPlugin, [
            { languages: ['javascript', 'json', 'typescript', 'liquid', 'handlebars'] },
        ]);
        return memo;
    },
});
