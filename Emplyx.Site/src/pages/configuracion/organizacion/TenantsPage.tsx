import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Building2, Plus, Edit, Trash2 } from 'lucide-react';

interface Tenant {
  id: string;
  legalName: string;
  taxId: string;
  companyType: string;
  mainContact: {
    name: string;
    email: string;
  };
}

const TenantsPage = () => {
  const navigate = useNavigate();
  const [tenants, setTenants] = useState<Tenant[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchTenants();
  }, []);

  const fetchTenants = async () => {
    try {
      const response = await fetch('https://localhost:5001/api/tenants');
      if (response.ok) {
        const data = await response.json();
        setTenants(data);
      }
    } catch (error) {
      console.error('Error fetching tenants', error);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (window.confirm('¿Estás seguro de que quieres eliminar este tenant?')) {
      try {
        await fetch(`https://localhost:5001/api/tenants/${id}`, { method: 'DELETE' });
        fetchTenants();
      } catch (error) {
        console.error('Error deleting tenant', error);
      }
    }
  };

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 flex items-center gap-2">
            <Building2 className="h-8 w-8 text-indigo-600" />
            Tenants
          </h1>
          <p className="text-gray-500 mt-1">Gestiona la entidad principal de la organización</p>
        </div>
        {tenants.length === 0 && (
          <button
            onClick={() => navigate('/tenants/nuevo')}
            className="bg-indigo-600 text-white px-4 py-2 rounded-lg flex items-center gap-2 hover:bg-indigo-700 transition-colors"
          >
            <Plus size={20} />
            Nuevo Tenant
          </button>
        )}
      </div>

      {loading ? (
        <div className="text-center py-10">Cargando...</div>
      ) : (
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
          <table className="w-full text-left">
            <thead className="bg-gray-50 border-b border-gray-100">
              <tr>
                <th className="px-6 py-4 font-semibold text-gray-700">Razón Social</th>
                <th className="px-6 py-4 font-semibold text-gray-700">NIF/CIF</th>
                <th className="px-6 py-4 font-semibold text-gray-700">Tipo</th>
                <th className="px-6 py-4 font-semibold text-gray-700">Contacto</th>
                <th className="px-6 py-4 font-semibold text-gray-700 text-right">Acciones</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {tenants.map((tenant) => (
                <tr key={tenant.id} className="hover:bg-gray-50 transition-colors">
                  <td className="px-6 py-4 text-gray-900 font-medium">{tenant.legalName}</td>
                  <td className="px-6 py-4 text-gray-600 font-mono text-sm">{tenant.taxId}</td>
                  <td className="px-6 py-4 text-gray-600">{tenant.companyType}</td>
                  <td className="px-6 py-4 text-gray-600">
                    <div className="text-sm font-medium">{tenant.mainContact.name}</div>
                    <div className="text-xs text-gray-500">{tenant.mainContact.email}</div>
                  </td>
                  <td className="px-6 py-4 text-right">
                    <div className="flex justify-end gap-2">
                      <button
                        onClick={() => navigate(`/tenants/editar/${tenant.id}`)}
                        className="p-2 text-gray-400 hover:text-indigo-600 hover:bg-indigo-50 rounded-lg transition-colors"
                        title="Editar"
                      >
                        <Edit size={18} />
                      </button>
                      <button
                        onClick={() => handleDelete(tenant.id)}
                        className="p-2 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                        title="Eliminar"
                      >
                        <Trash2 size={18} />
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
              {tenants.length === 0 && (
                <tr>
                  <td colSpan={5} className="px-6 py-10 text-center text-gray-500">
                    No hay datos registrados.
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

export default TenantsPage;
