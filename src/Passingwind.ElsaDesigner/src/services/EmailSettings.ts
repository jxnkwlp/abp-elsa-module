/**
 * Generate from swagger json url: https://localhost:44315/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 3
 **/
// @ts-ignore
/* eslint-disable */
import type { API } from "./typings";
import { request } from 'umi';

/**
 * *TODO* POST /api/setting-management/emailing/send-test-email 
 **/
export async function emailSettingsSendTestEmail(
    payload: API.SendTestEmailInput,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/setting-management/emailing/send-test-email`, {
        method: 'POST',
        data: payload,
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/setting-management/emailing 
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
