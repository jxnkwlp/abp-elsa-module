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
                name: 'login',
                path: '/auth/login/external/callback',
                component: './auth/login/callback',
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
        access: 'ElsaModule.Instances'
    },
    {
        name: 'InstanceDetail',
        path: '/instances/:id',
        component: './instance/detail',
        hideInMenu: true,
        access: 'ElsaModule.Instances'
    },
    {
        name: 'Definitions',
        icon: 'BranchesOutlined',
        path: '/definitions',
        component: './definition',
        access: 'ElsaModule.Definitions'
    },
    {
        name: 'DefinitionDetail',
        path: '/definitions/:id',
        component: './definition/detail',
        hideInMenu: true,
    },
    {
        name: 'Designer',
        icon: 'DeploymentUnitOutlined',
        path: '/designer',
        component: './designer',
        access: 'ElsaModule.Definitions.Publish'
    },
    {
        name: 'Variables',
        icon: 'KeyOutlined',
        path: '/variables',
        component: './variables',
        access: 'ElsaModule.GlobalVariables'
    },
    //
    {
        path: '/account',
        name: 'account',
        hideInMenu: true,
        routes: [
            {
                path: '/account/change-password',
                name: 'change-password',
                component: './profile/changepassword',
            },
            {
                path: '/account/apikeys',
                name: 'apikeys',
                component: './security/apikey',
            },
            {
                component: './404',
            },
        ],
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
                access: 'AbpIdentity.Users'
            },
            {
                path: 'roles',
                name: 'roles',
                component: './role',
                access: 'AbpIdentity.Roles'
            },
            {
                path: 'settings',
                name: 'settings',
                component: './setting',
            },
            // {
            //     path: 'audit/logs',
            //     name: 'auditLogs',
            //     component: './audit/logs',
            // },
            // {
            //     path: 'security/logs',
            //     name: 'securityLogs',
            //     component: './security/logs',
            // },
            {
                component: './404',
            },
        ],
    },


    // {
    //     path: '/test',
    //     name: 'test',
    //     icon: 'smile',
    //     component: './test',
    // },

    //
    {
        component: './404',
    },
];
