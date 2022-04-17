/**
 * Generate from swagger json url: https://localhost:44324/swagger/v1/swagger.json 
 **/
/**
 * enum types
* */
/**
 *  *TODO*
 **/
export enum ActivityTraits {
    Action = 1,
    Trigger = 2,
    Job = 4,
}

/**
 *  *TODO*
 **/
export enum CallingConventions {
    Standard = 1,
    VarArgs = 2,
    Any = 3,
    HasThis = 32,
    ExplicitThis = 64,
}

/**
 *  *TODO*
 **/
export enum EventAttributes {
    None = 0,
    SpecialName = 512,
    RTSpecialName = 1024,
}

/**
 *  *TODO*
 **/
export enum FieldAttributes {
    PrivateScope = 0,
    Private = 1,
    FamANDAssem = 2,
    Assembly = 3,
    Family = 4,
    FamORAssem = 5,
    Public = 6,
    FieldAccessMask = 7,
    Static = 16,
    InitOnly = 32,
    Literal = 64,
    NotSerialized = 128,
    HasFieldRVA = 256,
    SpecialName = 512,
    RTSpecialName = 1024,
    HasFieldMarshal = 4096,
    PinvokeImpl = 8192,
    HasDefault = 32768,
    ReservedMask = 38144,
}

/**
 *  *TODO*
 **/
export enum GenericParameterAttributes {
    None = 0,
    Covariant = 1,
    Contravariant = 2,
    VarianceMask = 3,
    ReferenceTypeConstraint = 4,
    NotNullableValueTypeConstraint = 8,
    DefaultConstructorConstraint = 16,
    SpecialConstraintMask = 28,
}

/**
 *  *TODO*
 **/
export enum LayoutKind {
    Sequential = 0,
    Explicit = 2,
    Auto = 3,
}

/**
 *  *TODO*
 **/
export enum MemberTypes {
    Constructor = 1,
    Event = 2,
    Field = 4,
    Method = 8,
    Property = 16,
    TypeInfo = 32,
    Custom = 64,
    NestedType = 128,
    All = 191,
}

/**
 *  *TODO*
 **/
export enum MethodAttributes {
    ReuseSlot = 0,
    PrivateScope = 1,
    Private = 2,
    FamANDAssem = 3,
    Assembly = 4,
    Family = 5,
    FamORAssem = 6,
    Public = 7,
    MemberAccessMask = 8,
    UnmanagedExport = 16,
    Static = 32,
    Final = 64,
    Virtual = 128,
    HideBySig = 256,
    NewSlot = 512,
    VtableLayoutMask = 1024,
    CheckAccessOnOverride = 2048,
    Abstract = 4096,
    SpecialName = 8192,
    RTSpecialName = 16384,
    PinvokeImpl = 32768,
    HasSecurity = 53248,
}

/**
 *  *TODO*
 **/
export enum MethodImplAttributes {
    IL = 0,
    Managed = 1,
    Native = 2,
    OPTIL = 3,
    Runtime = 4,
    CodeTypeMask = 8,
    Unmanaged = 16,
    ManagedMask = 32,
    NoInlining = 64,
    ForwardRef = 128,
    Synchronized = 256,
    NoOptimization = 512,
    PreserveSig = 4096,
    AggressiveInlining = 65535,
}

/**
 *  *TODO*
 **/
export enum ParameterAttributes {
    None = 0,
    In = 1,
    Out = 2,
    Lcid = 4,
    Retval = 8,
    Optional = 16,
    HasDefault = 4096,
    HasFieldMarshal = 8192,
    Reserved3 = 16384,
    Reserved4 = 32768,
    ReservedMask = 61440,
}

/**
 *  *TODO*
 **/
export enum PropertyAttributes {
    None = 0,
    SpecialName = 512,
    RTSpecialName = 1024,
    HasDefault = 4096,
    Reserved2 = 8192,
    Reserved3 = 16384,
    Reserved4 = 32768,
    ReservedMask = 62464,
}

/**
 *  *TODO*
 **/
export enum SecurityRuleSet {
    None = 0,
    Level1 = 1,
    Level2 = 2,
}

/**
 *  *TODO*
 **/
export enum TypeAttributes {
    NotPublic = 0,
    AutoLayout = 1,
    AnsiClass = 2,
    Class = 3,
    Public = 4,
    NestedPublic = 5,
    NestedPrivate = 6,
    NestedFamily = 7,
    NestedAssembly = 8,
    NestedFamANDAssem = 16,
    NestedFamORAssem = 24,
    VisibilityMask = 32,
    SequentialLayout = 128,
    ExplicitLayout = 256,
    LayoutMask = 1024,
    Interface = 2048,
    ClassSemanticsMask = 4096,
    Abstract = 8192,
    Sealed = 16384,
    SpecialName = 65536,
    RTSpecialName = 131072,
    Import = 196608,
    Serializable = 262144,
    WindowsRuntime = 264192,
    UnicodeClass = 1048576,
    AutoClass = 12582912,
}

/**
 *  *TODO*
 **/
export enum WorkflowContextFidelity {
    Burst = 0,
    Activity = 1,
}

/**
 *  *TODO*
 **/
export enum WorkflowPersistenceBehavior {
    Suspended = 0,
    WorkflowBurst = 1,
    ActivityExecuted = 2,
}

/**
 *  *TODO*
 **/
export enum WorkflowStatus {
    Idle = 0,
    Running = 1,
    Finished = 2,
    Suspended = 3,
    Faulted = 4,
    Cancelled = 5,
}

