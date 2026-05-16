import { BrowserRouter as Router, Routes, Route } from "react-router";
// import SignIn from "./pages/AuthPages/SignIn";
// import SignUp from "./pages/AuthPages/SignUp";
import NotFound from "./pages/OtherPage/NotFound";
import UserProfiles from "./pages/UserProfiles";
import Videos from "./pages/UiElements/Videos";
import Images from "./pages/UiElements/Images";
import Alerts from "./pages/UiElements/Alerts";
import Badges from "./pages/UiElements/Badges";
import Avatars from "./pages/UiElements/Avatars";
import Buttons from "./pages/UiElements/Buttons";
import LineChart from "./pages/Charts/LineChart";
import BarChart from "./pages/Charts/BarChart";
import Calendar from "./pages/Calendar";
import BasicTables from "./pages/Tables/BasicTables";
import FormElements from "./pages/Forms/FormElements";
import Blank from "./pages/Blank";
import AppLayout from "./layout/AppLayout";
import { ScrollToTop } from "./components/common/ScrollToTop";
import Home from "./pages/Dashboard/Home";
// import ForgotPassword from "./pages/AuthPages/ForgotPassword";
// import ResetPassword from "./pages/AuthPages/ResetPassword";
import { AuthProvider, useAuth } from "react-oidc-context";
import ProtectedRoute from "./routes/ProtectedRoute";
import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
// Nhớ import đúng đường dẫn hàm API lấy thông tin của bạn
import { getCustomerMeAPI } from './features/onboarding/api';
import { OnboardingForm } from "./features/onboarding/components/OnboardingForm";


const oidcConfig = {
  authority: "https://localhost:7036", // Your Auth Server URL
  client_id: "nexawork_react_web",
  redirect_uri: "http://localhost:5173/callback/login",
  response_type: "code",
  scope: "openid profile api",
  post_logout_redirect_uri: "http://localhost:5173/callback/logout",
};

function LoginCallback() {
  const auth = useAuth();
  const navigate = useNavigate();
  // Thêm state để theo dõi quá trình gọi API kiểm tra tên
  const [isCheckingProfile, setIsCheckingProfile] = useState(false);

  useEffect(() => {
    const checkUserStatus = async () => {
      // Khi OIDC xác nhận đăng nhập thành công và đã có thông tin user
      if (auth.isAuthenticated && auth.user?.profile?.sub) {
        setIsCheckingProfile(true);
        try {
          const identityId = auth.user.profile.sub;
          const data = await getCustomerMeAPI(identityId); 

          // Cắt khoảng trắng để chống lỗi tàng hình như hôm trước
          const fName = data.firstName?.trim();
          const lName = data.lastName?.trim();

          // Quyết định luồng đi ngay tại đây
          if (!fName || !lName) {
            // Thiếu tên -> Sang trang Onboarding
            navigate("/onboarding", { replace: true });
          } else {
            // Đã có tên -> Vào thẳng Dashboard
            navigate("/dashboard", { replace: true });
          }
        } catch (error) {
          console.error("Lỗi khi kiểm tra hồ sơ:", error);
          // Nếu lỗi API, an toàn nhất là cứ đẩy sang form Onboarding
          navigate("/onboarding", { replace: true });
        } finally {
          setIsCheckingProfile(false);
        }
      }
    };

    checkUserStatus();
  }, [auth.isAuthenticated, auth.user, navigate]);

  // Hiển thị trạng thái đang xử lý của OIDC hoặc đang gọi API kiểm tra
  if (auth.isLoading || isCheckingProfile) {
    return (
      <div className="min-h-screen flex items-center justify-center dark:bg-boxdark">
        <p className="text-gray-500 dark:text-gray-400">Processing login & checking profile...</p>
      </div>
    );
  }

  if (auth.error) {
    return <div>Oops... {auth.error.message}</div>;
  }

  return <div>Redirecting...</div>;
}

/**
 * Handle the logout callback. This is where the user will be redirected after they log out from the Auth Server.
 * @returns 
 */
function LogoutCallback() {
  // Clear any leftover manual state if you have any, then redirect to home
  window.location.replace("/");
  return <div>Logging you out completely...</div>;
}

export default function App() {
  return (
    <>
      <AuthProvider {...oidcConfig}>

        <Router>
          <ScrollToTop />
          <Routes>

            <Route path="/callback/login" element={<LoginCallback />} />
            <Route path="/callback/logout" element={<LogoutCallback />} />
            {/* Dashboard Layout */}
            {/* Auth Layout */}
            {/* <Route path="/signin" element={<SignIn />} />
            <Route path="/signup" element={<SignUp />} />
            <Route path="/forgot-password" element={<ForgotPassword />} />
            <Route path="/reset-password" element={<ResetPassword />} /> */}


            <Route element={<ProtectedRoute />}>

            <Route path="/onboarding" element={<OnboardingForm />} />

              <Route element={<AppLayout />}>
                <Route index path="/dashboard" element={<Home />} />
                <Route index path="/" element={<Home />} />

                {/* Others Page */}
                <Route path="/profile" element={<UserProfiles />} />
                <Route path="/calendar" element={<Calendar />} />
                <Route path="/blank" element={<Blank />} />

                {/* Forms */}
                <Route path="/form-elements" element={<FormElements />} />

                {/* Tables */}
                <Route path="/basic-tables" element={<BasicTables />} />

                {/* Ui Elements */}
                <Route path="/alerts" element={<Alerts />} />
                <Route path="/avatars" element={<Avatars />} />
                <Route path="/badge" element={<Badges />} />
                <Route path="/buttons" element={<Buttons />} />
                <Route path="/images" element={<Images />} />
                <Route path="/videos" element={<Videos />} />

                {/* Charts */}
                <Route path="/line-chart" element={<LineChart />} />
                <Route path="/bar-chart" element={<BarChart />} />
              </Route>
            </Route>



            {/* Fallback Route */}
            <Route path="*" element={<NotFound />} />
            <Route path="/blank" element={<Blank />} />

          </Routes>
        </Router>
      </AuthProvider>

    </>
  );
}