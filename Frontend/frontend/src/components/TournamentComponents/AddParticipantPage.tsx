import React, { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { addParticipant } from "../../api/tournamentApi";
import { ParticipantCreateDto } from "../../types";
import {
  Container, Form, Button, Alert
} from "react-bootstrap";

const AddParticipantPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [formData, setFormData] = useState<ParticipantCreateDto>({
    name: "",
  });

  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: name === "points" ? Number(value) : value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    try {
      await addParticipant(id!, formData);
      setSuccess(true);
      setTimeout(() => navigate(`/tournaments/${id}`), 1000);
    } catch (err) {
      setError("Не удалось добавить участника");
    }
  };

  return (
    <Container className="mb-3" style={{ maxWidth: 600 }}>
      <h3>Добавление участника</h3>
      <Form onSubmit={handleSubmit}>
        <Form.Group controlId="name" className="mb-3">
          <Form.Label>Имя участника</Form.Label>
          <Form.Control
            type="text"
            name="name"
            value={formData.name}
            onChange = {handleChange}
            required
          />
        </Form.Group>

        <Button type="submit">Добавить</Button>
      </Form>

      {error && <Alert variant="danger" className="mt-3">{error}</Alert>}
      {success && <Alert variant="success" className="mt-3">Участник успешно добавлен</Alert>}
    </Container>
  );
};

export default AddParticipantPage;
