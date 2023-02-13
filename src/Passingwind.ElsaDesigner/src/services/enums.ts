/**
 * Generate from url: https://localhost:44345/swagger/v1/swagger.json
 * It is recommended not to modify the document
 **/
//
// enum types
//
/**
 * *TODO*
 **/
export enum ActivityTraits {
    Action = 1,
    Trigger = 2,
    Job = 4,
}

/**
 * *TODO*
 **/
export enum LoginResultType {
    Success = 1,
    InvalidUserNameOrPassword = 2,
    NotAllowed = 3,
    LockedOut = 4,
    RequiresTwoFactor = 5,
}

/**
 * *TODO*
 **/
export enum LoginResultType {
    Success = 1,
    InvalidUserNameOrPassword = 2,
    NotAllowed = 3,
    LockedOut = 4,
    RequiresTwoFactor = 5,
}

/**
 * *TODO*
 **/
export enum MonacoCodeAnalysisSeverity {
    Unkown = 0,
    Hint = 1,
    Info = 2,
    Warning = 4,
    Error = 8,
}

/**
 * *TODO*
 **/
export enum MonacoCompletionItemKind {
    Function = 0,
    Class = 1,
    Field = 2,
    Variable = 3,
    Property = 4,
    Enum = 5,
    Others = 6,
}

/**
 * *TODO*
 **/
export enum WorkflowContextFidelity {
    Burst = 0,
    Activity = 1,
}

/**
 * *TODO*
 **/
export enum WorkflowInstanceStatus {
    Idle = 0,
    Running = 1,
    Finished = 2,
    Suspended = 3,
    Faulted = 4,
    Cancelled = 5,
}

/**
 * *TODO*
 **/
export enum WorkflowPersistenceBehavior {
    Suspended = 0,
    WorkflowBurst = 1,
    ActivityExecuted = 2,
}

