import React from 'react';
import { PostCard } from './PostCard';
import { PostDto } from '../types';

// Tạo dữ liệu giả (Mock Data) để test giao diện
const MOCK_POSTS: PostDto[] = [
  {
    id: '1',
    content: 'Exclusive photos of me yearning 😩',
    visibility: 0, // Public
    // Dùng một đường dẫn ảnh ngẫu nhiên trên mạng để làm mẫu
    mediaUrl: 'https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4?q=80&w=1000&auto=format&fit=crop', 
    createdAt: new Date().toISOString(),
  },
//   {
//     id: '2',
//     content: 'Just finished a great coding session! Building UI with React and Tailwind CSS is so much fun. 🚀💻',
//     visibility: 0,
//     mediaUrl: '', // Bài viết không có ảnh
//     createdAt: new Date().toISOString(),
//   }
];

export const PostFeed = () => {
  return (
    <div className="mt-6 flex flex-col gap-4">
      {/* Lặp qua mảng dữ liệu giả để in ra các thẻ PostCard */}
      {MOCK_POSTS.map((post) => (
        <PostCard key={post.id} post={post} />
      ))}
    </div>
  );
};