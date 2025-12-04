import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { ArrowLeft, Save, ChevronDown, ChevronUp, Trash2 } from 'lucide-react';
import { useOrganization } from '../../../context/OrganizationContext';
import { CreateEmployeeRequest, Employee, UpdateEmployeeRequest } from '../../../types/employee';

const EmployeeFormPage = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const { selectedCompany } = useOrganization();
  const [isLoading, setIsLoading] = useState(false);
  const [activeSection, setActiveSection] = useState<string | null>('general');

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
      const response = await fetch(`https://localhost:5001/api/employees/${id}`);
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

  const handleImageUpload = async (file: File) => {
    setIsLoading(true);
    try {
      const formDataUpload = new FormData();
      formDataUpload.append('file', file);

      const response = await fetch('https://localhost:5001/api/employees/upload-image', {
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
        ? `https://localhost:5001/api/employees/${id}`
        : 'https://localhost:5001/api/employees';
      
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
              {id ? 'Editar Empleado' : 'Nuevo Empleado'}
            </h1>
            <p className="text-gray-500">
              {id ? 'Modifica los datos del empleado' : 'Añade un nuevo empleado a la organización'}
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

        {/* Section 4: Custom Fields */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
          <button
            type="button"
            onClick={() => toggleSection('custom')}
            className="w-full flex items-center justify-between p-4 bg-gray-50 hover:bg-gray-100 transition-colors"
          >
            <span className="font-semibold text-gray-900">Campos Personalizados</span>
            {activeSection === 'custom' ? <ChevronUp size={20} /> : <ChevronDown size={20} />}
          </button>
          
          {activeSection === 'custom' && (
            <div className="p-6 space-y-4">
              <div className="flex justify-end">
                <button
                  type="button"
                  onClick={() => {
                    const newKey = `Campo ${Object.keys(formData.userFields || {}).length + 1}`;
                    setFormData({
                      ...formData,
                      userFields: { ...formData.userFields, [newKey]: '' }
                    });
                  }}
                  className="text-sm text-indigo-600 hover:text-indigo-700 font-medium"
                >
                  + Añadir Campo
                </button>
              </div>
              
              {Object.entries(formData.userFields || {}).map(([key, value], index) => (
                <div key={index} className="flex gap-4 items-start">
                  <div className="flex-1">
                    <input
                      type="text"
                      value={key}
                      onChange={(e) => {
                        const newKey = e.target.value;
                        if (newKey && !formData.userFields?.[newKey]) {
                           const newFields = { ...formData.userFields };
                           const currentValue = newFields[key];
                           delete newFields[key];
                           newFields[newKey] = currentValue;
                           setFormData({ ...formData, userFields: newFields });
                        }
                      }}
                      className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                      placeholder="Nombre del campo"
                    />
                  </div>
                  <div className="flex-1">
                    <input
                      type="text"
                      value={value}
                      onChange={(e) => {
                        setFormData({
                          ...formData,
                          userFields: { ...formData.userFields, [key]: e.target.value }
                        });
                      }}
                      className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
                      placeholder="Valor"
                    />
                  </div>
                  <button
                    type="button"
                    onClick={() => {
                      const newFields = { ...formData.userFields };
                      delete newFields[key];
                      setFormData({ ...formData, userFields: newFields });
                    }}
                    className="p-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                  >
                    <Trash2 size={20} />
                  </button>
                </div>
              ))}
              
              {Object.keys(formData.userFields || {}).length === 0 && (
                <p className="text-center text-gray-500 py-4">No hay campos personalizados definidos.</p>
              )}
            </div>
          )}
        </div>

      </form>
    </div>
  );
};

export default EmployeeFormPage;
