import { Link, useNavigate } from 'react-router-dom';

function Navbar({ user, setUser }) {
  const navigate = useNavigate();

  const handleLogout = () => {
    document.cookie = 'token=; Max-Age=0; path=/';
    document.cookie = 'refreshToken=; Max-Age=0; path=/';
    document.cookie = 'user=; Max-Age=0; path=/';
    setUser(null);
    navigate('/login');
  };

  return (
    <nav className="navbar">
      <div className="navbar-brand">
        <Link to="/">Center Baathy Elt3lemy</Link>
      </div>
      {user && (
        <div className="navbar-links">
          <Link to="/courses">Courses</Link>
          <Link to="/students">Students</Link>
          <Link to="/instructors">Instructors</Link>
          <Link to="/enrollments">Enrollments</Link>
        </div>
      )}
      <div className="navbar-auth">
        {user ? (
          <>
            <span className="navbar-user">Welcome, {user.username}</span>
            <button onClick={handleLogout} className="btn btn-logout">Logout</button>
          </>
        ) : (
          <>
            <Link to="/login" className="btn btn-login">Login</Link>
            <Link to="/register" className="btn btn-register">Register</Link>
          </>
        )}
      </div>
    </nav>
  );
}

export default Navbar;
