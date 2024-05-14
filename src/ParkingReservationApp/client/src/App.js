import React from 'react';
import CustomChatbot from './CustomChatbot';
import './index.css'; // Make sure to import the CSS file

function App() {
  return (
    <div className="App">
      <div className="chat-container">
        <CustomChatbot />
      </div>
    </div>
  );
}

export default App;
