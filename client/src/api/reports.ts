import api from './axios';

export interface RevenueReport {
  fromDate: string;
  toDate: string;
  totalRevenue: number;
  totalPaid: number;
  totalOutstanding: number;
  totalTrips: number;
  completedTrips: number;
}

export interface DebtReport {
  customerId: string;
  customerName: string;
  totalDebt: number;
  totalPaid: number;
  remainingDebt: number;
  invoiceCount: number;
  oldestInvoiceDate?: string;
  daysOverdue: number;
}

export interface TripStatusReport {
  status: string;
  count: number;
  totalDistance: number;
}

// Dashboard
export interface Dashboard {
  summary: DashboardSummary;
  revenue: DashboardRevenue;
  trips: DashboardTrip;
  debt: DashboardDebt;
  recentTrips: DashboardRecentTrip[];
  topCustomers: DashboardTopCustomer[];
  truckStatus: DashboardTruckStatus[];
}

export interface DashboardSummary {
  totalCustomers: number;
  totalTrucks: number;
  totalDrivers: number;
  activeTrips: number;
  pendingInvoices: number;
  totalOutstandingDebt: number;
}

export interface DashboardRevenue {
  todayRevenue: number;
  thisWeekRevenue: number;
  thisMonthRevenue: number;
  thisYearRevenue: number;
  lastMonthRevenue: number;
  revenueGrowth: number;
}

export interface DashboardTrip {
  todayTrips: number;
  thisWeekTrips: number;
  thisMonthTrips: number;
  completedTrips: number;
  inProgressTrips: number;
  scheduledTrips: number;
  averageDistance: number;
  totalDistance: number;
}

export interface DashboardDebt {
  totalDebt: number;
  overdueDebt: number;
  overdueInvoices: number;
  pendingInvoices: number;
  paidInvoices: number;
}

export interface DashboardRecentTrip {
  id: string;
  tripNumber: string;
  customerName: string;
  origin: string;
  destination: string;
  status: string;
  scheduledDate: string;
  distance?: number;
}

export interface DashboardTopCustomer {
  customerId: string;
  customerName: string;
  totalRevenue: number;
  tripCount: number;
  remainingDebt: number;
}

export interface DashboardTruckStatus {
  status: string;
  count: number;
  maintenanceDue: number;
}

// Revenue Detail
export interface RevenueDetailReport {
  fromDate: string;
  toDate: string;
  summary: RevenueSummary;
  revenueByPeriod: RevenueByPeriod[];
  revenueByCustomer: RevenueByCustomer[];
  revenueByTrip: RevenueByTrip[];
  trend: RevenueTrend;
}

export interface RevenueSummary {
  totalRevenue: number;
  totalPaid: number;
  totalOutstanding: number;
  averageInvoiceAmount: number;
  totalInvoices: number;
  paidInvoices: number;
  pendingInvoices: number;
  overdueInvoices: number;
}

export interface RevenueByPeriod {
  period: string;
  periodDate: string;
  revenue: number;
  paid: number;
  outstanding: number;
  invoiceCount: number;
}

export interface RevenueByCustomer {
  customerId: string;
  customerName: string;
  totalRevenue: number;
  totalPaid: number;
  remainingDebt: number;
  invoiceCount: number;
  tripCount: number;
}

export interface RevenueByTrip {
  tripId: string;
  tripNumber: string;
  customerName: string;
  revenue: number;
  paid: number;
  outstanding: number;
  issueDate: string;
  status: string;
}

export interface RevenueTrend {
  currentPeriodRevenue: number;
  previousPeriodRevenue: number;
  growthRate: number;
  trend: string;
}

// Trip Detail
export interface TripDetailReport {
  fromDate: string;
  toDate: string;
  summary: TripSummary;
  tripsByStatus: TripByStatus[];
  tripsByPeriod: TripByPeriod[];
  tripsByTruck: TripByTruck[];
  tripsByDriver: TripByDriver[];
  tripsByCustomer: TripByCustomer[];
}

export interface TripSummary {
  totalTrips: number;
  completedTrips: number;
  inProgressTrips: number;
  scheduledTrips: number;
  cancelledTrips: number;
  totalDistance: number;
  averageDistance: number;
  totalFuelCost: number;
  averageFuelCost: number;
  totalOtherCosts: number;
  totalRevenue: number;
  averageRevenue: number;
}

export interface TripByStatus {
  status: string;
  count: number;
  totalDistance: number;
  averageDistance: number;
  totalRevenue: number;
}

export interface TripByPeriod {
  period: string;
  periodDate: string;
  tripCount: number;
  completedCount: number;
  totalDistance: number;
  totalRevenue: number;
}

export interface TripByTruck {
  truckId: string;
  licensePlate: string;
  brand: string;
  model: string;
  tripCount: number;
  completedCount: number;
  totalDistance: number;
  totalRevenue: number;
  utilizationRate: number;
}

export interface TripByDriver {
  driverId: string;
  driverName: string;
  licenseNumber: string;
  tripCount: number;
  completedCount: number;
  totalDistance: number;
  totalRevenue: number;
  averageRating: number;
}

export interface TripByCustomer {
  customerId: string;
  customerName: string;
  tripCount: number;
  completedCount: number;
  totalDistance: number;
  totalRevenue: number;
  averageRevenue: number;
}

// Truck Report
export interface TruckReport {
  summary: TruckSummary;
  truckUtilization: TruckUtilization[];
  maintenanceSchedule: TruckMaintenance[];
  performance: TruckPerformance[];
}

