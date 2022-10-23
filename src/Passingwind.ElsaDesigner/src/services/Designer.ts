/**
 * Generate from swagger json url: https://localhost:44315/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 3
 **/
// @ts-ignore
/* eslint-disable */
import type { API } from "./typings";
import { request } from 'umi';

/**
 * *TODO* GET /api/elsa/designer/activity-types 
 **/
export async function getDesignerActivityTypes(
    options?: { [key: string]: any }
) {
    return request<API.ActivityTypeDescriptorListResult>(`/api/elsa/designer/activity-types`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/elsa/designer/runtime-select-list 
 **/
export async function getDesignerRuntimeSelectListItems(
    payload: API.RuntimeSelectListContext,
    options?: { [key: string]: any }
) {
    return request<API.RuntimeSelectListResult>(`/api/elsa/designer/runtime-select-list`, {
        method: 'POST',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/designer/scripting/javascript/type-definitions/{id} 
 **/
export async function getDesignerScriptTypeDefinition(
    id: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/designer/scripting/javascript/type-definitions/${id}`, {
        method: 'GET',
        getResponse: true,
        ...(options || {}),
    });
}
