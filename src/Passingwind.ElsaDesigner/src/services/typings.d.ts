/**
 * Generate from swagger json url: https://localhost:44315/swagger/v1/swagger.json
 * Total count: 160
 **/
import * as Enum from "./enums";

declare namespace API {
    /**
     * *TODO*
     **/
    type SelectList = {
        isFlagsEnum?: boolean | undefined;
        items?: SelectListItem[] | undefined;
    };

    /**
     * *TODO*
     **/
    type SelectListItem = {
        text?: string | undefined;
        value?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type ActivityInputDescriptor = {
        name?: string | undefined;
        type?: any | undefined;
        uiHint?: string | undefined;
        label?: string | undefined;
        hint?: string | undefined;
        options?: any | undefined;
        category?: string | undefined;
        order?: number | undefined;
        defaultValue?: any | undefined;
        defaultSyntax?: string | undefined;
        supportedSyntaxes?: string[] | undefined;
        isReadOnly?: boolean | undefined;
        isBrowsable?: boolean | undefined;
        isDesignerCritical?: boolean | undefined;
        defaultWorkflowStorageProvider?: string | undefined;
        disableWorkflowProviderSelection?: boolean | undefined;
        considerValuesAsOutcomes?: boolean | undefined;
    };

    /**
     * *TODO*
     **/
    type ActivityOutputDescriptor = {
        name?: string | undefined;
        type?: any | undefined;
        hint?: string | undefined;
        isBrowsable?: boolean | undefined;
        defaultWorkflowStorageProvider?: string | undefined;
        disableWorkflowProviderSelection?: boolean | undefined;
    };

    /**
     * *TODO*
     **/
    type ActivityDefinitionProperty = {
        name?: string | undefined;
        syntax?: string | undefined;
        expressions?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type SimpleException = {
        type?: any | undefined;
        message?: string | undefined;
        stackTrace?: string | undefined;
        innerException?: SimpleException | undefined;
        data?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowContextOptions = {
        contextType?: any | undefined;
        contextFidelity?: Enum.WorkflowContextFidelity | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowInput = {
        input?: any | undefined;
        storageProviderName?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowInputReference = {
        providerName?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowOutputReference = {
        providerName?: string | undefined;
        activityId?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowExecutionLog = {
        id?: string | undefined;
        creationTime?: string | undefined;
        creatorId?: string | undefined;
        workflowInstanceId?: string | undefined;
        activityId?: string | undefined;
        activityType?: string | undefined;
        timestamp?: string | undefined;
        eventName?: string | undefined;
        message?: string | undefined;
        source?: string | undefined;
        data?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowInstanceFault = {
        id?: string | undefined;
        faultedActivityId?: string | undefined;
        resuming?: boolean | undefined;
        activityInput?: any | undefined;
        message?: string | undefined;
        exception?: SimpleException | undefined;
    };

    /**
     * *TODO*
     **/
    type GlobalVariable = {
        id?: string | undefined;
        creationTime?: string | undefined;
        creatorId?: string | undefined;
        lastModificationTime?: string | undefined;
        lastModifierId?: string | undefined;
        key?: string | undefined;
        value?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type GlobalVariableCreateOrUpdate = {
        key: string;
        value?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type Activity = {
        activityId?: string | undefined;
        type?: string | undefined;
        name?: string | undefined;
        displayName?: string | undefined;
        description?: string | undefined;
        persistWorkflow?: boolean | undefined;
        loadWorkflowContext?: boolean | undefined;
        saveWorkflowContext?: boolean | undefined;
        attributes?: any | undefined;
        properties?: ActivityDefinitionProperty[] | undefined;
        propertyStorageProviders?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type ActivityConnection = {
        sourceId?: string | undefined;
        targetId?: string | undefined;
        outcome?: string | undefined;
        attributes?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type ActivityConnectionCreate = {
        sourceId?: string | undefined;
        targetId?: string | undefined;
        outcome?: string | undefined;
        attributes?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type ActivityCreateOrUpdate = {
        activityId?: string | undefined;
        type: string;
        name: string;
        displayName?: string | undefined;
        description?: string | undefined;
        persistWorkflow?: boolean | undefined;
        loadWorkflowContext?: boolean | undefined;
        saveWorkflowContext?: boolean | undefined;
        attributes?: any | undefined;
        properties?: ActivityDefinitionProperty[] | undefined;
        propertyStorageProviders?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type ActivityTypeDescriptor = {
        type?: string | undefined;
        displayName?: string | undefined;
        description?: string | undefined;
        category?: string | undefined;
        traits?: Enum.ActivityTraits | undefined;
        outcomes?: string[] | undefined;
        inputProperties?: ActivityInputDescriptor[] | undefined;
        outputProperties?: ActivityOutputDescriptor[] | undefined;
        customAttributes?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type ActivityTypeDescriptorListResult = {
        items?: ActivityTypeDescriptor[] | undefined;
        categories?: string[] | undefined;
    };

    /**
     * *TODO*
     **/
    type RuntimeSelectListContext = {
        providerTypeName?: string | undefined;
        context?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type RuntimeSelectListResult = {
        selectList?: SelectList | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowProviderDescriptor = {
        name?: string | undefined;
        type?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowSignalDispatchRequest = {
        workflowInstanceId?: string | undefined;
        correlationId?: string | undefined;
        input?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowSignalDispatchResult = {
        startedWorkflows?: WorkflowSignalResult[] | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowSignalExecuteRequest = {
        workflowInstanceId?: string | undefined;
        correlationId?: string | undefined;
        input?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowSignalExecuteResult = {
        startedWorkflows?: WorkflowSignalResult[] | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowSignalResult = {
        workflowInstanceId?: string | undefined;
        activityId?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowStorageProviderInfo = {
        name?: string | undefined;
        displayName?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowDefinition = {
        id?: string | undefined;
        creationTime?: string | undefined;
        creatorId?: string | undefined;
        lastModificationTime?: string | undefined;
        lastModifierId?: string | undefined;
        name?: string | undefined;
        displayName?: string | undefined;
        tenantId?: string | undefined;
        description?: string | undefined;
        latestVersion?: number | undefined;
        publishedVersion?: number | undefined;
        isSingleton?: boolean | undefined;
        deleteCompletedInstances?: boolean | undefined;
        channel?: string | undefined;
        tag?: string | undefined;
        persistenceBehavior?: Enum.WorkflowPersistenceBehavior | undefined;
        contextOptions?: WorkflowContextOptions | undefined;
        variables?: any | undefined;
        customAttributes?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowDefinitionCreateOrUpdate = {
        name: string;
        displayName?: string | undefined;
        description?: string | undefined;
        isSingleton?: boolean | undefined;
        deleteCompletedInstances?: boolean | undefined;
        channel?: string | undefined;
        tag?: string | undefined;
        persistenceBehavior?: Enum.WorkflowPersistenceBehavior | undefined;
        contextOptions?: WorkflowContextOptions | undefined;
        variables?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowDefinitionDispatchRequest = {
        activityId?: string | undefined;
        correlationId?: string | undefined;
        contextId?: string | undefined;
        input?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowDefinitionDispatchResult = {
        workflowInstanceId?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowDefinitionExecuteRequest = {
        activityId?: string | undefined;
        correlationId?: string | undefined;
        contextId?: string | undefined;
        input?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowDefinitionVersion = {
        id?: string | undefined;
        creationTime?: string | undefined;
        creatorId?: string | undefined;
        lastModificationTime?: string | undefined;
        lastModifierId?: string | undefined;
        definition?: WorkflowDefinition | undefined;
        version?: number | undefined;
        isLatest?: boolean | undefined;
        isPublished?: boolean | undefined;
        activities?: Activity[] | undefined;
        connections?: ActivityConnection[] | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowDefinitionVersionCreateOrUpdate = {
        definition: WorkflowDefinitionCreateOrUpdate;
        activities?: ActivityCreateOrUpdate[] | undefined;
        connections?: ActivityConnectionCreate[] | undefined;
        isPublished?: boolean | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowDefinitionVersionListItem = {
        id?: string | undefined;
        creationTime?: string | undefined;
        creatorId?: string | undefined;
        lastModificationTime?: string | undefined;
        lastModifierId?: string | undefined;
        version?: number | undefined;
        isLatest?: boolean | undefined;
        isPublished?: boolean | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowInstance = {
        id?: string | undefined;
        creationTime?: string | undefined;
        creatorId?: string | undefined;
        lastModificationTime?: string | undefined;
        lastModifierId?: string | undefined;
        workflowDefinitionId?: string | undefined;
        workflowDefinitionVersionId?: string | undefined;
        name?: string | undefined;
        version?: number | undefined;
        workflowStatus?: Enum.WorkflowInstanceStatus | undefined;
        correlationId?: string | undefined;
        contextType?: string | undefined;
        contextId?: string | undefined;
        lastExecutedTime?: string | undefined;
        finishedTime?: string | undefined;
        cancelledTime?: string | undefined;
        faultedTime?: string | undefined;
        lastExecutedActivityId?: string | undefined;
        input?: WorkflowInputReference | undefined;
        output?: WorkflowOutputReference | undefined;
        currentActivity?: WorkflowInstanceScheduledActivity | undefined;
        faults?: WorkflowInstanceFault[] | undefined;
        variables?: any | undefined;
        metadata?: any | undefined;
        scheduledActivities?: WorkflowInstanceScheduledActivity[] | undefined;
        blockingActivities?: WorkflowInstanceBlockingActivity[] | undefined;
        activityScopes?: WorkflowInstanceActivityScope[] | undefined;
        activityData?: WorkflowInstanceActivityData[] | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowInstanceActivityData = {
        activityId?: string | undefined;
        data?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowInstanceActivityScope = {
        activityId?: string | undefined;
        variables?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowInstanceBasic = {
        id?: string | undefined;
        creationTime?: string | undefined;
        creatorId?: string | undefined;
        lastModificationTime?: string | undefined;
        lastModifierId?: string | undefined;
        workflowDefinitionId?: string | undefined;
        workflowDefinitionVersionId?: string | undefined;
        name?: string | undefined;
        version?: number | undefined;
        workflowStatus?: Enum.WorkflowInstanceStatus | undefined;
        correlationId?: string | undefined;
        contextType?: string | undefined;
        contextId?: string | undefined;
        lastExecutedTime?: string | undefined;
        finishedTime?: string | undefined;
        cancelledTime?: string | undefined;
        faultedTime?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowInstanceBlockingActivity = {
        activityId?: string | undefined;
        activityType?: string | undefined;
        tag?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowInstanceDateCountStatistic = {
        date?: string | undefined;
        finishedCount?: number | undefined;
        failedCount?: number | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowInstanceDateCountStatisticsResult = {
        items?: WorkflowInstanceDateCountStatistic[] | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowInstanceDispatchRequest = {
        activityId?: string | undefined;
        input?: WorkflowInput | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowInstanceExecuteRequest = {
        activityId?: string | undefined;
        input?: WorkflowInput | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowInstanceExecutionLogSummary = {
        activities?: WorkflowInstanceExecutionLogSummaryActivity[] | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowInstanceExecutionLogSummaryActivity = {
        activityId?: string | undefined;
        activityType?: string | undefined;
        isExecuting?: boolean | undefined;
        isExecuted?: boolean | undefined;
        isFaulted?: boolean | undefined;
        executedCount?: number | undefined;
        startTime?: string | undefined;
        endTime?: string | undefined;
        duration?: number | undefined;
        outcomes?: string[] | undefined;
        stateData?: any | undefined;
        journalData?: any | undefined;
        message?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowInstanceRetryRequest = {
        runImmediately?: boolean | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowInstanceScheduledActivity = {
        activityId?: string | undefined;
        input?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowInstancesBatchDeleteRequest = {
        ids?: string[] | undefined;
    };

    /**
     * *TODO*
     **/
    type ChangePasswordInput = {
        currentPassword?: string | undefined;
        newPassword: string;
    };

    /**
     * *TODO*
     **/
    type Profile = {
        extraProperties?: any | undefined;
        userName?: string | undefined;
        email?: string | undefined;
        name?: string | undefined;
        surname?: string | undefined;
        phoneNumber?: string | undefined;
        isExternal?: boolean | undefined;
        hasPassword?: boolean | undefined;
        concurrencyStamp?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type Register = {
        extraProperties?: any | undefined;
        userName: string;
        emailAddress: string;
        password: string;
        appName: string;
    };

    /**
     * *TODO*
     **/
    type ResetPassword = {
        userId?: string | undefined;
        resetToken: string;
        password: string;
    };

    /**
     * *TODO*
     **/
    type SendPasswordResetCode = {
        email: string;
        appName: string;
        returnUrl?: string | undefined;
        returnUrlHash?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type UpdateProfile = {
        extraProperties?: any | undefined;
        userName?: string | undefined;
        email?: string | undefined;
        name?: string | undefined;
        surname?: string | undefined;
        phoneNumber?: string | undefined;
        concurrencyStamp?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type AbpLoginResult = {
        result?: Enum.LoginResultType | undefined;
        description?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type UserLoginInfo = {
        userNameOrEmailAddress: string;
        password: string;
        rememberMe?: boolean | undefined;
    };

    /**
     * *TODO*
     **/
    type GlobalVariablePagedResult = {
        items?: GlobalVariable[] | undefined;
        totalCount?: number | undefined;
    };

    /**
     * *TODO*
     **/
    type IdentityRoleListResult = {
        items?: IdentityRole[] | undefined;
    };

    /**
     * *TODO*
     **/
    type IdentityRolePagedResult = {
        items?: IdentityRole[] | undefined;
        totalCount?: number | undefined;
    };

    /**
     * *TODO*
     **/
    type IdentityUserPagedResult = {
        items?: IdentityUser[] | undefined;
        totalCount?: number | undefined;
    };

    /**
     * *TODO*
     **/
    type StringListResult = {
        items?: string[] | undefined;
    };

    /**
     * *TODO*
     **/
    type UserDataListResult = {
        items?: UserData[] | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowDefinitionPagedResult = {
        items?: WorkflowDefinition[] | undefined;
        totalCount?: number | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowDefinitionVersionListItemPagedResult = {
        items?: WorkflowDefinitionVersionListItem[] | undefined;
        totalCount?: number | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowExecutionLogListResult = {
        items?: WorkflowExecutionLog[] | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowExecutionLogPagedResult = {
        items?: WorkflowExecutionLog[] | undefined;
        totalCount?: number | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowInstanceBasicPagedResult = {
        items?: WorkflowInstanceBasic[] | undefined;
        totalCount?: number | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowProviderDescriptorListResult = {
        items?: WorkflowProviderDescriptor[] | undefined;
    };

    /**
     * *TODO*
     **/
    type WorkflowStorageProviderInfoListResult = {
        items?: WorkflowStorageProviderInfo[] | undefined;
    };

    /**
     * *TODO*
     **/
    type ApplicationAuthConfiguration = {
        policies?: any | undefined;
        grantedPolicies?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type ApplicationConfiguration = {
        localization?: ApplicationLocalizationConfiguration | undefined;
        auth?: ApplicationAuthConfiguration | undefined;
        setting?: ApplicationSettingConfiguration | undefined;
        currentUser?: CurrentUser | undefined;
        features?: ApplicationFeatureConfiguration | undefined;
        globalFeatures?: ApplicationGlobalFeatureConfiguration | undefined;
        multiTenancy?: MultiTenancyInfo | undefined;
        currentTenant?: CurrentTenant | undefined;
        timing?: Timing | undefined;
        clock?: Clock | undefined;
        objectExtensions?: ObjectExtensions | undefined;
    };

    /**
     * *TODO*
     **/
    type ApplicationFeatureConfiguration = {
        values?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type ApplicationGlobalFeatureConfiguration = {
        enabledFeatures?: string[] | undefined;
    };

    /**
     * *TODO*
     **/
    type ApplicationLocalizationConfiguration = {
        values?: any | undefined;
        languages?: LanguageInfo[] | undefined;
        currentCulture?: CurrentCulture | undefined;
        defaultResourceName?: string | undefined;
        languagesMap?: any | undefined;
        languageFilesMap?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type ApplicationSettingConfiguration = {
        values?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type Clock = {
        kind?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type CurrentCulture = {
        displayName?: string | undefined;
        englishName?: string | undefined;
        threeLetterIsoLanguageName?: string | undefined;
        twoLetterIsoLanguageName?: string | undefined;
        isRightToLeft?: boolean | undefined;
        cultureName?: string | undefined;
        name?: string | undefined;
        nativeName?: string | undefined;
        dateTimeFormat?: DateTimeFormat | undefined;
    };

    /**
     * *TODO*
     **/
    type CurrentUser = {
        isAuthenticated?: boolean | undefined;
        id?: string | undefined;
        tenantId?: string | undefined;
        impersonatorUserId?: string | undefined;
        impersonatorTenantId?: string | undefined;
        impersonatorUserName?: string | undefined;
        impersonatorTenantName?: string | undefined;
        userName?: string | undefined;
        name?: string | undefined;
        surName?: string | undefined;
        email?: string | undefined;
        emailVerified?: boolean | undefined;
        phoneNumber?: string | undefined;
        phoneNumberVerified?: boolean | undefined;
        roles?: string[] | undefined;
    };

    /**
     * *TODO*
     **/
    type DateTimeFormat = {
        calendarAlgorithmType?: string | undefined;
        dateTimeFormatLong?: string | undefined;
        shortDatePattern?: string | undefined;
        fullDateTimePattern?: string | undefined;
        dateSeparator?: string | undefined;
        shortTimePattern?: string | undefined;
        longTimePattern?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type IanaTimeZone = {
        timeZoneName?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type EntityExtension = {
        properties?: Record<any, ExtensionProperty> | undefined;
        configuration?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type ExtensionEnum = {
        fields?: ExtensionEnumField[] | undefined;
        localizationResource?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type ExtensionEnumField = {
        name?: string | undefined;
        value?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type ExtensionProperty = {
        type?: string | undefined;
        typeSimple?: string | undefined;
        displayName?: LocalizableString | undefined;
        api?: ExtensionPropertyApi | undefined;
        ui?: ExtensionPropertyUi | undefined;
        attributes?: ExtensionPropertyAttribute[] | undefined;
        configuration?: any | undefined;
        defaultValue?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type ExtensionPropertyApi = {
        onGet?: ExtensionPropertyApiGet | undefined;
        onCreate?: ExtensionPropertyApiCreate | undefined;
        onUpdate?: ExtensionPropertyApiUpdate | undefined;
    };

    /**
     * *TODO*
     **/
    type ExtensionPropertyApiCreate = {
        isAvailable?: boolean | undefined;
    };

    /**
     * *TODO*
     **/
    type ExtensionPropertyApiGet = {
        isAvailable?: boolean | undefined;
    };

    /**
     * *TODO*
     **/
    type ExtensionPropertyApiUpdate = {
        isAvailable?: boolean | undefined;
    };

    /**
     * *TODO*
     **/
    type ExtensionPropertyAttribute = {
        typeSimple?: string | undefined;
        config?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type ExtensionPropertyUi = {
        onTable?: ExtensionPropertyUiTable | undefined;
        onCreateForm?: ExtensionPropertyUiForm | undefined;
        onEditForm?: ExtensionPropertyUiForm | undefined;
        lookup?: ExtensionPropertyUiLookup | undefined;
    };

    /**
     * *TODO*
     **/
    type ExtensionPropertyUiForm = {
        isVisible?: boolean | undefined;
    };

    /**
     * *TODO*
     **/
    type ExtensionPropertyUiLookup = {
        url?: string | undefined;
        resultListPropertyName?: string | undefined;
        displayPropertyName?: string | undefined;
        valuePropertyName?: string | undefined;
        filterParamName?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type ExtensionPropertyUiTable = {
        isVisible?: boolean | undefined;
    };

    /**
     * *TODO*
     **/
    type LocalizableString = {
        name?: string | undefined;
        resource?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type ModuleExtension = {
        entities?: Record<any, EntityExtension> | undefined;
        configuration?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type ObjectExtensions = {
        modules?: Record<any, ModuleExtension> | undefined;
        enums?: Record<any, ExtensionEnum> | undefined;
    };

    /**
     * *TODO*
     **/
    type TimeZone = {
        iana?: IanaTimeZone | undefined;
        windows?: WindowsTimeZone | undefined;
    };

    /**
     * *TODO*
     **/
    type Timing = {
        timeZone?: TimeZone | undefined;
    };

    /**
     * *TODO*
     **/
    type WindowsTimeZone = {
        timeZoneId?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type CurrentTenant = {
        id?: string | undefined;
        name?: string | undefined;
        isAvailable?: boolean | undefined;
    };

    /**
     * *TODO*
     **/
    type MultiTenancyInfo = {
        isEnabled?: boolean | undefined;
    };

    /**
     * *TODO*
     **/
    type Feature = {
        name?: string | undefined;
        displayName?: string | undefined;
        value?: string | undefined;
        provider?: FeatureProvider | undefined;
        description?: string | undefined;
        valueType?: IStringValueType | undefined;
        depth?: number | undefined;
        parentName?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type FeatureGroup = {
        name?: string | undefined;
        displayName?: string | undefined;
        features?: Feature[] | undefined;
    };

    /**
     * *TODO*
     **/
    type FeatureProvider = {
        name?: string | undefined;
        key?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type GetFeatureListResult = {
        groups?: FeatureGroup[] | undefined;
    };

    /**
     * *TODO*
     **/
    type UpdateFeature = {
        name?: string | undefined;
        value?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type UpdateFeatures = {
        features?: UpdateFeature[] | undefined;
    };

    /**
     * *TODO*
     **/
    type ActionApiDescriptionModel = {
        uniqueName?: string | undefined;
        name?: string | undefined;
        httpMethod?: string | undefined;
        url?: string | undefined;
        supportedVersions?: string[] | undefined;
        parametersOnMethod?: MethodParameterApiDescriptionModel[] | undefined;
        parameters?: ParameterApiDescriptionModel[] | undefined;
        returnValue?: ReturnValueApiDescriptionModel | undefined;
        allowAnonymous?: boolean | undefined;
        implementFrom?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type ApplicationApiDescriptionModel = {
        modules?: Record<any, ModuleApiDescriptionModel> | undefined;
        types?: Record<any, TypeApiDescriptionModel> | undefined;
    };

    /**
     * *TODO*
     **/
    type ControllerApiDescriptionModel = {
        controllerName?: string | undefined;
        controllerGroupName?: string | undefined;
        isRemoteService?: boolean | undefined;
        apiVersion?: string | undefined;
        type?: string | undefined;
        interfaces?: ControllerInterfaceApiDescriptionModel[] | undefined;
        actions?: Record<any, ActionApiDescriptionModel> | undefined;
    };

    /**
     * *TODO*
     **/
    type ControllerInterfaceApiDescriptionModel = {
        type?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type MethodParameterApiDescriptionModel = {
        name?: string | undefined;
        typeAsString?: string | undefined;
        type?: string | undefined;
        typeSimple?: string | undefined;
        isOptional?: boolean | undefined;
        defaultValue?: any | undefined;
    };

    /**
     * *TODO*
     **/
    type ModuleApiDescriptionModel = {
        rootPath?: string | undefined;
        remoteServiceName?: string | undefined;
        controllers?: Record<any, ControllerApiDescriptionModel> | undefined;
    };

    /**
     * *TODO*
     **/
    type ParameterApiDescriptionModel = {
        nameOnMethod?: string | undefined;
        name?: string | undefined;
        jsonName?: string | undefined;
        type?: string | undefined;
        typeSimple?: string | undefined;
        isOptional?: boolean | undefined;
        defaultValue?: any | undefined;
        constraintTypes?: string[] | undefined;
        bindingSourceId?: string | undefined;
        descriptorName?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type PropertyApiDescriptionModel = {
        name?: string | undefined;
        jsonName?: string | undefined;
        type?: string | undefined;
        typeSimple?: string | undefined;
        isRequired?: boolean | undefined;
    };

    /**
     * *TODO*
     **/
    type ReturnValueApiDescriptionModel = {
        type?: string | undefined;
        typeSimple?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type TypeApiDescriptionModel = {
        baseType?: string | undefined;
        isEnum?: boolean | undefined;
        enumNames?: string[] | undefined;
        enumValues?: [] | undefined;
        genericArguments?: string[] | undefined;
        properties?: PropertyApiDescriptionModel[] | undefined;
    };

    /**
     * *TODO*
     **/
    type RemoteServiceErrorInfo = {
        code?: string | undefined;
        message?: string | undefined;
        details?: string | undefined;
        data?: any | undefined;
        validationErrors?: RemoteServiceValidationErrorInfo[] | undefined;
    };

    /**
     * *TODO*
     **/
    type RemoteServiceErrorResponse = {
        error?: RemoteServiceErrorInfo | undefined;
    };

    /**
     * *TODO*
     **/
    type RemoteServiceValidationErrorInfo = {
        message?: string | undefined;
        members?: string[] | undefined;
    };

    /**
     * *TODO*
     **/
    type IdentityRole = {
        extraProperties?: any | undefined;
        id?: string | undefined;
        name?: string | undefined;
        isDefault?: boolean | undefined;
        isStatic?: boolean | undefined;
        isPublic?: boolean | undefined;
        concurrencyStamp?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type IdentityRoleCreate = {
        extraProperties?: any | undefined;
        name: string;
        isDefault?: boolean | undefined;
        isPublic?: boolean | undefined;
    };

    /**
     * *TODO*
     **/
    type IdentityRoleUpdate = {
        extraProperties?: any | undefined;
        name: string;
        isDefault?: boolean | undefined;
        isPublic?: boolean | undefined;
        concurrencyStamp?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type IdentityUser = {
        extraProperties?: any | undefined;
        id?: string | undefined;
        creationTime?: string | undefined;
        creatorId?: string | undefined;
        lastModificationTime?: string | undefined;
        lastModifierId?: string | undefined;
        isDeleted?: boolean | undefined;
        deleterId?: string | undefined;
        deletionTime?: string | undefined;
        tenantId?: string | undefined;
        userName?: string | undefined;
        name?: string | undefined;
        surname?: string | undefined;
        email?: string | undefined;
        emailConfirmed?: boolean | undefined;
        phoneNumber?: string | undefined;
        phoneNumberConfirmed?: boolean | undefined;
        isActive?: boolean | undefined;
        lockoutEnabled?: boolean | undefined;
        lockoutEnd?: string | undefined;
        concurrencyStamp?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type IdentityUserCreate = {
        extraProperties?: any | undefined;
        userName: string;
        name?: string | undefined;
        surname?: string | undefined;
        email: string;
        phoneNumber?: string | undefined;
        isActive?: boolean | undefined;
        lockoutEnabled?: boolean | undefined;
        roleNames?: string[] | undefined;
        password: string;
    };

    /**
     * *TODO*
     **/
    type IdentityUserUpdate = {
        extraProperties?: any | undefined;
        userName: string;
        name?: string | undefined;
        surname?: string | undefined;
        email: string;
        phoneNumber?: string | undefined;
        isActive?: boolean | undefined;
        lockoutEnabled?: boolean | undefined;
        roleNames?: string[] | undefined;
        password?: string | undefined;
        concurrencyStamp?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type IdentityUserUpdateRoles = {
        roleNames: string[];
    };

    /**
     * *TODO*
     **/
    type LanguageInfo = {
        cultureName?: string | undefined;
        uiCultureName?: string | undefined;
        displayName?: string | undefined;
        flagIcon?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type NameValue = {
        name?: string | undefined;
        value?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type GetPermissionListResult = {
        entityDisplayName?: string | undefined;
        groups?: PermissionGroup[] | undefined;
    };

    /**
     * *TODO*
     **/
    type PermissionGrantInfo = {
        name?: string | undefined;
        displayName?: string | undefined;
        parentName?: string | undefined;
        isGranted?: boolean | undefined;
        allowedProviders?: string[] | undefined;
        grantedProviders?: ProviderInfo[] | undefined;
    };

    /**
     * *TODO*
     **/
    type PermissionGroup = {
        name?: string | undefined;
        displayName?: string | undefined;
        permissions?: PermissionGrantInfo[] | undefined;
    };

    /**
     * *TODO*
     **/
    type ProviderInfo = {
        providerName?: string | undefined;
        providerKey?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type UpdatePermission = {
        name?: string | undefined;
        isGranted?: boolean | undefined;
    };

    /**
     * *TODO*
     **/
    type UpdatePermissions = {
        permissions?: UpdatePermission[] | undefined;
    };

    /**
     * *TODO*
     **/
    type EmailSettings = {
        smtpHost?: string | undefined;
        smtpPort?: number | undefined;
        smtpUserName?: string | undefined;
        smtpPassword?: string | undefined;
        smtpDomain?: string | undefined;
        smtpEnableSsl?: boolean | undefined;
        smtpUseDefaultCredentials?: boolean | undefined;
        defaultFromAddress?: string | undefined;
        defaultFromDisplayName?: string | undefined;
    };

    /**
     * *TODO*
     **/
    type UpdateEmailSettings = {
        smtpHost?: string | undefined;
        smtpPort?: number | undefined;
        smtpUserName?: string | undefined;
        smtpPassword?: string | undefined;
        smtpDomain?: string | undefined;
        smtpEnableSsl?: boolean | undefined;
        smtpUseDefaultCredentials?: boolean | undefined;
        defaultFromAddress: string;
        defaultFromDisplayName: string;
    };

    /**
     * *TODO*
     **/
    type UserData = {
        id?: string | undefined;
        tenantId?: string | undefined;
        userName?: string | undefined;
        name?: string | undefined;
        surname?: string | undefined;
        email?: string | undefined;
        emailConfirmed?: boolean | undefined;
        phoneNumber?: string | undefined;
        phoneNumberConfirmed?: boolean | undefined;
    };

    /**
     * *TODO*
     **/
    type IStringValueType = {
        name?: string | undefined;
        properties?: any | undefined;
        validator?: IValueValidator | undefined;
    };

    /**
     * *TODO*
     **/
    type IValueValidator = {
        name?: string | undefined;
        properties?: any | undefined;
    };


}
