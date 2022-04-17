import { Collapse } from 'antd';
import React, { useEffect } from 'react';
import { getNodeTypeData } from './service';
import type { NodeTypeGroup } from './type';

type NodeTypesPanelProps = {
    onDrag: (type: string, e: any) => void;
};

const NodeTypesPanel: React.FC<NodeTypesPanelProps> = (props) => {
    const [data, setData] = React.useState<NodeTypeGroup[]>([]);

    const handleStartDrag = (e: { currentTarget: any; preventDefault: () => any }) => {
        const target = e.currentTarget;
        const type = target.getAttribute('data-type');
        props?.onDrag(type!, e);
        return e.preventDefault();
    };

    const load = async () => {
        const result = await getNodeTypeData();
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

            <div className="dnd-header">组件列表</div>
            <Collapse key="groups" collapsible="header" ghost className="dnd-body">
                {data.map((g, index) => {
                    return (
                        <Collapse.Panel header={g.name} key={index}>
                            <ul>
                                {g.children.map((item, index2) => {
                                    return (
                                        <li
                                            key={index2}
                                            data-type={item.type}
                                            onMouseDown={handleStartDrag}
                                        >
                                            <div className="dnd-rect">{item.label}</div>
                                        </li>
                                    );
                                })}
                            </ul>
                        </Collapse.Panel>
                    );
                })}
            </Collapse>
        </div>
    );
};

export default NodeTypesPanel;
