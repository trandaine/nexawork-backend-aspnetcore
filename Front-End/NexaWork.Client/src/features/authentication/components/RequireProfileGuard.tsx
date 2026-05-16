import React, { useEffect, useState } from 'react';
import { Navigate } from 'react-router-dom';
import { getCustomerMeAPI } from '../../onboarding/api';
import { useAuth } from 'react-oidc-context'; 

interface GuardProps {
  children: React.ReactNode;
}

export const RequireProfileGuard: React.FC<GuardProps> = ({ children }) => {
  const auth = useAuth();
  const [isProfileReady, setIsProfileReady] = useState<boolean | null>(null);

  useEffect(() => {
    const checkProfile = async () => {
      // Đảm bảo user đã đăng nhập
      if (auth.isAuthenticated) {
        try {
          // SỬA Ở ĐÂY: Thêm dấu ? vào auth.user?.profile?.sub để an toàn tuyệt đối
          const identityId = auth.user?.profile?.sub; 
          
          // Bắt lỗi thêm 1 lớp nữa: Nếu không tìm thấy ID thì ép ra form
          if (!identityId) {
            setIsProfileReady(false);
            return;
          }

          const data = await getCustomerMeAPI(identityId); // Gọi API lấy thông tin

          // XỬ LÝ LỖI Ở ĐÂY: Dùng ?.trim() để cắt gọt sạch sẽ mọi khoảng trắng tàng hình
          const fName = data.firstName?.trim();
          const lName = data.lastName?.trim();

          // Kiểm tra xem tên bị rỗng không sau khi đã trim()
          if (!fName || !lName) {
            setIsProfileReady(false); // Chưa có tên -> Bắt điền form
          } else {
            setIsProfileReady(true);  // Đã có tên -> Mở cửa
          }
        } catch (error) {
          console.error("Lỗi khi tải hồ sơ:", error);
          setIsProfileReady(false); 
        }
      }
    };

    checkProfile();
  }, [auth.isAuthenticated, auth.user]);

  // 1. Đang tải thông tin (Có thêm auth.activeNavigator để chờ OIDC xử lý mượt hơn)
  if (auth.isLoading || auth.activeNavigator || isProfileReady === null) {
    return (
      <div className="min-h-screen flex items-center justify-center dark:bg-boxdark">
         <p className="text-gray-500 dark:text-gray-400">Checking profile...</p>
      </div>
    );
  }

  // 2. Nếu thiếu Tên/Họ -> Đuổi sang trang Onboarding
  if (isProfileReady === false) {
    return <Navigate to="/onboarding" replace />;
  }

  // 3. Nếu mọi thứ OK -> Mở cửa cho vào trang Home
  return <>{children}</>;
};