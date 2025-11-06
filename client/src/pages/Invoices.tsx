import React, { useEffect, useState } from 'react';
import { Table, Button, Modal, Form, Input, InputNumber, DatePicker, Select, message, Space, Popconfirm, Tag } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined, DollarOutlined } from '@ant-design/icons';
import { invoicesApi, Invoice, CreateInvoice } from '../api/invoices';
import { customersApi, Customer } from '../api/customers';
import { tripsApi, Trip } from '../api/trips';
import { paymentsApi, CreatePayment } from '../api/payments';
import dayjs from 'dayjs';

const Invoices: React.FC = () => {
  const [invoices, setInvoices] = useState<Invoice[]>([]);
  const [customers, setCustomers] = useState<Customer[]>([]);
  const [trips, setTrips] = useState<Trip[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalVisible, setModalVisible] = useState(false);
  const [paymentModalVisible, setPaymentModalVisible] = useState(false);
  const [selectedInvoice, setSelectedInvoice] = useState<Invoice | null>(null);
  const [editingInvoice, setEditingInvoice] = useState<Invoice | null>(null);
  const [form] = Form.useForm();
  const [paymentForm] = Form.useForm();

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    setLoading(true);
    try {
      const [invoicesData, customersData, tripsData] = await Promise.all([
        invoicesApi.getAll(),
        customersApi.getAll(),
        tripsApi.getAll(),
      ]);
      setInvoices(invoicesData);
      setCustomers(customersData);
      setTrips(tripsData);
    } catch (error) {
      message.error('Lỗi khi tải dữ liệu!');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = () => {
    setEditingInvoice(null);
    form.resetFields();
    setModalVisible(true);
  };

  const handleEdit = (invoice: Invoice) => {
    setEditingInvoice(invoice);
    form.setFieldsValue({
      ...invoice,
      issueDate: dayjs(invoice.issueDate),
      dueDate: dayjs(invoice.dueDate),
    });
    setModalVisible(true);
  };

  const handleDelete = async (id: string) => {
    try {
      await invoicesApi.delete(id);
      message.success('Xóa thành công!');
      fetchData();
    } catch (error) {
      message.error('Lỗi khi xóa!');
    }
  };

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      const data: CreateInvoice = {
        ...values,
        issueDate: values.issueDate.toISOString(),
        dueDate: values.dueDate.toISOString(),
      };
      
      if (editingInvoice) {
        await invoicesApi.update(editingInvoice.id, data);
        message.success('Cập nhật thành công!');
      } else {
        await invoicesApi.create(data);
        message.success('Tạo mới thành công!');
      }
      setModalVisible(false);
      fetchData();
    } catch (error) {
      message.error('Lỗi khi lưu!');
    }
  };

  const handlePayment = (invoice: Invoice) => {
    setSelectedInvoice(invoice);
    paymentForm.resetFields();
    paymentForm.setFieldsValue({
      invoiceId: invoice.id,
      paymentDate: dayjs(),
    });
    setPaymentModalVisible(true);
  };

  const handlePaymentSubmit = async () => {
    try {
      const values = await paymentForm.validateFields();
      const data: CreatePayment = {
        ...values,
        paymentDate: values.paymentDate.toISOString(),
      };
      await paymentsApi.create(data);
      message.success('Thanh toán thành công!');
      setPaymentModalVisible(false);
      fetchData();
    } catch (error) {
      message.error('Lỗi khi thanh toán!');
    }
  };

  const columns = [
    { title: 'Số hóa đơn', dataIndex: 'invoiceNumber', key: 'invoiceNumber' },
    { title: 'Khách hàng', dataIndex: 'customerName', key: 'customerName' },
    { title: 'Ngày phát hành', dataIndex: 'issueDate', key: 'issueDate', render: (date: string) => dayjs(date).format('DD/MM/YYYY') },
    { title: 'Ngày đến hạn', dataIndex: 'dueDate', key: 'dueDate', render: (date: string) => dayjs(date).format('DD/MM/YYYY') },
    { title: 'Tổng tiền', dataIndex: 'totalAmount', key: 'totalAmount', render: (amount: number) => amount.toLocaleString('vi-VN') + ' đ' },
    { title: 'Đã trả', dataIndex: 'paidAmount', key: 'paidAmount', render: (amount: number) => amount.toLocaleString('vi-VN') + ' đ' },
    { title: 'Còn lại', dataIndex: 'remainingAmount', key: 'remainingAmount', render: (amount: number) => amount.toLocaleString('vi-VN') + ' đ' },
    { 
      title: 'Trạng thái', 
      dataIndex: 'status', 
      key: 'status',
      render: (status: string) => {
        const colors: Record<string, string> = {
          Pending: 'orange',
          Partial: 'blue',
          Paid: 'green',
          Overdue: 'red',
        };
        return <Tag color={colors[status]}>{status}</Tag>;
      }
    },
    {
      title: 'Hành động',
      key: 'action',
      render: (_: any, record: Invoice) => (
        <Space>
          <Button icon={<DollarOutlined />} onClick={() => handlePayment(record)}>Thanh toán</Button>
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
        <h1>Quản lý hóa đơn</h1>
        <Button type="primary" icon={<PlusOutlined />} onClick={handleCreate}>
          Thêm mới
        </Button>
      </div>
      <Table
        columns={columns}
        dataSource={invoices}
        loading={loading}
        rowKey="id"
      />
      <Modal
        title={editingInvoice ? 'Sửa hóa đơn' : 'Thêm hóa đơn'}
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
          <Form.Item name="tripId" label="Chuyến hàng">
            <Select allowClear>
              {trips.map(t => (
                <Select.Option key={t.id} value={t.id}>{t.tripNumber}</Select.Option>
              ))}
            </Select>
          </Form.Item>
          <Form.Item name="issueDate" label="Ngày phát hành" rules={[{ required: true }]}>
            <DatePicker style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="dueDate" label="Ngày đến hạn" rules={[{ required: true }]}>
            <DatePicker style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="amount" label="Số tiền" rules={[{ required: true }]}>
            <InputNumber style={{ width: '100%' }} min={0} />
          </Form.Item>
          <Form.Item name="taxAmount" label="Thuế">
            <InputNumber style={{ width: '100%' }} min={0} />
          </Form.Item>
          <Form.Item name="description" label="Mô tả">
            <Input.TextArea rows={3} />
          </Form.Item>
          <Form.Item name="notes" label="Ghi chú">
            <Input.TextArea rows={3} />
          </Form.Item>
        </Form>
      </Modal>
      <Modal
        title="Thanh toán"
        open={paymentModalVisible}
        onOk={handlePaymentSubmit}
        onCancel={() => setPaymentModalVisible(false)}
        width={500}
      >
        <Form form={paymentForm} layout="vertical">
          <Form.Item label="Hóa đơn">
            <Input value={selectedInvoice?.invoiceNumber} disabled />
          </Form.Item>
          <Form.Item label="Số tiền còn lại">
            <Input value={selectedInvoice?.remainingAmount.toLocaleString('vi-VN') + ' đ'} disabled />
          </Form.Item>
          <Form.Item name="paymentDate" label="Ngày thanh toán" rules={[{ required: true }]}>
            <DatePicker style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="amount" label="Số tiền" rules={[{ required: true }]}>
            <InputNumber style={{ width: '100%' }} min={0} max={selectedInvoice?.remainingAmount} />
          </Form.Item>
          <Form.Item name="paymentMethod" label="Phương thức" rules={[{ required: true }]}>
            <Select>
              <Select.Option value="Cash">Tiền mặt</Select.Option>
              <Select.Option value="BankTransfer">Chuyển khoản</Select.Option>
              <Select.Option value="Check">Séc</Select.Option>
            </Select>
          </Form.Item>
          <Form.Item name="referenceNumber" label="Số tham chiếu">
            <Input />
          </Form.Item>
          <Form.Item name="notes" label="Ghi chú">
            <Input.TextArea rows={3} />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default Invoices;

