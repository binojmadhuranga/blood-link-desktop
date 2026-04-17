# Blood Donation Management System 🩸

A desktop-based Blood Donation Management System developed using **WPF (.NET 8)** to manage blood donors, blood requests, hospitals, and donation records efficiently.

## 📌 Project Overview

The Blood Donation Management System is designed to digitize and improve the traditional blood donation process by providing a centralized platform for managing donor information, blood requests, and blood availability.

The system helps hospitals, administrators, and blood donors manage critical blood donation operations in a structured and efficient way.

## 🚀 Features

* Donor registration and management
* Blood request creation and tracking
* Blood group matching
* Hospital information management
* Donation history records
* Admin dashboard
* Search donors by blood group and location
* Inventory monitoring

## 🛠️ Technology Stack

* **Language:** C#
* **Framework:** WPF (.NET 8)
* **Architecture:** MVVM
* **Database:** SQLite / SQL Server
* **IDE:** JetBrains Rider

## 📂 Project Structure

```text
BloodDonationManagementSystem
│── Models
│── Views
│── ViewModels
│── Services
│── Data
│── Assets
│── App.xaml
│── MainWindow.xaml
```

## 🧱 Architecture

This project follows the **MVVM (Model-View-ViewModel)** architecture:

* **Models** → Data structures
* **Views** → User interface screens
* **ViewModels** → Application logic and binding
* **Services** → Business logic and helper services
* **Data** → Database connection and data access

## ⚙️ Modules

### Donor Management

* Add donor
* Edit donor
* Delete donor
* View donor details

### Blood Request Management

* Create blood request
* Match blood type with available donors

### Hospital Management

* Manage hospital records
* Handle emergency requests

### Admin Panel

* Monitor all records
* Generate reports

## 🗄️ Database Entities

* Donor
* BloodRequest
* Hospital
* DonationRecord
* Admin

## ▶️ Getting Started

### Clone Repository

```bash
git clone <repository-url>
```

### Open Project

Open the solution in JetBrains Rider.

### Run Application

Build and run the project using:

```bash
dotnet run
```

## 📈 Future Improvements

* User authentication
* Email notifications
* Report export
* Blood stock analytics

## 📚 Purpose

This project is developed as an academic system to improve blood donation management through desktop application technology.

## 👨‍💻 Author

Developed by Binoj
