import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import enrollmentService from '../services/enrollmentService';
import courseService from '../services/courseService';

function EnrollmentFormPage({ user }) {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEdit = Boolean(id);

  const role = user?.role;
  const canCreate = role === 'Admin' || role === 'Student';
  const canEdit = role === 'Admin' || role === 'Instructor';

  const [studentId, setStudentId] = useState('');
  const [courseId, setCourseId] = useState('');
  const [status, setStatus] = useState('Enrolled');
  const [grade, setGrade] = useState('');
  const [letterGrade, setLetterGrade] = useState('');
  const [completionDate, setCompletionDate] = useState('');
  const [courses, setCourses] = useState([]);
  const [loading, setLoading] = useState(false);
  const [fetching, setFetching] = useState(isEdit);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  // For Student role, pre-fill with their saved student ID if available
  useEffect(() => {
    if (!isEdit && role === 'Student') {
      const savedId = localStorage.getItem('myStudentId');
      if (savedId) {
        setStudentId(savedId);
      }
    }
  }, [isEdit, role]);

  // Load courses for dropdown (in create mode)
  useEffect(() => {
    if (!isEdit) {
      courseService.getAll()
        .then((res) => setCourses(res.data))
        .catch(() => {});
    }
  }, [isEdit]);

  useEffect(() => {
    if (!isEdit && !canCreate) {
      navigate('/enrollments');
      return;
    }
    if (isEdit && !canEdit) {
      navigate('/enrollments');
      return;
    }
    if (isEdit) {
      enrollmentService.getById(id)
        .then((res) => {
          const data = res.data;
          setStudentId(data.studentId || '');
          setCourseId(data.courseId || '');
          setStatus(data.status || 'Enrolled');
          setGrade(data.grade != null ? String(data.grade) : '');
          setLetterGrade(data.letterGrade || '');
          setCompletionDate(data.completionDate ? data.completionDate.split('T')[0] : '');
        })
        .catch(() => setError('Failed to load enrollment.'))
        .finally(() => setFetching(false));
    }
  }, [id, isEdit, canCreate, canEdit, navigate]);

  const handleSubmit = async (evt) => {
    evt.preventDefault();
    setError('');
    setSuccess('');
    setLoading(true);

    try {
      if (isEdit) {
        const payload = {
          status: status || null,
          grade: grade !== '' ? parseFloat(grade) : null,
          letterGrade: letterGrade || null,
          completionDate: completionDate || null,
        };
        await enrollmentService.update(id, payload);
        setSuccess('Enrollment updated successfully!');
      } else {
        const sid = parseInt(studentId);
        const payload = {
          studentId: sid,
          courseId: parseInt(courseId),
        };
        await enrollmentService.create(payload);
        // Save student ID for future use (so the enrollments list can fetch their data)
        if (role === 'Student') {
          localStorage.setItem('myStudentId', String(sid));
        }
        setSuccess('Enrollment created successfully!');
      }
      setTimeout(() => navigate('/enrollments'), 1500);
    } catch (err) {
      setError(err.response?.data?.message || err.response?.data?.title || 'Failed to save enrollment.');
    } finally {
      setLoading(false);
    }
  };

  if (fetching) return <div className="loading">Loading enrollment...</div>;

  return (
    <div className="form-page">
      <h1>{isEdit ? 'Edit Enrollment' : 'Add New Enrollment'}</h1>
      {error && <div className="alert alert-error">{error}</div>}
      {success && <div className="alert alert-success">{success}</div>}
      <form onSubmit={handleSubmit} className="entity-form">
        {!isEdit ? (
          <>
            <div className="form-group">
              <label htmlFor="studentId">Student ID</label>
              <input
                id="studentId"
                type="text"
                inputMode="numeric"
                pattern="[0-9]*"
                value={studentId}
                onChange={(evt) => setStudentId(evt.target.value)}
                required
                placeholder="Enter your student record ID"
              />
              {role === 'Student' && (
                <small className="form-hint">Enter your Student record ID (check with your admin if unsure)</small>
              )}
            </div>
            <div className="form-group">
              <label htmlFor="courseId">Course</label>
              {courses.length > 0 ? (
                <select
                  id="courseId"
                  value={courseId}
                  onChange={(evt) => setCourseId(evt.target.value)}
                  required
                >
                  <option value="">-- Select a Course --</option>
                  {courses.filter(c => c.isActive).map((c) => (
                    <option key={c.id} value={c.id}>
                      {c.courseCode} — {c.title} ({c.currentEnrollment}/{c.maxEnrollment} enrolled)
                    </option>
                  ))}
                </select>
              ) : (
                <input
                  id="courseId"
                  type="text"
                  inputMode="numeric"
                  pattern="[0-9]*"
                  value={courseId}
                  onChange={(evt) => setCourseId(evt.target.value)}
                  required
                  placeholder="Enter course ID number"
                />
              )}
            </div>
          </>
        ) : (
          <>
            <div className="form-group">
              <label htmlFor="status">Status</label>
              <select id="status" value={status} onChange={(evt) => setStatus(evt.target.value)}>
                <option value="Enrolled">Enrolled</option>
                <option value="Completed">Completed</option>
                <option value="Withdrawn">Withdrawn</option>
                <option value="Failed">Failed</option>
              </select>
            </div>
            <div className="form-row">
              <div className="form-group">
                <label htmlFor="grade">Grade (0-100)</label>
                <input
                  id="grade"
                  type="text"
                  inputMode="decimal"
                  value={grade}
                  onChange={(evt) => setGrade(evt.target.value)}
                  placeholder="e.g. 85.5"
                />
              </div>
              <div className="form-group">
                <label htmlFor="letterGrade">Letter Grade</label>
                <select id="letterGrade" value={letterGrade} onChange={(evt) => setLetterGrade(evt.target.value)}>
                  <option value="">-- Select --</option>
                  <option value="A+">A+</option>
                  <option value="A">A</option>
                  <option value="B+">B+</option>
                  <option value="B">B</option>
                  <option value="C+">C+</option>
                  <option value="C">C</option>
                  <option value="D+">D+</option>
                  <option value="D">D</option>
                  <option value="F">F</option>
                </select>
              </div>
            </div>
            <div className="form-group">
              <label htmlFor="completionDate">Completion Date</label>
              <input
                id="completionDate"
                type="date"
                value={completionDate}
                onChange={(evt) => setCompletionDate(evt.target.value)}
              />
            </div>
          </>
        )}
        <div className="form-actions">
          <button type="submit" className="btn btn-primary" disabled={loading}>
            {loading ? 'Saving...' : isEdit ? 'Update Enrollment' : 'Create Enrollment'}
          </button>
          <button type="button" className="btn btn-secondary" onClick={() => navigate('/enrollments')}>Cancel</button>
        </div>
      </form>
    </div>
  );
}

export default EnrollmentFormPage;
