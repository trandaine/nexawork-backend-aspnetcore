import { apiClient } from "../../shared/api/apiClient";
import { UserProfile } from "./types";


/**
 * Fetches the user profile information from the API.
 * @returns A promise resolving to the user profile data.
 */
export const getUserProfileApi = async (): Promise<UserProfile> => {
  // Gọi qua apiClient, KHÔNG CẦN quan tâm đến việc nhét token nữa!
  const response = await apiClient('/user/profile', {
    method: 'GET',
  });

  if (!response.ok) {
    throw new Error('Cannot get user information');
  }

  const data = await response.json();
  return data;
};