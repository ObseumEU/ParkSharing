import React from 'react';
import config from './config.js';
import MessageParser from './MessageParser.jsx';
import ActionProvider from './ActionProvider.jsx';
import Chatbot from 'react-chatbot-kit';
import 'react-chatbot-kit/build/main.css';

const CustomChatbot = () => {
  return (
    <div style={{ height: '100%', overflow: 'hidden' }}>
      <Chatbot
        config={config}
        messageParser={MessageParser}
        actionProvider={ActionProvider}
        customStyles={{
          botMessageBox: {
            backgroundColor: "#5ccc9d",
          },
          chatButton: {
            backgroundColor: "#5ccc9d",
          }
        }}
      />
    </div>
  );
};

export default CustomChatbot;
