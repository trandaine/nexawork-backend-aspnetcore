import React, { useState } from 'react';
import { useSetupProfile } from '../hook';
import { useNavigate } from 'react-router-dom'; 

export const OnboardingForm = () => {
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  
  const { handleSetupProfile, isLoading, error } = useSetupProfile();
  const navigate = useNavigate();

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!firstName.trim() || !lastName.trim()) {
      alert("Please fill in both First Name and Last Name!");
      return;
    }

    const success = await handleSetupProfile({ firstName, lastName });
    if (success) {
      navigate('/home'); // Lưu thành công, chuyển hướng vào trang chủ
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 dark:bg-boxdark">
      <div className="max-w-md w-full bg-white dark:bg-meta-4 rounded-xl shadow-lg p-8">
        <h2 className="text-2xl font-bold text-gray-900 dark:text-white mb-2 text-center">
          Welcome to NexaWork!
        </h2>
        <p className="text-sm text-gray-500 dark:text-gray-400 text-center mb-6">
          Please provide your name to complete your profile setup.
        </p>

        {error && <div className="mb-4 text-red-500 text-sm text-center">{error}</div>}

        <form onSubmit={onSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              First Name
            </label>
            <input
              type="text"
              value={firstName}
              onChange={(e) => setFirstName(e.target.value)}
              className="w-full rounded border border-stroke bg-transparent py-3 px-4 outline-none focus:border-primary dark:border-form-strokedark dark:bg-form-input"
              placeholder="e.g. John"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              Last Name
            </label>
            <input
              type="text"
              value={lastName}
              onChange={(e) => setLastName(e.target.value)}
              className="w-full rounded border border-stroke bg-transparent py-3 px-4 outline-none focus:border-primary dark:border-form-strokedark dark:bg-form-input"
              placeholder="e.g. Doe"
            />
          </div>

          <button
            type="submit"
            disabled={isLoading}
            className="w-full flex justify-center py-3 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 disabled:opacity-50"
          >
            {isLoading ? 'Saving...' : 'Complete Setup'}
          </button>
        </form>
      </div>
    </div>
  );
};