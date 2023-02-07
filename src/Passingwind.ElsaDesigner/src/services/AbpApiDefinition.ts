/**
 * Generate from url: https://localhost:44315/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 1
 **/
// @ts-ignore
/* eslint-disable */
import type { API } from "./typings";
import { request } from 'umi';

/**
 * *TODO* GET /api/abp/api-definition 
 **/
export async function getAbpApiDefinition(
    params: {
        includeTypes?: boolean | undefined
    },
    options?: { [key: string]: any }
) {
    return request<API.ApplicationApiDescriptionModel>(`/api/abp/api-definition`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}
