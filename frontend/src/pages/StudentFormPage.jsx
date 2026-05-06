import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import studentService from '../services/studentService';

function StudentFormPage({ user }) {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEdit = Boolean(id);

  const role = user?.role;
  const canCreate = role === 'Admin';
  const canEdit = role === 'Admin' || role === 'Student';

  const [form, setForm] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    studentNumber: '',
    major: '',
    graduationDate: '',
    isActive: true,
  });
  const [loading, setLoading] = useState(false);
  const [fetching, setFetching] = useState(isEdit);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  useEffect(() => {
    if (!isEdit && !canCreate) {
      navigate('/students');
      return;
    }
    if (isEdit && !canEdit) {
      navigate('/students');
      return;
    }
    if (isEdit) {
      studentService.getById(id)
        .then((res) => {
          const s = res.data;
          setForm({
            firstName: s.firstName || '',
            lastName: s.lastName || '',
            email: s.email || '',
            phone: s.phone || '',
            studentNumber: s.studentNumber || '',
            major: s.major || '',
            graduationDate: s.graduationDate ? s.graduationDate.split('T')[0] : '',
            isActive: s.isActive,
          });
        })
        .catch(() => setError('Failed to load student.'))
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

    const payload = { ...form };
    if (!payload.phone) payload.phone = null;
    if (!payload.major) payload.major = null;
    if (!payload.graduationDate) payload.graduationDate = null;

    try {
      if (isEdit) {
        await studentService.update(id, payload);
        setSuccess('Student updated successfully!');
      } else {
        await studentService.create(payload);
        setSuccess('Student created successfully!');
      }
      setTimeout(() => navigate('/students'), 1500);
    } catch (err) {
      setError(err.response?.data?.message || err.response?.data?.title || 'Failed to save student.');
    } finally {
      setLoading(false);
    }
  };

  if (fetching) return <div className="loading">Loading student...</div>;

  return (
    <div className="form-page">
      <h1>{isEdit ? 'Edit Student' : 'Add New Student'}</h1>
      {error && <div className="alert alert-error">{error}</div>}
      {success && <div className="alert alert-success">{success}</div>}
      <form onSubmit={handleSubmit} className="entity-form">
        <div className="form-row">
          <div className="form-group">
            <label htmlFor="firstName">First Name</label>
            <input id="firstName" name="firstName" value={form.firstName} onChange={handleChange} required minLength={2} />
          </div>
          <div className="form-group">
            <label htmlFor="lastName">Last Name</label>
            <input id="lastName" name="lastName" value={form.lastName} onChange={handleChange} required minLength={2} />
          </div>
        </div>
        <div className="form-group">
          <label htmlFor="email">Email</label>
          <input id="email" name="email" type="email" value={form.email} onChange={handleChange} required />
        </div>
        <div className="form-group">
          <label htmlFor="phone">Phone</label>
          <input id="phone" name="phone" type="tel" value={form.phone} onChange={handleChange} />
        </div>
        <div className="form-group">
          <label htmlFor="studentNumber">Student Number</label>
          <input id="studentNumber" name="studentNumber" value={form.studentNumber} onChange={handleChange} required />
        </div>
        <div className="form-group">
          <label htmlFor="major">Major</label>
          <input id="major" name="major" value={form.major} onChange={handleChange} />
        </div>
        {isEdit && (
          <>
            <div className="form-group">
              <label htmlFor="graduationDate">Graduation Date</label>
              <input id="graduationDate" name="graduationDate" type="date" value={form.graduationDate} onChange={handleChange} />
            </div>
            <div className="form-group checkbox-group">
              <label>
                <input type="checkbox" name="isActive" checked={form.isActive} onChange={handleChange} />
                Active
              </label>
            </div>
          </>
        )}
        <div className="form-actions">
          <button type="submit" className="btn btn-primary" disabled={loading}>
            {loading ? 'Saving...' : isEdit ? 'Update Student' : 'Create Student'}
          </button>
          <button type="button" className="btn btn-secondary" onClick={() => navigate('/students')}>Cancel</button>
        </div>
      </form>
    </div>
  );
}

export default StudentFormPage;
