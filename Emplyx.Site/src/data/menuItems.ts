import { 
  Home, Settings, LucideIcon, Building2, Users
} from 'lucide-react';

export interface NavItemData {
  label: string;
  icon: LucideIcon;
  href: string;
  children?: NavItemData[];
}

export const MENU_ITEMS: NavItemData[] = [
  {
    label: "Tenants",
    icon: Building2,
    href: "/tenants"
  },
  {
    label: "Usuarios",
    icon: Users,
    href: "/usuarios"
  }
];

export const CONFIG_ITEMS: NavItemData = {
  label: "Configuración",
  icon: Settings,
  href: "/configuracion",
  children: []
};
