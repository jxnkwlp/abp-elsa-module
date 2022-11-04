/**
 * Generate from swagger json url: https://localhost:44315/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 6
 **/
// @ts-ignore
/* eslint-disable */
import type { API } from "./typings";
import { request } from 'umi';

/**
 * *TODO* POST /api/elsa/workflow/global-variables 
 **/
export async function createGlobalVariable(
    payload: API.GlobalVariableCreateOrUpdate,
    options?: { [key: string]: any }
) {
    return request<API.GlobalVariable>(`/api/elsa/workflow/global-variables`, {
        method: 'POST',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* DELETE /api/elsa/workflow/global-variables/{id} 
 **/
export async function deleteGlobalVariable(
    id: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/workflow/global-variables/${id}`, {
        method: 'DELETE',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/global-variables/{id} 
 **/
export async function getGlobalVariable(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.GlobalVariable>(`/api/elsa/workflow/global-variables/${id}`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/global-variables/by-key/{key} 
 **/
export async function getGlobalVariableByKey(
    key: string,
    options?: { [key: string]: any }
) {
    return request<API.GlobalVariable>(`/api/elsa/workflow/global-variables/by-key/${key}`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/global-variables 
 **/
export async function getGlobalVariableList(
    params: {
        filter?: string | undefined,
        isSecret?: boolean | undefined,
        skipCount?: number | undefined,
        maxResultCount?: number | undefined
    },
    options?: { [key: string]: any }
) {
    return request<API.GlobalVariablePagedResult>(`/api/elsa/workflow/global-variables`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO* PUT /api/elsa/workflow/global-variables/{id} 
 **/
export async function updateGlobalVariable(
    id: string,
    payload: API.GlobalVariableCreateOrUpdate,
    options?: { [key: string]: any }
) {
    return request<API.GlobalVariable>(`/api/elsa/workflow/global-variables/${id}`, {
        method: 'PUT',
        data: payload,
        ...(options || {}),
    });
}
