# Course Management System API

A RESTful Web API built with ASP.NET Core for managing courses, instructors, students, and enrollments. This API implements JWT authentication, role-based authorization, and follows best practices for entity relationships and data management.

## Table of Contents

- [Technologies Used](#technologies-used)
- [Features](#features)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)
- [Authentication](#authentication)
- [Authorization](#authorization)
- [Database Schema](#database-schema)
- [HTTP-only Cookies for Authentication Security](#http-only-cookies-for-authentication-security)
- [Screenshots](#screenshots)

## Technologies Used

### Core Framework
- **ASP.NET Core 7.0** - A cross-platform, high-performance framework for building modern cloud-based web applications. It provides the foundation for building RESTful APIs with built-in support for dependency injection, middleware, and routing.

### ORM & Database
- **Entity Framework Core 7.0** - A modern object-relational mapper (ORM) that enables .NET developers to work with databases using .NET objects. It eliminates the need for most data-access code and provides features like LINQ queries, change tracking, and migrations.

- **SQLite** - A lightweight, serverless, self-contained SQL database engine. Perfect for development and small-scale deployments, requiring no separate database server installation.

### Authentication & Security
- **JWT (JSON Web Tokens)** - An open standard (RFC 7519) for securely transmitting information between parties as a JSON object. Used for stateless authentication where the server doesn't need to store session information.

- **BCrypt.Net** - A password hashing library that implements the bcrypt algorithm. It provides secure one-way hashing with automatic salt generation, protecting user passwords even if the database is compromised.

### API Documentation
- **Swagger/OpenAPI (Swashbuckle)** - An open-source framework for documenting RESTful APIs. It generates interactive documentation that allows developers to test endpoints directly from the browser.

### Design Patterns & Principles
- **Dependency Injection** - Built-in IoC (Inversion of Control) container for managing service lifetimes and dependencies.
- **Repository/Service Pattern** - Clean separation between data access and business logic.
- **DTOs (Data Transfer Objects)** - Objects used to transfer data between layers, preventing exposure of entity models.

## Features

- ✅ **Entity Relationships**
  - One-to-One: Instructor ↔ InstructorProfile
  - One-to-Many: Instructor → Courses
  - Many-to-Many: Student ↔ Course (via Enrollment)

- ✅ **JWT Authentication** - Secure token-based authentication
- ✅ **Role-based Authorization** - Admin, Instructor, Student roles
- ✅ **DTO Validation** - Request validation using Data Annotations
- ✅ **LINQ Optimizations** - Select() projections for efficient queries
- ✅ **AsNoTracking()** - Optimized read-only queries
- ✅ **Async Operations** - Non-blocking database calls
- ✅ **Swagger Documentation** - Interactive API documentation

## Getting Started

### Prerequisites

- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0) or later
- Any code editor (Visual Studio, VS Code, Rider)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd CourseManagementAPI
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Apply database migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access the API**
   - Swagger UI: `http://localhost:5000/swagger`
   - API Base URL: `http://localhost:5000/api`

### Default Admin User

The database is seeded with an admin user:
- **Username:** admin
- **Password:** Admin@123
- **Role:** Admin

## API Endpoints

### Authentication
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/login` | Login and get JWT token | No |
| POST | `/api/auth/register` | Register new user | No |

### Instructors
| Method | Endpoint | Description | Auth Required | Roles |
|--------|----------|-------------|---------------|-------|
| GET | `/api/instructors` | Get all instructors | Yes | Any |
| GET | `/api/instructors/{id}` | Get instructor by ID | Yes | Any |
| POST | `/api/instructors` | Create instructor | Yes | Admin |
| PUT | `/api/instructors/{id}` | Update instructor | Yes | Admin, Instructor |
| DELETE | `/api/instructors/{id}` | Delete instructor | Yes | Admin |
| GET | `/api/instructors/{id}/profile` | Get instructor profile | Yes | Any |
| PUT | `/api/instructors/{id}/profile` | Update instructor profile | Yes | Admin, Instructor |

### Students
| Method | Endpoint | Description | Auth Required | Roles |
|--------|----------|-------------|---------------|-------|
| GET | `/api/students` | Get all students | Yes | Admin, Instructor |
| GET | `/api/students/{id}` | Get student by ID | Yes | Any |
| POST | `/api/students` | Create student | Yes | Admin |
| PUT | `/api/students/{id}` | Update student | Yes | Admin, Student |
| DELETE | `/api/students/{id}` | Delete student | Yes | Admin |
| GET | `/api/students/{id}/enrollments` | Get student enrollments | Yes | Any |

### Courses
| Method | Endpoint | Description | Auth Required | Roles |
|--------|----------|-------------|---------------|-------|
| GET | `/api/courses` | Get all courses | No | - |
| GET | `/api/courses/{id}` | Get course by ID | No | - |
| POST | `/api/courses` | Create course | Yes | Admin, Instructor |
| PUT | `/api/courses/{id}` | Update course | Yes | Admin, Instructor |
| DELETE | `/api/courses/{id}` | Delete course | Yes | Admin |
| GET | `/api/courses/instructor/{id}` | Get courses by instructor | No | - |
| GET | `/api/courses/{id}/students` | Get enrolled students | Yes | Admin, Instructor |

### Enrollments
| Method | Endpoint | Description | Auth Required | Roles |
|--------|----------|-------------|---------------|-------|
| GET | `/api/enrollments` | Get all enrollments | Yes | Admin, Instructor |
| GET | `/api/enrollments/{id}` | Get enrollment by ID | Yes | Any |
| POST | `/api/enrollments` | Enroll student in course | Yes | Admin, Student |
| PUT | `/api/enrollments/{id}` | Update enrollment (grades) | Yes | Admin, Instructor |
| DELETE | `/api/enrollments/{id}` | Delete enrollment | Yes | Admin, Student |

## Authentication

### Login Request
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin@123"
}
```

### Login Response
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2026-04-02T19:00:00Z",
  "user": {
    "id": 1,
    "username": "admin",
    "email": "admin@coursemanagement.com",
    "role": "Admin"
  }
}
```

### Using the Token
Include the JWT token in the Authorization header:
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Authorization

The API implements role-based access control with three roles:

| Role | Permissions |
|------|-------------|
| **Admin** | Full access to all endpoints |
| **Instructor** | Can manage courses, view students, update enrollments |
| **Student** | Can view courses, enroll/unenroll, view own data |

## Database Schema

### Entity Relationship Diagram

```
┌─────────────────┐     1:1     ┌──────────────────────┐
│   Instructor    │────────────│   InstructorProfile  │
├─────────────────┤            ├──────────────────────┤
│ Id              │            │ Id                   │
│ FirstName       │            │ InstructorId (FK)    │
│ LastName        │            │ Bio                  │
│ Email           │            │ Education            │
│ Department      │            │ Specialization       │
│ HireDate        │            │ OfficeLocation       │
└────────┬────────┘            └──────────────────────┘
         │
         │ 1:N
         ▼
┌─────────────────┐
│     Course      │
├─────────────────┤
│ Id              │
│ CourseCode      │
│ Title           │
│ Credits         │
│ InstructorId(FK)│
└────────┬────────┘
         │
         │ N:M (via Enrollment)
         ▼
┌─────────────────┐            ┌─────────────────┐
│   Enrollment    │────────────│    Student      │
├─────────────────┤            ├─────────────────┤
│ Id              │            │ Id              │
│ StudentId (FK)  │            │ FirstName       │
│ CourseId (FK)   │            │ LastName        │
│ EnrollmentDate  │            │ Email           │
│ Status          │            │ StudentNumber   │
│ Grade           │            │ Major           │
└─────────────────┘            └─────────────────┘
```

## HTTP-only Cookies for Authentication Security

### Why HTTP-only Cookies are an Industry Standard

HTTP-only cookies are widely considered a security best practice for storing authentication tokens (like JWTs or session IDs) for the following reasons:

1. **Protection Against XSS Attacks**
   - JavaScript cannot access HTTP-only cookies (the `HttpOnly` flag prevents this)
   - Even if an attacker injects malicious JavaScript into your page, they cannot steal the authentication token
   - This is the primary security benefit over storing tokens in localStorage or regular cookies

2. **Automatic Transmission**
   - Cookies are automatically sent with every request to the same domain
   - Reduces the risk of developers forgetting to include the token in requests
   - Simplifies client-side code

3. **Secure Flag Support**
   - Can be combined with the `Secure` flag to ensure cookies are only sent over HTTPS
   - Prevents man-in-the-middle attacks from intercepting tokens

4. **SameSite Protection**
   - Modern browsers support the `SameSite` attribute
   - Provides protection against CSRF (Cross-Site Request Forgery) attacks
   - Options: `Strict`, `Lax`, or `None`

5. **Server-Side Control**
   - Server has full control over cookie lifetime and renewal
   - Can implement sliding expiration
   - Can invalidate tokens server-side immediately

### Best Practices for Implementation

```csharp
// Example cookie configuration
services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;           // Prevent JavaScript access
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // HTTPS only
    options.Cookie.SameSite = SameSiteMode.Strict;  // CSRF protection
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;
});
```

### Trade-offs

While HTTP-only cookies are secure, they have some considerations:
- **Not suitable for mobile apps** - Mobile applications typically use Authorization headers
- **CORS complications** - Requires careful CORS configuration for cross-origin requests
- **Size limitations** - Cookies have size limits (~4KB)

For this API, we use **Bearer tokens** in the Authorization header as it's more flexible for various clients (web, mobile, desktop). However, for web-only applications, HTTP-only cookies with refresh tokens would be the recommended approach.

## Project Structure

```
CourseManagementAPI/
├── Controllers/
│   ├── AuthController.cs
│   ├── InstructorsController.cs
│   ├── StudentsController.cs
│   ├── CoursesController.cs
│   └── EnrollmentsController.cs
├── Data/
│   └── ApplicationDbContext.cs
├── DTOs/
│   ├── UserDTOs.cs
│   ├── InstructorDTOs.cs
│   ├── InstructorProfileDTOs.cs
│   ├── StudentDTOs.cs
│   ├── CourseDTOs.cs
│   └── EnrollmentDTOs.cs
├── Models/
│   ├── User.cs
│   ├── Instructor.cs
│   ├── InstructorProfile.cs
│   ├── Student.cs
│   ├── Course.cs
│   └── Enrollment.cs
├── Services/
│   ├── AuthService.cs
│   ├── InstructorService.cs
│   ├── StudentService.cs
│   ├── CourseService.cs
│   └── EnrollmentService.cs
├── Migrations/
├── Screenshots/
├── Program.cs
├── appsettings.json
└── README.md
```

## Screenshots

Screenshots demonstrating the working API endpoints are available in the `Screenshots/` folder:

1. **Swagger Overview** - All available API endpoints
2. **Login Success** - JWT token returned on successful authentication
3. **Authorize Token** - Adding Bearer token for authentication
4. **Get Courses** - Retrieving course data (200 OK response)
5. **Create Instructor** - Creating a new instructor (201 Created response)
6. **Validation Error** - HTTP 400 response with validation error messages

## License

This project is created for educational purposes as part of the Web Engineering course assignment.
