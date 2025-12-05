import React, { useState } from 'react';
import { ChevronRight, ChevronDown, Building2, Building, Users, User } from 'lucide-react';
import { useOrganization } from '../../../context/OrganizationContext';
import { Employee } from '../../../types/employee';
import { API_BASE_URL } from '../../../config';

interface TreeNode {
  id: string;
  name: string;
  type: 'tenant' | 'empresa' | 'group' | 'employee';
  children?: TreeNode[];
  employees?: Employee[];
}

const EmployeesTreePage = () => {
  const { organizations } = useOrganization();
  const [expandedNodes, setExpandedNodes] = useState<Set<string>>(new Set());
  const [selectedNode, setSelectedNode] = useState<string | null>(null);
  const [employeesByGroup, setEmployeesByGroup] = useState<Record<string, Employee[]>>({});
  const [isLoading, setIsLoading] = useState(false);

  const toggleNode = (nodeId: string) => {
    const newExpanded = new Set(expandedNodes);
    if (newExpanded.has(nodeId)) {
      newExpanded.delete(nodeId);
    } else {
      newExpanded.add(nodeId);
    }
    setExpandedNodes(newExpanded);
  };

  const loadEmployeesForGroup = async (empresaId: string, groupName: string) => {
    const cacheKey = `${empresaId}-${groupName}`;
    if (employeesByGroup[cacheKey]) return;

    setIsLoading(true);
    try {
      const response = await fetch(`${API_BASE_URL}/employees?empresaId=${empresaId}`);
      if (response.ok) {
        const employees: Employee[] = await response.json();
        const filtered = employees.filter(e => e.groupName === groupName);
        setEmployeesByGroup(prev => ({ ...prev, [cacheKey]: filtered }));
      }
    } catch (error) {
      console.error('Error loading employees:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleGroupClick = async (nodeId: string, empresaId: string, groupName: string) => {
    setSelectedNode(nodeId);
    toggleNode(nodeId);
    await loadEmployeesForGroup(empresaId, groupName);
  };

  const buildTree = (): TreeNode[] => {
    const tenants = organizations.filter(o => o.type === 'tenant');
    const empresas = organizations.filter(o => o.type === 'empresa');

    return tenants.map(tenant => ({
      id: tenant.id,
      name: tenant.name,
      type: 'tenant' as const,
      children: empresas
        .map(empresa => ({
          id: empresa.id,
          name: empresa.name,
          type: 'empresa' as const,
          children: [
            { id: `${empresa.id}-General`, name: 'General', type: 'group' as const },
            { id: `${empresa.id}-IT`, name: 'IT', type: 'group' as const },
            { id: `${empresa.id}-RRHH`, name: 'RRHH', type: 'group' as const },
            { id: `${empresa.id}-Ventas`, name: 'Ventas', type: 'group' as const },
            { id: `${empresa.id}-Operaciones`, name: 'Operaciones', type: 'group' as const },
          ]
        }))
    }));
  };

  const renderIcon = (type: string) => {
    switch (type) {
      case 'tenant':
        return <Building2 size={16} className="text-indigo-600" />;
      case 'empresa':
        return <Building size={16} className="text-blue-600" />;
      case 'group':
        return <Users size={16} className="text-green-600" />;
      case 'employee':
        return <User size={16} className="text-gray-600" />;
      default:
        return null;
    }
  };

  const renderNode = (node: TreeNode, level: number = 0): React.ReactNode => {
    const hasChildren = node.children && node.children.length > 0;
    const isExpanded = expandedNodes.has(node.id);
    const isSelected = selectedNode === node.id;
    const paddingLeft = level * 20;

    const cacheKey = node.type === 'group' ? `${node.id.split('-')[0]}-${node.name}` : '';
    const employees = cacheKey ? employeesByGroup[cacheKey] || [] : [];

    return (
      <div key={node.id}>
        <div
          className={`flex items-center gap-2 py-2 px-3 hover:bg-gray-50 cursor-pointer rounded-lg transition-colors ${
            isSelected ? 'bg-indigo-50' : ''
          }`}
          style={{ paddingLeft: `${paddingLeft + 12}px` }}
          onClick={() => {
            if (node.type === 'group') {
              const empresaId = node.id.split('-')[0];
              handleGroupClick(node.id, empresaId, node.name);
            } else {
              toggleNode(node.id);
              setSelectedNode(node.id);
            }
          }}
        >
          {hasChildren && (
            <button className="p-0.5">
              {isExpanded ? <ChevronDown size={16} /> : <ChevronRight size={16} />}
            </button>
          )}
          {!hasChildren && <span className="w-5" />}
          {renderIcon(node.type)}
          <span className="text-sm font-medium text-gray-900">{node.name}</span>
          {node.type === 'group' && isExpanded && employees.length > 0 && (
            <span className="ml-auto text-xs text-gray-500">({employees.length})</span>
          )}
        </div>

        {isExpanded && hasChildren && (
          <div>
            {node.children!.map(child => renderNode(child, level + 1))}
          </div>
        )}

        {isExpanded && node.type === 'group' && employees.length > 0 && (
          <div className="ml-8 mt-1 space-y-1">
            {employees.map(employee => (
              <div
                key={employee.id}
                className="flex items-center gap-2 py-1.5 px-3 text-sm text-gray-700 hover:bg-gray-50 rounded-lg"
                style={{ paddingLeft: `${paddingLeft + 32}px` }}
              >
                <User size={14} className="text-gray-400" />
                <span>{employee.nombre} {employee.apellidos}</span>
                <span className="text-xs text-gray-500">({employee.alias})</span>
              </div>
            ))}
          </div>
        )}
      </div>
    );
  };

  const tree = buildTree();

  return (
    <div className="p-6">
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-900">Árbol de Empleados</h1>
        <p className="text-gray-500">Visualización jerárquica de la organización</p>
      </div>

      <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-4">
        {isLoading && (
          <div className="text-center py-4 text-gray-500">Cargando empleados...</div>
        )}
        <div className="space-y-1">
          {tree.map(node => renderNode(node))}
        </div>
        {tree.length === 0 && (
          <div className="text-center py-8 text-gray-500">
            No hay organizaciones disponibles
          </div>
        )}
      </div>
    </div>
  );
};

export default EmployeesTreePage;
