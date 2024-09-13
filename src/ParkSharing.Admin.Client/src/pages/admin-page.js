import React, { useEffect, useState } from 'react';
import { useAuth0 } from '@auth0/auth0-react';
import { PageLayout } from '../components/page-layout';
import axios from 'axios';

// Mapping function for recurrence
const getRecurrenceText = (recurrence) => {
  switch (recurrence) {
    case 0:
      return "Once";
    case 1:
      return "Daily";
    case 2:
      return "Weekly";
    case 3:
      return "Weekdays (Mon-Fri)";
    default:
      return "Unknown";
  }
};

export const AdminPage = () => {
  const { getAccessTokenSilently } = useAuth0();
  const [spots, setSpots] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchSpots = async () => {
      try {
        const token = await getAccessTokenSilently();
        const response = await axios.get(`${process.env.REACT_APP_API_SERVER_URL}/admin/spots`, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        setSpots(response.data);
      } catch (error) {
        console.error('Failed to fetch spots:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchSpots();
  }, [getAccessTokenSilently]);

  if (loading) {
    return (
      <PageLayout>
        <div>Načítání...</div>
      </PageLayout>
    );
  }

  return (
    <PageLayout>
      <h1>Admin - Parking Spots</h1>
      <div>
        {spots.map(spot => (
          <div key={spot.publicId} className="parking-spot">
            <h2>{spot.name}</h2>
            <div className="parking-spot-details">
              <div className="details-item">
                <p><strong>Bank Account:</strong></p>
                <p>{spot.bankAccount}</p>
              </div>
              <div className="details-item">
                <p><strong>Price per Hour:</strong></p>
                <p>{spot.pricePerHour}</p>
              </div>
            </div>
            <div className="availability">
              <h3>Availability</h3>
              {spot.availability && spot.availability.length > 0 ? (
                <ul>
                  {spot.availability.map(avail => (
                    <li key={avail.publicId}>
                      <div className="details-item">
                        <p><strong>Start:</strong></p>
                        <p>{new Date(avail.start).toLocaleString()}</p>
                      </div>
                      <div className="details-item">
                        <p><strong>End:</strong></p>
                        <p>{new Date(avail.end).toLocaleString()}</p>
                      </div>
                      <div className="details-item">
                        <p><strong>Recurrence:</strong></p>
                        <p>{getRecurrenceText(avail.recurrence)}</p>
                      </div>
                      {avail.dayOfWeek && (
                        <div className="details-item">
                          <p><strong>Day of Week:</strong></p>
                          <p>{avail.dayOfWeek}</p>
                        </div>
                      )}
                    </li>
                  ))}
                </ul>
              ) : (
                <p>No availability set</p>
              )}
            </div>
            <div className="reservations">
              <h3>Reservations</h3>
              {spot.reservations && spot.reservations.length > 0 ? (
                <ul>
                  {spot.reservations.map(reservation => (
                    <li key={reservation.publicId}>
                      <div className="details-item">
                        <p><strong>Phone:</strong></p>
                        <p>{reservation.phone}</p>
                      </div>
                      <div className="details-item">
                        <p><strong>Start:</strong></p>
                        <p>{new Date(reservation.start).toLocaleString()}</p>
                      </div>
                      <div className="details-item">
                        <p><strong>End:</strong></p>
                        <p>{new Date(reservation.end).toLocaleString()}</p>
                      </div>
                      <div className="details-item">
                        <p><strong>Price:</strong></p>
                        <p>{reservation.price}</p>
                      </div>
                      <div className="details-item">
                        <p><strong>Status:</strong></p>
                        <p>{reservation.state}</p>
                      </div>
                    </li>
                  ))}
                </ul>
              ) : (
                <p>No reservations</p>
              )}
            </div>
          </div>
        ))}
      </div>
    </PageLayout>
  );
};
