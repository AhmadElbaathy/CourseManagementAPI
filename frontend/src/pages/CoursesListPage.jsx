import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import courseService from '../services/courseService';

function CoursesListPage({ user }) {
  const [courses, setCourses] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const role = user?.role;
  const canCreate = role === 'Admin';
  const canEdit = role === 'Admin' || role === 'Instructor';
  const canDelete = role === 'Admin';

  useEffect(() => {
    fetchCourses();
  }, []);

  const fetchCourses = async () => {
    try {
      const response = await courseService.getAll();
      setCourses(response.data);
    } catch (err) {
      setError('Failed to load courses.');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    const confirmed = window.confirm('Are you sure you want to delete this course?');
    if (!confirmed) return;
    setError('');
    try {
      await courseService.delete(id);
      setCourses((prev) => prev.filter((c) => c.id !== id));
      setSuccess('Course deleted successfully.');
      setTimeout(() => setSuccess(''), 3000);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to delete course.');
    }
  };

  if (loading) return <div className="loading">Loading courses...</div>;

  return (
    <div className="list-page">
      <div className="list-header">
        <h1>Courses</h1>
        {canCreate && <Link to="/courses/new" className="btn btn-primary">+ Add Course</Link>}
      </div>
      {error && <div className="alert alert-error">{error}</div>}
      {success && <div className="alert alert-success">{success}</div>}
      {courses.length === 0 ? (
        <p className="empty-message">No courses found.</p>
      ) : (
        <div className="table-wrapper">
          <table>
            <thead>
              <tr>
                <th>ID</th>
                <th>Code</th>
                <th>Title</th>
                <th>Credits</th>
                <th>Semester</th>
                <th>Enrollment</th>
                <th>Active</th>
                {(canEdit || canDelete) && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {courses.map((course) => (
                <tr key={course.id}>
                  <td>{course.id}</td>
                  <td>{course.courseCode}</td>
                  <td>{course.title}</td>
                  <td>{course.credits}</td>
                  <td>{course.semester}</td>
                  <td>{course.currentEnrollment}/{course.maxEnrollment}</td>
                  <td>{course.isActive ? 'Yes' : 'No'}</td>
                  {(canEdit || canDelete) && (
                    <td>
                      <div className="action-buttons">
                        {canEdit && (role === 'Admin' || course.instructor?.id === user?.instructorId) && (
                          <Link to={`/courses/${course.id}`} className="btn btn-small btn-edit">Edit</Link>
                        )}
                        {canDelete && (role === 'Admin' || course.instructor?.id === user?.instructorId) && (
                          <button type="button" onClick={() => handleDelete(course.id)} className="btn btn-small btn-delete">Delete</button>
                        )}
                      </div>
                    </td>
                  )}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

export default CoursesListPage;
