// components/LoginForm.tsx
import React, { useState } from 'react';
import { apiFetch } from '../../api/api';
import { useNavigate } from 'react-router-dom';
import { Button } from 'react-bootstrap';
import { useAuth } from "../../api/AuthHook";
import { UserCleanDto } from "../../types";

const LoginForm: React.FC = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();
  const {login} = useAuth();

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    try {
      const response = await apiFetch("/user/login", {
        method: "POST",
        body: JSON.stringify({ email, password })
      });

      if (!response.ok) {
        throw new Error("Invalid credentials");
      }

      const data = await response.json();
      const user: UserCleanDto = {
        id: data.id,
        userName: data.userName,
        email: data.email,
      }
      login(user, data.accessToken)
      navigate("/news");
    } catch (err: any) {
      setError(err.message);
    }
  };

  return (
    <div>
      <form onSubmit={handleLogin}>
        <div>
          <label>Email:</label>
          <input value={email} onChange={(e) => setEmail(e.target.value)} />
        </div>
        <div>
          <label>Password:</label>
          <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} />
        </div>
        <button type="submit">Login</button>
        {error && <p style={{ color: "red" }}>{error}</p>}
      </form>
      <Button
        variant="link"
        onClick={() => navigate("/register")}
        className="mt-3"
      >
        Зарегистрироваться
      </Button>
    </div>
  );
};

export default LoginForm;
