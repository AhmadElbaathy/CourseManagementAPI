import API from './api';

const studentService = {
  getAll: () => API.get('/Students'),
  getById: (id) => API.get(`/Students/${id}`),
  create: (data) => API.post('/Students', data),
  update: (id, data) => API.put(`/Students/${id}`, data),
  delete: (id) => API.delete(`/Students/${id}`),
  getEnrollments: (id) => API.get(`/Students/${id}/enrollments`),
};

export default studentService;
