import React, { useEffect, useState, useRef } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { ArrowLeft, Save, Shield, Check, ChevronRight, ChevronDown, Eye, List, Users, Clock, Building2 } from 'lucide-react';
import { Role } from '../../../types/role';

// Mock Permissions Tree
const PERMISSION_MODULES = [
  {
    id: 'employees',
    label: 'Empleados',
    features: [
      {
        id: 'employees_mgmt',
        label: 'Gestión de Empleados',
        description: 'Permite visualizar, editar, crear o eliminar fichas de empleados.',
        type: 'level',
        levels: [
          { value: 'view', label: 'Ver', permissions: ['Employees.Read'] },
          { value: 'edit', label: 'Editar', permissions: ['Employees.Read', 'Employees.Update'] },
          { value: 'create', label: 'Crear', permissions: ['Employees.Read', 'Employees.Update', 'Employees.Create'] },
          { value: 'delete', label: 'Eliminar', permissions: ['Employees.Read', 'Employees.Update', 'Employees.Create', 'Employees.Delete'] }
        ]
      }
    ]
  },
  {
    id: 'time',
    label: 'Gestión de Tiempos',
    features: [
      {
        id: 'time_records',
        label: 'Gestión de Registros',
        description: 'Control sobre los registros de jornada y fichajes de los empleados.',
        type: 'level',
        levels: [
          { value: 'view', label: 'Ver', permissions: ['Time.Read'] },
          { value: 'edit', label: 'Editar', permissions: ['Time.Read', 'Time.Edit'] }
        ]
      },
      {
        id: 'time_approval',
        label: 'Aprobar Solicitudes',
        description: 'Capacidad para aprobar o rechazar solicitudes de vacaciones y ausencias.',
        type: 'boolean',
        permission: 'Time.Approve'
      }
    ]
  },
  {
    id: 'organization',
    label: 'Organización',
    features: [
      {
        id: 'org_structure',
        label: 'Estructura Organizativa',
        description: 'Gestión de la estructura organizativa, departamentos y centros de trabajo.',
        type: 'level',
        levels: [
          { value: 'view', label: 'Ver', permissions: ['Org.Read'] },
          { value: 'manage', label: 'Gestionar', permissions: ['Org.Read', 'Org.Manage'] }
        ]
      }
    ]
  }
];

const getFeatureLevel = (feature: any, currentPermissions: string[] = []) => {
  if (feature.type === 'boolean') {
    return currentPermissions.includes(feature.permission) ? 'yes' : 'no';
  }

  for (let i = feature.levels.length - 1; i >= 0; i--) {
    const level = feature.levels[i];
    const hasAll = level.permissions.every((p: string) => currentPermissions.includes(p));
    if (hasAll) return level.value;
  }
  
  return 'none';
};

const getLevelColor = (level: string) => {
  switch (level) {
    case 'view': return 'text-blue-700 bg-blue-50 border-blue-200';
    case 'edit': return 'text-indigo-700 bg-indigo-50 border-indigo-200';
    case 'create': return 'text-purple-700 bg-purple-50 border-purple-200';
    case 'delete': return 'text-red-700 bg-red-50 border-red-200';
    case 'manage': return 'text-indigo-700 bg-indigo-50 border-indigo-200';
    default: return 'text-gray-700 bg-gray-50 border-gray-200';
  }
};

const getModuleIcon = (moduleId: string) => {
  switch (moduleId) {
    case 'employees': return Users;
    case 'time': return Clock;
    case 'organization': return Building2;
    default: return Shield;
  }
};

