import { loginLogout } from '@/services/Login';
import { LockOutlined, LogoutOutlined, SettingOutlined, UserOutlined } from '@ant-design/icons';
import { Avatar, Menu, Spin } from 'antd';
import type { ItemType } from 'antd/lib/menu/hooks/useItems';
import { stringify } from 'querystring';
import type { MenuInfo } from 'rc-menu/lib/interface';
import React, { useCallback } from 'react';
import { history, useIntl, useModel } from 'umi';
import HeaderDropdown from '../HeaderDropdown';
import styles from './index.less';

export type GlobalHeaderRightProps = {
    menu?: boolean;
};

/**
 * 退出登录，并且将当前的 url 保存
 */
const loginOut = async () => {
    await loginLogout();
    const { query = {}, search, pathname } = history.location;
    // Note: There may be security issues, please note
    if (window.location.pathname !== '/auth/login') {
        history.replace({
            pathname: '/auth/login',
        });
    }
};

const AvatarDropdown: React.FC<GlobalHeaderRightProps> = ({ menu }) => {
    const { initialState, setInitialState } = useModel('@@initialState');
    const intl = useIntl();

    const onMenuClick = useCallback(
        (event: MenuInfo) => {
            const { key } = event;
            if (key === 'logout') {
                setInitialState((s) => ({ ...s, currentUser: undefined }));
                loginOut();
                return;
            }
            history.push(`/account/${key}`);
        },
        [setInitialState],
    );

    const loading = (
        <span className={`${styles.action} ${styles.account}`}>
            <Spin
                size="small"
                style={{
                    marginLeft: 8,
                    marginRight: 8,
                }}
            />
        </span>
    );

    if (!initialState) {
        return loading;
    }

    const { currentUser } = initialState;

    if (!currentUser || !currentUser.name) {
        return loading;
    }

    const menuItems: ItemType[] = [
        ...(menu
            ? [
                  {
                      key: 'center',
                      icon: <UserOutlined />,
                      label: '个人中心',
                  },
                  {
                      key: 'settings',
                      icon: <SettingOutlined />,
                      label: '个人设置',
                  },
                  {
                      type: 'divider' as const,
                  },
              ]
            : []),
        {
            key: 'change-password',
            icon: <LockOutlined />,
            label: intl.formatMessage({ id: 'app.changepassword' }),
        },
        {
            key: 'logout',
            icon: <LogoutOutlined />,
            label: intl.formatMessage({ id: 'app.logout' }),
        },
    ];

    const menuHeaderDropdown = (
        <Menu className={styles.menu} selectedKeys={[]} onClick={onMenuClick} items={menuItems} />
    );

    return (
        <HeaderDropdown overlay={menuHeaderDropdown}>
            <span className={`${styles.action} ${styles.account}`}>
                <Avatar
                    size="small"
                    className={styles.avatar}
                    icon={<UserOutlined />}
                    alt="avatar"
                />
                <span className={`${styles.name} anticon`}>{currentUser.name}</span>
            </span>
        </HeaderDropdown>
    );
};

export default AvatarDropdown;
