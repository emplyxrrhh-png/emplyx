export interface Employee {
    id: string;
    empresaId: string;
    centroTrabajoId?: string;
    nombre: string;
    apellidos: string;
    alias: string;
    groupName: string;
    type: string;
    status: string;
    image?: string;

    // Extended General Data
    contractType?: string;
    startDate?: string; // ISO Date string
    idioma?: string;
    remoteWork: boolean;
    notes?: string;
    
    // Control & Access
    idAccessGroup?: string;
    biometricID?: string;
    attControlled: boolean;
    accControlled: boolean;
    jobControlled: boolean;
    extControlled: boolean;
    riskControlled: boolean;

    // Web Credentials
    webLogin?: string;
    webPassword?: string;
    activeDirectory: boolean;

    // Dynamic Custom Fields (Key-Value)
    userFields?: Record<string, string>;

    // Roles
    roles?: UserRole[];
}

export interface UserRole {
    rolId: string;
    rolName: string;
    contextoId?: string;
    contextoNombre?: string;
    contextoTipo?: 'Global' | 'Empresa' | 'Grupo';
}

export interface CreateEmployeeRequest extends Omit<Employee, 'id'> {}

export interface UpdateEmployeeRequest extends Omit<Employee, 'id' | 'empresaId' | 'centroTrabajoId'> {}
