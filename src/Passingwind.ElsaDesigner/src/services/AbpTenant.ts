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
 * *TODO* GET /api/abp/multi-tenancy/tenants/by-id/{id} 
 **/
export async function abpTenantFindTenantById(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.FindTenantResult>(`/api/abp/multi-tenancy/tenants/by-id/${id}`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/abp/multi-tenancy/tenants/by-name/{name} 
 **/
export async function abpTenantFindTenantByName(
    name: string,
    options?: { [key: string]: any }
) {
    return request<API.FindTenantResult>(`/api/abp/multi-tenancy/tenants/by-name/${name}`, {
        method: 'GET',
        ...(options || {}),
    });
}
