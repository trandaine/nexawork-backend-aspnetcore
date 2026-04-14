import PageMeta from "../../components/common/PageMeta";
import AuthLayout from "./AuthPageLayout";
import SignInForm from "../../features/authentication/components/SignInForm";

export default function SignIn() {
  return (
    <>
      <PageMeta
        title="NexaWork SignIn"
        description="Sign in to your NexaWork account to access your dashboard and manage your projects."
      />
      <AuthLayout>
        <SignInForm />
      </AuthLayout>
    </>
  );
}
