// File: ./src/ParkSharing.Admin.Client/src/pages/delete-settings-page.js
import React, { useState } from "react";
import { useAuth0 } from "@auth0/auth0-react";
import { PageLayout } from "../components/page-layout";
import { deleteSettings } from "../services/api.service";

export const DeleteSettingsPage = () => {
  const { getAccessTokenSilently } = useAuth0();
  const [message, setMessage] = useState("");

  const handleDeleteSettings = async () => {
    try {
      const token = await getAccessTokenSilently();
      const response = await deleteSettings(token);
      setMessage(`Success: ${response.data}`);
    } catch (error) {
      setMessage(`Error: ${error.message}`);
    }
  };

  return (
    <PageLayout>
      <div className="delete-settings-page">
        <h1>Delete Settings</h1>
        <button onClick={handleDeleteSettings} className="button">
          Delete Settings
        </button>
        {message && <p>{message}</p>}
      </div>
    </PageLayout>
  );
};
