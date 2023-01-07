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
 * *TODO* GET /signals/dispatch/{token} 
 **/
export async function dispatchEndpointHandleGET(
    token: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/signals/dispatch/${token}`, {
        method: 'GET',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /signals/dispatch/{token} 
 **/
export async function dispatchEndpointHandlePOST(
    token: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/signals/dispatch/${token}`, {
        method: 'POST',
        getResponse: true,
        ...(options || {}),
    });
}
