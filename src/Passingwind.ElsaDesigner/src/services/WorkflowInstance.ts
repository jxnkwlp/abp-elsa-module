/**
 * Generate from swagger json url: https://localhost:44324/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 3
 **/
// @ts-ignore
/* eslint-disable */
import { request } from 'umi';

/**
 * *TODO* GET /api/workflow-instances/{id}
 * 
 **/
export async function getWorkflowInstance(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowInstance>(`/api/workflow-instances/${id}`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/workflow-instances/{id}/execution-logs
 * 
 **/
export async function getWorkflowInstanceExecutionLogs(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowExecutionLogListResult>(`/api/workflow-instances/${id}/execution-logs`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/workflow-instances
 * 
 **/
export async function getWorkflowInstanceList(
    params: {
        name?: string | undefined,
        version?: number | undefined,
        workflowStatus?: any | undefined,
        correlationId?: string | undefined,
        skipCount?: number | undefined,
        maxResultCount?: number | undefined
    },
    options?: { [key: string]: any }
) {
    return request<API.WorkflowInstanceBasicPagedResult>(`/api/workflow-instances`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}
