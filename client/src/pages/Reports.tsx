import React, { useEffect, useState } from 'react';
import {
  Card,
  Row,
  Col,
  Statistic,
  Table,
  DatePicker,
  Button,
  Tag,
  Select,
  Space,
  Typography,
  Tabs,
  Spin,
  Alert,
  Divider,
} from 'antd';
import {
  DollarOutlined,
  FileTextOutlined,
  CarOutlined,
  FilterOutlined,
  ReloadOutlined,
  DownloadOutlined,
  UserOutlined,
} from '@ant-design/icons';
import {
  LineChart,
  Line,
  BarChart,
  Bar,
  PieChart,
  Pie,
  Cell,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from 'recharts';
import {
  reportsApi,
  RevenueReport,
  DebtReport,
  TripStatusReport,
  RevenueDetailReport,
  TripDetailReport,
  TruckReport,
  DriverReport,
  CustomerReport,
} from '../api/reports';
import dayjs, { Dayjs } from 'dayjs';

const { Title, Text } = Typography;
const { RangePicker } = DatePicker;
const { Option } = Select;

const Reports: React.FC = () => {
  // Filter states
  const [dateRange, setDateRange] = useState<[Dayjs, Dayjs]>([
    dayjs().subtract(1, 'month'),
    dayjs(),
  ]);
  const [periodType, setPeriodType] = useState<string>('month');
  const [activeTab, setActiveTab] = useState<string>('revenue');

  // Report states
  const [revenueReport, setRevenueReport] = useState<RevenueReport | null>(null);
  const [revenueDetailReport, setRevenueDetailReport] = useState<RevenueDetailReport | null>(null);
  const [debtReport, setDebtReport] = useState<DebtReport[]>([]);
  const [tripStatusReport, setTripStatusReport] = useState<TripStatusReport[]>([]);
  const [tripDetailReport, setTripDetailReport] = useState<TripDetailReport | null>(null);
  const [truckReport, setTruckReport] = useState<TruckReport | null>(null);
  const [driverReport, setDriverReport] = useState<DriverReport | null>(null);
  const [customerReport, setCustomerReport] = useState<CustomerReport | null>(null);

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchReports();
  }, [activeTab, dateRange, periodType]);

  const fetchReports = async () => {
    setLoading(true);
    setError(null);
    try {
      const fromDate = dateRange[0].toISOString();
      const toDate = dateRange[1].toISOString();

      switch (activeTab) {
        case 'revenue':
          const [revenue, revenueDetail] = await Promise.all([
            reportsApi.getRevenueReport(fromDate, toDate),
            reportsApi.getRevenueDetailReport(fromDate, toDate, periodType),
          ]);
          setRevenueReport(revenue);
          setRevenueDetailReport(revenueDetail);
          break;
        case 'trips':
          const [tripStatus, tripDetail] = await Promise.all([
            reportsApi.getTripStatusReport(),
            reportsApi.getTripDetailReport(fromDate, toDate),
          ]);
          setTripStatusReport(tripStatus);
          setTripDetailReport(tripDetail);
          break;
        case 'debt':
          const debt = await reportsApi.getDebtReport();
          setDebtReport(debt);
          break;
        case 'trucks':
          const trucks = await reportsApi.getTruckReport();
          setTruckReport(trucks);
          break;
        case 'drivers':
          const drivers = await reportsApi.getDriverReport();
          setDriverReport(drivers);
          break;
        case 'customers':
          const customers = await reportsApi.getCustomerReport();
          setCustomerReport(customers);
          break;
      }
    } catch (err: any) {
      setError(err.message || 'Không thể tải dữ liệu báo cáo');
      console.error('Error fetching reports:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleDateRangeChange = (dates: any) => {
    if (dates) {
      setDateRange(dates);
    }
  };

  const handlePeriodTypeChange = (value: string) => {
    setPeriodType(value);
  };

  const handleRefresh = () => {
    fetchReports();
  };

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND',
    }).format(value);
  };

  const formatNumber = (value: number) => {
    return new Intl.NumberFormat('vi-VN').format(value);
  };

  const getStatusColor = (status: string) => {
    const colors: { [key: string]: string } = {
      Completed: 'success',
      InProgress: 'processing',
      Scheduled: 'default',
      Cancelled: 'error',
      Paid: 'success',
      Partial: 'warning',
      Pending: 'default',
      Overdue: 'error',
    };
    return colors[status] || 'default';
  };

  // Filter Panel Component
  const FilterPanel = () => (
    <Card
      title={
        <Space>
          <FilterOutlined />
          <span>Bộ Lọc</span>
        </Space>
      }
      extra={
        <Space>
          <Button icon={<ReloadOutlined />} onClick={handleRefresh} loading={loading}>
            Làm mới
          </Button>
          <Button icon={<DownloadOutlined />} type="primary">
            Xuất Excel
          </Button>
        </Space>
      }
      style={{ marginBottom: 16 }}
    >
      <Row gutter={16} align="middle">
        <Col xs={24} sm={12} md={8}>
          <Space direction="vertical" style={{ width: '100%' }}>
            <Text strong>Khoảng thời gian:</Text>
            <RangePicker
              value={dateRange}
              onChange={handleDateRangeChange}
              format="DD/MM/YYYY"
              style={{ width: '100%' }}
              allowClear={false}
            />
          </Space>
        </Col>
        <Col xs={24} sm={12} md={8}>
          <Space direction="vertical" style={{ width: '100%' }}>
            <Text strong>Loại báo cáo:</Text>
            <Select
              value={activeTab}
              onChange={setActiveTab}
              style={{ width: '100%' }}
            >
              <Option value="revenue">Doanh thu</Option>
              <Option value="trips">Chuyến hàng</Option>
              <Option value="debt">Công nợ</Option>
              <Option value="trucks">Xe tải</Option>
              <Option value="drivers">Tài xế</Option>
              <Option value="customers">Khách hàng</Option>
            </Select>
          </Space>
        </Col>
        {(activeTab === 'revenue' || activeTab === 'trips') && (
          <Col xs={24} sm={12} md={8}>
            <Space direction="vertical" style={{ width: '100%' }}>
              <Text strong>Chu kỳ:</Text>
              <Select
                value={periodType}
                onChange={handlePeriodTypeChange}
                style={{ width: '100%' }}
              >
                <Option value="day">Theo ngày</Option>
                <Option value="week">Theo tuần</Option>
                <Option value="month">Theo tháng</Option>
                <Option value="quarter">Theo quý</Option>
              </Select>
            </Space>
          </Col>
        )}
        <Col xs={24}>
          <Space>
            <Button
              type="primary"
              onClick={fetchReports}
              loading={loading}
              icon={<FilterOutlined />}
            >
              Áp dụng bộ lọc
            </Button>
            <Button
              onClick={() => {
                setDateRange([dayjs().subtract(1, 'month'), dayjs()]);
                setPeriodType('month');
              }}
            >
              Đặt lại
            </Button>
          </Space>
        </Col>
      </Row>
    </Card>
  );

  // Revenue Report Tab
  const RevenueTab = () => {
    if (!revenueReport || !revenueDetailReport) return null;

    const revenueByPeriodColumns = [
      { title: 'Chu kỳ', dataIndex: 'period', key: 'period' },
      {
        title: 'Doanh thu',
        dataIndex: 'revenue',
        key: 'revenue',
        render: (value: number) => formatCurrency(value),
        sorter: (a: any, b: any) => a.revenue - b.revenue,
      },
      {
        title: 'Đã thu',
        dataIndex: 'paid',
        key: 'paid',
        render: (value: number) => formatCurrency(value),
      },
      {
        title: 'Còn lại',
        dataIndex: 'outstanding',
        key: 'outstanding',
        render: (value: number) => (
          <Text type={value > 0 ? 'warning' : 'success'}>
            {formatCurrency(value)}
          </Text>
        ),
      },
      { title: 'Số hóa đơn', dataIndex: 'invoiceCount', key: 'invoiceCount' },
    ];

    const revenueByCustomerColumns = [
      { title: 'Khách hàng', dataIndex: 'customerName', key: 'customerName' },
      {
        title: 'Doanh thu',
        dataIndex: 'totalRevenue',
        key: 'totalRevenue',
        render: (value: number) => formatCurrency(value),
        sorter: (a: any, b: any) => a.totalRevenue - b.totalRevenue,
      },
      {
        title: 'Đã thu',
        dataIndex: 'totalPaid',
        key: 'totalPaid',
        render: (value: number) => formatCurrency(value),
      },
      {
        title: 'Công nợ',
        dataIndex: 'remainingDebt',
        key: 'remainingDebt',
        render: (value: number) => (
          <Text type={value > 0 ? 'danger' : 'success'}>
            {formatCurrency(value)}
          </Text>
        ),
      },
      { title: 'Số chuyến', dataIndex: 'tripCount', key: 'tripCount' },
    ];

    return (
      <div>
        <Row gutter={16} style={{ marginBottom: 24 }}>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Tổng doanh thu"
                value={revenueReport.totalRevenue}
                prefix={<DollarOutlined />}
                formatter={(value) => formatCurrency(Number(value))}
              />
            </Card>
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Đã thu"
                value={revenueReport.totalPaid}
                prefix={<DollarOutlined />}
                formatter={(value) => formatCurrency(Number(value))}
                valueStyle={{ color: '#52c41a' }}
              />
            </Card>
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Còn lại"
                value={revenueReport.totalOutstanding}
                prefix={<DollarOutlined />}
                formatter={(value) => formatCurrency(Number(value))}
                valueStyle={{ color: '#ff4d4f' }}
              />
            </Card>
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Chuyến hàng"
                value={revenueReport.completedTrips}
                prefix={<CarOutlined />}
                suffix={`/ ${revenueReport.totalTrips}`}
              />
            </Card>
          </Col>
        </Row>

        {revenueDetailReport.trend && (
          <Card title="Xu hướng doanh thu" style={{ marginBottom: 24 }}>
            <Row gutter={16}>
              <Col span={8}>
                <Statistic
                  title="Kỳ hiện tại"
                  value={revenueDetailReport.trend.currentPeriodRevenue}
                  formatter={(value) => formatCurrency(Number(value))}
                />
              </Col>
              <Col span={8}>
                <Statistic
                  title="Kỳ trước"
                  value={revenueDetailReport.trend.previousPeriodRevenue}
                  formatter={(value) => formatCurrency(Number(value))}
                />
              </Col>
              <Col span={8}>
                <Statistic
                  title="Tăng trưởng"
                  value={revenueDetailReport.trend.growthRate}
                  suffix="%"
                  valueStyle={{
                    color:
                      revenueDetailReport.trend.growthRate > 0
                        ? '#52c41a'
                        : revenueDetailReport.trend.growthRate < 0
                        ? '#ff4d4f'
                        : '#1890ff',
                  }}
                />
              </Col>
            </Row>
          </Card>
        )}

        <Row gutter={16}>
          <Col xs={24} lg={12}>
            <Card title="Doanh thu theo chu kỳ">
              <div style={{ height: 300, marginBottom: 16 }}>
                <ResponsiveContainer width="100%" height="100%">
                  <LineChart data={revenueDetailReport.revenueByPeriod}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="period" />
                    <YAxis tickFormatter={(value) => `${(value / 1000000).toFixed(0)}M`} />
                    <Tooltip formatter={(value: number) => formatCurrency(value)} />
                    <Legend />
                    <Line
                      type="monotone"
                      dataKey="revenue"
                      stroke="#1890ff"
                      name="Doanh thu"
                    />
                    <Line
                      type="monotone"
                      dataKey="paid"
                      stroke="#52c41a"
                      name="Đã thu"
                    />
                  </LineChart>
                </ResponsiveContainer>
              </div>
              <Table
                columns={revenueByPeriodColumns}
                dataSource={revenueDetailReport.revenueByPeriod}
                rowKey="period"
                pagination={false}
                size="small"
              />
            </Card>
          </Col>
          <Col xs={24} lg={12}>
            <Card title="Doanh thu theo khách hàng">
              <div style={{ height: 300, marginBottom: 16 }}>
                <ResponsiveContainer width="100%" height="100%">
                  <BarChart data={revenueDetailReport.revenueByCustomer.slice(0, 10)}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="customerName" angle={-45} textAnchor="end" height={100} />
                    <YAxis tickFormatter={(value) => `${(value / 1000000).toFixed(0)}M`} />
                    <Tooltip formatter={(value: number) => formatCurrency(value)} />
                    <Legend />
                    <Bar dataKey="totalRevenue" fill="#1890ff" name="Doanh thu" />
                  </BarChart>
                </ResponsiveContainer>
              </div>
              <Table
                columns={revenueByCustomerColumns}
                dataSource={revenueDetailReport.revenueByCustomer}
                rowKey="customerId"
                pagination={{ pageSize: 5 }}
                size="small"
              />
            </Card>
          </Col>
        </Row>
      </div>
    );
  };

  // Trip Report Tab
  const TripTab = () => {
    if (!tripDetailReport) return null;

    const tripStatusColumns = [
      { title: 'Trạng thái', dataIndex: 'status', key: 'status' },
      { title: 'Số lượng', dataIndex: 'count', key: 'count' },
      {
        title: 'Tổng khoảng cách',
        dataIndex: 'totalDistance',
        key: 'totalDistance',
        render: (value: number) => `${formatNumber(value)} km`,
      },
      {
        title: 'Doanh thu',
        dataIndex: 'totalRevenue',
        key: 'totalRevenue',
        render: (value: number) => formatCurrency(value),
      },
    ];

    const tripStatusData = tripStatusReport.map((item) => ({
      name: item.status,
      value: item.count,
      color: getStatusColor(item.status),
    }));

    return (
      <div>
        <Row gutter={16} style={{ marginBottom: 24 }}>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Tổng chuyến"
                value={tripDetailReport.summary.totalTrips}
                prefix={<CarOutlined />}
              />
            </Card>
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Hoàn thành"
                value={tripDetailReport.summary.completedTrips}
                valueStyle={{ color: '#52c41a' }}
              />
            </Card>
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Đang thực hiện"
                value={tripDetailReport.summary.inProgressTrips}
                valueStyle={{ color: '#1890ff' }}
              />
            </Card>
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Tổng khoảng cách"
                value={tripDetailReport.summary.totalDistance}
                suffix="km"
                formatter={(value) => formatNumber(Number(value))}
              />
            </Card>
          </Col>
        </Row>

        <Row gutter={16}>
          <Col xs={24} lg={12}>
            <Card title="Trạng thái chuyến hàng">
              <div style={{ height: 300 }}>
                <ResponsiveContainer width="100%" height="100%">
                  <PieChart>
                    <Pie
                      data={tripStatusData}
                      cx="50%"
                      cy="50%"
                      labelLine={false}
                      label={({ name, percent }: any) => `${name}: ${((percent as number) * 100).toFixed(0)}%`}
                      outerRadius={100}
                      fill="#8884d8"
                      dataKey="value"
                    >
                      {tripStatusData.map((entry, index) => (
                        <Cell key={`cell-${index}`} fill={entry.color} />
                      ))}
                    </Pie>
                    <Tooltip />
                  </PieChart>
                </ResponsiveContainer>
              </div>
            </Card>
          </Col>
          <Col xs={24} lg={12}>
            <Card title="Chuyến hàng theo trạng thái">
              <Table
                columns={tripStatusColumns}
                dataSource={tripDetailReport.tripsByStatus}
                rowKey="status"
                pagination={false}
                size="small"
              />
            </Card>
          </Col>
        </Row>
      </div>
    );
  };

  // Debt Report Tab
  const DebtTab = () => {
    const debtColumns = [
      { title: 'Khách hàng', dataIndex: 'customerName', key: 'customerName' },
      {
        title: 'Tổng nợ',
        dataIndex: 'totalDebt',
        key: 'totalDebt',
        render: (value: number) => formatCurrency(value),
        sorter: (a: any, b: any) => a.totalDebt - b.totalDebt,
      },
      {
        title: 'Đã trả',
        dataIndex: 'totalPaid',
        key: 'totalPaid',
        render: (value: number) => formatCurrency(value),
      },
      {
        title: 'Còn lại',
        dataIndex: 'remainingDebt',
        key: 'remainingDebt',
        render: (value: number) => (
          <Text type="danger" strong>
            {formatCurrency(value)}
          </Text>
        ),
        sorter: (a: any, b: any) => a.remainingDebt - b.remainingDebt,
      },
      {
        title: 'Số hóa đơn',
        dataIndex: 'invoiceCount',
        key: 'invoiceCount',
      },
      {
        title: 'Quá hạn',
        dataIndex: 'daysOverdue',
        key: 'daysOverdue',
        render: (value: number) =>
          value > 0 ? <Tag color="error">{value} ngày</Tag> : <Tag color="success">Đúng hạn</Tag>,
        sorter: (a: any, b: any) => a.daysOverdue - b.daysOverdue,
      },
    ];

    return (
      <Card title="Báo cáo công nợ">
        <Table
          columns={debtColumns}
          dataSource={debtReport}
          rowKey="customerId"
          loading={loading}
          pagination={{ pageSize: 10 }}
        />
      </Card>
    );
  };

  // Truck Report Tab
  const TruckTab = () => {
    if (!truckReport) return null;

    const maintenanceColumns = [
      { title: 'Biển số', dataIndex: 'licensePlate', key: 'licensePlate' },
      { title: 'Hãng', dataIndex: 'brand', key: 'brand' },
      { title: 'Model', dataIndex: 'model', key: 'model' },
      {
        title: 'Bảo trì cuối',
        dataIndex: 'lastMaintenanceDate',
        key: 'lastMaintenanceDate',
        render: (date: string) => (date ? dayjs(date).format('DD/MM/YYYY') : '-'),
      },
      {
        title: 'Bảo trì tiếp theo',
        dataIndex: 'nextMaintenanceDate',
        key: 'nextMaintenanceDate',
        render: (date: string) => (date ? dayjs(date).format('DD/MM/YYYY') : '-'),
      },
      {
        title: 'Còn lại',
        dataIndex: 'daysUntilMaintenance',
        key: 'daysUntilMaintenance',
        render: (days: number, record: any) => (
          <Tag color={record.isOverdue ? 'error' : days <= 7 ? 'warning' : 'success'}>
            {days} ngày
          </Tag>
        ),
      },
    ];

    return (
      <div>
        <Row gutter={16} style={{ marginBottom: 24 }}>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Tổng xe"
                value={truckReport.summary.totalTrucks}
                prefix={<CarOutlined />}
              />
            </Card>
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Sẵn sàng"
                value={truckReport.summary.availableTrucks}
                valueStyle={{ color: '#52c41a' }}
              />
            </Card>
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Đang sử dụng"
                value={truckReport.summary.inUseTrucks}
                valueStyle={{ color: '#1890ff' }}
              />
            </Card>
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Cần bảo trì"
                value={truckReport.summary.maintenanceDue}
                valueStyle={{ color: '#ff4d4f' }}
              />
            </Card>
          </Col>
        </Row>

        <Card title="Lịch bảo trì">
          <Table
            columns={maintenanceColumns}
            dataSource={truckReport.maintenanceSchedule}
            rowKey="truckId"
            pagination={{ pageSize: 10 }}
          />
        </Card>
      </div>
    );
  };

  // Driver Report Tab
  const DriverTab = () => {
    if (!driverReport) return null;

    const driverColumns = [
      { title: 'Tên tài xế', dataIndex: 'driverName', key: 'driverName' },
      { title: 'Bằng lái', dataIndex: 'licenseNumber', key: 'licenseNumber' },
      { title: 'Trạng thái', dataIndex: 'status', key: 'status' },
      { title: 'Số chuyến', dataIndex: 'totalTrips', key: 'totalTrips' },
      {
        title: 'Tỷ lệ hoàn thành',
        dataIndex: 'completionRate',
        key: 'completionRate',
        render: (value: number) => `${value.toFixed(1)}%`,
      },
      {
        title: 'Hết hạn bằng',
        dataIndex: 'licenseExpiryDate',
        key: 'licenseExpiryDate',
        render: (date: string, record: any) => {
          if (record.isLicenseExpired) {
            return <Tag color="error">Đã hết hạn</Tag>;
          }
          if (record.isLicenseExpiringSoon) {
            return <Tag color="warning">{record.daysUntilLicenseExpiry} ngày</Tag>;
          }
          return date ? dayjs(date).format('DD/MM/YYYY') : '-';
        },
      },
    ];

    return (
      <div>
        <Row gutter={16} style={{ marginBottom: 24 }}>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Tổng tài xế"
                value={driverReport.summary.totalDrivers}
                prefix={<UserOutlined />}
              />
            </Card>
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Sẵn sàng"
                value={driverReport.summary.availableDrivers}
                valueStyle={{ color: '#52c41a' }}
              />
            </Card>
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Sắp hết hạn"
                value={driverReport.summary.licenseExpiringSoon}
                valueStyle={{ color: '#ff4d4f' }}
              />
            </Card>
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Đã hết hạn"
                value={driverReport.summary.licenseExpired}
                valueStyle={{ color: '#ff4d4f' }}
              />
            </Card>
          </Col>
        </Row>

        <Card title="Hiệu suất tài xế">
          <Table
            columns={driverColumns}
            dataSource={driverReport.driverPerformance}
            rowKey="driverId"
            pagination={{ pageSize: 10 }}
          />
        </Card>
      </div>
    );
  };

  // Customer Report Tab
  const CustomerTab = () => {
    if (!customerReport) return null;

    const customerColumns = [
      { title: 'Khách hàng', dataIndex: 'customerName', key: 'customerName' },
      { title: 'Số chuyến', dataIndex: 'tripCount', key: 'tripCount' },
      {
        title: 'Doanh thu',
        dataIndex: 'totalRevenue',
        key: 'totalRevenue',
        render: (value: number) => formatCurrency(value),
        sorter: (a: any, b: any) => a.totalRevenue - b.totalRevenue,
      },
      {
        title: 'Công nợ',
        dataIndex: 'remainingDebt',
        key: 'remainingDebt',
        render: (value: number) => (
          <Text type={value > 0 ? 'danger' : 'success'}>
            {formatCurrency(value)}
          </Text>
        ),
      },
      {
        title: 'Trạng thái',
        dataIndex: 'isActive',
        key: 'isActive',
        render: (isActive: boolean) => (
          <Tag color={isActive ? 'success' : 'default'}>
            {isActive ? 'Hoạt động' : 'Không hoạt động'}
          </Tag>
        ),
      },
    ];

    return (
      <div>
        <Row gutter={16} style={{ marginBottom: 24 }}>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Tổng khách hàng"
                value={customerReport.summary.totalCustomers}
                prefix={<UserOutlined />}
              />
            </Card>
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Hoạt động"
                value={customerReport.summary.activeCustomers}
                valueStyle={{ color: '#52c41a' }}
              />
            </Card>
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Tổng doanh thu"
                value={customerReport.summary.totalRevenue}
                prefix={<DollarOutlined />}
                formatter={(value) => formatCurrency(Number(value))}
              />
            </Card>
          </Col>
          <Col xs={24} sm={12} lg={6}>
            <Card>
              <Statistic
                title="Tổng công nợ"
                value={customerReport.summary.totalDebt}
                prefix={<DollarOutlined />}
                formatter={(value) => formatCurrency(Number(value))}
                valueStyle={{ color: '#ff4d4f' }}
              />
            </Card>
          </Col>
        </Row>

        <Card title="Chi tiết khách hàng">
          <Table
            columns={customerColumns}
            dataSource={customerReport.customerDetails}
            rowKey="customerId"
            pagination={{ pageSize: 10 }}
          />
        </Card>
      </div>
    );
  };

  const tabItems = [
    {
      key: 'revenue',
      label: 'Doanh thu',
      children: <RevenueTab />,
    },
    {
      key: 'trips',
      label: 'Chuyến hàng',
      children: <TripTab />,
    },
    {
      key: 'debt',
      label: 'Công nợ',
      children: <DebtTab />,
    },
    {
      key: 'trucks',
      label: 'Xe tải',
      children: <TruckTab />,
    },
    {
      key: 'drivers',
      label: 'Tài xế',
      children: <DriverTab />,
    },
    {
      key: 'customers',
      label: 'Khách hàng',
      children: <CustomerTab />,
    },
  ];

  return (
    <div style={{ padding: '24px' }}>
      <Title level={2} style={{ marginBottom: 24 }}>
        Báo Cáo
      </Title>

      {error && (
        <Alert
          message="Lỗi"
          description={error}
          type="error"
          showIcon
          closable
          style={{ marginBottom: 16 }}
        />
      )}

      <FilterPanel />

      <Spin spinning={loading}>
        <Tabs
          activeKey={activeTab}
          onChange={setActiveTab}
          items={tabItems}
          size="large"
        />
      </Spin>
    </div>
  );
};

export default Reports;
