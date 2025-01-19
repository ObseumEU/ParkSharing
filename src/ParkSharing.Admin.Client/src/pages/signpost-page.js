import React from "react";
import { Link } from "react-router-dom";
import { PageLayout } from "../components/page-layout";

export const SignpostPage = () => {
  return (
    <PageLayout>
      <div style={{ textAlign: "center" }}>
        <h1>Chci parkování</h1>
        <div style={{ margin: "20px" }}>
          <Link to="https://parksharing.obseum.cloud/">
            <button className="button">Rezervovat</button>
          </Link>
        </div>
        <div style={{ margin: "20px" }}>
          <Link to="https://parksharing-admin.obseum.cloud/">
            <button className="button">Nabídnout</button>
          </Link>
        </div>
      </div>
    </PageLayout>
  );
};
