import { LoginResultType } from '@/services/enums';
import { loginExternalLoginCallback } from '@/services/Login';
import { Modal, Spin } from 'antd';
import { useEffect, useState } from 'react';
import { useHistory, useIntl, useLocation, useModel } from 'umi';
import styles from './index.less';

const LoginCallback: React.FC = () => {
    const { refresh } = useModel('@@initialState');

    const intl = useIntl();
    const history = useHistory();
    const location = useLocation();

    const [loading, setLoading] = useState<boolean>(false);

    const load = async () => {
        const remoteError = location.query?.remoteError ?? '';

        const result = await loginExternalLoginCallback(
            { remoteError: remoteError },
            { passError: true },
        );

        if (result?.result == LoginResultType.Success) {
            refresh();
            history.replace('/');
        } else {
            const msg = result?.description;

            if (result?.error?.message) {
                msg = result?.error?.message;
            }

            Modal.error({
                title: intl.formatMessage({ id: 'common.dict.error.tips' }),
                content: msg,
                onOk: () => {
                    history.replace('/auth/login');
                },
            });
        }
    };

    useEffect(() => {
        load();
    }, [0]);

    return (
        <div className={styles.container} style={{ paddingTop: '100px' }}>
            <Spin spinning />
        </div>
    );
};

export default LoginCallback;
