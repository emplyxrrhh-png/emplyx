import React, { useState, useEffect } from 'react';
import { Outlet } from 'react-router-dom';
import { Sidebar } from '../components/Sidebar';
import { Header } from '../components/Header';
import { WifiOff } from 'lucide-react';

export const MainLayout: React.FC = () => {
  const [isOffline, setIsOffline] = useState(false);

  useEffect(() => {
    const checkStatus = async () => {
      try {
        const controller = new AbortController();
        const timeoutId = setTimeout(() => controller.abort(), 5000);
        
        await fetch('https://localhost:5001/api/tenants', { 
            method: 'GET',
            signal: controller.signal
        });
        clearTimeout(timeoutId);
        setIsOffline(false);
      } catch (error) {
        setIsOffline(true);
      }
    };

    checkStatus();
    const interval = setInterval(checkStatus, 10000);

    return () => clearInterval(interval);
  }, []);

  return (
    <div className="flex flex-col h-screen bg-gray-50 overflow-hidden">
      <div className="flex flex-1 overflow-hidden">
        <Sidebar />
        
        <div className="flex-1 flex flex-col min-w-0">
          <Header />
          
          <main className="flex-1 overflow-y-auto p-6">
            <div className="max-w-7xl mx-auto">
              <Outlet />
            </div>
          </main>
        </div>
      </div>

      <footer className="bg-white border-t border-gray-200 py-4 px-6 text-left text-xs text-gray-400 z-10 flex justify-between items-center">
        <span>&copy; {new Date().getFullYear()} Emplyx. All rights reserved.</span>
        {isOffline && (
          <div className="flex items-center gap-2 px-2 py-0.5 bg-red-100 text-red-700 rounded text-xs font-medium animate-pulse">
            <WifiOff size={12} />
            <span>Offline</span>
          </div>
        )}
      </footer>
    </div>
  );
};
