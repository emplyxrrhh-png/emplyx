import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { ArrowLeft, Save, ChevronDown, ChevronUp, Shield, CheckCircle, XCircle, Search } from 'lucide-react';
import { useOrganization } from '../../../context/OrganizationContext';
import { CreateEmployeeRequest, Employee, UpdateEmployeeRequest, UserRole } from '../../../types/employee';
import { Role } from '../../../types/role';
import { API_BASE_URL } from '../../../config';

const EmployeeFormPage = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const { selectedCompany } = useOrganization();
  const [isLoading, setIsLoading] = useState(false);
  const [activeSection, setActiveSection] = useState<string | null>('general');
  
  // Role State
  const [roleSearchTerm, setRoleSearchTerm] = useState('');
  const [showAllRoles, setShowAllRoles] = useState(false);

  // Mock Roles
  const [availableRoles] = useState<Role[]>([
    { id: '1', name: 'Administrador', description: 'Acceso total', isSystem: true, permissions: [], isCommon: true },
    { id: '2', name: 'RRHH', description: 'Gestión de personal', isSystem: false, permissions: [], isCommon: true },
    { id: '3', name: 'Supervisor', description: 'Gestión de equipos', isSystem: false, permissions: [], isCommon: true },
    { id: '4', name: 'Empleado', description: 'Acceso básico', isSystem: true, permissions: [], isCommon: true },
    // Adding more mock roles to simulate a larger list
    { id: '5', name: 'Gerente', description: 'Acceso a informes gerenciales', isSystem: false, permissions: [] },
    { id: '6', name: 'Auditor', description: 'Solo lectura para auditoría', isSystem: false, permissions: [] },
    { id: '7', name: 'Soporte IT', description: 'Acceso técnico', isSystem: false, permissions: [] },
    { id: '8', name: 'Becario', description: 'Acceso limitado', isSystem: false, permissions: [] },
  ]);

  const displayedRoles = roleSearchTerm 
    ? availableRoles.filter(role => 
        role.name.toLowerCase().includes(roleSearchTerm.toLowerCase()) ||
        role.description.toLowerCase().includes(roleSearchTerm.toLowerCase())
      )
    : showAllRoles 
      ? availableRoles 
      : availableRoles.filter(r => r.isCommon);

  const [formData, setFormData] = useState<Partial<Employee>>({
    nombre: '',
    apellidos: '',
    alias: '',
    groupName: '',
    type: 'Standard',
    status: 'Active',
    contractType: 'Indefinido',
    idioma: 'es',
    remoteWork: false,
    attControlled: true,
    accControlled: true,
    jobControlled: false,
    extControlled: false,
    riskControlled: false,
    activeDirectory: false,
    userFields: {}
  });

  useEffect(() => {
    if (id) {
      fetchEmployee();
    }
  }, [id]);

  const fetchEmployee = async () => {
    setIsLoading(true);
    try {
      const response = await fetch(`${API_BASE_URL}/employees/${id}`);
      if (response.ok) {
        const data = await response.json();
        setFormData(data);
      }
    } catch (error) {
      console.error('Error fetching employee:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const toggleRole = (roleName: string) => {
    setFormData(prev => {
      const currentRoles = prev.roles || [];
      const roleExists = currentRoles.some(r => r.rolName === roleName);
      
      let newRoles: UserRole[];
      
      if (roleExists) {
        // Remove role
        newRoles = currentRoles.filter(r => r.rolName !== roleName);
      } else {
        // Add role (default to Global context for now, as per new UI simplicity)
        // We need to find the role ID from availableRoles
        const roleDef = availableRoles.find(r => r.name === roleName);
        const newRole: UserRole = {
          rolId: roleDef?.id || '0', // Fallback ID
          rolName: roleName,
          contextoTipo: 'Global'
        };
        newRoles = [...currentRoles, newRole];
      }
      
      return { ...prev, roles: newRoles };
    });
  };

  const handleImageUpload = async (file: File) => {
    setIsLoading(true);
    try {
      const formDataUpload = new FormData();
      formDataUpload.append('file', file);

      const response = await fetch(`${API_BASE_URL}/employees/upload-image`, {
        method: 'POST',
        body: formDataUpload,
      });

      if (response.ok) {
        const data = await response.json();
        setFormData(prev => ({ ...prev, image: data.url }));
      } else {
        console.error('Error uploading image');
        alert('Error al subir la imagen');
      }
    } catch (error) {
      console.error('Error uploading image:', error);
      alert('Error al subir la imagen');
    } finally {
      setIsLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedCompany) return;

    setIsLoading(true);
    try {
      const url = id 
        ? `${API_BASE_URL}/employees/${id}`
        : `${API_BASE_URL}/employees`;
      
      const method = id ? 'PUT' : 'POST';
      
      const body: CreateEmployeeRequest | UpdateEmployeeRequest = {
        ...formData as any,
        empresaId: selectedCompany.id
      };

      const response = await fetch(url, {
        method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body),
      });

      if (response.ok) {
        navigate('/empleados');
      }
    } catch (error) {
      console.error('Error saving employee:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const toggleSection = (section: string) => {
    setActiveSection(activeSection === section ? null : section);
  };

  if (!selectedCompany && !id) {
      return <div className="p-6">Selecciona una empresa primero.</div>;
  }

  return (
    <div className="p-6 max-w-4xl mx-auto">
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center gap-4">
          <button
            onClick={() => navigate('/empleados')}
            className="p-2 hover:bg-gray-100 rounded-full transition-colors"
          >
            <ArrowLeft size={24} className="text-gray-600" />
          </button>
          <div>
            <h1 className="text-2xl font-bold text-gray-900">
              {id ? 'Editar Usuario' : 'Nuevo Usuario'}
            </h1>
            <p className="text-gray-500">
              {id ? 'Modifica los datos del usuario' : 'Añade un nuevo usuario a la organización'}
            </p>
          </div>
        </div>
        
        <div 
          className="relative w-24 h-32 border-2 border-gray-300 border-dashed rounded-lg hover:border-indigo-500 transition-colors cursor-pointer overflow-hidden bg-gray-50 flex items-center justify-center"
          onDragOver={(e) => e.preventDefault()}
          onDrop={(e) => {
            e.preventDefault();
            const file = e.dataTransfer.files[0];
            if (file) {
              handleImageUpload(file);
            }
          }}
          onClick={() => document.getElementById('header-file-upload')?.click()}
        >
          {formData.image ? (
            <img src={formData.image} alt="Employee" className="w-full h-full object-cover" />
          ) : (
            <div className="text-center p-1">
              <span className="text-xs text-gray-500">Foto</span>
            </div>
          )}
          <input 
            id="header-file-upload" 
            type="file" 
            className="hidden" 
            accept="image/*"
            onChange={(e) => {
              const file = e.target.files?.[0];
              if (file) handleImageUpload(file);
            }}
          />
        </div>

        <button
          onClick={handleSubmit}
          disabled={isLoading}
          className="flex items-center gap-2 px-6 py-2.5 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors disabled:opacity-50"
        >
          <Save size={20} />
          {isLoading ? 'Guardando...' : 'Guardar'}
        </button>
      </div>

      <form onSubmit={handleSubmit} className="space-y-4">
        
        {/* Section 1: General Data */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
          <button
            type="button"
            onClick={() => toggleSection('general')}
            className="w-full flex items-center justify-between p-4 bg-gray-50 hover:bg-gray-100 transition-colors"
          >
            <span className="font-semibold text-gray-900">Datos Generales</span>
            {activeSection === 'general' ? <ChevronUp size={20} /> : <ChevronDown size={20} />}
          </button>
          
          {activeSection === 'general' && (
            <div className="p-6 grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Nombre</label>
                <input
                  type="text"
                  required
                  value={formData.nombre}
                  onChange={e => setFormData({ ...formData, nombre: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Apellidos</label>
                <input
                  type="text"
                  required
                  value={formData.apellidos}
                  onChange={e => setFormData({ ...formData, apellidos: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Alias</label>
                <input
                  type="text"
                  required
                  value={formData.alias}
                  onChange={e => setFormData({ ...formData, alias: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Grupo</label>
                <select
                  value={formData.groupName}
                  onChange={e => setFormData({ ...formData, groupName: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                >
                  <option value="">Seleccionar...</option>
                  <option value="General">General</option>
                  <option value="IT">IT</option>
                  <option value="RRHH">RRHH</option>
                  <option value="Ventas">Ventas</option>
                  <option value="Operaciones">Operaciones</option>
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Tipo</label>
                <select
                  value={formData.type}
                  onChange={e => setFormData({ ...formData, type: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                >
                  <option value="Standard">Estándar</option>
                  <option value="Manager">Manager</option>
                  <option value="Admin">Administrador</option>
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Estado</label>
                <select
                  value={formData.status}
                  onChange={e => setFormData({ ...formData, status: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                >
                  <option value="Active">Activo</option>
                  <option value="Inactive">Inactivo</option>
                  <option value="Leave">Excedencia</option>
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Idioma</label>
                <select
                  value={formData.idioma || 'es'}
                  onChange={e => setFormData({ ...formData, idioma: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                >
                  <option value="es">Español</option>
                  <option value="en">Inglés</option>
                  <option value="fr">Francés</option>
                  <option value="de">Alemán</option>
                  <option value="it">Italiano</option>
                  <option value="pt">Portugués</option>
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Tipo de Contrato</label>
                <input
                  type="text"
                  disabled
                  value={formData.contractType || ''}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg bg-gray-100 cursor-not-allowed"
                />
                <p className="text-xs text-gray-500 mt-1">Gestionado desde Contratos</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Fecha de Inicio</label>
                <input
                  type="date"
                  disabled
                  value={formData.startDate ? new Date(formData.startDate).toISOString().split('T')[0] : ''}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg bg-gray-100 cursor-not-allowed"
                />
                <p className="text-xs text-gray-500 mt-1">Gestionado desde Contratos</p>
              </div>
            </div>
          )}
        </div>

        {/* Section: Teletrabajo */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
          <button
            type="button"
            onClick={() => toggleSection('remote')}
            className="w-full flex items-center justify-between p-4 bg-gray-50 hover:bg-gray-100 transition-colors"
          >
            <span className="font-semibold text-gray-900">Teletrabajo</span>
            {activeSection === 'remote' ? <ChevronUp size={20} /> : <ChevronDown size={20} />}
          </button>
          
          {activeSection === 'remote' && (
            <div className="p-6">
               <label className="flex items-center gap-3 cursor-pointer">
                  <input
                    type="checkbox"
                    checked={formData.remoteWork || false}
                    onChange={e => setFormData({ ...formData, remoteWork: e.target.checked })}
                    className="w-4 h-4 text-indigo-600 rounded focus:ring-indigo-500"
                  />
                  <span className="text-sm font-medium text-gray-700">Habilitar Teletrabajo</span>
                </label>
                <p className="text-sm text-gray-500 mt-2">Configuración avanzada próximamente.</p>
            </div>
          )}
        </div>

        {/* Section: Notas */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
          <button
            type="button"
            onClick={() => toggleSection('notes')}
            className="w-full flex items-center justify-between p-4 bg-gray-50 hover:bg-gray-100 transition-colors"
          >
            <span className="font-semibold text-gray-900">Notas</span>
            {activeSection === 'notes' ? <ChevronUp size={20} /> : <ChevronDown size={20} />}
          </button>
          
          {activeSection === 'notes' && (
            <div className="p-6">
                <textarea
                  value={formData.notes || ''}
                  onChange={e => setFormData({ ...formData, notes: e.target.value })}
                  rows={5}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                  placeholder="Añade notas o comentarios sobre el empleado..."
                />
            </div>
          )}
        </div>

        {/* Section 2: Control & Access */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
          <button
            type="button"
            onClick={() => toggleSection('control')}
            className="w-full flex items-center justify-between p-4 bg-gray-50 hover:bg-gray-100 transition-colors"
          >
            <span className="font-semibold text-gray-900">Control y Accesos</span>
            {activeSection === 'control' ? <ChevronUp size={20} /> : <ChevronDown size={20} />}
          </button>
          
          {activeSection === 'control' && (
            <div className="p-6 space-y-6">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">ID Grupo Acceso</label>
                    <input
                    type="text"
                    value={formData.idAccessGroup || ''}
                    onChange={e => setFormData({ ...formData, idAccessGroup: e.target.value })}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                    />
                </div>
                <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">ID Biométrico</label>
                    <input
                    type="text"
                    value={formData.biometricID || ''}
                    onChange={e => setFormData({ ...formData, biometricID: e.target.value })}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                    />
                </div>
              </div>
              
              <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
                {[
                  { key: 'attControlled', label: 'Control de Presencia' },
                  { key: 'accControlled', label: 'Control de Acceso' },
                  { key: 'jobControlled', label: 'Control de Tareas' },
                  { key: 'extControlled', label: 'Control Externo' },
                  { key: 'riskControlled', label: 'Control de Riesgos' },
                ].map((item) => (
                  <label key={item.key} className="flex items-center gap-3 p-3 border border-gray-200 rounded-lg cursor-pointer hover:bg-gray-50">
                    <input
                      type="checkbox"
                      checked={(formData as any)[item.key]}
                      onChange={e => setFormData({ ...formData, [item.key]: e.target.checked })}
                      className="w-4 h-4 text-indigo-600 rounded focus:ring-indigo-500"
                    />
                    <span className="text-sm font-medium text-gray-700">{item.label}</span>
                  </label>
                ))}
              </div>
            </div>
          )}
        </div>

        {/* Section 3: Web Credentials */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
          <button
            type="button"
            onClick={() => toggleSection('credentials')}
            className="w-full flex items-center justify-between p-4 bg-gray-50 hover:bg-gray-100 transition-colors"
          >
            <span className="font-semibold text-gray-900">Credenciales Web</span>
            {activeSection === 'credentials' ? <ChevronUp size={20} /> : <ChevronDown size={20} />}
          </button>
          
          {activeSection === 'credentials' && (
            <div className="p-6 grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Login Web</label>
                <input
                  type="text"
                  value={formData.webLogin || ''}
                  onChange={e => setFormData({ ...formData, webLogin: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Password Web</label>
                <input
                  type="password"
                  value={formData.webPassword || ''}
                  onChange={e => setFormData({ ...formData, webPassword: e.target.value })}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                />
              </div>
              <div className="md:col-span-2">
                <label className="flex items-center gap-3 p-3 border border-gray-200 rounded-lg cursor-pointer hover:bg-gray-50 w-fit">
                    <input
                      type="checkbox"
                      checked={formData.activeDirectory}
                      onChange={e => setFormData({ ...formData, activeDirectory: e.target.checked })}
                      className="w-4 h-4 text-indigo-600 rounded focus:ring-indigo-500"
                    />
                    <span className="text-sm font-medium text-gray-700">Usar Active Directory</span>
                </label>
              </div>
            </div>
          )}
        </div>

        {/* Section 4: Roles & Permissions */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
          <button
            type="button"
            onClick={() => toggleSection('roles')}
            className="w-full flex items-center justify-between p-4 bg-gray-50 hover:bg-gray-100 transition-colors"
          >
            <span className="font-semibold text-gray-900">Roles y Permisos</span>
            {activeSection === 'roles' ? <ChevronUp size={20} /> : <ChevronDown size={20} />}
          </button>
          
          {activeSection === 'roles' && (
            <div className="p-0">
              <div className="p-6 border-b border-gray-100 bg-gray-50/50 flex flex-col md:flex-row md:items-center justify-between gap-4">
                <div>
                  <p className="text-sm text-gray-500">
                    {(formData.roles || []).length} roles seleccionados
                  </p>
                </div>
                <div className="relative w-full md:w-64">
                  <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" size={18} />
                  <input
                    type="text"
                    placeholder="Buscar roles..."
                    value={roleSearchTerm}
                    onChange={(e) => setRoleSearchTerm(e.target.value)}
                    className="w-full pl-10 pr-4 py-2 border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500 text-sm"
                  />
                </div>
              </div>
              
              <div className="max-h-96 overflow-y-auto p-4">
                {displayedRoles.length === 0 ? (
                  <div className="text-center py-8 text-gray-500">
                    No se encontraron roles que coincidan con tu búsqueda.
                  </div>
                ) : (
                  <>
                    {!roleSearchTerm && !showAllRoles && (
                      <div className="mb-4 flex items-center justify-between px-1">
                        <h3 className="text-xs font-semibold text-gray-500 uppercase tracking-wider">Roles Frecuentes</h3>
                        <button 
                          type="button"
                          onClick={() => setShowAllRoles(true)}
                          className="text-sm text-indigo-600 hover:text-indigo-700 font-medium hover:underline"
                        >
                          Ver todos los roles ({availableRoles.length})
                        </button>
                      </div>
                    )}
                    
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-3">
                      {displayedRoles.map(role => {
                        const isSelected = (formData.roles || []).some(r => r.rolName === role.name);
                        return (
                          <div
                            key={role.id}
                            onClick={() => toggleRole(role.name)}
                            className={`p-3 rounded-lg border cursor-pointer transition-all flex items-start gap-3 ${
                              isSelected
                                ? 'bg-indigo-50 border-indigo-200 ring-1 ring-indigo-500'
                                : 'bg-white border-gray-200 hover:border-indigo-300 hover:bg-gray-50'
                            }`}
                          >
                            <div className={`mt-0.5 p-1.5 rounded-md flex-shrink-0 ${
                              isSelected ? 'bg-indigo-100 text-indigo-600' : 'bg-gray-100 text-gray-400'
                            }`}>
                              {isSelected ? <CheckCircle size={16} /> : <Shield size={16} />}
                            </div>
                            <div className="min-w-0">
                              <h3 className={`text-sm font-medium truncate ${
                                isSelected ? 'text-indigo-900' : 'text-gray-900'
                              }`}>
                                {role.name}
                              </h3>
                              <p className="text-xs text-gray-500 mt-0.5 line-clamp-2">{role.description}</p>
                            </div>
                          </div>
                        );
                      })}
                    </div>

                    {!roleSearchTerm && showAllRoles && (
                      <div className="mt-4 text-center">
                        <button 
                          type="button"
                          onClick={() => setShowAllRoles(false)}
                          className="text-sm text-gray-500 hover:text-gray-700 hover:underline"
                        >
                          Mostrar solo roles frecuentes
                        </button>
                      </div>
                    )}
                  </>
                )}
              </div>
              
              {/* Selected Roles Summary Footer */}
              {(formData.roles || []).length > 0 && (
                <div className="bg-gray-50 px-6 py-3 border-t border-gray-100 flex flex-wrap gap-2">
                  <span className="text-xs font-medium text-gray-500 py-1">Seleccionados:</span>
                  {(formData.roles || []).map((role, idx) => (
                    <span key={idx} className="inline-flex items-center gap-1 px-2 py-1 rounded-full bg-indigo-100 text-indigo-700 text-xs font-medium">
                      {role.rolName}
                      <button 
                        type="button"
                        onClick={(e) => { e.stopPropagation(); toggleRole(role.rolName); }}
                        className="hover:text-indigo-900"
                      >
                        <XCircle size={12} />
                      </button>
                    </span>
                  ))}
                </div>
              )}
            </div>
          )}
        </div>

      </form>
    </div>
  );
};

export default EmployeeFormPage;
