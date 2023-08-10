import { isInteger } from '@antv/util';
import type { SortOrder } from 'antd/lib/table/interface';
import moment from 'moment';
import type { GlobalAPI } from './global';
import { API } from './typings';

export const randString = (prefix: string = '', length: number = 5) => {
    let result = '';
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz';
    const charactersLength = characters.length;
    for (let i = 0; i < length; i++) {
        result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }
    if (prefix) {
        return `${prefix}_${result}`;
    }
    return result;
};

export const showDownloadJsonFile = (fileName: string, json: string) => {
    const blob = new Blob([json], { type: 'application/json' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    window.URL.revokeObjectURL(url);
};

export const showDownloadFile = (fileName: string, data: any, contentType: string = '') => {
    const blob = new Blob([data], { type: contentType });
    console.log(blob);
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    window.URL.revokeObjectURL(url);
};

export const enumToStatus = (value: any) => {
    const result = Object.keys(value)
        .filter((x) => isInteger(x))
        .forEach((x) => {
            return {
                [x]: {
                    text: value[x],
                },
            };
        });
    return result;
};

// export const enumToRecord = (value: any) => {
//     return Object.keys(value)
//         .filter((x) => parseInt(x) >= 0)
//         .map((key) => {
//             const v = parseInt(key);
//             return [v]:value[key]
//         });
// };

export const formatTableSorter = (sorter: Record<string, SortOrder>) => {
    return Object.keys(sorter ?? {})
        .map((x) => {
            return `${x} ${sorter[x] == 'ascend' ? '' : 'desc'}`;
        })
        ?.join(', ');
};

export const saveTableQueryConfig = (tableId: string, params: any) => {
    if (params) window.sessionStorage.setItem('table_query_' + tableId, JSON.stringify(params));
    else window.sessionStorage.removeItem('table_query_' + tableId);
};

export const getTableQueryConfig = (tableId: string) => {
    const json = window.sessionStorage.getItem('table_query_' + tableId) ?? '';
    if (json) return JSON.parse(json) as GlobalAPI.TableQueryConfig;
    else return undefined;
};

export const formatDateTimeToUtc = (value: string) => {
    if (value) {
        if (moment(value).isValid()) {
            return moment(value).utc();
        }
    }
    return value;
};

export const formatUserName = (value: API.IdentityUser) => {
    if (value.name && value.surname) return value.name + ' ' + value.surname;
    else if (value.name) return value.name;
    else if (value.surname) return value.surname;
    else return '';
};
