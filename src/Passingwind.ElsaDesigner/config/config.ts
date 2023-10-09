// https://umijs.org/config/
import { defineConfig } from 'umi';

import MonacoWebpackPlugin from 'monaco-editor-webpack-plugin';
import defaultSettings from './defaultSettings';
import proxy from './proxy';
import routes from './routes';

const { REACT_APP_ENV } = process.env;

export default defineConfig({
    hash: true,
    antd: {},
    dva: {
        hmr: true,
    },
    layout: {
        // https://umijs.org/zh-CN/plugins/plugin-layout
        locale: true,
        siderWidth: 208,
        ...defaultSettings,
    },
    // https://umijs.org/zh-CN/plugins/plugin-locale
    locale: {
        default: 'en-US',
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
    access: {
        strictMode: true,
    },
    // Theme for antd: https://ant.design/docs/react/customize-theme-cn
    theme: {
        // 如果不想要 configProvide 动态设置主题需要把这个设置为 default
        // 只有设置为 variable， 才能使用 configProvide 动态设置主色调
        // https://ant.design/docs/react/customize-theme-variable-cn
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
    openAPI: [],
    nodeModulesTransform: { type: 'none' },
    mfsu: {},
    webpack5: {},
    exportStatic: {},
    chainWebpack: (memo) => {
        // 更多配置 https://github.com/Microsoft/monaco-editor-webpack-plugin#options
        memo.plugin('monaco-editor-webpack-plugin').use(MonacoWebpackPlugin, [
            {
                languages: [
                    'json',
                    'yaml',
                    'javascript',
                    'typescript',
                    'liquid',
                    'handlebars',
                    'csharp',
                    'sql',
                ],
                // customLanguages: [
                //     {
                //         label: 'yaml',
                //         entry: 'monaco-yaml',
                //         worker: {
                //             id: 'monaco-yaml/yamlWorker',
                //             entry: 'monaco-yaml/yaml.worker',
                //         },
                //     },
                // ],
            },
        ]);
        return memo;
    },
});
