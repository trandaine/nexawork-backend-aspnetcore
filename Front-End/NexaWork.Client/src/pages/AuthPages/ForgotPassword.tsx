import PageMeta from "../../components/common/PageMeta";
import { ForgotPasswordForm } from "../../features/authentication/components/ForgotPasswordForm";
import AuthLayout from "./AuthPageLayout";


export default function ForgotPassword() {
  return (
    <>
      <PageMeta
        title="Forgot Password"
        description=""
      />
      <AuthLayout>
        <ForgotPasswordForm />
      </AuthLayout>
    </>
  );
}
