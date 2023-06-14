export default {
    $schema: 'http://json-schema.org/draft-04/schema#',
    type: 'object',
    properties: {
        activities: {
            type: 'array',
            items: [
                {
                    type: 'object',
                    properties: {
                        activityId: {
                            type: 'string',
                        },
                        type: {
                            type: 'string',
                        },
                        name: {
                            type: 'string',
                        },
                        displayName: {
                            type: 'string',
                        },
                        description: {
                            type: 'string',
                        },
                        persistWorkflow: {
                            type: 'boolean',
                        },
                        loadWorkflowContext: {
                            type: 'boolean',
                        },
                        saveWorkflowContext: {
                            type: 'boolean',
                        },
                        attributes: {
                            type: 'object',
                            properties: {
                                x: {
                                    type: 'integer',
                                },
                                y: {
                                    type: 'integer',
                                },
                                outcomes: {
                                    type: 'array',
                                    items: [
                                        {
                                            type: ['string', 'integer'],
                                        },
                                    ],
                                },
                            },
                            required: ['x', 'y'],
                        },
                        properties: {
                            type: 'array',
                            items: [
                                {
                                    type: 'object',
                                    properties: {
                                        name: {
                                            type: 'string',
                                        },
                                        syntax: {
                                            type: 'string',
                                        },
                                        expressions: {
                                            type: 'object',
                                            properties: {
                                                Literal: {
                                                    type: 'string',
                                                },
                                                Json: {
                                                    type: 'string',
                                                },
                                                JavaScript: {
                                                    type: 'string',
                                                },
                                                'C#': {
                                                    type: 'string',
                                                },
                                                SQL: {
                                                    type: 'string',
                                                },
                                            },
                                            required: [],
                                        },
                                    },
                                    required: ['name', 'expressions'],
                                },
                            ],
                        },
                        propertyStorageProviders: {
                            type: 'object',
                        },
                        id: {
                            type: 'string',
                        },
                        label: {
                            type: 'string',
                        },
                    },
                    required: ['activityId', 'type', 'name', 'id'],
                },
            ],
        },
        connections: {
            type: 'array',
            items: [
                {
                    type: 'object',
                    properties: {
                        sourceId: {
                            type: 'string',
                        },
                        targetId: {
                            type: 'string',
                        },
                        outcome: {
                            type: 'string',
                        },
                        attributes: {
                            type: 'object',
                            properties: {
                                sourcePort: {
                                    type: 'string',
                                },
                                targetPort: {
                                    type: 'string',
                                },
                            },
                            required: ['sourcePort', 'targetPort'],
                        },
                    },
                    required: ['sourceId', 'targetId', 'attributes'],
                },
            ],
        },
        name: {
            type: 'string',
        },
        displayName: {
            type: 'string',
        },
        tenantId: {
            type: 'null',
        },
        description: {
            type: 'null',
        },
        isSingleton: {
            type: 'boolean',
        },
        deleteCompletedInstances: {
            type: 'boolean',
        },
        id: {
            type: 'string',
        },
    },
    // required: ['activities', 'connections', 'name', 'displayName', 'id'],
};
