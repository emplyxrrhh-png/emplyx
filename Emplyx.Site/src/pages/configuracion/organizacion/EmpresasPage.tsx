import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Plus, Edit, Trash2, Building2 } from 'lucide-react';
import { API_BASE_URL } from '../../../config';

interface Empresa {
  id: string;
  nombre: string;
  razonSocial: string;
  cif: string;
  pais: string;
  isActive: boolean;
}

const EmpresasPage = () => {
  const navigate = useNavigate();
  const [empresas, setEmpresas] = useState<Empresa[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchEmpresas();
  }, []);

  const fetchEmpresas = async () => {
    try {
      const response = await fetch(`${API_BASE_URL}/empresas`);
      if (response.ok) {
        const data = await response.json();
        setEmpresas(data);
      } else {
        console.error('Error fetching empresas');
      }
    } catch (error) {
      console.error('Error fetching empresas', error);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (window.confirm('¿Estás seguro de que quieres eliminar esta empresa?')) {
      try {
        const response = await fetch(`${API_BASE_URL}/empresas/${id}`, {
          method: 'DELETE',
        });
        if (response.ok) {
          fetchEmpresas();
        }
      } catch (error) {
        console.error('Error deleting empresa', error);
      }
    }
  };

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 flex items-center gap-2">
            <Building2 className="h-8 w-8 text-indigo-600" />
            Empresas
          </h1>
          <p className="text-gray-500 mt-1">Gestiona las empresas de tu organización</p>
        </div>
        <button
          onClick={() => navigate('/configuracion/organizacion/empresas/nueva')}
          className="bg-indigo-600 text-white px-4 py-2 rounded-lg flex items-center gap-2 hover:bg-indigo-700 transition-colors"
        >
          <Plus size={20} />
          Nueva Empresa
        </button>
      </div>

      {loading ? (
        <div className="text-center py-10">Cargando...</div>
      ) : (
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
          <table className="w-full text-left">
            <thead className="bg-gray-50 border-b border-gray-100">
              <tr>
                <th className="px-6 py-4 font-semibold text-gray-700">Nombre</th>
                <th className="px-6 py-4 font-semibold text-gray-700">Razón Social</th>
                <th className="px-6 py-4 font-semibold text-gray-700">CIF / VAT</th>
                <th className="px-6 py-4 font-semibold text-gray-700">País</th>
                <th className="px-6 py-4 font-semibold text-gray-700">Estado</th>
                <th className="px-6 py-4 font-semibold text-gray-700 text-right">Acciones</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {empresas.map((empresa) => (
                <tr key={empresa.id} className="hover:bg-gray-50 transition-colors">
                  <td className="px-6 py-4 text-gray-900 font-medium">{empresa.nombre}</td>
                  <td className="px-6 py-4 text-gray-600">{empresa.razonSocial}</td>
                  <td className="px-6 py-4 text-gray-600 font-mono text-sm">{empresa.cif}</td>
                  <td className="px-6 py-4 text-gray-600">{empresa.pais}</td>
                  <td className="px-6 py-4">
                    <span className={`px-2 py-1 rounded-full text-xs font-medium ${
                      empresa.isActive 
                        ? 'bg-green-100 text-green-700' 
                        : 'bg-gray-100 text-gray-700'
                    }`}>
                      {empresa.isActive ? 'Activa' : 'Inactiva'}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-right">
                    <div className="flex justify-end gap-2">
                      <button
                        onClick={() => navigate(`/configuracion/organizacion/empresas/editar/${empresa.id}`)}
                        className="p-2 text-gray-400 hover:text-indigo-600 hover:bg-indigo-50 rounded-lg transition-colors"
                        title="Editar"
                      >
                        <Edit size={18} />
                      </button>
                      <button
                        onClick={() => handleDelete(empresa.id)}
                        className="p-2 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                        title="Eliminar"
                      >
                        <Trash2 size={18} />
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
              {empresas.length === 0 && (
                <tr>
                  <td colSpan={6} className="px-6 py-10 text-center text-gray-500">
                    No hay empresas registradas. ¡Crea la primera!
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

export default EmpresasPage;
