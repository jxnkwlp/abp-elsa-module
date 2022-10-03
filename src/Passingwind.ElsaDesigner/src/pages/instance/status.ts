import { WorkflowStatus } from "@/services/enums";
import { formatMessage, useIntl } from "umi";

export const workflowStatusEnum = {
    [WorkflowStatus.Idle]: {
        text: formatMessage({ id: 'page.instance.status.idle' }),
        status: 'default',
    },
    [WorkflowStatus.Running]: {
        text: formatMessage({ id: 'page.instance.status.running' }),
        status: 'processing',
    },
    [WorkflowStatus.Finished]: {
        text: formatMessage({ id: 'page.instance.status.finished' }),
        status: 'success',
    },
    [WorkflowStatus.Suspended]: {
        text: formatMessage({ id: 'page.instance.status.suspended' }),
        status: 'warning',
    },
    [WorkflowStatus.Faulted]: {
        text: formatMessage({ id: 'page.instance.status.faulted' }),
        status: 'error',
    },
    [WorkflowStatus.Cancelled]: {
        text: formatMessage({ id: 'page.instance.status.cancelled' }),
        status: 'default',
    },
}
