import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import { VitePWA } from 'vite-plugin-pwa'

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
