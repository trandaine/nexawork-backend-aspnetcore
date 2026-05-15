import React from 'react';
import { PostDto } from '../types';

interface PostCardProps {
  post: PostDto;
}

export const PostCard: React.FC<PostCardProps> = ({ post }) => {
  // Hàm format thời gian để hiển thị cho đẹp
  const formattedDate = new Date(post.createdAt).toLocaleDateString();

  return (
    <div className="bg-white dark:bg-gray-800 p-4 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 mb-4">
      
      {/* Header: Avatar và Tên (Dùng customerName) */}
      <div className="flex items-center gap-3 mb-3">
        <img
          src="https://i.pravatar.cc/150?img=11" // Tạm thời dùng avatar ảo
          alt="avatar"
          className="w-10 h-10 rounded-full object-cover"
        />
        <div>
          {/* Lấy tên người dùng từ DB */}
          <h3 className="font-semibold text-sm text-gray-900 dark:text-white">{post.customerName}</h3>
          <p className="text-xs text-gray-500 dark:text-gray-400">
            {post.visibility === 0 ? '🌐 Public' : '🔒 Restricted'} • {formattedDate}
          </p>
        </div>
      </div>

      {/* Nội dung bài viết */}
      <p className="text-sm text-gray-700 dark:text-gray-300 mb-3 whitespace-pre-wrap">
        {post.content}
      </p>

      {/* Hiển thị Hình ảnh (nếu bài viết có ảnh) */}
      {post.mediaUrl && (
        <div className="rounded-lg overflow-hidden mb-3 border border-gray-200 dark:border-gray-700 bg-black flex justify-center">
          <img
            src={`https://localhost:7172${post.mediaUrl}`} // Đường dẫn ảnh từ backend
            alt="Post media"
            className="w-full max-h-[500px] object-contain"
          />
        </div>
      )}

      {/* Các nút tương tác (Sử dụng dữ liệu likesCount, commentsCount, sharesCount thật) */}
      <div className="flex items-center justify-between pt-3 border-t border-gray-200 dark:border-gray-700 text-gray-500 dark:text-gray-400 text-sm font-medium">
        <button className="hover:text-blue-600 dark:hover:text-blue-400 flex items-center gap-2 transition-colors">
          👍 {post.likesCount > 0 ? post.likesCount : 'Like'}
        </button>
        <button className="hover:text-blue-600 dark:hover:text-blue-400 flex items-center gap-2 transition-colors">
          💬 {post.commentsCount > 0 ? post.commentsCount : 'Comment'}
        </button>
        <button className="hover:text-blue-600 dark:hover:text-blue-400 flex items-center gap-2 transition-colors">
          🔁 {post.sharesCount > 0 ? post.sharesCount : 'Repost'}
        </button>
        <button className="hover:text-blue-600 dark:hover:text-blue-400 flex items-center gap-2 transition-colors">
          📤 Send
        </button>
      </div>
    </div>
  );
};