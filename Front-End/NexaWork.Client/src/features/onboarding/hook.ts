import { useState } from 'react';
import { getCustomerMeAPI, setupProfileAPI } from './api';
import { SetupProfileRequest } from './types';

export const useSetupProfile = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSetupProfile = async (identityId: string, firstName: string, lastName: string) => {
    setIsLoading(true);
    setError(null);
    try {
      
      const existingCustomer = await getCustomerMeAPI(identityId);
      const realCustomerId = existingCustomer?.customerId;

      const payload: SetupProfileRequest = {
        identityUserId: identityId,
        customerId: realCustomerId,
        firstName: firstName.trim(),
        lastName: lastName.trim(),
        // Pad the missing fields with empty strings instead of leaving them undefined
        headline: "",
        summary: "",
        location: ""
      }

      await setupProfileAPI(identityId, payload);
      return true; // Báo hiệu đã lưu thành công
    } catch (err: any) {
      setError(err.response?.data?.message || 'Có lỗi xảy ra khi lưu thông tin!');
      return false; // Báo hiệu lưu thất bại
    } finally {
      setIsLoading(false);
    }
  };

  return { handleSetupProfile, isLoading, error };
};
  