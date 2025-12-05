export interface Role {
  id: string;
  name: string;
  description: string;
  isSystem: boolean;
  permissions: string[];
}

export interface CreateRoleRequest {
  name: string;
  description: string;
  permissions: string[];
}

export interface UpdateRoleRequest {
  name: string;
  description: string;
  permissions: string[];
}
