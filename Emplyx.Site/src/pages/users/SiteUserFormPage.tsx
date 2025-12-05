import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { ArrowLeft, Save, User as UserIcon, Mail, Shield, CheckCircle, XCircle, Lock, Eye, EyeOff } from 'lucide-react';
import { User } from '../../types/user';
import { API_BASE_URL } from '../../config';

interface Role {
  id: string;
  name: string;
  description: string;
}

const SiteUserFormPage = () => {
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

  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);

  // Fixed Roles for Site
  const [availableRoles] = useState<Role[]>([
    { id: '3fbdede0-0773-4cb6-9881-9350d0f4955e', name: 'Administrador Global', description: 'Acceso total a la plataforma.' },
    { id: 'a9d6d2dc-136f-42e4-9b77-75568128ed5f', name: 'Operador', description: 'Operativa diaria de la plataforma.' },
  ]);

  useEffect(() => {
    if (isEditMode) {
      setIsLoading(true);
      fetch(`${API_BASE_URL}/users/${id}`)
        .then(res => {
          if (!res.ok) throw new Error('Failed to fetch user');
          return res.json();
        })
        .then(data => {
          setFormData({
            id: data.id,
            username: data.userName,
            email: data.email,
            fullName: data.displayName,
            isActive: data.isActive,
            roles: data.roles.map((r: any) => r.rolId),
          });
        })
        .catch(err => {
          console.error(err);
          alert('Error al cargar el usuario');
          navigate('/usuarios');
        })
        .finally(() => setIsLoading(false));
    }
  }, [isEditMode, id, navigate]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSaving(true);
    
    try {
      const url = isEditMode 
        ? `${API_BASE_URL}/users/${id}`
        : `${API_BASE_URL}/users`;
      
      const method = isEditMode ? 'PUT' : 'POST';
      
      const body = isEditMode ? {
        usuarioId: id,
        displayName: formData.fullName,
        email: formData.email,
        isActive: formData.isActive,
        roles: formData.roles.map(r => ({ rolId: r })),
        perfil: {},
        contextos: [],
        licencias: [],
        passwordHash: password || undefined
      } : {
        userName: formData.username,
        email: formData.email,
        displayName: formData.fullName,
        roles: formData.roles.map(r => ({ rolId: r })),
        perfil: {},
        contextos: [],
        licencias: [],
        passwordHash: password
      };

      const response = await fetch(url, {
        method,
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(body),
      });

      if (!response.ok) {
        const errorText = await response.text();
        console.error('Server response:', errorText);
        throw new Error(`Failed to save user: ${errorText}`);
      }

      navigate('/usuarios');
    } catch (error) {
      console.error('Error saving user:', error);
      alert('Error al guardar el usuario');
    } finally {
      setIsSaving(false);
    }
  };

  const toggleRole = (roleId: string) => {
    setFormData(prev => {
      const newRoles = prev.roles.includes(roleId)
        ? prev.roles.filter(r => r !== roleId)
        : [...prev.roles, roleId];
      return { ...prev, roles: newRoles };
    });
  };

  if (isLoading) {
    return <div className="p-6 text-center text-gray-500">Cargando usuario...</div>;
  }

  return (
    <div className="p-6 max-w-4xl mx-auto">
      <div className="flex items-center gap-4 mb-8">
        <button
          onClick={() => navigate('/usuarios')}
          className="p-2 hover:bg-gray-100 rounded-lg transition-colors text-gray-500"
        >
          <ArrowLeft size={24} />
        </button>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">
            {isEditMode ? 'Editar Usuario del Site' : 'Nuevo Usuario del Site'}
          </h1>
          <p className="text-gray-500">
            {isEditMode ? 'Modifica los datos del usuario' : 'Crea un nuevo usuario con acceso al site'}
          </p>
        </div>
      </div>

      <form onSubmit={handleSubmit} className="space-y-6" autoComplete="off">
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
                placeholder="Ej: Admin Site"
                autoComplete="off"
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
                placeholder="Ej: admin"
                autoComplete="off"
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
                  placeholder="admin@emplyx.site"
                  autoComplete="off"
                />
              </div>
            </div>

            <div className="space-y-2">
              <label className="text-sm font-medium text-gray-700">
                {isEditMode ? 'Nueva Contraseña (Opcional)' : 'Contraseña'}
              </label>
              <div className="relative">
                <Lock className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" size={18} />
                <input
                  type={showPassword ? 'text' : 'password'}
                  required={!isEditMode}
                  value={password}
                  onChange={e => setPassword(e.target.value)}
                  className="w-full pl-10 pr-12 py-2 border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  placeholder={isEditMode ? 'Dejar en blanco para mantener actual' : 'Contraseña segura'}
                  autoComplete="new-password"
                />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
                >
                  {showPassword ? <EyeOff size={18} /> : <Eye size={18} />}
                </button>
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
              Selecciona el rol que corresponda al usuario en el Site
            </p>
          </div>
          
          <div className="p-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              {availableRoles.map(role => (
                <div
                  key={role.id}
                  onClick={() => toggleRole(role.id)}
                  className={`p-4 rounded-lg border cursor-pointer transition-all flex items-start gap-3 ${
                    formData.roles.includes(role.id)
                      ? 'bg-indigo-50 border-indigo-200 ring-1 ring-indigo-500'
                      : 'bg-white border-gray-200 hover:border-indigo-300 hover:bg-gray-50'
                  }`}
                >
                  <div className={`mt-0.5 p-1.5 rounded-md flex-shrink-0 ${
                    formData.roles.includes(role.id) ? 'bg-indigo-100 text-indigo-600' : 'bg-gray-100 text-gray-400'
                  }`}>
                    {formData.roles.includes(role.id) ? <CheckCircle size={16} /> : <Shield size={16} />}
                  </div>
                  <div className="min-w-0">
                    <h3 className={`text-sm font-medium truncate ${
                      formData.roles.includes(role.id) ? 'text-indigo-900' : 'text-gray-900'
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
            onClick={() => navigate('/usuarios')}
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

export default SiteUserFormPage;
