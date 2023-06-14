import MonacoEditor from '@/components/MonacoEditor';
import { ModalForm, ProForm } from '@ant-design/pro-components';
import { setDiagnosticsOptions } from 'monaco-yaml';
import { useEffect } from 'react';
import { useIntl } from 'umi';
import YAML from 'yaml';

const VariableForm: React.FC<{
    visible: boolean | undefined;
    data: any;
    onVisibleChange: (visible: boolean) => void;
    onSubmit: (values: any) => Promise<boolean | void>;
}> = (props) => {
    const intl = useIntl();
    const [editForm] = ProForm.useForm();

    const { data, visible, onVisibleChange, onSubmit } = props;

    useEffect(() => {
        if (visible) {
            if (Object.keys(data ?? {}).length == 0) {
                editForm.setFieldsValue({
                    valueString: '#Example\r\nYOU_KEY: YOU_KEY_Value',
                });
            } else {
                editForm.setFieldsValue({
                    valueString: YAML.stringify(data),
                });
            }
        }
    }, [visible]);

    useEffect(() => {
        setDiagnosticsOptions({
            validate: true,
            enableSchemaRequest: true,
            format: true,
            hover: true,
            completion: true,
            schemas: [
                {
                    uri: 'http://json-schema.org/draft-04/schema#',
                    fileMatch: ['*'],
                    // @ts-ignore
                    schema: {
                        type: 'object',
                        properties: {},
                    },
                },
            ],
        });
    }, [0]);

    return (
        <ModalForm<{ valueString: string | undefined }>
            form={editForm}
            layout="horizontal"
            preserve={false}
            width={680}
            modalProps={{ maskClosable: false, destroyOnClose: true }}
            title={intl.formatMessage({ id: 'page.definition.edit.variables' })}
            visible={visible}
            onVisibleChange={onVisibleChange}
            onFinish={async (value) => {
                // TODO
                // const validateResult = new Validator().validate(data, definitionJsonSchema);
                const result = YAML.parse(value.valueString ?? '');
                if (typeof result != 'object') {
                    return false;
                }
                onSubmit?.(result);
                return true;
            }}
        >
            <ProForm.Item name="valueString">
                <MonacoEditor
                    language="yaml"
                    height={430}
                    options={{ minimap: { autohide: false } }}
                    border
                />
            </ProForm.Item>
        </ModalForm>
    );
};

export default VariableForm;
