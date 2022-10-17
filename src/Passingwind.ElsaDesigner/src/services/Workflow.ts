/**
 * Generate from swagger json url: https://localhost:44315/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 2
 **/
// @ts-ignore
/* eslint-disable */
import type { API } from "./typings";
import { request } from 'umi';

/**
 * *TODO* GET /api/workflows/providers 
 **/
export async function getWorkflowProviders(
    options?: { [key: string]: any }
) {
    return request<API.WorkflowProviderDescriptorListResult>(`/api/workflows/providers`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/workflows/storage-providers 
 **/
export async function getWorkflowStorageProviders(
    options?: { [key: string]: any }
) {
    return request<API.WorkflowStorageProviderInfoListResult>(`/api/workflows/storage-providers`, {
        method: 'GET',
        ...(options || {}),
    });
}
