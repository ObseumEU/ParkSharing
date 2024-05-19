import React, { useState, useEffect, useCallback } from "react";
import { useAuth0 } from "@auth0/auth0-react";
import { PageLayout } from "../components/page-layout";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import { getParkingSpots, addParkingSpot, updateParkingSpot, deleteParkingSpot } from "../services/api.service";

export const ProtectedPage = () => {
  const { getAccessTokenSilently } = useAuth0();
  const [bankAccount, setBankAccount] = useState("");
  const [parkingSpotName, setParkingSpotName] = useState("");
  const [availability, setAvailability] = useState([{ start: new Date(), end: new Date(), recurrence: "Jednorázově" }]);
  const [spotId, setSpotId] = useState(null);

  const fetchSpots = useCallback(async () => {
    const token = await getAccessTokenSilently();
    const { data } = await getParkingSpots(token);
    if (data && data.length > 0) {
      const spot = data[0]; // Assuming a single parking spot for simplicity
      setSpotId(spot.id);
      setBankAccount(spot.bankAccount);
      setParkingSpotName(spot.name);
      // Ensure dates are parsed as Date objects
      const parsedAvailability = spot.availability.map(slot => ({
        ...slot,
        start: new Date(slot.start),
        end: new Date(slot.end)
      }));
      setAvailability(parsedAvailability);
    }
  }, [getAccessTokenSilently]);

  useEffect(() => {
    fetchSpots();
  }, [fetchSpots]);

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
    const newAvailability = [...availability, { start: new Date(), end: new Date(), recurrence: "Jednorázově" }];
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
        <div className="section">
          <h2>Dostupnost</h2>
          <p>Kdy je parkovací stání možné půjčit. </p>
          {availability.map((slot, index) => (
            <div key={index} className="availability-item">
              <div className="availability-item__header">
                <h3>Dostupnost {index + 1}</h3>
              </div>
              <div className="availability-item__body">
                <label>Začátek:</label>
                <DatePicker 
                  selected={slot.start} 
                  onChange={(date) => handleChangeAvailability(index, "start", date)} 
                  showTimeSelect 
                  dateFormat="Pp"
                  className="input-field"
                />
                <label>Konec:</label>
                <DatePicker 
                  selected={slot.end} 
                  onChange={(date) => handleChangeAvailability(index, "end", date)} 
                  showTimeSelect 
                  dateFormat="Pp"
                  className="input-field"
                />
                <label>Opakování:</label>
                <select 
                  value={slot.recurrence} 
                  onChange={(e) => handleChangeAvailability(index, "recurrence", e.target.value)} 
                  className="input-field"
                >
                  <option>Jednorázově</option>
                  <option>Denně</option>
                  <option>Týdně</option>
                  <option>Měsíčně</option>
                </select>
                <button onClick={() => handleRemoveAvailability(index)} className="button button-remove">Smazat</button>
              </div>
            </div>
          ))}
          <button onClick={handleAddAvailability} className="button">Přidat dostupnost</button>
        </div>
      </div>
    </PageLayout>
  );
};