const FeatureRow = ({ feature, currentPermissions, onLevelChange, disabled }: { 
  feature: any, 
  currentPermissions: string[], 
  onLevelChange: (feature: any, value: string) => void,
  disabled: boolean 
}) => {
  const [isOpen, setIsOpen] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const currentValue = getFeatureLevel(feature, currentPermissions);
  
  const options = feature.type === 'boolean' 
    ? [
        { value: 'no', label: 'No', color: 'text-gray-600 bg-gray-50 border-gray-200' },
        { value: 'yes', label: 'Sí', color: 'text-green-700 bg-green-50 border-green-200' }
      ]
    : [
        { value: 'none', label: 'Sin Acceso', color: 'text-gray-600 bg-gray-50 border-gray-200' },
        ...(feature.levels?.map((l: any) => ({ 
            value: l.value, 
            label: l.label,
            color: getLevelColor(l.value) 
        })) || [])
      ];

  const selectedOption = options.find(o => o.value === currentValue) || options[0];

  return (
    <div className="flex items-center justify-between py-2 border-b border-gray-100 last:border-0">
        <div>
            <span className="text-gray-700 font-medium block">{feature.label}</span>
            {feature.description && (
                <span className="text-sm text-gray-500 block mt-0.5">{feature.description}</span>
            )}
        </div>
        
        <div className="relative" ref={dropdownRef}>
            <button
                type="button"
                onClick={() => !disabled && setIsOpen(!isOpen)}
                disabled={disabled}
                className={`flex items-center justify-between gap-2 px-3 py-2 text-sm font-medium rounded-lg border transition-all ${
                    isOpen ? 'ring-2 ring-indigo-100 border-indigo-300' : 'border-transparent hover:border-gray-300'
                } ${selectedOption.color}`}
            >
                <span className="whitespace-nowrap">{selectedOption.label}</span>
                <ChevronDown size={16} className={`transition-transform duration-200 ${isOpen ? 'rotate-180' : ''}`} />
            </button>

            {isOpen && (
                <div className="absolute right-0 z-50 mt-1 min-w-full w-max bg-white rounded-lg shadow-xl border border-gray-100 py-1 animate-in fade-in zoom-in-95 duration-100">
                    {options.map((option) => (
                        <button
                            key={option.value}
                            type="button"
                            onClick={() => {
                                onLevelChange(feature, option.value);
                                setIsOpen(false);
                            }}
                            className={`w-full text-left px-4 py-2 text-sm flex items-center justify-between gap-3 hover:bg-gray-50 transition-colors ${
                                option.value === currentValue ? 'text-indigo-600 font-medium bg-indigo-50' : 'text-gray-700'
                            }`}
                        >
                            <span className="whitespace-nowrap">{option.label}</span>
                            {option.value === currentValue && <Check size={14} />}
                        </button>
                    ))}
                </div>
            )}
        </div>
    </div>
  );
};

