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
 * *TODO* GET /test1 
 **/
export async function test1(
    options?: { [key: string]: any }
) {
    return request<any>(`/test1`, {
        method: 'GET',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /test3 
 **/
export async function test3(
    options?: { [key: string]: any }
) {
    return request<any>(`/test3`, {
        method: 'GET',
        getResponse: true,
        ...(options || {}),
    });
}
