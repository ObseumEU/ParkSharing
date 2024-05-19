import React, { useState, useEffect, useCallback } from "react";
import { useAuth0 } from "@auth0/auth0-react";
import { PageLayout } from "../components/page-layout";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import { getParkingSpot, addParkingSpot, updateParkingSpot } from "../services/api.service";

export const SettingsPage = () => {
  const { getAccessTokenSilently } = useAuth0();
  const [bankAccount, setBankAccount] = useState("");
  const [parkingSpotName, setParkingSpotName] = useState("");
  const [availability, setAvailability] = useState([{ start: new Date(), end: new Date(), recurrence: "One-time" }]);
  const [spotId, setSpotId] = useState(null);

  const fetchSpot = useCallback(async () => {
    const token = await getAccessTokenSilently();
    const { data } = await getParkingSpot(token);
    if (data) {
      setSpotId(data.id);
      setBankAccount(data.bankAccount);
      setParkingSpotName(data.name);
      const parsedAvailability = data.availability.map(slot => ({
        ...slot,
        start: new Date(slot.start),
        end: new Date(slot.end)
      }));
      setAvailability(parsedAvailability);
    }
  }, [getAccessTokenSilently]);

  useEffect(() => {
    fetchSpot();
  }, [fetchSpot]);

  const saveSpot = useCallback(async (updatedSpot) => {
    const token = await getAccessTokenSilently();
    if (spotId) {
      await updateParkingSpot(token, { id: spotId, ...updatedSpot });
    } else {
      const { data } = await addParkingSpot(token, updatedSpot);
      setSpotId(data.id);
    }
  }, [getAccessTokenSilently, spotId]);

  useEffect(() => {
    saveSpot({ bankAccount, name: parkingSpotName, availability });
  }, [bankAccount, parkingSpotName, availability, saveSpot]);

  const handleAddAvailability = () => {
    const newAvailability = [...availability, { start: new Date(), end: new Date(), recurrence: "One-time" }];
    setAvailability(newAvailability);
  };

  const handleRemoveAvailability = (index) => {
    const newAvailability = availability.filter((_, i) => i !== index);
    setAvailability(newAvailability);
  };

  const handleChangeAvailability = (index, key, value) => {
    const newAvailability = [...availability];
    newAvailability[index][key] = value;
    setAvailability(newAvailability);
  };

  return (
    <PageLayout>
      <div className="protected-page">
        <h1>Settings</h1>

        <div className="section">
          <h2>Bank Account</h2>
          <input 
            type="text" 
            placeholder="Bank Account Number" 
            value={bankAccount} 
            onChange={(e) => setBankAccount(e.target.value)} 
            className="input-field"
          />
        </div>

        <div className="section">
          <h2>Parking Spot Name</h2>
          <input 
            type="text" 
            placeholder="e.g., CS453" 
            value={parkingSpotName} 
            onChange={(e) => setParkingSpotName(e.target.value)} 
            className="input-field"
          />
        </div>
      </div>
    </PageLayout>
  );
};
