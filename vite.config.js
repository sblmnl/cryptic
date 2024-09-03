import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
    root: './src/WebUI',
    plugins: [react({ jsxRuntime: 'classic' })],
    server: {
        proxy: {
            '/api': {
                target: 'https://localhost:8443',
                changeOrigin: true,
            }
        },
        watch: {
            ignored: ["**/*.fs"]
        }
    }
})