import MonacoEditor from '@/components/MonacoEditor';
import { getAbpApplicationConfiguration } from '@/services/AbpApplicationConfiguration';
import { PageContainer, ProForm, ProFormText } from '@ant-design/pro-components';
import { Graph } from '@antv/x6';
import { usePortal } from '@antv/x6-react-shape';
import Editor from '@monaco-editor/react';
import { Button, Card } from 'antd';
import React, { useCallback, useRef } from 'react';
import { useEffect, useState } from 'react';
import { useIntl } from 'umi';
import MonacorEditorInput from './designer/form-input/monacor-editor-input';

const Test: React.FC = () => {
    return (
        <PageContainer>
            <Card></Card>
        </PageContainer>
    );
};

export default Test;
