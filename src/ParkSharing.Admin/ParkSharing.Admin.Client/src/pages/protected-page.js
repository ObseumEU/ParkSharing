import React, { useState } from "react";
import { useAuth0 } from "@auth0/auth0-react";
import { PageLayout } from "../components/page-layout";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

export const ProtectedPage = () => {
  const [bankAccount, setBankAccount] = useState("");
  const [parkingSpotName, setParkingSpotName] = useState("");
  const [availability, setAvailability] = useState([{ start: new Date(), end: new Date(), recurrence: "Jednorázově" }]);

  const handleAddAvailability = () => {
    setAvailability([...availability, { start: new Date(), end: new Date(), recurrence: "Jednorázově" }]);
  };

  const handleRemoveAvailability = (index) => {
    const newAvailability = availability.filter((_, i) => i !== index);
    setAvailability(newAvailability);
  };

  return (
    <PageLayout>
      <div className="protected-page">
        <h1>Protected Page</h1>
        <p>Only authenticated users can access this page.</p>

        <div className="section">
          <h2>Číslo bankovního účtu</h2>
          <input 
            type="text" 
            placeholder="Číslo bankovního účtu" 
            value={bankAccount} 
            onChange={(e) => setBankAccount(e.target.value)} 
            className="input-field"
          />
        </div>

        <div className="section">
          <h2>Parkovací místo</h2>
          <input 
            type="text" 
            placeholder="Např. CS453" 
            value={parkingSpotName} 
            onChange={(e) => setParkingSpotName(e.target.value)} 
            className="input-field"
          />

          <h2>Dostupnost</h2>
          {availability.map((slot, index) => (
            <div key={index} className="availability-item">
              <DatePicker 
                selected={slot.start} 
                onChange={(date) => {
                  const newAvailability = [...availability];
                  newAvailability[index].start = date;
                  setAvailability(newAvailability);
                }} 
                showTimeSelect 
                dateFormat="Pp"
                className="input-field"
              />
              <DatePicker 
                selected={slot.end} 
                onChange={(date) => {
                  const newAvailability = [...availability];
                  newAvailability[index].end = date;
                  setAvailability(newAvailability);
                }} 
                showTimeSelect 
                dateFormat="Pp"
                className="input-field"
              />
              <select 
                value={slot.recurrence} 
                onChange={(e) => {
                  const newAvailability = [...availability];
                  newAvailability[index].recurrence = e.target.value;
                  setAvailability(newAvailability);
                }} 
                className="input-field"
              >
                <option>Jednorázově</option>
                <option>Denně</option>
                <option>Týdně</option>
                <option>Měsíčně</option>
              </select>
              <button onClick={() => handleRemoveAvailability(index)} className="button button-remove">Smazat</button>
            </div>
          ))}
          <button onClick={handleAddAvailability} className="button">Přidat </button>
        </div>
      </div>
    </PageLayout>
  );
};
