import React, { useEffect, useState } from 'react';
import { Table, Button, Modal, Form, Input, InputNumber, DatePicker, Select, message, Space, Popconfirm } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import { trucksApi, Truck, CreateTruck } from '../api/trucks';
import dayjs from 'dayjs';

const Trucks: React.FC = () => {
  const [trucks, setTrucks] = useState<Truck[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalVisible, setModalVisible] = useState(false);
  const [editingTruck, setEditingTruck] = useState<Truck | null>(null);
  const [form] = Form.useForm();

  useEffect(() => {
    fetchTrucks();
  }, []);

  const fetchTrucks = async () => {
    setLoading(true);
    try {
      const data = await trucksApi.getAll();
      setTrucks(data);
    } catch (error) {
      message.error('Lỗi khi tải dữ liệu!');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = () => {
    setEditingTruck(null);
    form.resetFields();
    setModalVisible(true);
  };

  const handleEdit = (truck: Truck) => {
    setEditingTruck(truck);
    form.setFieldsValue({
      ...truck,
      lastMaintenanceDate: truck.lastMaintenanceDate ? dayjs(truck.lastMaintenanceDate) : null,
    });
    setModalVisible(true);
  };

  const handleDelete = async (id: string) => {
    try {
      await trucksApi.delete(id);
      message.success('Xóa thành công!');
      fetchTrucks();
    } catch (error) {
      message.error('Lỗi khi xóa!');
    }
  };

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      const data: CreateTruck = {
        ...values,
        lastMaintenanceDate: values.lastMaintenanceDate ? values.lastMaintenanceDate.toISOString() : undefined,
      };
      
      if (editingTruck) {
        await trucksApi.update(editingTruck.id, data);
        message.success('Cập nhật thành công!');
      } else {
        await trucksApi.create(data);
        message.success('Tạo mới thành công!');
      }
      setModalVisible(false);
      fetchTrucks();
    } catch (error) {
      message.error('Lỗi khi lưu!');
    }
  };

  const columns = [
    { title: 'Biển số', dataIndex: 'licensePlate', key: 'licensePlate' },
    { title: 'Hãng', dataIndex: 'brand', key: 'brand' },
    { title: 'Model', dataIndex: 'model', key: 'model' },
    { title: 'Năm', dataIndex: 'year', key: 'year' },
    { title: 'Tải trọng (tấn)', dataIndex: 'capacity', key: 'capacity' },
    { 
      title: 'Trạng thái', 
      dataIndex: 'status', 
      key: 'status',
      render: (status: string) => {
        const colors: Record<string, string> = {
          Available: 'green',
          InUse: 'blue',
          Maintenance: 'orange',
          Inactive: 'red',
        };
        return <span style={{ color: colors[status] }}>{status}</span>;
      }
    },
    {
      title: 'Hành động',
      key: 'action',
      render: (_: any, record: Truck) => (
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
        <h1>Quản lý xe tải</h1>
        <Button type="primary" icon={<PlusOutlined />} onClick={handleCreate}>
          Thêm mới
        </Button>
      </div>
      <Table
        columns={columns}
        dataSource={trucks}
        loading={loading}
        rowKey="id"
      />
      <Modal
        title={editingTruck ? 'Sửa xe tải' : 'Thêm xe tải'}
        open={modalVisible}
        onOk={handleSubmit}
        onCancel={() => setModalVisible(false)}
        width={600}
      >
        <Form form={form} layout="vertical">
          <Form.Item name="licensePlate" label="Biển số" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="brand" label="Hãng" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="model" label="Model" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="year" label="Năm" rules={[{ required: true }]}>
            <InputNumber style={{ width: '100%' }} min={1900} max={2100} />
          </Form.Item>
          <Form.Item name="capacity" label="Tải trọng (tấn)" rules={[{ required: true }]}>
            <InputNumber style={{ width: '100%' }} min={0} step={0.1} />
          </Form.Item>
          <Form.Item name="status" label="Trạng thái" rules={[{ required: true }]}>
            <Select>
              <Select.Option value="Available">Available</Select.Option>
              <Select.Option value="InUse">InUse</Select.Option>
              <Select.Option value="Maintenance">Maintenance</Select.Option>
              <Select.Option value="Inactive">Inactive</Select.Option>
            </Select>
          </Form.Item>
          <Form.Item name="lastMaintenanceDate" label="Ngày bảo dưỡng cuối">
            <DatePicker style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="maintenanceIntervalDays" label="Chu kỳ bảo dưỡng (ngày)">
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

export default Trucks;

