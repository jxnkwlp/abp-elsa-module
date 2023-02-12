import component from './en-US/component';
import globalHeader from './en-US/globalHeader';
import menu from './en-US/menu';
import pages from './en-US/pages';
import pwa from './en-US/pwa';
import settingDrawer from './en-US/settingDrawer';
import settings from './en-US/settings';
import designer from './en-US/designer'

export default {
    'navBar.lang': 'Languages',
    'layout.user.link.help': 'Help',
    'layout.user.link.privacy': 'Privacy',
    'layout.user.link.terms': 'Terms',
    'app.copyright.produced': 'Produced by Passingwind',
    'app.preview.down.block': 'Download this page to your local project',
    'app.welcome.link.fetch-blocks': 'Get all block',
    'app.welcome.link.block-list': 'Quickly build standard, pages based on `block` development',
    'app.logout': 'Logout',
    'app.changepassword': 'Change Password',
    'app.account.apikeys': 'API Keys',
    'app.account.settings': 'Settings',
    'app.account.center': 'Center',
    'common.dict.create': 'Create',
    'common.dict.edit': 'Edit',
    'common.dict.delete': 'Delete',
    'common.dict.delete.confirm': "Are you sure delete?",
    'common.dict.delete.success': "Deleted successfully",
    'common.dict.save': 'Save',
    'common.dict.submit': 'Submit',
    'common.dict.created.success': 'Added successfully',
    'common.dict.modified.success': 'Successfully modified',
    'common.dict.deleted.success': 'Successfully deleted',
    'common.dict.table-action': "Action",
    'common.dict.table.clearSelected': "Clear Selected",
    'common.dict.success': "Success",
    'common.dict.creationTime': 'Creation Time',
    'common.dict.lastModificationTime': 'Modification Time',
    'common.dict.import': 'Import',
    'common.dict.export': 'Export',
    'common.dict.loading': 'Loading...',
    'common.dict.default': 'Default',
    'common.dict.info.tips': 'Notice',
    'common.dict.error.tips': 'Error',
    'common.dict.success.tips': 'Notice',
    'common.http.response.error.requestFailed': 'Request failed',
    'common.http.response.error.networkError1': 'network anomaly',
    'common.http.response.error.networkError2': 'Your network is abnormal, unable to connect to the server',
    'common.noaccess': 'You do not have the necessary permission to view this area or page.',
    ...globalHeader,
    ...menu,
    ...settingDrawer,
    ...settings,
    ...pwa,
    ...component,
    ...pages,
    ...designer,
};
