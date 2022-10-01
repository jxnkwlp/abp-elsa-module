import { WorkflowStatus } from "@/services/enums";

export const workflowStatusEnum = {
    [WorkflowStatus.Idle]: {
        text: WorkflowStatus[WorkflowStatus.Idle],
        status: 'default',
    },
    [WorkflowStatus.Running]: {
        text: WorkflowStatus[WorkflowStatus.Running],
        status: 'processing',
    },
    [WorkflowStatus.Finished]: {
        text: WorkflowStatus[WorkflowStatus.Finished],
        status: 'success',
    },
    [WorkflowStatus.Suspended]: {
        text: WorkflowStatus[WorkflowStatus.Suspended],
        status: 'warning',
    },
    [WorkflowStatus.Faulted]: {
        text: WorkflowStatus[WorkflowStatus.Faulted],
        status: 'error',
    },
    [WorkflowStatus.Cancelled]: {
        text: WorkflowStatus[WorkflowStatus.Cancelled],
        status: 'default',
    },
}
