# ParkSharing Notification Service

Welcome to the ParkSharing Notification Service, the unsung hero that quietly ensures users are notified about their precious parking spot reservations. Because what's the point of reserving a spot if you don’t get a fancy email about it?

## Overview

This microservice is the backbone of our communication strategy, handling notifications via email whenever a reservation is created. It listens for events, fetches user information, and sends templated emails using the power of SMTP. All so users can know when and where their car will be gloriously parked.

## Features

- **Event-Driven Notifications:** Listens to reservation events and triggers email notifications. Because nothing says efficiency like over-complicating simple tasks.
- **Email Templates:** Supports basic templating. Currently, we have exactly one template—so versatile!
- **SMTP Integration:** Sends emails using your trusty SMTP server, assuming you’ve configured it correctly. Otherwise, good luck!

## Components

### NotificationConsumer
The `NotificationConsumer` is the heart of this service. It listens for `ReservationCreatedEvent`, fetches user details, and sends out the notifications.

### EmailClient
Handles the actual sending of emails. It uses SMTP, because who doesn’t love setting up email servers?

### EmailService
Manages email templates and sends out emails by replacing template placeholders with actual values. Simple, right?

### EmailTemplateService
This service manages email templates, with a current total of one template. Talk about minimalism!

### UserInfoService
Fetches user information from other services because this microservice is way too important to know these details on its own.

## Configuration

Make sure you have your SMTP server details configured correctly in the `EmailConfig` class. You'll need:

- `SmtpServer`: The server address.
- `SmtpPort`: The port to connect to.
- `SenderEmail`: The email address from which notifications will be sent.
- `SenderPassword`: The password for the sender's email account.

Without these, well, you’re out of luck.

## Running the Service

1. **Build and Run**: Standard .NET commands apply. If you're here, you probably already know them. If not, Google is your friend.
2. **Configure SMTP**: Don’t forget to set up your SMTP server credentials in your configuration.
3. **Sit Back and Relax**: Once the service is up and running, it will dutifully send out emails whenever a reservation is made. Probably.

## Logging

The service logs some basic information, including when it starts and what environment it's running in. If it crashes, you'll know where to look—probably in a log file somewhere.

## Conclusion

Congratulations, you’ve made it through the documentation of the most important notification service in this entire parking ecosystem. Without it, users might forget they’ve reserved a parking spot, and that would just be tragic. Happy parking! 🚗📩