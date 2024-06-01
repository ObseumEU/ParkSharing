import axios from 'axios';

const apiServerUrl = process.env.REACT_APP_API_SERVER_URL;

export const getParkingSpot = async (accessToken) => {
  const config = {
    url: `${apiServerUrl}/api/avaliability`,
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
    url: `${apiServerUrl}/api/avaliability`,
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
    url: `${apiServerUrl}/api/settings`,
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
    url: `${apiServerUrl}/api/settings`,
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
    url: `${apiServerUrl}/api/reservation`,
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
    url: `${apiServerUrl}/api/reservation/reject`,
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
    url: `${apiServerUrl}/api/reservation/allow`,
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