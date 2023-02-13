/**
 * Generate from url: https://localhost:44345/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 2
 **/
// @ts-ignore
/* eslint-disable */
import type { API } from "./typings";
import { request } from 'umi';

/**
 * *TODO* GET /api/setting-management/oauth2 
 **/
export async function getOAuth2Settings(
    options?: { [key: string]: any }
) {
    return request<API.OAuth2Settings>(`/api/setting-management/oauth2`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/setting-management/oauth2 
 **/
export async function updateOAuth2Settings(
    payload: API.OAuth2SettingUpdate,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/setting-management/oauth2`, {
        method: 'POST',
        data: payload,
        getResponse: true,
        ...(options || {}),
    });
}
