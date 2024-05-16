// File: src/ParkSharing/client-app/src/config.js

import { createChatBotMessage } from 'react-chatbot-kit';

const config = {
  botName: "ParkSharing",
  initialMessages: [createChatBotMessage(`Sdílení a rezervace parkovacích míst ve Velvarii! 🚗
  Nabízejte svá parkovací místa, když je nepoužíváte.
  Rezervujte parkování.
  
  Stačí napsat co potřebujete.
  `)],
  customStyles: {
    botMessageBox: {
      backgroundColor: "#376B7E",
    },
    chatButton: {
      backgroundColor: "#5ccc9d",
    },
  },
  state: {
    userType: null, // Add a state property to track user type
  },
  customComponents: {},
  widgets: [],
};

export default config;
