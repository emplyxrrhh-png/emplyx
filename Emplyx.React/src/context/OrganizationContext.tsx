import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';

export interface OrganizationItem {
  id: string;
  name: string;
  type: 'tenant' | 'empresa';
}

interface OrganizationContextType {
  selectedCompany: OrganizationItem | null;
  setSelectedCompany: (company: OrganizationItem) => void;
  organizations: OrganizationItem[];
  isLoading: boolean;
}

const OrganizationContext = createContext<OrganizationContextType | undefined>(undefined);

export const OrganizationProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [selectedCompany, setSelectedCompany] = useState<OrganizationItem | null>(null);
  const [organizations, setOrganizations] = useState<OrganizationItem[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [tenantsRes, empresasRes] = await Promise.all([
          fetch('https://localhost:5001/api/tenants'),
          fetch('https://localhost:5001/api/empresas')
        ]);

        const tenants = tenantsRes.ok ? await tenantsRes.json() : [];
        const empresas = empresasRes.ok ? await empresasRes.json() : [];

        const combined: OrganizationItem[] = [
          ...tenants.map((t: any) => ({ 
              id: t.id, 
              name: t.commercialName || t.CommercialName || t.legalName || t.LegalName || 'Tenant', 
              type: 'tenant' as const 
          })),
          ...empresas.map((e: any) => ({ 
              id: e.id, 
              name: e.tradeName || e.TradeName || e.legalName || e.LegalName || e.nombre || 'Empresa', 
              type: 'empresa' as const 
          }))
        ];

        setOrganizations(combined);
        if (combined.length > 0) {
            // Try to restore from localStorage or default to first
            const saved = localStorage.getItem('selectedCompanyId');
            const found = saved ? combined.find(o => o.id === saved) : null;
            setSelectedCompany(found || combined[0]);
        }
      } catch (error) {
        console.error("Error fetching organizations", error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchData();
  }, []);

  const handleSetSelectedCompany = (company: OrganizationItem) => {
      setSelectedCompany(company);
      localStorage.setItem('selectedCompanyId', company.id);
  };

  return (
    <OrganizationContext.Provider value={{ selectedCompany, setSelectedCompany: handleSetSelectedCompany, organizations, isLoading }}>
      {children}
    </OrganizationContext.Provider>
  );
};

export const useOrganization = () => {
  const context = useContext(OrganizationContext);
  if (context === undefined) {
    throw new Error('useOrganization must be used within an OrganizationProvider');
  }
  return context;
};
