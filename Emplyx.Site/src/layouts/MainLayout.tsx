import React, { useState, useEffect } from 'react';
import { Outlet } from 'react-router-dom';
import { Sidebar } from '../components/Sidebar';
import { Header } from '../components/Header';
import { WifiOff } from 'lucide-react';
import { API_BASE_URL } from '../config';

export const MainLayout: React.FC = () => {
  const [isOffline, setIsOffline] = useState(false);

  useEffect(() => {
    const checkStatus = async () => {
      try {
        const controller = new AbortController();
        const timeoutId = setTimeout(() => controller.abort(), 10000);
        
        await fetch(`${API_BASE_URL}/tenants`, { 
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
    const interval = setInterval(checkStatus, 5000);

    return () => clearInterval(interval);
  }, []);

  return (
    <div className="flex h-screen bg-gray-50 overflow-hidden">
      <Sidebar />
      
      <div className="flex-1 flex flex-col min-w-0">
        <Header />
        
        <main className="flex-1 overflow-y-auto p-6">
          <div className="max-w-7xl mx-auto">
            <Outlet />
          </div>
        </main>

        <footer className="bg-white border-t border-gray-200 py-4 px-6 flex justify-between items-center text-xs text-gray-400">
          <span>&copy; {new Date().getFullYear()} Emplyx. All rights reserved.</span>
          {isOffline && (
            <div className="flex items-center gap-2 px-2 py-1 bg-red-50 text-red-600 rounded border border-red-100 animate-pulse">
              <WifiOff size={12} />
              <span className="font-medium">Offline</span>
            </div>
          )}
        </footer>
      </div>
    </div>
  );
};
