// import { Navigate, Outlet } from 'react-router-dom';

// const ProtectedRoute = () => {
//   // Lấy token từ localStorage
//   const token = localStorage.getItem('userToken');

//   // Kiểm tra: Nếu không có token, hoặc token mang chuỗi 'undefined' do lỗi cũ
//   if (!token || token === 'undefined') {
//     // Chuyển hướng về trang đăng nhập, dùng replace để người dùng 
//     // không thể bấm nút Back trên trình duyệt để quay lại trang được bảo vệ
//     console.log('No valid token found. Redirecting to login page.');
//     return <Navigate to="/signin" replace />;
//   }

//   // Nếu có token, cho phép render các component con (ví dụ: Dashboard)
//   console.log('Valid token found. Access granted to protected route.');
//   return <Outlet />;
// };

// export default ProtectedRoute;