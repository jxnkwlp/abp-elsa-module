/**
 * Generate from url: https://localhost:44345/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 24
 **/
// @ts-ignore
/* eslint-disable */
import type { API } from "./typings";
import { request } from 'umi';

/**
 * *TODO* POST /api/elsa/workflow/definitions 
 **/
export async function createWorkflowDefinition(
    payload: API.WorkflowDefinitionVersionCreateOrUpdate,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDefinitionVersion>(`/api/elsa/workflow/definitions`, {
        method: 'POST',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* DELETE /api/elsa/workflow/definitions/{id} 
 **/
export async function deleteWorkflowDefinition(
    id: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/workflow/definitions/${id}`, {
        method: 'DELETE',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* DELETE /api/elsa/workflow/definitions/{id}/iam/owners/{userId} 
 **/
export async function deleteWorkflowDefinitionOwner(
    id: string,    userId: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/workflow/definitions/${id}/iam/owners/${userId}`, {
        method: 'DELETE',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* DELETE /api/elsa/workflow/definitions/{id}/versions/{version} 
 **/
export async function deleteWorkflowDefinitionVersion(
    id: string,    version: number,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/workflow/definitions/${id}/versions/${version}`, {
        method: 'DELETE',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/definitions/{id} 
 **/
export async function getWorkflowDefinition(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDefinitionVersion>(`/api/elsa/workflow/definitions/${id}`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/definitions/{id}/definition 
 **/
export async function getWorkflowDefinitionDefinition(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDefinition>(`/api/elsa/workflow/definitions/${id}/definition`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/definitions/{id}/iam 
 **/
export async function getWorkflowDefinitionIam(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDefinitionIamResult>(`/api/elsa/workflow/definitions/${id}/iam`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/definitions 
 **/
export async function getWorkflowDefinitionList(
    params: {
        filter?: string | undefined,
        isSingleton?: boolean | undefined,
        sorting?: string | undefined,
        skipCount?: number | undefined,
        maxResultCount?: number | undefined
    },
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDefinitionBasicPagedResult>(`/api/elsa/workflow/definitions`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/definitions/{id}/versions/{version}/previous-version 
 **/
export async function getWorkflowDefinitionPreviousVersion(
    id: string,    version: number,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDefinitionVersion>(`/api/elsa/workflow/definitions/${id}/versions/${version}/previous-version`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/definitions/{id}/variables 
 **/
export async function getWorkflowDefinitionVariables(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowVariables>(`/api/elsa/workflow/definitions/${id}/variables`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/definitions/{id}/versions/{version} 
 **/
export async function getWorkflowDefinitionVersion(
    id: string,    version: number,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDefinitionVersion>(`/api/elsa/workflow/definitions/${id}/versions/${version}`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/definitions/{id}/versions 
 **/
export async function getWorkflowDefinitionVersions(
    id: string,
    params: {
        skipCount?: number | undefined,
        maxResultCount?: number | undefined
    },
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDefinitionVersionListItemPagedResult>(`/api/elsa/workflow/definitions/${id}/versions`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO* PUT /api/elsa/workflow/definitions/{id} 
 **/
export async function updateWorkflowDefinition(
    id: string,
    payload: API.WorkflowDefinitionVersionCreateOrUpdate,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDefinitionVersion>(`/api/elsa/workflow/definitions/${id}`, {
        method: 'PUT',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* PUT /api/elsa/workflow/definitions/{id}/definition 
 **/
export async function updateWorkflowDefinitionDefinition(
    id: string,
    payload: API.WorkflowDefinitionCreateOrUpdate,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDefinition>(`/api/elsa/workflow/definitions/${id}/definition`, {
        method: 'PUT',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* PUT /api/elsa/workflow/definitions/{id}/variables 
 **/
export async function updateWorkflowDefinitionVariables(
    id: string,
    payload: API.WorkflowVariableUpdate,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowVariables>(`/api/elsa/workflow/definitions/${id}/variables`, {
        method: 'PUT',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/elsa/workflow/definitions/{id}/iam/owners 
 **/
export async function workflowDefinitionAddOwner(
    id: string,
    payload: API.WorkflowDefinitionAddOwnerRequest,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/workflow/definitions/${id}/iam/owners`, {
        method: 'POST',
        data: payload,
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/elsa/workflow/definitions/{id}/dispatch 
 **/
export async function workflowDefinitionDispatch(
    id: string,
    payload: API.WorkflowDefinitionDispatchRequest,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDefinitionDispatchResult>(`/api/elsa/workflow/definitions/${id}/dispatch`, {
        method: 'POST',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/elsa/workflow/definitions/{id}/execute 
 **/
export async function workflowDefinitionExecute(
    id: string,
    payload: API.WorkflowDefinitionExecuteRequest,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/workflow/definitions/${id}/execute`, {
        method: 'POST',
        data: payload,
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/elsa/workflow/definitions/export 
 **/
export async function workflowDefinitionExport2(
    payload: API.WorkflowDefinitionExportRequest,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/workflow/definitions/export`, {
        method: 'POST',
        data: payload,
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/elsa/workflow/definitions/import 
 **/
export async function workflowDefinitionImport(
    payload: API.WorkflowDefinitionImportRequest,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDefinitionImportResult>(`/api/elsa/workflow/definitions/import`, {
        method: 'POST',
        requestType: 'form',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* PUT /api/elsa/workflow/definitions/{id}/publish 
 **/
export async function workflowDefinitionPublish(
    id: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/workflow/definitions/${id}/publish`, {
        method: 'PUT',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/elsa/workflow/definitions/{id}/retract 
 **/
export async function workflowDefinitionRetract(
    id: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/workflow/definitions/${id}/retract`, {
        method: 'POST',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/elsa/workflow/definitions/{id}/revert 
 **/
export async function workflowDefinitionRevert(
    id: string,
    payload: API.WorkflowDefinitionRevertRequest,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/workflow/definitions/${id}/revert`, {
        method: 'POST',
        data: payload,
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* PUT /api/elsa/workflow/definitions/{id}/unpublish 
 **/
export async function workflowDefinitionUnPublish(
    id: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/workflow/definitions/${id}/unpublish`, {
        method: 'PUT',
        getResponse: true,
        ...(options || {}),
    });
}
