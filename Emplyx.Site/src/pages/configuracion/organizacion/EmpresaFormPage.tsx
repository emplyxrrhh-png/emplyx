import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { ArrowLeft, Save, Building2 } from 'lucide-react';
import { AccordionSection } from '../../../components/AccordionSection';
import { API_BASE_URL } from '../../../../config';

// Define the full interface based on the backend DTO
// For brevity, I'll use 'any' for some nested objects or define them loosely
// In a real app, strict typing is better.

const initialData = {
  // Inheritance flags
  inheritAddresses: false,
  inheritContact: false,
  inheritFiscal: false,
  inheritBank: false,
  inheritAccess: false,

  internalId: '',
  companyType: '',
  legalName: '',
  tradeName: '',
  taxId: '',
  countryOfConstitution: '',
  dateOfConstitution: '',
  commercialRegister: '',
  provinceOfRegister: '',
  registerDetails: '',
  
  legalAddress: { street: '', zipCode: '', city: '', province: '', country: '' },
  fiscalAddress: { street: '', zipCode: '', city: '', province: '', country: '' },
  
  mainContact: { name: '', jobTitle: '', phone: '', mobile: '', email: '' },
  generalPhone: '',
  generalEmail: '',
  billingEmail: '',
  website: '',
  socialMedia: '',
  
  vatRegime: '',
  intraCommunityVAT: '',
  irpfRegime: '',
  currency: 'EUR',
  paymentMethod: '',
  paymentTerm: '',
  creditLimit: 0,
  poRequired: false,
  invoiceDeliveryMethod: '',
  billingNotes: '',
  
  bankAccount: { accountHolder: '', iban: '', bic: '', bankName: '', sepaAuth: false, sepaAuthDate: '', sepaReference: '' },
  
  portalAccess: false,
  adminUser: { name: '', email: '', phone: '' },
  language: '',
  timeZone: '',
  
  internalNotes: '',
  tags: ''
};

