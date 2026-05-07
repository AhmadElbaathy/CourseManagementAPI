import API from './api';

const authService = {
  login: (username, password) => {
    return API.post('/Auth/login', { username, password });
  },

  register: (username, email, password) => {
    return API.post('/Auth/register', { username, email, password });
  },
};

export default authService;
