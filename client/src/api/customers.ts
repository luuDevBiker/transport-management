import api from './axios';

export interface Customer {
  id: string;
  name: string;
  companyName?: string;
  phone: string;
  email?: string;
  address: string;
  taxCode?: string;
  notes?: string;
  createdAt: string;
}

export interface CreateCustomer {
  name: string;
  companyName?: string;
  phone: string;
  email?: string;
  address: string;
  taxCode?: string;
  notes?: string;
}

export const customersApi = {
  getAll: async (): Promise<Customer[]> => {
    const response = await api.get<Customer[]>('/customers');
    return response.data;
  },
  getById: async (id: string): Promise<Customer> => {
    const response = await api.get<Customer>(`/customers/${id}`);
    return response.data;
  },
  create: async (data: CreateCustomer): Promise<Customer> => {
    const response = await api.post<Customer>('/customers', data);
    return response.data;
  },
  update: async (id: string, data: CreateCustomer): Promise<Customer> => {
    const response = await api.put<Customer>(`/customers/${id}`, data);
    return response.data;
  },
  delete: async (id: string): Promise<void> => {
    await api.delete(`/customers/${id}`);
  },
};

