module.exports = {
    extends: ["react-app"],
    rules: {
        "no-mixed-operators": 0,
        "jsx-a11y/anchor-is-valid": 0,
        "@typescript-eslint/no-redeclare": 0, // TODO: fix
    },
    ignorePatterns: [
        "build/*",
    ]
}
