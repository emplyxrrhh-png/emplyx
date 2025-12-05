import { Routes, Route } from 'react-router-dom';
import { MainLayout } from './layouts/MainLayout';
import { ProtectedRoute } from './components/ProtectedRoute';
import { Construction } from 'lucide-react';
import { MENU_ITEMS, CONFIG_ITEMS, NavItemData } from './data/menuItems';
import EmpresasPage from './pages/configuracion/organizacion/EmpresasPage';
import EmpresaFormPage from './pages/configuracion/organizacion/EmpresaFormPage';
import TenantsPage from './pages/configuracion/organizacion/TenantsPage';
import TenantFormPage from './pages/configuracion/organizacion/TenantFormPage';
import CentrosTrabajoPage from './pages/configuracion/organizacion/CentrosTrabajoPage';
import CentroTrabajoFormPage from './pages/configuracion/organizacion/CentroTrabajoFormPage';
import EmployeesPage from './pages/configuracion/organizacion/EmployeesPage';
import EmployeeFormPage from './pages/configuracion/organizacion/EmployeeFormPage';
import EmployeesTreePage from './pages/configuracion/organizacion/EmployeesTreePage';
import RolesPage from './pages/configuracion/usuarios/RolesPage';
import RoleFormPage from './pages/configuracion/usuarios/RoleFormPage';
import UsersPage from './pages/configuracion/usuarios/UsersPage';
import UserFormPage from './pages/configuracion/usuarios/UserFormPage';
import LoginPage from './pages/LoginPage';
import { OrganizationProvider } from './context/OrganizationContext';

const UnderConstruction = ({ title }: { title: string }) => (
  <div className="flex flex-col items-center justify-center h-[60vh] text-center">
    <div className="w-24 h-24 bg-indigo-50 rounded-full flex items-center justify-center mb-6">
      <Construction size={48} className="text-indigo-600" />
    </div>
    <h1 className="text-3xl font-bold text-gray-900 mb-2">{title}</h1>
    <p className="text-gray-500 max-w-md">
      Estamos trabajando duro para traerte esta funcionalidad. ¡Vuelve pronto!
    </p>
  </div>
);

const Dashboard = () => (
  <div>
    <h1 className="text-2xl font-bold text-gray-900 mb-6">Bienvenido a Emplyx</h1>
    <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
      {[1, 2, 3].map((i) => (
        <div key={i} className="bg-white p-6 rounded-xl shadow-sm border border-gray-100">
          <div className="h-4 w-24 bg-gray-100 rounded mb-4"></div>
          <div className="h-8 w-16 bg-indigo-100 rounded mb-2"></div>
          <div className="h-4 w-full bg-gray-50 rounded"></div>
        </div>
      ))}
    </div>
  </div>
);

const flattenRoutes = (items: NavItemData[]): { path: string; title: string }[] => {
  let routes: { path: string; title: string }[] = [];
  items.forEach(item => {
    if (item.href && item.href !== '/') {
       // Remove leading slash for Route path if it's not root, but react-router v6 handles absolute paths fine in nested routes if they match parent.
       // Actually, since MainLayout is at "/", child routes like "/organization" will work.
       routes.push({ path: item.href, title: item.label });
    }
    if (item.children) {
      routes = [...routes, ...flattenRoutes(item.children)];
    }
  });
  return routes;
};

function App() {
  const allRoutes = [
    ...flattenRoutes(MENU_ITEMS),
    ...flattenRoutes([CONFIG_ITEMS])
  ];

  return (
    <OrganizationProvider>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route element={<ProtectedRoute />}>
          <Route path="/" element={<MainLayout />}>
            <Route index element={<Dashboard />} />
            
            {/* Rutas implementadas manualmente */}
            <Route path="/configuracion/organizacion/empresas" element={<EmpresasPage />} />
            <Route path="/configuracion/organizacion/empresas/nueva" element={<EmpresaFormPage />} />
            <Route path="/configuracion/organizacion/empresas/editar/:id" element={<EmpresaFormPage />} />

            <Route path="/configuracion/organizacion/tenant" element={<TenantsPage />} />
            <Route path="/configuracion/organizacion/tenant/nuevo" element={<TenantFormPage />} />
            <Route path="/configuracion/organizacion/tenant/editar/:id" element={<TenantFormPage />} />

            <Route path="/configuracion/organizacion/centros-trabajo" element={<CentrosTrabajoPage />} />
            <Route path="/configuracion/organizacion/centros-trabajo/nuevo" element={<CentroTrabajoFormPage />} />
            <Route path="/configuracion/organizacion/centros-trabajo/editar/:id" element={<CentroTrabajoFormPage />} />

            <Route path="/empleados" element={<EmployeesPage />} />
            <Route path="/empleados/arbol" element={<EmployeesTreePage />} />
            <Route path="/empleados/nuevo" element={<EmployeeFormPage />} />
            <Route path="/empleados/editar/:id" element={<EmployeeFormPage />} />

            <Route path="/configuracion/usuarios/roles" element={<RolesPage />} />
            <Route path="/configuracion/usuarios/roles/nuevo" element={<RoleFormPage />} />
            <Route path="/configuracion/usuarios/roles/editar/:id" element={<RoleFormPage />} />

            <Route path="/configuracion/usuarios" element={<UsersPage />} />
            <Route path="/configuracion/usuarios/nuevo" element={<UserFormPage />} />
            <Route path="/configuracion/usuarios/editar/:id" element={<UserFormPage />} />

            {allRoutes.map((route) => {
              // Excluir rutas que ya tienen implementación específica
              if (route.path === '/configuracion/organizacion/empresas') return null;
              if (route.path === '/configuracion/organizacion/tenant') return null;
              if (route.path === '/configuracion/organizacion/centros-trabajo') return null;
              if (route.path === '/empleados') return null;
              if (route.path === '/configuracion/usuarios/roles') return null;
              if (route.path === '/configuracion/usuarios') return null;
              
              return (
                <Route 
                  key={route.path} 
                  path={route.path} 
                  element={<UnderConstruction title={route.title} />} 
                />
              );
            })}
            <Route path="*" element={<UnderConstruction title="Página no encontrada" />} />
          </Route>
        </Route>
      </Routes>
    </OrganizationProvider>
  );
}

export default App;
