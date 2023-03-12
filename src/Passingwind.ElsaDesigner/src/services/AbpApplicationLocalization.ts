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
 * *TODO* GET /api/abp/application-localization 
 **/
export async function getAbpApplicationLocalization(
    params: {
        cultureName: string,
        onlyDynamics?: boolean | undefined
    },
    options?: { [key: string]: any }
) {
    return request<API.ApplicationLocalization>(`/api/abp/application-localization`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}
