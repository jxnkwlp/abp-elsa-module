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
 * *TODO* GET /api/permission-management/permissions 
 **/
export async function getPermissions(
    params: {
        providerName?: string | undefined,
        providerKey?: string | undefined
    },
    options?: { [key: string]: any }
) {
    return request<API.GetPermissionListResult>(`/api/permission-management/permissions`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO* PUT /api/permission-management/permissions 
 **/
export async function updatePermissions(
    params: {
        providerName?: string | undefined,
        providerKey?: string | undefined
    },
    payload: API.UpdatePermissions,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/permission-management/permissions`, {
        method: 'PUT',
        params: params,
        data: payload,
        getResponse: true,
        ...(options || {}),
    });
}
