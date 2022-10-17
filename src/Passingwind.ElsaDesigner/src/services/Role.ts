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
 * *TODO* POST /api/identity/roles 
 **/
export async function createRole(
    payload: API.IdentityRoleCreate,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRole>(`/api/identity/roles`, {
        method: 'POST',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* DELETE /api/identity/roles/{id} 
 **/
export async function deleteRole(
    id: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/identity/roles/${id}`, {
        method: 'DELETE',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/identity/roles/all 
 **/
export async function getAllRoleList(
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleListResult>(`/api/identity/roles/all`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/identity/roles/{id} 
 **/
export async function getRole(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRole>(`/api/identity/roles/${id}`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/identity/roles 
 **/
export async function getRoleList(
    params: {
        filter?: string | undefined,
        sorting?: string | undefined,
        skipCount?: number | undefined,
        maxResultCount?: number | undefined
    },
    options?: { [key: string]: any }
) {
    return request<API.IdentityRolePagedResult>(`/api/identity/roles`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO* PUT /api/identity/roles/{id} 
 **/
export async function updateRole(
    id: string,
    payload: API.IdentityRoleUpdate,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRole>(`/api/identity/roles/${id}`, {
        method: 'PUT',
        data: payload,
        ...(options || {}),
    });
}
