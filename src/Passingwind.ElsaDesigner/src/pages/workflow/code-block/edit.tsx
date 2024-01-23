import MonacorEditorInput from '@/pages/designer/form-input/monacor-editor-input';
import { createGlobalCode, getGlobalCode, updateGlobalCode } from '@/services/GlobalCode';
import type { API } from '@/services/typings';
import type { ProFormInstance } from '@ant-design/pro-components';
import {
    PageContainer,
    ProCard,
    ProForm,
    ProFormItem,
    ProFormRadio,
    ProFormText,
    ProFormTextArea,
} from '@ant-design/pro-components';
import { useAsyncEffect } from 'ahooks';
import { Button, message } from 'antd';
import { useEffect, useRef, useState } from 'react';
import { useAccess, useHistory, useIntl, useLocation, useParams } from 'umi';

const languages = {
    0: 'javascript',
    1: 'csharp',
};

const Edit: React.FC = () => {
    const location = useLocation();
    const history = useHistory();
    const params = useParams();
    const intl = useIntl();
    const access = useAccess();

    const [loading, setLoading] = useState<boolean>();

    const formRef = useRef<ProFormInstance>();

    const [id, setId] = useState<string>();
    const [language, setLanguage] = useState(languages[0]);

    const [data, setData] = useState<API.GlobalCode>();

    const handleUpdate = async (values: any, publish: boolean) => {
        await formRef.current?.validateFields();
        setLoading(true);
        if (!id) {
            const result = await createGlobalCode(values);

            if (result?.id) {
                setId(result.id);
                message.success(intl.formatMessage({ id: 'common.dict.save.success' }));
                history.replace('/workflows/code-block/edit/' + result.id);
            }
        } else {
            const result = await updateGlobalCode(id!, { ...values, publish: publish });

            if (result?.id) {
                message.success(intl.formatMessage({ id: 'common.dict.save.success' }));
            }
        }
        setLoading(false);
    };

    useAsyncEffect(async () => {
        if (!id) return;

        const result = await getGlobalCode(id);
        if (result) {
            formRef.current?.setFieldsValue(result);
            setData(result);
        }
    }, [id]);

    useEffect(() => {
        // @ts-ignore
        const _id = params?.id ?? '';

        if (_id) {
            setId(_id);
        }
    }, [0]);

    return (
        <PageContainer
            title={intl.formatMessage({ id: id ? 'common.dict.edit' : 'common.dict.create' })}
        >
            <ProCard>
                <ProForm<API.GlobalCodeCreateOrUpdate>
                    initialValues={{ type: 0, language: 0 }}
                    onValuesChange={(values) => {
                        if (values.language >= 0) {
                            setLanguage(languages[values.language]);
                        }
                    }}
                    formRef={formRef}
                    submitter={{
                        render: (props, doms) => {
                            return [
                                <Button
                                    key="publish"
                                    type="primary"
                                    loading={loading}
                                    onClick={async () => {
                                        await handleUpdate(props.form?.getFieldsValue(), true);
                                    }}
                                >
                                    {intl.formatMessage({ id: 'page.globalCode.publish' })}
                                </Button>,
                                <Button
                                    key="save"
                                    loading={loading}
                                    onClick={async () => {
                                        await handleUpdate(props.form?.getFieldsValue(), false);
                                    }}
                                >
                                    {intl.formatMessage({ id: 'page.globalCode.save' })}
                                </Button>,
                            ];
                        },
                    }}
                >
                    <ProFormText
                        label={intl.formatMessage({ id: 'page.globalCode.field.name' })}
                        name="name"
                        rules={[{ required: true }, { max: 32 }]}
                        width="md"
                    />
                    <ProFormTextArea
                        label={intl.formatMessage({ id: 'page.globalCode.field.description' })}
                        name="description"
                        rules={[{ max: 128 }]}
                        width="lg"
                        fieldProps={{ rows: 1 }}
                    />
                    <ProFormRadio.Group
                        label={intl.formatMessage({ id: 'page.globalCode.field.type' })}
                        name="type"
                        options={[
                            { value: 0, label: 'Action' },
                            { value: 1, label: 'Condition' },
                        ]}
                    />
                    <ProFormRadio.Group
                        label={intl.formatMessage({ id: 'page.globalCode.field.language' })}
                        name="language"
                        options={[
                            { value: 0, label: 'Javascript' },
                            { value: 1, label: 'CSharp' },
                        ]}
                    />

                    <ProFormItem
                        label={intl.formatMessage({ id: 'page.globalCode.field.content' })}
                        name="content"
                    >
                        <MonacorEditorInput id="global-code" height={380} language={language} />
                    </ProFormItem>
                </ProForm>
            </ProCard>
        </PageContainer>
    );
};

export default Edit;
