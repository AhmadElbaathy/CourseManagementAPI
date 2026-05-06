import API from './api';

const courseService = {
  getAll: () => API.get('/Courses'),
  getById: (id) => API.get(`/Courses/${id}`),
  create: (data) => API.post('/Courses', data),
  update: (id, data) => API.put(`/Courses/${id}`, data),
  delete: (id) => API.delete(`/Courses/${id}`),
};

export default courseService;
