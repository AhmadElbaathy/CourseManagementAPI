import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import instructorService from '../services/instructorService';

function InstructorsListPage({ user }) {
  const [instructors, setInstructors] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const role = user?.role;
  const canCreate = role === 'Admin';
  const canEdit = role === 'Admin' || role === 'Instructor';
  const canDelete = role === 'Admin';

  useEffect(() => {
    fetchInstructors();
  }, []);

  const fetchInstructors = async () => {
    try {
      const response = await instructorService.getAll();
      setInstructors(response.data);
    } catch (err) {
      setError('Failed to load instructors.');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    const confirmed = window.confirm('Are you sure you want to delete this instructor?');
    if (!confirmed) return;
    setError('');
    try {
      await instructorService.delete(id);
      setInstructors((prev) => prev.filter((i) => i.id !== id));
      setSuccess('Instructor deleted successfully.');
      setTimeout(() => setSuccess(''), 3000);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to delete instructor.');
    }
  };

  if (loading) return <div className="loading">Loading instructors...</div>;

  return (
    <div className="list-page">
      <div className="list-header">
        <h1>Instructors</h1>
        {canCreate && <Link to="/instructors/new" className="btn btn-primary">+ Add Instructor</Link>}
      </div>
      {error && <div className="alert alert-error">{error}</div>}
      {success && <div className="alert alert-success">{success}</div>}
      {instructors.length === 0 ? (
        <p className="empty-message">No instructors found.</p>
      ) : (
        <div className="table-wrapper">
          <table>
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                <th>Email</th>
                <th>Department</th>
                <th>Courses</th>
                <th>Active</th>
                {(canEdit || canDelete) && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {instructors.map((instructor) => (
                <tr key={instructor.id}>
                  <td>{instructor.id}</td>
                   <td>
                    {instructor.firstName} {instructor.lastName}
                    {!instructor.isProfileComplete && (
                      <span className="badge" style={{marginLeft: '8px', fontSize: '0.8em', padding: '2px 6px', backgroundColor: '#ffc107', color: '#000', borderRadius: '4px'}}>Needs Update</span>
                    )}
                  </td>
                  <td>{instructor.email}</td>
                  <td>{instructor.department}</td>
                  <td>{instructor.courseCount}</td>
                  <td>{instructor.isActive ? 'Yes' : 'No'}</td>
                  {(canEdit || canDelete) && (
                    <td>
                      <div className="action-buttons">
                        {canEdit && (role === 'Admin' || instructor.id === user?.instructorId) && (
                          <Link to={`/instructors/${instructor.id}`} className="btn btn-small btn-edit">Edit</Link>
                        )}
                        {canDelete && (role === 'Admin' || instructor.id === user?.instructorId) && (
                          <button type="button" onClick={() => handleDelete(instructor.id)} className="btn btn-small btn-delete">Delete</button>
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

export default InstructorsListPage;
