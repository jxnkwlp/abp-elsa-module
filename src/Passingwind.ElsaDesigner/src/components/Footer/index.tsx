import { GithubOutlined } from '@ant-design/icons';
import { DefaultFooter } from '@ant-design/pro-components';
import { useIntl } from 'umi';

const Footer: React.FC = () => {
    const intl = useIntl();
    const defaultMessage = intl.formatMessage({
        id: 'app.copyright.produced',
    });

    const currentYear = new Date().getFullYear();

    return (
        <DefaultFooter
            copyright={`${currentYear} ${defaultMessage}`}
            links={[
                {
                    key: 'github',
                    title: <GithubOutlined />,
                    href: 'https://github.com/jxnkwlp/abp-elsa-module',
                    blankTarget: true,
                },
            ]}
        />
    );
};

export default Footer;
