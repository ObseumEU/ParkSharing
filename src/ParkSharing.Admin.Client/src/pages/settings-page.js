import React, { useState, useEffect, useCallback } from "react";
import { useAuth0 } from "@auth0/auth0-react";
import { PageLayout } from "../components/page-layout";
import { getSettings, updateSettings } from "../services/api.service";

export const SettingsPage = () => {
  const { getAccessTokenSilently } = useAuth0();
  const [bankAccount, setBankAccount] = useState("");
  const [phone, setPhone] = useState("");
  const [parkingSpotName, setParkingSpotName] = useState("");
  const [pricePerHour, setPricePerHour] = useState(0);
  const [loading, setLoading] = useState(true);
  const [settingsChanged, setSettingsChanged] = useState(false);

  const fetchSettings = useCallback(async () => {
    try {
      const token = await getAccessTokenSilently();
      const { data } = await getSettings(token);
      if (data) {
        setBankAccount(data.bankAccount);
        setParkingSpotName(data.name);
        setPricePerHour(data.pricePerHour);
        setPhone(data.phone);
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
        pricePerHour,
        phone
      });
      setSettingsChanged(false);
    } catch (error) {
      console.error("Failed to update settings:", error);
      alert("Failed to update settings.");
    }
  };

  const handleChange = (setter) => (event) => {
    setter(event.target.value);
    setSettingsChanged(true);
  };

  if (loading) {
    return (
      <PageLayout>
        <div className="protected-page">
          <p>Načítání...</p>
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
            onChange={handleChange(setBankAccount)}
            className="input-field"
          />
        </div>
        <div className="section">
          <h2>Telefon</h2>
          <input
            type="text"
            placeholder="např. 724 764 298"
            value={phone}
            onChange={handleChange(setPhone)}
            className="input-field"
          />
        </div>
        <div className="section">
          <h2>Název parkovacího stání</h2>
          <input
            type="text"
            placeholder="např. CS453"
            value={parkingSpotName}
            onChange={handleChange(setParkingSpotName)}
            className="input-field"
          />
        </div>
        <div className="section">
          <h2>Cena Kč / Hod.</h2>
          <input
            type="number"
            placeholder="40"
            value={pricePerHour}
            onChange={(e) => {
              setPricePerHour(parseFloat(e.target.value));
              setSettingsChanged(true);
            }}
            className="input-field"
          />
        </div>
        {settingsChanged && (
          <button onClick={handleSaveSettings} className="button">
            Uložit
          </button>
        )}
      </div>
    </PageLayout>
  );
};
