import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import courseService from '../services/courseService';
import instructorService from '../services/instructorService';

function CourseFormPage({ user }) {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEdit = Boolean(id);

  const role = user?.role;
  const canCreate = role === 'Admin';
  const canEdit = role === 'Admin' || role === 'Instructor';

  const [form, setForm] = useState({
    courseCode: '',
    title: '',
    description: '',
    credits: 3,
    maxEnrollment: 30,
    semester: '',
    startDate: '',
    endDate: '',
    instructorId: '',
    isActive: true,
  });
  const [instructors, setInstructors] = useState([]);
  const [loading, setLoading] = useState(false);
  const [fetching, setFetching] = useState(isEdit);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  useEffect(() => {
    if (role === 'Admin') {
      instructorService.getAll()
        .then(res => setInstructors(res.data))
        .catch(() => {});
    }
  }, [role]);

  useEffect(() => {
    if (!isEdit && !canCreate) {
      navigate('/courses');
      return;
    }
    if (isEdit && !canEdit) {
      navigate('/courses');
      return;
    }
    if (isEdit) {
      courseService.getById(id)
        .then((res) => {
          const c = res.data;
          setForm({
            courseCode: c.courseCode || '',
            title: c.title || '',
            description: c.description || '',
            credits: c.credits || 3,
            maxEnrollment: c.maxEnrollment || 30,
            semester: c.semester || '',
            startDate: c.startDate ? c.startDate.split('T')[0] : '',
            endDate: c.endDate ? c.endDate.split('T')[0] : '',
            instructorId: c.instructor?.id || '',
            isActive: c.isActive,
          });
        })
        .catch(() => setError('Failed to load course.'))
        .finally(() => setFetching(false));
    }
  }, [id, isEdit, canCreate, canEdit, navigate]);

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setForm({ ...form, [name]: type === 'checkbox' ? checked : value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');
    setLoading(true);

    const payload = {
      ...form,
      credits: parseInt(form.credits),
      maxEnrollment: parseInt(form.maxEnrollment),
      instructorId: role === 'Instructor' ? user?.instructorId : (form.instructorId ? parseInt(form.instructorId) : null),
    };

    try {
      if (isEdit) {
        await courseService.update(id, payload);
        setSuccess('Course updated successfully!');
      } else {
        await courseService.create(payload);
        setSuccess('Course created successfully!');
      }
      setTimeout(() => navigate('/courses'), 1500);
    } catch (err) {
      setError(err.response?.data?.message || err.response?.data?.title || 'Failed to save course.');
    } finally {
      setLoading(false);
    }
  };

  if (fetching) return <div className="loading">Loading course...</div>;

  return (
    <div className="form-page">
      <h1>{isEdit ? 'Edit Course' : 'Add New Course'}</h1>
      {error && <div className="alert alert-error">{error}</div>}
      {success && <div className="alert alert-success">{success}</div>}
      <form onSubmit={handleSubmit} className="entity-form">
        <div className="form-group">
          <label htmlFor="courseCode">Course Code</label>
          <input id="courseCode" name="courseCode" value={form.courseCode} onChange={handleChange} required minLength={2} />
        </div>
        <div className="form-group">
          <label htmlFor="title">Title</label>
          <input id="title" name="title" value={form.title} onChange={handleChange} required minLength={3} />
        </div>
        <div className="form-group">
          <label htmlFor="description">Description</label>
          <textarea id="description" name="description" value={form.description} onChange={handleChange} rows={3} />
        </div>
        <div className="form-row">
          <div className="form-group">
            <label htmlFor="credits">Credits</label>
            <input id="credits" name="credits" type="number" min={1} max={10} value={form.credits} onChange={handleChange} required />
          </div>
          <div className="form-group">
            <label htmlFor="maxEnrollment">Max Enrollment</label>
            <input id="maxEnrollment" name="maxEnrollment" type="number" min={1} max={500} value={form.maxEnrollment} onChange={handleChange} />
          </div>
        </div>
        <div className="form-group">
          <label htmlFor="semester">Semester</label>
          <input id="semester" name="semester" value={form.semester} onChange={handleChange} required />
        </div>
        <div className="form-row">
          <div className="form-group">
            <label htmlFor="startDate">Start Date</label>
            <input id="startDate" name="startDate" type="date" value={form.startDate} onChange={handleChange} required />
          </div>
          <div className="form-group">
            <label htmlFor="endDate">End Date</label>
            <input id="endDate" name="endDate" type="date" value={form.endDate} onChange={handleChange} required />
          </div>
        </div>
        {role === 'Admin' && (
          <div className="form-group">
            <label htmlFor="instructorId">Assigned Instructor</label>
            <select id="instructorId" name="instructorId" value={form.instructorId} onChange={handleChange}>
              <option value="">-- No Instructor Assigned --</option>
              {instructors.map(inst => (
                <option key={inst.id} value={inst.id}>
                  {inst.fullName} ({inst.department})
                </option>
              ))}
            </select>
          </div>
        )}
        {isEdit && (
          <div className="form-group checkbox-group">
            <label>
              <input type="checkbox" name="isActive" checked={form.isActive} onChange={handleChange} />
              Active
            </label>
          </div>
        )}
        <div className="form-actions">
          <button type="submit" className="btn btn-primary" disabled={loading}>
            {loading ? 'Saving...' : isEdit ? 'Update Course' : 'Create Course'}
          </button>
          <button type="button" className="btn btn-secondary" onClick={() => navigate('/courses')}>Cancel</button>
        </div>
      </form>
    </div>
  );
}

export default CourseFormPage;
