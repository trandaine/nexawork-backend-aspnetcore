import PageMeta from "../../components/common/PageMeta";
import SignUpForm from "../../features/authentication/components/SignUpForm";
import AuthLayout from "./AuthPageLayout";

export default function SignUp() {
  return (
    <>
      <PageMeta
        title="NexaWork SignUp"
        description="Create a new NexaWork account to access your dashboard and manage your projects."
      />
      <AuthLayout>
        <SignUpForm />
      </AuthLayout>
    </>
  );
}
