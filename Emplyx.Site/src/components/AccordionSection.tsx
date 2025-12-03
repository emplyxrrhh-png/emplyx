import React, { useState } from 'react';
import { ChevronDown, ChevronUp } from 'lucide-react';

interface AccordionSectionProps {
  title: React.ReactNode;
  children: React.ReactNode;
  defaultOpen?: boolean;
  isOpen?: boolean;
  onToggle?: (isOpen: boolean) => void;
}

export const AccordionSection: React.FC<AccordionSectionProps> = ({ title, children, defaultOpen = false, isOpen: controlledIsOpen, onToggle }) => {
  const [internalIsOpen, setInternalIsOpen] = useState(defaultOpen);

  const isOpen = controlledIsOpen !== undefined ? controlledIsOpen : internalIsOpen;

  const handleToggle = () => {
    const newState = !isOpen;
    if (onToggle) {
      onToggle(newState);
    }
    if (controlledIsOpen === undefined) {
      setInternalIsOpen(newState);
    }
  };

  return (
    <div className="border border-gray-200 rounded-lg mb-4 overflow-hidden">
      <button
        type="button"
        onClick={handleToggle}
        className="w-full px-6 py-4 flex justify-between items-center bg-gray-50 hover:bg-gray-100 transition-colors"
      >
        <div className="text-lg font-medium text-gray-900 flex-1 flex items-center">{title}</div>
        {isOpen ? <ChevronUp size={20} className="text-gray-500 ml-4" /> : <ChevronDown size={20} className="text-gray-500 ml-4" />}
      </button>
      {isOpen && (
        <div className="p-6 bg-white border-t border-gray-200">
          {children}
        </div>
      )}
    </div>
  );
};
