import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
    plugins: [react()],
                            server: {
                                host: true, // Доступ внутри Docker-сети
                                port: 3000,
                                strictPort: true,
                            },
});
