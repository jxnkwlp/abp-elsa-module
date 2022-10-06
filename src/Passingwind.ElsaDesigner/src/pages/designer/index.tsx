import type { API } from '@/services/typings';
import { showDownloadJsonFile } from '@/services/utils';
import {
    createWorkflowDefinition,
    deleteWorkflowDefinitionVersion,
    getWorkflowDefinition,
    getWorkflowDefinitionPreviousVersion,
    getWorkflowDefinitionVersion,
    getWorkflowDefinitionVersions,
    updateWorkflowDefinition,
} from '@/services/WorkflowDefinition';
import { GlobalOutlined, SaveOutlined, SettingOutlined } from '@ant-design/icons';
import { ModalForm } from '@ant-design/pro-form';
import { PageContainer } from '@ant-design/pro-layout';
import ProTable from '@ant-design/pro-table';
import { DagreLayout } from '@antv/layout';
import type { Node } from '@antv/x6';
import {
    Button,
    Card,
    Col,
    Dropdown,
    Form,
    Menu,
    message,
    Modal,
    Popconfirm,
    Row,
    Space,
    Spin,
    Tag,
    Typography,
    Upload,
} from 'antd';
import type { RcFile } from 'antd/lib/upload';
import { isArray } from 'lodash';
import React, { useEffect, useRef } from 'react';
import { MonacoDiffEditor } from 'react-monaco-editor';
import { useHistory, useIntl, useLocation } from 'umi';
import EditFormItems from '../definition/edit-form-items';
import type { FlowActionType } from './flow';
import Flow from './flow';
import './index.less';
import NodePropForm from './node-prop-form';
import {
    conventToGraphData,
    conventToServerData,
    getNodeTypeRawData,
    getPropertySyntaxes,
} from './service';
import type {
    IGraphData,
    NodeEditFormData,
    NodeTypeProperty,
    NodeUpdateData,
    NodeUpdatePropData,
} from './type';

