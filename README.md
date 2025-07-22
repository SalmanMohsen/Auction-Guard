# Auction Guard: Real Estate Auction Platform

Auction Guard is a comprehensive real estate auction platform designed as a 4th-year project. It facilitates user account management, property listings with detailed views, and real-time bidding functionalities. The platform incorporates secure electronic payments, including cryptocurrencies, a robust notification system, and a transparent rating and review mechanism.

## About The Project

This project provides a secure, reliable, and user-friendly environment for real estate auctions. It caters to three main user roles: **Administrators**, **Sellers**, and **Bidders**, each with a specific set of permissions. The platform is built with a clean, layered architecture and modern technologies to ensure scalability and maintainability.

### Key Features

* [cite_start]**User Management**[cite: 3]:
    * [cite_start]Secure user registration with email or phone number[cite: 5].
    * [cite_start]JWT-based authentication for secure access[cite: 6].
    * Role-based access control (Admin, Seller, Bidder) with granular permissions.
    * [cite_start]User profile management, including personal information updates[cite: 8].
    * [cite_start]Password recovery via email[cite: 9].
    * [cite_start]Account deletion[cite: 10].
    * [cite_start]Admin functionality to blacklist users[cite: 70].

* [cite_start]**Property Listings**[cite: 11]:
    * [cite_start]Sellers can create, edit, and manage property listings with detailed information, including descriptions, images, and initial pricing[cite: 40, 43].
    * [cite_start]Admins can review and approve new property listings to ensure quality and compliance[cite: 34].
    * [cite_start]Advanced search and filtering options based on location, price, number of rooms, and property type[cite: 13].
    * [cite_start]Users can save properties to a favorites list for easy access[cite: 37].

* [cite_start]**Real-Time Auctions**[cite: 17]:
    * [cite_start]Sellers can schedule auctions with specific start and end times, minimum bid increments, and optional guarantee deposits[cite: 30, 31].
    * [cite_start]Real-time bidding with live updates on the current highest bid[cite: 25, 57].
    * [cite_start]Instant notifications for users when they are outbid, or when an auction they are interested in is starting or ending soon[cite: 18, 47, 48].
    * [cite_start]Sellers can create special offers that are triggered when the bid amount reaches a certain threshold[cite: 29].

* [cite_start]**Secure Payments & Invoicing**[cite: 44]:
    * Secure payment processing for auction winners, with support for traditional payment methods and cryptocurrencies.
    * [cite_start]Automated invoicing system that generates and sends invoices to auction winners[cite: 68].
    * Secure handling of financial transactions and payment authorizations.

* [cite_start]**Ratings & Reviews**[cite: 50]:
    * [cite_start]Users can rate and review sellers and properties after an auction has ended, promoting transparency and trust[cite: 51].
    * [cite_start]Admins can moderate reviews to prevent inappropriate content[cite: 53].

### Tech Stack

This project is built using a modern technology stack for both the backend and frontend.

* **Backend**:
    * .NET 8
    * ASP.NET Core Web API
    * Entity Framework Core 8
    * Microsoft SQL Server
    * ASP.NET Core Identity for authentication and authorization
    * JWT Bearer Tokens for secure API communication

* **Frontend**:
    * React 19
    * TypeScript
    * Vite
    * Tailwind

### Project Structure

The project follows a clean, layered architecture to ensure a separation of concerns and maintainability.

* **`AuctionGuard.Domain`**: Contains the core domain entities and business logic.
* **`AuctionGuard.Application`**: Implements the application logic, services, and DTOs.
* **`AuctionGuard.Infrastructure`**: Handles data access, database migrations, and external service integrations.
* **`AuctionGuard.API`**: Exposes the RESTful API endpoints for the frontend to consume.
* **`AuctionGuardClient`**: The React-based frontend application.

## Getting Started

To get a local copy up and running, follow these simple steps.

### Prerequisites

* .NET 8 SDK
* Node.js and npm
* Microsoft SQL Server

### Installation

1.  **Clone the repo**
    ```sh
    git clone [https://github.com/your_username/Auction-Guard.git](https://github.com/your_username/Auction-Guard.git)
    ```
2.  **Backend Setup**
    * Navigate to the `AuctionGuard` directory.
    * Update the connection strings in `AuctionGuard/AuctionGuard.API/appsettings.json`.
    * Apply the database migrations:
        ```sh
        dotnet ef database update --project AuctionGuard/AuctionGuard.Infrastructure
        ```
    * Run the API:
        ```sh
        dotnet run --project AuctionGuard/AuctionGuard.API
        ```
3.  **Frontend Setup**
    * Navigate to the `AuctionGuardClient` directory.
    * Install NPM packages:
        ```sh
        npm install
        ```
    * Start the development server:
        ```sh
        npm run dev
        ```

## License

Distributed under the MIT License. See `LICENSE` for more information.

## Contact

Salman Mohsen - [salman.mohsen88@example.com](mailto:salman.mohsen88@example.com)

Project Link: [https://github.com/salmanmohsen/auction-guard](https://github.com/salmanmohsen/auction-guard)
