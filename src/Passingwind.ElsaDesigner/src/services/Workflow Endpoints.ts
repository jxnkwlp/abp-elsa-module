/**
 * Generate from url: https://localhost:44345/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 5
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
 * *TODO* GET /test2 
 **/
export async function test2GET(
    options?: { [key: string]: any }
) {
    return request<any>(`/test2`, {
        method: 'GET',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /test2 
 **/
export async function test2POST(
    options?: { [key: string]: any }
) {
    return request<any>(`/test2`, {
        method: 'POST',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /test3 
 **/
export async function test3GET(
    options?: { [key: string]: any }
) {
    return request<any>(`/test3`, {
        method: 'GET',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /test3 
 **/
export async function test3POST(
    options?: { [key: string]: any }
) {
    return request<any>(`/test3`, {
        method: 'POST',
        getResponse: true,
        ...(options || {}),
    });
}
