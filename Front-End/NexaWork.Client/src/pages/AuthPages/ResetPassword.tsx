import PageMeta from "../../components/common/PageMeta";
import { ResetPasswordForm } from "../../features/authentication/components/ResetPasswordForm";
import AuthLayout from "./AuthPageLayout";


export default function ResetPassword() {
  return (
    <>
      <PageMeta
        title="Forgot Password"
        description=""
      />
      <AuthLayout>
        <ResetPasswordForm />
      </AuthLayout>
    </>
  );
}
