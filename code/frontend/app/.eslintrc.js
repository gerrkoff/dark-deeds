module.exports = {
    extends: ['react-app'],
    rules: {
        'no-mixed-operators': 0,
        'jsx-a11y/anchor-is-valid': 0,
        'no-restricted-imports': [
            'error',
            {
                patterns: ['.*'],
            },
        ],
        'simple-import-sort/imports': 'error',
        'simple-import-sort/exports': 'error',
    },
    ignorePatterns: ['build/*'],
    plugins: ['simple-import-sort'],
}
