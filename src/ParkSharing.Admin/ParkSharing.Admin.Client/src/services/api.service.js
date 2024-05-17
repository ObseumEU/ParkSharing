import axios from 'axios';

const apiServerUrl = process.env.REACT_APP_API_SERVER_URL;

export const getParkingSpots = async (accessToken) => {
  const config = {
    url: `${apiServerUrl}/api/parkingspots`,
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

export const addParkingSpot = async (accessToken, spot) => {
  const config = {
    url: `${apiServerUrl}/api/parkingspots`,
    method: 'POST',
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

export const updateParkingSpot = async (accessToken, spot) => {
  const config = {
    url: `${apiServerUrl}/api/parkingspots/${spot.id}`,
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

export const deleteParkingSpot = async (accessToken, id) => {
  const config = {
    url: `${apiServerUrl}/api/parkingspots/${id}`,
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
