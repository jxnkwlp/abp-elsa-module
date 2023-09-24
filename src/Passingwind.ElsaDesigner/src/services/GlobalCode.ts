/**
 * Generate from url: https://localhost:44345/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 8
 **/
// @ts-ignore
/* eslint-disable */
import type { API } from "./typings";
import { request } from 'umi';

/**
 * *TODO* POST /api/elsa/workflow/global-codes 
 **/
export async function createGlobalCode(
    payload: API.GlobalCodeCreateOrUpdate,
    options?: { [key: string]: any }
) {
    return request<API.GlobalCode>(`/api/elsa/workflow/global-codes`, {
        method: 'POST',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* DELETE /api/elsa/workflow/global-codes/{id} 
 **/
export async function deleteGlobalCode(
    id: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/workflow/global-codes/${id}`, {
        method: 'DELETE',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* DELETE /api/elsa/workflow/global-codes/{id}/versions/{version} 
 **/
export async function deleteGlobalCodeByVersion(
    id: string,    version: number,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/workflow/global-codes/${id}/versions/${version}`, {
        method: 'DELETE',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/global-codes/{id} 
 **/
export async function getGlobalCode(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.GlobalCode>(`/api/elsa/workflow/global-codes/${id}`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/global-codes/{id}/versions/{version} 
 **/
export async function getGlobalCodeByVersion(
    id: string,    version: number,
    options?: { [key: string]: any }
) {
    return request<API.GlobalCode>(`/api/elsa/workflow/global-codes/${id}/versions/${version}`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/global-codes 
 **/
export async function getGlobalCodeList(
    params: {
        filter?: string | undefined,
        sorting?: string | undefined,
        skipCount?: number | undefined,
        maxResultCount?: number | undefined
    },
    options?: { [key: string]: any }
) {
    return request<API.GlobalCodePagedResult>(`/api/elsa/workflow/global-codes`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/global-codes/{id}/versions 
 **/
export async function getGlobalCodeVersions(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.Int32ListResult>(`/api/elsa/workflow/global-codes/${id}/versions`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* PUT /api/elsa/workflow/global-codes/{id} 
 **/
export async function updateGlobalCode(
    id: string,
    payload: API.GlobalCodeCreateOrUpdate,
    options?: { [key: string]: any }
) {
    return request<API.GlobalCode>(`/api/elsa/workflow/global-codes/${id}`, {
        method: 'PUT',
        data: payload,
        ...(options || {}),
    });
}
