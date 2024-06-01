import { useAuth0 } from "@auth0/auth0-react";
import React from "react";
import { MobileNavBarTab } from "./mobile-nav-bar-tab";

export const MobileNavBarTabs = ({ handleClick }) => {
  const { isAuthenticated } = useAuth0();

  return (
    <div className="mobile-nav-bar__tabs">
      {/* <MobileNavBarTab
        path="/profile"
        label="Parkování"
        handleClick={handleClick}
      /> */}
      {/* <MobileNavBarTab
        path="/public"
        label="Public"
        handleClick={handleClick}
      /> */}
      {isAuthenticated && (
        <>
          <MobileNavBarTab
            path="/reservations"
            label="Rezervace"
            handleClick={handleClick}
          />
          <MobileNavBarTab
            path="/protected"
            label="Dostupnost"
            handleClick={handleClick}
          />
          <MobileNavBarTab
            path="/settings"
            label="Nastavení"
            handleClick={handleClick}
          />
          {/* <MobileNavBarTab
            path="/protected"
            label="Správa"
            handleClick={handleClick}
          /> */}
        </>
      )}
    </div>
  );
};