export interface TruckSummary {
  totalTrucks: number;
  availableTrucks: number;
  inUseTrucks: number;
  maintenanceTrucks: number;
  inactiveTrucks: number;
  maintenanceDue: number;
  maintenanceOverdue: number;
  averageUtilization: number;
}

export interface TruckUtilization {
  truckId: string;
  licensePlate: string;
  brand: string;
  model: string;
  status: string;
  tripCount: number;
  daysInUse: number;
  utilizationRate: number;
  totalDistance: number;
  totalRevenue: number;
}

export interface TruckMaintenance {
  truckId: string;
  licensePlate: string;
  brand: string;
  model: string;
  lastMaintenanceDate?: string;
  nextMaintenanceDate?: string;
  maintenanceIntervalDays?: number;
  daysUntilMaintenance: number;
  isOverdue: boolean;
  status: string;
}

export interface TruckPerformance {
  truckId: string;
  licensePlate: string;
  tripCount: number;
  totalDistance: number;
  totalFuelCost: number;
  averageFuelCostPerKm: number;
  totalRevenue: number;
  profitMargin: number;
  averageTripDistance: number;
}

// Driver Report
export interface DriverReport {
  summary: DriverSummary;
  driverPerformance: DriverPerformance[];
  driverTrips: DriverTrip[];
}

export interface DriverSummary {
  totalDrivers: number;
  availableDrivers: number;
  onTripDrivers: number;
  offDutyDrivers: number;
  inactiveDrivers: number;
  licenseExpiringSoon: number;
  licenseExpired: number;
}

export interface DriverPerformance {
  driverId: string;
  driverName: string;
  licenseNumber: string;
  status: string;
  totalTrips: number;
  completedTrips: number;
  inProgressTrips: number;
  completionRate: number;
  totalDistance: number;
  averageDistance: number;
  totalRevenue: number;
  licenseExpiryDate?: string;
  daysUntilLicenseExpiry: number;
  isLicenseExpiringSoon: boolean;
  isLicenseExpired: boolean;
}

export interface DriverTrip {
  driverId: string;
  driverName: string;
  tripId: string;
  tripNumber: string;
  customerName: string;
  origin: string;
  destination: string;
  status: string;
  scheduledDate: string;
  actualStartDate?: string;
  actualEndDate?: string;
  distance?: number;
  revenue?: number;
}

// Customer Report
export interface CustomerReport {
  summary: CustomerSummary;
  customerDetails: CustomerDetail[];
  customerRevenue: CustomerRevenue[];
  customerDebt: CustomerDebt[];
}

export interface CustomerSummary {
  totalCustomers: number;
  activeCustomers: number;
  inactiveCustomers: number;
  totalRevenue: number;
  totalDebt: number;
  averageRevenuePerCustomer: number;
  totalTrips: number;
}

export interface CustomerDetail {
  customerId: string;
  customerName: string;
  phone: string;
  email: string;
  address: string;
  tripCount: number;
  invoiceCount: number;
  totalRevenue: number;
  totalPaid: number;
  remainingDebt: number;
  lastTripDate?: string;
  lastInvoiceDate?: string;
  isActive: boolean;
}

export interface CustomerRevenue {
  customerId: string;
  customerName: string;
  period: string;
  periodDate: string;
  revenue: number;
  paid: number;
  outstanding: number;
  tripCount: number;
  invoiceCount: number;
}

export interface CustomerDebt {
  customerId: string;
  customerName: string;
  totalDebt: number;
  overdueDebt: number;
  totalInvoices: number;
  overdueInvoices: number;
  pendingInvoices: number;
  oldestInvoiceDate?: string;
  daysOverdue: number;
}

export const reportsApi = {
  getRevenueReport: async (fromDate?: string, toDate?: string): Promise<RevenueReport> => {
    const params = new URLSearchParams();
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    const response = await api.get<RevenueReport>(`/reports/revenue?${params.toString()}`);
    return response.data;
  },
  getDebtReport: async (): Promise<DebtReport[]> => {
    const response = await api.get<DebtReport[]>('/reports/debt');
    return response.data;
  },
  getTripStatusReport: async (): Promise<TripStatusReport[]> => {
    const response = await api.get<TripStatusReport[]>('/reports/trip-status');
    return response.data;
  },
  getDashboard: async (): Promise<Dashboard> => {
    const response = await api.get<Dashboard>('/reports/dashboard');
    return response.data;
  },
  getRevenueDetailReport: async (fromDate?: string, toDate?: string, periodType: string = 'month'): Promise<RevenueDetailReport> => {
    const params = new URLSearchParams();
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    params.append('periodType', periodType);
    const response = await api.get<RevenueDetailReport>(`/reports/revenue-detail?${params.toString()}`);
    return response.data;
  },
  getTripDetailReport: async (fromDate?: string, toDate?: string): Promise<TripDetailReport> => {
    const params = new URLSearchParams();
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);
    const response = await api.get<TripDetailReport>(`/reports/trip-detail?${params.toString()}`);
    return response.data;
  },
  getTruckReport: async (): Promise<TruckReport> => {
    const response = await api.get<TruckReport>('/reports/truck');
    return response.data;
  },
  getDriverReport: async (): Promise<DriverReport> => {
    const response = await api.get<DriverReport>('/reports/driver');
    return response.data;
  },
  getCustomerReport: async (): Promise<CustomerReport> => {
    const response = await api.get<CustomerReport>('/reports/customer');
    return response.data;
  },
};
