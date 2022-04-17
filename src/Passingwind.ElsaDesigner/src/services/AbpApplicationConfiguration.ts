/**
 * Generate from swagger json url: https://localhost:44324/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 1
 **/
// @ts-ignore
/* eslint-disable */
import { request } from 'umi';

/**
 * *TODO* GET /api/abp/application-configuration
 * 
 **/
export async function getAbpApplicationConfiguration(
    options?: { [key: string]: any }
) {
    return request<API.ApplicationConfiguration>(`/api/abp/application-configuration`, {
        method: 'GET',
        ...(options || {}),
    });
}
