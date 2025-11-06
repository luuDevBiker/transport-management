import api from './axios';

export interface Payment {
  id: string;
  paymentNumber: string;
  invoiceId: string;
  invoiceNumber: string;
  paymentDate: string;
  amount: number;
  paymentMethod: string;
  referenceNumber?: string;
  notes?: string;
  createdAt: string;
}

export interface CreatePayment {
  invoiceId: string;
  paymentDate: string;
  amount: number;
  paymentMethod: string;
  referenceNumber?: string;
  notes?: string;
}

export const paymentsApi = {
  getAll: async (): Promise<Payment[]> => {
    const response = await api.get<Payment[]>('/payments');
    return response.data;
  },
  getById: async (id: string): Promise<Payment> => {
    const response = await api.get<Payment>(`/payments/${id}`);
    return response.data;
  },
  getByInvoiceId: async (invoiceId: string): Promise<Payment[]> => {
    const response = await api.get<Payment[]>(`/payments/invoice/${invoiceId}`);
    return response.data;
  },
  create: async (data: CreatePayment): Promise<Payment> => {
    const response = await api.post<Payment>('/payments', data);
    return response.data;
  },
  delete: async (id: string): Promise<void> => {
    await api.delete(`/payments/${id}`);
  },
};

