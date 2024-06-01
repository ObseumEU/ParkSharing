// File: src/ParkSharing/client-app/src/MessageParser.jsx

import axios from 'axios'; // Added missing import statement

class MessageParser {
    constructor(actionProvider) {
        this.actionProvider = actionProvider;
    }

    parse(message) {
        axios.post(`api/parking`, { input: message }, {
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
