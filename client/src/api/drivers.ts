import api from './axios';

export interface Driver {
  id: string;
  fullName: string;
  phone: string;
  email?: string;
  licenseNumber: string;
  licenseExpiryDate: string;
  address?: string;
  status: string;
  assignedTruckId?: string;
  notes?: string;
  createdAt: string;
}

export interface CreateDriver {
  fullName: string;
  phone: string;
  email?: string;
  licenseNumber: string;
  licenseExpiryDate: string;
  address?: string;
  status?: string;
  notes?: string;
}

export const driversApi = {
  getAll: async (): Promise<Driver[]> => {
    const response = await api.get<Driver[]>('/drivers');
    return response.data;
  },
  getById: async (id: string): Promise<Driver> => {
    const response = await api.get<Driver>(`/drivers/${id}`);
    return response.data;
  },
  create: async (data: CreateDriver): Promise<Driver> => {
    const response = await api.post<Driver>('/drivers', data);
    return response.data;
  },
  update: async (id: string, data: CreateDriver): Promise<Driver> => {
    const response = await api.put<Driver>(`/drivers/${id}`, data);
    return response.data;
  },
  delete: async (id: string): Promise<void> => {
    await api.delete(`/drivers/${id}`);
  },
};

