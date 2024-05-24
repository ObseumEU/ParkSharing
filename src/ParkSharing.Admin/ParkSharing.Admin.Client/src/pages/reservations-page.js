import React, { useEffect, useState, useCallback } from "react";
import { useAuth0 } from "@auth0/auth0-react";
import { PageLayout } from "../components/page-layout";
import { getReservations, rejectReservation, allowReservation } from "../services/api.service";

export const ReservationsPage = () => {
  const { getAccessTokenSilently } = useAuth0();
  const [reservations, setReservations] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchReservations = useCallback(async () => {
    try {
      const token = await getAccessTokenSilently();
      const { data } = await getReservations(token);
      if (data) {
        setReservations(data);
      }
    } catch (error) {
      console.error("Nepodařilo se načíst rezervace:", error);
    } finally {
      setLoading(false);
    }
  }, [getAccessTokenSilently]);

  useEffect(() => {
    fetchReservations();
  }, [fetchReservations]);

  const handleRejectReservation = async (reservationId) => {
    try {
      const token = await getAccessTokenSilently();
      await rejectReservation(token, reservationId);
      setReservations(reservations.map(r => r.id === reservationId ? { ...r, state: 1 } : r));
    } catch (error) {
      console.error("Nepodařilo se zamítnout rezervaci:", error);
    }
  };

  const handleAllowReservation = async (reservationId) => {
    try {
      const token = await getAccessTokenSilently();
      await allowReservation(token, reservationId);
      setReservations(reservations.map(r => r.id === reservationId ? { ...r, state: 0 } : r));
    } catch (error) {
      console.error("Nepodařilo se povolit rezervaci:", error);
    }
  };

  const getTotalHours = (start, end) => {
    const startDate = new Date(start);
    const endDate = new Date(end);
    const diffMs = endDate - startDate;
    return (diffMs / (1000 * 60 * 60)).toFixed(2);
  };

  const formatDate = (date) => {
    const options = { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' };
    return new Date(date).toLocaleString('cs-CZ', options);
  };

  if (loading) {
    return (
      <PageLayout>
        <div className="reservations-page">
          <p>Načítání...</p>
        </div>
      </PageLayout>
    );
  }

  return (
    <PageLayout>
      <div className="reservations-page">
        <h1>Rezervace</h1>
        {reservations.length === 0 ? (
          <p>Žádné rezervace k dispozici.</p>
        ) : (
          reservations.map((reservation) => (
            <div 
              key={reservation.id} 
              className={`reservation-item ${reservation.state === 1 ? 'reservation-rejected' : ''}`}
            >
              <p><strong>Telefon:</strong> {reservation.phone}</p>
              <p><strong>Rezervace:</strong> {formatDate(reservation.start)} - {formatDate(reservation.end)}</p>
              <p><strong>Celkový čas:</strong> {getTotalHours(reservation.start, reservation.end)} hodin</p>
              <p><strong>Cena:</strong> {reservation.price} Kč</p>
              <button 
                onClick={() => reservation.state === 0 ? handleRejectReservation(reservation.id) : handleAllowReservation(reservation.id)} 
                className="button button-reject"
              >
                {reservation.state === 0 ? 'Zamítnout' : 'Povolit'}
              </button>
            </div>
          ))
        )}
      </div>
    </PageLayout>
  );
};
