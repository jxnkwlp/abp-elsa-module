/**
 * Generate from swagger json url: https://localhost:44315/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 2
 **/
// @ts-ignore
/* eslint-disable */
import { request } from 'umi';

/**
 * *TODO* GET /api/setting-management/emailing
 * 
 **/
export async function getEmailSettings(
    options?: { [key: string]: any }
) {
    return request<API.EmailSettings>(`/api/setting-management/emailing`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/setting-management/emailing
 * 
 **/
export async function updateEmailSettings(
    payload: API.UpdateEmailSettings,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/setting-management/emailing`, {
        method: 'POST',
        data: payload,
        getResponse: true,
        ...(options || {}),
    });
}
