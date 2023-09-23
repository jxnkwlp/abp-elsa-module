import { getWorkflowDefinitionDefinition } from '@/services/WorkflowDefinition';
import type { API } from '@/services/typings';
import { PageContainer } from '@ant-design/pro-layout';
import { useAsyncEffect } from 'ahooks';
import { Alert, Button, Col, Modal, Row, Space } from 'antd';
import React from 'react';
import { Access, useAccess, useHistory, useIntl, useParams } from 'umi';
import { Basic } from './detail-basic';
import { ErrorList } from './detail-errorlist';
import { LatestList } from './detail-latestlist';
import { StatisticsDateCount } from './detail-statistics-date-count';
import { StatisticsStatusCount } from './detail-statistics-state-count';
import { VersionGraph } from './detail-version-graph';

import './detail.less';

const Index: React.FC = () => {
    const history = useHistory();
    const params = useParams();
    const intl = useIntl();
    const access = useAccess();

    const [id, setId] = React.useState<string>();
    const [loading, setLoading] = React.useState<boolean>(false);

    const [title, setTitle] = React.useState<string>();
    const [data, setData] = React.useState<API.WorkflowDefinition>();
    const loadData = async (id: string) => {
        setLoading(true);

        // defintion
        const result = await getWorkflowDefinitionDefinition(id);
        setData(result);

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

        setLoading(false);
    };

    useAsyncEffect(async () => {
        // @ts-ignore
        const sid = params.id ?? '';
        if (!sid) {
            history.goBack();
            return;
        }

        setId(sid);
        await loadData(sid);
    }, [0]);

    return (
        <PageContainer
            title={title}
            extra={
                <Space>
                    {access['ElsaWorkflow.Definitions.CreateOrUpdateOrPublish'] ? (
                        <>
                            <Button
                                key="designer"
                                type="primary"
                                disabled={!data}
                                onClick={() => history.push(`/designer?id=${data!.id}`)}
                            >
                                {intl.formatMessage({ id: 'page.definition.designer' })}
                            </Button>
                        </>
                    ) : (
                        <></>
                    )}
                </Space>
            }
            loading={loading}
        >
            {id && data && <Basic id={id} data={data} />}

            <Access
                accessible={access['ElsaWorkflow.Instances.Statistics']}
                fallback={
                    <Alert
                        type="error"
                        message={intl.formatMessage(
                            { id: 'common.noaccess' },
                            { permission: 'ElsaWorkflow.Instances.Statistics' },
                        )}
                    />
                }
            >
                <Row gutter={12}>
                    <Col span={8}>{id && <StatisticsDateCount id={id} />}</Col>
                    <Col span={8}>{id && <StatisticsStatusCount id={id} />}</Col>
                    <Col span={8}>{id && <ErrorList id={id} />}</Col>
                </Row>

                {id && <LatestList id={id} />}
            </Access>

            {id && <VersionGraph id={id} />}
        </PageContainer>
    );
};

export default Index;
