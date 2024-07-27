// File: ./src/ParkSharing.Admin.Client/src/app.js
import { useAuth0 } from "@auth0/auth0-react";
import React from "react";
import { Route, Routes } from "react-router-dom";
import { PageLoader } from "./components/page-loader";
import { AuthenticationGuard } from "./components/authentication-guard";
import { AdminPage } from "./pages/admin-page";
import { CallbackPage } from "./pages/callback-page";
import { HomePage } from "./pages/home-page";
import { NotFoundPage } from "./pages/not-found-page";
import { ProtectedPage } from "./pages/protected-page";
import { PublicPage } from "./pages/public-page";
import { ReservationsPage } from "./pages/reservations-page";
import { SettingsPage } from "./pages/settings-page";
import Policies from "./pages/policies-page";
import { DeleteSettingsPage } from "./pages/delete-settings-page"; // Ensure this is imported

export const App = () => {
  const { isLoading } = useAuth0();

  if (isLoading) {
    return (
      <div className="page-layout">
        <PageLoader />
      </div>
    );
  }

  return (
    <Routes>
      <Route path="/" element={<HomePage />} />
      <Route path="/public" element={<PublicPage />} />
      <Route
        path="/protected"
        element={<AuthenticationGuard component={ProtectedPage} />}
      />
      <Route
        path="/reservations"
        element={<AuthenticationGuard component={ReservationsPage} />}
      />
      <Route
        path="/settings"
        element={<AuthenticationGuard component={SettingsPage} />}
      />
      <Route path="/callback" element={<CallbackPage />} />
      <Route
        path="/admin"
        element={<AuthenticationGuard component={AdminPage} />}
      />
      <Route path="/policies" element={<Policies />} />
      <Route
        path="/delete-settings"
        element={<AuthenticationGuard component={DeleteSettingsPage} permission="write:admin-deletesettings" />}
      />
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
};
