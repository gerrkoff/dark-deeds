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
    },
    ignorePatterns: ['build/*'],
}
