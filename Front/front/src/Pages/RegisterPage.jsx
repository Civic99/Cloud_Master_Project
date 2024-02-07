import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import '../styles.css'; // Import the CSS file
import axios from 'axios';
import { Link } from 'react-router-dom';

const RegisterPage = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.post('http://localhost:8717/api/User/Register', {
        username,
        password
      });

      if (response.status === 200) {
        console.log('Registration successful');
      } else {
        // Handle error
        console.error('Failed to register');
      }
    } catch (error) {
      console.error('Failed to register', error);
    }
  };

  return (
    <div className="container">
      <h2>Register</h2>
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label>Username:</label>
          <input type="text" value={username} onChange={(e) => setUsername(e.target.value)} />
        </div>
        <div className="form-group">
          <label>Password:</label>
          <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} />
        </div>
        <button type="submit">Register</button>
      </form>
      <div className="login-link">
        <Link to="/login">Login</Link>
      </div>
    </div>
  );
};

export default RegisterPage;