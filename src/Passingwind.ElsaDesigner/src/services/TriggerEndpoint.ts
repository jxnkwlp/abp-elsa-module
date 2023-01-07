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
 * *TODO* GET /signals/trigger/{token} 
 **/
export async function triggerEndpointHandleGET(
    token: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/signals/trigger/${token}`, {
        method: 'GET',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /signals/trigger/{token} 
 **/
export async function triggerEndpointHandlePOST(
    token: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/signals/trigger/${token}`, {
        method: 'POST',
        getResponse: true,
        ...(options || {}),
    });
}
