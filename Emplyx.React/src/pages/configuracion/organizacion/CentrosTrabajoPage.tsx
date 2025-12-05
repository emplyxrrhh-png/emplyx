import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Plus, Edit, Trash2, MapPin } from 'lucide-react';
import { useOrganization } from '../../../context/OrganizationContext';
import { API_BASE_URL } from '../../../config';

interface CentroTrabajo {
  id: string;
  nombre: string;
  empresaNombre: string;
  ciudad: string;
  pais: string;
  isActive: boolean;
}

const CentrosTrabajoPage = () => {
  const navigate = useNavigate();
  const { selectedCompany } = useOrganization();
  const [centros, setCentros] = useState<CentroTrabajo[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchCentros();
  }, [selectedCompany]);

  const fetchCentros = async () => {
    if (!selectedCompany) return;
    
    try {
      const queryParam = selectedCompany.type === 'tenant' 
        ? `tenantId=${selectedCompany.id}` 
        : `empresaId=${selectedCompany.id}`;

      const response = await fetch(`${API_BASE_URL}/centros-trabajo?${queryParam}`);
      if (response.ok) {
        const data = await response.json();
        setCentros(data);
      } else {
        console.error('Error fetching centros:', response.statusText);
        setCentros([]);
      }
    } catch (error) {
      console.error('Error fetching centros', error);
      setCentros([]);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (window.confirm('¿Estás seguro de que quieres eliminar este centro de trabajo?')) {
      try {
        const response = await fetch(`${API_BASE_URL}/centros-trabajo/${id}`, {
          method: 'DELETE',
        });
        if (response.ok) {
          fetchCentros();
        } else {
          alert('Error al eliminar el centro de trabajo');
        }
      } catch (error) {
        console.error('Error deleting centro', error);
        alert('Error al eliminar el centro de trabajo');
      }
    }
  };

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 flex items-center gap-2">
            <MapPin className="h-8 w-8 text-indigo-600" />
            Centros de Trabajo
          </h1>
          <p className="text-gray-500 mt-1">
            Gestiona los centros de trabajo de <span className="font-semibold text-indigo-600">{selectedCompany?.name || 'tu organización'}</span>
          </p>
        </div>
        <button
          onClick={() => navigate('/configuracion/organizacion/centros-trabajo/nuevo')}
          className="bg-indigo-600 text-white px-4 py-2 rounded-lg flex items-center gap-2 hover:bg-indigo-700 transition-colors"
        >
          <Plus size={20} />
          Nuevo Centro
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
                <th className="px-6 py-4 font-semibold text-gray-700">Empresa</th>
                <th className="px-6 py-4 font-semibold text-gray-700">Ciudad</th>
                <th className="px-6 py-4 font-semibold text-gray-700">País</th>
                <th className="px-6 py-4 font-semibold text-gray-700">Estado</th>
                <th className="px-6 py-4 font-semibold text-gray-700 text-right">Acciones</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {centros.map((centro) => (
                <tr key={centro.id} className="hover:bg-gray-50 transition-colors">
                  <td className="px-6 py-4 text-gray-900 font-medium">{centro.nombre}</td>
                  <td className="px-6 py-4 text-gray-600">{centro.empresaNombre}</td>
                  <td className="px-6 py-4 text-gray-600">{centro.ciudad}</td>
                  <td className="px-6 py-4 text-gray-600">{centro.pais}</td>
                  <td className="px-6 py-4">
                    <span className={`px-2 py-1 rounded-full text-xs font-medium ${
                      centro.isActive 
                        ? 'bg-green-100 text-green-700' 
                        : 'bg-gray-100 text-gray-700'
                    }`}>
                      {centro.isActive ? 'Activo' : 'Inactivo'}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-right">
                    <div className="flex justify-end gap-2">
                      <button
                        onClick={() => navigate(`/configuracion/organizacion/centros-trabajo/editar/${centro.id}`)}
                        className="p-2 text-gray-400 hover:text-indigo-600 hover:bg-indigo-50 rounded-lg transition-colors"
                        title="Editar"
                      >
                        <Edit size={18} />
                      </button>
                      <button
                        onClick={() => handleDelete(centro.id)}
                        className="p-2 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                        title="Eliminar"
                      >
                        <Trash2 size={18} />
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
              {centros.length === 0 && (
                <tr>
                  <td colSpan={6} className="px-6 py-10 text-center text-gray-500">
                    No hay centros de trabajo registrados.
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

export default CentrosTrabajoPage;
