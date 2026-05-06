import API from './api';

const enrollmentService = {
  getAll: () => API.get('/Enrollments'),
  getById: (id) => API.get(`/Enrollments/${id}`),
  create: (data) => API.post('/Enrollments', data),
  update: (id, data) => API.put(`/Enrollments/${id}`, data),
  delete: (id) => API.delete(`/Enrollments/${id}`),
};

export default enrollmentService;
