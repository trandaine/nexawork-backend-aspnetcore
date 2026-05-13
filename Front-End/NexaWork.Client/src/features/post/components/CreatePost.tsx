import React, { useState } from 'react';
import { useCreatePost } from '../hook';
import { PostVisibility } from '../types';

export const CreatePostForm = () => {
  // 1. Manage form input states
  const [content, setContent] = useState('');
  const [visibility, setVisibility] = useState<PostVisibility>(PostVisibility.Public);
  const [mediaFile, setMediaFile] = useState<File | null>(null);

  // 2. Call the custom hook
  const { handleCreatePost, isLoading, error, isSuccess } = useCreatePost();

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
    <div className="bg-white p-4 rounded-xl shadow-sm border border-gray-200 mb-6">
      <form onSubmit={onSubmit}>
        {/* Content Input */}
        <textarea
          className="w-full p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none"
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
            <label className="cursor-pointer text-gray-500 hover:text-blue-600 flex items-center gap-1 text-sm">
              <span>📷 Attach file</span>
              <input
                type="file"
                className="hidden"
                onChange={handleFileChange}
                disabled={isLoading}
              />
            </label>
            {mediaFile && <span className="text-xs text-green-600 truncate max-w-[100px]">{mediaFile.name}</span>}

            {/* Visibility Select */}
            <select
              className="text-sm border-gray-300 rounded-md focus:ring-blue-500 p-1"
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
              isLoading || !content.trim() ? 'bg-blue-300 cursor-not-allowed' : 'bg-blue-600 hover:bg-blue-700'
            }`}
          >
            {isLoading ? 'Posting...' : 'Post'}
          </button>
        </div>

        {/* Error Message */}
        {error && <p className="text-red-500 text-xs mt-2">{error}</p>}
      </form>
    </div>
  );
};