/**
 * Generate from swagger json url: https://localhost:44315/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 3
 **/
// @ts-ignore
/* eslint-disable */
import { request } from 'umi';

/**
 * *TODO* POST /api/account/register
 * 
 **/
export async function accountRegister(
    payload: API.Register,
    options?: { [key: string]: any }
) {
    return request<API.IdentityUser>(`/api/account/register`, {
        method: 'POST',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/account/send-password-reset-code
 * 
 **/
export async function accountSendPasswordResetCode(
    payload: API.SendPasswordResetCode,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/account/send-password-reset-code`, {
        method: 'POST',
        data: payload,
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/account/reset-password
 * 
 **/
export async function accountResetPassword(
    payload: API.ResetPassword,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/account/reset-password`, {
        method: 'POST',
        data: payload,
        getResponse: true,
        ...(options || {}),
    });
}
