// App.js

import React, { useState, useEffect, useRef } from 'react';
import axios from 'axios';
import ReactMarkdown from 'react-markdown';
import './App.css';

const App = () => {
  const initialBotMessage =
    '👋 Vítejte! Rezervujte si parkování snadno. Napište den a čas, kdy chcete místo, např.: 🗓️ "Zítra 8:00 - 17:00" 🚗';

  // Initialize messages from localStorage
  const [messages, setMessages] = useState(() => {
    const storedMessages = localStorage.getItem('chatMessages');
    return storedMessages
      ? JSON.parse(storedMessages)
      : [{ type: 'bot', content: initialBotMessage }];
  });
  const [inputMessage, setInputMessage] = useState('');
  const [isBotTyping, setIsBotTyping] = useState(false);
  const [canSendMessage, setCanSendMessage] = useState(true);
  const messagesEndRef = useRef(null);
  const inputRef = useRef(null);

  // Save messages to localStorage whenever messages change
  useEffect(() => {
    const lastMessages = messages.slice(-50); // Keep only the last 50 messages
    localStorage.setItem('chatMessages', JSON.stringify(lastMessages));
  }, [messages]);

  // Scroll to bottom when messages change
  useEffect(() => {
    if (messagesEndRef.current) {
      // Optimize scroll to run after DOM updates
      setTimeout(() => {
        messagesEndRef.current.scrollIntoView({ behavior: 'smooth' });
      }, 100);
    }
  }, [messages]);

  const handleSendMessage = async () => {
    const trimmedMessage = inputMessage.trim();
    if (trimmedMessage === '') return;

    setInputMessage('');
    setIsBotTyping(true);
    setCanSendMessage(false);

    // Update messages with user input
    setMessages((prevMessages) => [
      ...prevMessages,
      { type: 'user', content: trimmedMessage },
    ]);

    // Keep focus on the input field
    if (inputRef.current) {
      inputRef.current.focus();
    }

    try {
      const apiUrl =
        process.env.REACT_APP_API_SERVER_URL || 'https://default-api-url.com';
      const response = await axios.post(
        `${apiUrl}/parking`,
        { input: trimmedMessage },
        { withCredentials: true }
      );

      const botReply = response.data.reply;
      setMessages((prevMessages) => [
        ...prevMessages,
        { type: 'bot', content: botReply },
      ]);
    } catch (error) {
      console.error('Error sending message:', error);
      setMessages((prevMessages) => [
        ...prevMessages,
        {
          type: 'bot',
          content:
            'Omlouváme se, došlo k chybě při zpracování vašeho požadavku.',
        },
      ]);
    } finally {
      setIsBotTyping(false);
      setCanSendMessage(true);
    }
  };

  const handleInputKeyDown = (e) => {
    if (e.key === 'Enter' && canSendMessage && inputMessage.trim() !== '') {
      e.preventDefault(); // Prevents adding a new line
      handleSendMessage();
    }
  };

  return (
    <div className="App">
      <div className="chat-container">
        <div className="chat-header">
          <h2>Sdílení Parkování</h2>
        </div>
        <div className="chat-messages">
          <div className="messages-wrapper">
            {messages.map((message, index) => (
              <div
                key={index}
                className={`message ${
                  message.type === 'user' ? 'user-message' : 'bot-message'
                }`}
              >
                <div className="message-content">
                  <ReactMarkdown>{message.content}</ReactMarkdown>
                </div>
              </div>
            ))}
            {isBotTyping && (
              <div className="message bot-message typing">
                <div className="message-content">
                  <div className="typing-indicator">
                    <span></span>
                    <span></span>
                    <span></span>
                  </div>
                </div>
              </div>
            )}
            <div ref={messagesEndRef} />
          </div>
        </div>
        <div className="chat-input">
          <input
            ref={inputRef}
            type="text"
            placeholder="Napsat zprávu..."
            value={inputMessage}
            onChange={(e) => setInputMessage(e.target.value)}
            onKeyDown={handleInputKeyDown}
            autoComplete="off"
            aria-label="Napsat zprávu"
          />
          <button
            onClick={handleSendMessage}
            disabled={!canSendMessage || inputMessage.trim() === ''}
          >
            <svg viewBox="0 0 24 24" className="send-icon">
              <path d="M2,21L23,12L2,3V10L17,12L2,14V21Z" />
            </svg>
          </button>
        </div>
      </div>
    </div>
  );
};

export default App;
