import React, { useState, useEffect, useCallback } from "react";
import { useAuth0 } from "@auth0/auth0-react";
import { PageLayout } from "../components/page-layout";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import { getParkingSpot, updateParkingSpot } from "../services/api.service";

export const ProtectedPage = () => {
  const { getAccessTokenSilently } = useAuth0();
  const [availability, setAvailability] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchSpot = useCallback(async () => {
    try {
      const token = await getAccessTokenSilently();
      const { data } = await getParkingSpot(token);
      if (data) {
        const parsedAvailability = data.availability.map(slot => ({
          ...slot,
          start: new Date(slot.start),
          end: new Date(slot.end),
          dayOfWeek: slot.dayOfWeek || null,
        }));
        setAvailability(parsedAvailability);
      }
    } catch (error) {
      console.error("Failed to fetch parking spot:", error);
    } finally {
      setLoading(false);
    }
  }, [getAccessTokenSilently]);

  useEffect(() => {
    fetchSpot();
  }, [fetchSpot]);

  const saveAvailability = useCallback(async (newAvailability) => {
    try {
      const token = await getAccessTokenSilently();
      await updateParkingSpot(token, { availability: newAvailability });
    } catch (error) {
      console.error("Failed to update parking spot:", error);
    }
  }, [getAccessTokenSilently]);

  const handleAddAvailability = async () => {
    const newAvailability = [...availability, { start: new Date(), end: new Date(), recurrence: "Jednorázové" }];
    setAvailability(newAvailability);
    await saveAvailability(newAvailability);
  };

  const handleRemoveAvailability = async (index) => {
    const newAvailability = availability.filter((_, i) => i !== index);
    setAvailability(newAvailability);
    await saveAvailability(newAvailability);
  };

  const handleChangeAvailability = async (index, key, value) => {
    const newAvailability = [...availability];
    newAvailability[index][key] = value;
    setAvailability(newAvailability);
    await saveAvailability(newAvailability);
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
        <div className="section">
          <h2>Dostupnost parkovacího místa</h2>
          <p>Nastavte, kdy je vaše parkovací místo k dispozici pro ostatní.</p>
          {availability.map((slot, index) => (
            <div key={index} className="availability-item">
              <div className="availability-item__header">
                <h3>Dostupnost {index + 1}</h3>
              </div>
              <div className="availability-item__body">
                <label>Opakování:</label>
                <select 
                  value={slot.recurrence} 
                  onChange={(e) => handleChangeAvailability(index, "recurrence", e.target.value)} 
                  className="input-field"
                >
                  <option>Jednorázové</option>
                  <option>Denně</option>
                  <option>Týdně</option>
                  <option>Pracovní dny (Po-Pá)</option>
                </select>
                {slot.recurrence === "Týdně" && (
                  <>
                    <label>Den v týdnu:</label>
                    <select 
                      value={slot.dayOfWeek || ""} 
                      onChange={(e) => handleChangeAvailability(index, "dayOfWeek", e.target.value)} 
                      className="input-field"
                    >
                      <option value="">Vyberte den</option>
                      <option value="Sunday">Neděle</option>
                      <option value="Monday">Pondělí</option>
                      <option value="Tuesday">Úterý</option>
                      <option value="Wednesday">Středa</option>
                      <option value="Thursday">Čtvrtek</option>
                      <option value="Friday">Pátek</option>
                      <option value="Saturday">Sobota</option>
                    </select>
                  </>
                )}
                {slot.recurrence === "Jednorázové" ? (
                  <>
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
                  </>
                ) : (
                  <>
                    <label>Od:</label>
                    <DatePicker 
                      selected={slot.start} 
                      onChange={(date) => handleChangeAvailability(index, "start", date)} 
                      showTimeSelect 
                      showTimeSelectOnly
                      timeIntervals={15}
                      timeCaption="Čas"
                      dateFormat="hh:mm"
                      className="input-field"
                    />
                    <label>Do:</label>
                    <DatePicker 
                      selected={slot.end} 
                      onChange={(date) => handleChangeAvailability(index, "end", date)} 
                      showTimeSelect 
                      showTimeSelectOnly
                      timeIntervals={15}
                      timeCaption="Čas"
                      dateFormat="hh:mm"
                      className="input-field"
                    />
                  </>
                )}
                <button onClick={() => handleRemoveAvailability(index)} className="button button-remove">Odstranit</button>
              </div>
            </div>
          ))}
          <button onClick={handleAddAvailability} className="button">Přidat dostupnost</button>
        </div>
      </div>
    </PageLayout>
  );
};
