/**
 * Generate from url: https://localhost:44345/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 12
 **/
// @ts-ignore
/* eslint-disable */
import type { API } from "./typings";
import { request } from 'umi';

/**
 * *TODO* POST /api/elsa/workflow/teams 
 **/
export async function createWorkflowTeam(
    payload: API.WorkflowTeamCreateOrUpdate,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowTeam>(`/api/elsa/workflow/teams`, {
        method: 'POST',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* DELETE /api/elsa/workflow/teams/{id} 
 **/
export async function deleteWorkflowTeam(
    id: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/workflow/teams/${id}`, {
        method: 'DELETE',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* DELETE /api/elsa/workflow/teams/{id}/role-scopes/{roleName} 
 **/
export async function deleteWorkflowTeamRoleScope(
    id: string,    roleName: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/workflow/teams/${id}/role-scopes/${roleName}`, {
        method: 'DELETE',
        getResponse: true,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/teams/{id} 
 **/
export async function getWorkflowTeam(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowTeam>(`/api/elsa/workflow/teams/${id}`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/teams/assignable-definitions 
 **/
export async function getWorkflowTeamAssignableDefinition(
    params: {
        filter?: string | undefined,
        isSingleton?: boolean | undefined,
        deleteCompletedInstances?: boolean | undefined,
        channel?: string | undefined,
        tag?: string | undefined,
        groupId?: string | undefined,
        persistenceBehavior?: any | undefined,
        sorting?: string | undefined,
        skipCount?: number | undefined,
        maxResultCount?: number | undefined
    },
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDefinitionBasicListResult>(`/api/elsa/workflow/teams/assignable-definitions`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/teams/assignable-roles 
 **/
export async function getWorkflowTeamAssignableRoles(
    options?: { [key: string]: any }
) {
    return request<API.IdentityRoleListResult>(`/api/elsa/workflow/teams/assignable-roles`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/teams/assignable-users 
 **/
export async function getWorkflowTeamAssignableUsers(
    params: {
        filter?: string | undefined,
        roleId?: string | undefined,
        skipCount?: number | undefined,
        maxResultCount?: number | undefined
    },
    options?: { [key: string]: any }
) {
    return request<API.IdentityUserListResult>(`/api/elsa/workflow/teams/assignable-users`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/teams 
 **/
export async function getWorkflowTeamList(
    params: {
        skipCount?: number | undefined,
        maxResultCount?: number | undefined
    },
    options?: { [key: string]: any }
) {
    return request<API.WorkflowTeamBasicPagedResult>(`/api/elsa/workflow/teams`, {
        method: 'GET',
        params: params,
        ...(options || {}),
    });
}

/**
 * *TODO* GET /api/elsa/workflow/teams/{id}/role-scopes 
 **/
export async function getWorkflowTeamRoleScopes(
    id: string,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowTeamRoleScopeListResult>(`/api/elsa/workflow/teams/${id}/role-scopes`, {
        method: 'GET',
        ...(options || {}),
    });
}

/**
 * *TODO* PUT /api/elsa/workflow/teams/{id} 
 **/
export async function updateWorkflowTeam(
    id: string,
    payload: API.WorkflowTeamCreateOrUpdate,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowTeam>(`/api/elsa/workflow/teams/${id}`, {
        method: 'PUT',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* PUT /api/elsa/workflow/teams/{id}/role-scopes 
 **/
export async function workflowTeamSetRoleScope(
    id: string,
    payload: API.WorkflowTeamRoleScopeCreateOrUpdate,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowTeamRoleScopeListResult>(`/api/elsa/workflow/teams/${id}/role-scopes`, {
        method: 'PUT',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* PUT /api/elsa/workflow/teams/{id}/users 
 **/
export async function workflowTeamSetUsers(
    id: string,
    payload: API.WorkflowTeamUserUpdateRequest,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/workflow/teams/${id}/users`, {
        method: 'PUT',
        data: payload,
        getResponse: true,
        ...(options || {}),
    });
}
