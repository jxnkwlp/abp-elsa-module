import { WorkflowPersistenceBehavior } from '@/services/enums';
import { ProFormText, ProFormTextArea, ProFormSwitch, ProFormSelect } from '@ant-design/pro-form';
import { isNumber } from 'lodash';

const EditFormItems: React.FC = () => {
    return (
        <>
            <ProFormText label="Name" name="name" rules={[{ required: true }, { max: 32 }]} />
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
            <ProFormSwitch label="Singleton" name="isSingleton" />
            <ProFormText label="Tag" name="tag" />
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
            {/* <ProFormTextArea
                label="Variables"
                name="Variables"
                fieldProps={{ rows: 2, autoSize: { minRows: 2, maxRows: 5 } }}
            /> */}
        </>
    );
};

export default EditFormItems;
