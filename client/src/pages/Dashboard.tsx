import React, { useEffect, useState } from 'react';
import {
  Card,
  Row,
  Col,
  Statistic,
  Table,
  Tag,
  Typography,
  Spin,
  Alert,
  Space,
  Progress,
} from 'antd';
import {
  DollarOutlined,
  CarOutlined,
  UserOutlined,
  TruckOutlined,
  FileTextOutlined,
  WarningOutlined,
  ArrowUpOutlined,
  ArrowDownOutlined,
  CheckCircleOutlined,
  ClockCircleOutlined,
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
import { reportsApi, Dashboard as DashboardData } from '../api/reports';
import dayjs from 'dayjs';

const { Title, Text } = Typography;

const Dashboard: React.FC = () => {
  const [dashboard, setDashboard] = useState<DashboardData | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchDashboard();
    // Refresh every 5 minutes
    const interval = setInterval(fetchDashboard, 5 * 60 * 1000);
    return () => clearInterval(interval);
  }, []);

  const fetchDashboard = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await reportsApi.getDashboard();
      setDashboard(data);
    } catch (err: any) {
      setError(err.message || 'Không thể tải dữ liệu dashboard');
      console.error('Error fetching dashboard:', err);
    } finally {
      setLoading(false);
    }
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
      Available: 'success',
      InUse: 'processing',
      Maintenance: 'warning',
      Inactive: 'default',
      Paid: 'success',
      Partial: 'warning',
      Pending: 'default',
      Overdue: 'error',
    };
    return colors[status] || 'default';
  };

  if (loading && !dashboard) {
    return (
      <div style={{ textAlign: 'center', padding: '50px' }}>
        <Spin size="large" />
        <div style={{ marginTop: 16 }}>Đang tải dashboard...</div>
      </div>
    );
  }

  if (error) {
    return (
      <Alert
        message="Lỗi"
        description={error}
        type="error"
        showIcon
        action={
          <button onClick={fetchDashboard} style={{ padding: '4px 8px' }}>
            Thử lại
          </button>
        }
      />
    );
  }

  if (!dashboard) return null;

  const { summary, revenue, trips, debt, recentTrips, topCustomers, truckStatus } = dashboard;

  // Revenue chart data
  const revenueData = [
    { name: 'Hôm nay', value: revenue.todayRevenue },
    { name: 'Tuần này', value: revenue.thisWeekRevenue },
    { name: 'Tháng này', value: revenue.thisMonthRevenue },
    { name: 'Năm nay', value: revenue.thisYearRevenue },
  ];

  // Trip status pie chart data
  const tripStatusData = [
    { name: 'Hoàn thành', value: trips.completedTrips, color: '#52c41a' },
    { name: 'Đang thực hiện', value: trips.inProgressTrips, color: '#1890ff' },
    { name: 'Đã lên lịch', value: trips.scheduledTrips, color: '#faad14' },
  ];

  // Truck status data
  const truckStatusData = truckStatus.map((item) => ({
    name: item.status,
    value: item.count,
  }));

  // Recent trips columns
  const recentTripsColumns = [
    {
      title: 'Mã chuyến',
      dataIndex: 'tripNumber',
      key: 'tripNumber',
      width: 150,
    },
    {
      title: 'Khách hàng',
      dataIndex: 'customerName',
      key: 'customerName',
    },
    {
      title: 'Tuyến đường',
      key: 'route',
      render: (_: any, record: any) => (
        <Text>
          {record.origin} → {record.destination}
        </Text>
      ),
    },
    {
      title: 'Khoảng cách',
      dataIndex: 'distance',
      key: 'distance',
      render: (distance: number) => (distance ? `${formatNumber(distance)} km` : '-'),
    },
    {
      title: 'Ngày',
      dataIndex: 'scheduledDate',
      key: 'scheduledDate',
      render: (date: string) => dayjs(date).format('DD/MM/YYYY'),
    },
    {
      title: 'Trạng thái',
      dataIndex: 'status',
      key: 'status',
      render: (status: string) => (
        <Tag color={getStatusColor(status)}>{status}</Tag>
      ),
    },
  ];

  // Top customers columns
  const topCustomersColumns = [
    {
      title: 'Khách hàng',
      dataIndex: 'customerName',
      key: 'customerName',
    },
    {
      title: 'Doanh thu',
      dataIndex: 'totalRevenue',
      key: 'totalRevenue',
      render: (value: number) => formatCurrency(value),
      sorter: (a: any, b: any) => a.totalRevenue - b.totalRevenue,
    },
    {
      title: 'Số chuyến',
      dataIndex: 'tripCount',
      key: 'tripCount',
      sorter: (a: any, b: any) => a.tripCount - b.tripCount,
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
      sorter: (a: any, b: any) => a.remainingDebt - b.remainingDebt,
    },
  ];

  return (
    <div style={{ padding: '24px' }}>
      <Title level={2} style={{ marginBottom: 24 }}>
        Dashboard Tổng Quan
      </Title>

      {/* Summary Cards */}
      <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Tổng Khách Hàng"
              value={summary.totalCustomers}
              prefix={<UserOutlined />}
              valueStyle={{ color: '#1890ff' }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Tổng Xe Tải"
              value={summary.totalTrucks}
              prefix={<TruckOutlined />}
              valueStyle={{ color: '#52c41a' }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Tổng Tài Xế"
              value={summary.totalDrivers}
              prefix={<UserOutlined />}
              valueStyle={{ color: '#722ed1' }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Chuyến Đang Thực Hiện"
              value={summary.activeTrips}
              prefix={<CarOutlined />}
              valueStyle={{ color: '#fa8c16' }}
            />
          </Card>
        </Col>
      </Row>

      {/* Revenue Section */}
      <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
        <Col xs={24} lg={16}>
          <Card title="Doanh Thu" extra={<DollarOutlined />}>
            <Row gutter={16}>
              <Col span={12}>
                <Statistic
                  title="Hôm nay"
                  value={revenue.todayRevenue}
                  prefix={<DollarOutlined />}
                  formatter={(value) => formatCurrency(Number(value))}
                />
              </Col>
              <Col span={12}>
                <Statistic
                  title="Tuần này"
                  value={revenue.thisWeekRevenue}
                  prefix={<DollarOutlined />}
                  formatter={(value) => formatCurrency(Number(value))}
                />
              </Col>
              <Col span={12} style={{ marginTop: 16 }}>
                <Statistic
                  title="Tháng này"
                  value={revenue.thisMonthRevenue}
                  prefix={<DollarOutlined />}
                  formatter={(value) => formatCurrency(Number(value))}
                />
                <div style={{ marginTop: 8 }}>
                  {revenue.revenueGrowth > 0 ? (
                    <Space>
                      <ArrowUpOutlined style={{ color: '#52c41a' }} />
                      <Text type="success">
                        +{revenue.revenueGrowth.toFixed(1)}% so với tháng trước
                      </Text>
                    </Space>
                  ) : revenue.revenueGrowth < 0 ? (
                    <Space>
                      <ArrowDownOutlined style={{ color: '#ff4d4f' }} />
                      <Text type="danger">
                        {revenue.revenueGrowth.toFixed(1)}% so với tháng trước
                      </Text>
                    </Space>
                  ) : (
                    <Text>Không thay đổi</Text>
                  )}
                </div>
              </Col>
              <Col span={12} style={{ marginTop: 16 }}>
                <Statistic
                  title="Năm nay"
                  value={revenue.thisYearRevenue}
                  prefix={<DollarOutlined />}
                  formatter={(value) => formatCurrency(Number(value))}
                />
              </Col>
            </Row>
            <div style={{ marginTop: 24, height: 250 }}>
              <ResponsiveContainer width="100%" height="100%">
                <BarChart data={revenueData}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis dataKey="name" />
                  <YAxis tickFormatter={(value) => `${(value / 1000000).toFixed(0)}M`} />
                  <Tooltip formatter={(value: number) => formatCurrency(value)} />
                  <Legend />
                  <Bar dataKey="value" fill="#1890ff" name="Doanh thu" />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </Card>
        </Col>
        <Col xs={24} lg={8}>
          <Card title="Trạng Thái Chuyến Hàng">
            <div style={{ marginBottom: 16 }}>
              <Space direction="vertical" style={{ width: '100%' }}>
                <div>
                  <Text>Hoàn thành: </Text>
                  <Text strong>{trips.completedTrips}</Text>
                </div>
                <div>
                  <Text>Đang thực hiện: </Text>
                  <Text strong>{trips.inProgressTrips}</Text>
                </div>
                <div>
                  <Text>Đã lên lịch: </Text>
                  <Text strong>{trips.scheduledTrips}</Text>
                </div>
                <div style={{ marginTop: 8 }}>
                  <Text type="secondary">
                    Tổng khoảng cách: {formatNumber(trips.totalDistance)} km
                  </Text>
                </div>
                <div>
                  <Text type="secondary">
                    Trung bình: {formatNumber(trips.averageDistance)} km/chuyến
                  </Text>
                </div>
              </Space>
            </div>
            <div style={{ height: 200 }}>
              <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                  <Pie
                    data={tripStatusData}
                    cx="50%"
                    cy="50%"
                    labelLine={false}
                    label={({ name, percent }: any) => `${name}: ${((percent as number) * 100).toFixed(0)}%`}
                    outerRadius={80}
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
      </Row>

      {/* Debt & Invoices Section */}
      <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
        <Col xs={24} lg={12}>
          <Card
            title="Công Nợ"
            extra={
              <Tag color={debt.overdueDebt > 0 ? 'error' : 'success'}>
                {debt.overdueInvoices > 0 && (
                  <WarningOutlined style={{ marginRight: 4 }} />
                )}
                {debt.overdueInvoices} hóa đơn quá hạn
              </Tag>
            }
          >
            <Row gutter={16}>
              <Col span={12}>
                <Statistic
                  title="Tổng công nợ"
                  value={debt.totalDebt}
                  prefix={<DollarOutlined />}
                  formatter={(value) => formatCurrency(Number(value))}
                  valueStyle={{ color: debt.totalDebt > 0 ? '#ff4d4f' : '#52c41a' }}
                />
              </Col>
              <Col span={12}>
                <Statistic
                  title="Công nợ quá hạn"
                  value={debt.overdueDebt}
                  prefix={<WarningOutlined />}
                  formatter={(value) => formatCurrency(Number(value))}
                  valueStyle={{ color: '#ff4d4f' }}
                />
              </Col>
              <Col span={24} style={{ marginTop: 16 }}>
                <Space direction="vertical" style={{ width: '100%' }}>
                  <div>
                    <Text>Hóa đơn chưa thanh toán: </Text>
                    <Text strong>{debt.pendingInvoices}</Text>
                  </div>
                  <div>
                    <Text>Hóa đơn đã thanh toán: </Text>
                    <Text strong type="success">
                      {debt.paidInvoices}
                    </Text>
                  </div>
                  {debt.totalDebt > 0 && (
                    <Progress
                      percent={((debt.totalDebt - debt.overdueDebt) / debt.totalDebt) * 100}
                      status={debt.overdueDebt > 0 ? 'exception' : 'active'}
                      format={(percent) => `${percent?.toFixed(1)}% đúng hạn`}
                    />
                  )}
                </Space>
              </Col>
            </Row>
          </Card>
        </Col>
        <Col xs={24} lg={12}>
          <Card title="Trạng Thái Xe Tải">
            <Row gutter={16}>
              {truckStatus.map((item) => (
                <Col span={12} key={item.status} style={{ marginBottom: 16 }}>
                  <Card size="small">
                    <Statistic
                      title={item.status}
                      value={item.count}
                      prefix={<TruckOutlined />}
                    />
                    {item.maintenanceDue > 0 && (
                      <Tag color="warning" style={{ marginTop: 8 }}>
                        {item.maintenanceDue} cần bảo trì
                      </Tag>
                    )}
                  </Card>
                </Col>
              ))}
            </Row>
          </Card>
        </Col>
      </Row>

      {/* Recent Trips & Top Customers */}
      <Row gutter={[16, 16]}>
        <Col xs={24} lg={14}>
          <Card title="Chuyến Hàng Gần Đây" extra={<CarOutlined />}>
            <Table
              columns={recentTripsColumns}
              dataSource={recentTrips}
              rowKey="id"
              pagination={{ pageSize: 5 }}
              size="small"
            />
          </Card>
        </Col>
        <Col xs={24} lg={10}>
          <Card title="Top Khách Hàng" extra={<UserOutlined />}>
            <Table
              columns={topCustomersColumns}
              dataSource={topCustomers}
              rowKey="customerId"
              pagination={false}
              size="small"
            />
          </Card>
        </Col>
      </Row>
    </div>
  );
};

export default Dashboard;
