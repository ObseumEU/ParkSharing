// File: src/ParkSharing/client-app/src/OwnerPage.js

import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './OwnerPage.css';

const OwnerPage = () => {
  const [spots, setSpots] = useState([]);
  const [newAvailability, setNewAvailability] = useState({ dayOfWeek: '', startTime: '', endTime: '', isRecurring: false });

  useEffect(() => {
    // Fetch parking spots for the owner
    axios.get('http://localhost:5239/owner/spots', { withCredentials: true })
      .then(response => setSpots(response.data))
      .catch(error => console.error('Error fetching spots:', error));
  }, []);

  const handleAddAvailability = (spotId) => {
    axios.post(`http://localhost:5239/owner/spots/${spotId}/availability`, newAvailability, { withCredentials: true })
      .then(response => {
        setSpots(spots.map(spot => spot.parkingSpotId === spotId ? { ...spot, availabilities: response.data.availabilities } : spot));
      })
      .catch(error => console.error('Error adding availability:', error));
  };

  const handleInputChange = (event) => {
    const { name, value, type, checked } = event.target;
    setNewAvailability(prevState => ({ ...prevState, [name]: type === 'checkbox' ? checked : value }));
  };

  return (
    <div className="owner-page">
      <h1>Manage My Parking Spots</h1>
      {spots.map(spot => (
        <div key={spot.parkingSpotId} className="spot-card">
          <h2>{spot.name}</h2>
          <p>Price per hour: {spot.pricePerHour}</p>
          <div className="availabilities">
            {spot.availabilities.map(avail => (
              <div key={avail.availabilityId} className="availability">
                <p>{avail.dayOfWeek || avail.specificDate}: {avail.startTime} - {avail.endTime}</p>
                <p>Recurring: {avail.isRecurring ? 'Yes' : 'No'}</p>
              </div>
            ))}
          </div>
          <div className="new-availability">
            <h3>Add New Availability</h3>
            <label>
              Day of Week:
              <select name="dayOfWeek" value={newAvailability.dayOfWeek} onChange={handleInputChange}>
                <option value="">Select</option>
                {['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'].map(day => (
                  <option key={day} value={day}>{day}</option>
                ))}
              </select>
            </label>
            <label>
              Start Time:
              <input type="time" name="startTime" value={newAvailability.startTime} onChange={handleInputChange} />
            </label>
            <label>
              End Time:
              <input type="time" name="endTime" value={newAvailability.endTime} onChange={handleInputChange} />
            </label>
            <label>
              Recurring:
              <input type="checkbox" name="isRecurring" checked={newAvailability.isRecurring} onChange={handleInputChange} />
            </label>
            <button onClick={() => handleAddAvailability(spot.parkingSpotId)}>Add Availability</button>
          </div>
        </div>
      ))}
    </div>
  );
};

export default OwnerPage;
