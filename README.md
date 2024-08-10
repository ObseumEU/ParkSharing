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

## Setup and Running (Good Luck)

If you're brave enough to attempt running this:

### 1. Clone the Repository

```bash
git clone https://github.com/your-repo/parksharing.git
cd parksharing
```

### 2. Build and Run with Docker

```bash
docker-compose up --build
```

This will:

- Build Docker images (because we love making things complicated).
- Start MongoDB and RabbitMQ (because SQL was too easy).
- Run all backend services (so you can witness the chaos in action).

### 3. Access the Application

If it actually works, you can find the apps here:

- **Admin Client**: [http://localhost:4225](http://localhost:4225)
- **Reservation Client**: [http://localhost:4224](http://localhost:4224)

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