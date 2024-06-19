import React, { useState, useEffect, useCallback, useRef } from "react";
import { useAuth0 } from "@auth0/auth0-react";
import { PageLayout } from "../components/page-layout";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import { getParkingSpot, updateParkingSpot } from "../services/api.service";
import { CSSTransition, TransitionGroup } from "react-transition-group";

const recurrenceMap = {
  "Jednorázové": 0,
  "Denně": 1,
  "Týdně": 2,
  "Pracovní dny (Po-Pá)": 3,
};

const recurrenceReverseMap = {
  0: "Jednorázové",
  1: "Denně",
  2: "Týdně",
  3: "Pracovní dny (Po-Pá)",
};

export const ProtectedPage = () => {
  const { getAccessTokenSilently } = useAuth0();
  const [availability, setAvailability] = useState([]);
  const [editIndex, setEditIndex] = useState(null);
  const [loading, setLoading] = useState(true);
  const [editData, setEditData] = useState(null);
  const newItemRef = useRef(null);

  const fetchSpot = useCallback(async () => {
    try {
      const token = await getAccessTokenSilently();
      const { data } = await getParkingSpot(token);
      if (data) {
        const parsedAvailability = data.availability.map((slot) => ({
          ...slot,
          start: new Date(slot.start),
          end: new Date(slot.end),
          recurrence: recurrenceReverseMap[slot.recurrence],
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

  const saveAvailability = useCallback(
    async (newAvailabilityList) => {
      try {
        const token = await getAccessTokenSilently();
        const updatedAvailability = newAvailabilityList.map((slot) => ({
          ...slot,
          recurrence: recurrenceMap[slot.recurrence],
        }));
        await updateParkingSpot(token, { availability: updatedAvailability });
      } catch (error) {
        console.error("Failed to update parking spot:", error);
      }
    },
    [getAccessTokenSilently]
  );

  const handleAddAvailability = () => {
    setEditData({
      start: new Date(),
      end: new Date(),
      recurrence: "Jednorázové",
    });
    setEditIndex(availability.length); // Set editIndex to the length of availability to denote a new item
  };

  const handleSaveAvailability = async () => {
    let updatedAvailability = [...availability];
    if (editIndex !== null && editIndex < availability.length) {
      updatedAvailability[editIndex] = editData;
    } else {
      updatedAvailability.push(editData);
    }
    setAvailability(updatedAvailability);
    setEditIndex(null);
    setEditData(null);
    await saveAvailability(updatedAvailability);
  };

  const handleChangeEditData = (key, value) => {
    setEditData((prev) => ({ ...prev, [key]: value }));
  };

  const handleEditAvailability = (index) => {
    setEditData(availability[index]);
    setEditIndex(index);
  };

  const handleDeleteAvailability = (index) => {
    const updatedAvailability = availability.filter((_, i) => i !== index);
    setAvailability(updatedAvailability);
    saveAvailability(updatedAvailability);
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
          <TransitionGroup>
            {availability.map((slot, index) => (
              <CSSTransition
                key={index}
                timeout={500}
                classNames="availability-item"
              >
                <div className="availability-item">
                  {editIndex === index ? (
                    <div className="availability-item__body">
                      <label>Opakování:</label>
                      <select
                        value={editData.recurrence}
                        onChange={(e) =>
                          handleChangeEditData("recurrence", e.target.value)
                        }
                        className="input-field"
                      >
                        <option>Jednorázové</option>
                        <option>Denně</option>
                        <option>Týdně</option>
                        <option>Pracovní dny (Po-Pá)</option>
                      </select>
                      {editData.recurrence === "Týdně" && (
                        <>
                          <label>Den v týdnu:</label>
                          <select
                            value={editData.dayOfWeek || ""}
                            onChange={(e) =>
                              handleChangeEditData("dayOfWeek", e.target.value)
                            }
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
                      {editData.recurrence === "Jednorázové" ? (
                        <>
                          <label>Začátek:</label>
                          <DatePicker
                            selected={editData.start}
                            onChange={(date) =>
                              handleChangeEditData("start", date)
                            }
                            showTimeSelect
                            dateFormat="dd/MM/yyyy HH:mm"
                            timeFormat="HH:mm"
                            className="input-field custom-datepicker"
                          />
                          <label>Konec:</label>
                          <DatePicker
                            selected={editData.end}
                            onChange={(date) =>
                              handleChangeEditData("end", date)
                            }
                            showTimeSelect
                            dateFormat="dd/MM/yyyy HH:mm"
                            timeFormat="HH:mm"
                            className="input-field custom-datepicker"
                          />
                        </>
                      ) : (
                        <>
                          <label>Od:</label>
                          <DatePicker
                            selected={editData.start}
                            onChange={(date) =>
                              handleChangeEditData("start", date)
                            }
                            showTimeSelect
                            showTimeSelectOnly
                            timeIntervals={60} // Updated to hourly intervals
                            timeCaption="Čas"
                            dateFormat="HH:mm"
                            timeFormat="HH:mm"
                            className="input-field custom-datepicker"
                          />
                          <label>Do:</label>
                          <DatePicker
                            selected={editData.end}
                            onChange={(date) =>
                              handleChangeEditData("end", date)
                            }
                            showTimeSelect
                            showTimeSelectOnly
                            timeIntervals={60} // Updated to hourly intervals
                            timeCaption="Čas"
                            dateFormat="HH:mm"
                            timeFormat="HH:mm"
                            className="input-field custom-datepicker"
                          />
                        </>
                      )}
                      <div className="button-container-right">
                        <button onClick={handleSaveAvailability} className="button">
                          Uložit dostupnost
                        </button>
                      </div>
                    </div>
                  ) : (
                    <div className="availability-item__body">
                      <p>Opakování: {slot.recurrence}</p>
                      {slot.recurrence === "Týdně" && (
                        <p>Den v týdnu: {slot.dayOfWeek}</p>
                      )}
                      <p>Začátek: {slot.start.toLocaleString()}</p>
                      <p>Konec: {slot.end.toLocaleString()}</p>
                      <div className="button-container-right">
                        <button
                          onClick={() => handleEditAvailability(index)}
                          className="button button-edit"
                        >
                          ✎
                        </button>
                        <button
                          onClick={() => handleDeleteAvailability(index)}
                          className="button button-delete"
                        >
                          ✖
                        </button>
                      </div>
                    </div>
                  )}
                </div>
              </CSSTransition>
            ))}
            {editIndex === availability.length && (
              <CSSTransition timeout={500} classNames="availability-item">
                <div className="availability-item" ref={newItemRef}>
                  <div className="availability-item__body">
                    <label>Opakování:</label>
                    <select
                      value={editData.recurrence}
                      onChange={(e) =>
                        handleChangeEditData("recurrence", e.target.value)
                      }
                      className="input-field"
                    >
                      <option>Jednorázové</option>
                      <option>Denně</option>
                      <option>Týdně</option>
                      <option>Pracovní dny (Po-Pá)</option>
                    </select>
                    {editData.recurrence === "Týdně" && (
                      <>
                        <label>Den v týdnu:</label>
                        <select
                          value={editData.dayOfWeek || ""}
                          onChange={(e) =>
                            handleChangeEditData("dayOfWeek", e.target.value)
                          }
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
                    {editData.recurrence === "Jednorázové" ? (
                      <>
                        <label>Začátek:</label>
                        <DatePicker
                          selected={editData.start}
                          onChange={(date) =>
                            handleChangeEditData("start", date)
                          }
                          showTimeSelect
                          dateFormat="dd/MM/yyyy HH:mm"
                          timeFormat="HH:mm"
                          className="input-field custom-datepicker"
                        />
                        <label>Konec:</label>
                        <DatePicker
                          selected={editData.end}
                          onChange={(date) =>
                            handleChangeEditData("end", date)
                          }
                          showTimeSelect
                          dateFormat="dd/MM/yyyy HH:mm"
                          timeFormat="HH:mm"
                          className="input-field custom-datepicker"
                        />
                      </>
                    ) : (
                      <>
                        <label>Od:</label>
                        <DatePicker
                          selected={editData.start}
                          onChange={(date) =>
                            handleChangeEditData("start", date)
                          }
                          showTimeSelect
                          showTimeSelectOnly
                          timeIntervals={60} // Updated to hourly intervals
                          timeCaption="Čas"
                          dateFormat="HH:mm"
                          timeFormat="HH:mm"
                          className="input-field custom-datepicker"
                        />
                        <label>Do:</label>
                        <DatePicker
                          selected={editData.end}
                          onChange={(date) =>
                            handleChangeEditData("end", date)
                          }
                          showTimeSelect
                          showTimeSelectOnly
                          timeIntervals={60} // Updated to hourly intervals
                          timeCaption="Čas"
                          dateFormat="HH:mm"
                          timeFormat="HH:mm"
                          className="input-field custom-datepicker"
                        />
                      </>
                    )}
                    <div className="button-container-right">
                      <button onClick={handleSaveAvailability} className="button">
                        Uložit dostupnost
                      </button>
                    </div>
                  </div>
                </div>
              </CSSTransition>
            )}
          </TransitionGroup>
          {editIndex === null && (
            <button onClick={handleAddAvailability} className="button">
              Přidat dostupnost
            </button>
          )}
        </div>
      </div>
    </PageLayout>
  );
};
