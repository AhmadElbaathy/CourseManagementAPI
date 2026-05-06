import { useState, useEffect } from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Navbar from './components/Navbar';
import ProtectedRoute from './components/ProtectedRoute';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import DashboardPage from './pages/DashboardPage';
import CoursesListPage from './pages/CoursesListPage';
import CourseFormPage from './pages/CourseFormPage';
import StudentsListPage from './pages/StudentsListPage';
import StudentFormPage from './pages/StudentFormPage';
import InstructorsListPage from './pages/InstructorsListPage';
import InstructorFormPage from './pages/InstructorFormPage';
import EnrollmentsListPage from './pages/EnrollmentsListPage';
import EnrollmentFormPage from './pages/EnrollmentFormPage';
import './App.css';

function getCookie(name) {
  const match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'));
  return match ? decodeURIComponent(match[2]) : null;
}

function App() {
  const [user, setUser] = useState(null);

  // On mount, restore user from cookie
  useEffect(() => {
    const userCookie = getCookie('user');
    if (userCookie) {
      try {
        setUser(JSON.parse(userCookie));
      } catch {
        setUser(null);
      }
    }
  }, []);

  return (
    <BrowserRouter>
      <Navbar user={user} setUser={setUser} />
      <main className="main-content">
        <Routes>
          {/* Public routes */}
          <Route path="/login" element={<LoginPage setUser={setUser} />} />
          <Route path="/register" element={<RegisterPage />} />

          {/* Protected routes */}
          <Route element={<ProtectedRoute user={user} />}>
            <Route path="/" element={<DashboardPage user={user} />} />
            <Route path="/courses" element={<CoursesListPage user={user} />} />
            <Route path="/courses/new" element={<CourseFormPage user={user} />} />
            <Route path="/courses/:id" element={<CourseFormPage user={user} />} />
            <Route path="/students" element={<StudentsListPage user={user} />} />
            <Route path="/students/new" element={<StudentFormPage user={user} />} />
            <Route path="/students/:id" element={<StudentFormPage user={user} />} />
            <Route path="/instructors" element={<InstructorsListPage user={user} />} />
            <Route path="/instructors/new" element={<InstructorFormPage user={user} />} />
            <Route path="/instructors/:id" element={<InstructorFormPage user={user} />} />
            <Route path="/enrollments" element={<EnrollmentsListPage user={user} />} />
            <Route path="/enrollments/new" element={<EnrollmentFormPage user={user} />} />
            <Route path="/enrollments/:id" element={<EnrollmentFormPage user={user} />} />
          </Route>
        </Routes>
      </main>
    </BrowserRouter>
  );
}

export default App;
