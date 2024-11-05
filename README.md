# ParkSharing Application: Overengineering at Its Finest

Welcome to the **ParkSharing** Application, a true masterpiece of overengineering! This project is the pinnacle of complexity, created by someone who, let's be honest, has no clue how to develop a frontend with React and couldn't find good UX if it hit them in the face. But hey, it's the thought that counts, right?

## Table of Contents

- [Overview (If You Dare)](#overview-if-you-dare)
- [Prerequisites (Because Simple Just Isn't Enough)](#prerequisites-because-simple-just-isnt-enough)
- [Microservices (Because One Service is Never Enough)](#microservices-because-one-service-is-never-enough)
- [Setup and Running (Good Luck)](#setup-and-running-good-luck)
- [Environment Configuration (The Maze of Variables)](#environment-configuration-the-maze-of-variables)
- [Development and Debugging (Enjoy the Pain)](#development-and-debugging-enjoy-the-pain)
- [Contributing (Please Save Us)](#contributing-please-save-us)
- [License (Use at Your Own Risk)](#license-use-at-your-own-risk)

## Overview (If You Dare)

Behold, the **ParkSharing** Application—a marvel of unnecessary complexity! This project features not one, not two, but an entire collection of microservices, each one more convoluted than the last. It's all held together by a developer who can barely string together a React component and has the design sense of a blindfolded toddler with a crayon.

### The Brilliantly Complicated Setup Includes:

- **Admin Server**: Because we couldn't just manage parking spots simply. Oh no, we had to involve Auth0 and a ton of other unnecessary tech.
- **Reservation Server**: Managing reservations? Better add layers upon layers of complexity!
- **Notification Server**: Why just send an email when you can architect an entire microservice for it?
- **Admin Client**: A React frontend that's so basic, you'll wonder if it's from 2010. Spoiler: the UX is terrible.
- **Reservation Client**: Another React disaster, featuring CSS that's more experimental than functional.

## Prerequisites (Because Simple Just Isn't Enough)

Before you can even think about running this masterpiece, make sure your machine is loaded with:

- **Docker**: Because plain old code is too mainstream.
- **.NET SDK**: Needed if you dare to run the backend outside of Docker. Good luck with that.
- **Node.js & npm**: For the React apps that will make you question your life choices.

## Microservices (Because One Service is Never Enough)

Why have one simple service when you can overcomplicate everything with multiple microservices? Here's what you'll be dealing with:

### 1. Admin Server

The backend beast that tries (and often fails) to manage parking spots and user permissions with ASP.NET, MongoDB, and a dash of unnecessary complexity.

### 2. Reservation Server

The core of our parking reservation nightmare. Built with .NET, MongoDB, and MassTransit, because why not throw in some messaging chaos?

### 3. Notification Server

An entire service dedicated to sending emails. Yes, really. We couldn't just use an email API—we had to build a whole system around it.

### 4. Admin Client

A React frontend that's so basic, it hurts. It's tied together with the UX equivalent of duct tape and string.

### 5. Reservation Client

Another React experiment gone wrong. Features a custom chat interface that relies heavily on cookies because who needs a database?

To run the Aspire project located in the `/src/ParkSharing.AppHost` folder, follow these steps. This process will help you get the entire microservices-based application up and running locally, leveraging .NET Aspire’s orchestration capabilities:

## How run project ( Good luck )

### Prerequisites

Before starting, ensure you have the following installed on your system:
- **.NET SDK 8.0 or higher**
- **Docker Desktop** (or Podman if you prefer an alternative)
- **Node.js and npm** (for frontend applications)

### Addition for running our project 
- **Version of Node.js** must be higher than '20.13'
- when you finnaly have done all steps, make sure if your **Docker is running** before you start this app

### Step 1: Install .NET Aspire Workload

First, you need to install the .NET Aspire workload. Open your terminal and run:

```bash
dotnet workload install aspire
```

This command installs the necessary components and templates to work with .NET Aspire.

### Step 2: Build the Aspire Project

Navigate to the root of your Aspire project:

```bash
cd /src/ParkSharing.AppHost
```

Here, you’ll find the main orchestrator project that wires together all other services and dependencies.

### Step 3: Run the Aspire Application

To run the application, you can use the following command:

```bash
dotnet run --project ParkSharing.AppHost
```

This will launch the Aspire orchestrator, which will manage all the interconnected services, like MongoDB, RabbitMQ, and your various microservices (Admin Server, Reservation Server, Notification Server, etc.).

If you prefer to start the application using Docker, make sure Docker Desktop is running, and then use the following command to bring up the entire environment:

```bash
docker-compose up --build
```

This Docker command will spin up all necessary containers, ensuring that every service is correctly configured and running.

### Step 4: Access the Aspire Dashboard

Once the project is running, the Aspire dashboard will automatically be available. This dashboard is a powerful tool that lets you monitor all the components of your application, including logs, traces, and environment configurations.

If you started the application via the command line, you’ll need to manually open the dashboard URL printed in the terminal. If you’re using Visual Studio, it will open automatically in your browser.

### Additional Notes

- **Configuration**: You can adjust the environment variables and other configurations in the `docker-compose.yml` file or directly in your IDE.
- **Development**: For local development, you can run each service individually if needed. Ensure that MongoDB and RabbitMQ are running separately if you go this route.

By following these steps, you’ll have the entire ParkSharing Aspire application running locally, fully leveraging the powerful orchestration features of .NET Aspire.

If you encounter any issues, ensure that all dependencies are correctly installed, and refer to the logs provided in the Aspire dashboard for troubleshooting.

## Environment Configuration (The Maze of Variables)

This project wouldn't be complete without a labyrinth of environment variables. Set them all in the `docker-compose.yml` file, because why keep things simple?

- **MongoDB**:
  - `MONGO_INITDB_ROOT_USERNAME`: Because we all love typing long environment variable names.
  - `MONGO_INITDB_ROOT_PASSWORD`: Don't forget this one—or do, and watch everything break.
- **RabbitMQ**:
  - `RABBITMQ_DEFAULT_USER`: The default user you'll inevitably forget.
  - `RABBITMQ_DEFAULT_PASS`: The password you'll need to dig through your notes to find.
- **Auth0**:
  - `AUTH0_DOMAIN`: A domain to remind you that nothing here is straightforward.
  - `AUTH0_AUDIENCE`: Because why not add more complexity?
  - `AUTH0_CLIENT_ID`: Just another key to lose track of.
  - `AUTH0_CALLBACK_URL`: Set it, forget it, and then panic when things don't work.

## Development and Debugging (Enjoy the Pain)

For the masochists out there who want to develop or debug this, you can run the services individually. But don't say we didn't warn you:

- **MongoDB** and **RabbitMQ** containers need to be started separately, just to keep you on your toes.
- Debugging is enabled, but whether it actually helps is another story.

## Contributing (Please Save Us)

Think you can fix this mess? We welcome contributions—seriously, we need all the help we can get. Fork the repo, make your changes, and open a pull request. Maybe you can succeed where AI and lack of frontend skills failed.

## License (Use at Your Own Risk)

This project is licensed under the MIT License. Use it, break it, fix it—just don't blame us when everything falls apart.

---

Happy coding! 🚀 Or at least, good luck trying to make sense of this overengineered monstrosity.
