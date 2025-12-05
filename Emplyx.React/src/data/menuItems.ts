import { 
  Home, Building2, Users, Calendar, FileText, Map, MessageSquare, 
  Bot, BarChart3, Euro, Bell, Shield, Settings, Sliders, LucideIcon,
  Clock, Monitor, Database, FileBarChart, MapPin, Network
} from 'lucide-react';

export interface NavItemData {
  label: string;
  icon: LucideIcon;
  href: string;
  children?: NavItemData[];
}

export const MENU_ITEMS: NavItemData[] = [
  { label: "Inicio", icon: Home, href: "/" },
  { 
    label: "Usuarios", 
    icon: Users, 
    href: "/empleados",
    children: [
      { label: "Lista", icon: Users, href: "/empleados" },
      { label: "Árbol", icon: Network, href: "/empleados/arbol" },
      { label: "Expediente de RRHH", icon: Users, href: "/usuarios/expediente" },
      { label: "OnBoarding", icon: Users, href: "/usuarios/onboarding" },
      { label: "Solicitudes", icon: Users, href: "/usuarios/solicitudes" },
      { label: "Encuestas de RRHH", icon: Users, href: "/usuarios/encuestas" }
    ]
  },
  { label: "Calendario", icon: Calendar, href: "/gestion-horaria/calendario" },
  {
    label: "Gestión Horaria",
    icon: Calendar,
    href: "/gestion-horaria",
    children: [
      { label: "Estado del absentismo", icon: Calendar, href: "/gestion-horaria/absentismo" }
    ]
  },
  { label: "Gestor documental", icon: FileText, href: "/gestor-documental" },
  { label: "Estado de las Zonas", icon: Map, href: "/estado-zonas" },
  {
    label: "Comunicaciones",
    icon: MessageSquare,
    href: "/comunicaciones",
    children: [
      { label: "Comunicados", icon: MessageSquare, href: "/comunicaciones/comunicados" }
    ]
  },
  { label: "Reglas", icon: Bot, href: "/reglas" },
  {
    label: "Análisis",
    icon: BarChart3,
    href: "/analisis",
    children: [
      { label: "Analytics by Genius", icon: BarChart3, href: "/analisis/genius" }
    ]
  },
  {
    label: "Informes",
    icon: FileBarChart,
    href: "/informes",
    children: [
      { label: "Informes", icon: FileText, href: "/informes/lista" },
      { label: "Informes solicitados", icon: FileText, href: "/informes/solicitados" }
    ]
  },
  {
    label: "Control de Costes",
    icon: Euro,
    href: "/realcost",
    children: [
      { label: "Centros de coste", icon: Euro, href: "/realcost/centros-coste" }
    ]
  },
  { label: "Alertas", icon: Bell, href: "/alertas" }
];

export const CONFIG_ITEMS: NavItemData = {
  label: "Configuración",
  icon: Settings,
  href: "/configuracion",
  children: [
    {
      label: "Organización",
      icon: Building2,
      href: "/configuracion/organizacion",
      children: [
        { label: "Grupo de Empresas", icon: FileText, href: "/configuracion/organizacion/tenant" },
        { label: "Empresas", icon: Building2, href: "/configuracion/organizacion/empresas" },
        { label: "Roles", icon: Shield, href: "/configuracion/usuarios/roles" },
        { label: "Suscripción", icon: Users, href: "/configuracion/usuarios" },
        { label: "Centros de Trabajo", icon: MapPin, href: "/configuracion/organizacion/centros-trabajo" },
        { label: "Expediente", icon: FileText, href: "/configuracion/usuarios/ficha" },
        { label: "Grupos", icon: Users, href: "/configuracion/usuarios/grupos" },
        { label: "Convenios", icon: FileText, href: "/configuracion/horaria/convenios" }
      ]
    },
    {
      label: "Horarios",
      icon: Clock,
      href: "/configuracion/horaria",
      children: [
        { label: "Definición", icon: Sliders, href: "/configuracion/horaria/definicion" },
        { label: "Justificaciones", icon: MessageSquare, href: "/configuracion/horaria/justificaciones" },
        { label: "Saldos", icon: BarChart3, href: "/configuracion/horaria/saldos" },
        { label: "Metricas", icon: BarChart3, href: "/configuracion/horaria/metricas" },
      ]
    },
    { label: "Notificaciones", icon: Bell, href: "/configuracion/notificaciones" },
    { label: "Zonas", icon: Map, href: "/configuracion/zonas" },
    {
      label: "Seguridad",
      icon: Shield,
      href: "/configuracion/seguridad",
      children: [
        { label: "Auditoria", icon: FileText, href: "/configuracion/seguridad/auditoria" },
      ]
    },
    {
      label: "Enlaces de Datos",
      icon: Database,
      href: "/configuracion/datos",
      children: [
        { label: "Importar/Exportar", icon: Database, href: "/configuracion/datos/importar-exportar" }
      ]
    },
    {
      label: "General",
      icon: Sliders,
      href: "/configuracion/general",
      children: [
        { label: "Fecha Cierre", icon: Calendar, href: "/configuracion/general/fecha-cierre" }
      ]
    },
    { label: "Terminales", icon: Monitor, href: "/configuracion/terminales" }
  ]
};
