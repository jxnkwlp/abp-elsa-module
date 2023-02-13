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
 * *TODO* GET /api/elsa/workflow/providers 
 **/
export async function getWorkflowProviders(
    options?: { [key: string]: any }
) {
    return request<API.WorkflowProviderDescriptorListResult>(`/api/elsa/workflow/providers`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/storage-providers 
 **/
export async function getWorkflowStorageProviders(
    options?: { [key: string]: any }
) {
    return request<API.WorkflowStorageProviderInfoListResult>(`/api/elsa/workflow/storage-providers`, {
        method: 'GET',
        ...(options || {}),
    });
}
