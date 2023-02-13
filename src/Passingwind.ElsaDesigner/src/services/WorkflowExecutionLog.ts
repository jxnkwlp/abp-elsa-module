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
 * *TODO* GET /api/elsa/workflow/execution-logs/{id} 
 **/
export async function getWorkflowExecutionLog(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowExecutionLog>(`/api/elsa/workflow/execution-logs/${id}`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/execution-logs 
 **/
export async function getWorkflowExecutionLogList(
    params: {
        workflowInstanceId?: string | undefined,
        activityId?: string | undefined,
        skipCount?: number | undefined,
        maxResultCount?: number | undefined
    },
    options?: { [key: string]: any }
) {
    return request<API.WorkflowExecutionLogPagedResult>(`/api/elsa/workflow/execution-logs`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}
