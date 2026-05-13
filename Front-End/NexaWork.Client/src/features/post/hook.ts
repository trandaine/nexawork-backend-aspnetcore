import { useState } from 'react';
import { createPostAPI } from './api';
import { CreatePostRequest } from './types';

export const useCreatePost = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [isSuccess, setIsSuccess] = useState(false);

  const handleCreatePost = async (data: CreatePostRequest) => {
    setIsLoading(true);
    setError(null);
    setIsSuccess(false);
    
    try {
      const response = await createPostAPI(data);
      setIsSuccess(true);
      return response; 
    } catch (err: any) {
      // Xử lý lỗi từ backend trả về
      setError(err.response?.data?.message || 'Đã có lỗi xảy ra khi tạo bài viết!');
    } finally {
      setIsLoading(false);
    }
  };

  return { handleCreatePost, isLoading, error, isSuccess };
};