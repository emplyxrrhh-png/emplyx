import { defineConfig, loadEnv } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')
  
  // Default URLs por modo
  const defaultUrl = mode === 'development' 
    ? 'https://localhost:5001/api' 
    : 'https://emplyxapi.azurewebsites.net/api';
  
  return {
    plugins: [react()],
    define: {
      'import.meta.env.VITE_API_URL': JSON.stringify(
        env.VITE_API_URL || defaultUrl
      )
    }
  }
})
