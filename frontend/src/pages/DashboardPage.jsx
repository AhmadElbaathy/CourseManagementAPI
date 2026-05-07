import { Link } from 'react-router-dom';

function DashboardPage({ user }) {
  return (
    <div className="dashboard-page">
      <h1>Center Baathy Elt3lemy</h1>
      <p className="dashboard-welcome">
        Welcome back, <strong>{user?.username}</strong>! Use the navigation below to manage your course data.
      </p>
      <div className="dashboard-grid">
        <Link to="/courses" className="dashboard-card">
          <div className="card-icon">📚</div>
          <h2>Courses</h2>
          <p>View, add, edit, and delete courses</p>
        </Link>
        <Link to="/students" className="dashboard-card">
          <div className="card-icon">🎓</div>
          <h2>Students</h2>
          <p>View, add, edit, and delete students</p>
        </Link>
        <Link to="/instructors" className="dashboard-card">
          <div className="card-icon">👨‍🏫</div>
          <h2>Instructors</h2>
          <p>View, add, edit, and delete instructors</p>
        </Link>
        <Link to="/enrollments" className="dashboard-card">
          <div className="card-icon">📝</div>
          <h2>Enrollments</h2>
          <p>View, add, edit, and delete enrollments</p>
        </Link>
      </div>
    </div>
  );
}

export default DashboardPage;
