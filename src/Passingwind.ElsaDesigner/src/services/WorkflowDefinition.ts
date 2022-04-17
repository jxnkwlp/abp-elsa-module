/**
 * Generate from swagger json url: https://localhost:44324/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 12
 **/
// @ts-ignore
/* eslint-disable */
import { request } from 'umi';

/**
 * *TODO* GET /api/workflow-definitions/{id}
 * 
 **/
export async function getWorkflowDefinition(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDefinitionVersion>(`/api/workflow-definitions/${id}`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* PUT /api/workflow-definitions/{id}
 * 
 **/
export async function updateWorkflowDefinition(
    id: string,
    payload: API.WorkflowDefinitionVersionCreateOrUpdate,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDefinitionVersion>(`/api/workflow-definitions/${id}`, {
        method: 'PUT',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* DELETE /api/workflow-definitions/{id}
 * 
 **/
export async function deleteWorkflowDefinition(
    id: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/workflow-definitions/${id}`, {
        method: 'DELETE',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/workflow-definitions
 * 
 **/
export async function getWorkflowDefinitionList(
    params: {
        filter?: string | undefined,
        skipCount?: number | undefined,
        maxResultCount?: number | undefined
    },
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDefinitionPagedResult>(`/api/workflow-definitions`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/workflow-definitions
 * 
 **/
export async function createWorkflowDefinition(
    payload: API.WorkflowDefinitionVersionCreateOrUpdate,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDefinitionVersion>(`/api/workflow-definitions`, {
        method: 'POST',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* DELETE /api/workflow-definitions/{id}/versions/{version}
 * 
 **/
export async function deleteWorkflowDefinitionVersion(
    id: string,    version: number,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/workflow-definitions/${id}/versions/${version}`, {
        method: 'DELETE',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/workflow-definitions/{id}/versions/{version}
 * 
 **/
export async function getWorkflowDefinitionVersion(
    id: string,    version: number,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDefinitionVersion>(`/api/workflow-definitions/${id}/versions/${version}`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/workflow-definitions/{id}/definition
 * 
 **/
export async function getWorkflowDefinitionDefinition(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDefinition>(`/api/workflow-definitions/${id}/definition`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* PUT /api/workflow-definitions/{id}/definition
 * 
 **/
export async function updateWorkflowDefinitionDefinition(
    id: string,
    payload: API.WorkflowDefinitionCreateOrUpdate,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDefinition>(`/api/workflow-definitions/${id}/definition`, {
        method: 'PUT',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/workflow-definitions/{id}/versions
 * 
 **/
export async function getWorkflowDefinitionVersions(
    id: string,
    params: {
        skipCount?: number | undefined,
        maxResultCount?: number | undefined
    },
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDefinitionVersionListItemPagedResult>(`/api/workflow-definitions/${id}/versions`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO* PUT /api/workflow-definitions/{id}/unpublish
 * 
 **/
export async function workflowDefinitionUnPublish(
    id: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/workflow-definitions/${id}/unpublish`, {
        method: 'PUT',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* PUT /api/workflow-definitions/{id}/publish
 * 
 **/
export async function workflowDefinitionPublish(
    id: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/workflow-definitions/${id}/publish`, {
        method: 'PUT',
        getResponse: true,
        ...(options || {}),
    });
}
