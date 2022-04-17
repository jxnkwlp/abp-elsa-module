import { isInteger } from '@antv/util';

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

export const ShowDownloadJsonFile = (fileName: string, json: string) => {
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
    console.log(result);
    return result;
};
