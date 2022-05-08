export default [
    {
        path: '/auth',
        layout: false,
        routes: [
            {
                path: '/auth',
                routes: [
                    {
                        name: 'login',
                        path: '/auth/login',
                        component: './auth/login',
                    },
                ],
            },
            {
                component: './404',
            },
        ],
    },
    // {
    //     path: '/welcome',
    //     name: 'welcome',
    //     icon: 'smile',
    //     component: './Welcome',
    // },
    // {
    //     path: '/admin',
    //     name: 'admin',
    //     icon: 'crown',
    //     access: 'canAdmin',
    //     component: './Admin',
    //     routes: [
    //         {
    //             path: '/admin/sub-page',
    //             name: 'sub-page',
    //             icon: 'smile',
    //             component: './Welcome',
    //         },
    //         {
    //             component: './404',
    //         },
    //     ],
    // },
    //
    {
        name: 'Dashboard',
        icon: 'table',
        path: '/dashboard',
        component: './dashboard',
    },
    {
        name: 'Instances',
        icon: 'table',
        path: '/instance',
        component: './instance',
    },
    {
        name: 'Instance Detail',
        icon: 'table',
        path: '/instance/:id',
        component: './instance/detail',
        hideInMenu: true,
    },
    {
        name: 'Definitions',
        icon: 'table',
        path: '/definition',
        component: './definition',
    },
    {
        name: 'Designer',
        icon: 'table',
        path: '/designer',
        component: './designer',
    },
    // {
    //     name: 'TableList',
    //     icon: 'table',
    //     path: '/TableList',
    //     component: './TableList',
    // },
    // {
    //     path: '/test',
    //     name: 'test',
    //     icon: 'smile',
    //     component: './test',
    // },

    {
        path: '/account/changepassword',
        name: 'Change Password',
        icon: 'smile',
        hideInMenu: true,
        component: './profile/changepassword',
    },

    {
        path: '/',
        redirect: '/dashboard',
    },
    {
        component: './404',
    },
];
