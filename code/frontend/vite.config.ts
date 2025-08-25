import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import viteCompression from 'vite-plugin-compression'

// https://vitejs.dev/config/
export default defineConfig({
    server: {
        host: true,
        port: 3000,
        strictPort: true,
    },
    plugins: [
        react(),
        viteCompression({
            algorithm: 'gzip',
            ext: '.gz',
            threshold: 0,
            filter: (file: string) =>
                /\.(js|mjs|css|html|json|svg)$/.test(file),
        }),
        viteCompression({
            algorithm: 'brotliCompress',
            ext: '.br',
            threshold: 0,
            filter: (file: string) =>
                /\.(js|mjs|css|html|json|svg)$/.test(file),
        }),
    ],
})
