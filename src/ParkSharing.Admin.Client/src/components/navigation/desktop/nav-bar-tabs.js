import { useAuth0 } from "@auth0/auth0-react";
import React from "react";
import { NavBarTab } from "./nav-bar-tab";

export const NavBarTabs = () => {
  const { isAuthenticated } = useAuth0();

  const handleRoadmapClick = () => {
    window.open("https://parksharingobseum.featurebase.app/", "_blank");
  };

  const handlePatreonClick = () => {
    window.open("https://www.patreon.com/fomodog/membership", "_blank");
  };

  return (
    <div className="nav-bar__tabs">
      {/* <NavBarTab path="/profile" label="Profile" /> */}
      {/* <NavBarTab path="/public" label="Public" /> */}
      {isAuthenticated && (
        <>
          <div className="nav-bar__tab" onClick={handlePatreonClick}>Nakrm vývojáře</div>
          <NavBarTab path="/reservations" label="Rezervace" />
          <NavBarTab path="/protected" label="Dostupnost" />
          <NavBarTab path="/settings" label="Nastavení" />
          <div className="nav-bar__tab" onClick={handleRoadmapClick}>Feedback</div>
          {/* <NavBarTab path="/admin" label="Admin" /> */}
        </>
      )}
    </div>
  );
};
