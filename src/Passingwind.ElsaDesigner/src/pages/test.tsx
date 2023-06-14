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
import { setDiagnosticsOptions } from 'monaco-yaml';

const Test: React.FC = () => {
    const [error, setError] = useState(false);

    const checkError = (marks) => {
        console.log(marks);
        setError(marks.length > 0);
    };

    useEffect(() => {
        setError(false);
        setDiagnosticsOptions({
            validate: true,
            enableSchemaRequest: true,
            format: true,
            hover: true,
            completion: true,
            schemas: [
                {
                    uri: 'http://myserver/foo-schema.json',
                    fileMatch: ['*'],
                    schema: {
                        id: 'http://myserver/foo-schema.json',
                        type: 'object',
                        properties: {
                            boolean: {
                                description: 'Are boolean supported?',
                                type: 'boolean',
                            },
                            enum: {
                                description: 'Pick your starter',
                                enum: ['Bulbasaur', 'Squirtle', 'Charmander', 'Pikachu'],
                            },
                        },
                        required: ['boolean', 'enum'],
                    },
                },
            ],
        });
    }, []);

    return (
        <PageContainer>
            <Card>
                <Button>result: {error.toString()}</Button>
                <MonacoEditor language="yaml" height={500} onValidate={checkError} />
            </Card>
        </PageContainer>
    );
};

export default Test;
