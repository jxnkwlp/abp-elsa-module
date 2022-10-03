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
        icon: 'table',
        path: '/dashboard',
        component: './dashboard',
    },
    {
        name: 'Instances',
        icon: 'table',
        path: '/instances',
        component: './instance',
    },
    {
        name: 'Instance Detail',
        icon: 'table',
        path: '/instances/:id',
        component: './instance/detail',
        hideInMenu: true,
    },
    {
        name: 'Definitions',
        icon: 'table',
        path: '/definitions',
        component: './definition',
    },
    {
        name: 'Designer',
        icon: 'table',
        path: '/designer',
        component: './designer',
    },
    //
    {
        path: '/account/change-password',
        name: 'Change Password',
        icon: 'smile',
        hideInMenu: true,
        component: './profile/changepassword',
    },
    // //
    // {
    //     path: '/test',
    //     name: 'test page',
    //     icon: 'smile',
    //     component: './test',
    // },

    //
    {
        component: './404',
    },
];
