import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import instructorService from '../services/instructorService';

function InstructorFormPage({ user }) {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEdit = Boolean(id);

  const role = user?.role;
  const canCreate = role === 'Admin';
  const canEdit = role === 'Admin' || role === 'Instructor';

  const [form, setForm] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    department: '',
    hireDate: '',
    isActive: true,
  });
  const [loading, setLoading] = useState(false);
  const [fetching, setFetching] = useState(isEdit);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  useEffect(() => {
    if (!isEdit && !canCreate) {
      navigate('/instructors');
      return;
    }
    if (isEdit && !canEdit) {
      navigate('/instructors');
      return;
    }
    if (isEdit) {
      instructorService.getById(id)
        .then((res) => {
          const i = res.data;
          setForm({
            firstName: i.firstName || '',
            lastName: i.lastName || '',
            email: i.email || '',
            phone: i.phone || '',
            department: i.department || '',
            hireDate: i.hireDate ? i.hireDate.split('T')[0] : '',
            isActive: i.isActive,
          });
        })
        .catch(() => setError('Failed to load instructor.'))
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
    if (!payload.hireDate) payload.hireDate = null;

    try {
      if (isEdit) {
        await instructorService.update(id, payload);
        setSuccess('Instructor updated successfully!');
      } else {
        await instructorService.create(payload);
        setSuccess('Instructor created successfully!');
      }
      setTimeout(() => navigate('/instructors'), 1500);
    } catch (err) {
      setError(err.response?.data?.message || err.response?.data?.title || 'Failed to save instructor.');
    } finally {
      setLoading(false);
    }
  };

  if (fetching) return <div className="loading">Loading instructor...</div>;

  return (
    <div className="form-page">
      <h1>{isEdit ? 'Edit Instructor' : 'Add New Instructor'}</h1>
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
          <label htmlFor="department">Department</label>
          <input id="department" name="department" value={form.department} onChange={handleChange} required />
        </div>
        <div className="form-group">
          <label htmlFor="hireDate">Hire Date</label>
          <input id="hireDate" name="hireDate" type="date" value={form.hireDate} onChange={handleChange} />
        </div>
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
            {loading ? 'Saving...' : isEdit ? 'Update Instructor' : 'Create Instructor'}
          </button>
          <button type="button" className="btn btn-secondary" onClick={() => navigate('/instructors')}>Cancel</button>
        </div>
      </form>
    </div>
  );
}

export default InstructorFormPage;
