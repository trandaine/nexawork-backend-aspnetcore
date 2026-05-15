import { useState, useEffect } from 'react';
import { createPostAPI, getPostsAPI } from './api';
import { CreatePostRequest, PostDto } from './types';

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

export const useGetPosts = () => {
  const [posts, setPosts] = useState<PostDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  const fetchPosts = async () => {
    setIsLoading(true);
    try {
      const data = await getPostsAPI();
      setPosts(data); // Đưa dữ liệu từ .NET vào biến state
    } catch (error) {
      console.error('Lỗi khi tải danh sách bài viết:', error);
    } finally {
      setIsLoading(false);
    }
  };

  // Tự động gọi API lấy bài viết ngay khi component hiển thị lần đầu
  useEffect(() => {
    fetchPosts();
  }, []);

  return { posts, isLoading, refetch: fetchPosts };
};