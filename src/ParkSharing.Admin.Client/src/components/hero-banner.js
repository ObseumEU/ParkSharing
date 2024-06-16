// File: ./src/ParkSharing.Admin/ParkSharing.Admin.Client/src/components/hero-banner.js

import React from "react";
import { SignupButton } from "./buttons/signup-button";
import { LoginButton } from "./buttons/login-button";

export const HeroBanner = () => {
  const logo = "https://cdn.auth0.com/blog/developer-hub/react-logo.svg";

  return (
    <div className="hero-banner hero-banner--pink-yellow">
        <img className="hero-banner__image" src={`${process.env.PUBLIC_URL}/logo.svg`} alt="Logo" />
      <h1 className="hero-banner__headline">Sdílej svoje parkovací místo!</h1>
      <div className="hero-banner__buttons">
        {/* <SignupButton /> */}
        {/* <LoginButton /> */}
      </div>
    </div>
  );
};
