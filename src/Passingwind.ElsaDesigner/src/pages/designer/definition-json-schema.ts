export default
    {
        "$schema": "http://json-schema.org/draft-06/schema#",
        "$ref": "#/definitions/Self",
        "definitions": {
            "Self": {
                "type": "object",
                "additionalProperties": true,
                "properties": {
                    "activities": {
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/Activity"
                        }
                    },
                    "connections": {
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/Connection"
                        }
                    }
                },
                "required": [
                    "activities",
                    "connections"
                ],
                "title": "Self"
            },
            "Activity": {
                "type": "object",
                "additionalProperties": false,
                "properties": {
                    "$id": {
                        "type": "string",
                        "format": "integer"
                    },
                    "activityId": {
                        "type": "string",
                        "format": "uuid"
                    },
                    "type": {
                        "type": "string"
                    },
                    "name": {
                        "type": "string"
                    },
                    "displayName": {
                        "type": "string"
                    },
                    "description": {
                        "type": "string"
                    },
                    "persistWorkflow": {
                        "type": "boolean"
                    },
                    "loadWorkflowContext": {
                        "type": "boolean"
                    },
                    "saveWorkflowContext": {
                        "type": "boolean"
                    },
                    "attributes": {
                        "$ref": "#/definitions/ActivityAttributes"
                    },
                    "properties": {
                        "type": "array",
                        "items": {
                            "$ref": "#/definitions/Property"
                        }
                    },
                    "propertyStorageProviders": {
                        "$ref": "#/definitions/PropertyStorageProviders"
                    },
                    "id": {
                        "type": "string",
                        "format": "uuid"
                    },
                    "label": {
                        "type": "string"
                    },
                    "icon": {
                        "type": "null"
                    }
                },
                "required": [
                    "activityId",
                    "attributes",
                    "id",
                    "label",
                    "name",
                    "properties",
                    "type"
                ],
                "title": "Activity"
            },
            "ActivityAttributes": {
                "type": "object",
                "additionalProperties": false,
                "properties": {
                    "$id": {
                        "type": "string",
                        "format": "integer"
                    },
                    "x": {
                        "type": "integer"
                    },
                    "y": {
                        "type": "integer"
                    },
                    "outcomes": {
                        "type": "array",
                        "items": {
                            "type": "string"
                        }
                    }
                },
                "required": [
                    "outcomes"
                ],
                "title": "ActivityAttributes"
            },
            "Property": {
                "type": "object",
                "additionalProperties": false,
                "properties": {
                    "$id": {
                        "type": "string",
                        "format": "integer"
                    },
                    "name": {
                        "type": "string"
                    },
                    "expressions": {
                        "$ref": "#/definitions/Expressions"
                    },
                    "syntax": {
                        "type": "string"
                    }
                },
                "required": [
                    "expressions"
                ],
                "title": "Property"
            },
            "Expressions": {
                "type": "object",
                "additionalProperties": false,
                "properties": {
                    "$id": {
                        "type": "string",
                        "format": "integer"
                    },
                    "Literal": {
                        "type": "string"
                    },
                    "JavaScript": {
                        "type": "string"
                    },
                    "C#": {
                        "type": "string"
                    },
                    "Json": {
                        "type": "string"
                    }
                },
                "required": [
                    "$id"
                ],
                "title": "Expressions"
            },
            "PropertyStorageProviders": {
                "type": "object",
                "additionalProperties": false,
                "properties": {
                    "$id": {
                        "type": "string",
                        "format": "integer"
                    },
                    "Output": {
                        "type": "string"
                    },
                    "Path": {
                        "type": "string"
                    }
                },
                "required": [],
                "title": "PropertyStorageProviders"
            },
            "Connection": {
                "type": "object",
                "additionalProperties": false,
                "properties": {
                    "sourceId": {
                        "type": "string",
                        "format": "uuid"
                    },
                    "targetId": {
                        "type": "string",
                        "format": "uuid"
                    },
                    "sourceActivityId": {
                        "type": "string",
                        "format": "uuid"
                    },
                    "targetActivityId": {
                        "type": "string",
                        "format": "uuid"
                    },
                    "outcome": {
                        "type": "string"
                    },
                    "attributes": {
                        "$ref": "#/definitions/ConnectionAttributes"
                    }
                },
                "required": [
                    "outcome",
                    "sourceId",
                    "targetId"
                ],
                "title": "Connection"
            },
            "ConnectionAttributes": {
                "type": "object",
                "additionalProperties": false,
                "properties": {
                    "sourcePort": {
                        "type": "string"
                    },
                    "targetPort": {
                        "type": "string"
                    }
                },
                "required": [],
                "title": "ConnectionAttributes"
            }
        }
    }
