// File: ./src/ParkSharing.Admin.Client/src/components/authentication-guard.js
import { withAuthenticationRequired, useAuth0 } from "@auth0/auth0-react";
import React from "react";
import { PageLoader } from "./page-loader";
import { jwtDecode } from "jwt-decode"; // Corrected import

export const AuthenticationGuard = ({ component, permission }) => {
  const Component = withAuthenticationRequired(component, {
    onRedirecting: () => (
      <div className="page-layout">
        <PageLoader />
      </div>
    ),
  });

  const { getAccessTokenSilently } = useAuth0();
  const [hasPermission, setHasPermission] = React.useState(false);

  React.useEffect(() => {
    const checkPermissions = async () => {
      const token = await getAccessTokenSilently();
      const decodedToken = jwtDecode(token); // Corrected usage
      const permissions = decodedToken.permissions || [];
      setHasPermission(!permission || permissions.includes(permission));
    };
    checkPermissions();
  }, [getAccessTokenSilently, permission]);

  if (!hasPermission) {
    return <div className="page-layout"><h1>Access Denied</h1></div>;
  }

  return <Component />;
};
