// Dữ liệu dùng để gửi đi khi người dùng điền Form (PUT)
// export interface SetupProfileRequest {
//   firstName: string;
//   lastName: string;
// }

export interface CustomerMeResponse {
  customerId: string;
  identityUserId: string;
  firstName: string;
  lastName: string;
}


// Dữ liệu nhận về khi kiểm tra thông tin người dùng (GET)
export interface SetupProfileRequest {
  customerId: string;
  identityUserId: string;
  firstName: string | null;
  lastName: string | null;
  headline: string | null;
  summary: string | null;
  location: string | null;
}