const Index: React.FC = () => {
    const location = useLocation();
    const history = useHistory();
    const intl = useIntl();

    const flowAction = useRef<FlowActionType>();

    const [loading, setLoading] = React.useState(false);

    const [submiting, setSubmiting] = React.useState(false);
    const [id, setId] = React.useState<string>();
    const [version, setVersion] = React.useState<number>(1);
    const [, setDefinitionVersion] = React.useState<API.WorkflowDefinitionVersion>();
    const [definition, setDefinition] = React.useState<API.WorkflowDefinition>();

    const [oldVersion, setOldVersion] = React.useState<number>();

    const [graphData, setGraphData] = React.useState<IGraphData>();

    const [editModalTitle, setEditModalTitle] = React.useState<string>();
    const [editModalVisible, setEditModalVisible] = React.useState<boolean>(false);

    const [nodeTypePropFormTitle, setNodeTypePropFormTitle] = React.useState<React.ReactNode>('');
    const [nodeTypePropFormVisible, setNodeTypePropFormVisible] = React.useState<boolean>(false);
    const [nodeTypePropList, setNodeTypePropList] = React.useState<NodeTypeProperty[]>([]);
    const [nodeTypeDescriptor, setNodeTypeDescriptor] =
        React.useState<API.ActivityTypeDescriptor>();
    const [nodePropertySyntaxs, setNodePropertySyntaxs] =
        React.useState<Record<string, string[]>>();

    const [editNodeId, setEditNodeId] = React.useState<string>('');
    const [editNodeFormData, setEditNodeFormData] = React.useState<NodeEditFormData>();
    const [editNodeFormRef] = Form.useForm();

    const [editForm] = Form.useForm();

    const [versionListModalVisible, setVersionListModalVisible] = React.useState<boolean>(false);

    const [versionDiffModalTitle, setVersionDiffModalTitle] = React.useState<string>('Diff');
    const [versionDiffModalVisible, setVersionDiffModalVisible] = React.useState<boolean>(false);
    const [versionDiffData, setVersionDiffData] =
        React.useState<{ source: string; target: string }>();

    const loadServerData = async (
        definiton: API.WorkflowDefinitionVersion,
        autoLayout: boolean = false,
    ) => {
        const gData = await conventToGraphData(definiton.activities!, definiton.connections!);

        // if (item.sourceActivityId) sourceId = item.sourceActivityId;
        // if (item.targetActivityId) targetId = item.targetActivityId;

        // if (item.source) sourceId = item.source;
        // if (item.target) targetId = item.target;

        if (autoLayout) {
            const layout = new DagreLayout({
                type: 'dagre',
                rankdir: 'TB',
                nodesep: 60,
                ranksep: 40,
            });

            // @ts-ignore
            const newModel = layout.layout(gData);
            // @ts-ignore
            setGraphData(newModel);
        } else {
            setGraphData(gData);
        }
    };

    const handleOnExport = async () => {
        if (!definition) return;

        const result = await flowAction.current?.getGraphData();
        if (result) {
            const result2 = conventToServerData(result);
            showDownloadJsonFile(
                `${definition.name}-${version}.json`,
                JSON.stringify(
                    {
                        ...result2,
                        ...definition,
                    },
                    null,
                    2,
                ),
            );
        } else {
            message.error('Get graph data failed');
        }
    };

    const handleOnImport = async (file: RcFile) => {
        try {
            const content = await file.text();
            const data = JSON.parse(content);
            const data2 = { connections: [], activities: data.activities };
            // compatible with offlice export json file
            if (data?.connections) {
                data2.connections = data.connections?.map((x: any) => {
                    return {
                        sourceId: x.sourceId ?? x.sourceActivityId,
                        targetId: x.targetId ?? x.targetActivityId,
                        outcome: x.outcome,
                    };
                });
            }

            await loadServerData(data2 as API.WorkflowDefinition, true);
            message.info('import successful.');
        } catch (error) {
            console.error(error);
            message.error('Import file failed');
        }

        // const result = await flowAction.current?.getGraphData();
    };

    // show node edit form
    // 显示节点属性编辑表单
    const handleOnShowNodeEditForm = async (nodeConfig: Node.Properties, node: Node) => {
        const loading2 = message.loading(intl.formatMessage({ id: 'common.dict.loading' }));
        //
        setNodeTypePropFormTitle(
            <Space>
                {intl.formatMessage({ id: 'page.designer.settings.title' })}
                <span>{` - ${nodeConfig.displayName}`}</span>
                <small>({nodeConfig.type})</small>
            </Space>,
        );

        setEditNodeId(node.id);

        // load node type & peoperties
        const allNodeTypes = await getNodeTypeRawData();
        const nodeType = allNodeTypes.items?.find((x) => x.type == nodeConfig.type);

        if (!nodeType) {
            message.error(`The node type '${nodeConfig.type}' not found.`);
            return;
        }

        const propItems = (nodeType.inputProperties ?? [])
            .filter((x) => x.isBrowsable)
            .map((x) => {
                return {
                    ...x,
                    isRequired: x.isDesignerCritical,
                } as NodeTypeProperty;
            });

        // save to status
        setNodeTypePropList(propItems ?? []);

        // build node edit data
        const nodeData: NodeEditFormData = {
            ...(node.getData() ?? {}),
            name: node.getProp('name') ?? '',
            displayName: node.getProp('displayName') ?? '',
            description: node.getProp('description') ?? '',
            props: {},
        };

        setNodeTypeDescriptor(nodeType);

        // initial all form fields
        const propertySyntaxs = {};
        propItems?.forEach((propItem) => {
            //
            const propSyntax = getPropertySyntaxes(propItem);
            propertySyntaxs[propItem.name] = propSyntax.supports;
            const defaultSyntax = propSyntax.default;
            //
            const defaultValue: string | object | number | undefined =
                propItem.defaultValue ?? undefined;
            let syntaxStringValue: string = '';
            if (defaultValue) {
                if (typeof defaultValue == 'object') {
                    syntaxStringValue = JSON.stringify(defaultValue);
                } else if (defaultValue) {
                    syntaxStringValue = defaultValue?.toString();
                } else {
                    syntaxStringValue = '';
                }
            }

            if (defaultSyntax)
                nodeData.props[propItem.name] = {
                    syntax: 'Default',
                    value: defaultValue,
                    expressions: {
                        Default: defaultValue,
                        [defaultSyntax]: syntaxStringValue,
                    },
                };

            if (propSyntax.editor) {
                nodeData.props[propItem.name] = {
                    syntax: propSyntax.editor,
                    value: defaultValue,
                    noSyntax: true, // special case
                    expressions: {
                        Literal: syntaxStringValue,
                    },
                };
            }
        });

        setNodePropertySyntaxs(propertySyntaxs);

        // property
        const sourceProperties = (node.getProp('properties') ?? []) as NodeUpdatePropData[];
        console.debug('activity properties source : ', sourceProperties);

        // load form data and overwrite
        sourceProperties.forEach((item) => {
            const syntax = !item.syntax ? 'Default' : item.syntax;
            let syntaxValue: any = undefined;
            let expressionValue: string = '';

            // load syntax value
            let valueSyntax = '';
            if (Object.keys(item.expressions ?? {}).filter((x) => x != '$id').length > 0) {
                valueSyntax = Object.keys(item.expressions ?? {}).filter((x) => x != '$id')[0];
                expressionValue = item.expressions![valueSyntax];
            }

            if (syntax == 'Default') {
                const property = nodeType.inputProperties?.find((x) => x.name == item.name);
                // default
                syntaxValue = expressionValue;
                //
                if (property?.uiHint == 'check-list' || property?.uiHint == 'multi-text') {
                    if (
                        (expressionValue.startsWith('{') && expressionValue.endsWith('}')) ||
                        (expressionValue.startsWith('[') && expressionValue.endsWith(']'))
                    ) {
                        syntaxValue = JSON.parse(expressionValue);
                    }
                }
            }

            const current = nodeData.props[item.name] ?? {};
            const expressions = current?.expressions ?? {};
            if (current.noSyntax) {
                nodeData.props[item.name] = {
                    ...current,
                    expressions: {
                        ...expressions,
                        Default: syntaxValue,
                        [valueSyntax]: expressionValue,
                        Literal: expressionValue,
                    },
                };
            } else {
                nodeData.props[item.name] = {
                    ...current,
                    syntax: syntax,
                    expressions: {
                        ...expressions,
                        Default: syntaxValue,
                        [valueSyntax]: expressionValue,
                    },
                };
            }
        });

        setEditNodeFormData(nodeData);

        console.debug('load form data: ', nodeData);

        editNodeFormRef.resetFields();
        editNodeFormRef.setFieldsValue(nodeData);

        // show
        setNodeTypePropFormVisible(true);
        loading2();
    };

    // handle on node edit form submit
    // 更新节点数据
    const handleUpdateNodeProperties = async (formData: NodeEditFormData) => {
        console.debug('save form data: ', formData);
        const result: NodeUpdateData = {
            ...formData,
            name: formData.name,
            displayName: formData.displayName,
            description: formData.description,
            properties: [],
            outcomes: [],
            attribtues: formData,
        };

        if (formData.props) {
            // as default, one syntax map one expressions key value
            // if not, use expressions first key as syntax and use expressions first value as value
            // if syntax is default, use expressions first key as syntax
            for (const name in formData.props ?? {}) {
                const curObj = formData.props[name];
                //
                let valueSyntaxName = curObj.syntax;
                let syntaxSourceValue: any = undefined;
                let sytaxStringValue: string = '';
                const expressions = curObj.expressions ?? {};
                const syntaxes = nodePropertySyntaxs![name];
                //
                if (curObj.expressions && Object.keys(expressions).length > 0) {
                    if (curObj.syntax == 'Default' && syntaxes.length > 0) {
                        valueSyntaxName = syntaxes[0];
                        syntaxSourceValue = curObj.expressions?.[curObj.syntax] ?? undefined;
                    } else {
                        syntaxSourceValue = curObj.expressions?.[valueSyntaxName] ?? undefined;
                    }
                    // special case
                    if (Object.keys(expressions).indexOf(curObj.syntax) == -1) {
                        // first key value
                        valueSyntaxName = Object.keys(expressions)[0];
                        syntaxSourceValue = curObj.expressions?.[valueSyntaxName] ?? undefined;
                    }

                    // server save value as string
                    if (syntaxSourceValue) {
                        if (typeof syntaxSourceValue == 'object')
                            sytaxStringValue = JSON.stringify(syntaxSourceValue);
                        else if (typeof syntaxSourceValue != 'string')
                            sytaxStringValue = syntaxSourceValue.toString();
                        else {
                            sytaxStringValue = syntaxSourceValue as string;
                        }
                    }

                    // end
                    result.properties.push({
                        name: name,
                        syntax:
                            curObj.syntax == 'Default'
                                ? undefined
                                : curObj.syntax == valueSyntaxName
                                ? valueSyntaxName
                                : undefined,
                        expressions: { [valueSyntaxName]: sytaxStringValue },
                        value: syntaxSourceValue,
                    });
                }
            }
        }

        // combination all output
        const outcomes = nodeTypeDescriptor?.outcomes ?? [];
        const outcomeValueProp = nodeTypePropList.find((x) => x.considerValuesAsOutcomes);

        if (outcomeValueProp) {
            const newValue = result.properties.find((x) => x.name == outcomeValueProp.name)?.value;
            if (newValue) {
                if (isArray<string>(newValue)) {
                    outcomes.push(...newValue);
                } else if (
                    typeof newValue == 'string' &&
                    newValue.startsWith('[') &&
                    newValue.endsWith(']')
                ) {
                    outcomes.push(...JSON.parse(newValue));
                }
            }
        }

        // special case
        if (nodeTypeDescriptor?.type == 'Switch') {
            const newValue = result.properties.find((x) => x.name == 'Cases')?.value;
            if (newValue) {
                if (
                    typeof newValue == 'string' &&
                    newValue.startsWith('[') &&
                    newValue.endsWith(']')
                ) {
                    const o: string[] = JSON.parse(newValue).map((x: any) => {
                        return x.name;
                    });
                    outcomes.push(...o);
                }
            }
        }

        result.outcomes = outcomes;

        console.debug('updated node: ', result);
        flowAction.current?.updateNodeProperties(editNodeId, result);
    };

    const handleSaveGraphData = async (publish: boolean = false) => {
        setSubmiting(true);
        const gdata = await flowAction.current?.getGraphData();

        if (gdata?.nodes.length == 0) {
            message.error('No nodes in the graph');
            setSubmiting(false);
            return;
        }

        const { activities, connections } = conventToServerData(gdata!);

        let result = null;
        if (id) {
            result = await updateWorkflowDefinition(id, {
                definition: definition as API.WorkflowDefinitionCreateOrUpdate,
                activities,
                connections,
                isPublished: publish,
            });
        } else {
            result = await createWorkflowDefinition({
                definition: definition as API.WorkflowDefinitionCreateOrUpdate,
                activities,
                connections,
                isPublished: publish,
            });
        }

        if (result) {
            if (publish) {
                message.success(
                    intl.formatMessage(
                        { id: 'page.definition.published.success' },
                        { version: result.version },
                    ),
                );
            } else {
                message.success(
                    intl.formatMessage(
                        { id: 'page.definition.saved.success' },
                        { version: result.version },
                    ),
                );
            }
            // new
            if (!id) {
                // message.success('Create successed.');
                history.replace(`/designer?id=${result.definition?.id}`);
            }

            setId(result.definition!.id);
            setVersion(result.version!);
            //
            setDefinitionVersion(result);
            setDefinition(result.definition);
        }

        setSubmiting(false);
    };

    const showCreateModal = () => {
        setEditModalTitle(intl.formatMessage({ id: 'common.dict.create' }));
        setEditModalVisible(true);
    };

    const handleVersionComparison = async (
        sourceVersionNumber: number,
        targetVersionNumber?: number,
    ) => {
        const loading = message.loading(intl.formatMessage({ id: 'common.dict.loading' }));

        const sourceVersion = await getWorkflowDefinitionVersion(id!, sourceVersionNumber);
        let targetVersion: API.WorkflowDefinitionVersion | undefined = undefined;
        if (targetVersionNumber) {
            targetVersion = await getWorkflowDefinitionVersion(id!, targetVersionNumber);
        } else {
            targetVersion = await getWorkflowDefinitionPreviousVersion(id!, sourceVersionNumber);
        }

        if (!targetVersion) {
            message.error('The comparison version not found.');
            return;
        }

        setVersionDiffData({
            source: JSON.stringify(
                { activities: sourceVersion.activities, connections: sourceVersion.connections },
                null,
                2,
            ),
            target: JSON.stringify(
                { activities: targetVersion.activities, connections: targetVersion.connections },
                null,
                2,
            ),
        });
        setVersionDiffModalTitle(
            intl.formatMessage(
                {
                    id: 'page.definition.versions.comparison.label',
                },
                { v1: sourceVersionNumber, v2: targetVersion.version },
            ),
        );
        setVersionDiffModalVisible(true);
        loading();
    };

    const loadData = async (did: string, version?: number) => {
        setLoading(true);
        let definitonVersion: API.WorkflowDefinitionVersion;
        if (version) definitonVersion = await getWorkflowDefinitionVersion(did, version);
        else definitonVersion = await getWorkflowDefinition(did);
        //
        setLoading(false);
        if (definitonVersion) {
            setDefinitionVersion(definitonVersion);
            //
            setDefinition(definitonVersion.definition);
            setVersion(definitonVersion.version!);
            //
            await loadServerData(definitonVersion);
        } else {
            history.goBack();
        }
    };

    useEffect(() => {
        // @ts-ignore
        const qid = location.query?.id ?? '';
        setId(qid);
        if (qid) {
            loadData(qid);
        } else {
            showCreateModal();
        }
    }, []);

    return (
        <PageContainer>
            <Card
                onKeyDown={(e) => {
                    e.preventDefault();
                    const charCode = String.fromCharCode(e.which).toLowerCase();
                    if ((e.ctrlKey || e.metaKey) && charCode === 's') {
                        console.log('ctrl+s');
                    }
                }}
                className="designer"
                title={
                    <>
                        <span style={{ fontSize: 18 }}>{definition?.name} </span>
                        <Tag>
                            {intl.formatMessage({ id: 'page.definition.latest' })}:{' '}
                            {definition?.latestVersion ?? 1}
                        </Tag>
                        {definition?.publishedVersion && (
                            <Tag>
                                {intl.formatMessage({ id: 'page.definition.published' })}:{' '}
                                {definition?.publishedVersion}
                            </Tag>
                        )}
                    </>
                }
                extra={
                    <Space>
                        <Button
                            type="default"
                            disabled={!definition?.name}
                            loading={submiting}
                            icon={<SaveOutlined />}
                            onClick={async () => {
                                await handleSaveGraphData();
                            }}
                        >
                            {intl.formatMessage({ id: 'common.dict.save' })}
                        </Button>
                        <Button
                            type="default"
                            disabled={!definition?.name}
                            loading={submiting}
                            icon={<GlobalOutlined />}
                            onClick={async () => {
                                await handleSaveGraphData(true);
                            }}
                        >
                            {intl.formatMessage({ id: 'page.definition.publish' })}
                        </Button>

                        <Dropdown.Button
                            disabled={submiting || !definition?.name}
                            // icon={<SettingOutlined />}
                            onClick={() => {
                                setEditModalTitle(intl.formatMessage({ id: 'common.dict.edit' }));
                                setDefinition({
                                    ...definition,
                                    // @ts-ignore
                                    variablesString: JSON.stringify(definition.variables ?? {}),
                                });
                                setEditModalVisible(true);
                            }}
                            overlay={
                                <Menu>
                                    <Menu.Item
                                        key="1"
                                        onClick={() => {
                                            setVersionListModalVisible(true);
                                        }}
                                        disabled={!id}
                                    >
                                        {intl.formatMessage({ id: 'page.definition.versions' })}
                                    </Menu.Item>
                                    <Menu.Divider />
                                    <Menu.Item
                                        key="2"
                                        onClick={() => {
                                            handleOnExport();
                                        }}
                                    >
                                        {intl.formatMessage({ id: 'common.dict.export' })}
                                    </Menu.Item>
                                    <Menu.Item key="3">
                                        <Upload
                                            accept=".json"
                                            showUploadList={false}
                                            beforeUpload={(file) => {
                                                handleOnImport(file);
                                                return false;
                                            }}
                                        >
                                            {intl.formatMessage({ id: 'common.dict.import' })}
                                        </Upload>
                                    </Menu.Item>
                                </Menu>
                            }
                        >
                            <SettingOutlined />
                            {intl.formatMessage({ id: 'page.definition.settings' })}
                        </Dropdown.Button>
                    </Space>
                }
            >
                <Spin spinning={loading}>
                    <Flow
                        actionRef={flowAction}
                        graphData={graphData}
                        onNodeDoubleClick={handleOnShowNodeEditForm}
                    />
                </Spin>
            </Card>
            {/*  */}
            <ModalForm
                form={editForm}
                layout="horizontal"
                preserve={false}
                labelCol={{ span: 5 }}
                width={600}
                labelWrap
                title={editModalTitle}
                visible={editModalVisible}
                initialValues={definition}
                onVisibleChange={setEditModalVisible}
                onFinish={async (formData) => {
                    setDefinition({
                        ...definition,
                        ...formData,
                        // @ts-ignore
                        variables: JSON.parse(formData.variablesString ?? '{}'),
                    });
                    return true;
                }}
                onValuesChange={(value) => {
                    if (value.displayName) {
                        editForm.setFieldsValue({
                            name: value.displayName?.replaceAll(' ', '-'),
                        });
                    }
                }}
            >
                <EditFormItems />
            </ModalForm>
            {/*  */}
            <ModalForm
                form={editNodeFormRef}
                layout="horizontal"
                modalProps={{ maskClosable: false, destroyOnClose: true }}
                preserve={false}
                labelWrap={true}
                title={nodeTypePropFormTitle}
                labelCol={{ span: 5 }}
                grid={true}
                width={800}
                // request={async () => {
                //     return { success: true, data: editNodeData };
                // }}
                initialValues={editNodeFormData}
                visible={nodeTypePropFormVisible}
                scrollToFirstError
                onVisibleChange={setNodeTypePropFormVisible}
                onFinish={async (formData) => {
                    handleUpdateNodeProperties({
                        ...editNodeFormData,
                        ...formData,
                    } as NodeEditFormData);
                    return true;
                }}
                // onValuesChange={(v) => {
                //     console.log(v);
                // }}
            >
                <NodePropForm
                    workflowDefinitionId={id}
                    properties={nodeTypePropList}
                    getFieldValue={editNodeFormRef.getFieldValue}
                    setFieldsValue={editNodeFormRef.setFieldsValue}
                    setFieldValue={(k, v) => {
                        editNodeFormRef.setFields([{ name: k, value: v }]);
                    }}
                />
            </ModalForm>
            {/*  */}
            <Modal
                title={intl.formatMessage({ id: 'page.definition.versions' })}
                destroyOnClose
                visible={versionListModalVisible}
                onCancel={() => setVersionListModalVisible(false)}
                width={680}
                onOk={() => {
                    if (oldVersion) {
                        setVersionListModalVisible(false);
                        message.loading(intl.formatMessage({ id: 'common.dict.loading' }), 1);
                        loadData(id!, oldVersion);
                    } else {
                        message.error(
                            intl.formatMessage({ id: 'page.definition.versions.no-select' }),
                        );
                    }
                }}
            >
                <ProTable<API.WorkflowDefinitionVersionListItem>
                    search={false}
                    toolBarRender={false}
                    rowKey="version"
                    columns={[
                        {
                            title: intl.formatMessage({ id: 'page.definition.field.version' }),
                            dataIndex: 'version',
                        },
                        {
                            title: intl.formatMessage({ id: 'page.definition.field.isLatest' }),
                            dataIndex: 'isLatest',
                            align: 'center',
                            valueEnum: { true: { text: 'Y' }, false: { text: '-' } },
                        },
                        {
                            title: intl.formatMessage({ id: 'page.definition.field.isPublished' }),
                            dataIndex: 'isPublished',
                            align: 'center',
                            valueEnum: { true: { text: 'Y' }, false: { text: '-' } },
                        },
                        {
                            title: intl.formatMessage({
                                id: 'page.definition.field.lastModificationTime',
                            }),
                            dataIndex: 'creationTime',
                            valueType: 'dateTime',
                            width: 160,
                            align: 'center',
                            renderText: (_, record) => {
                                return record.lastModificationTime ?? record.creationTime;
                            },
                        },
                        {
                            title: intl.formatMessage({ id: 'page.definition.field.comparison' }),
                            dataIndex: 'Comparison',
                            width: 120,
                            align: 'center',
                            render: (_, record) => {
                                return (
                                    <>
                                        <a
                                            onClick={() => {
                                                handleVersionComparison(record.version!, version);
                                            }}
                                        >
                                            {intl.formatMessage({
                                                id: 'page.definition.versions.comparison-latest',
                                            })}
                                        </a>
                                        <br />
                                        <a
                                            onClick={() => {
                                                handleVersionComparison(record.version!);
                                            }}
                                        >
                                            {intl.formatMessage({
                                                id: 'page.definition.versions.comparison-previous',
                                            })}
                                        </a>
                                    </>
                                );
                            },
                        },
                        {
                            title: intl.formatMessage({ id: 'common.dict.table-action' }),
                            valueType: 'option',
                            width: 80,
                            align: 'center',
                            render: (text, record, _, action) => {
                                return (
                                    <Popconfirm
                                        title={intl.formatMessage({
                                            id: 'common.dict.delete.confirm',
                                        })}
                                        onConfirm={async () => {
                                            if (record.isPublished || record.isLatest) {
                                                message.error(
                                                    'Cannot delete published or latest version',
                                                );
                                            } else {
                                                const result =
                                                    await deleteWorkflowDefinitionVersion(
                                                        id!,
                                                        record.version!,
                                                    );

                                                if (result?.response?.ok) {
                                                    action?.reload();
                                                }
                                            }
                                        }}
                                    >
                                        <a>{intl.formatMessage({ id: 'common.dict.delete' })}</a>
                                    </Popconfirm>
                                );
                            },
                        },
                    ]}
                    rowSelection={{
                        type: 'radio',
                        alwaysShowAlert: false,
                        onChange: (keys) => {
                            if (keys.length > 0) {
                                const v = parseInt(keys[0].toString());
                                setOldVersion(v);
                            }
                        },
                    }}
                    tableAlertRender={false}
                    pagination={{ pageSize: 10 }}
                    request={async (params) => {
                        const { current, pageSize } = params;
                        delete params.current;
                        delete params.pageSize;
                        const skipCount = (current! - 1) * pageSize!;
                        const result = await getWorkflowDefinitionVersions(id!, {
                            skipCount,
                            maxResultCount: pageSize,
                        });
                        if (result) {
                            setOldVersion(undefined);
                            return {
                                success: true,
                                data: result.items,
                                total: result.totalCount,
                            };
                        } else {
                            return {
                                success: false,
                            };
                        }
                    }}
                />
            </Modal>
            {/*  */}
            <Modal
                title={versionDiffModalTitle}
                visible={versionDiffModalVisible}
                onCancel={() => setVersionDiffModalVisible(false)}
                footer={false}
                width="90%"
                destroyOnClose
            >
                <MonacoDiffEditor
                    height="600"
                    language="json"
                    original={versionDiffData?.source}
                    value={versionDiffData?.target}
                    options={{
                        automaticLayout: true,
                        autoIndent: 'keep',
                        autoClosingBrackets: 'languageDefined',
                        foldingStrategy: 'auto',
                    }}
                />
            </Modal>
        </PageContainer>
    );
};

export default Index;
