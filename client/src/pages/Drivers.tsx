import React, { useEffect, useState } from 'react';
import { Table, Button, Modal, Form, Input, DatePicker, Select, message, Space, Popconfirm } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import { driversApi, Driver, CreateDriver } from '../api/drivers';
import dayjs from 'dayjs';

const Drivers: React.FC = () => {
  const [drivers, setDrivers] = useState<Driver[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalVisible, setModalVisible] = useState(false);
  const [editingDriver, setEditingDriver] = useState<Driver | null>(null);
  const [form] = Form.useForm();

  useEffect(() => {
    fetchDrivers();
  }, []);

  const fetchDrivers = async () => {
    setLoading(true);
    try {
      const data = await driversApi.getAll();
      setDrivers(data);
    } catch (error) {
      message.error('Lỗi khi tải dữ liệu!');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = () => {
    setEditingDriver(null);
    form.resetFields();
    setModalVisible(true);
  };

  const handleEdit = (driver: Driver) => {
    setEditingDriver(driver);
    form.setFieldsValue({
      ...driver,
      licenseExpiryDate: dayjs(driver.licenseExpiryDate),
    });
    setModalVisible(true);
  };

  const handleDelete = async (id: string) => {
    try {
      await driversApi.delete(id);
      message.success('Xóa thành công!');
      fetchDrivers();
    } catch (error) {
      message.error('Lỗi khi xóa!');
    }
  };

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      const data: CreateDriver = {
        ...values,
        licenseExpiryDate: values.licenseExpiryDate.toISOString(),
      };
      
      if (editingDriver) {
        await driversApi.update(editingDriver.id, data);
        message.success('Cập nhật thành công!');
      } else {
        await driversApi.create(data);
        message.success('Tạo mới thành công!');
      }
      setModalVisible(false);
      fetchDrivers();
    } catch (error) {
      message.error('Lỗi khi lưu!');
    }
  };

  const columns = [
    { title: 'Họ tên', dataIndex: 'fullName', key: 'fullName' },
    { title: 'SĐT', dataIndex: 'phone', key: 'phone' },
    { title: 'Email', dataIndex: 'email', key: 'email' },
    { title: 'Số bằng lái', dataIndex: 'licenseNumber', key: 'licenseNumber' },
    { 
      title: 'Ngày hết hạn', 
      dataIndex: 'licenseExpiryDate', 
      key: 'licenseExpiryDate',
      render: (date: string) => dayjs(date).format('DD/MM/YYYY')
    },
    { 
      title: 'Trạng thái', 
      dataIndex: 'status', 
      key: 'status',
      render: (status: string) => {
        const colors: Record<string, string> = {
          Available: 'green',
          OnTrip: 'blue',
          OffDuty: 'orange',
          Inactive: 'red',
        };
        return <span style={{ color: colors[status] }}>{status}</span>;
      }
    },
    {
      title: 'Hành động',
      key: 'action',
      render: (_: any, record: Driver) => (
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
        <h1>Quản lý tài xế</h1>
        <Button type="primary" icon={<PlusOutlined />} onClick={handleCreate}>
          Thêm mới
        </Button>
      </div>
      <Table
        columns={columns}
        dataSource={drivers}
        loading={loading}
        rowKey="id"
      />
      <Modal
        title={editingDriver ? 'Sửa tài xế' : 'Thêm tài xế'}
        open={modalVisible}
        onOk={handleSubmit}
        onCancel={() => setModalVisible(false)}
        width={600}
      >
        <Form form={form} layout="vertical">
          <Form.Item name="fullName" label="Họ tên" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="phone" label="SĐT" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="email" label="Email">
            <Input type="email" />
          </Form.Item>
          <Form.Item name="licenseNumber" label="Số bằng lái" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="licenseExpiryDate" label="Ngày hết hạn" rules={[{ required: true }]}>
            <DatePicker style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="address" label="Địa chỉ">
            <Input.TextArea rows={2} />
          </Form.Item>
          <Form.Item name="status" label="Trạng thái" rules={[{ required: true }]}>
            <Select>
              <Select.Option value="Available">Available</Select.Option>
              <Select.Option value="OnTrip">OnTrip</Select.Option>
              <Select.Option value="OffDuty">OffDuty</Select.Option>
              <Select.Option value="Inactive">Inactive</Select.Option>
            </Select>
          </Form.Item>
          <Form.Item name="notes" label="Ghi chú">
            <Input.TextArea rows={3} />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default Drivers;

