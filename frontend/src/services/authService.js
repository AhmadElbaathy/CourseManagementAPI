import API from './api';

const authService = {
  login: (username, password) => {
    return API.post('/Auth/login', { username, password });
  },

  register: (username, email, password, role) => {
    return API.post('/Auth/register', { username, email, password, role });
  },
};

export default authService;
