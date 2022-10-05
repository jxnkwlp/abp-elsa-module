/**
 * Generate from swagger json url: https://localhost:44324/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 3
 **/
// @ts-ignore
/* eslint-disable */
import type { API } from "./typings";
import { request } from 'umi';

/**
 * *TODO* GET /api/designer/activity-types 
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
 * *TODO* POST /api/designer/runtime-select-list 
 **/
export async function getDesignerRuntimeSelectListItems(
    payload: API.RuntimeSelectListContext,
    options?: { [key: string]: any }
) {
    return request<API.RuntimeSelectListResult>(`/api/designer/runtime-select-list`, {
        method: 'POST',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/designer/scripting/javascript/type-definitions/{id} 
 **/
export async function getDesignerScriptTypeDefinition(
    id: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/designer/scripting/javascript/type-definitions/${id}`, {
        method: 'GET',
        getResponse: true,
        ...(options || {}),
    });
}
