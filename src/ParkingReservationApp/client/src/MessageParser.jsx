// src/MessageParser.jsx
import axios from 'axios';

class MessageParser {
    constructor(actionProvider) {
        this.actionProvider = actionProvider;
    }

    parse(message) {
        axios.post('http://localhost:5239/parking', { input: message }, {
            withCredentials: true // This ensures cookies are sent along with the request
        })
            .then(response => {
                this.actionProvider.handleMessage(response.data.reply);
            })
            .catch(error => {
                console.error('Error:', error);
                this.actionProvider.handleMessage('Sorry, something went wrong with your request.');
            });
    }
}

export default MessageParser;
