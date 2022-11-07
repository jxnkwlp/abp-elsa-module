/**
 * Generate from swagger json url: https://localhost:44315/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 14
 **/
// @ts-ignore
/* eslint-disable */
import type { API } from "./typings";
import { request } from 'umi';

/**
 * *TODO* GET /100 
 **/
export async function 100(
    options?: { [key: string]: any }
) {
    return request<any>(`/100`, {
        method: 'GET',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /101 
 **/
export async function 101(
    options?: { [key: string]: any }
) {
    return request<any>(`/101`, {
        method: 'GET',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /1018 
 **/
export async function 1018(
    options?: { [key: string]: any }
) {
    return request<any>(`/1018`, {
        method: 'GET',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /300 
 **/
export async function 300(
    options?: { [key: string]: any }
) {
    return request<any>(`/300`, {
        method: 'GET',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /fork 
 **/
export async function fork(
    options?: { [key: string]: any }
) {
    return request<any>(`/fork`, {
        method: 'GET',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* DELETE /js-test 
 **/
export async function js-testDELETE(
    options?: { [key: string]: any }
) {
    return request<any>(`/js-test`, {
        method: 'DELETE',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /js-test 
 **/
export async function js-testGET(
    options?: { [key: string]: any }
) {
    return request<any>(`/js-test`, {
        method: 'GET',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* DELETE /test2 
 **/
export async function test2DELETE(
    options?: { [key: string]: any }
) {
    return request<any>(`/test2`, {
        method: 'DELETE',
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
 * *TODO* PATCH /test2 
 **/
export async function test2PATCH(
    options?: { [key: string]: any }
) {
    return request<any>(`/test2`, {
        method: 'PATCH',
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
 * *TODO* GET /test5 
 **/
export async function test5GET(
    options?: { [key: string]: any }
) {
    return request<any>(`/test5`, {
        method: 'GET',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /test5 
 **/
export async function test5POST(
    options?: { [key: string]: any }
) {
    return request<any>(`/test5`, {
        method: 'POST',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /test6 
 **/
export async function test6(
    options?: { [key: string]: any }
) {
    return request<any>(`/test6`, {
        method: 'GET',
        getResponse: true,
        ...(options || {}),
    });
}
