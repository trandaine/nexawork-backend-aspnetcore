import React from 'react';
import { PostCard } from './PostCard';
import { useGetPosts } from '../hook'; // Import hook

export const PostFeed = () => {
  // Gọi hook để lấy danh sách bài viết thật
  const { posts, isLoading } = useGetPosts();

  if (isLoading) {
    return <div className="text-center text-gray-500 dark:text-gray-400 mt-6 py-4">Loading posts...</div>;
  }

  if (!posts || posts.length === 0) {
    return <div className="text-center text-gray-500 dark:text-gray-400 mt-6 py-4 border border-dashed border-gray-300 dark:border-gray-700 rounded-xl">No posts yet. Be the first to post!</div>;
  }

  return (
    <div className="mt-6 flex flex-col gap-4">
      {/* Duyệt qua danh sách bài viết thật, lưu ý dùng postId thay vì id */}
      {posts.map((post) => (
        <PostCard key={post.postId} post={post} />
      ))}
    </div>
  );
};