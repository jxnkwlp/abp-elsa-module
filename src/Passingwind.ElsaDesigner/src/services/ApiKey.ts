/**
 * Generate from swagger json url: https://localhost:44315/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 5
 **/
// @ts-ignore
/* eslint-disable */
import type { API } from "./typings";
import { request } from 'umi';

/**
 * *TODO* POST /api/secrets/apikeys 
 **/
export async function createApiKey(
    payload: API.ApiKeyCreateOrUpdate,
    options?: { [key: string]: any }
) {
    return request<API.ApiKey>(`/api/secrets/apikeys`, {
        method: 'POST',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* DELETE /api/secrets/apikeys/{id} 
 **/
export async function deleteApiKey(
    id: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/secrets/apikeys/${id}`, {
        method: 'DELETE',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/secrets/apikeys/{id} 
 **/
export async function getApiKey(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.ApiKey>(`/api/secrets/apikeys/${id}`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/secrets/apikeys 
 **/
export async function getApiKeyList(
    params: {
        skipCount?: number | undefined,
        maxResultCount?: number | undefined
    },
    options?: { [key: string]: any }
) {
    return request<API.ApiKeyPagedResult>(`/api/secrets/apikeys`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO* PUT /api/secrets/apikeys/{id} 
 **/
export async function updateApiKey(
    id: string,
    payload: API.ApiKeyCreateOrUpdate,
    options?: { [key: string]: any }
) {
    return request<API.ApiKey>(`/api/secrets/apikeys/${id}`, {
        method: 'PUT',
        data: payload,
        ...(options || {}),
    });
}
