import { loginLogout } from '@/services/Login';
import { LogoutOutlined, SettingOutlined, UserOutlined } from '@ant-design/icons';
import { Avatar, Menu, Spin } from 'antd';
import type { MenuInfo } from 'rc-menu/lib/interface';
import React, { useCallback } from 'react';
import { history, useModel } from 'umi';
import HeaderDropdown from '../HeaderDropdown';
import styles from './index.less';

export type GlobalHeaderRightProps = {
    menu?: boolean;
};

const loginOut = async () => {
    await loginLogout();
    if (window.location.pathname !== '/auth/login') {
        history.replace({
            pathname: '/auth/login',
        });
    }
};

const AvatarDropdown: React.FC<GlobalHeaderRightProps> = ({ menu }) => {
    const { initialState, setInitialState } = useModel('@@initialState');

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

    const menuHeaderDropdown = (
        <Menu className={styles.menu} selectedKeys={[]} onClick={onMenuClick}>
            {menu && (
                <Menu.Item key="center">
                    <UserOutlined />
                    个人中心
                </Menu.Item>
            )}
            {menu && (
                <Menu.Item key="settings">
                    <SettingOutlined />
                    个人设置
                </Menu.Item>
            )}
            {menu && <Menu.Divider />}

            <Menu.Item key="changepassword">
                <SettingOutlined />
                修改密码
            </Menu.Item>
            <Menu.Divider />

            <Menu.Item key="logout">
                <LogoutOutlined />
                退出登录
            </Menu.Item>
        </Menu>
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
