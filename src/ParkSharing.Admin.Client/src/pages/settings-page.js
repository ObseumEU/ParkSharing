import React, { useState, useEffect, useCallback } from "react";
import { useAuth0 } from "@auth0/auth0-react";
import { PageLayout } from "../components/page-layout";
import { getSettings, updateSettings } from "../services/api.service";

export const SettingsPage = () => {
  const { getAccessTokenSilently } = useAuth0();
  const [bankAccount, setBankAccount] = useState("");
  const [parkingSpotName, setParkingSpotName] = useState("");
  const [pricePerHour, setPricePerHour] = useState(0);
  const [loading, setLoading] = useState(true);

  const fetchSettings = useCallback(async () => {
    try {
      const token = await getAccessTokenSilently();
      const { data } = await getSettings(token);
      if (data) {
        setBankAccount(data.bankAccount);
        setParkingSpotName(data.name);
        setPricePerHour(data.pricePerHour);
      }
    } catch (error) {
      console.error("Failed to fetch settings:", error);
    } finally {
      setLoading(false);
    }
  }, [getAccessTokenSilently]);

  useEffect(() => {
    fetchSettings();
  }, [fetchSettings]);

  const handleSaveSettings = async () => {
    try {
      const token = await getAccessTokenSilently();
      await updateSettings(token, {
        bankAccount,
        name: parkingSpotName,
        pricePerHour
      });
    } catch (error) {
      console.error("Failed to update settings:", error);
      alert("Failed to update settings.");
    }
  };

  if (loading) {
    return (
      <PageLayout>
        <div className="protected-page">
          <p>Loading...</p>
        </div>
      </PageLayout>
    );
  }

  return (
    <PageLayout>
      <div className="protected-page">
        <h1>Nastavení</h1>
        <div className="section">
          <h2>Bankovní účet</h2>
          <input
            type="text"
            placeholder="např. 22223111/0100"
            value={bankAccount}
            onChange={(e) => setBankAccount(e.target.value)}
            className="input-field"
          />
        </div>
        <div className="section">
          <h2>Název parkovacího stání</h2>
          <input
            type="text"
            placeholder="např. CS453"
            value={parkingSpotName}
            onChange={(e) => setParkingSpotName(e.target.value)}
            className="input-field"
          />
        </div>
        <div className="section">
          <h2>Cena Kč / Hod.</h2>
          <input
            type="number"
            placeholder="40"
            value={pricePerHour}
            onChange={(e) => setPricePerHour(parseFloat(e.target.value))}
            className="input-field"
          />
        </div>
        <button onClick={handleSaveSettings} className="button">
          Uložit
        </button>
      </div>
    </PageLayout>
  );
};
