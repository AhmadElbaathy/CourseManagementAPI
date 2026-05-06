import API from './api';

const instructorService = {
  getAll: () => API.get('/Instructors'),
  getById: (id) => API.get(`/Instructors/${id}`),
  create: (data) => API.post('/Instructors', data),
  update: (id, data) => API.put(`/Instructors/${id}`, data),
  delete: (id) => API.delete(`/Instructors/${id}`),
};

export default instructorService;
