// @ts-ignore
/* eslint-disable */

import { TablePaginationConfig } from "antd";
import { SortOrder } from "antd/es/table/interface";

declare namespace GlobalAPI {

    // type CurrentUser = {
    //     isAuthenticated: boolean,
    //     name?: string;
    //     surName?: string;
    //     userName?: string;
    // }

    type NoticeIconList = {
        data?: NoticeIconItem[];
        /** 列表的内容总数 */
        total?: number;
        success?: boolean;
    };

    type NoticeIconItemType = 'notification' | 'message' | 'event';

    type NoticeIconItem = {
        id?: string;
        extra?: string;
        key?: string;
        read?: boolean;
        avatar?: string;
        title?: string;
        status?: string;
        datetime?: string;
        description?: string;
        type?: NoticeIconItemType;
    };

    type TableQueryConfig = {
        pagination?: TablePaginationConfig | undefined,
        sort?: Record<string, SortOrder> | null,
        filter?: any | undefined,
    }
}
