// File: ./src/ParkSharing.Admin.Client/src/services/api.service.js
import axios from 'axios';

const apiServerUrl = process.env.REACT_APP_API_SERVER_URL;
console.log('API Server URL:', process.env.REACT_APP_API_SERVER_URL);

export const getParkingSpot = async (accessToken) => {
  const config = {
    url: `${apiServerUrl}/avaliability`,
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${accessToken}`,
    },
  };

  try {
    const response = await axios(config);
    return { data: response.data, error: null };
  } catch (error) {
    return { data: null, error };
  }
};

export const updateParkingSpot = async (accessToken, spot) => {
  const config = {
    url: `${apiServerUrl}/avaliability`,
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${accessToken}`,
    },
    data: spot,
  };

  try {
    const response = await axios(config);
    return { data: response.data, error: null };
  } catch (error) {
    return { data: null, error };
  }
};

export const getSettings = async (accessToken) => {
  const config = {
    url: `${apiServerUrl}/settings`,
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${accessToken}`,
    },
  };

  try {
    const response = await axios(config);
    return { data: response.data, error: null };
  } catch (error) {
    return { data: null, error };
  }
};

export const updateSettings = async (accessToken, settings) => {
  const config = {
    url: `${apiServerUrl}/settings`,
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${accessToken}`,
    },
    data: settings,
  };

  try {
    const response = await axios(config);
    return { data: response.data, error: null };
  } catch (error) {
    return { data: null, error };
  }
};

export const getReservations = async (accessToken) => {
  const config = {
    url: `${apiServerUrl}/reservation`,
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${accessToken}`,
    },
  };

  try {
    const response = await axios(config);
    return { data: response.data, error: null };
  } catch (error) {
    return { data: null, error };
  }
};

export const rejectReservation = async (accessToken, reservationId) => {
  const config = {
    url: `${apiServerUrl}/reservation/reject`,
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${accessToken}`,
    },
    params: {
      reservationId,
    },
  };

  try {
    await axios(config);
  } catch (error) {
    console.error("Failed to reject reservation:", error);
  }
};

export const allowReservation = async (accessToken, reservationId) => {
  const config = {
    url: `${apiServerUrl}/reservation/allow`,
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${accessToken}`,
    },
    params: {
      reservationId,
    },
  };

  try {
    await axios(config);
  } catch (error) {
    console.error("Failed to allow reservation:", error);
  }
};

export const deleteSettings = async (accessToken) => {
  const config = {
    url: `${apiServerUrl}/admin/deletesettings`,
    method: 'DELETE',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${accessToken}`,
    },
  };

  try {
    const response = await axios(config);
    return { data: response.data, error: null };
  } catch (error) {
    return { data: null, error };
  }
};
