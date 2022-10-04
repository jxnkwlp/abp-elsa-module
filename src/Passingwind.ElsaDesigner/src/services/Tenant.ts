/**
 * Generate from swagger json url: https://localhost:44315/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 8
 **/
// @ts-ignore
/* eslint-disable */
import type { API } from "./typings";
import { request } from 'umi';

/**
 * *TODO* POST /api/multi-tenancy/tenants 
 **/
export async function createTenant(
    payload: API.TenantCreate,
    options?: { [key: string]: any }
) {
    return request<API.Tenant>(`/api/multi-tenancy/tenants`, {
        method: 'POST',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* DELETE /api/multi-tenancy/tenants/{id} 
 **/
export async function deleteTenant(
    id: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/multi-tenancy/tenants/${id}`, {
        method: 'DELETE',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* DELETE /api/multi-tenancy/tenants/{id}/default-connection-string 
 **/
export async function deleteTenantDefaultConnectionString(
    id: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/multi-tenancy/tenants/${id}/default-connection-string`, {
        method: 'DELETE',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/multi-tenancy/tenants/{id} 
 **/
export async function getTenant(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.Tenant>(`/api/multi-tenancy/tenants/${id}`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/multi-tenancy/tenants/{id}/default-connection-string 
 **/
export async function getTenantDefaultConnectionString(
    id: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/multi-tenancy/tenants/${id}/default-connection-string`, {
        method: 'GET',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/multi-tenancy/tenants 
 **/
export async function getTenantList(
    params: {
        filter?: string | undefined,
        sorting?: string | undefined,
        skipCount?: number | undefined,
        maxResultCount?: number | undefined
    },
    options?: { [key: string]: any }
) {
    return request<API.TenantPagedResult>(`/api/multi-tenancy/tenants`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO* PUT /api/multi-tenancy/tenants/{id} 
 **/
export async function updateTenant(
    id: string,
    payload: API.TenantUpdate,
    options?: { [key: string]: any }
) {
    return request<API.Tenant>(`/api/multi-tenancy/tenants/${id}`, {
        method: 'PUT',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* PUT /api/multi-tenancy/tenants/{id}/default-connection-string 
 **/
export async function updateTenantDefaultConnectionString(
    id: string,
    params: {
        defaultConnectionString?: string | undefined
    },
    options?: { [key: string]: any }
) {
    return request<any>(`/api/multi-tenancy/tenants/${id}/default-connection-string`, {
        method: 'PUT',
        params: params,
        getResponse: true,
        ...(options || {}),
    });
}
