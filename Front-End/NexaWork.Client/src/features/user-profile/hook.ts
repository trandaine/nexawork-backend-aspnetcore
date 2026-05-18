// import { useState, useEffect } from 'react';
// import { UserProfile } from './types';

// export const useUserProfileData = () => {
//   // Bơm thẳng dữ liệu giả vào đây để test giao diện
//   return { 
//     profileData: {
//        id: "123",
//        username: "Tài Đẹp Trai", 
//        email: "taideptrai@gmail.com"
//     } as UserProfile, 
//     isLoading: false, 
//     error: null 
//   };
// };



import { useState, useEffect } from 'react';
import { getUserProfileApi } from './api';
import { UserProfile } from './types';

export const useUserProfileData = () => {
  const [profileData, setProfileData] = useState<UserProfile | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    // Hàm gọi API
    const fetchProfile = async () => {
      setIsLoading(true);
      setError(null);
      try {
        const data = await getUserProfileApi();
        setProfileData(data);
      } catch (err: any) {
        console.error("Lỗi khi tải thông tin user:", err);
        setError(err.message || "Failed to load profile data");
      } finally {
        setIsLoading(false);
      }
    };

    // Chạy hàm này 1 lần duy nhất khi component được render
    fetchProfile();
  }, []);

  // Trả về dữ liệu để Giao diện sử dụng
  return { profileData, isLoading, error };
};