/**
 * Generate from swagger json url: https://localhost:44315/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 3
 **/
// @ts-ignore
/* eslint-disable */
import { request } from 'umi';

/**
 * *TODO* POST /api/account/login
 * 
 **/
export async function loginLogin(
    payload: API.UserLoginInfo,
    options?: { [key: string]: any }
) {
    return request<API.AbpLoginResult>(`/api/account/login`, {
        method: 'POST',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/account/logout
 * 
 **/
export async function loginLogout(
    options?: { [key: string]: any }
) {
    return request<any>(`/api/account/logout`, {
        method: 'GET',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/account/check-password
 * 
 **/
export async function loginCheckPassword(
    payload: API.UserLoginInfo,
    options?: { [key: string]: any }
) {
    return request<API.AbpLoginResult>(`/api/account/check-password`, {
        method: 'POST',
        data: payload,
        ...(options || {}),
    });
}
