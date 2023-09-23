import {
    getWorkflowDefinition,
    getWorkflowDefinitionVersion,
    getWorkflowDefinitionVersions,
} from '@/services/WorkflowDefinition';
import type { API } from '@/services/typings';
import { DownOutlined } from '@ant-design/icons';
import { useAsyncEffect } from 'ahooks';
import { Card, Dropdown, Modal, Space, Tag } from 'antd';
import React, { useRef } from 'react';
import { useIntl } from 'umi';
import type { FlowActionType } from '../designer/flow';
import Flow from '../designer/flow';
import { conventToGraphData } from '../designer/service';
import type { IGraphData } from '../designer/type';

export const VersionGraph: React.FC<{ id: string }> = (props) => {
    const intl = useIntl();

    const [id, setId] = React.useState<string>(props.id);
    const [loading, setLoading] = React.useState<boolean>(false);

    const flowAction = useRef<FlowActionType>();

    const [versionData, setVersionData] = React.useState<API.WorkflowDefinitionVersion>();
    const [graphData, setGraphData] = React.useState<IGraphData>();
    const [versionList, setVersionList] = React.useState<API.WorkflowDefinitionVersionListItem[]>(
        [],
    );

    const loadVersion = async (version?: number) => {
        let _data: API.WorkflowDefinitionVersion;
        if (version) _data = await getWorkflowDefinitionVersion(id, version);
        else _data = await getWorkflowDefinition(id);

        if (!_data) {
            Modal.error({
                title: intl.formatMessage({ id: 'common.dict.error.tips' }),
                content: intl.formatMessage({ id: 'page.definition.versionNotFound' }),
                onOk: () => {
                    // history.goBack();
                },
            });
            return;
        }

        setVersionData(_data);

        const data = await conventToGraphData(_data.activities ?? [], _data.connections ?? []);
        setGraphData(data);
    };

    const loadVersionList = async () => {
        // all version
        const versionList = await getWorkflowDefinitionVersions(id, { maxResultCount: 1000 });
        setVersionList(versionList?.items ?? []);
    };

    const handleSwitchVersion = async (version: number) => {
        setLoading(true);
        await loadVersion(version);
        setLoading(false);
    };

    useAsyncEffect(async () => {
        await loadVersion();
        await loadVersionList();
    }, [0]);

    return (
        <Card
            loading={loading}
            title={
                <Space>
                    {intl.formatMessage({ id: 'page.definition.graph' })}
                    <Tag>{`version: ${versionData?.version ?? '-'}`}</Tag>
                </Space>
            }
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
    );
};
