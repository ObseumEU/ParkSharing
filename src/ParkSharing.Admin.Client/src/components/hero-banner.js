import React from "react";
import { useNavigate } from "react-router-dom";

export const HeroBanner = () => {
  const logo = "https://cdn.auth0.com/blog/developer-hub/react-logo.svg";
  const navigate = useNavigate();

  return (
    <div className="hero-banner hero-banner--pink-yellow">
      <img className="hero-banner__image" src={`${process.env.PUBLIC_URL}/logo.svg`} alt="Logo" />
      <h1 className="hero-banner__headline">Sdílej svoje parkovací místo!</h1>
      <div className="hero-banner__buttons">
        <button className="button__reserve" onClick={() => window.location.href = "https://parksharing.obseum.cloud"}>
          Rezervovat
        </button>
        <button className="button__share" onClick={() => navigate("/settings")}>
          Sdílet
        </button>
      </div>
    </div>
  );
};
