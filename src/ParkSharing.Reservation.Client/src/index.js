// File: src/ParkSharing/client-app/src/index.js

import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import App from './App';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom'; // Updated import

ReactDOM.render(
  <React.StrictMode>
    <Router>
      <Routes> {/* Updated from Switch to Routes */}
        <Route exact path="/" element={<App />} /> {/* Updated from component to element */}
      </Routes>
    </Router>
  </React.StrictMode>,
  document.getElementById('root')
);
