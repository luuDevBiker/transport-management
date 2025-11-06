import api from './axios';

export interface Invoice {
  id: string;
  invoiceNumber: string;
  customerId: string;
  customerName: string;
  tripId?: string;
  tripNumber?: string;
  issueDate: string;
  dueDate: string;
  amount: number;
  taxAmount?: number;
  totalAmount: number;
  paidAmount: number;
  remainingAmount: number;
  status: string;
  description?: string;
  notes?: string;
  createdAt: string;
}

export interface CreateInvoice {
  customerId: string;
  tripId?: string;
  issueDate: string;
  dueDate: string;
  amount: number;
  taxAmount?: number;
  description?: string;
  notes?: string;
}

export const invoicesApi = {
  getAll: async (): Promise<Invoice[]> => {
    const response = await api.get<Invoice[]>('/invoices');
    return response.data;
  },
  getById: async (id: string): Promise<Invoice> => {
    const response = await api.get<Invoice>(`/invoices/${id}`);
    return response.data;
  },
  getByCustomerId: async (customerId: string): Promise<Invoice[]> => {
    const response = await api.get<Invoice[]>(`/invoices/customer/${customerId}`);
    return response.data;
  },
  create: async (data: CreateInvoice): Promise<Invoice> => {
    const response = await api.post<Invoice>('/invoices', data);
    return response.data;
  },
  update: async (id: string, data: CreateInvoice): Promise<Invoice> => {
    const response = await api.put<Invoice>(`/invoices/${id}`, data);
    return response.data;
  },
  delete: async (id: string): Promise<void> => {
    await api.delete(`/invoices/${id}`);
  },
};

