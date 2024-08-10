# ParkSharing Aspire Application

Welcome to the ParkSharing Aspire Application! This project is the core of our local development setup, providing a seamless experience for developing, testing, and running our microservices-based parking management system. Below you'll find everything you need to get started with running this application locally.

## Prerequisites

Before you dive into running this application, ensure you have the following installed on your local machine:

- **Docker**: Required for containerizing and running all services.
- **.NET SDK**: To build and run the .NET projects if not using Docker.
- **Node.js & npm**: Required for building and running the frontend applications.

## Overview

This Aspire setup configures and orchestrates several services and clients:

- **Admin Server**: Manages administrative tasks and integrates with Auth0 for authentication.
- **Notification Server**: Handles email notifications.
- **Reservation Server**: Manages parking reservations.
- **Admin Client**: The frontend for administrators.
- **Reservation Client**: The frontend for users making reservations.
- **MongoDB**: The database where all the data lives.
- **RabbitMQ**: The message broker for inter-service communication.

## Running the Application

Follow these steps to get the application up and running:

### 1. Clone the Repository

First, clone the repository to your local machine:

```bash
git clone https://github.com/your-repo/parksharing.git
cd parksharing
```

### 2. Build and Run with Docker

All services and clients are configured to run within Docker containers. To build and run the entire application stack, use the following command:

```bash
docker-compose up --build
```

This will:

- Build the Docker images for the Admin and Reservation Clients.
- Start the MongoDB and RabbitMQ containers.
- Run all the backend services (Admin Server, Reservation Server, Notification Server).
- Expose the necessary ports for each service.

### 3. Access the Application

Once everything is up and running, you can access the application via the following URLs:

- **Admin Client**: [http://localhost:4225](http://localhost:4225)
- **Reservation Client**: [http://localhost:4224](http://localhost:4224)

### 4. Configuration

The application uses several environment variables to configure the services. These are set in the `docker-compose.yml` file and include:

- **MongoDB Configuration**: 
  - `MONGO_INITDB_ROOT_USERNAME`
  - `MONGO_INITDB_ROOT_PASSWORD`
- **RabbitMQ Configuration**: 
  - `RABBITMQ_DEFAULT_USER`
  - `RABBITMQ_DEFAULT_PASS`
- **Auth0 Integration**: 
  - `AUTH0_DOMAIN`
  - `AUTH0_AUDIENCE`
  - `AUTH0_CLIENT_ID`
  - `AUTH0_CALLBACK_URL`

### 5. Customize and Extend

The Aspire framework allows for easy extension and customization of the services. You can modify the `ParkSharing.AppHost` configuration to add new services, change environment variables, or update Docker configurations.

## Additional Information

- **Development**: For local development, you can run services individually outside of Docker using Visual Studio or your preferred IDE. Make sure to start MongoDB and RabbitMQ containers separately if you do this.
- **Debugging**: The setup supports debug builds with additional logging enabled. You can configure this within your IDE or directly in the `docker-compose.yml` file.

## Conclusion

With this setup, you should have a fully operational local environment for the ParkSharing application. Docker makes it easy to manage dependencies, ensuring consistency across all developers' machines. If you run into any issues, check the logs in Docker and ensure all services are running as expected. Happy coding! 🚀