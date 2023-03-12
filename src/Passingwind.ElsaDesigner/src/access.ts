import allRoutes from '../config/routes';
import type { API } from './services/typings';

/**
 * @see https://umijs.org/zh-CN/plugins/plugin-access
 * */
export default function access(
    initialState:
        | { currentUser?: API.CurrentUser; grantedPolicies: Record<string, boolean> }
        | undefined,
) {
    const { currentUser, grantedPolicies } = initialState ?? {};
    const obj = {
        isAdmin: currentUser && (currentUser.roles ?? []).indexOf('admin') >= 0,
    };

    const base = {};

    function getBase(data: any[]) {
        data.forEach((ele: { access: any; routes: any }) => {
            if (ele.access) {
                Object.assign(base, { [ele.access]: false });
            }
            if (ele.routes) {
                getBase(ele.routes);
            }
        });
    }

    getBase(allRoutes);

    return Object.assign(base, grantedPolicies, { public: true }, obj);
}
