import type { Edge, Graph, Node } from '@antv/x6';
import React from 'react';

export interface IGraphData {
    nodes: Node.Properties[];
    edges: Edge.Properties[];
}

// export interface INode extends Node.Metadata {
//     nodes: Node.Metadata;
//     edges: Edge.Metadata;
// }

export interface NodeTypeGroup {
    id: string;
    name: string;
    children: Node.Metadata[];
}

export type NodeTypeProperty = {
    name: string;
    type: string;
    uiHint: NodePropertyUiHints;
    label: string;
    hint: string;
    options?: NodeTypePropertyOption;
    category: string;
    order: number;
    defaultValue?: any;
    defaultSyntax?: NodePropertySyntaxNames;
    supportedSyntaxes?: NodePropertySyntaxNames[];
    isReadOnly: boolean;
    isRequired: boolean;
    considerValuesAsOutcomes: boolean;
    defaultWorkflowStorageProvider: string;
    disableWorkflowProviderSelection: boolean;
};

export type NodeTypePropertyOption = {
    isFlagsEnum?: boolean;
    items?: [];
    editorHeight?: string;
    syntax?: string;
    runtimeSelectListProviderType?: string;
    context?: any;
    remoteUri?: string;
};

export type NodeUpdateData = {
    name: string;
    displayName: string;
    description?: string;
    properties: NodeUpdatePropData[];
    outcomes: string[];
    attributes?: Record<string, any>;
    [key: string]: any;
};

export type NodeUpdatePropData = {
    name: string;
    syntax: string | undefined;
    value?: any;
    // valueType?: 'string' | 'number' | 'boolean' | 'object' | 'array';
    expressions?: Record<string, string>;
};

export type NodeStatus = 'default' | 'success' | 'failed' | 'running' | 'inactive';

export type NodeData = {
    id: string;
    name: string;
    displayName: string;
    status: NodeStatus,
    icon?: React.ReactDOM
}

/**
 *  表单数据类型
 */
export type NodeEditFormData = {
    id: string;
    name: string;
    displayName: string;
    description?: string;
    props: NodeEditFormPropData;
    properties: NodeUpdatePropData[];
    propertyStorageProviders: Record<string, any>;
    [key: string]: any;
};

export type NodeEditFormPropItemData = {
    // name: string;
    syntax: string;
    noSyntax?: boolean;
    value?: any;
    expressions?: Record<string, any>;
};

export type NodeEditFormPropData = Record<string, NodeEditFormPropItemData>;

export type EdgeEditFormData = {
    edgeId: string;
    outcomes: string[]
}

export type ToolBarGroupData = {
    group: string;
    items: ToolBarItemData[];
};

export type ToolBarItemData = {
    name: string;
    tooltip: string;
    icon: React.ReactNode;
    disabled?: boolean;
    onClick: (graph: Graph) => void;
};

// export type NodeTypeStyleName =
//     | 'normal'
//     | 'default'
//     | 'success'
//     | 'error'
//     | 'processing'
//     | 'warning';

// export const NodeTypeStyleNames = [
//     'normal',
//     'default',
//     'success',
//     'error',
//     'processing',
//     'warning',
// ];

export type NodePropertySyntax = {
    /** 支持的列表，可以空 */
    supports: string[] | undefined;
    /** 默认 */
    default: string | undefined;
    /** 固定输入 */
    editor: string | undefined;
};

// https://github1s.com/elsa-workflows/elsa-core/blob/HEAD/src/core/Elsa.Abstractions/Design/ActivityInputUIHints.cs
export type NodePropertyUiHints =
    | 'single-line'
    | 'multi-line'
    | 'checkbox'
    | 'check-list'
    | 'radio-list'
    | 'dropdown'
    | 'multi-text'
    | 'code-editor'
    | 'dictionary'
    | 'json'
    // obsolete
    | 'dynamic-list'
    | 'switch-case-builder'
    | 'cron-expression'

export type NodePropertySyntaxNames = 'Literal' | 'Json' | 'JavaScript' | 'Liquid' | 'C#' | 'Variable' | 'SQL' | 'Switch'
