import { useAuth0 } from "@auth0/auth0-react";
import React, { useState } from "react";
import { PageLayout } from "../components/page-layout";

export const ProtectedPage = () => {
  const { user } = useAuth0();
  const [bankAccount, setBankAccount] = useState("");
  const [parkingSpots, setParkingSpots] = useState([]);
  const [spotName, setSpotName] = useState("");
  const [availability, setAvailability] = useState({
    from: "",
    to: "",
    repeat: "none",
  });

  const handleAddSpot = () => {
    const newSpot = {
      name: spotName,
      availability: { ...availability },
    };
    setParkingSpots([...parkingSpots, newSpot]);
    setSpotName("");
    setAvailability({ from: "", to: "", repeat: "none" });
  };

  return (
    <PageLayout>
      <div className="content-layout">
        <h1 id="page-title" className="content__title">
          Protected Page
        </h1>
        <div className="content__body">
          <p id="page-description">
            <strong>Only authenticated users can access this page.</strong>
          </p>
          <div>
            <h2>Set Bank Account</h2>
            <input
              type="text"
              value={bankAccount}
              onChange={(e) => setBankAccount(e.target.value)}
              placeholder="Bank Account Number"
            />
          </div>
          <div>
            <h2>Create Parking Spot</h2>
            <input
              type="text"
              value={spotName}
              onChange={(e) => setSpotName(e.target.value)}
              placeholder="Parking Spot Name"
            />
            <h3>Set Availability</h3>
            <input
              type="time"
              value={availability.from}
              onChange={(e) =>
                setAvailability({ ...availability, from: e.target.value })
              }
              placeholder="From"
            />
            <input
              type="time"
              value={availability.to}
              onChange={(e) =>
                setAvailability({ ...availability, to: e.target.value })
              }
              placeholder="To"
            />
            <select
              value={availability.repeat}
              onChange={(e) =>
                setAvailability({ ...availability, repeat: e.target.value })
              }
            >
              <option value="none">One Time</option>
              <option value="daily">Daily</option>
              <option value="weekly">Weekly</option>
              <option value="monthly">Monthly</option>
            </select>
            <button onClick={handleAddSpot}>Add Parking Spot</button>
          </div>
          <div>
            <h2>My Parking Spots</h2>
            <ul>
              {parkingSpots.map((spot, index) => (
                <li key={index}>
                  {spot.name}: {spot.availability.from} - {spot.availability.to} ({spot.availability.repeat})
                </li>
              ))}
            </ul>
          </div>
        </div>
      </div>
    </PageLayout>
  );
};
