import React, { useState } from 'react';
import { useLocation } from 'react-router-dom';
import { Menu, ArrowLeft, ChevronDown, Building, Building2 } from 'lucide-react';
import { NavItem } from './NavItem';
import { MENU_ITEMS, CONFIG_ITEMS } from '../data/menuItems';
import { useOrganization } from '../context/OrganizationContext';
import clsx from 'clsx';

export const Sidebar: React.FC = () => {
  const [isCollapsed, setIsCollapsed] = useState(() => {
    const match = document.cookie.match(new RegExp('(^| )sidebar_collapsed=([^;]+)'));
    return match ? match[2] === 'true' : true;
  });
  
  const setCollapsedState = (state: boolean) => {
    setIsCollapsed(state);
    document.cookie = `sidebar_collapsed=${state}; path=/; max-age=31536000; SameSite=Strict; Secure`;
  };

  const [isCompanyMenuOpen, setIsCompanyMenuOpen] = useState(false);
  const { selectedCompany, setSelectedCompany, organizations } = useOrganization();
  const location = useLocation();
  const isConfigMode = location.pathname.startsWith('/configuracion');

  const getFilteredConfigItems = () => {
    if (!isConfigMode || !CONFIG_ITEMS.children) return MENU_ITEMS;

    const configChildren = CONFIG_ITEMS.children.map(section => {
      if (section.label === "Organización" && section.children) {
        return {
          ...section,
          children: section.children.filter(item => {
            if (selectedCompany?.type === 'tenant') {
              // If Tenant is selected: Show Empresas, Hide Centros de Trabajo
              return item.label !== "Centros de Trabajo";
            } else if (selectedCompany?.type === 'empresa') {
              // If Empresa is selected: Hide Empresas, Show Centros de Trabajo
              return item.label !== "Empresas" && item.label !== "Grupo de Empresas";
            }
            return true;
          })
        };
      }
      return section;
    });

    return configChildren;
  };

  const navItems = isConfigMode ? getFilteredConfigItems() : MENU_ITEMS;

  return (
    <aside 
      className={clsx(
        "flex flex-col h-screen bg-white border-r border-gray-200 transition-all duration-300 ease-in-out z-40",
        isCollapsed ? "w-20" : "w-72"
      )}
    >
      {/* App Header */}
      <div className="h-16 flex items-center px-4 border-b border-gray-100 relative">
        <div className={clsx("flex items-center gap-3 w-full", isCollapsed ? "justify-center" : "justify-between")}>
          <div className="w-10 h-10 rounded-lg bg-indigo-600 flex items-center justify-center text-white font-bold shrink-0 cursor-pointer" onClick={() => setCollapsedState(!isCollapsed)}>
            E
          </div>
          {!isCollapsed && (
            <div className="flex-1 min-w-0">
              <p className="text-lg font-bold text-gray-900 truncate">Emplyx</p>
            </div>
          )}
          {!isCollapsed && (
            <button onClick={() => setCollapsedState(true)} className="p-1 hover:bg-gray-100 rounded">
              <Menu size={16} className="text-gray-400" />
            </button>
          )}
        </div>
      </div>

      {/* Company Switcher */}
      {!isCollapsed && (
        <div className="px-4 py-4 border-b border-gray-100">
          <div className="relative">
            <button 
              onClick={() => setIsCompanyMenuOpen(!isCompanyMenuOpen)}
              className="w-full flex items-center justify-between p-2 bg-gray-50 hover:bg-gray-100 rounded-lg border border-gray-200 transition-colors"
            >
              <div className="flex items-center gap-2 min-w-0">
                <div className={clsx(
                  "p-1 rounded border border-gray-200",
                  selectedCompany?.type === 'tenant' ? "bg-purple-50" : "bg-white"
                )}>
                  {selectedCompany?.type === 'tenant' ? (
                    <Building2 size={14} className="text-purple-600" />
                  ) : (
                    <Building size={14} className="text-indigo-600" />
                  )}
                </div>
                <span className="text-sm font-medium text-gray-700 truncate">
                  {selectedCompany?.name || "Seleccionar..."}
                </span>
              </div>
              <ChevronDown size={14} className="text-gray-500" />
            </button>

            {isCompanyMenuOpen && (
              <div className="absolute top-full left-0 right-0 mt-1 bg-white border border-gray-100 rounded-lg shadow-lg z-50 py-1 max-h-60 overflow-y-auto">
                {organizations.map((org) => (
                  <button
                    key={`${org.type}-${org.id}`}
                    onClick={() => {
                      setSelectedCompany(org);
                      setIsCompanyMenuOpen(false);
                    }}
                    className={clsx(
                      "w-full flex items-center justify-between px-3 py-2 text-sm transition-colors",
                      selectedCompany?.id === org.id && selectedCompany?.type === org.type
                        ? "bg-indigo-50 text-indigo-700"
                        : "text-gray-600 hover:bg-gray-50"
                    )}
                  >
                    <div className="flex items-center gap-2 truncate">
                      {org.type === 'tenant' ? <Building2 size={14} className="text-purple-500" /> : <Building size={14} />}
                      <span className="truncate">{org.name}</span>
                    </div>
                  </button>
                ))}
                {organizations.length === 0 && (
                  <div className="px-3 py-2 text-sm text-gray-400 text-center">No hay organizaciones</div>
                )}
              </div>
            )}
          </div>
        </div>
      )}

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

