/**
 * Generate from swagger json url: https://localhost:44315/swagger/v1/swagger.json
 * Total count: 164
 **/
declare namespace API {
    /**
     *  *TODO*
     **/
    type ActivityInputDescriptor = {
        name?: string | undefined;
        type?: Type | undefined;
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
     *  *TODO*
     **/
    type ActivityOutputDescriptor = {
        name?: string | undefined;
        type?: Type | undefined;
        hint?: string | undefined;
        defaultWorkflowStorageProvider?: string | undefined;
        disableWorkflowProviderSelection?: boolean | undefined;
    };

    /**
     *  *TODO*
     **/
    type ActivityDefinitionProperty = {
        name?: string | undefined;
        syntax?: string | undefined;
        expressions?: object | undefined;
    };

    /**
     *  *TODO*
     **/
    type ActivityScope = {
        activityId?: string | undefined;
        variables?: Variables | undefined;
    };

    /**
     *  *TODO*
     **/
    type BlockingActivity = {
        activityId?: string | undefined;
        activityType?: string | undefined;
        tag?: string | undefined;
    };

    /**
     *  *TODO*
     **/
    type ScheduledActivity = {
        activityId?: string | undefined;
        input?: any | undefined;
    };

    /**
     *  *TODO*
     **/
    type SimpleException = {
        type?: Type | undefined;
        message?: string | undefined;
        stackTrace?: string | undefined;
        innerException?: SimpleException | undefined;
        data?: object | undefined;
    };

    /**
     *  *TODO*
     **/
    type Variables = {
        data?: object | undefined;
    };

    /**
     *  *TODO*
     **/
    type WorkflowContextOptions = {
        contextType?: Type | undefined;
        contextFidelity?: WorkflowContextFidelity | undefined;
    };

    /**
     *  *TODO*
     **/
    type WorkflowFault = {
        exception?: SimpleException | undefined;
        message?: string | undefined;
        faultedActivityId?: string | undefined;
        activityInput?: any | undefined;
        resuming?: boolean | undefined;
    };

    /**
     *  *TODO*
     **/
    type WorkflowInput = {
        input?: any | undefined;
        storageProviderName?: string | undefined;
    };

    /**
     *  *TODO*
     **/
    type WorkflowInputReference = {
        providerName?: string | undefined;
    };

    /**
     *  *TODO*
     **/
    type WorkflowOutputReference = {
        providerName?: string | undefined;
        activityId?: string | undefined;
    };

    /**
     *  *TODO*
     **/
    type JToken = {
    };

    /**
     *  *TODO*
     **/
    type WorkflowExecutionLog = {
        id?: number | undefined;
        creationTime?: string | undefined;
        creatorId?: string | undefined;
        workflowInstanceId?: string | undefined;
        activityId?: string | undefined;
        activityType?: string | undefined;
        timestamp?: string | undefined;
        eventName?: string | undefined;
        message?: string | undefined;
        source?: string | undefined;
        data?: object | undefined;
    };

    /**
     *  *TODO*
     **/
    type ActivityTypeDescriptor = {
        type?: string | undefined;
        displayName?: string | undefined;
        description?: string | undefined;
        category?: string | undefined;
        traits?: ActivityTraits | undefined;
        outcomes?: string[] | undefined;
        inputProperties?: ActivityInputDescriptor[] | undefined;
        outputProperties?: ActivityOutputDescriptor[] | undefined;
        customAttributes?: object | undefined;
    };

    /**
     *  *TODO*
     **/
    type ActivityTypeDescriptorListResult = {
        items?: ActivityTypeDescriptor[] | undefined;
        categories?: string[] | undefined;
    };

    /**
     *  *TODO*
     **/
    type Activity = {
        creationTime?: string | undefined;
        creatorId?: string | undefined;
        lastModificationTime?: string | undefined;
        lastModifierId?: string | undefined;
        activityId?: string | undefined;
        type?: string | undefined;
        name?: string | undefined;
        displayName?: string | undefined;
        description?: string | undefined;
        persistWorkflow?: boolean | undefined;
        loadWorkflowContext?: boolean | undefined;
        saveWorkflowContext?: boolean | undefined;
        arrtibutes?: object | undefined;
        properties?: ActivityDefinitionProperty[] | undefined;
    };

    /**
     *  *TODO*
     **/
    type ActivityConnection = {
        creationTime?: string | undefined;
        creatorId?: string | undefined;
        sourceId?: string | undefined;
        targetId?: string | undefined;
        outcome?: string | undefined;
        arrtibutes?: object | undefined;
    };

    /**
     *  *TODO*
     **/
    type ActivityConnectionCreate = {
        sourceId?: string | undefined;
        targetId?: string | undefined;
        outcome?: string | undefined;
        arrtibutes?: object | undefined;
    };

    /**
     *  *TODO*
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
        arrtibutes?: object | undefined;
        properties?: ActivityDefinitionProperty[] | undefined;
        propertyStorageProviders?: object | undefined;
    };

    /**
     *  *TODO*
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
        persistenceBehavior?: WorkflowPersistenceBehavior | undefined;
        contextOptions?: WorkflowContextOptions | undefined;
        variables?: object | undefined;
        customAttributes?: object | undefined;
    };

    /**
     *  *TODO*
     **/
    type WorkflowDefinitionCreateOrUpdate = {
        name: string;
        displayName?: string | undefined;
        description?: string | undefined;
        isSingleton?: boolean | undefined;
        deleteCompletedInstances?: boolean | undefined;
        channel?: string | undefined;
        tag?: string | undefined;
        persistenceBehavior?: WorkflowPersistenceBehavior | undefined;
    };

    /**
     *  *TODO*
     **/
    type WorkflowDefinitionDispatchRequest = {
        activityId?: string | undefined;
        correlationId?: string | undefined;
        contextId?: string | undefined;
        input?: any | undefined;
    };

    /**
     *  *TODO*
     **/
    type WorkflowDefinitionDispatchResult = {
        workflowInstanceId?: string | undefined;
    };

    /**
     *  *TODO*
     **/
    type WorkflowDefinitionExecuteRequest = {
        activityId?: string | undefined;
        correlationId?: string | undefined;
        contextId?: string | undefined;
        input?: any | undefined;
    };

    /**
     *  *TODO*
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
     *  *TODO*
     **/
    type WorkflowDefinitionVersionCreateOrUpdate = {
        definition: WorkflowDefinitionCreateOrUpdate;
        activities?: ActivityCreateOrUpdate[] | undefined;
        connections?: ActivityConnectionCreate[] | undefined;
        isPublished?: boolean | undefined;
    };

    /**
     *  *TODO*
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
     *  *TODO*
     **/
    type WorkflowInstance = {
        id?: string | undefined;
        creationTime?: string | undefined;
        creatorId?: string | undefined;
        lastModificationTime?: string | undefined;
        lastModifierId?: string | undefined;
        definitionId?: string | undefined;
        definitionVersionId?: string | undefined;
        name?: string | undefined;
        version?: number | undefined;
        workflowStatus?: number | undefined;
        correlationId?: string | undefined;
        contextType?: string | undefined;
        contextId?: string | undefined;
        lastExecutedTime?: string | undefined;
        finishedTime?: string | undefined;
        cancelledTime?: string | undefined;
        faultedTime?: string | undefined;
        variables?: object | undefined;
        input?: WorkflowInputReference | undefined;
        output?: WorkflowOutputReference | undefined;
        fault?: WorkflowFault | undefined;
        scheduledActivities?: ScheduledActivity[] | undefined;
        blockingActivities?: BlockingActivity[] | undefined;
        scopes?: ActivityScope[] | undefined;
        currentActivity?: ScheduledActivity | undefined;
        lastExecutedActivityId?: string | undefined;
        activityData?: object | undefined;
        metadata?: object | undefined;
    };

    /**
     *  *TODO*
     **/
    type WorkflowInstanceBasic = {
        id?: string | undefined;
        creationTime?: string | undefined;
        creatorId?: string | undefined;
        lastModificationTime?: string | undefined;
        lastModifierId?: string | undefined;
        definitionId?: string | undefined;
        definitionVersionId?: string | undefined;
        name?: string | undefined;
        version?: number | undefined;
        workflowStatus?: number | undefined;
        correlationId?: string | undefined;
        contextType?: string | undefined;
        contextId?: string | undefined;
        lastExecutedTime?: string | undefined;
        finishedTime?: string | undefined;
        cancelledTime?: string | undefined;
        faultedTime?: string | undefined;
        fault?: WorkflowFault | undefined;
    };

    /**
     *  *TODO*
     **/
    type WorkflowInstanceDispatchRequest = {
        activityId?: string | undefined;
        input?: WorkflowInput | undefined;
    };

    /**
     *  *TODO*
     **/
    type WorkflowInstanceExecuteRequest = {
        activityId?: string | undefined;
        input?: WorkflowInput | undefined;
    };

    /**
     *  *TODO*
     **/
    type WorkflowInstanceRetryRequest = {
        runImmediately?: boolean | undefined;
    };

    /**
     *  *TODO*
     **/
    type IntPtr = {
    };

    /**
     *  *TODO*
     **/
    type ModuleHandle = {
        mdStreamVersion?: number | undefined;
    };

    /**
     *  *TODO*
     **/
    type Assembly = {
        definedTypes?: TypeInfo[] | undefined;
        exportedTypes?: Type[] | undefined;
        codeBase?: string | undefined;
        entryPoint?: MethodInfo | undefined;
        fullName?: string | undefined;
        imageRuntimeVersion?: string | undefined;
        isDynamic?: boolean | undefined;
        location?: string | undefined;
        reflectionOnly?: boolean | undefined;
        isCollectible?: boolean | undefined;
        isFullyTrusted?: boolean | undefined;
        customAttributes?: CustomAttributeData[] | undefined;
        escapedCodeBase?: string | undefined;
        manifestModule?: Module | undefined;
        modules?: Module[] | undefined;
        globalAssemblyCache?: boolean | undefined;
        hostContext?: number | undefined;
        securityRuleSet?: SecurityRuleSet | undefined;
    };

    /**
     *  *TODO*
     **/
    type ConstructorInfo = {
        name?: string | undefined;
        declaringType?: Type | undefined;
        reflectedType?: Type | undefined;
        module?: Module | undefined;
        customAttributes?: CustomAttributeData[] | undefined;
        isCollectible?: boolean | undefined;
        metadataToken?: number | undefined;
        attributes?: MethodAttributes | undefined;
        methodImplementationFlags?: MethodImplAttributes | undefined;
        callingConvention?: CallingConventions | undefined;
        isAbstract?: boolean | undefined;
        isConstructor?: boolean | undefined;
        isFinal?: boolean | undefined;
        isHideBySig?: boolean | undefined;
        isSpecialName?: boolean | undefined;
        isStatic?: boolean | undefined;
        isVirtual?: boolean | undefined;
        isAssembly?: boolean | undefined;
        isFamily?: boolean | undefined;
        isFamilyAndAssembly?: boolean | undefined;
        isFamilyOrAssembly?: boolean | undefined;
        isPrivate?: boolean | undefined;
        isPublic?: boolean | undefined;
        isConstructedGenericMethod?: boolean | undefined;
        isGenericMethod?: boolean | undefined;
        isGenericMethodDefinition?: boolean | undefined;
        containsGenericParameters?: boolean | undefined;
        methodHandle?: RuntimeMethodHandle | undefined;
        isSecurityCritical?: boolean | undefined;
        isSecuritySafeCritical?: boolean | undefined;
        isSecurityTransparent?: boolean | undefined;
        memberType?: MemberTypes | undefined;
    };

    /**
     *  *TODO*
     **/
    type CustomAttributeData = {
        attributeType?: Type | undefined;
        constructor?: ConstructorInfo | undefined;
        constructorArguments?: CustomAttributeTypedArgument[] | undefined;
        namedArguments?: CustomAttributeNamedArgument[] | undefined;
    };

    /**
     *  *TODO*
     **/
    type CustomAttributeNamedArgument = {
        memberInfo?: MemberInfo | undefined;
        typedValue?: CustomAttributeTypedArgument | undefined;
        memberName?: string | undefined;
        isField?: boolean | undefined;
    };

    /**
     *  *TODO*
     **/
    type CustomAttributeTypedArgument = {
        argumentType?: Type | undefined;
        value?: any | undefined;
    };

    /**
     *  *TODO*
     **/
    type EventInfo = {
        name?: string | undefined;
        declaringType?: Type | undefined;
        reflectedType?: Type | undefined;
        module?: Module | undefined;
        customAttributes?: CustomAttributeData[] | undefined;
        isCollectible?: boolean | undefined;
        metadataToken?: number | undefined;
        memberType?: MemberTypes | undefined;
        attributes?: EventAttributes | undefined;
        isSpecialName?: boolean | undefined;
        addMethod?: MethodInfo | undefined;
        removeMethod?: MethodInfo | undefined;
        raiseMethod?: MethodInfo | undefined;
        isMulticast?: boolean | undefined;
        eventHandlerType?: Type | undefined;
    };

    /**
     *  *TODO*
     **/
    type FieldInfo = {
        name?: string | undefined;
        declaringType?: Type | undefined;
        reflectedType?: Type | undefined;
        module?: Module | undefined;
        customAttributes?: CustomAttributeData[] | undefined;
        isCollectible?: boolean | undefined;
        metadataToken?: number | undefined;
        memberType?: MemberTypes | undefined;
        attributes?: FieldAttributes | undefined;
        fieldType?: Type | undefined;
        isInitOnly?: boolean | undefined;
        isLiteral?: boolean | undefined;
        isNotSerialized?: boolean | undefined;
        isPinvokeImpl?: boolean | undefined;
        isSpecialName?: boolean | undefined;
        isStatic?: boolean | undefined;
        isAssembly?: boolean | undefined;
        isFamily?: boolean | undefined;
        isFamilyAndAssembly?: boolean | undefined;
        isFamilyOrAssembly?: boolean | undefined;
        isPrivate?: boolean | undefined;
        isPublic?: boolean | undefined;
        isSecurityCritical?: boolean | undefined;
        isSecuritySafeCritical?: boolean | undefined;
        isSecurityTransparent?: boolean | undefined;
        fieldHandle?: RuntimeFieldHandle | undefined;
    };

    /**
     *  *TODO*
     **/
    type ICustomAttributeProvider = {
    };

    /**
     *  *TODO*
     **/
    type MemberInfo = {
        memberType?: MemberTypes | undefined;
        declaringType?: Type | undefined;
        reflectedType?: Type | undefined;
        name?: string | undefined;
        module?: Module | undefined;
        customAttributes?: CustomAttributeData[] | undefined;
        isCollectible?: boolean | undefined;
        metadataToken?: number | undefined;
    };

    /**
     *  *TODO*
     **/
    type MethodBase = {
        memberType?: MemberTypes | undefined;
        name?: string | undefined;
        declaringType?: Type | undefined;
        reflectedType?: Type | undefined;
        module?: Module | undefined;
        customAttributes?: CustomAttributeData[] | undefined;
        isCollectible?: boolean | undefined;
        metadataToken?: number | undefined;
        attributes?: MethodAttributes | undefined;
        methodImplementationFlags?: MethodImplAttributes | undefined;
        callingConvention?: CallingConventions | undefined;
        isAbstract?: boolean | undefined;
        isConstructor?: boolean | undefined;
        isFinal?: boolean | undefined;
        isHideBySig?: boolean | undefined;
        isSpecialName?: boolean | undefined;
        isStatic?: boolean | undefined;
        isVirtual?: boolean | undefined;
        isAssembly?: boolean | undefined;
        isFamily?: boolean | undefined;
        isFamilyAndAssembly?: boolean | undefined;
        isFamilyOrAssembly?: boolean | undefined;
        isPrivate?: boolean | undefined;
        isPublic?: boolean | undefined;
        isConstructedGenericMethod?: boolean | undefined;
        isGenericMethod?: boolean | undefined;
        isGenericMethodDefinition?: boolean | undefined;
        containsGenericParameters?: boolean | undefined;
        methodHandle?: RuntimeMethodHandle | undefined;
        isSecurityCritical?: boolean | undefined;
        isSecuritySafeCritical?: boolean | undefined;
        isSecurityTransparent?: boolean | undefined;
    };

    /**
     *  *TODO*
     **/
    type MethodInfo = {
        name?: string | undefined;
        declaringType?: Type | undefined;
        reflectedType?: Type | undefined;
        module?: Module | undefined;
        customAttributes?: CustomAttributeData[] | undefined;
        isCollectible?: boolean | undefined;
        metadataToken?: number | undefined;
        attributes?: MethodAttributes | undefined;
        methodImplementationFlags?: MethodImplAttributes | undefined;
        callingConvention?: CallingConventions | undefined;
        isAbstract?: boolean | undefined;
        isConstructor?: boolean | undefined;
        isFinal?: boolean | undefined;
        isHideBySig?: boolean | undefined;
        isSpecialName?: boolean | undefined;
        isStatic?: boolean | undefined;
        isVirtual?: boolean | undefined;
        isAssembly?: boolean | undefined;
        isFamily?: boolean | undefined;
        isFamilyAndAssembly?: boolean | undefined;
        isFamilyOrAssembly?: boolean | undefined;
        isPrivate?: boolean | undefined;
        isPublic?: boolean | undefined;
        isConstructedGenericMethod?: boolean | undefined;
        isGenericMethod?: boolean | undefined;
        isGenericMethodDefinition?: boolean | undefined;
        containsGenericParameters?: boolean | undefined;
        methodHandle?: RuntimeMethodHandle | undefined;
        isSecurityCritical?: boolean | undefined;
        isSecuritySafeCritical?: boolean | undefined;
        isSecurityTransparent?: boolean | undefined;
        memberType?: MemberTypes | undefined;
        returnParameter?: ParameterInfo | undefined;
        returnType?: Type | undefined;
        returnTypeCustomAttributes?: ICustomAttributeProvider | undefined;
    };

    /**
     *  *TODO*
     **/
    type Module = {
        assembly?: Assembly | undefined;
        fullyQualifiedName?: string | undefined;
        name?: string | undefined;
        mdStreamVersion?: number | undefined;
        moduleVersionId?: string | undefined;
        scopeName?: string | undefined;
        moduleHandle?: ModuleHandle | undefined;
        customAttributes?: CustomAttributeData[] | undefined;
        metadataToken?: number | undefined;
    };

    /**
     *  *TODO*
     **/
    type ParameterInfo = {
        attributes?: ParameterAttributes | undefined;
        member?: MemberInfo | undefined;
        name?: string | undefined;
        parameterType?: Type | undefined;
        position?: number | undefined;
        isIn?: boolean | undefined;
        isLcid?: boolean | undefined;
        isOptional?: boolean | undefined;
        isOut?: boolean | undefined;
        isRetval?: boolean | undefined;
        defaultValue?: any | undefined;
        rawDefaultValue?: any | undefined;
        hasDefaultValue?: boolean | undefined;
        customAttributes?: CustomAttributeData[] | undefined;
        metadataToken?: number | undefined;
    };

    /**
     *  *TODO*
     **/
    type PropertyInfo = {
        name?: string | undefined;
        declaringType?: Type | undefined;
        reflectedType?: Type | undefined;
        module?: Module | undefined;
        customAttributes?: CustomAttributeData[] | undefined;
        isCollectible?: boolean | undefined;
        metadataToken?: number | undefined;
        memberType?: MemberTypes | undefined;
        propertyType?: Type | undefined;
        attributes?: PropertyAttributes | undefined;
        isSpecialName?: boolean | undefined;
        canRead?: boolean | undefined;
        canWrite?: boolean | undefined;
        getMethod?: MethodInfo | undefined;
        setMethod?: MethodInfo | undefined;
    };

    /**
     *  *TODO*
     **/
    type TypeInfo = {
        name?: string | undefined;
        customAttributes?: CustomAttributeData[] | undefined;
        isCollectible?: boolean | undefined;
        metadataToken?: number | undefined;
        isInterface?: boolean | undefined;
        memberType?: MemberTypes | undefined;
        namespace?: string | undefined;
        assemblyQualifiedName?: string | undefined;
        fullName?: string | undefined;
        assembly?: Assembly | undefined;
        module?: Module | undefined;
        isNested?: boolean | undefined;
        declaringType?: Type | undefined;
        declaringMethod?: MethodBase | undefined;
        reflectedType?: Type | undefined;
        underlyingSystemType?: Type | undefined;
        isTypeDefinition?: boolean | undefined;
        isArray?: boolean | undefined;
        isByRef?: boolean | undefined;
        isPointer?: boolean | undefined;
        isConstructedGenericType?: boolean | undefined;
        isGenericParameter?: boolean | undefined;
        isGenericTypeParameter?: boolean | undefined;
        isGenericMethodParameter?: boolean | undefined;
        isGenericType?: boolean | undefined;
        isGenericTypeDefinition?: boolean | undefined;
        isSZArray?: boolean | undefined;
        isVariableBoundArray?: boolean | undefined;
        isByRefLike?: boolean | undefined;
        hasElementType?: boolean | undefined;
        genericTypeArguments?: Type[] | undefined;
        genericParameterPosition?: number | undefined;
        genericParameterAttributes?: GenericParameterAttributes | undefined;
        attributes?: TypeAttributes | undefined;
        isAbstract?: boolean | undefined;
        isImport?: boolean | undefined;
        isSealed?: boolean | undefined;
        isSpecialName?: boolean | undefined;
        isClass?: boolean | undefined;
        isNestedAssembly?: boolean | undefined;
        isNestedFamANDAssem?: boolean | undefined;
        isNestedFamily?: boolean | undefined;
        isNestedFamORAssem?: boolean | undefined;
        isNestedPrivate?: boolean | undefined;
        isNestedPublic?: boolean | undefined;
        isNotPublic?: boolean | undefined;
        isPublic?: boolean | undefined;
        isAutoLayout?: boolean | undefined;
        isExplicitLayout?: boolean | undefined;
        isLayoutSequential?: boolean | undefined;
        isAnsiClass?: boolean | undefined;
        isAutoClass?: boolean | undefined;
        isUnicodeClass?: boolean | undefined;
        isCOMObject?: boolean | undefined;
        isContextful?: boolean | undefined;
        isEnum?: boolean | undefined;
        isMarshalByRef?: boolean | undefined;
        isPrimitive?: boolean | undefined;
        isValueType?: boolean | undefined;
        isSignatureType?: boolean | undefined;
        isSecurityCritical?: boolean | undefined;
        isSecuritySafeCritical?: boolean | undefined;
        isSecurityTransparent?: boolean | undefined;
        structLayoutAttribute?: StructLayoutAttribute | undefined;
        typeInitializer?: ConstructorInfo | undefined;
        typeHandle?: RuntimeTypeHandle | undefined;
        guid?: string | undefined;
        baseType?: Type | undefined;
        isSerializable?: boolean | undefined;
        containsGenericParameters?: boolean | undefined;
        isVisible?: boolean | undefined;
        genericTypeParameters?: Type[] | undefined;
        declaredConstructors?: ConstructorInfo[] | undefined;
        declaredEvents?: EventInfo[] | undefined;
        declaredFields?: FieldInfo[] | undefined;
        declaredMembers?: MemberInfo[] | undefined;
        declaredMethods?: MethodInfo[] | undefined;
        declaredNestedTypes?: TypeInfo[] | undefined;
        declaredProperties?: PropertyInfo[] | undefined;
        implementedInterfaces?: Type[] | undefined;
    };

    /**
     *  *TODO*
     **/
    type StructLayoutAttribute = {
        typeId?: any | undefined;
        value?: LayoutKind | undefined;
    };

    /**
     *  *TODO*
     **/
    type RuntimeFieldHandle = {
        value?: IntPtr | undefined;
    };

    /**
     *  *TODO*
     **/
    type RuntimeMethodHandle = {
        value?: IntPtr | undefined;
    };

    /**
     *  *TODO*
     **/
    type RuntimeTypeHandle = {
        value?: IntPtr | undefined;
    };

    /**
     *  *TODO*
     **/
    type Type = {
        name?: string | undefined;
        customAttributes?: CustomAttributeData[] | undefined;
        isCollectible?: boolean | undefined;
        metadataToken?: number | undefined;
        isInterface?: boolean | undefined;
        memberType?: MemberTypes | undefined;
        namespace?: string | undefined;
        assemblyQualifiedName?: string | undefined;
        fullName?: string | undefined;
        assembly?: Assembly | undefined;
        module?: Module | undefined;
        isNested?: boolean | undefined;
        declaringType?: Type | undefined;
        declaringMethod?: MethodBase | undefined;
        reflectedType?: Type | undefined;
        underlyingSystemType?: Type | undefined;
        isTypeDefinition?: boolean | undefined;
        isArray?: boolean | undefined;
        isByRef?: boolean | undefined;
        isPointer?: boolean | undefined;
        isConstructedGenericType?: boolean | undefined;
        isGenericParameter?: boolean | undefined;
        isGenericTypeParameter?: boolean | undefined;
        isGenericMethodParameter?: boolean | undefined;
        isGenericType?: boolean | undefined;
        isGenericTypeDefinition?: boolean | undefined;
        isSZArray?: boolean | undefined;
        isVariableBoundArray?: boolean | undefined;
        isByRefLike?: boolean | undefined;
        hasElementType?: boolean | undefined;
        genericTypeArguments?: Type[] | undefined;
        genericParameterPosition?: number | undefined;
        genericParameterAttributes?: GenericParameterAttributes | undefined;
        attributes?: TypeAttributes | undefined;
        isAbstract?: boolean | undefined;
        isImport?: boolean | undefined;
        isSealed?: boolean | undefined;
        isSpecialName?: boolean | undefined;
        isClass?: boolean | undefined;
        isNestedAssembly?: boolean | undefined;
        isNestedFamANDAssem?: boolean | undefined;
        isNestedFamily?: boolean | undefined;
        isNestedFamORAssem?: boolean | undefined;
        isNestedPrivate?: boolean | undefined;
        isNestedPublic?: boolean | undefined;
        isNotPublic?: boolean | undefined;
        isPublic?: boolean | undefined;
        isAutoLayout?: boolean | undefined;
        isExplicitLayout?: boolean | undefined;
        isLayoutSequential?: boolean | undefined;
        isAnsiClass?: boolean | undefined;
        isAutoClass?: boolean | undefined;
        isUnicodeClass?: boolean | undefined;
        isCOMObject?: boolean | undefined;
        isContextful?: boolean | undefined;
        isEnum?: boolean | undefined;
        isMarshalByRef?: boolean | undefined;
        isPrimitive?: boolean | undefined;
        isValueType?: boolean | undefined;
        isSignatureType?: boolean | undefined;
        isSecurityCritical?: boolean | undefined;
        isSecuritySafeCritical?: boolean | undefined;
        isSecurityTransparent?: boolean | undefined;
        structLayoutAttribute?: StructLayoutAttribute | undefined;
        typeInitializer?: ConstructorInfo | undefined;
        typeHandle?: RuntimeTypeHandle | undefined;
        guid?: string | undefined;
        baseType?: Type | undefined;
        isSerializable?: boolean | undefined;
        containsGenericParameters?: boolean | undefined;
        isVisible?: boolean | undefined;
    };

    /**
     *  *TODO*
     **/
    type ChangePasswordInput = {
        currentPassword?: string | undefined;
        newPassword: string;
    };

    /**
     *  *TODO*
     **/
    type Profile = {
        extraProperties?: object | undefined;
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
     *  *TODO*
     **/
    type Register = {
        extraProperties?: object | undefined;
        userName: string;
        emailAddress: string;
        password: string;
        appName: string;
    };

    /**
     *  *TODO*
     **/
    type ResetPassword = {
        userId?: string | undefined;
        resetToken: string;
        password: string;
    };

    /**
     *  *TODO*
     **/
    type SendPasswordResetCode = {
        email: string;
        appName: string;
        returnUrl?: string | undefined;
        returnUrlHash?: string | undefined;
    };

    /**
     *  *TODO*
     **/
    type UpdateProfile = {
        extraProperties?: object | undefined;
        userName?: string | undefined;
        email?: string | undefined;
        name?: string | undefined;
        surname?: string | undefined;
        phoneNumber?: string | undefined;
        concurrencyStamp?: string | undefined;
    };

    /**
     *  *TODO*
     **/
    type AbpLoginResult = {
        result?: LoginResultType | undefined;
        description?: string | undefined;
    };

    /**
     *  *TODO*
     **/
    type UserLoginInfo = {
        userNameOrEmailAddress: string;
        password: string;
        rememberMe?: boolean | undefined;
    };

    /**
     *  *TODO*
     **/
    type IdentityRoleListResult = {
        items?: IdentityRole[] | undefined;
    };

    /**
     *  *TODO*
     **/
    type IdentityRolePagedResult = {
        items?: IdentityRole[] | undefined;
        totalCount?: number | undefined;
    };

    /**
     *  *TODO*
     **/
    type IdentityUserPagedResult = {
        items?: IdentityUser[] | undefined;
        totalCount?: number | undefined;
    };

    /**
     *  *TODO*
     **/
    type TenantPagedResult = {
        items?: Tenant[] | undefined;
        totalCount?: number | undefined;
    };

    /**
     *  *TODO*
     **/
    type UserDataListResult = {
        items?: UserData[] | undefined;
    };

    /**
     *  *TODO*
     **/
    type WorkflowDefinitionPagedResult = {
        items?: WorkflowDefinition[] | undefined;
        totalCount?: number | undefined;
    };

    /**
     *  *TODO*
     **/
    type WorkflowDefinitionVersionListItemPagedResult = {
        items?: WorkflowDefinitionVersionListItem[] | undefined;
        totalCount?: number | undefined;
    };

    /**
     *  *TODO*
     **/
    type WorkflowExecutionLogListResult = {
        items?: WorkflowExecutionLog[] | undefined;
    };

    /**
     *  *TODO*
     **/
    type WorkflowExecutionLogPagedResult = {
        items?: WorkflowExecutionLog[] | undefined;
        totalCount?: number | undefined;
    };

    /**
     *  *TODO*
     **/
    type WorkflowInstanceBasicPagedResult = {
        items?: WorkflowInstanceBasic[] | undefined;
        totalCount?: number | undefined;
    };

    /**
     *  *TODO*
     **/
    type ApplicationAuthConfiguration = {
        policies?: object | undefined;
        grantedPolicies?: object | undefined;
    };

    /**
     *  *TODO*
     **/
    type ApplicationConfiguration = {
        localization?: ApplicationLocalizationConfiguration | undefined;
        auth?: ApplicationAuthConfiguration | undefined;
        setting?: ApplicationSettingConfiguration | undefined;
        currentUser?: CurrentUser | undefined;
        features?: ApplicationFeatureConfiguration | undefined;
        multiTenancy?: MultiTenancyInfo | undefined;
        currentTenant?: CurrentTenant | undefined;
        timing?: Timing | undefined;
        clock?: Clock | undefined;
        objectExtensions?: ObjectExtensions | undefined;
    };

    /**
     *  *TODO*
     **/
    type ApplicationFeatureConfiguration = {
        values?: object | undefined;
    };

    /**
     *  *TODO*
     **/
    type ApplicationLocalizationConfiguration = {
        values?: object | undefined;
        languages?: LanguageInfo[] | undefined;
        currentCulture?: CurrentCulture | undefined;
        defaultResourceName?: string | undefined;
        languagesMap?: object | undefined;
        languageFilesMap?: object | undefined;
    };

    /**
     *  *TODO*
     **/
    type ApplicationSettingConfiguration = {
        values?: object | undefined;
    };

    /**
     *  *TODO*
     **/
    type Clock = {
        kind?: string | undefined;
    };

    /**
     *  *TODO*
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
     *  *TODO*
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
     *  *TODO*
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
     *  *TODO*
     **/
    type IanaTimeZone = {
        timeZoneName?: string | undefined;
    };

    /**
     *  *TODO*
     **/
    type EntityExtension = {
        properties?: object | undefined;
        configuration?: object | undefined;
    };

    /**
     *  *TODO*
     **/
    type ExtensionEnum = {
        fields?: ExtensionEnumField[] | undefined;
        localizationResource?: string | undefined;
    };

    /**
     *  *TODO*
     **/
    type ExtensionEnumField = {
        name?: string | undefined;
        value?: any | undefined;
    };

    /**
     *  *TODO*
     **/
    type ExtensionProperty = {
        type?: string | undefined;
        typeSimple?: string | undefined;
        displayName?: LocalizableString | undefined;
        api?: ExtensionPropertyApi | undefined;
        ui?: ExtensionPropertyUi | undefined;
        attributes?: ExtensionPropertyAttribute[] | undefined;
        configuration?: object | undefined;
        defaultValue?: any | undefined;
    };

    /**
     *  *TODO*
     **/
    type ExtensionPropertyApi = {
        onGet?: ExtensionPropertyApiGet | undefined;
        onCreate?: ExtensionPropertyApiCreate | undefined;
        onUpdate?: ExtensionPropertyApiUpdate | undefined;
    };

    /**
     *  *TODO*
     **/
    type ExtensionPropertyApiCreate = {
        isAvailable?: boolean | undefined;
    };

    /**
     *  *TODO*
     **/
    type ExtensionPropertyApiGet = {
        isAvailable?: boolean | undefined;
    };

    /**
     *  *TODO*
     **/
    type ExtensionPropertyApiUpdate = {
        isAvailable?: boolean | undefined;
    };

    /**
     *  *TODO*
     **/
    type ExtensionPropertyAttribute = {
        typeSimple?: string | undefined;
        config?: object | undefined;
    };

    /**
     *  *TODO*
     **/
    type ExtensionPropertyUi = {
        onTable?: ExtensionPropertyUiTable | undefined;
        onCreateForm?: ExtensionPropertyUiForm | undefined;
        onEditForm?: ExtensionPropertyUiForm | undefined;
        lookup?: ExtensionPropertyUiLookup | undefined;
    };

    /**
     *  *TODO*
     **/
    type ExtensionPropertyUiForm = {
        isVisible?: boolean | undefined;
    };

    /**
     *  *TODO*
     **/
    type ExtensionPropertyUiLookup = {
        url?: string | undefined;
        resultListPropertyName?: string | undefined;
        displayPropertyName?: string | undefined;
        valuePropertyName?: string | undefined;
        filterParamName?: string | undefined;
    };

    /**
     *  *TODO*
     **/
    type ExtensionPropertyUiTable = {
        isVisible?: boolean | undefined;
    };

    /**
     *  *TODO*
     **/
    type LocalizableString = {
        name?: string | undefined;
        resource?: string | undefined;
    };

    /**
     *  *TODO*
     **/
    type ModuleExtension = {
        entities?: object | undefined;
        configuration?: object | undefined;
    };

    /**
     *  *TODO*
     **/
    type ObjectExtensions = {
        modules?: object | undefined;
        enums?: object | undefined;
    };

    /**
     *  *TODO*
     **/
    type TimeZone = {
        iana?: IanaTimeZone | undefined;
        windows?: WindowsTimeZone | undefined;
    };

    /**
     *  *TODO*
     **/
    type Timing = {
        timeZone?: TimeZone | undefined;
    };

    /**
     *  *TODO*
     **/
    type WindowsTimeZone = {
        timeZoneId?: string | undefined;
    };

    /**
     *  *TODO*
     **/
    type CurrentTenant = {
        id?: string | undefined;
        name?: string | undefined;
        isAvailable?: boolean | undefined;
    };

    /**
     *  *TODO*
     **/
    type MultiTenancyInfo = {
        isEnabled?: boolean | undefined;
    };

    /**
     *  *TODO*
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
     *  *TODO*
     **/
    type FeatureGroup = {
        name?: string | undefined;
        displayName?: string | undefined;
        features?: Feature[] | undefined;
    };

    /**
     *  *TODO*
     **/
    type FeatureProvider = {
        name?: string | undefined;
        key?: string | undefined;
    };

    /**
     *  *TODO*
     **/
    type GetFeatureListResult = {
        groups?: FeatureGroup[] | undefined;
    };

    /**
     *  *TODO*
     **/
    type UpdateFeature = {
        name?: string | undefined;
        value?: string | undefined;
    };

    /**
     *  *TODO*
     **/
    type UpdateFeatures = {
        features?: UpdateFeature[] | undefined;
    };

    /**
     *  *TODO*
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
     *  *TODO*
     **/
    type ApplicationApiDescriptionModel = {
        modules?: object | undefined;
        types?: object | undefined;
    };

    /**
     *  *TODO*
     **/
    type ControllerApiDescriptionModel = {
        controllerName?: string | undefined;
        controllerGroupName?: string | undefined;
        isRemoteService?: boolean | undefined;
        apiVersion?: string | undefined;
        type?: string | undefined;
        interfaces?: ControllerInterfaceApiDescriptionModel[] | undefined;
        actions?: object | undefined;
    };

    /**
     *  *TODO*
     **/
    type ControllerInterfaceApiDescriptionModel = {
        type?: string | undefined;
    };

    /**
     *  *TODO*
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
     *  *TODO*
     **/
    type ModuleApiDescriptionModel = {
        rootPath?: string | undefined;
        remoteServiceName?: string | undefined;
        controllers?: object | undefined;
    };

    /**
     *  *TODO*
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
     *  *TODO*
     **/
    type PropertyApiDescriptionModel = {
        name?: string | undefined;
        jsonName?: string | undefined;
        type?: string | undefined;
        typeSimple?: string | undefined;
        isRequired?: boolean | undefined;
    };

    /**
     *  *TODO*
     **/
    type ReturnValueApiDescriptionModel = {
        type?: string | undefined;
        typeSimple?: string | undefined;
    };

    /**
     *  *TODO*
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
     *  *TODO*
     **/
    type RemoteServiceErrorInfo = {
        code?: string | undefined;
        message?: string | undefined;
        details?: string | undefined;
        data?: object | undefined;
        validationErrors?: RemoteServiceValidationErrorInfo[] | undefined;
    };

    /**
     *  *TODO*
     **/
    type RemoteServiceErrorResponse = {
        error?: RemoteServiceErrorInfo | undefined;
    };

    /**
     *  *TODO*
     **/
    type RemoteServiceValidationErrorInfo = {
        message?: string | undefined;
        members?: string[] | undefined;
    };

    /**
     *  *TODO*
     **/
    type IdentityRole = {
        extraProperties?: object | undefined;
        id?: string | undefined;
        name?: string | undefined;
        isDefault?: boolean | undefined;
        isStatic?: boolean | undefined;
        isPublic?: boolean | undefined;
        concurrencyStamp?: string | undefined;
    };

    /**
     *  *TODO*
     **/
    type IdentityRoleCreate = {
        extraProperties?: object | undefined;
        name: string;
        isDefault?: boolean | undefined;
        isPublic?: boolean | undefined;
    };

    /**
     *  *TODO*
     **/
    type IdentityRoleUpdate = {
        extraProperties?: object | undefined;
        name: string;
        isDefault?: boolean | undefined;
        isPublic?: boolean | undefined;
        concurrencyStamp?: string | undefined;
    };

    /**
     *  *TODO*
     **/
    type IdentityUser = {
        extraProperties?: object | undefined;
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
     *  *TODO*
     **/
    type IdentityUserCreate = {
        extraProperties?: object | undefined;
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
     *  *TODO*
     **/
    type IdentityUserUpdate = {
        extraProperties?: object | undefined;
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
     *  *TODO*
     **/
    type IdentityUserUpdateRoles = {
        roleNames: string[];
    };

    /**
     *  *TODO*
     **/
    type LanguageInfo = {
        cultureName?: string | undefined;
        uiCultureName?: string | undefined;
        displayName?: string | undefined;
        flagIcon?: string | undefined;
    };

    /**
     *  *TODO*
     **/
    type NameValue = {
        name?: string | undefined;
        value?: string | undefined;
    };

    /**
     *  *TODO*
     **/
    type GetPermissionListResult = {
        entityDisplayName?: string | undefined;
        groups?: PermissionGroup[] | undefined;
    };

    /**
     *  *TODO*
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
     *  *TODO*
     **/
    type PermissionGroup = {
        name?: string | undefined;
        displayName?: string | undefined;
        permissions?: PermissionGrantInfo[] | undefined;
    };

    /**
     *  *TODO*
     **/
    type ProviderInfo = {
        providerName?: string | undefined;
        providerKey?: string | undefined;
    };

    /**
     *  *TODO*
     **/
    type UpdatePermission = {
        name?: string | undefined;
        isGranted?: boolean | undefined;
    };

    /**
     *  *TODO*
     **/
    type UpdatePermissions = {
        permissions?: UpdatePermission[] | undefined;
    };

    /**
     *  *TODO*
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
     *  *TODO*
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
     *  *TODO*
     **/
    type Tenant = {
        extraProperties?: object | undefined;
        id?: string | undefined;
        name?: string | undefined;
        concurrencyStamp?: string | undefined;
    };

    /**
     *  *TODO*
     **/
    type TenantCreate = {
        extraProperties?: object | undefined;
        name: string;
        adminEmailAddress: string;
        adminPassword: string;
    };

    /**
     *  *TODO*
     **/
    type TenantUpdate = {
        extraProperties?: object | undefined;
        name: string;
        concurrencyStamp?: string | undefined;
    };

    /**
     *  *TODO*
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
     *  *TODO*
     **/
    type IStringValueType = {
        name?: string | undefined;
        properties?: object | undefined;
        validator?: IValueValidator | undefined;
    };

    /**
     *  *TODO*
     **/
    type IValueValidator = {
        name?: string | undefined;
        properties?: object | undefined;
    };


}
