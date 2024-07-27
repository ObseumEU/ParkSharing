// File: ./src/ParkSharing.Admin.Client/src/components/navigation/desktop/nav-bar-tabs.js
import React from "react";
import { NavBarTab } from "./nav-bar-tab";
import { useAuth0 } from "@auth0/auth0-react";
import { jwtDecode } from "jwt-decode"; // Corrected import

export const NavBarTabs = () => {
  const { isAuthenticated, getAccessTokenSilently } = useAuth0();
  const [hasPermission, setHasPermission] = React.useState(false);

  React.useEffect(() => {
    const checkPermissions = async () => {
      if (isAuthenticated) {
        const token = await getAccessTokenSilently();
        const decodedToken = jwtDecode(token); // Corrected usage
        const permissions = decodedToken.permissions || [];
        setHasPermission(permissions.includes("write:admin-deletesettings"));
      }
    };
    checkPermissions();
  }, [isAuthenticated, getAccessTokenSilently]);

  return (
    <div className="nav-bar__tabs">
      {isAuthenticated && (
        <>
          <NavBarTab path="/reservations" label="Rezervace" />
          <NavBarTab path="/protected" label="Dostupnost" />
          <NavBarTab path="/settings" label="Nastavení" />
          {hasPermission && <NavBarTab path="/delete-settings" label="Delete Settings" />}
        </>
      )}
    </div>
  );
};
