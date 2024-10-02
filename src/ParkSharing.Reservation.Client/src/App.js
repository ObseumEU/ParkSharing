/* ./src/ParkSharing.Reservation.Client/src/App.js */

import React from 'react';
import CustomChat from './CustomChat';
import './index.css';

function App() {
  const isMobile = window.innerWidth <= 640;

  return (
    <div className="App">
      {isMobile ? (
        <div className="main-chat-container">
          <CustomChat />
        </div>
      ) : (
        <div className="main-chat-container">
          <CustomChat />
        </div>
        // <div className="desktop-message">
        //   <p>Tato aplikace je podporována pouze na mobilních zařízeních.</p>
        // </div>
      )}
    </div>
  );
}

export default App;
