import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { ArrowLeft, Save, Building2 } from 'lucide-react';

interface EmpresaFormData {
  nombre: string;
  razonSocial: string;
  cif: string;
  direccion: string;
  telefono: string;
  email: string;
  web: string;
  pais: string;
}

const initialData: EmpresaFormData = {
  nombre: '',
  razonSocial: '',
  cif: '',
  direccion: '',
  telefono: '',
  email: '',
  web: '',
  pais: ''
};

const EmpresaFormPage = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const [formData, setFormData] = useState<EmpresaFormData>(initialData);
  const [loading, setLoading] = useState(false);
  const [fetching, setFetching] = useState(!!id);

  useEffect(() => {
    if (id) {
      fetchEmpresa(id);
    }
  }, [id]);

  const fetchEmpresa = async (empresaId: string) => {
    try {
      const response = await fetch(`https://localhost:5001/api/empresas/${empresaId}`);
      if (response.ok) {
        const data = await response.json();
        setFormData({
          nombre: data.nombre,
          razonSocial: data.razonSocial,
          cif: data.cif,
          direccion: data.direccion || '',
          telefono: data.telefono || '',
          email: data.email || '',
          web: data.web || '',
          pais: data.pais || ''
        });
      }
    } catch (error) {
      console.error('Error fetching empresa', error);
    } finally {
      setFetching(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      const url = id 
        ? `https://localhost:5001/api/empresas/${id}`
        : 'https://localhost:5001/api/empresas';
      
      const method = id ? 'PUT' : 'POST';

      const response = await fetch(url, {
        method,
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(formData),
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

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  if (fetching) {
    return <div className="p-6 text-center">Cargando...</div>;
  }

  return (
    <div className="p-6 max-w-4xl mx-auto">
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

      <form onSubmit={handleSubmit} className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div className="space-y-2">
            <label className="text-sm font-medium text-gray-700">Nombre *</label>
            <input
              type="text"
              name="nombre"
              required
              value={formData.nombre}
              onChange={handleChange}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 outline-none transition-all"
              placeholder="Nombre comercial"
            />
          </div>

          <div className="space-y-2">
            <label className="text-sm font-medium text-gray-700">Razón Social *</label>
            <input
              type="text"
              name="razonSocial"
              required
              value={formData.razonSocial}
              onChange={handleChange}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 outline-none transition-all"
              placeholder="Razón Social completa"
            />
          </div>

          <div className="space-y-2">
            <label className="text-sm font-medium text-gray-700">CIF / VAT *</label>
            <input
              type="text"
              name="cif"
              required
              value={formData.cif}
              onChange={handleChange}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 outline-none transition-all"
              placeholder="Identificación fiscal"
            />
          </div>

          <div className="space-y-2">
            <label className="text-sm font-medium text-gray-700">País</label>
            <input
              type="text"
              name="pais"
              value={formData.pais}
              onChange={handleChange}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 outline-none transition-all"
              placeholder="País de residencia fiscal"
            />
          </div>

          <div className="space-y-2 md:col-span-2">
            <label className="text-sm font-medium text-gray-700">Dirección</label>
            <input
              type="text"
              name="direccion"
              value={formData.direccion}
              onChange={handleChange}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 outline-none transition-all"
              placeholder="Dirección completa"
            />
          </div>

          <div className="space-y-2">
            <label className="text-sm font-medium text-gray-700">Teléfono</label>
            <input
              type="tel"
              name="telefono"
              value={formData.telefono}
              onChange={handleChange}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 outline-none transition-all"
              placeholder="+34 600 000 000"
            />
          </div>

          <div className="space-y-2">
            <label className="text-sm font-medium text-gray-700">Email</label>
            <input
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 outline-none transition-all"
              placeholder="contacto@empresa.com"
            />
          </div>

          <div className="space-y-2 md:col-span-2">
            <label className="text-sm font-medium text-gray-700">Sitio Web</label>
            <input
              type="url"
              name="web"
              value={formData.web}
              onChange={handleChange}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 outline-none transition-all"
              placeholder="https://www.empresa.com"
            />
          </div>
        </div>

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
  );
};

export default EmpresaFormPage;
