import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import { VitePWA } from 'vite-plugin-pwa'

// https://vitejs.dev/config/
export default defineConfig({
    server: {
        host: true,
        port: 3000,
        strictPort: true,
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
