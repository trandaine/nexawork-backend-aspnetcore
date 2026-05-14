import { useState } from 'react';
import { createPostAPI } from './api';
import { CreatePostRequest } from './types';

export const useCreatePost = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleCreatePost = async (data: CreatePostRequest) => {
    setIsLoading(true);
    setError(null);

  try {
      await createPostAPI(data);
      return true; // Trả về true để báo hiệu form reset dữ liệu
    } catch (err: any) {
      // Xử lý lỗi từ backend
      setError(err.response?.data?.message || 'An error occurred while creating the post!');
      return false; // Trả về false khi thất bại
    } finally {
      setIsLoading(false);
    }
  };

  return { handleCreatePost, isLoading, error };
};                     