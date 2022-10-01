import type { API } from '@/services/typings';

export type WorkflowInstanceExecutionLogSummaryActivity2 = {
    name?: string,
    displayName?: string,
}

export type WorkflowInstanceActivitySummaryInfo = WorkflowInstanceExecutionLogSummaryActivity2 & API.WorkflowInstanceExecutionLogSummaryActivity;

// export default WorkflowInstanceEdge = {
//     id: string,
//     status: string,
//     status: 'success' | 'failed'
// }

// export type WorkflowInstanceActivityStatusData = {
//     id: string,
//     name?: string,
//     displayName?: string,
//     type?: string,
//     outcomes?: string[]
//     inbound?: any
//     data?: any
// }
