// features/user-profile/api.ts

// 1. Dùng lại axiosInstance giống y hệt phần Post và Onboarding
import { axiosInstance } from '../../shared/api/axiosInstance'; 
import { UserProfile } from './types';

/**
 * Fetches the user profile information from the API.
 * @returns A promise resolving to the user profile data.
 */
export const getUserProfileApi = async (): Promise<UserProfile> => {
  // axiosInstance đã tự động nhét token vào header rồi
  const response = await axiosInstance.get('/user/profile');
  
  // Axios tự động parse JSON, bạn không cần phải response.json() hay check response.ok nữa!
  return response.data;
};


// import { apiClient } from "../../shared/api/apiClient";
// import { UserProfile } from "./types";


// /**
//  * Fetches the user profile information from the API.
//  * @returns A promise resolving to the user profile data.
//  */
// export const getUserProfileApi = async (): Promise<UserProfile> => {
//   // Gọi qua apiClient, KHÔNG CẦN quan tâm đến việc nhét token nữa!
//   const response = await apiClient('/user/profile', {
//     method: 'GET',
//   });

//   if (!response.ok) {
//     throw new Error('Cannot get user information');
//   }

//   const data = await response.json();
//   return data;
// };