// src/components/Footer.tsx
import React from "react";
import { Container } from "react-bootstrap";

const Footer = () => {
  return (
    <footer className="bg-dark text-light text-center py-3 mt-auto">
      <Container>
        <small>© {new Date().getFullYear()} Турнирная платформа | Все права защищены</small>
      </Container>
    </footer>
  );
};

export default Footer;
