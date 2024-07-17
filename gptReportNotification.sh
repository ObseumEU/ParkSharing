#!/bin/bash

echo ""


search_directory="./src/ParkSharing.Notification"
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

# Uncomment if needed for CSS files
# find "$search_directory" -type d \( -path "$exclude_path1" -o -path "$exclude_path2" \) -prune -o -type f -name "*.css" -print | while read -r file_path
# do
#     echo "--------------------------------------------------"
#     echo "File: $file_path"
#     echo "--------------------------------------------------"
#     cat "$file_path"  # Use cat to print the file content
#     echo  # Adds a newline for better readability between files
# done
