// src/ParkingReservationApp/client-app/src/ActionProvider.jsx
import React from 'react';
import ReactMarkdown from 'react-markdown';
import { createElement } from 'react';

class ActionProvider {
    constructor(createChatBotMessage, setStateFunc) {
        this.createChatBotMessage = createChatBotMessage;
        this.setState = setStateFunc;
    }

    handleMessage = (markdownMessage) => {
        const messageComponent = this.createChatBotMessage(
            <ReactMarkdown 
                children={markdownMessage}
                components={{
                    p: ({ node, ...props }) => createElement('span', props)
                }}
            />
        );
        this.updateChatbotState(messageComponent);
    }

    updateChatbotState(message) {
        this.setState(prevState => ({
            ...prevState,
            messages: [...prevState.messages, message]
        }));
    }
}

export default ActionProvider;
