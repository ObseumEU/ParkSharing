// App.js

import React, { useState, useEffect, useRef } from 'react';
import axios from 'axios';
import Cookies from 'js-cookie';
import ReactMarkdown from 'react-markdown'; // Import react-markdown
import './App.css';

const App = () => {
  const [messages, setMessages] = useState([]);
  const [inputMessage, setInputMessage] = useState('');
  const [isBotTyping, setIsBotTyping] = useState(false);
  const [canSendMessage, setCanSendMessage] = useState(true);
  const messagesEndRef = useRef(null);
  const inputRef = useRef(null); // Reference to the input field

  // Load messages from cookies on mount
  useEffect(() => {
    const storedMessages = Cookies.get('chatMessages');
    if (storedMessages) {
      setMessages(JSON.parse(storedMessages));
    } else {
      // If no stored messages, show initial bot message with Markdown
      const initialBotMessage =
        '👋 **Vítejte!** Rezervujte si parkování snadno. Napište den a čas, kdy chcete místo, např.: 🗓️ "_Zítra 8:00 - 17:00_" 🚗';
      setMessages([{ type: 'bot', content: initialBotMessage }]);
    }
  }, []);

  // Save messages to cookies whenever messages change
  useEffect(() => {
    // Keep only the last 50 messages
    const lastMessages = messages.slice(-50);
    Cookies.set('chatMessages', JSON.stringify(lastMessages), { expires: 1 / 36 }); // 40 minutes
  }, [messages]);

  // Scroll to bottom when messages change
  useEffect(() => {
    if (messagesEndRef.current) {
      // Use setTimeout to ensure it runs after the DOM updates
      setTimeout(() => {
        messagesEndRef.current.scrollIntoView({ behavior: 'smooth' });
      }, 100);
    }
  }, [messages]);

  const handleSendMessage = async () => {
    if (inputMessage.trim() === '') return;

    // Add user message to messages
    const newMessages = [...messages, { type: 'user', content: inputMessage }];
    setMessages(newMessages);
    setInputMessage('');
    setIsBotTyping(true);
    setCanSendMessage(false);

    // Keep focus on the input after sending the message
    if (inputRef.current) {
      inputRef.current.focus();
    }

    try {
      // Send message to backend
      const response = await axios.post(
        `${process.env.REACT_APP_API_SERVER_URL}/parking`,
        {
          input: inputMessage,
        },
        {
          withCredentials: true,
        }
      );

      const botReply = response.data.reply;
      // Add bot message to messages
      setMessages((prevMessages) => [...prevMessages, { type: 'bot', content: botReply }]);
    } catch (error) {
      console.error('Error sending message:', error);
      setMessages((prevMessages) => [
        ...prevMessages,
        {
          type: 'bot',
          content: 'Omlouváme se, došlo k chybě při zpracování vašeho požadavku.',
        },
      ]);
    } finally {
      setIsBotTyping(false);
      setCanSendMessage(true);
    }
  };

  const handleInputKeyPress = (e) => {
    if (e.key === 'Enter' && canSendMessage && inputMessage.trim() !== '') {
      e.preventDefault(); // Prevents adding a new line on mobile devices
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
                className={`message ${message.type === 'user' ? 'user-message' : 'bot-message'}`}
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
            ref={inputRef} // Attach the ref to the input element
            type="text"
            placeholder="Napsat zprávu..."
            value={inputMessage}
            onChange={(e) => setInputMessage(e.target.value)}
            onKeyPress={handleInputKeyPress}
            autoComplete="off"
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
