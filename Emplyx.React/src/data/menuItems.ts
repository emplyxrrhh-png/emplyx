import { 
  Home, Building2, Users, Calendar, FileText, Map, MessageSquare, 
  Bot, BarChart3, Euro, Bell, Shield, Settings, Sliders, LucideIcon,
  Clock, Monitor
} from 'lucide-react';

export interface NavItemData {
  label: string;
  icon: LucideIcon;
  href: string;
  children?: NavItemData[];
}

export const MENU_ITEMS: NavItemData[] = [
  { label: "Inicio", icon: Home, href: "/" },
  { label: "Organización", icon: Building2, href: "/organizacion" },
  {
    label: "Usuarios",
    icon: Users,
    href: "/usuarios",
    children: [
      { label: "Expediente de RRHH", icon: Users, href: "/usuarios/expediente" },
      { label: "OnBoarding", icon: Users, href: "/usuarios/onboarding" },
      { label: "Solicitudes", icon: Users, href: "/usuarios/solicitudes" },
      { label: "Encuestas de RRHH", icon: Users, href: "/usuarios/encuestas" }
    ]
  },
  {
    label: "Gestión Horaria",
    icon: Calendar,
    href: "/gestion-horaria",
    children: [
      { label: "Calendario", icon: Calendar, href: "/gestion-horaria/calendario" },
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
  { label: "Bots", icon: Bot, href: "/bots" },
  {
    label: "Análisis, informes y datos",
    icon: BarChart3,
    href: "/analisis",
    children: [
      { label: "Analytics by Genius", icon: BarChart3, href: "/analisis/genius" },
      { label: "Informes", icon: BarChart3, href: "/analisis/informes" },
      { label: "Informes solicitados", icon: BarChart3, href: "/analisis/solicitados" },
      { label: "Importar/Exportar", icon: BarChart3, href: "/analisis/importar-exportar" }
    ]
  },
  {
    label: "RealCost",
    icon: Euro,
    href: "/realcost",
    children: [
      { label: "Centros de coste", icon: Euro, href: "/realcost/centros-coste" }
    ]
  },
  { label: "Alertas", icon: Bell, href: "/alertas" },
  {
    label: "Seguridad y auditoría",
    icon: Shield,
    href: "/seguridad",
    children: [
      { label: "Auditoría", icon: Shield, href: "/seguridad/auditoria" },
      { label: "Licencia", icon: Shield, href: "/seguridad/licencia" },
      { label: "Fecha de cierre", icon: Shield, href: "/seguridad/fecha-cierre" },
      { label: "Seguridad avanzada", icon: Shield, href: "/seguridad/avanzada" }
    ]
  },
  {
    label: "Configuración gestión horaria",
    icon: Sliders,
    href: "/config-horaria",
    children: [
      { label: "Convenios", icon: Sliders, href: "/config-horaria/convenios" },
      { label: "Horarios", icon: Sliders, href: "/config-horaria/horarios" },
      { label: "Justificaciones", icon: Sliders, href: "/config-horaria/justificaciones" },
      { label: "Saldos", icon: Sliders, href: "/config-horaria/saldos" },
      { label: "Indicadores", icon: Sliders, href: "/config-horaria/indicadores" }
    ]
  }
];

export const CONFIG_ITEMS: NavItemData = {
  label: "Configuración",
  icon: Settings,
  href: "/configuracion",
  children: [
    {
      label: "Usuarios",
      icon: Users,
      href: "/configuracion/usuarios",
      children: [
        { label: "Ficha", icon: FileText, href: "/configuracion/usuarios/ficha" },
        { label: "Grupos", icon: Users, href: "/configuracion/usuarios/grupos" },
        { label: "Roles", icon: Shield, href: "/configuracion/usuarios/roles" },
      ]
    },
    { label: "Zonas", icon: Map, href: "/configuracion/zonas" },
    { label: "Notificaciones", icon: Bell, href: "/configuracion/notificaciones" },
    { label: "Terminales", icon: Monitor, href: "/configuracion/terminales" },
    {
      label: "Horaria",
      icon: Clock,
      href: "/configuracion/horaria",
      children: [
        { label: "Convenios", icon: FileText, href: "/configuracion/horaria/convenios" },
        { label: "Definición", icon: Sliders, href: "/configuracion/horaria/definicion" },
        { label: "Justificaciones", icon: MessageSquare, href: "/configuracion/horaria/justificaciones" },
        { label: "Saldos", icon: BarChart3, href: "/configuracion/horaria/saldos" },
        { label: "Metricas", icon: BarChart3, href: "/configuracion/horaria/metricas" },
      ]
    },
    {
      label: "Seguridad",
      icon: Shield,
      href: "/configuracion/seguridad",
      children: [
        { label: "Auditoria", icon: FileText, href: "/configuracion/seguridad/auditoria" },
        { label: "Cierre Contable", icon: Euro, href: "/configuracion/seguridad/cierre-contable" },
      ]
    }
  ]
};
