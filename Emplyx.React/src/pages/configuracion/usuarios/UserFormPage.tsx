import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { ArrowLeft, Save, User as UserIcon, Mail, Shield, CheckCircle, XCircle } from 'lucide-react';
import { User } from '../../../types/user';
import { Role } from '../../../types/role';

const UserFormPage = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const isEditMode = !!id;
  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);

  const [formData, setFormData] = useState<User>({
    id: '',
    username: '',
    email: '',
    fullName: '',
    isActive: true,
    roles: [],
  });

  // Mock Roles
  const [availableRoles] = useState<Role[]>([
    { id: '1', name: 'Owner', description: 'Propietario de la suscripción', isSystem: true, permissions: [], isCommon: true },
    { id: '2', name: 'Collaborator', description: 'Colaborador de la suscripción', isSystem: false, permissions: [], isCommon: true },
  ]);

  useEffect(() => {
    if (isEditMode) {
      setIsLoading(true);
      // Mock fetch user
      setTimeout(() => {
        setFormData({
          id: id,
          username: 'jdoe',
          email: 'john.doe@emplyx.com',
          fullName: 'John Doe',
          isActive: true,
          roles: ['RRHH', 'Supervisor'],
        });
        setIsLoading(false);
      }, 500);
    }
  }, [isEditMode, id]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSaving(true);
    // Mock save
    setTimeout(() => {
      setIsSaving(false);
      navigate('/configuracion/usuarios');
    }, 1000);
  };

  const toggleRole = (roleName: string) => {
    setFormData(prev => {
      const newRoles = prev.roles.includes(roleName)
        ? prev.roles.filter(r => r !== roleName)
        : [...prev.roles, roleName];
      return { ...prev, roles: newRoles };
    });
  };

  if (isLoading) {
    return <div className="p-6 text-center text-gray-500">Cargando suscripción...</div>;
  }

  return (
    <div className="p-6 max-w-4xl mx-auto">
      <div className="flex items-center gap-4 mb-8">
        <button
          onClick={() => navigate('/configuracion/usuarios')}
          className="p-2 hover:bg-gray-100 rounded-lg transition-colors text-gray-500"
        >
          <ArrowLeft size={24} />
        </button>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">
            {isEditMode ? 'Editar usuario de la suscripción' : 'Nuevo usuario de la suscripción'}
          </h1>
          <p className="text-gray-500">
            {isEditMode ? 'Modifica los datos del usuario de la suscripción' : 'Crea un nuevo usuario de la suscripción en el sistema'}
          </p>
        </div>
      </div>

      <form onSubmit={handleSubmit} className="space-y-6">
        {/* Basic Info Card */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
          <div className="p-6 border-b border-gray-100 bg-gray-50/50">
            <h2 className="text-lg font-semibold text-gray-900 flex items-center gap-2">
              <UserIcon size={20} className="text-indigo-600" />
              Información Personal
            </h2>
          </div>
          <div className="p-6 grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="space-y-2">
              <label className="text-sm font-medium text-gray-700">Nombre Completo</label>
              <input
                type="text"
                required
                value={formData.fullName}
                onChange={e => setFormData({ ...formData, fullName: e.target.value })}
                className="w-full px-4 py-2 border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500"
                placeholder="Ej: Juan Pérez"
              />
            </div>

            <div className="space-y-2">
              <label className="text-sm font-medium text-gray-700">Nombre de Usuario</label>
              <input
                type="text"
                required
                value={formData.username}
                onChange={e => setFormData({ ...formData, username: e.target.value })}
                className="w-full px-4 py-2 border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500"
                placeholder="Ej: jperez"
              />
            </div>

            <div className="space-y-2">
              <label className="text-sm font-medium text-gray-700">Correo Electrónico</label>
              <div className="relative">
                <Mail className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" size={18} />
                <input
                  type="email"
                  required
                  value={formData.email}
                  onChange={e => setFormData({ ...formData, email: e.target.value })}
                  className="w-full pl-10 pr-4 py-2 border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  placeholder="ejemplo@emplyx.com"
                />
              </div>
            </div>

            <div className="space-y-2">
              <label className="text-sm font-medium text-gray-700">Estado</label>
              <div className="flex items-center gap-4 mt-2">
                <button
                  type="button"
                  onClick={() => setFormData({ ...formData, isActive: true })}
                  className={`flex items-center gap-2 px-4 py-2 rounded-lg border transition-colors ${
                    formData.isActive
                      ? 'bg-green-50 border-green-200 text-green-700'
                      : 'bg-white border-gray-200 text-gray-500 hover:bg-gray-50'
                  }`}
                >
                  <CheckCircle size={18} />
                  Activo
                </button>
                <button
                  type="button"
                  onClick={() => setFormData({ ...formData, isActive: false })}
                  className={`flex items-center gap-2 px-4 py-2 rounded-lg border transition-colors ${
                    !formData.isActive
                      ? 'bg-red-50 border-red-200 text-red-700'
                      : 'bg-white border-gray-200 text-gray-500 hover:bg-gray-50'
                  }`}
                >
                  <XCircle size={18} />
                  Inactivo
                </button>
              </div>
            </div>
          </div>
        </div>

        {/* Roles Card */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
          <div className="p-6 border-b border-gray-100 bg-gray-50/50">
            <h2 className="text-lg font-semibold text-gray-900 flex items-center gap-2">
              <Shield size={20} className="text-indigo-600" />
              Asignación de Roles
            </h2>
            <p className="text-sm text-gray-500 mt-1">
              Selecciona el rol que corresponda al usuario en esta suscripción
            </p>
          </div>
          
          <div className="p-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              {availableRoles.map(role => (
                <div
                  key={role.id}
                  onClick={() => toggleRole(role.name)}
                  className={`p-4 rounded-lg border cursor-pointer transition-all flex items-start gap-3 ${
                    formData.roles.includes(role.name)
                      ? 'bg-indigo-50 border-indigo-200 ring-1 ring-indigo-500'
                      : 'bg-white border-gray-200 hover:border-indigo-300 hover:bg-gray-50'
                  }`}
                >
                  <div className={`mt-0.5 p-1.5 rounded-md flex-shrink-0 ${
                    formData.roles.includes(role.name) ? 'bg-indigo-100 text-indigo-600' : 'bg-gray-100 text-gray-400'
                  }`}>
                    {formData.roles.includes(role.name) ? <CheckCircle size={16} /> : <Shield size={16} />}
                  </div>
                  <div className="min-w-0">
                    <h3 className={`text-sm font-medium truncate ${
                      formData.roles.includes(role.name) ? 'text-indigo-900' : 'text-gray-900'
                    }`}>
                      {role.name}
                    </h3>
                    <p className="text-xs text-gray-500 mt-0.5">{role.description}</p>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>

        <div className="flex justify-end gap-4 pt-4">
          <button
            type="button"
            onClick={() => navigate('/configuracion/usuarios')}
            className="px-6 py-2 text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors"
          >
            Cancelar
          </button>
          <button
            type="submit"
            disabled={isSaving}
            className="flex items-center gap-2 px-6 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors disabled:opacity-50"
          >
            <Save size={20} />
            {isSaving ? 'Guardando...' : 'Guardar Usuario'}
          </button>
        </div>
      </form>
    </div>
  );
};

export default UserFormPage;
