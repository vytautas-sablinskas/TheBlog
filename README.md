# TheBlog

Welcome to TheBlog, a dynamic blogging platform where users can write articles, post comments, and engage with content through ratings. Built on .NET 7, this application offers a sleek user interface with React and robust back-end capabilities using Entity Framework Core and a RESTful API.

## Features

- **Article and Comment Posting**: Users can write and publish articles and comments.
- **Ratings**: Articles can be rated, with the most popular ones featured prominently.
- **Home Page Summaries**: Summaries of the most recent and top-rated articles are displayed on the home page.
- **Role-Based Access Control**: Enhanced security with role-based permissions for creating, editing, and deleting content.
- **Authentication**: Secure login system using JWT tokens with a refresh strategy, specifically employed for API interactions.
- **Comment Moderation**: Administrators can block inappropriate comments, which will enter a 'blocked' state but remain in the system for review.
- **Reporting System**: Users can report comments for review by administrators.
- **Accessibility for Non-Users**: Non-registered visitors can view articles and comments.

## Project Structure

- **TheBlog.MVC**: The heart of the application, containing all primary logic. It integrates React within its MVC architecture for front-end interactivity, with JavaScript enabling dynamic content handling. Starting page is rendered by MVC app and then React takes over navigation and logic of front end.
- **TheBlog.Data**: This component houses extensive files shared between the API and MVC application. The API, featuring JWT authentication, is primarily designed for training purposes, demonstrating robust API implementation. It manages articles and related data but does not drive the main application logic.
- **THeBlog.API**: This component is remake of Articles controller with authentication using JWT tokens.
- **TheBlog.UnitTests**: This component has unit tests for main logic of the application using Moq framework.

## Technologies Used

- **.NET 7**: The core framework for building the application.
- **Entity Framework Core**: Utilized for a "Code First" approach in database creation and management.
- **React**: Powers the front-end user interface within the MVC structure.
- **JavaScript**: For dynamic client-side scripting.
- **JWT Authentication**: Provides secure API access, mainly for educational and demonstration purposes.
