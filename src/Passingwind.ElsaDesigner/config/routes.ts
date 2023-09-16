export default [
    {
        path: '/auth',
        layout: false,
        access: 'public',
        routes: [
            {
                name: 'login',
                path: '/auth/login',
                component: './auth/login',
                access: 'public',
            },
            {
                name: 'login',
                path: '/auth/login/external/callback',
                component: './auth/login/callback',
                access: 'public',
            },
            {
                component: './404',
            },
        ],
    },
    {
        path: '/',
        redirect: '/dashboard',
        access: 'public',
    },
    //
    {
        name: 'Dashboard',
        icon: 'DashboardOutlined',
        path: '/dashboard',
        component: './dashboard',
        access: 'public',
    },
    {
        name: 'Instances',
        icon: 'ClusterOutlined',
        path: '/instances',
        component: './instance',
        access: 'ElsaWorkflow.Instances',
    },
    {
        name: 'InstanceDetail',
        path: '/instances/:id',
        component: './instance/detail',
        hideInMenu: true,
        access: 'ElsaWorkflow.Instances',
    },
    {
        name: 'Definitions',
        icon: 'BranchesOutlined',
        path: '/definitions',
        component: './definition',
        access: 'ElsaWorkflow.Definitions',
    },
    {
        name: 'DefinitionDetail',
        path: '/definitions/:id',
        component: './definition/detail',
        hideInMenu: true,
        access: 'ElsaWorkflow.Definitions',
    },
    {
        name: 'Designer',
        icon: 'DeploymentUnitOutlined',
        path: '/designer',
        component: './designer',
        access: 'ElsaWorkflow.Definitions.CreateOrUpdateOrPublish',
    },
    {
        path: '/workflows',
        name: 'Workflows',
        icon: 'ToolOutlined',
        access: 'public',
        routes: [
            {
                path: '/workflows/variables',
                name: 'Variables',
                component: './workflow/variables',
                access: 'ElsaWorkflow.GlobalVariables',
            },
            {
                path: '/workflows/teams',
                name: 'Teams',
                component: './workflow/teams',
                access: 'ElsaWorkflow.WorkflowTeams',
            },
            {
                path: '/workflows/group',
                name: 'Groups',
                component: './workflow/groups',
                access: 'ElsaWorkflow.WorkflowGroups',
            },
            {
                component: './404',
            },
        ],
    },
    //
    {
        path: '/account',
        name: 'account',
        hideInMenu: true,
        access: 'public',
        routes: [
            {
                path: '/account/change-password',
                name: 'change-password',
                component: './profile/changepassword',
                access: 'public',
            },
            {
                path: '/account/apikeys',
                name: 'apikeys',
                component: './security/apikey',
                access: 'public',
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
        access: 'public',
        routes: [
            {
                path: 'users',
                name: 'users',
                component: './user',
                access: 'AbpIdentity.Users',
            },
            {
                path: 'roles',
                name: 'roles',
                component: './role',
                access: 'AbpIdentity.Roles',
            },
            {
                path: 'settings',
                name: 'settings',
                component: './setting',
                access: 'public',
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
    //     access: 'public',
    // },

    //
    {
        component: './404',
    },
];
