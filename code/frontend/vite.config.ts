import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import { VitePWA } from 'vite-plugin-pwa'
import type { RollupLog, WarningHandlerWithDefault } from 'rollup'

const signalRUtilsPath = '/node_modules/@microsoft/signalr/dist/esm/Utils.js'
const signalRAnnotationLines = new Set([189, 207])

function isKnownSignalRAnnotationWarning(warning: RollupLog): boolean {
    const normalizedId = warning.id?.replaceAll('\\', '/')

    return (
        warning.code === 'INVALID_ANNOTATION' &&
        normalizedId?.endsWith(signalRUtilsPath) === true &&
        warning.loc?.column === 0 &&
        signalRAnnotationLines.has(warning.loc.line) &&
        warning.message.includes('"/*#__PURE__*/"') &&
        warning.message.includes(
            'contains an annotation that Rollup cannot interpret due to the position of the comment',
        )
    )
}

export const handleRollupWarning: WarningHandlerWithDefault = (warning, warn) => {
    if (isKnownSignalRAnnotationWarning(warning)) {
        return
    }

    warn(warning)
}

// https://vitejs.dev/config/
export default defineConfig({
    server: {
        host: true,
        port: 3000,
        strictPort: true,
        // Allow the Selenium Grid container (infra/docker-compose.yml) to load the
        // dev server via host.docker.internal when running e2e tests locally.
        allowedHosts: ['host.docker.internal'],
    },
    build: {
        rollupOptions: {
            onwarn: handleRollupWarning,
        },
    },
    plugins: [
        react(),
        VitePWA({
            registerType: 'autoUpdate',
            injectRegister: 'auto',
            manifest: false,
            workbox: {
                globPatterns: ['**/*.{js,css,html,ico,svg,png,json,woff,woff2}'],
                globIgnores: ['old/**', 'splashscreens/**'],
                navigateFallback: '/index.html',
                navigateFallbackDenylist: [
                    /^\/api/,
                    /^\/ws/,
                    /^\/swagger/,
                    /^\/healthcheck/,
                    /^\/authorize/,
                    /^\/token/,
                    /^\/register/,
                    /^\/\.well-known/,
                    /^\/mcp/,
                ],
            },
        }),
    ],
})
