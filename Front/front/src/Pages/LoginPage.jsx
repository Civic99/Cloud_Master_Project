import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import axios from 'axios'; // Import Axios
import '../styles.css'; // Import the CSS file
import { useDispatch } from 'react-redux';

const LoginPage = () => {
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
  
    const handleLogin = async (e) => {
      e.preventDefault();
      try {
        const response = await axios.post('http://localhost:8717/api/User/Login', {
          username,
          password
        });
  
        if (response.status === 200) {
          const data = response.data;
          dispatch({ type: 'SET_USER_ID', payload: data.userId });
          navigate('/products');
        } else {
          // Handle error
          console.error('Failed to login');
        }
      } catch (error) {
        console.error('Failed to login', error);
      }
    };
  
    return (
      <div className="container">
        <h2>Login</h2>
        <form onSubmit={handleLogin}>
          <div className="form-group">
            <label>Username:</label>
            <input type="text" value={username} onChange={(e) => setUsername(e.target.value)} />
          </div>
          <div className="form-group">
            <label>Password:</label>
            <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} />
          </div>
          <button type="submit">Login</button>
        </form>
        <div className="register-link">
          <Link to="/register">Register</Link>
        </div>
      </div>
    );
  };

export default LoginPage;