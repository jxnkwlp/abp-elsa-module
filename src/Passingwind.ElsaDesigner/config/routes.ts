export default [
    {
        path: '/auth',
        layout: false,
        routes: [
            {
                name: 'login',
                path: '/auth/login',
                component: './auth/login',
            },
            {
                component: './404',
            },
        ],
    },
    {
        path: '/',
        redirect: '/dashboard',
    },
    //
    {
        name: 'Dashboard',
        icon: 'DashboardOutlined',
        path: '/dashboard',
        component: './dashboard',
    },
    {
        name: 'Instances',
        icon: 'ClusterOutlined',
        path: '/instances',
        component: './instance',
    },
    {
        name: 'Instance Detail',
        path: '/instances/:id',
        component: './instance/detail',
        hideInMenu: true,
    },
    {
        name: 'Definitions',
        icon: 'BranchesOutlined',
        path: '/definitions',
        component: './definition',
    },
    {
        name: 'Variables',
        icon: 'KeyOutlined',
        path: '/variables',
        component: './variables',
    },
    {
        name: 'Designer',
        icon: 'DeploymentUnitOutlined',
        path: '/designer',
        component: './designer',
    },

    //
    {
        path: '/account/change-password',
        name: 'Change Password',
        hideInMenu: true,
        component: './profile/changepassword',
    },
    //
    {
        path: '/system',
        name: 'system',
        icon: 'SettingOutlined',
        hideInBreadcrumb: true,
        routes: [
            {
                path: 'users',
                name: 'users',
                component: './user',
            },
            {
                path: 'roles',
                name: 'roles',
                component: './role',
            },
            {
                path: 'settings',
                name: 'settings',
                component: './setting',
            },
            {
                component: './404',
            },
        ],
    },

    //
    {
        path: '/test',
        name: 'test page',
        icon: 'smile',
        component: './test',
    },

    //
    {
        component: './404',
    },
];
