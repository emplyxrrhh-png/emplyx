import React, { useState } from 'react';
import { useLocation } from 'react-router-dom';
import { Menu, ArrowLeft } from 'lucide-react';
import { NavItem } from './NavItem';
import { MENU_ITEMS, CONFIG_ITEMS } from '../data/menuItems';
import clsx from 'clsx';

export const Sidebar: React.FC = () => {
  const [isCollapsed, setIsCollapsed] = useState(true);
  const location = useLocation();
  const isConfigMode = location.pathname.startsWith('/configuracion');

  const navItems = isConfigMode ? (CONFIG_ITEMS.children || []) : MENU_ITEMS;

  return (
    <aside 
      className={clsx(
        "flex flex-col h-screen bg-white border-r border-gray-200 transition-all duration-300 ease-in-out z-40",
        isCollapsed ? "w-20" : "w-72"
      )}
    >
      {/* Company Selector */}
      <div className="h-16 flex items-center px-4 border-b border-gray-100 relative">
        <div className={clsx("flex items-center gap-3 w-full", isCollapsed ? "justify-center" : "justify-between")}>
          <div className="w-10 h-10 rounded-lg bg-indigo-600 flex items-center justify-center text-white font-bold shrink-0 cursor-pointer" onClick={() => setIsCollapsed(!isCollapsed)}>
            E
          </div>
          {!isCollapsed && (
            <div className="flex-1 min-w-0">
              <p className="text-sm font-semibold text-gray-900 truncate">Emplyx Corp</p>
              <p className="text-xs text-gray-500 truncate">Enterprise Plan</p>
            </div>
          )}
          {!isCollapsed && (
            <button onClick={() => setIsCollapsed(true)} className="p-1 hover:bg-gray-100 rounded">
              <Menu size={16} className="text-gray-400" />
            </button>
          )}
        </div>
      </div>

      {/* Navigation */}
      <div className="flex-1 overflow-y-auto py-4 scrollbar-hide">
        <nav className="space-y-1 px-2">
          {navItems.map((item, index) => (
            <NavItem key={index} {...item} isCollapsed={isCollapsed} />
          ))}
        </nav>
      </div>

      {/* Configuration & Footer */}
      <div className="border-t border-gray-100 p-2 bg-gray-50/50">
        {isConfigMode ? (
          <NavItem 
            label="Volver" 
            icon={ArrowLeft} 
            href="/" 
            isCollapsed={isCollapsed} 
          />
        ) : (
          <NavItem 
            {...CONFIG_ITEMS} 
            children={undefined} 
            isCollapsed={isCollapsed} 
          />
        )}
      </div>
    </aside>
  );
};

