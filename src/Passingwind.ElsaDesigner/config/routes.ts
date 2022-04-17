export default [
    // {
    //     path: '/user',
    //     layout: false,
    //     routes: [
    //         {
    //             path: '/user',
    //             routes: [
    //                 {
    //                     name: 'login',
    //                     path: '/user/login',
    //                     component: './user/Login',
    //                 },
    //             ],
    //         },
    //         {
    //             component: './404',
    //         },
    //     ],
    // },
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
        name: 'dashboard',
        icon: 'table',
        path: '/dashboard',
        component: './dashboard',
    },
    {
        name: 'instance',
        icon: 'table',
        path: '/instance',
        component: './instance',
    },
    {
        name: 'instance details',
        icon: 'table',
        path: '/instance/:id',
        component: './instance/detail',
        hideInMenu: true,
    },
    {
        name: 'definition',
        icon: 'table',
        path: '/definition',
        component: './definition',
    },
    {
        name: 'designer',
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

    //
    {
        path: '/',
        redirect: '/dashboard',
    },
    {
        component: './404',
    },
];
