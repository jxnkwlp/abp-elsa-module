import { createApiKey, deleteApiKey, getApiKeyList } from '@/services/ApiKey';
import type { API } from '@/services/typings';
import { formatDateTimeToUtc, randString } from '@/services/utils';
import type { ActionType, ProColumnType } from '@ant-design/pro-components';
import {
    ModalForm,
    PageContainer,
    ProFormDateTimePicker,
    ProFormSelect,
    ProFormText,
    ProTable,
} from '@ant-design/pro-components';
import { Button, Form, message, Modal, Popconfirm, Space, Tag, Typography } from 'antd';
import moment from 'moment';
import React, { useRef, useState } from 'react';
import { formatMessage, useIntl } from 'umi';

const handleAdd = async (data: any) => {
    const response = await createApiKey(data);
    if (response) {
        message.success(formatMessage({ id: 'common.dict.created.success' }));
        return response;
    }
    return false;
};

const handleDelete = async (id: string) => {
    const response = await deleteApiKey(id);
    if (response?.response?.ok) {
        message.success(formatMessage({ id: 'common.dict.deleted.success' }));
        return true;
    }
    return false;
};

const Index: React.FC = () => {
    const tableActionRef = useRef<ActionType>();
    const intl = useIntl();

    const [editModalVisible, setEditModalVisible] = useState<boolean>(false);
    const [editModalTitle, setEditModalTitle] = useState<string>('');
    const [editModalForm] = Form.useForm();

    const columns: ProColumnType<API.GlobalVariable>[] = [
        {
            dataIndex: 'name',
            title: intl.formatMessage({ id: 'page.apikey.field.name' }),
            search: false,
            copyable: true,
            width: 150,
        },
        {
            dataIndex: 'expirationTime',
            title: intl.formatMessage({ id: 'page.apikey.field.expirationTime' }),
            search: false,
            width: 150,
            // valueType: 'dateTime',
            renderText: (_) => {
                return _ ? (
                    <Space>
                        {moment(_).format()}
                        {moment(_).isBefore(moment()) ? <Tag color="red">Expired</Tag> : null}
                    </Space>
                ) : (
                    '-'
                );
            },
        },
        {
            dataIndex: 'creationTime',
            title: intl.formatMessage({ id: 'common.dict.creationTime' }),
            search: false,
            width: 150,
            valueType: 'dateTime',
        },
        {
            title: intl.formatMessage({ id: 'common.dict.table-action' }),
            valueType: 'option',
            width: 110,
            align: 'center',
            fixed: 'right',
            render: (text, record, _, action) => [
                <Popconfirm
                    key="delete"
                    title={intl.formatMessage({ id: 'common.dict.delete.confirm' })}
                    onConfirm={async () => {
                        if (await handleDelete(record.id!)) {
                            action?.reload();
                        }
                    }}
                >
                    <a>{intl.formatMessage({ id: 'common.dict.delete' })}</a>
                </Popconfirm>,
            ],
        },
    ];

    return (
        <PageContainer>
            <ProTable<API.ApiKey>
                columns={columns}
                actionRef={tableActionRef}
                search={false}
                scroll={{ x: 600 }}
                rowKey="id"
                toolBarRender={() => [
                    <Button
                        key="add"
                        type="primary"
                        onClick={() => {
                            setEditModalVisible(true);
                            setEditModalTitle(intl.formatMessage({ id: 'common.dict.create' }));
                            editModalForm.setFieldsValue({
                                name: 'key-' + randString(),
                                expirationType: 0,
                            });
                        }}
                    >
                        {intl.formatMessage({ id: 'common.dict.create' })}
                    </Button>,
                ]}
                request={async (params) => {
                    const { current, pageSize } = params;
                    delete params.current;
                    delete params.pageSize;
                    const skipCount = (current! - 1) * pageSize!;
                    const result = await getApiKeyList({
                        ...params,
                        skipCount,
                        maxResultCount: pageSize,
                    });
                    if (result)
                        return {
                            success: true,
                            data: result.items,
                            total: result.totalCount,
                        };
                    else {
                        return {
                            success: false,
                        };
                    }
                }}
            />

            <ModalForm
                title={editModalTitle}
                width={650}
                visible={editModalVisible}
                modalProps={{ destroyOnClose: true, maskClosable: false }}
                preserve={false}
                onVisibleChange={setEditModalVisible}
                form={editModalForm}
                onFinish={async (value) => {
                    // console.log(value);
                    value.expirationTime = formatDateTimeToUtc(value.expirationTime);
                    const result = await handleAdd(value);
                    if (result) {
                        setEditModalVisible(false);
                        tableActionRef.current?.reload();
                        //
                        Modal.success({
                            title: intl.formatMessage({ id: 'common.dict.created.success' }),
                            width: 500,
                            content: (
                                <>
                                    <Typography.Paragraph>
                                        {intl.formatMessage({ id: 'page.apikey.tips.p1' })}
                                    </Typography.Paragraph>
                                    <Typography.Paragraph copyable strong>
                                        {result.secret}
                                    </Typography.Paragraph>
                                    <Typography.Paragraph>
                                        {intl.formatMessage({ id: 'page.apikey.tips.p2' })}
                                    </Typography.Paragraph>
                                </>
                            ),
                        });
                    }
                }}
                layout="horizontal"
                labelWrap
                labelCol={{ span: 4 }}
                onValuesChange={(values: {
                    name: string;
                    expirationType: number;
                    expirationTime: string;
                }) => {
                    if (values.expirationType >= 0) {
                        if (values.expirationType == 0)
                            editModalForm.setFieldsValue({
                                expirationTime: null,
                            });
                        else
                            editModalForm.setFieldsValue({
                                expirationTime: moment().add('days', values.expirationType),
                            });
                    }
                }}
            >
                <ProFormText
                    rules={[{ required: true }, { max: 32 }]}
                    name="name"
                    label={intl.formatMessage({ id: 'page.apikey.field.name' })}
                />
                <ProFormSelect
                    rules={[{ required: true }]}
                    name="expirationType"
                    label={intl.formatMessage({ id: 'page.apikey.field.expirationRange' })}
                    options={[
                        {
                            label: intl.formatMessage({
                                id: 'page.apikey.field.expirationRange.r0',
                            }),
                            value: 0,
                        },
                        {
                            label: intl.formatMessage({
                                id: 'page.apikey.field.expirationRange.r1',
                            }),
                            value: 1,
                        },
                        {
                            label: intl.formatMessage({
                                id: 'page.apikey.field.expirationRange.r2',
                            }),
                            value: 7,
                        },
                        {
                            label: intl.formatMessage({
                                id: 'page.apikey.field.expirationRange.r3',
                            }),
                            value: 15,
                        },
                        {
                            label: intl.formatMessage({
                                id: 'page.apikey.field.expirationRange.r4',
                            }),
                            value: 30,
                        },
                        {
                            label: intl.formatMessage({
                                id: 'page.apikey.field.expirationRange.r5',
                            }),
                            value: '60',
                        },
                        {
                            label: intl.formatMessage({
                                id: 'page.apikey.field.expirationRange.r6',
                            }),
                            value: '90',
                        },
                        {
                            label: intl.formatMessage({
                                id: 'page.apikey.field.expirationRange.r7',
                            }),
                            value: '180',
                        },
                        {
                            label: intl.formatMessage({
                                id: 'page.apikey.field.expirationRange.r8',
                            }),
                            value: '365',
                        },
                    ]}
                />
                <ProFormDateTimePicker
                    rules={[{ required: false }]}
                    name="expirationTime"
                    label={intl.formatMessage({
                        id: 'page.apikey.field.expirationTime',
                    })}
                    fieldProps={{
                        disabledDate: (_) => moment().isAfter(_),
                    }}
                />
            </ModalForm>
        </PageContainer>
    );
};

export default Index;
