/**
 * Generate from swagger json url: https://localhost:44324/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 2
 **/
// @ts-ignore
/* eslint-disable */
import { request } from 'umi';

/**
 * *TODO* GET /api/workflow-execution-logs/{id}
 * 
 **/
export async function getWorkflowExecutionLog(
    id: number,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowExecutionLog>(`/api/workflow-execution-logs/${id}`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/workflow-execution-logs
 * 
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
    return request<API.WorkflowExecutionLogPagedResult>(`/api/workflow-execution-logs`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}
