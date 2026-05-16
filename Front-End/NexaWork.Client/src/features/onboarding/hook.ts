import { useState } from 'react';
import { setupProfileAPI } from './api';
import { SetupProfileRequest } from './types';

export const useSetupProfile = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSetupProfile = async (data: SetupProfileRequest) => {
    setIsLoading(true);
    setError(null);
    try {
      await setupProfileAPI(data);
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