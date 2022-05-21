/**
 * Generate from swagger json url: https://localhost:44324/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 9
 **/
// @ts-ignore
/* eslint-disable */
import { request } from 'umi';

/**
 * *TODO* DELETE /api/workflow-instances
 * 
 **/
export async function batchDeleteWorkflowInstance(
    payload: API.WorkflowInstancesBatchDeleteRequest,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/workflow-instances`, {
        method: 'DELETE',
        data: payload,
        getResponse: true,
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

/**
 * *TODO* POST /api/workflow-instances/{id}/cancel
 * 
 **/
export async function workflowInstanceCancel(
    id: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/workflow-instances/${id}/cancel`, {
        method: 'POST',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* DELETE /api/workflow-instances/{id}
 * 
 **/
export async function deleteWorkflowInstance(
    id: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/workflow-instances/${id}`, {
        method: 'DELETE',
        getResponse: true,
        ...(options || {}),
    });
}

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
 * *TODO* POST /api/workflow-instances/{id}/dispatch
 * 
 **/
export async function workflowInstanceDispatch(
    id: string,
    payload: API.WorkflowInstanceDispatchRequest,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/workflow-instances/${id}/dispatch`, {
        method: 'POST',
        data: payload,
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/workflow-instances/{id}/execute
 * 
 **/
export async function workflowInstanceExecute(
    id: string,
    payload: API.WorkflowInstanceExecuteRequest,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/workflow-instances/${id}/execute`, {
        method: 'POST',
        data: payload,
        getResponse: true,
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
 * *TODO* POST /api/workflow-instances/{id}/retry
 * 
 **/
export async function workflowInstanceRetry(
    id: string,
    payload: API.WorkflowInstanceRetryRequest,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/workflow-instances/${id}/retry`, {
        method: 'POST',
        data: payload,
        getResponse: true,
        ...(options || {}),
    });
}
