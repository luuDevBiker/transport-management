import React, { useEffect, useState } from 'react';
import { Table, Button, Modal, Form, Input, InputNumber, DatePicker, Select, message, Space, Popconfirm } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import { tripsApi, Trip, CreateTrip } from '../api/trips';
import { customersApi, Customer } from '../api/customers';
import { trucksApi, Truck } from '../api/trucks';
import { driversApi, Driver } from '../api/drivers';
import dayjs from 'dayjs';

const Trips: React.FC = () => {
  const [trips, setTrips] = useState<Trip[]>([]);
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [trucks, setTrucks] = useState<Truck[]>([]);
  const [drivers, setDrivers] = useState<Driver[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalVisible, setModalVisible] = useState(false);
  const [editingTrip, setEditingTrip] = useState<Trip | null>(null);
  const [form] = Form.useForm();

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    setLoading(true);
    try {
      const [tripsData, customersData, trucksData, driversData] = await Promise.all([
        tripsApi.getAll(),
        customersApi.getAll(),
        trucksApi.getAll(),
        driversApi.getAll(),
      ]);
      setTrips(tripsData);
      setCustomers(customersData);
      setTrucks(trucksData);
      setDrivers(driversData);
    } catch (error) {
      message.error('Lỗi khi tải dữ liệu!');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = () => {
    setEditingTrip(null);
    form.resetFields();
    setModalVisible(true);
  };

  const handleEdit = (trip: Trip) => {
    setEditingTrip(trip);
    form.setFieldsValue({
      ...trip,
      scheduledDate: dayjs(trip.scheduledDate),
    });
    setModalVisible(true);
  };

  const handleDelete = async (id: string) => {
    try {
      await tripsApi.delete(id);
      message.success('Xóa thành công!');
      fetchData();
    } catch (error) {
      message.error('Lỗi khi xóa!');
    }
  };

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      const data: CreateTrip = {
        ...values,
        scheduledDate: values.scheduledDate.toISOString(),
      };
      
      if (editingTrip) {
        await tripsApi.update(editingTrip.id, data);
        message.success('Cập nhật thành công!');
      } else {
        await tripsApi.create(data);
        message.success('Tạo mới thành công!');
      }
      setModalVisible(false);
      fetchData();
    } catch (error) {
      message.error('Lỗi khi lưu!');
    }
  };

  const columns = [
    { title: 'Mã chuyến', dataIndex: 'tripNumber', key: 'tripNumber' },
    { title: 'Khách hàng', dataIndex: 'customerName', key: 'customerName' },
    { title: 'Xe tải', dataIndex: 'truckLicensePlate', key: 'truckLicensePlate' },
    { title: 'Tài xế', dataIndex: 'driverName', key: 'driverName' },
    { title: 'Điểm đi', dataIndex: 'origin', key: 'origin' },
    { title: 'Điểm đến', dataIndex: 'destination', key: 'destination' },
    { 
      title: 'Ngày dự kiến', 
      dataIndex: 'scheduledDate', 
      key: 'scheduledDate',
      render: (date: string) => dayjs(date).format('DD/MM/YYYY')
    },
    { title: 'Khoảng cách (km)', dataIndex: 'distance', key: 'distance' },
    { 
      title: 'Trạng thái', 
      dataIndex: 'status', 
      key: 'status',
      render: (status: string) => {
        const colors: Record<string, string> = {
          Scheduled: 'blue',
          InProgress: 'orange',
          Completed: 'green',
          Cancelled: 'red',
        };
        return <span style={{ color: colors[status] }}>{status}</span>;
      }
    },
    {
      title: 'Hành động',
      key: 'action',
      render: (_: any, record: Trip) => (
        <Space>
          <Button icon={<EditOutlined />} onClick={() => handleEdit(record)} />
          <Popconfirm
            title="Bạn có chắc muốn xóa?"
            onConfirm={() => handleDelete(record.id)}
          >
            <Button danger icon={<DeleteOutlined />} />
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <div>
      <div style={{ marginBottom: 16, display: 'flex', justifyContent: 'space-between' }}>
        <h1>Quản lý chuyến hàng</h1>
        <Button type="primary" icon={<PlusOutlined />} onClick={handleCreate}>
          Thêm mới
        </Button>
      </div>
      <Table
        columns={columns}
        dataSource={trips}
        loading={loading}
        rowKey="id"
      />
      <Modal
        title={editingTrip ? 'Sửa chuyến hàng' : 'Thêm chuyến hàng'}
        open={modalVisible}
        onOk={handleSubmit}
        onCancel={() => setModalVisible(false)}
        width={600}
      >
        <Form form={form} layout="vertical">
          <Form.Item name="customerId" label="Khách hàng" rules={[{ required: true }]}>
            <Select>
              {customers.map(c => (
                <Select.Option key={c.id} value={c.id}>{c.name}</Select.Option>
              ))}
            </Select>
          </Form.Item>
          <Form.Item name="truckId" label="Xe tải" rules={[{ required: true }]}>
            <Select>
              {trucks.map(t => (
                <Select.Option key={t.id} value={t.id}>{t.licensePlate}</Select.Option>
              ))}
            </Select>
          </Form.Item>
          <Form.Item name="driverId" label="Tài xế" rules={[{ required: true }]}>
            <Select>
              {drivers.map(d => (
                <Select.Option key={d.id} value={d.id}>{d.fullName}</Select.Option>
              ))}
            </Select>
          </Form.Item>
          <Form.Item name="origin" label="Điểm đi" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="destination" label="Điểm đến" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="scheduledDate" label="Ngày dự kiến" rules={[{ required: true }]}>
            <DatePicker style={{ width: '100%' }} showTime />
          </Form.Item>
          <Form.Item name="distance" label="Khoảng cách (km)" rules={[{ required: true }]}>
            <InputNumber style={{ width: '100%' }} min={0} />
          </Form.Item>
          <Form.Item name="notes" label="Ghi chú">
            <Input.TextArea rows={3} />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default Trips;

