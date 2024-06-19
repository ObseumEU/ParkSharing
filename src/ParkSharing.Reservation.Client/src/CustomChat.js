import React, { useState, useEffect } from 'react';
import { Widget, addResponseMessage, toggleWidget } from 'react-chat-widget';
import 'react-chat-widget/lib/styles.css';
import axios from 'axios';
import './CustomChat.css';

const apiServerUrl = process.env.REACT_APP_API_SERVER_URL;
console.log('API Server URL:', process.env.REACT_APP_API_SERVER_URL);

const CustomChat = () => {
  const [isBotTyping, setIsBotTyping] = useState(false);

  const handleNewUserMessage = async (newMessage) => {
    setIsBotTyping(true);

    // Send the message to the backend
    try {
      const response = await axios.post(`${apiServerUrl}/parking`, {
        input: newMessage,
      }, {
        withCredentials: true,
      });
      addResponseMessage(response.data.reply);
    } catch (error) {
      console.error('Error sending message:', error);
      addResponseMessage('Sorry, something went wrong with your request.');
    } finally {
      setIsBotTyping(false);
    }
  };

  // Add initial message and open the chat widget
  useEffect(() => {
    addResponseMessage('Sdílení a rezervace parkovacích míst ve Velvarii! 🚗 Stačí napsat na kdy chcete místo rezervovat.');
    toggleWidget(); // Open the chat widget
  }, []);

  return (
    <div className="chat-container">
      <Widget
        handleNewUserMessage={handleNewUserMessage}
        title="Sdílení parkování"
        subtitle="Příklad: Chci rezerovat parkovací místo na zítřek od 8:00 do 15:00"
      />
    </div>
  );
};

export default CustomChat;
