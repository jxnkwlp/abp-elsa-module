export interface INodePropertyExpressionsEvalProvider {
    type: string;
    eval: (value: string) => any;
    toString: (obj: any) => string;
}

export class NodePropertyExpressionsEvalFactory {
    private static _prviders: Record<string, INodePropertyExpressionsEvalProvider> = {};

    public static register(provider: INodePropertyExpressionsEvalProvider) {
        this._prviders[provider.type] = provider;
    }

    public static eval(type: string, value: string): any | undefined {
        const result = this._prviders?.[type]?.eval(value);
        return result ?? undefined;
    }

    public static toString(type: string, value: any | undefined): string | undefined {
        const result = this._prviders?.[type]?.toString(value);
        return result ?? undefined;
    }

    public static evalExpression(source: Record<string, string>): Record<string, any> {
        return Object.fromEntries(Object.entries(source).map(([key, value]) => [key, this.eval(key, value)]));
    }

    public static expressionToString(source: Record<string, any>): Record<string, string | undefined> {
        return Object.fromEntries(Object.entries(source).map(([key, value]) => [key, this.toString(key, value)]));
    }

}

export class JSONExpressionsEvalProvider implements INodePropertyExpressionsEvalProvider {
    type = 'JSON';
    eval(value: string) {
        return undefined;
    }
    toString(obj: any) {
        return '';
    }
}


NodePropertyExpressionsEvalFactory.register(new JSONExpressionsEvalProvider());
