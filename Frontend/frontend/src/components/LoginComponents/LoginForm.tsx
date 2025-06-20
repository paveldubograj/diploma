// components/LoginForm.tsx
import React, { useState } from 'react';
import { apiFetch } from '../../api/api';
import { useNavigate } from 'react-router-dom';
import { Alert, Button, Col, Form, Row } from 'react-bootstrap';
import { useAuth } from "../../api/AuthHook";
import { UserCleanDto } from "../../types";
import { loginF } from '../../api/userApi';

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
      const data = await loginF(email, password);
      const user: UserCleanDto = {
        id: data.id,
        userName: data.userName,
        email: data.email
      }
      login(user, data.accessToken)
      navigate("/");
    } catch (err: any) {
      setError(err.message);
    }
  };

  return (
    <Row className="justify-content-md-center">
        <Col md={6}>
          <h3 className="mb-4">Вход</h3>
          <Form onSubmit={handleLogin}>
            <Form.Group controlId="formEmail" className="mb-3">
              <Form.Label>Email</Form.Label>
              <Form.Control
                type="email"
                placeholder="Введите email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
              />
            </Form.Group>

            <Form.Group controlId="formPassword" className="mb-3">
              <Form.Label>Пароль</Form.Label>
              <Form.Control
                type="password"
                placeholder="Введите пароль"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
            </Form.Group>

            {error && <Alert variant="danger">{error}</Alert>}

            <Button variant="primary" type="submit" className="w-100">
              Войти
            </Button>
          </Form>

          <div className="text-center mt-3">
            <Button variant="link" onClick={() => navigate("/register")}>
              Зарегистрироваться
            </Button>
          </div>
        </Col>
      </Row>
  );
};

export default LoginForm;
