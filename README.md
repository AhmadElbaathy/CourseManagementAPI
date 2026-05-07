# Course Management System — Full Stack Web Application

## Application Description

This is a full-stack web engineering project consisting of:

- **Backend**: ASP.NET Core Web API for managing courses, instructors, students, and enrollments. Features JWT authentication with refresh tokens, fine-grained role-based authorization with ownership enforcement, Hangfire background jobs, and Swagger UI documentation. Built with Entity Framework Core and SQLite.

- **Frontend**: React single-page application that integrates with the backend API. Features user registration and login, cookie-based auth persistence, protected routes, role-aware UI rendering, and full CRUD operations for all data models. Built with React, React Router, and Axios.

---

## Tech Stack

| Layer      | Technology                                    |
|------------|-----------------------------------------------|
| Backend    | ASP.NET Core 8, Entity Framework Core, SQLite |
| Auth       | JWT Bearer Tokens + Refresh Tokens            |
| Background | Hangfire (Memory Storage)                     |
| Frontend   | React, React Router v6, Axios                 |
| API Docs   | Swagger / OpenAPI                             |

---

## Setup Instructions

### Prerequisites
- .NET 8 SDK (or later)
- Node.js (v16 or later) and npm

### Backend Setup

```bash
# Navigate to project root
cd CourseManagementAPI

# Restore dependencies
dotnet restore

# Run the backend (starts on http://localhost:5238)
dotnet run
```

The API will be available at `http://localhost:5238`.  
Swagger UI is available at `http://localhost:5238/swagger`.  
Hangfire Dashboard is available at `http://localhost:5238/hangfire`.

### Frontend Setup

```bash
# Navigate to frontend directory
cd CourseManagementAPI/frontend

# Install dependencies
npm install

# Start the development server (starts on http://localhost:3000)
npm start
```

The React app will open in your browser at `http://localhost:3000`.

> **Important:** Make sure the backend is running before starting the frontend.

---

## Role-Based Access Control (RBAC)

The system enforces three user roles with distinct permissions:

| Action                        | Admin | Instructor | Student |
|-------------------------------|:-----:|:----------:|:-------:|
| View courses (public)         | ✅    | ✅         | ✅      |
| Create course                 | ✅    | ❌         | ❌      |
| Update course (own only)      | ✅    | ✅ (own)   | ❌      |
| Delete course                 | ✅    | ❌         | ❌      |
| View all students             | ✅    | ✅         | ❌      |
| Create student                | ✅    | ❌         | ❌      |
| Update student (own only)     | ✅    | ❌         | ✅ (own)|
| Delete student                | ✅    | ❌         | ❌      |
| View all enrollments          | ✅    | ✅         | ❌      |
| Create enrollment (own only)  | ✅    | ❌         | ✅ (own)|
| Delete enrollment (own only)  | ✅    | ❌         | ✅ (own)|
| Manage instructor profiles    | ✅    | ✅ (own)   | ❌      |

**Ownership enforcement** is applied server-side via JWT claims (`InstructorId`, `StudentId`). Users attempting to modify resources they don't own receive a `403 Forbidden` response.

---

## API Routes

### Authentication
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/Auth/register` | Public | Register a new user (Admin/Instructor/Student) |
| POST | `/api/Auth/login` | Public | Login and receive JWT + refresh token |
| POST | `/api/Auth/refresh-token` | Public | Refresh an expired JWT token |
| POST | `/api/Auth/revoke-token` | Public | Revoke a refresh token (logout) |

### Courses
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/Courses` | Public | Get all courses |
| GET | `/api/Courses/{id}` | Public | Get a course by ID |
| POST | `/api/Courses` | Admin | Create a new course |
| PUT | `/api/Courses/{id}` | Admin / Instructor (own) | Update a course |
| DELETE | `/api/Courses/{id}` | Admin | Delete a course |
| GET | `/api/Courses/instructor/{instructorId}` | Public | Get courses by instructor |
| GET | `/api/Courses/{id}/students` | Admin / Instructor | Get enrolled students for a course |

### Students
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/Students` | Admin / Instructor | Get all students |
| GET | `/api/Students/{id}` | Authenticated | Get a student by ID |
| POST | `/api/Students` | Admin | Create a new student |
| PUT | `/api/Students/{id}` | Admin / Student (own) | Update a student |
| DELETE | `/api/Students/{id}` | Admin | Delete a student |
| GET | `/api/Students/{id}/enrollments` | Admin / Instructor / Student (own) | Get student's enrollments |

### Instructors
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/Instructors` | Authenticated | Get all instructors |
| GET | `/api/Instructors/{id}` | Authenticated | Get an instructor by ID |
| POST | `/api/Instructors` | Admin | Create a new instructor |
| PUT | `/api/Instructors/{id}` | Admin / Instructor (own) | Update an instructor |
| DELETE | `/api/Instructors/{id}` | Admin | Delete an instructor |
| GET | `/api/Instructors/{id}/profile` | Authenticated | Get instructor profile |
| PUT | `/api/Instructors/{id}/profile` | Admin / Instructor (own) | Create or update instructor profile |

### Enrollments
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/Enrollments` | Admin / Instructor | Get all enrollments |
| GET | `/api/Enrollments/{id}` | Authenticated | Get an enrollment by ID |
| POST | `/api/Enrollments` | Admin / Student (self-enroll) | Create a new enrollment |
| PUT | `/api/Enrollments/{id}` | Admin / Instructor / Student (own) | Update an enrollment |
| DELETE | `/api/Enrollments/{id}` | Admin / Student (own) | Delete an enrollment (unenroll) |

---

## Background Jobs (Hangfire)

Three recurring Hangfire jobs run automatically:

| Job | Schedule | Description |
|-----|----------|-------------|
| `cleanup-expired-tokens` | Daily (midnight) | Removes expired refresh tokens from the database |
| `daily-enrollment-report` | Daily (8 AM) | Generates a daily summary report of enrollments |
| `auto-complete-enrollments` | Weekly | Deactivates old/inactive enrollments |

The Hangfire Dashboard is accessible at `/hangfire` in the development environment.

---

## Screenshots

### Login Page
![Login Page](./Screenshots/login.png)

### Register Page
![Register Page](./Screenshots/register.png)

### Dashboard
![Dashboard](./Screenshots/dashboard.png)

### Courses Page
![Courses Page](./Screenshots/courses.png)

### Students Page
![Students Page](./Screenshots/students.png)

### Instructors Page
![Instructors Page](./Screenshots/instructors.png)

### Enrollments Page
![Enrollments Page](./Screenshots/enrollments.png)

---

## Submission Checklist

- [x] GitHub Repository link shared
- [x] README.md completed with all sections
- [x] Screenshots inserted in the Screenshots section above
- [x] Backend runs successfully (`dotnet run`)
- [x] Frontend runs successfully (`npm start`)
- [x] All CRUD operations work for every model
- [x] Login and Register work correctly
- [x] Cookie-based authentication persists across pages
- [x] Protected routes redirect to login when not authenticated
- [x] Role-based access control enforced on all protected endpoints
- [x] Ownership enforcement — users can only modify their own resources
- [x] Instructor profile management (create/update own profile)
- [x] Student self-enrollment with duplicate enrollment check
- [x] JWT refresh token flow implemented
- [x] Hangfire background jobs configured and running
- [x] Swagger UI available for API exploration
- [x] Laptop ready for live demo in the lab
