/**
 * Generate from url: https://localhost:44345/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 1
 **/
// @ts-ignore
/* eslint-disable */
import type { API } from "./typings";
import { request } from 'umi';

/**
 * *TODO* GET /api/elsa/workflow/channels 
 **/
export async function getWorkflowChannelList(
    options?: { [key: string]: any }
) {
    return request<API.StringListResult>(`/api/elsa/workflow/channels`, {
        method: 'GET',
        ...(options || {}),
    });
}
