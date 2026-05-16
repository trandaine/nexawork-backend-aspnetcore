import { axiosInstance } from '../../shared/api/axiosInstance';
import { SetupProfileRequest, CustomerMeResponse } from './types';

// API: Lấy thông tin cơ bản của user hiện tại
export const getCustomerMeAPI = async (identityId: string): Promise<CustomerMeResponse> => {
  const response = await axiosInstance.get(`/Customers/me/${identityId}`);
  return response.data;
};

// API: Cập nhật FirstName và LastName
export const setupProfileAPI = async (id: string, data: SetupProfileRequest) => {
  const response = await axiosInstance.post(`/Customers/update-customer/${id}`, data);
  return response.data;
};