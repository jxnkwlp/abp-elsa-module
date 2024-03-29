import { ApiOutlined, SettingOutlined } from '@ant-design/icons';
import { Collapse, Spin, Tooltip } from 'antd';
import React, { useEffect } from 'react';
import { useIntl } from 'umi';
import { getNodeIconByType, getNodeTypeData } from './service';
import type { NodeTypeGroup } from './type';

type NodeTypesPanelProps = {
    onDrag: (type: string, e: any) => void;
};

const NodeTypesPanel: React.FC<NodeTypesPanelProps> = (props) => {
    const [data, setData] = React.useState<NodeTypeGroup[]>([]);
    const [loading, setLoading] = React.useState(false);

    const intl = useIntl();

    const handleStartDrag = (e: { currentTarget: any; preventDefault: () => any }) => {
        const target = e.currentTarget;
        const type = target.getAttribute('data-type');
        props?.onDrag(type!, e);
        return e.preventDefault();
    };

    const load = async () => {
        setLoading(true);
        const result = await getNodeTypeData();
        setLoading(false);
        setData(result);
    };

    useEffect(() => {
        load();
    }, []);

    return (
        <div className="dnd-box">
            {/* <Collapse defaultActiveKey={['1']}>
                <Collapse.Panel header="This is panel header 1" key="1">
                    <p>32132123</p>
                </Collapse.Panel>
                <Collapse.Panel header="This is panel header 2" key="2">
                    <p>32123</p>
                </Collapse.Panel>
            </Collapse> */}

            <div className="dnd-header">
                {intl.formatMessage({ id: 'page.designer.component' })}
            </div>
            {/* <Spin spinning={loading}> */}
            <Collapse key="groups" collapsible="header" ghost className="dnd-body">
                {data.map((g, index) => {
                    return (
                        <Collapse.Panel header={g.name} key={index}>
                            <ul>
                                {g.children.map((item, index2) => {
                                    return (
                                        <Tooltip
                                            title={item.description ?? item.label}
                                            placement="right"
                                            key={index2}
                                        >
                                            <li
                                                key={index2}
                                                data-type={item.type}
                                                onMouseDown={handleStartDrag}
                                            >
                                                <div className="dnd-rect">
                                                    <div className={`node default`}>
                                                        <span className="icon">
                                                            {getNodeIconByType(item.type) ?? (
                                                                <ApiOutlined />
                                                            )}
                                                        </span>
                                                        <span className="label">{item.label}</span>
                                                    </div>
                                                </div>
                                            </li>
                                        </Tooltip>
                                    );
                                })}
                            </ul>
                        </Collapse.Panel>
                    );
                })}
            </Collapse>
            {/* </Spin> */}
        </div>
    );
};

export default NodeTypesPanel;
