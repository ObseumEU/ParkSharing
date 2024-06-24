#!/bin/bash

exclude_paths=("./src/ParkSharing.Reservation.Client/node_modules" "./src/ParkSharing.Reservation.Client/dist" "./src/ParkSharing.Reservation.Client/build" )

search_directory="./src/ParkSharing.Reservation.Server"

find "$search_directory" -type d \( -path "${exclude_paths[0]}" -o -path "${exclude_paths[1]}" -o -path "${exclude_paths[2]}" \) -prune -o -type f -name "*.cs" -print | while read -r file_path; do
    echo "--------------------------------------------------"
    echo "File: $file_path"
    echo "--------------------------------------------------"
    cat "$file_path"  # Use cat to print the file content
    echo  # Adds a newline for better readability between files
done

search_directory="./src/ParkSharing.Reservation.Client"

find "$search_directory" -type d \( -path "${exclude_paths[0]}" -o -path "${exclude_paths[1]}" -o -path "${exclude_paths[2]}" \) -prune -o -type f -name "*.css" -print | while read -r file_path; do
    echo "--------------------------------------------------"
    echo "File: $file_path"
    echo "--------------------------------------------------"
    cat "$file_path"  # Use cat to print the file content
    echo  # Adds a newline for better readability between files
done

find "$search_directory" -type d \( -path "${exclude_paths[0]}" -o -path "${exclude_paths[1]}" -o -path "${exclude_paths[2]}" \) -prune -o -type f -name "*.js" -print | while read -r file_path; do
    echo "--------------------------------------------------"
    echo "File: $file_path"
    echo "--------------------------------------------------"
    cat "$file_path"  # Use cat to print the file content
    echo  # Adds a newline for better readability between files
done

# echo "----------------------------"
# echo "File: src\ParkSharing.Reservation.Client\webpack.config.js"
# echo "----------------------------"
# cat ./src/ParkSharing.Reservation.Client/webpack.config.js
# echo

# echo "----------------------------"
# echo "File: src/ParkSharing.Reservation.Client/package.json"
# echo "----------------------------"
# cat ./src/ParkSharing.Reservation.Client/package.json
# echo