const RoleFormPage = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const [isLoading, setIsLoading] = useState(false);
  const [showOnlyActive, setShowOnlyActive] = useState(false);
  const [expandedModules, setExpandedModules] = useState<string[]>(['employees', 'time', 'organization']);
  
  const [formData, setFormData] = useState<Partial<Role>>({
    name: '',
    description: '',
    isSystem: false,
    permissions: []
  });

  useEffect(() => {
    if (id) {
      // Mock fetch existing role
      setIsLoading(true);
      // Simulating API call
      setTimeout(() => {
        setFormData({
          id: id,
          name: id === '1' ? 'Administrador' : (id === '2' ? 'RRHH' : 'Rol Editado'),
          description: id === '1' ? 'Acceso total al sistema' : (id === '2' ? 'Gestión de empleados y organización' : 'Descripción del rol'),
          isSystem: false,
          permissions: ['Employees.Read', 'Time.Read']
        });
        setIsLoading(false);
      }, 100);
    }
  }, [id]);

  const toggleModule = (moduleId: string) => {
    setExpandedModules(prev => 
      prev.includes(moduleId) 
        ? prev.filter(id => id !== moduleId)
        : [...prev, moduleId]
    );
  };

  const handleFeatureChange = (feature: any, value: string) => {
    setFormData(prev => {
      const current = prev.permissions || [];
      let newPermissions = [...current];

      if (feature.type === 'boolean') {
        if (value === 'yes') {
          if (!newPermissions.includes(feature.permission)) {
            newPermissions.push(feature.permission);
          }
        } else {
          newPermissions = newPermissions.filter(p => p !== feature.permission);
        }
      } else {
        // Remove all permissions related to this feature first
        const allFeaturePermissions = new Set<string>();
        feature.levels.forEach((l: any) => l.permissions.forEach((p: string) => allFeaturePermissions.add(p)));
        
        newPermissions = newPermissions.filter(p => !allFeaturePermissions.has(p));

        // Add permissions for the selected level
        if (value !== 'none') {
          const selectedLevel = feature.levels.find((l: any) => l.value === value);
          if (selectedLevel) {
            newPermissions.push(...selectedLevel.permissions);
          }
        }
      }
      
      // Deduplicate just in case
      return { ...prev, permissions: Array.from(new Set(newPermissions)) };
    });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    // TODO: Implement API call
    setTimeout(() => {
      setIsLoading(false);
      navigate('/configuracion/usuarios/roles');
    }, 1000);
  };

  return (
    <div className="p-6 max-w-5xl mx-auto">
      <div className="flex items-center gap-4 mb-6">
        <button
          onClick={() => navigate('/configuracion/usuarios/roles')}
          className="p-2 hover:bg-gray-100 rounded-lg transition-colors"
        >
          <ArrowLeft size={24} className="text-gray-600" />
        </button>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">
            {id ? 'Editar Rol' : 'Nuevo Rol'}
          </h1>
          <p className="text-gray-500">
            {id ? 'Modifica los permisos del rol existente' : 'Define un nuevo rol y sus permisos'}
          </p>
        </div>
      </div>

      <form onSubmit={handleSubmit} className="space-y-6">
        {/* Basic Info */}
        <div className="bg-white p-6 rounded-xl shadow-sm border border-gray-100">
          <h2 className="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
            <Shield size={20} className="text-indigo-600" />
            Información Básica
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Nombre del Rol</label>
              <input
                type="text"
                required
                value={formData.name || ''}
                onChange={e => setFormData({ ...formData, name: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                placeholder="Ej. Gestor de Ausencias"
                disabled={formData.isSystem}
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Descripción</label>
              <input
                type="text"
                value={formData.description || ''}
                onChange={e => setFormData({ ...formData, description: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                placeholder="Describe el propósito de este rol"
              />
            </div>
          </div>
        </div>

        {/* Permissions Picker */}
        <div className="bg-white p-6 rounded-xl shadow-sm border border-gray-100">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-lg font-semibold text-gray-900">Permisos del Sistema</h2>
            <div className="flex items-center gap-1 bg-gray-100 p-1 rounded-lg">
              <button
                type="button"
                onClick={() => setShowOnlyActive(false)}
                className={`p-1.5 rounded-md transition-all ${!showOnlyActive ? 'bg-white shadow-sm text-indigo-600' : 'text-gray-500 hover:text-gray-700'}`}
                title="Ver todos"
              >
                <List size={16} />
              </button>
              <button
                type="button"
                onClick={() => setShowOnlyActive(true)}
                className={`p-1.5 rounded-md transition-all ${showOnlyActive ? 'bg-white shadow-sm text-indigo-600' : 'text-gray-500 hover:text-gray-700'}`}
                title="Ver solo activos"
              >
                <Eye size={16} />
              </button>
            </div>
          </div>
          <div className="space-y-4">
            {PERMISSION_MODULES.map((module) => {
              // Calculate if any permission in this module is selected
              const allModulePermissions = new Set<string>();
              module.features.forEach(f => {
                if (f.type === 'level') {
                  f.levels?.forEach(l => l.permissions.forEach(p => allModulePermissions.add(p)));
                } else {
                  if (f.permission) allModulePermissions.add(f.permission);
                }
              });
              
              const selectedCount = Array.from(allModulePermissions).filter(p => formData.permissions?.includes(p)).length;
              
              if (showOnlyActive && selectedCount === 0) return null;

              const ModuleIcon = getModuleIcon(module.id);

              return (
                <div key={module.id} className="border border-gray-200 rounded-lg">
                  <div 
                    className={`bg-gray-50 p-4 flex items-center justify-between cursor-pointer hover:bg-gray-100 transition-colors ${
                      expandedModules.includes(module.id) ? 'rounded-t-lg' : 'rounded-lg'
                    }`}
                    onClick={() => toggleModule(module.id)}
                  >
                    <div className="flex items-center gap-4">
                      <div className="p-2 bg-white rounded-lg border border-gray-200">
                        <ModuleIcon size={20} className="text-indigo-600" />
                      </div>
                      <div>
                        <h3 className="font-medium text-gray-900">{module.label}</h3>
                        <p className="text-sm text-gray-500">
                          {selectedCount > 0 
                            ? `${selectedCount} permisos activos` 
                            : 'Sin permisos configurados'}
                        </p>
                      </div>
                    </div>

                    <button
                      type="button"
                      className={`p-2 rounded-lg transition-colors ${
                        expandedModules.includes(module.id) 
                          ? 'bg-gray-200 text-gray-700' 
                          : 'text-gray-500'
                      }`}
                    >
                      {expandedModules.includes(module.id) ? <ChevronDown size={20} /> : <ChevronRight size={20} />}
                    </button>
                  </div>

                  {expandedModules.includes(module.id) && (
                    <div className="p-4 border-t border-gray-200 bg-white rounded-b-lg">
                      {module.features.map((feature) => {
                        if (showOnlyActive) {
                           const val = getFeatureLevel(feature, formData.permissions || []);
                           if (val === 'none' || val === 'no') return null;
                        }
                        return (
                        <FeatureRow
                          key={feature.id}
                          feature={feature}
                          currentPermissions={formData.permissions || []}
                          onLevelChange={handleFeatureChange}
                          disabled={formData.isSystem || false}
                        />
                      )})}
                    </div>
                  )}
                </div>
              );
            })}
          </div>
        </div>

        <div className="flex justify-end gap-4">
          <button
            type="button"
            onClick={() => navigate('/configuracion/usuarios/roles')}
            className="px-6 py-2 border border-gray-300 text-gray-700 font-medium rounded-lg hover:bg-gray-50 transition-colors"
          >
            Cancelar
          </button>
          <button
            type="submit"
            disabled={isLoading || formData.isSystem}
            className="flex items-center gap-2 px-6 py-2 bg-indigo-600 text-white font-medium rounded-lg hover:bg-indigo-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <Save size={20} />
            {isLoading ? 'Guardando...' : 'Guardar Rol'}
          </button>
        </div>
      </form>
    </div>
  );
};

export default RoleFormPage;
