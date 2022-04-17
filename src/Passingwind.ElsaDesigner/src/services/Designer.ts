/**
 * Generate from swagger json url: https://localhost:44324/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 2
 **/
// @ts-ignore
/* eslint-disable */
import { request } from 'umi';

/**
 * *TODO* GET /api/designer/activity-types
 * 
 **/
export async function getDesignerActivityTypes(
    options?: { [key: string]: any }
) {
    return request<API.ActivityTypeDescriptorListResult>(`/api/designer/activity-types`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/designer/script-type-definitions/{id}
 * 
 **/
export async function getDesignerScriptTypeDefinition(
    id: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/designer/script-type-definitions/${id}`, {
        method: 'GET',
        getResponse: true,
        ...(options || {}),
    });
}
