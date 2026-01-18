# FitnessManager 🏋️‍♂️

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Language: C#](https://img.shields.io/badge/Language-C%23-239120.svg?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Framework: .NET Core](https://img.shields.io/badge/Framework-.NET%20Core-512BD4.svg?logo=dotnet)](https://dotnet.microsoft.com/)

A console-based fitness club management system built with **C#** and **.NET Core**. This application simulates core gym operations including user management, role-based access control, equipment tracking, and personal trainer bookings, using text files for data persistence.

## 📖 Table of Contents
- [Project Overview](#project-overview)
- [Key Features](#key-features)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [User Roles & Permissions](#user-roles--permissions)
- [Installation & Setup](#installation--setup)
- [Usage Guide](#usage-guide)
- [Application Architecture](#application-architecture)
- [Development Roadmap](#development-roadmap)
- [Documentation](#documentation)
- [License](#license)
- [Contact](#contact)

## 🎯 Project Overview
**FitnessManager** is a console application designed to simulate the management of a fitness club. The system enables handling of basic gym processes such as user registration/login, role assignment, equipment management, and trainer-client assignments.

**Core Objectives:**
- Provide a simulated environment for fitness club management operations
- Implement Role-Based Access Control (RBAC) for different user types
- Demonstrate object-oriented programming principles in C#
- Offer a foundation for future expansion with database integration and GUI

**Target Audience:**
- Gym administrators and staff
- Personal trainers
- Club members (clients)
- Developers learning C# and system design patterns

## ✨ Key Features
- **User Authentication**: Secure registration and login system with SHA256 password hashing
- **Role-Based Access Control (RBAC)**: Four distinct user roles with specific permissions
- **Equipment Management**: Add, remove, and browse gym machines and equipment
- **Trainer-Client System**: Members can book and cancel personal trainer services
- **File-Based Data Persistence**: Uses text files (`users.txt`, `logs.txt`) for data storage
- **Activity Logging**: All user actions are recorded in log files for audit trails
- **Console Interface**: Clean, role-specific menus for different user types

## 🛠️ Technology Stack
- **Programming Language**: C#
- **Framework**: .NET Core 3.1 or newer
- **Development Environment**: Visual Studio
- **Key Libraries**:
  - `System.IO` - File system operations
  - `System.Security.Cryptography` - Password hashing with SHA256
- **Data Storage**: Text files (no database dependency)


## 👥 User Roles & Permissions
The system implements a four-tier Role-Based Access Control (RBAC) model:

### 1. Administrator
**Example User:** `MainAdmin` | **Password:** `admin123`

**Permissions:**
- Full user management (view, add, remove users of all roles)
- Complete equipment control (add, remove, browse all machines)
- Password change for own account
- Logout capability

### 2. Worker (Gym Staff)
**Example User:** `Worker1` | **Password:** `worker123`

**Permissions:**
- Equipment management (add, browse, remove gym machines)
- Password change for own account
- Logout capability

### 3. Personal Trainer
**Example User:** `Trainer1` | **Password:** `trainer123`

**Permissions:**
- View list of assigned members/clients
- Change own password
- Logout capability

### 4. Member (Client)
**Example User:** `Member1` | **Password:** `member123`

**Permissions:**
- Browse available personal trainers
- Purchase trainer service (assign self to trainer)
- Cancel trainer service (remove assignment)
- Change own password
- Logout capability

## 🚀 Installation & Setup

### Prerequisites
- **.NET Core 3.1** or newer
- **Visual Studio** (recommended) or **Visual Studio Code** with C# extensions

### Installation Steps
1. **Clone the repository:**
  ```bash
  git clone https://github.com/hasikmilosz/Fitness_System_Management.git
  cd Fitness_System_Management
  ```

2. **Open in Visual Studio:**
- Open `FitnessManager.sln` in Visual Studio
- The solution should load with all projects

3. **Build and run:**
- Press `Ctrl+F5` (Start Without Debugging) or `F5` (Start Debugging)
- Alternatively, use the command line:
  ```
  dotnet build
  dotnet run
  ```

## 📱 Usage Guide

### First-Time Setup
1. Run the application for the first time
2. The system will create necessary text files in the `Bin/Debug/` directory
3. Default users are created as per the documentation in `role.pdf`

### Typical User Flow
1. **Launch the application** - Console interface appears
2. **Register or Login** - New users can register; existing users login with credentials
3. **Role-Specific Menu** - Based on your role (Admin, Worker, Trainer, Member), you'll see different options
4. **Perform Actions** - Use the menu to perform allowed operations
5. **Logout** - Safely end your session

### Example Use Case Scenarios

**Administrator:**
- Log in as `MainAdmin` with password `admin123`
- View all registered users across different roles
- Add new equipment to the gym
- Remove inactive members
- Change your own password

**Member:**
- Log in as `Member1` with password `member123`
- Browse available personal trainers
- Purchase a trainer service to get assigned
- Later, cancel the service if needed
- Change your password for security

**Worker:**
- Log in as `Worker1` with password `worker123`
- Add new gym machines that arrive
- Remove broken or decommissioned equipment
- Browse the current equipment list

## 🏗️ Application Architecture

### Core Design Patterns
- **Object-Oriented Design**: Inheritance hierarchy with `User` as abstract base class
- **Role-Based Access Control (RBAC)**: Permission system tied to user roles
- **Single Responsibility Principle**: Separate classes for users, file management, and menus
- **Repository Pattern (simplified)**: `FileManager` handles all data persistence

### Data Flow
Console Input → Menu System → RBAC Check → Business Logic → FileManager → Text Files → Activity Logging

### Key Classes Overview
- **`User` (Abstract)**: Base class with `UserName`, `Password` (hashed), `Role` properties
- **`Admin`/`Worker`/`PersonalTrainer`/`Member`**: Concrete user types with specific functionalities
- **`RBAC`**: Maps roles to permissible actions within the system
- **`FileManager`**: Handles user authentication, registration, and activity logging to files

### Data Storage
- **`users.txt`**: Stores user credentials (username, hashed password, role)
- **`logs.txt`**: Records all user actions with timestamps for auditing
- **Text-based approach**: Provides simplicity and transparency for this educational project

## 🔮 Development Roadmap

### Current Limitations
- Console-based interface only (no GUI)
- Text file storage (not scalable for large datasets)
- No unit testing suite
- Basic error handling with console messages

### Planned Enhancements

#### Short-term Goals
1. **Graphical User Interface (GUI)**
   - Windows Forms or WPF application
   - More intuitive user experience
   - Visual representation of data

2. **Database Integration**
   - SQLite for lightweight local storage
   - Entity Framework for data access
   - Improved data integrity and querying

#### Medium-term Goals
3. **Extended Features**
   - Class scheduling and reservations
   - Payment processing simulation
   - Membership tier system
   - Equipment maintenance tracking

4. **Advanced Architecture**
   - REST API layer for multi-client support
   - Dependency injection for better testability
   - Comprehensive unit test coverage

#### Long-term Vision
5. **Modernization**
   - Web-based interface (ASP.NET Core)
   - Mobile companion app
   - Cloud deployment options
   - Integration with fitness wearables

## 📚 Documentation
Comprehensive documentation is available in the `Documentation/` folder:

- **`dokumentacja.pdf`**: Complete system documentation (in Polish) including:
  - Project title and description
  - Technology stack details
  - Directory structure explanation
  - Installation and running instructions
  - Application functionality overview
  - Data structures and classes
  - Error handling approach
  - Testing methodology
  - Current limitations and future plans

- **`role.pdf`**: User role specifications including:
  - Detailed permission breakdown for each role
  - Example users and passwords for testing
  - Role-specific capabilities and restrictions

## 📜 License
This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

### Portfolio Note
This repository serves as a demonstration project for my portfolio. It showcases my ability to design and implement a functional system with:
- Object-oriented design principles in C#
- Role-Based Access Control implementation
- File-based data persistence
- Console application development
- System architecture planning

You are welcome to:
- **Explore and learn** from the code structure and design patterns
- **Use as a foundation** for your own projects (in accordance with the MIT License)
- **Fork and modify** for educational purposes

## 📧 Contact
Miłosz Hasik – [hasikmilosz@gmail.com](mailto:hasikmilosz@gmail.com)

---
*Thank you for checking out FitnessManager! This project represents a practical implementation of fitness club management concepts using C# and .NET Core.*
