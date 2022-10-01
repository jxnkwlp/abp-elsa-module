module.exports = {
    extends: [require.resolve('@umijs/fabric/dist/eslint')],
    globals: {
        ANT_DESIGN_PRO_ONLY_DO_NOT_USE_IN_YOUR_PRODUCTION: true,
        page: true,
        REACT_APP_ENV: true,
    },
    rules: {
        'no-shadow': 'off',
        '@typescript-eslint/no-shadow': ['off'],
        '@typescript-eslint/no-unused-vars': ['warn'],
    },
};
