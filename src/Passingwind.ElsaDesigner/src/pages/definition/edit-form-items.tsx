import { WorkflowContextFidelity, WorkflowPersistenceBehavior } from '@/services/enums';
import ProForm, {
    ProFormDependency,
    ProFormSelect,
    ProFormSwitch,
    ProFormText,
    ProFormTextArea,
} from '@ant-design/pro-form';
import { Tabs } from 'antd';
import MonacorEditorInput from '../designer/form-input/monacor-editor-input';

const EditFormItems: React.FC = () => {
    return (
        <>
            <Tabs defaultActiveKey="1" type="line">
                <Tabs.TabPane tab="Basic" key="1">
                    <ProFormText
                        label="Name"
                        name="name"
                        rules={[
                            { required: true },
                            { max: 32 },
                            {
                                pattern: /^[\u4e00-\u9fa5A-Za-z0-9\-\_]*$/,
                                message: 'Invalid characters',
                            },
                        ]}
                    />
                    <ProFormText
                        label="Display Name"
                        name="displayName"
                        rules={[{ required: true }, { max: 64 }]}
                    />
                    <ProFormTextArea
                        label="Description"
                        name="description"
                        fieldProps={{ rows: 2, autoSize: { minRows: 2, maxRows: 5 } }}
                        rules={[{ max: 128 }]}
                    />
                </Tabs.TabPane>
                <Tabs.TabPane tab="Variables" key="2">
                    {/* <ProFormTextArea
                        name="variablesString"
                        fieldProps={{ rows: 5, autoSize: { minRows: 5, maxRows: 10 } }}
                        rules={[
                            { type: 'string', pattern: /^{.*}$/, message: 'json format error' },
                        ]}
                    /> */}
                    <ProForm.Item name="variablesString">
                        <MonacorEditorInput language="json" height={300} />
                    </ProForm.Item>
                </Tabs.TabPane>
                <Tabs.TabPane tab="Workflow Context" key="3">
                    <ProFormText
                        label="Type"
                        name={['contextOptions', 'type']}
                        rules={[{ required: false }, { max: 128 }]}
                        placeholder="The fully qualified workflow context type name."
                    />
                    <ProFormSelect
                        label="Fidelity"
                        name={['contextOptions', 'contextFidelity']}
                        request={async () => {
                            return Object.keys(WorkflowContextFidelity)
                                .filter((x) => parseInt(x) >= 0)
                                .map((key) => {
                                    return {
                                        value: parseInt(key),
                                        label: WorkflowContextFidelity[key],
                                    };
                                });
                        }}
                        placeholder="The workflow context refresh fidelity controls the behavior of when to load and persist the workflow context."
                    />
                </Tabs.TabPane>
                <Tabs.TabPane tab="Advanced" key="4">
                    <ProFormText
                        label="Tag"
                        name="tag"
                        rules={[{ required: false }, { max: 64 }]}
                    />
                    <ProFormText
                        label="Channel"
                        name="channel"
                        rules={[{ required: false }, { max: 64 }]}
                    />
                    <ProFormSelect
                        label="Persistence Behavior"
                        name="persistenceBehavior"
                        request={async () => {
                            return Object.keys(WorkflowPersistenceBehavior)
                                .filter((x) => parseInt(x) >= 0)
                                .map((key) => {
                                    return {
                                        value: parseInt(key),
                                        label: WorkflowPersistenceBehavior[key],
                                    };
                                });
                        }}
                        initialValue={WorkflowPersistenceBehavior.WorkflowBurst}
                    />
                    <ProFormSwitch label="Singleton" name="isSingleton" />
                </Tabs.TabPane>
            </Tabs>
        </>
    );
};

export default EditFormItems;
