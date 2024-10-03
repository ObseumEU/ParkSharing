import React from "react";
import { useNavigate } from "react-router-dom";

export const HeroBanner = () => {
  const logo = "https://cdn.auth0.com/blog/developer-hub/react-logo.svg";
  const navigate = useNavigate();

  const handleSendLink = () => {
    const reservationLink = "https://parksharing.obseum.cloud";

    // Copy the reservation link to the clipboard
    navigator.clipboard.writeText(reservationLink)
      .then(() => {
        alert("Odkaz pro rezervaci byl zkopírován do schránky!");
      })
      .catch(err => {
        console.error('Failed to copy the link: ', err);
        alert("Nepodařilo se zkopírovat odkaz. Zkuste to prosím znovu.");
      });
  };

  return (
    <div className="hero-banner hero-banner--pink-yellow">
      <img
        className="hero-banner__image"
        src={`${process.env.PUBLIC_URL}/logo.svg`}
        alt="Logo"
      />
      <h1 className="hero-banner__headline">Administrace parkovacích stání!</h1>
      <div className="hero-banner__buttons">
        <button className="button__reserve" onClick={handleSendLink}>
          <span className="material-icons">share</span>
          Sdílet parkování
        </button>
      </div>
    </div>
  );
};
