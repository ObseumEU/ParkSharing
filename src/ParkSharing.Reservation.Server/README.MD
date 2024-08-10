# ParkSharing Reservation Client

Welcome to the ParkSharing Reservation Client! 🎉 This project is your one-stop-shop for managing parking spots, making reservations, and deploying a fancy chat widget that will likely dazzle users with its gradients and stylish animations.

## Table of Contents

- [Overview](#overview)
- [Setup](#setup)
- [Usage](#usage)
- [File Structure](#file-structure)
- [Contributing](#contributing)
- [License](#license)

## Overview

This microservice is an essential part of the ParkSharing ecosystem. It comes with everything you need to help users reserve parking spots and manage their availability, all wrapped in a sleek UI. The star of the show here is our "CustomChat" component—because who doesn't want a chatbot with gradients?

## Setup

So you want to set this up locally? Brave soul. Here’s what you’ll need to do:

1. **Clone the repository:**
    ```bash
    git clone https://github.com/YourUserName/ParkSharing-Reservation-Client.git
    cd ParkSharing-Reservation-Client
    ```

2. **Install dependencies:**
    ```bash
    npm install
    ```

3. **Run the development server:**
    ```bash
    npm start
    ```
   The app will be running on `http://localhost:4001`. Feel free to poke around, but don’t say we didn’t warn you about the gradients.

## Usage

Once you’ve got it running, you’ll be greeted by a rather enthusiastic chat widget that can handle parking spot reservations like a pro. The app also features an **OwnerPage** where parking spot owners can manage their spots and availability.

## File Structure

Here’s a quick overview of the chaos:

- **`src/`**: Where the magic happens.
  - **`App.js`**: Main entry point for our app. If this doesn’t work, nothing will.
  - **`CustomChat.js`**: The shining star of this project. This chat widget will interact with users in a way you never thought possible.
  - **`OwnerPage.js`**: Allows parking spot owners to manage their empire of asphalt.
  - **`index.js`**: Renders the app into the DOM. Spoiler: it’s important.
  - **`index.css`**: Global styles, because we need everything to look just right.
  
- **`public/`**: Publicly accessible files.
  - **`index.html`**: The shell of the app. Try not to mess with it too much.
  
- **`webpack.config.js`**: The configuration that makes all of this possible. If something breaks, start here.

## Contributing

Want to contribute? Good luck! Open a pull request, and we’ll see if your code is up to the task of making our chat widget even more fabulous.

## License

This project is licensed under the MIT License—because, let’s face it, you can’t own a color gradient.

Happy coding! 🌈🚗
