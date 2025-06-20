// src/components/Header.tsx
import React from "react";
import { Container, Nav, Navbar, NavDropdown } from "react-bootstrap";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../api/AuthHook";
import { hasRole } from "../utils/auth";

const Header = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <Navbar bg="dark" variant="dark" expand="lg" className="mb-4">
      <Container>
        <Navbar.Brand as={Link} to="/">🏆 Турнирная платформа</Navbar.Brand>
        <Navbar.Toggle aria-controls="main-navbar" />
        <Navbar.Collapse id="main-navbar">
          <Nav className="me-auto">
            <Nav.Link as={Link} to="/news">Новости</Nav.Link>
            <Nav.Link as={Link} to="/matches">Матчи</Nav.Link>
            <Nav.Link as={Link} to="/tournaments">Турниры</Nav.Link>
            {hasRole("admin") && (
              <Nav.Link as={Link} to="/admin/users">Пользователи</Nav.Link>
            )}
          </Nav>
          <Nav>
            {user ? (
              <>
                <Nav.Link as={Link} to="/profile">Профиль</Nav.Link>
                <Nav.Link onClick={handleLogout}>Выход</Nav.Link>
              </>
            ) : (
              <>
                <Nav.Link as={Link} to="/login">Вход</Nav.Link>
                <Nav.Link as={Link} to="/register">Регистрация</Nav.Link>
              </>
            )}
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
};

export default Header;
