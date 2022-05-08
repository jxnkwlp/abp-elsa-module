/**
 * Generate from swagger json url: https://localhost:44315/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 4
 **/
// @ts-ignore
/* eslint-disable */
import { request } from 'umi';

/**
 * *TODO* GET /api/identity/users/lookup/{id}
 * 
 **/
export async function userLookupFindById(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.UserData>(`/api/identity/users/lookup/${id}`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/identity/users/lookup/by-username/{userName}
 * 
 **/
export async function userLookupFindByUserName(
    userName: string,
    options?: { [key: string]: any }
) {
    return request<API.UserData>(`/api/identity/users/lookup/by-username/${userName}`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/identity/users/lookup/search
 * 
 **/
export async function userLookupSearch(
    params: {
        filter?: string | undefined,
        sorting?: string | undefined,
        skipCount?: number | undefined,
        maxResultCount?: number | undefined
    },
    options?: { [key: string]: any }
) {
    return request<API.UserDataListResult>(`/api/identity/users/lookup/search`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/identity/users/lookup/count
 * 
 **/
export async function getUserLookupCount(
    params: {
        filter?: string | undefined
    },
    options?: { [key: string]: any }
) {
    return request<any>(`/api/identity/users/lookup/count`, {
        method: 'GET',
        params: params,
        getResponse: true,
        ...(options || {}),
    });
}
