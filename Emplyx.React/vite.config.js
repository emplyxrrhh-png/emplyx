import { defineConfig, loadEnv } from 'vite';
import react from '@vitejs/plugin-react';
// https://vitejs.dev/config/
export default defineConfig(function (_a) {
    var mode = _a.mode;
    var env = loadEnv(mode, process.cwd(), '');
    return {
        plugins: [react()],
        define: {
            'import.meta.env.VITE_API_URL': JSON.stringify(env.VITE_API_URL || 'https://emplyxapi.azurewebsites.net/api')
        }
    };
});
