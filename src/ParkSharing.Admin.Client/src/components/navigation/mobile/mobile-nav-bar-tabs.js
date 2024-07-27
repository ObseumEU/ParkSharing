// File: ./src/ParkSharing.Admin.Client/src/components/navigation/mobile/mobile-nav-bar-tabs.js
import React from "react";
import { MobileNavBarTab } from "./mobile-nav-bar-tab";
import { useAuth0 } from "@auth0/auth0-react";
import { jwtDecode } from "jwt-decode"; // Corrected import

export const MobileNavBarTabs = ({ handleClick }) => {
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
    <div className="mobile-nav-bar__tabs">
      {isAuthenticated && (
        <>
          <MobileNavBarTab path="/reservations" label="Rezervace" handleClick={handleClick} />
          <MobileNavBarTab path="/protected" label="Dostupnost" handleClick={handleClick} />
          <MobileNavBarTab path="/settings" label="Nastavení" handleClick={handleClick} />
          {hasPermission && <MobileNavBarTab path="/delete-settings" label="Delete Settings" handleClick={handleClick} />}
        </>
      )}
    </div>
  );
};
