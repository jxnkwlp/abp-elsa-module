import { API } from "./services/typings";

/**
 * @see https://umijs.org/zh-CN/plugins/plugin-access
 * */
export default function access(initialState: { currentUser?: API.CurrentUser, grantedPolicies: Record<string, boolean> } | undefined) {
    const { currentUser, grantedPolicies } = initialState ?? {};
    const obj = {
        isAdmin: currentUser && (currentUser.roles ?? []).indexOf('admin') >= 0
    };

    return Object.assign({}, grantedPolicies, obj);
}