const EmpresaFormContext = React.createContext<{
  formData: any;
  handleChange: (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => void;
} | null>(null);

const SectionHeader = ({ title, name }: { title: string, name: string }) => {
  const context = React.useContext(EmpresaFormContext);
  if (!context) return <span>{title}</span>;
  const { formData, handleChange } = context;
  
  return (
    <div className="flex items-center justify-between w-full pr-4">
      <span>{title}</span>
      <label 
        className="flex items-center gap-2 text-sm font-normal text-indigo-600 cursor-pointer bg-indigo-50 px-3 py-1 rounded-full border border-indigo-100 hover:bg-indigo-100 transition-colors"
        onClick={(e) => e.stopPropagation()}
      >
        <input 
            type="checkbox" 
            name={name}
            checked={!!formData[name]}
            onChange={handleChange}
            className="w-4 h-4 text-indigo-600 rounded focus:ring-indigo-500 border-gray-300"
        />
        Se gestiona en el tenant
      </label>
    </div>
  );
};

const Input = ({ label, name, type = "text", required = false, placeholder = "" }: any) => {
  const context = React.useContext(EmpresaFormContext);
  if (!context) return null;
  const { formData, handleChange } = context;
  
  const value = name.includes('.') 
    ? name.split('.').reduce((o: any, i: any) => o ? o[i] : '', formData) 
    : (formData as any)[name];

  return (
    <div className="space-y-1">
      <label className="text-sm font-medium text-gray-700">{label} {required && '*'}</label>
      <input
        type={type}
        name={name}
        required={required}
        value={value || ''}
        onChange={handleChange}
        className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 outline-none transition-all"
        placeholder={placeholder}
      />
    </div>
  );
};

const Checkbox = ({ label, name }: any) => {
  const context = React.useContext(EmpresaFormContext);
  if (!context) return null;
  const { formData, handleChange } = context;

  const checked = name.includes('.')
      ? name.split('.').reduce((o: any, i: any) => o ? o[i] : false, formData)
      : (formData as any)[name];

  return (
    <div className="flex items-center gap-2">
      <input
        type="checkbox"
        name={name}
        checked={!!checked}
        onChange={handleChange}
        className="w-4 h-4 text-indigo-600 border-gray-300 rounded focus:ring-indigo-500"
      />
      <label className="text-sm text-gray-700">{label}</label>
    </div>
  );
};

const EmpresaFormPage = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const [formData, setFormData] = useState(initialData);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (id) {
      fetchEmpresa(id);
    }
  }, [id]);

  const fetchEmpresa = async (empresaId: string) => {
    try {
      const response = await fetch(`${API_BASE_URL}/empresas/${empresaId}`);
      if (response.ok) {
        const data = await response.json();
        // Ensure nested objects are not null
        setFormData({
          ...initialData,
          ...data,
          legalAddress: data.legalAddress || initialData.legalAddress,
          fiscalAddress: data.fiscalAddress || initialData.fiscalAddress,
          mainContact: data.mainContact || initialData.mainContact,
          adminUser: data.adminUser || initialData.adminUser,
          // Handle dates
          dateOfConstitution: data.dateOfConstitution ? data.dateOfConstitution.split('T')[0] : '',
          acceptanceDate: data.acceptanceDate ? data.acceptanceDate.split('T')[0] : '',
          bankAccount: {
             ...(data.bankAccount || initialData.bankAccount),
             sepaAuthDate: data.bankAccount?.sepaAuthDate ? data.bankAccount.sepaAuthDate.split('T')[0] : ''
          }
        });
      }
    } catch (error) {
      console.error('Error fetching empresa', error);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      // Validation: Check for duplicate NIF/CIF
      try {
        const checkResponse = await fetch(`${API_BASE_URL}/empresas`);
        if (checkResponse.ok) {
          const companies = await checkResponse.json();
          // Check if any other company has the same taxId
          const duplicate = companies.find((c: any) => 
            c.taxId?.toLowerCase() === formData.taxId?.toLowerCase() && c.id !== id
          );
          
          if (duplicate) {
            alert('No se puede guardar: Ya existe una empresa con el mismo NIF/CIF.');
            setLoading(false);
            return;
          }
        }
      } catch (checkError) {
        console.error('Error checking for duplicates', checkError);
        // Continue with save if check fails? Or block? 
        // Usually safer to block or warn, but for now let's log and proceed or maybe return?
        // Let's assume if we can't check, we might get a backend error anyway.
      }

      const url = id 
        ? `${API_BASE_URL}/empresas/${id}`
        : `${API_BASE_URL}/empresas`;
      
      const method = id ? 'PUT' : 'POST';

      // Prepare payload (handle dates, numbers, etc if needed)
      const payload = {
        ...formData,
        dateOfConstitution: formData.dateOfConstitution || null,
        bankAccount: {
            ...formData.bankAccount,
            sepaAuthDate: formData.bankAccount.sepaAuthDate || null
        }
      };

      const response = await fetch(url, {
        method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload),
      });

      if (response.ok) {
        navigate('/configuracion/organizacion/empresas');
      } else {
        console.error('Error saving empresa');
      }
    } catch (error) {
      console.error('Error saving empresa', error);
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value, type } = e.target;
    
    // Handle nested properties like "legalAddress.street"
    if (name.includes('.')) {
      const [parent, child] = name.split('.');
      setFormData(prev => ({
        ...prev,
        [parent]: {
          ...prev[parent as keyof typeof prev] as any,
          [child]: value
        }
      }));
    } else {
      const val = type === 'checkbox' ? (e.target as HTMLInputElement).checked : value;
      setFormData(prev => ({ ...prev, [name]: val }));
    }
  };

  return (
    <EmpresaFormContext.Provider value={{ formData, handleChange }}>
      <div className="p-6 max-w-5xl mx-auto">
      <div className="flex items-center gap-4 mb-6">
        <button
          onClick={() => navigate('/configuracion/organizacion/empresas')}
          className="p-2 hover:bg-gray-100 rounded-full transition-colors"
        >
          <ArrowLeft size={24} className="text-gray-600" />
        </button>
        <div>
          <h1 className="text-2xl font-bold text-gray-900 flex items-center gap-2">
            <Building2 className="h-8 w-8 text-indigo-600" />
            {id ? 'Editar Empresa' : 'Nueva Empresa'}
          </h1>
        </div>
      </div>

      <form onSubmit={handleSubmit}>
        
        <AccordionSection title="1. Datos Identificativos" defaultOpen={true}>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <Input label="Tipo de Empresa" name="companyType" required placeholder="SL, SA, Autónomo..." />
            <Input label="Razón Social" name="legalName" required />
            <Input label="Nombre Comercial" name="tradeName" />
            <Input label="NIF/CIF" name="taxId" required />
            <Input label="País de Constitución" name="countryOfConstitution" />
            <Input label="Fecha Constitución" name="dateOfConstitution" type="date" />
            <Input label="Registro Mercantil" name="commercialRegister" />
            <Input label="Provincia Registro" name="provinceOfRegister" />
            <div className="md:col-span-3">
              <Input label="Datos Registrales (Tomo, Libro, Folio...)" name="registerDetails" />
            </div>
          </div>
        </AccordionSection>

        <AccordionSection 
          title={<SectionHeader title="2. Direcciones" name="inheritAddresses" />}
          isOpen={formData.inheritAddresses ? false : undefined}
        >
          <h4 className="font-medium text-gray-900 mb-3">Domicilio Social</h4>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
            <div className="md:col-span-3"><Input label="Calle / Nº / Piso" name="legalAddress.street" /></div>
            <Input label="Código Postal" name="legalAddress.zipCode" />
            <Input label="Ciudad" name="legalAddress.city" />
            <Input label="Provincia" name="legalAddress.province" />
            <Input label="País" name="legalAddress.country" />
          </div>
          
          <h4 className="font-medium text-gray-900 mb-3">Domicilio Fiscal</h4>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div className="md:col-span-3"><Input label="Calle / Nº / Piso" name="fiscalAddress.street" /></div>
            <Input label="Código Postal" name="fiscalAddress.zipCode" />
            <Input label="Ciudad" name="fiscalAddress.city" />
            <Input label="Provincia" name="fiscalAddress.province" />
            <Input label="País" name="fiscalAddress.country" />
          </div>
        </AccordionSection>

        <AccordionSection 
          title={<SectionHeader title="3. Datos de Contacto" name="inheritContact" />}
          isOpen={formData.inheritContact ? false : undefined}
        >
          <h4 className="font-medium text-gray-900 mb-3">Persona de Contacto Principal</h4>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
            <Input label="Nombre y Apellidos" name="mainContact.name" required />
            <Input label="Cargo" name="mainContact.jobTitle" />
            <Input label="Email" name="mainContact.email" type="email" />
            <Input label="Teléfono Fijo" name="mainContact.phone" />
            <Input label="Móvil" name="mainContact.mobile" />
          </div>

          <h4 className="font-medium text-gray-900 mb-3">Contacto General</h4>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <Input label="Teléfono General" name="generalPhone" />
            <Input label="Email General" name="generalEmail" type="email" />
            <Input label="Email Facturación" name="billingEmail" type="email" />
            <Input label="Web" name="website" />
            <div className="md:col-span-2"><Input label="Redes Sociales" name="socialMedia" placeholder="LinkedIn, Twitter..." /></div>
          </div>
        </AccordionSection>

        <AccordionSection 
          title={<SectionHeader title="4. Datos Fiscales" name="inheritFiscal" />}
          isOpen={formData.inheritFiscal ? false : undefined}
        >
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
            <Input label="Régimen IVA" name="vatRegime" />
            <Input label="NIF-IVA Intracomunitario" name="intraCommunityVAT" />
            <Input label="Régimen IRPF" name="irpfRegime" />
            <Input label="Moneda" name="currency" />
            <Input label="Forma de Pago" name="paymentMethod" />
            <Input label="Plazo de Pago" name="paymentTerm" />
            <Input label="Límite Crédito" name="creditLimit" type="number" />
            <div className="flex items-end mb-2"><Checkbox label="Número de Pedido Obligatorio" name="poRequired" /></div>
            <Input label="Envío Factura" name="invoiceDeliveryMethod" placeholder="Email, Portal..." />
            <div className="md:col-span-3"><Input label="Observaciones Facturación" name="billingNotes" /></div>
          </div>

          <h4 className="font-medium text-gray-900 mb-3">Datos Bancarios</h4>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <Input label="Titular Cuenta" name="bankAccount.accountHolder" />
            <Input label="IBAN" name="bankAccount.iban" />
            <Input label="BIC/SWIFT" name="bankAccount.bic" />
            <Input label="Banco" name="bankAccount.bankName" />
            <Input label="Fecha Firma SEPA" name="bankAccount.sepaAuthDate" type="date" />
            <Input label="Referencia Mandato" name="bankAccount.sepaReference" />
            <div className="flex items-end mb-2"><Checkbox label="Autorización SEPA" name="bankAccount.sepaAuth" /></div>
          </div>
        </AccordionSection>

        <AccordionSection 
          title={<SectionHeader title="5. Configuración de Acceso" name="inheritAccess" />}
          isOpen={formData.inheritAccess ? false : undefined}
        >
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-4">
            <div className="flex items-end mb-2"><Checkbox label="Acceso a Portal" name="portalAccess" /></div>
            <Input label="Idioma" name="language" />
            <Input label="Zona Horaria" name="timeZone" />
          </div>
          <h4 className="font-medium text-gray-900 mb-3">Usuario Administrador</h4>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <Input label="Nombre" name="adminUser.name" />
            <Input label="Email" name="adminUser.email" type="email" />
            <Input label="Teléfono" name="adminUser.phone" />
          </div>
        </AccordionSection>

        <AccordionSection title="6. Documentación Adjunta">
          <div className="p-4 bg-gray-50 border border-gray-200 rounded-lg text-center text-gray-500">
            <p>La gestión documental estará disponible próximamente.</p>
          </div>
        </AccordionSection>

        <div className="mt-8 flex justify-end gap-3">
          <button
            type="button"
            onClick={() => navigate('/configuracion/organizacion/empresas')}
            className="px-4 py-2 text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors"
          >
            Cancelar
          </button>
          <button
            type="submit"
            disabled={loading}
            className="px-4 py-2 text-white bg-indigo-600 rounded-lg hover:bg-indigo-700 transition-colors flex items-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <Save size={20} />
            {loading ? 'Guardando...' : 'Guardar Empresa'}
          </button>
        </div>
      </form>
    </div>
    </EmpresaFormContext.Provider>
  );
};

export default EmpresaFormPage;
