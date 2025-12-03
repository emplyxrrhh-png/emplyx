import React, { useState, useRef } from 'react';
import { Link } from 'react-router-dom';
import { ChevronRight } from 'lucide-react';
import clsx from 'clsx';
import { NavItemData } from '../data/menuItems';

export interface NavItemProps extends NavItemData {
  isCollapsed: boolean;
}

export const NavItem: React.FC<NavItemProps> = ({ label, icon: Icon, href, children, isCollapsed }) => {
  const [isHovered, setIsHovered] = useState(false);
  const [coords, setCoords] = useState({ top: 0, left: 0 });
  const itemRef = useRef<HTMLDivElement>(null);
  const hasChildren = children && children.length > 0;

  const handleMouseEnter = () => {
    if (itemRef.current) {
      const rect = itemRef.current.getBoundingClientRect();
      setCoords({
        top: rect.top,
        left: rect.right
      });
    }
    setIsHovered(true);
  };

  return (
    <>
      <div 
        ref={itemRef}
        className="relative"
        onMouseEnter={handleMouseEnter}
        onMouseLeave={() => setIsHovered(false)}
      >
        <Link
          to={href || '#'}
          className={clsx(
            "flex items-center px-4 py-3 text-gray-600 hover:bg-indigo-50 hover:text-indigo-600 transition-colors duration-200",
            isCollapsed ? "justify-center" : "justify-between"
          )}
        >
          <div className="flex items-center gap-3">
            <Icon size={22} strokeWidth={1.5} />
            {!isCollapsed && <span className="font-medium text-sm">{label}</span>}
          </div>
          {!isCollapsed && hasChildren && (
            <ChevronRight size={16} className="text-gray-400" />
          )}
        </Link>
      </div>

      {/* Fixed Overlay for Submenu to avoid clipping */}
      {hasChildren && isHovered && (
        <div 
          className="fixed z-50 bg-white rounded-lg shadow-xl border border-gray-100 py-2 w-56 animate-in fade-in slide-in-from-left-2 duration-200"
          style={{ 
            top: coords.top, 
            left: coords.left + 8 // Add some gap
          }}
          onMouseEnter={() => setIsHovered(true)}
          onMouseLeave={() => setIsHovered(false)}
        >
          {children.map((child, index) => (
            <Link
              key={index}
              to={child.href || '#'}
              className="block px-4 py-2 text-sm text-gray-600 hover:bg-indigo-50 hover:text-indigo-600"
            >
              {child.label}
            </Link>
          ))}
        </div>
      )}
    </>
  );
};
