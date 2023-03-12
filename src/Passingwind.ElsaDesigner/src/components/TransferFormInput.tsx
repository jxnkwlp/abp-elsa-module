import type { ProFieldRequestData } from '@ant-design/pro-components';
import type { TransferProps } from 'antd';
import { Spin, Transfer } from 'antd';
import type { TransferItem } from 'antd/lib/transfer';
import React, { useEffect, useState } from 'react';

export type TransferFormItemProps<T> = TransferProps<T> & {
    value?: string[];
    onChange?: (value: string[]) => void;
    request?: ProFieldRequestData;
    height?: string | number;
    panelWidth?: string | number;
};

function TransferFormInput<T>(props: TransferFormItemProps<T>) {
    const { onChange, request } = props;
    //
    const [value, setValue] = useState<string[]>(props.value ?? []);
    const [items, setItems] = useState<TransferItem[]>();
    //
    const [loading, setLoading] = React.useState(false);

    const handleChanged = (targetKeys: string[], _direction, _moveKeys) => {
        setValue(targetKeys);
        onChange?.(targetKeys);
    };

    useEffect(() => {
        let isInitial = true;
        const initial = async () => {
            setLoading(true);
            const result = await request?.({}, props);
            setLoading(false);
            setItems(result ?? []);
        };
        if (request && isInitial) {
            initial();
        }
        return () => {
            isInitial = false;
        };
    }, [0]);

    return (
        <Spin spinning={loading}>
            <Transfer
                dataSource={items}
                {...props}
                targetKeys={value}
                onChange={handleChanged}
                listStyle={{ height: props.height ?? 300, width: props.panelWidth }}
            />
        </Spin>
    );
}

export default TransferFormInput;
