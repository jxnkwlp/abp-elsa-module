import { WorkflowPersistenceBehavior } from '@/services/enums';
import { ProFormText, ProFormTextArea, ProFormSwitch, ProFormSelect } from '@ant-design/pro-form';

const EditFormItems: React.FC = () => {
    return (
        <>
            <ProFormText label="Name" name="name" rules={[{ required: true }, { max: 16 }]} />
            <ProFormText
                label="Display Name"
                name="displayName"
                rules={[{ required: true }, { max: 16 }]}
            />
            <ProFormTextArea
                label="Description"
                name="description"
                fieldProps={{ rows: 2, autoSize: { minRows: 2, maxRows: 5 } }}
                rules={[{ max: 64 }]}
            />
            <ProFormSwitch label="Singleton" name="isSingleton" />
            <ProFormText label="Tag" name="tag" />
            <ProFormSelect
                label="Persistence Behavior"
                name="persistenceBehavior"
                options={Object.entries(WorkflowPersistenceBehavior).map(([key, value]) => ({
                    label: value,
                    value: key,
                }))}
                initialValue={WorkflowPersistenceBehavior.WorkflowBurst}
            />
            {/* <ProFormTextArea
                label="Variables"
                name="Variables"
                fieldProps={{ rows: 2, autoSize: { minRows: 2, maxRows: 5 } }}
            /> */}
        </>
    );
};

export default EditFormItems;
