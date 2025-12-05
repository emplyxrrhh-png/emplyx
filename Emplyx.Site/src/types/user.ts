export interface User {
  id: string;
  username: string;
  email: string;
  fullName: string;
  isActive: boolean;
  roles: string[];
  lastLogin?: string;
}
