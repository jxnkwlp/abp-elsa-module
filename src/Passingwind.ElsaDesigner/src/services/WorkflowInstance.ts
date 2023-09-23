/**
 * Generate from url: https://localhost:44345/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 19
 **/
// @ts-ignore
/* eslint-disable */
import type { API } from "./typings";
import { request } from 'umi';

/**
 * *TODO* DELETE /api/elsa/workflow/instances 
 **/
export async function batchDeleteWorkflowInstance(
    payload: API.WorkflowInstanceBatchActionRequest,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/workflow/instances`, {
        method: 'DELETE',
        data: payload,
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* DELETE /api/elsa/workflow/instances/{id} 
 **/
export async function deleteWorkflowInstance(
    id: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/workflow/instances/${id}`, {
        method: 'DELETE',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/instances/{id} 
 **/
export async function getWorkflowInstance(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowInstance>(`/api/elsa/workflow/instances/${id}`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/instances/assignable-definitions 
 **/
export async function getWorkflowInstanceAssignableDefinition(
    params: {
        filter?: string | undefined,
        isSingleton?: boolean | undefined,
        deleteCompletedInstances?: boolean | undefined,
        channel?: string | undefined,
        tag?: string | undefined,
        groupId?: string | undefined,
        persistenceBehavior?: any | undefined,
        sorting?: string | undefined,
        skipCount?: number | undefined,
        maxResultCount?: number | undefined
    },
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDefinitionBasicPagedResult>(`/api/elsa/workflow/instances/assignable-definitions`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/instances/{id}/basic 
 **/
export async function getWorkflowInstanceBasic(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowInstanceBasic>(`/api/elsa/workflow/instances/${id}/basic`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/instances/{id}/definition 
 **/
export async function getWorkflowInstanceDefinition(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDefinitionVersion>(`/api/elsa/workflow/instances/${id}/definition`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/instances/{id}/execution-logs 
 **/
export async function getWorkflowInstanceExecutionLogs(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowExecutionLogListResult>(`/api/elsa/workflow/instances/${id}/execution-logs`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/instances/{id}/faults 
 **/
export async function getWorkflowInstanceFaults(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowInstanceFaultListResult>(`/api/elsa/workflow/instances/${id}/faults`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/instances/faults/by-definition/{id} 
 **/
export async function getWorkflowInstanceFaultsByWorkflowDefinition(
    id: string,
    params: {
        skipCount?: number | undefined,
        maxResultCount?: number | undefined
    },
    options?: { [key: string]: any }
) {
    return request<API.WorkflowInstanceFaultPagedResult>(`/api/elsa/workflow/instances/faults/by-definition/${id}`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/instances 
 **/
export async function getWorkflowInstanceList(
    params: {
        workflowDefinitionId?: string | undefined,
        name?: string | undefined,
        version?: number | undefined,
        workflowStatus?: any | undefined,
        correlationId?: string | undefined,
        creationTimes?: string[] | undefined,
        finishedTimes?: string[] | undefined,
        lastExecutedTimes?: string[] | undefined,
        faultedTimes?: string[] | undefined,
        sorting?: string | undefined,
        skipCount?: number | undefined,
        maxResultCount?: number | undefined
    },
    options?: { [key: string]: any }
) {
    return request<API.WorkflowInstanceBasicPagedResult>(`/api/elsa/workflow/instances`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/instances/{id}/execution-logs/summary 
 **/
export async function getWorkflowInstanceLogSummary(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowInstanceExecutionLogSummary>(`/api/elsa/workflow/instances/${id}/execution-logs/summary`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/instances/statistics/status 
 **/
export async function getWorkflowInstanceStatusCountStatistics(
    params: {
        workflowDefinitionId?: string | undefined
    },
    options?: { [key: string]: any }
) {
    return request<API.WorkflowInstanceStatusCountStatisticsResult>(`/api/elsa/workflow/instances/statistics/status`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/instances/statistics/count-date 
 **/
export async function getWorkflowInstanceStatusDateCountStatistics(
    params: {
        datePeriod?: number | undefined,
        tz?: number | undefined,
        workflowDefinitionId?: string | undefined
    },
    options?: { [key: string]: any }
) {
    return request<API.WorkflowInstanceDateCountStatisticsResult>(`/api/elsa/workflow/instances/statistics/count-date`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/elsa/workflow/instances/cancel 
 **/
export async function workflowInstanceBatchCancel(
    payload: API.WorkflowInstanceBatchActionRequest,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/workflow/instances/cancel`, {
        method: 'POST',
        data: payload,
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/elsa/workflow/instances/retry 
 **/
export async function workflowInstanceBatchRetry(
    payload: API.WorkflowInstanceBatchActionRequest,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/workflow/instances/retry`, {
        method: 'POST',
        data: payload,
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/elsa/workflow/instances/{id}/cancel 
 **/
export async function workflowInstanceCancel(
    id: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/workflow/instances/${id}/cancel`, {
        method: 'POST',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/elsa/workflow/instances/{id}/dispatch 
 **/
export async function workflowInstanceDispatch(
    id: string,
    payload: API.WorkflowInstanceDispatchRequest,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/workflow/instances/${id}/dispatch`, {
        method: 'POST',
        data: payload,
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/elsa/workflow/instances/{id}/execute 
 **/
export async function workflowInstanceExecute(
    id: string,
    payload: API.WorkflowInstanceExecuteRequest,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/workflow/instances/${id}/execute`, {
        method: 'POST',
        data: payload,
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/elsa/workflow/instances/{id}/retry 
 **/
export async function workflowInstanceRetry(
    id: string,
    payload: API.WorkflowInstanceRetryRequest,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/workflow/instances/${id}/retry`, {
        method: 'POST',
        data: payload,
        getResponse: true,
        ...(options || {}),
    });
}
