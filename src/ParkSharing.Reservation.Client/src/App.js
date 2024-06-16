import React from 'react';
import CustomChat from './CustomChat';
import './index.css'; // Make sure to import the CSS file

function App() {
  return (
    <div className="App">
      <div className="chat-container">
        <CustomChat />
      </div>
    </div>
  );
}

export default App;
