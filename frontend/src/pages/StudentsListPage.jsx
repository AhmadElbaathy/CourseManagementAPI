import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import studentService from '../services/studentService';

function StudentsListPage({ user }) {
  const [students, setStudents] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const role = user?.role;
  // Backend: GET all = Admin,Instructor. Create/Delete = Admin. Update = Admin,Student.
  const canList = role === 'Admin' || role === 'Instructor';
  const canCreate = role === 'Admin';
  const canEdit = role === 'Admin' || role === 'Student';
  const canDelete = role === 'Admin';

  useEffect(() => {
    if (canList) {
      fetchStudents();
    } else {
      setLoading(false);
    }
  }, [canList]);

  const fetchStudents = async () => {
    try {
      const response = await studentService.getAll();
      setStudents(response.data);
    } catch (err) {
      setError('Failed to load students.');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    const confirmed = window.confirm('Are you sure you want to delete this student?');
    if (!confirmed) return;
    setError('');
    try {
      await studentService.delete(id);
      setStudents((prev) => prev.filter((s) => s.id !== id));
      setSuccess('Student deleted successfully.');
      setTimeout(() => setSuccess(''), 3000);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to delete student.');
    }
  };

  if (loading) return <div className="loading">Loading students...</div>;

  if (!canList) {
    return (
      <div className="list-page">
        <div className="permission-denied">
          <div className="lock-icon">🔒</div>
          <h2>Access Restricted</h2>
          <p>You don't have permission to view the students list. Only Admin and Instructor roles can access this page.</p>
        </div>
      </div>
    );
  }

  return (
    <div className="list-page">
      <div className="list-header">
        <h1>Students</h1>
        {canCreate && <Link to="/students/new" className="btn btn-primary">+ Add Student</Link>}
      </div>
      {error && <div className="alert alert-error">{error}</div>}
      {success && <div className="alert alert-success">{success}</div>}
      {students.length === 0 ? (
        <p className="empty-message">No students found.</p>
      ) : (
        <div className="table-wrapper">
          <table>
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                <th>Email</th>
                <th>Student #</th>
                <th>Major</th>
                <th>Active</th>
                {(canEdit || canDelete) && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {students.map((student) => (
                <tr key={student.id}>
                  <td>{student.id}</td>
                  <td>
                    {student.firstName} {student.lastName}
                    {!student.isProfileComplete && (
                      <span className="badge" style={{marginLeft: '8px', fontSize: '0.8em', padding: '2px 6px', backgroundColor: '#ffc107', color: '#000', borderRadius: '4px'}}>Needs Update</span>
                    )}
                  </td>
                  <td>{student.email}</td>
                  <td>{student.studentNumber}</td>
                  <td>{student.major || '-'}</td>
                  <td>{student.isActive ? 'Yes' : 'No'}</td>
                  {(canEdit || canDelete) && (
                    <td>
                      <div className="action-buttons">
                        {canEdit && <Link to={`/students/${student.id}`} className="btn btn-small btn-edit">Edit</Link>}
                        {canDelete && <button type="button" onClick={() => handleDelete(student.id)} className="btn btn-small btn-delete">Delete</button>}
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

export default StudentsListPage;
