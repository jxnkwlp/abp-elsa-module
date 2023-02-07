/**
 * Generate from url: https://localhost:44315/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 10
 **/
// @ts-ignore
/* eslint-disable */
import type { API } from "./typings";
import { request } from 'umi';

/**
 * *TODO* POST /api/identity/users 
 **/
export async function createUser(
    payload: API.IdentityUserCreate,
    options?: { [key: string]: any }
) {
    return request<API.IdentityUser>(`/api/identity/users`, {
        method: 'POST',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* DELETE /api/identity/users/{id} 
 **/
export async function deleteUser(
    id: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/identity/users/${id}`, {
        method: 'DELETE',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/identity/users/{id} 
 **/
export async function getUser(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityUser>(`/api/identity/users/${id}`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/identity/users/assignable-roles 
 **/
export async function getUserAssignableRoles(
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleListResult>(`/api/identity/users/assignable-roles`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/identity/users 
 **/
export async function getUserList(
    params: {
        filter?: string | undefined,
        sorting?: string | undefined,
        skipCount?: number | undefined,
        maxResultCount?: number | undefined
    },
    options?: { [key: string]: any }
) {
    return request<API.IdentityUserPagedResult>(`/api/identity/users`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/identity/users/{id}/roles 
 **/
export async function getUserRoles(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleListResult>(`/api/identity/users/${id}/roles`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* PUT /api/identity/users/{id} 
 **/
export async function updateUser(
    id: string,
    payload: API.IdentityUserUpdate,
    options?: { [key: string]: any }
) {
    return request<API.IdentityUser>(`/api/identity/users/${id}`, {
        method: 'PUT',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* PUT /api/identity/users/{id}/roles 
 **/
export async function updateUserRoles(
    id: string,
    payload: API.IdentityUserUpdateRoles,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/identity/users/${id}/roles`, {
        method: 'PUT',
        data: payload,
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/identity/users/by-email/{email} 
 **/
export async function userFindByEmail(
    email: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityUser>(`/api/identity/users/by-email/${email}`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/identity/users/by-username/{userName} 
 **/
export async function userFindByUsername(
    userName: string,
    options?: { [key: string]: any }
) {
    return request<API.IdentityUser>(`/api/identity/users/by-username/${userName}`, {
        method: 'GET',
        ...(options || {}),
    });
}
