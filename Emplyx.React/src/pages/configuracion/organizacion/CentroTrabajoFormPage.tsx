import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { ArrowLeft, Save, MapPin } from 'lucide-react';
import { AccordionSection } from '../../../components/AccordionSection';
import { useOrganization } from '../../../context/OrganizationContext';
import { API_BASE_URL } from '../../../config';

const initialData = {
  internalId: '',
  nombre: '',
  empresaId: '',
  
  address: { street: '', zipCode: '', city: '', province: '', country: '' },
  
  contact: { name: '', phone: '', email: '' },
  
  timeZone: '',
  language: '',
  
  isActive: true
};

const CentroTrabajoFormContext = React.createContext<{
  formData: any;
  handleChange: (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => void;
} | null>(null);

const Input = ({ label, name, type = "text", required = false, placeholder = "" }: any) => {
  const context = React.useContext(CentroTrabajoFormContext);
  if (!context) return null;
  const { formData, handleChange } = context;
  
  const value = name.includes('.') 
    ? name.split('.').reduce((o: any, i: any) => o ? o[i] : '', formData) 
    : (formData as any)[name];

  return (
    <div>
      <label className="block text-sm font-medium text-gray-700 mb-1">
        {label} {required && <span className="text-red-500">*</span>}
      </label>
      <input
        type={type}
        name={name}
        value={value || ''}
        onChange={handleChange}
        placeholder={placeholder}
        className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-all"
        required={required}
      />
    </div>
  );
};

const Select = ({ label, name, options, required = false }: any) => {
  const context = React.useContext(CentroTrabajoFormContext);
  if (!context) return null;
  const { formData, handleChange } = context;

  const value = name.includes('.') 
    ? name.split('.').reduce((o: any, i: any) => o ? o[i] : '', formData) 
    : (formData as any)[name];

  return (
    <div>
      <label className="block text-sm font-medium text-gray-700 mb-1">
        {label} {required && <span className="text-red-500">*</span>}
      </label>
      <select
        name={name}
        value={value || ''}
        onChange={handleChange}
        className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-all"
        required={required}
      >
        <option value="">Seleccionar...</option>
        {options.map((opt: any) => (
          <option key={opt.value} value={opt.value}>{opt.label}</option>
        ))}
      </select>
    </div>
  );
};

const CentroTrabajoFormPage = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const { selectedCompany } = useOrganization();
  const [formData, setFormData] = useState(initialData);
  const [empresas, setEmpresas] = useState<any[]>([]);

  useEffect(() => {
    const fetchEmpresas = async () => {
      if (selectedCompany?.type === 'tenant') {
        try {
          const response = await fetch(`${API_BASE_URL}/empresas?tenantId=${selectedCompany.id}`);
          if (response.ok) {
            const data = await response.json();
            setEmpresas(data);
          }
        } catch (e) {
          console.error("Error fetching empresas", e);
        }
      }
    };
    fetchEmpresas();
  }, [selectedCompany]);
  
  useEffect(() => {
    if (selectedCompany && !id) {
        if (selectedCompany.type === 'empresa') {
            setFormData(prev => ({
                ...prev,
                empresaId: selectedCompany.id
            }));
        }
    }

    if (id) {
      const fetchCentro = async () => {
        try {
          const response = await fetch(`${API_BASE_URL}/centros-trabajo/${id}`);
          if (response.ok) {
            const data = await response.json();
            setFormData(data);
          } else {
            console.error('Error fetching centro');
            alert('No se pudo cargar el centro de trabajo');
            navigate('/configuracion/organizacion/centros-trabajo');
          }
        } catch (error) {
          console.error('Error fetching centro', error);
          alert('Error de conexión');
          navigate('/configuracion/organizacion/centros-trabajo');
        }
      };
      fetchCentro();
    }
  }, [id, selectedCompany, navigate]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value, type } = e.target;
    
    if (name.includes('.')) {
      const [parent, child] = name.split('.');
      setFormData(prev => ({
        ...prev,
        [parent]: {
          ...(prev as any)[parent],
          [child]: value
        }
      }));
    } else {
      setFormData(prev => ({
        ...prev,
        [name]: type === 'checkbox' ? (e.target as HTMLInputElement).checked : value
      }));
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    try {
      const url = id 
        ? `${API_BASE_URL}/centros-trabajo/${id}`
        : `${API_BASE_URL}/centros-trabajo`;
      
      const method = id ? 'PUT' : 'POST';
      
      const response = await fetch(url, {
        method,
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(formData),
      });

      if (response.ok) {
        navigate('/configuracion/organizacion/centros-trabajo');
      } else {
        console.error('Error saving centro trabajo');
        alert('Error al guardar. Asegúrese de que el servidor está funcionando y la base de datos está actualizada.');
      }
    } catch (error) {
      console.error('Error saving centro trabajo', error);
      alert('Error de conexión al guardar.');
    }
  };

  return (
    <CentroTrabajoFormContext.Provider value={{ formData, handleChange }}>
      <form onSubmit={handleSubmit} className="p-6 max-w-5xl mx-auto">
        <div className="flex items-center justify-between mb-6">
          <div className="flex items-center gap-4">
            <button
              type="button"
              onClick={() => navigate('/configuracion/organizacion/centros-trabajo')}
              className="p-2 hover:bg-gray-100 rounded-full transition-colors"
            >
              <ArrowLeft size={24} className="text-gray-600" />
            </button>
            <div>
              <h1 className="text-2xl font-bold text-gray-900 flex items-center gap-2">
                <MapPin className="h-8 w-8 text-indigo-600" />
                {id ? 'Editar Centro de Trabajo' : 'Nuevo Centro de Trabajo'}
              </h1>
              <p className="text-gray-500 mt-1">
                {id ? 'Modifica los datos del centro de trabajo' : 'Registra un nuevo centro de trabajo'}
              </p>
            </div>
          </div>
          <button
            type="submit"
            className="bg-indigo-600 text-white px-6 py-2 rounded-lg flex items-center gap-2 hover:bg-indigo-700 transition-colors shadow-sm"
          >
            <Save size={20} />
            Guardar
          </button>
        </div>

        <div className="space-y-6">
          {/* Datos Principales */}
          <AccordionSection title="Datos Principales" defaultOpen={true}>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <Input label="Nombre del Centro" name="nombre" required placeholder="Ej: Oficina Central" />
              <Input label="Código Interno" name="internalId" placeholder="Ej: CT-001" />
              
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Empresa {selectedCompany?.type === 'tenant' && <span className="text-red-500">*</span>}
                </label>
                {selectedCompany?.type === 'tenant' ? (
                    <select
                        name="empresaId"
                        value={formData.empresaId}
                        onChange={handleChange}
                        className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent transition-all"
                        required
                    >
                        <option value="">Seleccionar Empresa...</option>
                        {empresas.map((e: any) => (
                            <option key={e.id} value={e.id}>{e.tradeName || e.legalName || e.nombre}</option>
                        ))}
                    </select>
                ) : (
                    <>
                        <input
                        type="text"
                        value={selectedCompany?.name || ''}
                        disabled
                        className="w-full px-3 py-2 border border-gray-300 rounded-lg bg-gray-100 text-gray-500 cursor-not-allowed"
                        />
                        <input type="hidden" name="empresaId" value={formData.empresaId} />
                    </>
                )}
              </div>

              <div className="flex items-center mt-6">
                <label className="flex items-center gap-2 cursor-pointer">
                  <input
                    type="checkbox"
                    name="isActive"
                    checked={formData.isActive}
                    onChange={handleChange}
                    className="w-4 h-4 text-indigo-600 rounded focus:ring-indigo-500 border-gray-300"
                  />
                  <span className="text-sm font-medium text-gray-700">Centro Activo</span>
                </label>
              </div>
            </div>
          </AccordionSection>

          {/* Dirección */}
          <AccordionSection title="Dirección">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="md:col-span-2">
                <Input label="Dirección" name="address.street" placeholder="Calle, número, piso..." />
              </div>
              <Input label="Código Postal" name="address.zipCode" />
              <Input label="Ciudad" name="address.city" />
              <Input label="Provincia" name="address.province" />
              <Input label="País" name="address.country" />
            </div>
          </AccordionSection>

          {/* Contacto */}
          <AccordionSection title="Información de Contacto">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <Input label="Persona de Contacto" name="contact.name" />
              <Input label="Teléfono" name="contact.phone" />
              <Input label="Email" name="contact.email" type="email" />
            </div>
          </AccordionSection>

          {/* Configuración Regional */}
          <AccordionSection title="Configuración Regional">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <Select 
                label="Zona Horaria" 
                name="timeZone" 
                options={[
                  { value: 'Europe/Madrid', label: '(GMT+01:00) Madrid' },
                  { value: 'Europe/London', label: '(GMT+00:00) London' },
                  { value: 'America/New_York', label: '(GMT-05:00) New York' },
                ]} 
              />
              <Select 
                label="Idioma" 
                name="language" 
                options={[
                  { value: 'es', label: 'Español' },
                  { value: 'en', label: 'English' },
                  { value: 'fr', label: 'Français' },
                ]} 
              />
            </div>
          </AccordionSection>
        </div>
      </form>
    </CentroTrabajoFormContext.Provider>
  );
};

export default CentroTrabajoFormPage;
