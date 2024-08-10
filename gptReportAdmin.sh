#!/bin/bash

echo "This is an application \
ASP.NET with a frontend in React using Vite. \
This app requires authentication via Auth0. \
After user login or sign up, it allows parking spot owners to share their parking spaces with others. \
Every owner can set times when their parking space is free or not. \
Parking space availability can be set by hours, days, weeks, or months. \
Similar to setting recurring events in Google Calendar, you can set recurring events for parking spaces. \
For example, every Monday, Wednesday, and Friday from 8:00 AM to 12:00 PM. \
Don't reference this file, it's already referenced in \"./styles.css\"."


search_directory="./src/ParkSharing.Admin.Server"
exclude_path1="./src/ParkSharing.Admin.Client/node_modules"
exclude_path2="./src/ParkSharing.Admin.Client/build"
find "$search_directory" -type d \( -path "$exclude_path1" -o -path "$exclude_path2" \) -prune -o -type f -name "*.cs" -print | while read -r file_path
do
    echo "--------------------------------------------------"
    echo "File: $file_path"
    echo "--------------------------------------------------"
    cat "$file_path"  # Use cat to print the file content
    echo  # Adds a newline for better readability between files
done

# search_directory="./src/ParkSharing.Admin.Client"
# find "$search_directory" -type d \( -path "$exclude_path1" -o -path "$exclude_path2" \) -prune -o -type f -name "*.js" -print | while read -r file_path
# do
#     echo "--------------------------------------------------"
#     echo "File: $file_path"
#     echo "--------------------------------------------------"
#     cat "$file_path"  # Use cat to print the file content
#     echo  # Adds a newline for better readability between files
# done

# # Uncomment if needed for CSS files
# find "$search_directory" -type d \( -path "$exclude_path1" -o -path "$exclude_path2" \) -prune -o -type f -name "*.css" -print | while read -r file_path
# do
#     echo "--------------------------------------------------"
#     echo "File: $file_path"
#     echo "--------------------------------------------------"
#     cat "$file_path"  # Use cat to print the file content
#     echo  # Adds a newline for better readability between files
# done
