import { WorkflowInstanceStatus } from "@/services/enums";
import { formatMessage, useIntl } from "umi";

export const workflowStatusEnum = {
    [WorkflowInstanceStatus.Idle]: {
        text: formatMessage({ id: 'page.instance.status.idle' }),
        status: 'default',
    },
    [WorkflowInstanceStatus.Running]: {
        text: formatMessage({ id: 'page.instance.status.running' }),
        status: 'processing',
    },
    [WorkflowInstanceStatus.Finished]: {
        text: formatMessage({ id: 'page.instance.status.finished' }),
        status: 'success',
    },
    [WorkflowInstanceStatus.Suspended]: {
        text: formatMessage({ id: 'page.instance.status.suspended' }),
        status: 'warning',
    },
    [WorkflowInstanceStatus.Faulted]: {
        text: formatMessage({ id: 'page.instance.status.faulted' }),
        status: 'error',
    },
    [WorkflowInstanceStatus.Cancelled]: {
        text: formatMessage({ id: 'page.instance.status.cancelled' }),
        status: 'default',
    },
}
