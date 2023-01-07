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
 * *TODO* POST /api/_monaco/csharp/hoverinfo 
 **/
export async function monacoCsharpCompletionHoverInfo(
    payload: API.HoverInfoRequest,
    options?: { [key: string]: any }
) {
    return request<API.HoverInfoResult>(`/api/_monaco/csharp/hoverinfo`, {
        method: 'POST',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/_monaco/csharp/tabcompletion 
 **/
export async function monacoCsharpCompletionTabCompletion(
    payload: API.TabCompletionRequest,
    options?: { [key: string]: any }
) {
    return request<API.TabCompletionResult>(`/api/_monaco/csharp/tabcompletion`, {
        method: 'POST',
        data: payload,
        ...(options || {}),
    });
}
