import React from "react";
import { NavLink } from "react-router-dom";

export const NavBarBrand = () => {
  return (
    <div className="nav-bar__brand">
      <NavLink to="/">
        <img
          className="nav-bar__logo"
          src={`${process.env.PUBLIC_URL}/logo.svg`}
          alt="Auth0 shield logo"
          width="122"
          height="36"
        />
      </NavLink>
    </div>
  );
};
