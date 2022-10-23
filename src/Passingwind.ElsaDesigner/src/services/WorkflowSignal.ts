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
 * *TODO* POST /api/elsa/workflows/signals/dispatch/{signalName} 
 **/
export async function workflowSignalDispatch(
    signalName: string,
    payload: API.WorkflowSignalDispatchRequest,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowSignalDispatchResult>(`/api/elsa/workflows/signals/dispatch/${signalName}`, {
        method: 'POST',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/elsa/workflows/signals/execute/{signalName} 
 **/
export async function workflowSignalExecute(
    signalName: string,
    payload: API.WorkflowSignalExecuteRequest,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowSignalExecuteResult>(`/api/elsa/workflows/signals/execute/${signalName}`, {
        method: 'POST',
        data: payload,
        ...(options || {}),
    });
}
