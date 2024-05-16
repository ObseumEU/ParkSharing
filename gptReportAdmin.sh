#!/bin/bash


echo "This is application \
ASP.NET with frontend React with Vite \ 
This app need have authentication used with Auth0\ 
After user login, or sign up. He used for parking spod owners to share theus parking space for others. \
Every owner has abilise set times when parking space is free and when is not. \
Every space possible set by hours, days, weeks, months. \
Similar how set recuring event is googel evet. Possible set recuring event for parking space. \
For example every monday, wednesday, friday from 8:00 to 12:00. "

search_directory="./src/ParkSharing.Admin/ParkSharing.Admin.Client"
exclude_path="src/ParkSharing.Admin/ParkSharing.Admin.Client/node_modules"
find "$search_directory" -type d -path "./$exclude_path" -prune -o -type f -name "*.js" -print | while read -r file_path
do
    echo "--------------------------------------------------"
    echo "File: $file_path"
    echo "--------------------------------------------------"
    cat "$file_path"  # Use cat to print the file content
    echo  # Adds a newline for better readability between files
done