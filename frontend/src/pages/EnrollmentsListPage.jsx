import { useState, useEffect, useCallback } from 'react';
import { Link } from 'react-router-dom';
import enrollmentService from '../services/enrollmentService';
import studentService from '../services/studentService';

function EnrollmentsListPage({ user }) {
  const [enrollments, setEnrollments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const role = user?.role;
  // Backend: GET all = Admin,Instructor. Create/Delete = Admin,Student. Update = Admin,Instructor.
  const canList = role === 'Admin' || role === 'Instructor';
  const canCreate = role === 'Admin' || role === 'Student';
  const canEdit = role === 'Admin' || role === 'Instructor' || role === 'Student';
  const canDelete = role === 'Admin' || role === 'Student';

  // For Student role: load their enrollments via GET /api/Students/{id}/enrollments
  const loadStudentEnrollments = useCallback(async () => {
    if (!user?.studentId) {
      setLoading(false);
      return;
    }
    try {
      const res = await studentService.getEnrollments(user.studentId);
      setEnrollments(res.data);
    } catch {
      setError('Failed to load your enrollments.');
    } finally {
      setLoading(false);
    }
  }, [user]);

  useEffect(() => {
    if (canList) {
      enrollmentService.getAll()
        .then((res) => setEnrollments(res.data))
        .catch(() => setError('Failed to load enrollments.'))
        .finally(() => setLoading(false));
    } else if (role === 'Student') {
      loadStudentEnrollments();
    } else {
      setLoading(false);
    }
  }, [canList, role, loadStudentEnrollments]);

  const handleDelete = async (id) => {
    const confirmed = window.confirm('Are you sure you want to delete this enrollment?');
    if (!confirmed) return;
    setError('');
    try {
      await enrollmentService.delete(id);
      setEnrollments((prev) => prev.filter((e) => e.id !== id));
      setSuccess('Enrollment deleted successfully.');
      setTimeout(() => setSuccess(''), 3000);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to delete enrollment.');
    }
  };

  if (loading) return <div className="loading">Loading enrollments...</div>;

  const isStudentWithNoId = false; // no longer applicable since user.id is always present

  return (
    <div className="list-page">
      <div className="list-header">
        <h1>{role === 'Student' ? 'My Enrollments' : 'Enrollments'}</h1>
        {canCreate && <Link to="/enrollments/new" className="btn btn-primary">+ Add Enrollment</Link>}
      </div>
      {error && <div className="alert alert-error">{error}</div>}
      {success && <div className="alert alert-success">{success}</div>}

      {isStudentWithNoId ? (
        <div className="empty-message">
          <p>You haven't created any enrollments yet.</p>
          <p style={{marginTop: '8px'}}>Click <Link to="/enrollments/new" style={{color: '#3b82f6', fontWeight: 600}}>+ Add Enrollment</Link> to enroll in a course!</p>
        </div>
      ) : enrollments.length === 0 ? (
        <div className="empty-message">
          {role === 'Student'
            ? 'No enrollments found. Click "+ Add Enrollment" to enroll in a course!'
            : 'No enrollments found.'
          }
        </div>
      ) : (
        <div className="table-wrapper">
          <table>
            <thead>
              <tr>
                <th>ID</th>
                {canList && <th>Student</th>}
                <th>Course</th>
                <th>Status</th>
                <th>Grade</th>
                <th>Letter</th>
                {(canEdit || canDelete) && <th>Actions</th>}
              </tr>
            </thead>
            <tbody>
              {enrollments.map((enrollment) => (
                <tr key={enrollment.id}>
                  <td>{enrollment.id}</td>
                  {canList && <td>{enrollment.student?.fullName || enrollment.studentId}</td>}
                  <td>{enrollment.courseName || enrollment.course?.title || enrollment.courseId}</td>
                  <td>{enrollment.status}</td>
                  <td>{enrollment.grade ?? '-'}</td>
                  <td>{enrollment.letterGrade || '-'}</td>
                  {(canEdit || canDelete) && (
                    <td>
                      <div className="action-buttons">
                        {canEdit && (role === 'Admin' || role === 'Student' || enrollment.course?.instructorId === user?.id) && (
                          <Link to={`/enrollments/${enrollment.id}`} className="btn btn-small btn-edit">Edit</Link>
                        )}
                        {canDelete && (role === 'Admin' || role === 'Student' || enrollment.course?.instructorId === user?.id) && (
                          <button type="button" onClick={() => handleDelete(enrollment.id)} className="btn btn-small btn-delete">Delete</button>
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

export default EnrollmentsListPage;
