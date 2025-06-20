import React, { useState } from "react";
import { Form, Button, Container, Alert, Col, Row } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import { register } from "../../api/userApi";

const RegisterPage = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [email, setEmail] = useState("");
  const [error, setError] = useState("");
  const navigate = useNavigate();

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const response = await register(username, email, password);

      navigate("/login");
    } catch (err: any) {
      setError(err.message || "Ошибка регистрации");
    }
  };

  return (
    <Row className="justify-content-md-center">
      <Col md={6}>
        <h2 className="mb-4">Регистрация</h2>
        {error && <Alert variant="danger">{error}</Alert>}
        <Form onSubmit={handleRegister}>
          <Form.Group className="mb-3" controlId="username">
            <Form.Label>Логин</Form.Label>
            <Form.Control
              type="text"
              value={username}
              placeholder="Введите имя"
              onChange={(e) => setUsername(e.target.value)}
              required
            />
          </Form.Group>

          <Form.Group className="mb-3" controlId="email">
            <Form.Label>Email</Form.Label>
            <Form.Control
              type="text"
              value={email}
              placeholder="Введите email"
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </Form.Group>

          <Form.Group className="mb-3" controlId="password">
            <Form.Label>Пароль</Form.Label>
            <Form.Control
              type="password"
              placeholder="Введите пароль"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </Form.Group>

          <Button type="submit" variant="primary">
            Зарегистрироваться
          </Button>
        </Form>
      </Col>
    </Row>
  );
};

export default RegisterPage;
