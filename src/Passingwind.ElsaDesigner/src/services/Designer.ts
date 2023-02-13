/**
 * Generate from url: https://localhost:44345/swagger/v1/swagger.json
 * It is recommended not to modify the document
 * Total count: 8
 **/
// @ts-ignore
/* eslint-disable */
import type { API } from "./typings";
import { request } from 'umi';

/**
 * *TODO* POST /api/elsa/designer/scripting/csharp/analysis/{id} 
 **/
export async function designerCSharpLanguageCodeAnalysis(
    id: string,
    payload: API.WorkflowDesignerCSharpLanguageAnalysisRequest,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDesignerCSharpLanguageAnalysisResult>(`/api/elsa/designer/scripting/csharp/analysis/${id}`, {
        method: 'POST',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/elsa/designer/scripting/csharp/format 
 **/
export async function designerCSharpLanguageCodeFormatter(
    payload: API.WorkflowDesignerCSharpLanguageFormatterRequest,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDesignerCSharpLanguageFormatterResult>(`/api/elsa/designer/scripting/csharp/format`, {
        method: 'POST',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/elsa/designer/scripting/csharp/completions/{id} 
 **/
export async function designerCSharpLanguageCompletionProvider(
    id: string,
    payload: API.WorkflowDesignerCSharpLanguageCompletionProviderRequest,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDesignerCSharpLanguageCompletionProviderResult>(`/api/elsa/designer/scripting/csharp/completions/${id}`, {
        method: 'POST',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/elsa/designer/scripting/csharp/hovers/{id} 
 **/
export async function designerCSharpLanguageHoverProvider(
    id: string,
    payload: API.WorkflowDesignerCSharpLanguageHoverProviderRequest,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDesignerCSharpLanguageHoverProviderResult>(`/api/elsa/designer/scripting/csharp/hovers/${id}`, {
        method: 'POST',
        data: payload,
        ...(options || {}),
    });
}

/**
 * *TODO* POST /api/elsa/designer/scripting/csharp/signatures/{id} 
 **/
export async function designerCSharpLanguageSignatureProvider(
    id: string,
    payload: API.WorkflowDesignerCSharpLanguageSignatureProviderRequest,
    options?: { [key: string]: any }
) {
    return request<API.WorkflowDesignerCSharpLanguageSignatureProviderResult>(`/api/elsa/designer/scripting/csharp/signatures/${id}`, {
        method: 'POST',
        data: payload,
        ...(options || {}),
    });
}

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
 * *TODO* GET /api/elsa/designer/scripting/javascript/type-definitions/{id} 
 **/
export async function getDesignerJavaScriptTypeDefinition(
    id: string,
    options?: { [key: string]: any }
) {
    return request<any>(`/api/elsa/designer/scripting/javascript/type-definitions/${id}`, {
        method: 'GET',
        getResponse: true,
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
