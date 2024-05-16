#!/bin/bash


echo "Write .net aplication using chat gpt API\
\ Write ony code, file by file one bu one, not exmplanantions , only just full workign code. No examples!
Application is used for reservation of parking places in comunity parking between nebours. Some nebour have parking place but they dont use them all the time. Main goal is make able to toher people from outside worl or from our comunity reserve some places and send money to bank account. Every person who want share place have bank account. If not the parking place is free. People must be able in chat register parking place for share, set what times regulary or one time what day is avaliable. They must be able manage own place sharing times. Other people, everyone must be able reserve some places wit limits for only 2 places same time one day. If they reserve some place they need pay for it if its not free. Everything is possible in chat on website. Chat must be usable on mobile phone\
Key features\
*Frontend is chat\
* Backend is C# net core 8\
* Database is postgres\
* Using docker \
* Chat must bee full screen on mobile phones    
\
On backend is used chatgpt for undertund commands and handle converstion.\
\ http://localhost:5239/parking is endpoitn for sending and receiving messages from chatbot. The chatbot is runnign on this endpoint http://localhost:5239/parking is endpoitn for sending and receiving messages from chatbot.
Frontend must use already existing chat solution modul/framework for example react-simple-chatbot.\
\
Write first how connect react fe to backend\
\
Conversation logic is on backend\
\
Write code, not text. \
\ In React command line I use powershell
Used xUnit test for test" \
 My code must be secure POST method Parking.  I need to be able anyone from internet use application, but i need to be sure nobody misuse the endpoint and fake different conversation of someone else. Everyone must be able handle and comunicate only with theis session conversation. Use Sessions for this

# Specify the directory to search in. Default is current directory if not provided.
search_directory="${1:-.}"

# Define the path to exclude from search
exclude_path="src/ParkSharing/client/node_modules"

# Find all .cs files while excluding the specified path and print their paths and contents
find "$search_directory" -type d -path "./$exclude_path" -prune -o -type f -name "*.cs" -print | while read -r file_path
do
    echo "--------------------------------------------------"
    echo "File: $file_path"
    echo "--------------------------------------------------"
    cat "$file_path"  # Use cat to print the file content
    echo  # Adds a newline for better readability between files
done

echo "--------------------------------------------------"
echo "File: ./src/ParkSharing/client/src/App.js"
echo "--------------------------------------------------"
cat "./src/ParkSharing/client/src/App.js"



echo "--------------------------------------------------"
echo "File: ./src/ParkSharing/client/package.json"
echo "--------------------------------------------------"
cat "./src/ParkSharing/client/package.json"


echo "--------------------------------------------------"
echo "File: src/ParkSharing/client/src/CustomChatbot.js"
echo "--------------------------------------------------"
cat "src/ParkSharing/client/src/CustomChatbot.js"

echo "--------------------------------------------------"
echo "File: src/ParkSharing/client/src/MessageParser.jsx"
echo "--------------------------------------------------"
cat "src/ParkSharing/client/src/MessageParser.jsx"

echo "--------------------------------------------------"
echo "File: src/ParkSharing/client/src/config.js"
echo "--------------------------------------------------"
cat "src/ParkSharing/client/src/config.js"

echo "--------------------------------------------------"
echo "File: src/ParkSharing/client/src/ActionProvider.jsx"
echo "--------------------------------------------------"
cat "src/ParkSharing/client/src/ActionProvider.jsx"

echo "--------------------------------------------------"
echo "File: src/ParkSharing/client/src/index.css"
echo "--------------------------------------------------"
cat "src/ParkSharing/client/src/index.css"