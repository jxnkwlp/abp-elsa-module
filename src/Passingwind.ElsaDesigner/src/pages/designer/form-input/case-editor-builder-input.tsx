import ProForm, {
    ModalForm,
    ProFormDependency,
    ProFormGroup,
    ProFormList,
    ProFormSelect,
    ProFormText,
} from '@ant-design/pro-form';
import { Button, Col, Form, Row } from 'antd';
import React, { useEffect } from 'react';
import { getEditorLanguage } from '../service';
import MonacorEditorInput from './monacor-editor-input';

type CaseEditorInputProps = {
    value?: string;
    onChange?: (value: string) => void;
    // render?: () => React.ReactNode;
    title?: string;
};

const CaseEditorInput: React.FC<CaseEditorInputProps> = (props) => {
    const [data, setData] = React.useState<any>([]);
    const [caseEditVisible, setCaseEditVisible] = React.useState<boolean>(false);
    const [caseEditTitle, setCaseEditTitle] = React.useState<string>(props?.title ?? 'Cases Edit');
    const [caseEditForm] = Form.useForm();

    useEffect(() => {
        setData(JSON.parse(props?.value ?? '[]'));
    }, [caseEditForm, props]);

    return (
        <>
            <Row>
                <Col flex={1}>
                    <div style={{ wordBreak: 'break-all' }}>{JSON.stringify(data ?? [])}</div>
                </Col>
                <Col>
                    <Button
                        type="default"
                        onClick={() => {
                            setCaseEditVisible(true);
                            caseEditForm.setFieldsValue({ input: data });
                        }}
                    >
                        Configure
                    </Button>
                </Col>
            </Row>

            <ModalForm
                form={caseEditForm}
                title={caseEditTitle}
                width={680}
                layout="horizontal"
                labelCol={{ span: 4 }}
                modalProps={{ destroyOnClose: true, maskClosable: false }}
                visible={caseEditVisible}
                onVisibleChange={setCaseEditVisible}
                initialValues={data}
                onFinish={async (formValue) => {
                    setData(formValue?.input ?? []);
                    props.onChange?.(JSON.stringify(formValue?.input ?? []));
                    return true;
                }}
                grid
            >
                <ProFormList
                    name="input"
                    itemContainerRender={(doms) => {
                        return <ProFormGroup>{doms}</ProFormGroup>;
                    }}
                    min={1}
                    actionGuard={{
                        beforeAddRow: (value) => {
                            value = { syntax: 'JavaScript' };
                            return true;
                        },
                    }}
                >
                    {(meta, index, action) => {
                        return (
                            <>
                                <ProFormText
                                    name={['name']}
                                    label="Name"
                                    placeholder="Name"
                                    rules={[{ required: true }, { max: 64 }]}
                                    colProps={{ span: 16 }}
                                />
                                {/* <Col span={16}>
                                // import !!! must use ProForm.Item
                                    <ProForm.Item name={['name']} label="Name">
                                        <MonacorEditorInput height={30} />
                                    </ProForm.Item>
                                </Col> */}
                                <ProFormSelect
                                    name={['syntax']}
                                    label="Syntax"
                                    options={[
                                        {
                                            label: 'JavaScript',
                                            value: 'JavaScript',
                                        },
                                        {
                                            label: 'Liquid',
                                            value: 'Liquid',
                                        },
                                    ]}
                                    rules={[{ required: true }]}
                                    allowClear={false}
                                    // initialValue="JavaScript"
                                    colProps={{ span: 8 }}
                                />
                                <Col span={24}>
                                    <ProFormDependency name={['syntax']}>
                                        {({ syntax }) => {
                                            // console.log(syntax);
                                            // const itemValue =
                                            //     action.getCurrentRowData()?.expressions?.[syntax] ??
                                            //     '';
                                            // console.log(itemValue);
                                            return syntax ? (
                                                <ProForm.Item
                                                    // label="Expressions"
                                                    name={['expressions', syntax]}
                                                    rules={[
                                                        { required: true, message: 'Required' },
                                                    ]}
                                                >
                                                    <MonacorEditorInput
                                                        height={150}
                                                        // value={itemValue}
                                                        language={getEditorLanguage(syntax)}
                                                    />
                                                </ProForm.Item>
                                            ) : (
                                                <span />
                                            );
                                        }}
                                    </ProFormDependency>
                                    <div
                                        style={{
                                            borderBottom: '1px #eee solid',
                                            height: '1px',
                                            marginBottom: '15px',
                                        }}
                                        className="ant-col ant-col-24"
                                    />
                                </Col>
                            </>
                        );
                    }}
                </ProFormList>
            </ModalForm>
        </>
    );
};

export default CaseEditorInput;
