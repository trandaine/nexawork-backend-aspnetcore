import React, { useState } from 'react';
import { useCreatePost } from '../hook';
import { PostVisibility } from '../types';

export const CreatePostForm = () => {
  // 1. Manage form input states
  const [content, setContent] = useState('');
  const [visibility, setVisibility] = useState<PostVisibility>(PostVisibility.Public);
  const [mediaFile, setMediaFile] = useState<File | null>(null);

  // 2. Call the custom hook
  const { handleCreatePost, isLoading, error } = useCreatePost();

  // 3. Handle file selection
  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      setMediaFile(e.target.files[0]);
    }
  };

  // 4. Handle form submission
  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault(); 
   
    if (!content.trim()) {
      alert('Please enter your post content!');
      return;
    }

    const result = await handleCreatePost({
      content,
      visibility,
      mediaFile,
    });

    if (result) {
      // Reset form on success
      setContent('');
      setMediaFile(null);
      setVisibility(PostVisibility.Public);
      alert('Post created successfully!');
    }
  };

  return (
    // THÊM dark:bg-gray-800 và dark:border-gray-700 cho nền ngoài cùng
    <div className="bg-white dark:bg-gray-800 p-4 rounded-xl shadow-sm border border-gray-200 dark:border-gray-700 mb-6">
      <form onSubmit={onSubmit}>
        {/* Content Input */}
        <textarea
          // THÊM dark:bg-gray-700, dark:border-gray-600, và dark:text-white cho ô nhập chữ
          className="w-full p-3 border border-gray-300 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none"
          rows={3}
          placeholder="What do you want to talk about?"
          value={content}
          onChange={(e) => setContent(e.target.value)}
          disabled={isLoading}
        />

        {/* Media attachment and Visibility settings */}
        <div className="flex items-center justify-between mt-3">
          <div className="flex items-center gap-4">
            {/* File upload */}
            {/* THÊM dark:text-gray-300 cho chữ */}
            <label className="cursor-pointer text-gray-500 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 flex items-center gap-1 text-sm">
              <span>📷 Attach file</span>
              <input
                type="file"
                className="hidden"
                onChange={handleFileChange}
                disabled={isLoading}
              />
            </label>
            {mediaFile && <span className="text-xs text-green-600 dark:text-green-400 truncate max-w-[100px]">{mediaFile.name}</span>}

            {/* Visibility Select */}
            {/* THÊM màu tối cho nút Select */}
            <select
              className="text-sm border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 dark:text-white rounded-md focus:ring-blue-500 p-1"
              value={visibility}
              onChange={(e) => setVisibility(Number(e.target.value))}
              disabled={isLoading}
            >
              <option value={PostVisibility.Public}>Anyone</option>
              <option value={PostVisibility.Connections}>Connections only</option>
              <option value={PostVisibility.Private}>Only me</option>
            </select>
          </div>

          {/* Submit Button */}
          <button
            type="submit"
            disabled={isLoading || !content.trim()}
            className={`px-4 py-2 rounded-lg text-white font-medium text-sm transition-colors ${
              isLoading || !content.trim() 
                ? 'bg-blue-300 dark:bg-blue-800 cursor-not-allowed' 
                : 'bg-blue-600 hover:bg-blue-700 dark:bg-blue-500 dark:hover:bg-blue-600'
            }`}
          >
            {isLoading ? 'Posting...' : 'Post'}
          </button>
        </div>

        {/* Error Message */}
        {error && <p className="text-red-500 dark:text-red-400 text-xs mt-2">{error}</p>}
      </form>
    </div>
  );
};