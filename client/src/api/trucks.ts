import api from './axios';

export interface Truck {
  id: string;
  licensePlate: string;
  brand: string;
  model: string;
  year: number;
  capacity: number;
  status: string;
  lastMaintenanceDate?: string;
  nextMaintenanceDate?: string;
  maintenanceIntervalDays?: number;
  notes?: string;
  createdAt: string;
}

export interface CreateTruck {
  licensePlate: string;
  brand: string;
  model: string;
  year: number;
  capacity: number;
  status?: string;
  lastMaintenanceDate?: string;
  maintenanceIntervalDays?: number;
  notes?: string;
}

export const trucksApi = {
  getAll: async (): Promise<Truck[]> => {
    const response = await api.get<Truck[]>('/trucks');
    return response.data;
  },
  getById: async (id: string): Promise<Truck> => {
    const response = await api.get<Truck>(`/trucks/${id}`);
    return response.data;
  },
  create: async (data: CreateTruck): Promise<Truck> => {
    const response = await api.post<Truck>('/trucks', data);
    return response.data;
  },
  update: async (id: string, data: CreateTruck): Promise<Truck> => {
    const response = await api.put<Truck>(`/trucks/${id}`, data);
    return response.data;
  },
  delete: async (id: string): Promise<void> => {
    await api.delete(`/trucks/${id}`);
  },
};

