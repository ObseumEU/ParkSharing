import axios from 'axios';

const apiServerUrl = process.env.REACT_APP_API_SERVER_URL;

export const getParkingSpot = async (accessToken) => {
  const config = {
    url: `${apiServerUrl}/api/parkingspot`,
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
    url: `${apiServerUrl}/api/parkingspot`,
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
    url: `${apiServerUrl}/api/parkingspot`,
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
