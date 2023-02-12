import type { API } from '@/services/typings';
import {
    getWorkflowDefinition,
    getWorkflowDefinitionDefinition,
    getWorkflowDefinitionVersion,
    getWorkflowDefinitionVersions,
} from '@/services/WorkflowDefinition';
import { DownOutlined } from '@ant-design/icons';
import { PageContainer } from '@ant-design/pro-layout';
import { Button, Card, Dropdown, Modal, Space, Tag } from 'antd';
import React, { useEffect, useRef } from 'react';
import { useAccess, useHistory, useIntl, useParams } from 'umi';
import type { FlowActionType } from '../designer/flow';
import Flow from '../designer/flow';
import { conventToGraphData } from '../designer/service';
import type { IGraphData } from '../designer/type';
import './detail.less';

const Index: React.FC = () => {
    const history = useHistory();
    const params = useParams();
    const intl = useIntl();
    const access = useAccess();

    const flowAction = useRef<FlowActionType>();

    const [id, setId] = React.useState<string>();
    const [title, setTitle] = React.useState<string>();
    const [data, setData] = React.useState<API.WorkflowDefinition>();
    const [versionData, setVersionData] = React.useState<API.WorkflowDefinitionVersion>();
    const [loading, setLoading] = React.useState<boolean>(false);
    const [graphData, setGraphData] = React.useState<IGraphData>();

    const [versionList, setVersionList] = React.useState<API.WorkflowDefinitionVersionListItem[]>(
        [],
    );

    const loadVersion = async (id: string, version?: number) => {
        let _data: API.WorkflowDefinitionVersion;
        if (version) _data = await getWorkflowDefinitionVersion(id, version);
        else _data = await getWorkflowDefinition(id);

        if (!_data) {
            Modal.error({
                title: intl.formatMessage({ id: 'common.dict.error.tips' }),
                content: intl.formatMessage({ id: 'page.definition.versionNotFound' }),
                onOk: () => {
                    history.goBack();
                },
            });
            return;
        }

        setVersionData(_data);

        const data = await conventToGraphData(_data.activities ?? [], _data.connections ?? []);
        setGraphData(data);
    };

    const loadData = async (id: string) => {
        setLoading(true);

        // defintion
        const result = await getWorkflowDefinitionDefinition(id);
        setData(result);
        // const result = await getWorkflowDefinitionVersion(definitionId, version);

        if (!result) {
            Modal.error({
                title: intl.formatMessage({ id: 'common.dict.error.tips' }),
                content: intl.formatMessage({ id: 'page.definition.notFound' }),
                onOk: () => {
                    history.goBack();
                },
            });
            return;
        }

        setTitle(result.displayName);

        // version
        await loadVersion(id, result.latestVersion!);

        // all version
        const versionList = await getWorkflowDefinitionVersions(id, { maxResultCount: 1000 });
        setVersionList(versionList?.items ?? []);
        //
        setLoading(false);
    };

    const handleSwitchVersion = async (version: number) => {
        setLoading(true);
        await loadVersion(id!, version);
        setLoading(false);
    };

    useEffect(() => {
        const sid = params.id ?? '';
        if (!sid) {
            history.goBack();
            return;
        }

        setId(sid);
        loadData(sid);
    }, [0]);

    return (
        <PageContainer
            title={title}
            extra={
                <Space>
                    {access['ElsaModule.Definitions.Publish'] ? (
                        <Button
                            type="primary"
                            disabled={!data}
                            onClick={() => history.push(`/designer?id=${data!.id}`)}
                        >
                            {intl.formatMessage({ id: 'page.definition.designer' })}
                        </Button>
                    ) : (
                        <></>
                    )}
                </Space>
            }
        >
            <Card
                loading={loading}
                title={<Tag>{`version: ${versionData?.version ?? '-'}`}</Tag>}
                extra={
                    <Dropdown
                        menu={{
                            items: versionList.map((x) => {
                                return { label: x.version!, key: x.version! };
                            }),
                            onClick: async (item) => {
                                await handleSwitchVersion(item.key as any as number);
                            },
                        }}
                    >
                        <a onClick={(e) => e.preventDefault()}>
                            <Space>
                                {intl.formatMessage({ id: 'page.definition.switchVersion' })}
                                <DownOutlined />
                            </Space>
                        </a>
                    </Dropdown>
                }
            >
                <Flow
                    readonly
                    actionRef={flowAction}
                    showMiniMap={false}
                    showNodeTypes={false}
                    showToolbar={false}
                    graphData={graphData}
                />
            </Card>
        </PageContainer>
    );
};

export default Index;
