export interface UsuarioPerfilDto {
  nombres?: string;
  apellidos?: string;
  departamento?: string;
  cargo?: string;
  telefono?: string;
}

export interface UsuarioRolDto {
  rolId: string;
  rolName?: string;
  assignedAtUtc: string;
}

export interface UsuarioContextoDto {
  contextoId: string;
  isPrimary: boolean;
  linkedAtUtc: string;
}

export interface UsuarioLicenciaDto {
  licenciaId: string;
  assignedAtUtc: string;
}

export interface UsuarioSesionDto {
  id: string;
  device: string;
  ipAddress?: string;
  createdAtUtc: string;
  expiresAtUtc: string;
  isActive: boolean;
  closedAtUtc?: string;
}

export interface User {
  id: string;
  userName: string;
  email: string;
  displayName: string;
  isActive: boolean;
  clearanceId?: string;
  externalIdentityId?: string;
  preferredContextoId?: string;
  createdAtUtc: string;
  updatedAtUtc: string;
  lastLoginAtUtc?: string;
  lastPasswordChangeAtUtc?: string;
  perfil: UsuarioPerfilDto;
  roles: UsuarioRolDto[];
  contextos: UsuarioContextoDto[];
  licencias: UsuarioLicenciaDto[];
  sesiones: UsuarioSesionDto[];
}

