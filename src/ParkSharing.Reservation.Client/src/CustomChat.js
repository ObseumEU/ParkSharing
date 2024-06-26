import React, { useState, useEffect } from 'react';
import { Widget, addResponseMessage, addUserMessage, toggleWidget, renderCustomComponent } from 'react-chat-widget';
import axios from 'axios';
import Cookies from 'js-cookie';
import 'react-chat-widget/lib/styles.css';
import './CustomChat.css';

const apiServerUrl = process.env.REACT_APP_API_SERVER_URL;
console.log('API Server URL:', process.env.REACT_APP_API_SERVER_URL);

const CustomChat = () => {
  const [isBotTyping, setIsBotTyping] = useState(false);
  const [canSendMessage, setCanSendMessage] = useState(true);

  // Load messages from cookies
  useEffect(() => {
    const storedMessages = Cookies.get('chatMessages');
    if (storedMessages) {
      JSON.parse(storedMessages).forEach(message => {
        if (message.type === 'user') {
          addUserMessage(message.content);
        } else {
          addResponseMessage(message.content);
        }
      });
    } else {
      // Display initial message only if there are no stored messages
      if (!Cookies.get('initialMessageShown')) {
        addResponseMessage('Sdílení a rezervace parkovacích míst ve Velvarii! 🚗 Stačí napsat na kdy chcete místo rezervovat.');
        // Set a cookie to indicate that the initial message has been shown
        Cookies.set('initialMessageShown', 'true', { expires: 1 / 36 }); // 40 minutes
      }
    }
    toggleWidget(); // Open the chat widget
  }, []);

  // Save messages to cookies
  const saveMessagesToCookies = (messages) => {
    // Keep only the last 20 messages
    const lastMessages = messages.slice(-20);
    Cookies.set('chatMessages', JSON.stringify(lastMessages), { expires: 1 / 36 }); // 40 minutes
  };

  const handleNewUserMessage = async (newMessage) => {
    setIsBotTyping(true);
    setCanSendMessage(false);

    // Add user message to cookies
    const currentMessages = Cookies.get('chatMessages') ? JSON.parse(Cookies.get('chatMessages')) : [];
    currentMessages.push({ type: 'user', content: newMessage });
    saveMessagesToCookies(currentMessages);

    // Send the message to the backend
    try {
      const response = await axios.post(`${apiServerUrl}/parking`, {
        input: newMessage,
      }, {
        withCredentials: true,
      });

      const botReply = response.data.reply;
      addResponseMessage(botReply);

      // Add bot message to cookies
      currentMessages.push({ type: 'bot', content: botReply });
      saveMessagesToCookies(currentMessages);
    } catch (error) {
      console.error('Error sending message:', error);
      addResponseMessage('Sorry, something went wrong with your request.');
    } finally {
      setIsBotTyping(false);
      setCanSendMessage(true);
    }
  };

  const CustomInput = ({ handleNewUserMessage }) => {
    const [message, setMessage] = useState('');

    const handleSubmit = (event) => {
      event.preventDefault();
      if (message.trim()) {
        handleNewUserMessage(message);
        setMessage('');
      }
    };

    return (
      <form onSubmit={handleSubmit} className="rcw-new-message">
        <input
          className="rcw-input"
          type="text"
          value={message}
          onChange={(e) => setMessage(e.target.value)}
          placeholder="Type a message..."
          disabled={!canSendMessage}
        />
        <button type="submit" className="rcw-send" disabled={!canSendMessage}>
          Send
        </button>
      </form>
    );
  };

  return (
    <div className="chat-container">
      <Widget
        handleNewUserMessage={handleNewUserMessage}
        title="Sdílení parkování"
        subtitle="Příklad: Chci rezerovat parkovací místo na zítřek od 8:00 do 15:00"
        customLauncher={() => {}}
        customStyles={{
          launcher: {
            display: 'none',
          }
        }}
        renderCustomComponent={() => renderCustomComponent(CustomInput, { handleNewUserMessage })}
      />
    </div>
  );
};

export default CustomChat;
