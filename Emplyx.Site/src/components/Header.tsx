import React, { useState, useEffect } from 'react';
import { Bell, Search, WifiOff } from 'lucide-react';

export const Header: React.FC = () => {
  const [isOffline, setIsOffline] = useState(false);

  useEffect(() => {
    const checkStatus = async () => {
      try {
        const controller = new AbortController();
        const timeoutId = setTimeout(() => controller.abort(), 10000);
        
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
    const interval = setInterval(checkStatus, 5000);

    return () => clearInterval(interval);
  }, []);

  return (
    <header className="h-16 bg-white border-b border-gray-200 flex items-center justify-between px-6 z-30">
      {/* Left: Search or Breadcrumbs (Optional) */}
      <div className="flex items-center gap-4 w-96">
        <div className="relative w-full">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" size={18} />
          <input 
            type="text" 
            placeholder="Buscar..." 
            className="w-full pl-10 pr-4 py-2 bg-gray-50 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-all"
          />
        </div>
      </div>

      {/* Right: User Profile & Actions */}
      <div className="flex items-center gap-6">
        {isOffline && (
          <div className="flex items-center gap-2 px-3 py-1 bg-red-100 text-red-700 rounded-md border border-red-200 animate-pulse">
            <WifiOff size={14} />
            <span className="text-xs font-medium">Offline</span>
          </div>
        )}

        <button className="relative p-2 text-gray-500 hover:bg-gray-100 rounded-full transition-colors">
          <Bell size={20} />
          <span className="absolute top-2 right-2 w-2 h-2 bg-red-500 rounded-full border-2 border-white"></span>
        </button>

        <div className="flex items-center gap-3 pl-6 border-l border-gray-200">
          <div className="text-right hidden md:block">
            <p className="text-sm font-semibold text-gray-900">Ana García</p>
            <p className="text-xs text-gray-500">HR Manager</p>
          </div>
          <div className="w-10 h-10 rounded-full bg-gradient-to-br from-indigo-500 to-purple-600 p-[2px]">
            <div className="w-full h-full rounded-full bg-white p-[2px]">
              <img 
                src="https://api.dicebear.com/7.x/avataaars/svg?seed=Ana" 
                alt="User" 
                className="w-full h-full rounded-full object-cover"
              />
            </div>
          </div>
        </div>
      </div>
    </header>
  );
};
