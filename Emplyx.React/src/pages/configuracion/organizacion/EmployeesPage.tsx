import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Plus, Search, Edit, Trash2, User, ChevronRight, ChevronDown, Users, Building } from 'lucide-react';
import { useOrganization } from '../../../context/OrganizationContext';
import { Employee } from '../../../types/employee';

const EmployeesPage = () => {
  const navigate = useNavigate();
  const { selectedCompany } = useOrganization();
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedGroup, setSelectedGroup] = useState<string | null>(null);
  const [isGroupsExpanded, setIsGroupsExpanded] = useState(true);

  useEffect(() => {
    if (selectedCompany) {
      fetchEmployees();
    }
  }, [selectedCompany]);

  const fetchEmployees = async () => {
    if (!selectedCompany) return;
    
    setIsLoading(true);
    try {
      let url = `https://localhost:5001/api/employees?`;
      if (selectedCompany.type === 'empresa') {
          url += `empresaId=${selectedCompany.id}`;
      } else {
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

  // Extract unique groups from employees
  const uniqueGroups = Array.from(new Set(employees.map(e => e.groupName).filter(Boolean))).sort();

  const filteredEmployees = employees.filter(employee => {
    const matchesSearch = 
      employee.nombre.toLowerCase().includes(searchTerm.toLowerCase()) ||
      (employee.apellidos && employee.apellidos.toLowerCase().includes(searchTerm.toLowerCase())) ||
      employee.alias.toLowerCase().includes(searchTerm.toLowerCase());
    
    const matchesGroup = selectedGroup ? employee.groupName === selectedGroup : true;

    return matchesSearch && matchesGroup;
  });

  if (!selectedCompany || selectedCompany.type !== 'empresa') {
      return (
          <div className="p-6 text-center text-gray-500">
              Por favor, selecciona una Empresa para ver sus empleados.
          </div>
      );
  }

  return (
    <div className="p-6 h-[calc(100vh-4rem)] flex flex-col">
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

      <div className="flex gap-6 flex-1 overflow-hidden">
        {/* Left Sidebar - Groups Tree */}
        <div className="w-64 bg-white rounded-xl shadow-sm border border-gray-100 flex flex-col overflow-hidden">
          <div className="p-4 border-b border-gray-100 font-medium text-gray-700 flex items-center gap-2">
            <Building size={18} className="text-indigo-600" />
            Organización
          </div>
          <div className="flex-1 overflow-y-auto p-2">
            <div className="space-y-1">
              {/* All Employees Node */}
              <button
                onClick={() => setSelectedGroup(null)}
                className={`w-full flex items-center gap-2 px-3 py-2 rounded-lg text-sm transition-colors ${
                  selectedGroup === null 
                    ? 'bg-indigo-50 text-indigo-700 font-medium' 
                    : 'text-gray-600 hover:bg-gray-50'
                }`}
              >
                <Users size={16} />
                Todos los empleados
                <span className="ml-auto text-xs bg-gray-100 px-2 py-0.5 rounded-full text-gray-500">
                  {employees.length}
                </span>
              </button>

              {/* Groups Folder */}
              <div>
                <button
                  onClick={() => setIsGroupsExpanded(!isGroupsExpanded)}
                  className="w-full flex items-center gap-2 px-3 py-2 rounded-lg text-sm text-gray-600 hover:bg-gray-50 transition-colors"
                >
                  {isGroupsExpanded ? <ChevronDown size={16} /> : <ChevronRight size={16} />}
                  <span className="font-medium">Grupos</span>
                </button>
                
                {isGroupsExpanded && (
                  <div className="ml-4 mt-1 space-y-1 border-l-2 border-gray-100 pl-2">
                    {uniqueGroups.map(group => {
                      const count = employees.filter(e => e.groupName === group).length;
                      return (
                        <button
                          key={group}
                          onClick={() => setSelectedGroup(group)}
                          className={`w-full flex items-center gap-2 px-3 py-2 rounded-lg text-sm transition-colors ${
                            selectedGroup === group 
                              ? 'bg-indigo-50 text-indigo-700 font-medium' 
                              : 'text-gray-600 hover:bg-gray-50'
                          }`}
                        >
                          <span className="truncate">{group}</span>
                          <span className="ml-auto text-xs bg-gray-100 px-2 py-0.5 rounded-full text-gray-500">
                            {count}
                          </span>
                        </button>
                      );
                    })}
                    {uniqueGroups.length === 0 && (
                      <div className="px-3 py-2 text-xs text-gray-400 italic">
                        No hay grupos definidos
                      </div>
                    )}
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>

        {/* Right Content - Employees List */}
        <div className="flex-1 bg-white rounded-xl shadow-sm border border-gray-100 flex flex-col overflow-hidden">
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
            <div className="flex-1 overflow-auto">
              <table className="w-full text-left text-sm">
                <thead className="bg-gray-50 text-gray-600 font-medium text-xs sticky top-0 z-10">
                  <tr>
                    {!selectedGroup && <th className="px-4 py-2">Grupo</th>}
                    <th className="px-4 py-2">Nombre</th>
                    <th className="px-4 py-2">Apellidos</th>
                    <th className="px-4 py-2">Alias</th>
                    <th className="px-4 py-2 text-right">Acciones</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-100">
                  {filteredEmployees.map((employee) => (
                    <tr key={employee.id} className="hover:bg-gray-50">
                      {!selectedGroup && <td className="px-4 py-2 text-gray-600">{employee.groupName}</td>}
                      <td className="px-4 py-2">
                        <div className="font-medium text-gray-900">{employee.nombre}</div>
                      </td>
                      <td className="px-4 py-2 text-gray-600">{employee.apellidos}</td>
                      <td className="px-4 py-2 text-gray-500">{employee.alias}</td>
                      <td className="px-4 py-2 text-right">
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
                      <td colSpan={selectedGroup ? 4 : 5} className="px-4 py-6 text-center text-gray-500">
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
    </div>
  );
};

export default EmployeesPage;
