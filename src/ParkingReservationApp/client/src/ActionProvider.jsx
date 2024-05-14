// src/ActionProvider.jsx
import React from 'react';
import ReactMarkdown from 'react-markdown';
import { createElement } from 'react'; // Import createElement for custom rendering

class ActionProvider {
    constructor(createChatBotMessage, setStateFunc) {
        this.createChatBotMessage = createChatBotMessage;
        this.setState = setStateFunc;
    }

    handleMessage = (markdownMessage) => {
        // Create a React element that renders Markdown without wrapping text in <p> tags.
        const messageComponent = this.createChatBotMessage(
            <ReactMarkdown 
                children={markdownMessage}
                components={{
                    // Override paragraph element to directly return its children instead of wrapping in <p>
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
