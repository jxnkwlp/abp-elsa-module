/**
 * Generate from url: https://localhost:44345/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 6
 **/
// @ts-ignore
/* eslint-disable */
import type { API } from "./typings";
import { request } from 'umi';

/**
 * *TODO* GET /api/account/login 
 **/
export async function getLogin(
    options?: { [key: string]: any }
) {
    return request<API.AccountResult>(`/api/account/login`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/account/check-password 
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

/**
 * *TODO* POST /api/account/login/external 
 **/
export async function loginExternalLogin(
    params: {
        provider?: string | undefined
    },
    options?: { [key: string]: any }
) {
    return request<any>(`/api/account/login/external`, {
        method: 'POST',
        params: params,
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/account/login/external/callback 
 **/
export async function loginExternalLoginCallback(
    params: {
        remoteError?: string | undefined
    },
    options?: { [key: string]: any }
) {
    return request<API.AbpLoginResult>(`/api/account/login/external/callback`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/account/login 
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
