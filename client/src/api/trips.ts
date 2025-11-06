import api from './axios';

export interface Trip {
  id: string;
  tripNumber: string;
  customerId: string;
  customerName: string;
  truckId: string;
  truckLicensePlate: string;
  driverId: string;
  driverName: string;
  dispatcherId?: string;
  origin: string;
  destination: string;
  scheduledDate: string;
  actualStartDate?: string;
  actualEndDate?: string;
  status: string;
  distance: number;
  fuelCost?: number;
  otherCosts?: number;
  notes?: string;
  createdAt: string;
}

export interface CreateTrip {
  customerId: string;
  truckId: string;
  driverId: string;
  origin: string;
  destination: string;
  scheduledDate: string;
  distance: number;
  notes?: string;
}

export const tripsApi = {
  getAll: async (): Promise<Trip[]> => {
    const response = await api.get<Trip[]>('/trips');
    return response.data;
  },
  getById: async (id: string): Promise<Trip> => {
    const response = await api.get<Trip>(`/trips/${id}`);
    return response.data;
  },
  create: async (data: CreateTrip): Promise<Trip> => {
    const response = await api.post<Trip>('/trips', data);
    return response.data;
  },
  update: async (id: string, data: CreateTrip): Promise<Trip> => {
    const response = await api.put<Trip>(`/trips/${id}`, data);
    return response.data;
  },
  updateStatus: async (id: string, status: string): Promise<Trip> => {
    const response = await api.patch<Trip>(`/trips/${id}/status`, { status });
    return response.data;
  },
  delete: async (id: string): Promise<void> => {
    await api.delete(`/trips/${id}`);
  },
};

