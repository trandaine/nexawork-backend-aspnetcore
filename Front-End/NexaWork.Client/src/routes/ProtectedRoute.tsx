import { Outlet } from 'react-router-dom';
import { useAuth } from 'react-oidc-context';
import { useEffect } from 'react';

const ProtectedRoute = () => {
  const auth = useAuth();

  useEffect(() => {
    // If the app is done loading the auth state, and the user is NOT authenticated,
    // and a redirect isn't already happening, send them to the Auth Server.
    if (!auth.isLoading && !auth.isAuthenticated && !auth.activeNavigator) {
      console.log('No valid session found. Redirecting to NexaWork Auth Server...');
      auth.signinRedirect();
    }
  // }, [auth]);
  }, [auth.isAuthenticated, auth.isLoading, auth.activeNavigator, auth]);

  // While the library is checking the token or redirecting, show a loading state
  if (auth.isLoading || auth.activeNavigator) {
    // return <div>Loading your workspace...</div>; // Replace with a spinner if you want!
    return (
      <div className="min-h-screen flex items-center justify-center">
        <p>Loading your workspace...</p>
      </div>
    );
  }

  // If we still aren't authenticated (about to redirect), don't render the secure pages
  if (!auth.isAuthenticated) {
    return null; 
  }

  // If we have a token, render the Dashboard!
  console.log('Valid token found. Access granted.');
  return <Outlet />;
};

export default ProtectedRoute;