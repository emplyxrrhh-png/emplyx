import React from 'react';
import { Outlet } from 'react-router-dom';
import { Sidebar } from '../components/Sidebar';
import { Header } from '../components/Header';

export const MainLayout: React.FC = () => {
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

        <footer className="bg-white border-t border-gray-200 py-4 px-6 text-center text-xs text-gray-400">
          &copy; {new Date().getFullYear()} Emplyx. All rights reserved.
        </footer>
      </div>
    </div>
  );
};
