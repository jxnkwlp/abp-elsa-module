import component from './zh-CN/component';
import globalHeader from './zh-CN/globalHeader';
import menu from './zh-CN/menu';
import pwa from './zh-CN/pwa';
import settingDrawer from './zh-CN/settingDrawer';
import settings from './zh-CN/settings';
import pages from './zh-CN/pages';
import designer from './zh-CN/designer'

export default {
    'navBar.lang': '语言',
    'layout.user.link.help': '帮助',
    'layout.user.link.privacy': '隐私',
    'layout.user.link.terms': '条款',
    'app.copyright.produced': 'Produced by Passingwind',
    'app.preview.down.block': '下载此页面到本地项目',
    'app.welcome.link.fetch-blocks': '获取全部区块',
    'app.welcome.link.block-list': '基于 block 开发，快速构建标准页面',
    'common.dict.create': '添加',
    'common.dict.edit': '编辑',
    'common.dict.delete': '删除',
    'common.dict.delete.confirm': "确定要删除吗？",
    'common.dict.delete.success': "删除成功",
    'common.dict.save': '保存',
    'common.dict.save.success': '保存成功',
    'common.dict.submit': '提交',
    'common.dict.created.success': '添加成功',
    'common.dict.modified.success': '修改成功',
    'common.dict.deleted.success': '删除成功',
    'common.dict.table-action': "操作",
    'common.dict.creationTime': '创建时间',
    'common.dict.lastModificationTime': '修改时间',
    'common.dict.import': '导入',
    'common.dict.export': '导出',
    'common.dict.loading': '加载中...',
    'common.dict.default': '默认',
    ...pages,
    ...globalHeader,
    ...menu,
    ...settingDrawer,
    ...settings,
    ...pwa,
    ...component,
    ...designer,
};
