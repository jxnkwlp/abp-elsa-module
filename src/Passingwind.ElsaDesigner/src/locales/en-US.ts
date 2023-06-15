import component from './en-US/component';
import globalHeader from './en-US/globalHeader';
import menu from './en-US/menu';
import pages from './en-US/pages';
import pwa from './en-US/pwa';
import settingDrawer from './en-US/settingDrawer';
import settings from './en-US/settings';
import designer from './en-US/designer';

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
    'common.dict.confirm': 'Confirm',
    'common.dict.delete': 'Delete',
    'common.dict.delete.confirm': 'Are you sure you want to delete it? Deleted and not recoverable',
    'common.dict.delete.success': 'Deleted successfully',
    'common.dict.save': 'Save',
    'common.dict.save.success': 'Saved successfully',
    'common.dict.cancel': 'Cancel',
    'common.dict.submit': 'Submit',
    'common.dict.created.success': 'Added successfully',
    'common.dict.modified.success': 'Successfully modified',
    'common.dict.deleted.success': 'Successfully deleted',
    'common.dict.table-action': 'Action',
    'common.dict.table.clearSelected': 'Clear Selected',
    'common.dict.success': 'Success',
    'common.dict.creationTime': 'Creation Time',
    'common.dict.lastModificationTime': 'Modification Time',
    'common.dict.import': 'Import',
    'common.dict.export': 'Export',
    'common.dict.loading': 'Loading...',
    'common.dict.default': 'Default',
    'common.dict.info.tips': 'Notice',
    'common.dict.error.tips': 'Error',
    'common.dict.success.tips': 'Notice',
    'common.dict.general': 'General',
    'common.dict.common': 'Common',
    'common.http.response.error.requestFailed': 'Request failed',
    'common.http.response.error.networkError1': 'Network Error',
    'common.http.response.error.networkError2':
        'An exception occurred in your network, unable to connect to the server',
    'common.http.response.statusCode.200': 'The server successfully returned the requested data',
    'common.http.response.statusCode.201': 'Create or modify data successfully',
    'common.http.response.statusCode.202':
        'A request has been queued in the background (asynchronous task)',
    'common.http.response.statusCode.204': 'The data was deleted successfully',
    'common.http.response.statusCode.400': 'There was an error in the outgoing request',
    'common.http.response.statusCode.401': 'Unauthorized',
    'common.http.response.statusCode.403': 'Insufficient permissions',
    'common.http.response.statusCode.404': 'The requested resource does not exist',
    'common.http.response.statusCode.406': 'The requested format is not available',
    'common.http.response.statusCode.410':
        'The requested resource has been permanently deleted and will no longer be available',
    'common.http.response.statusCode.422': 'A validation error occurred while creating an object',
    'common.http.response.statusCode.500':
        'An error occurred on the server, please check the server',
    'common.http.response.statusCode.501': 'Gateway error',
    'common.http.response.statusCode.502': 'Gateway error',
    'common.http.response.statusCode.503':
        'The service is unavailable, the server is temporarily overloaded or under maintenance',
    'common.http.response.statusCode.504': 'Gateway timed out',
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
