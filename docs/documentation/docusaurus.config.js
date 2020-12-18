module.exports = {
    title: 'checkout-sdk-net',
    tagline: 'Checkout.com SDK for .Net',
    url: 'https://checkout.github.io',
    baseUrl: '/checkout-sdk-net/',
    favicon: 'img/favicon.png',
    organizationName: 'checkout', // Usually your GitHub org/user name.
    projectName: 'checkout-sdk-net', // Usually your repo name.
    scripts: ['https://embed.runkit.com'],
    themeConfig: {
        navbar: {
            title: 'checkout-sdk-net',
            logo: {
                alt: 'cko-dotnet',
                src: 'img/logo.png',
            },
            links: [
                {
                    to: 'https://docs.checkout.com/',
                    activeBasePath: 'docs',
                    label: 'Docs',
                    position: 'right',
                },
                // {
                //     to: 'playground',
                //     activeBasePath: 'playground',
                //     label: 'Playground',
                //     position: 'right',
                // },
                {
                    href: 'https://github.com/checkout/checkout-sdk-net',
                    label: 'GitHub',
                    position: 'right',
                },
            ],
        },
        footer: {
            style: 'dark',
            copyright: `© ${new Date().getFullYear()} Checkout.com    `,
        },
        googleAnalytics: {
            trackingID: 'UA-165971486-1',
        },
    },
    presets: [
        [
            '@docusaurus/preset-classic',
            {
                docs: {
                    sidebarPath: require.resolve('./sidebars.js'),
                    routeBasePath: '',
                    // editUrl: 'https://github.com/facebook/docusaurus/edit/master/website/'
                },
                theme: {
                    customCss: require.resolve('./src/css/custom.css'),
                },
            },
        ],
    ],
    plugins: [
        // Basic usage.
        require.resolve('@docusaurus/plugin-google-analytics'),
    ],
};
