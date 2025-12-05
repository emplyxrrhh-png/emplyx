import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { ArrowLeft, Save, Building2 } from 'lucide-react';
import { AccordionSection } from '../../../components/AccordionSection';
import { API_BASE_URL } from '../../../config';

// Define the full interface based on the backend DTO
// For brevity, I'll use 'any' for some nested objects or define them loosely
// In a real app, strict typing is better.

const initialData = {
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
  
  employeeCount: 0,
  collectiveAgreement: '',
  cnae: '',
  workDayType: '',
  hasShifts: false,
  
  relationType: '',
  segment: '',
  sector: '',
  companySize: '',
  annualRevenue: 0,
  source: '',
  internalAccountManager: '',
  status: '',
  
  portalAccess: false,
  adminUser: { name: '', email: '', phone: '' },
  language: '',
  timeZone: '',
  
  commercialComms: false,
  operationalComms: false,
  preferredChannel: '',
  gdprConsent: false,
  termsAccepted: false,
  privacyAccepted: false,
  acceptanceDate: '',
  acceptanceIP: '',
  
  internalNotes: '',
  tags: ''
};

const TenantFormContext = React.createContext<{
  formData: any;
  handleChange: (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => void;
} | null>(null);

const Input = ({ label, name, type = "text", required = false, placeholder = "" }: any) => {
  const context = React.useContext(TenantFormContext);
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
  const context = React.useContext(TenantFormContext);
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

const TenantFormPage = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const [formData, setFormData] = useState(initialData);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (id) {
      fetchTenant(id);
    }
  }, [id]);

  const fetchTenant = async (tenantId: string) => {
    try {
      const response = await fetch(`${API_BASE_URL}/tenants/${tenantId}`);
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
      console.error('Error fetching tenant', error);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      const url = id 
        ? `${API_BASE_URL}/tenants/${id}`
        : `${API_BASE_URL}/tenants`;
      
      const method = id ? 'PUT' : 'POST';

      // Prepare payload (handle dates, numbers, etc if needed)
      const payload = {
        ...formData,
        dateOfConstitution: formData.dateOfConstitution || null,
        acceptanceDate: formData.acceptanceDate || null,
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
        navigate('/tenants');
      } else {
        console.error('Error saving tenant');
      }
    } catch (error) {
      console.error('Error saving tenant', error);
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
    <TenantFormContext.Provider value={{ formData, handleChange }}>
      <div className="p-6 max-w-5xl mx-auto">
      <div className="flex items-center gap-4 mb-6">
        <button
          onClick={() => navigate('/tenants')}
          className="p-2 hover:bg-gray-100 rounded-full transition-colors"
        >
          <ArrowLeft size={24} className="text-gray-600" />
        </button>
        <div>
          <h1 className="text-2xl font-bold text-gray-900 flex items-center gap-2">
            <Building2 className="h-8 w-8 text-indigo-600" />
            {id ? 'Editar Tenant' : 'Nuevo Tenant'}
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

        <AccordionSection title="2. Direcciones">
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

        <AccordionSection title="3. Datos de Contacto">
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

        <AccordionSection title="4. Datos Fiscales y Facturación">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
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
        </AccordionSection>

        <AccordionSection title="5. Datos Bancarios">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <Input label="Titular Cuenta" name="bankAccount.accountHolder" />
            <Input label="IBAN" name="bankAccount.iban" />
            <Input label="BIC/SWIFT" name="bankAccount.bic" />
            <Input label="Banco" name="bankAccount.bankName" />
            <div className="flex items-end mb-2"><Checkbox label="Autorización SEPA" name="bankAccount.sepaAuth" /></div>
            <Input label="Fecha Firma SEPA" name="bankAccount.sepaAuthDate" type="date" />
            <Input label="Referencia Mandato" name="bankAccount.sepaReference" />
          </div>
        </AccordionSection>

        <AccordionSection title="6. Datos Laborales / RRHH">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <Input label="Nº Empleados" name="employeeCount" type="number" />
            <Input label="Convenio Colectivo" name="collectiveAgreement" />
            <Input label="CNAE" name="cnae" />
            <Input label="Tipo Jornada" name="workDayType" />
            <div className="flex items-end mb-2"><Checkbox label="Turnos de Trabajo" name="hasShifts" /></div>
          </div>
        </AccordionSection>

        <AccordionSection title="7. Datos Comerciales">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <Input label="Tipo Relación" name="relationType" />
            <Input label="Segmento" name="segment" />
            <Input label="Sector" name="sector" />
            <Input label="Tamaño Empresa" name="companySize" />
            <Input label="Facturación Anual" name="annualRevenue" type="number" />
            <Input label="Origen" name="source" />
            <Input label="Account Manager" name="internalAccountManager" />
            <Input label="Estado" name="status" />
          </div>
        </AccordionSection>

        <AccordionSection title="8. Configuración de Acceso">
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

        <AccordionSection title="9. Preferencias y Consentimientos">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <Checkbox label="Recibir Comunicaciones Comerciales" name="commercialComms" />
            <Checkbox label="Recibir Avisos Operativos" name="operationalComms" />
            <Input label="Canal Preferido" name="preferredChannel" />
            <Checkbox label="Consentimiento RGPD" name="gdprConsent" />
            <Checkbox label="Aceptación Términos" name="termsAccepted" />
            <Checkbox label="Aceptación Privacidad" name="privacyAccepted" />
            <Input label="Fecha Aceptación" name="acceptanceDate" type="date" />
            <Input label="IP Aceptación" name="acceptanceIP" />
          </div>
        </AccordionSection>

        <AccordionSection title="10. Documentación Adjunta">
          <div className="p-4 bg-gray-50 border border-gray-200 rounded-lg text-center text-gray-500">
            <p>La gestión documental estará disponible próximamente.</p>
          </div>
        </AccordionSection>



        <div className="mt-8 flex justify-end gap-3">
          <button
            type="button"
            onClick={() => navigate('/tenants')}
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
            {loading ? 'Guardando...' : 'Guardar Tenant'}
          </button>
        </div>
      </form>
    </div>
    </TenantFormContext.Provider>
  );
};

export default TenantFormPage;
