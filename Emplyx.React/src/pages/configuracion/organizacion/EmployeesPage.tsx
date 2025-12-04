import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Plus, Search, Edit, Trash2, User } from 'lucide-react';
import { useOrganization } from '../../../context/OrganizationContext';
import { Employee } from '../../../types/employee';

const EmployeesPage = () => {
  const navigate = useNavigate();
  const { selectedCompany } = useOrganization();
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');

  useEffect(() => {
    if (selectedCompany) {
      fetchEmployees();
    }
  }, [selectedCompany]);

  const fetchEmployees = async () => {
    if (!selectedCompany) return;
    
    setIsLoading(true);
    try {
      // Determine query param based on selected context type
      // Assuming selectedCompany.type 'empresa' maps to empresaId
      // If we had work center selection in context, we would use that.
      // For now, let's assume we filter by Empresa if type is empresa.
      
      let url = `https://localhost:5001/api/employees?`;
      if (selectedCompany.type === 'empresa') {
          url += `empresaId=${selectedCompany.id}`;
      } else {
          // If tenant, we might not be able to fetch all employees directly 
          // unless we implement a tenant-level endpoint or iterate.
          // For now, let's clear list if not an empresa.
          setEmployees([]);
          setIsLoading(false);
          return;
      }

      const response = await fetch(url);
      if (response.ok) {
        const data = await response.json();
        setEmployees(data);
      }
    } catch (error) {
      console.error('Error fetching employees:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm('¿Estás seguro de que deseas eliminar este empleado?')) return;

    try {
      const response = await fetch(`https://localhost:5001/api/employees/${id}`, {
        method: 'DELETE',
      });

      if (response.ok) {
        setEmployees(employees.filter(e => e.id !== id));
      }
    } catch (error) {
      console.error('Error deleting employee:', error);
    }
  };

  const filteredEmployees = employees.filter(employee => 
    employee.nombre.toLowerCase().includes(searchTerm.toLowerCase()) ||
    (employee.apellidos && employee.apellidos.toLowerCase().includes(searchTerm.toLowerCase())) ||
    employee.alias.toLowerCase().includes(searchTerm.toLowerCase())
  );

  if (!selectedCompany || selectedCompany.type !== 'empresa') {
      return (
          <div className="p-6 text-center text-gray-500">
              Por favor, selecciona una Empresa para ver sus empleados.
          </div>
      );
  }

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Empleados</h1>
          <p className="text-gray-500">Gestión de empleados para {selectedCompany.name}</p>
        </div>
        <button
          onClick={() => navigate('/empleados/nuevo')}
          className="flex items-center gap-2 px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors"
        >
          <Plus size={20} />
          Nuevo Empleado
        </button>
      </div>

      <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
        <div className="p-4 border-b border-gray-100">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" size={20} />
            <input
              type="text"
              placeholder="Buscar empleados..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500"
            />
          </div>
        </div>

        {isLoading ? (
          <div className="p-8 text-center text-gray-500">Cargando empleados...</div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-sm">
              <thead className="bg-gray-50 text-gray-600 font-medium text-xs">
                <tr>
                  <th className="px-4 py-1.5">Grupo</th>
                  <th className="px-4 py-1.5">Nombre</th>
                  <th className="px-4 py-1.5">Apellidos</th>
                  <th className="px-4 py-1.5">Alias</th>
                  <th className="px-4 py-1.5 text-right">Acciones</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100">
                {filteredEmployees.map((employee) => (
                  <tr key={employee.id} className="hover:bg-gray-50">
                    <td className="px-4 py-1.5 text-gray-600">{employee.groupName}</td>
                    <td className="px-4 py-1.5">
                      <div className="font-medium text-gray-900">{employee.nombre}</div>
                    </td>
                    <td className="px-4 py-1.5 text-gray-600">{employee.apellidos}</td>
                    <td className="px-4 py-1.5 text-gray-500">{employee.alias}</td>
                    <td className="px-4 py-1.5 text-right">
                      <div className="flex justify-end gap-2">
                        <button
                          onClick={() => navigate(`/empleados/editar/${employee.id}`)}
                          className="p-1.5 text-gray-400 hover:text-indigo-600 hover:bg-indigo-50 rounded-lg transition-colors"
                        >
                          <Edit size={16} />
                        </button>
                        <button
                          onClick={() => handleDelete(employee.id)}
                          className="p-1.5 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                        >
                          <Trash2 size={16} />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
                {filteredEmployees.length === 0 && (
                  <tr>
                    <td colSpan={5} className="px-4 py-6 text-center text-gray-500">
                      No se encontraron empleados
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
};

export default EmployeesPage;
