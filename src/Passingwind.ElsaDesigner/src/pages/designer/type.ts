import type { Node } from '@antv/x6/es';
import type { Cell, Edge, Graph } from '@antv/x6';

export interface IGraphData {
    nodes: Node.Metadata[] | Cell.Properties;
    edges: Edge.Metadata[] | Cell.Properties;
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
    uiHint: string;
    label: string;
    hint: string;
    options?: NodeTypePropertyOption;
    category: string;
    order: number;
    defaultValue?: any;
    defaultSyntax?: string;
    supportedSyntaxes?: string[];
    isReadOnly: boolean;
    isRequired: boolean;
    considerValuesAsOutcomes: boolean;
};

export type NodeTypePropertyOption = {
    isFlagsEnum?: boolean;
    items?: [];
    editorHeight?: string;
    syntax?: string;
};

export type NodeUpdateData = {
    name: string;
    displayName: string;
    description?: string;
    properties: NodeUpdatePropData[];
    outcomes: string[];
    attribtues?: Record<string, any>;
    [key: string]: any;
};

export type NodeUpdatePropData = {
    name: string;
    syntax?: string;
    value?: any;
    valueType?: 'string' | 'number' | 'boolean' | 'object' | 'array';
    expressions?: Record<string, string>;
};

/**
 *  表单数据类型
 */
export type NodeEditFormData = {
    name: string;
    displayName: string;
    description?: string;
    props: NodeEditFormPropData;
    attribtues?: Record<string, any>;
    [key: string]: any;
};

export type NodeEditFormPropItemData = {
    name: string;
    syntax: string;
    value?: any;
    expressions?: Record<string, any>;
    // supportedSyntaxes?: string[];
};

export type NodeEditFormPropData = Record<string, NodeEditFormPropItemData>;

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
