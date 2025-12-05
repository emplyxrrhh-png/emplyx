import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { ArrowLeft, Save, Shield, Check, ChevronRight, ChevronDown } from 'lucide-react';
import { Role } from '../../../types/role';

// Mock Permissions Tree
const PERMISSION_MODULES = [
  {
    id: 'employees',
    label: 'Empleados',
    permissions: [
      { id: 'Employees.Read', label: 'Ver Empleados' },
      { id: 'Employees.Create', label: 'Crear Empleados' },
      { id: 'Employees.Update', label: 'Editar Empleados' },
      { id: 'Employees.Delete', label: 'Eliminar Empleados' },
    ]
  },
  {
    id: 'time',
    label: 'Gestión de Tiempos',
    permissions: [
      { id: 'Time.Read', label: 'Ver Registros' },
      { id: 'Time.Approve', label: 'Aprobar Solicitudes' },
      { id: 'Time.Edit', label: 'Editar Registros' },
    ]
  },
  {
    id: 'organization',
    label: 'Organización',
    permissions: [
      { id: 'Org.Read', label: 'Ver Estructura' },
      { id: 'Org.Manage', label: 'Gestionar Estructura' },
    ]
  }
];

const RoleFormPage = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const [isLoading, setIsLoading] = useState(false);
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
      setTimeout(() => {
        setFormData({
          id: id,
          name: 'Rol Editado',
          description: 'Descripción del rol',
          isSystem: false,
          permissions: ['Employees.Read', 'Time.Read']
        });
        setIsLoading(false);
      }, 500);
    }
  }, [id]);

  const toggleModule = (moduleId: string) => {
    setExpandedModules(prev => 
      prev.includes(moduleId) 
        ? prev.filter(id => id !== moduleId)
        : [...prev, moduleId]
    );
  };

  const togglePermission = (permissionId: string) => {
    setFormData(prev => {
      const currentPermissions = prev.permissions || [];
      const newPermissions = currentPermissions.includes(permissionId)
        ? currentPermissions.filter(p => p !== permissionId)
        : [...currentPermissions, permissionId];
      
      return { ...prev, permissions: newPermissions };
    });
  };

  const toggleModulePermissions = (moduleId: string, permissions: { id: string }[]) => {
    const modulePermissionIds = permissions.map(p => p.id);
    const currentPermissions = formData.permissions || [];
    
    // Check if all module permissions are currently selected
    const allSelected = modulePermissionIds.every(id => currentPermissions.includes(id));
    
    let newPermissions;
    if (allSelected) {
      // Deselect all
      newPermissions = currentPermissions.filter(id => !modulePermissionIds.includes(id));
    } else {
      // Select all (add missing ones)
      const toAdd = modulePermissionIds.filter(id => !currentPermissions.includes(id));
      newPermissions = [...currentPermissions, ...toAdd];
    }
    
    setFormData({ ...formData, permissions: newPermissions });
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
                value={formData.name}
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
                value={formData.description}
                onChange={e => setFormData({ ...formData, description: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                placeholder="Describe el propósito de este rol"
              />
            </div>
          </div>
        </div>

        {/* Permissions Picker */}
        <div className="bg-white p-6 rounded-xl shadow-sm border border-gray-100">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">Permisos del Sistema</h2>
          <div className="space-y-4">
            {PERMISSION_MODULES.map((module) => {
              const modulePermissionIds = module.permissions.map(p => p.id);
              const selectedCount = modulePermissionIds.filter(id => formData.permissions?.includes(id)).length;
              const isAllSelected = selectedCount === module.permissions.length;
              const isIndeterminate = selectedCount > 0 && !isAllSelected;

              return (
                <div key={module.id} className="border border-gray-200 rounded-lg overflow-hidden">
                  <div className="bg-gray-50 p-3 flex items-center justify-between">
                    <div className="flex items-center gap-3">
                      <button
                        type="button"
                        onClick={() => toggleModule(module.id)}
                        className="p-1 hover:bg-gray-200 rounded transition-colors"
                      >
                        {expandedModules.includes(module.id) ? <ChevronDown size={18} /> : <ChevronRight size={18} />}
                      </button>
                      
                      <div className="flex items-center gap-2">
                        <input
                          type="checkbox"
                          checked={isAllSelected}
                          ref={input => {
                            if (input) input.indeterminate = isIndeterminate;
                          }}
                          onChange={() => toggleModulePermissions(module.id, module.permissions)}
                          className="w-4 h-4 text-indigo-600 rounded focus:ring-indigo-500 border-gray-300"
                          disabled={formData.isSystem}
                        />
                        <span className="font-medium text-gray-900">{module.label}</span>
                      </div>
                    </div>
                    <span className="text-xs font-medium text-gray-500 bg-white px-2 py-1 rounded border border-gray-200">
                      {selectedCount} / {module.permissions.length} seleccionados
                    </span>
                  </div>

                  {expandedModules.includes(module.id) && (
                    <div className="p-4 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-3 bg-white">
                      {module.permissions.map((permission) => (
                        <label 
                          key={permission.id} 
                          className={`flex items-center gap-3 p-3 rounded-lg border cursor-pointer transition-all ${
                            formData.permissions?.includes(permission.id)
                              ? 'border-indigo-200 bg-indigo-50'
                              : 'border-gray-200 hover:border-gray-300'
                          }`}
                        >
                          <input
                            type="checkbox"
                            checked={formData.permissions?.includes(permission.id)}
                            onChange={() => togglePermission(permission.id)}
                            className="w-4 h-4 text-indigo-600 rounded focus:ring-indigo-500 border-gray-300"
                            disabled={formData.isSystem}
                          />
                          <span className={`text-sm ${
                            formData.permissions?.includes(permission.id) ? 'text-indigo-900 font-medium' : 'text-gray-700'
                          }`}>
                            {permission.label}
                          </span>
                        </label>
                      ))}
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
