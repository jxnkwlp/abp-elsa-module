/**
 * Generate from swagger json url: https://localhost:44324/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 1
 **/
// @ts-ignore
/* eslint-disable */
import type { API } from "./typings";
import { request } from 'umi';

/**
 * *TODO* GET /api/workflows/channels 
 **/
export async function getWorkflowChannelList(
    options?: { [key: string]: any }
) {
    return request<API.StringListResult>(`/api/workflows/channels`, {
        method: 'GET',
        ...(options || {}),
    });
}
