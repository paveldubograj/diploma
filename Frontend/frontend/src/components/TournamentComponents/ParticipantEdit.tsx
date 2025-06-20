// src/pages/ParticipantEdit.tsx
import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Alert, Button, Card, Container, Form, Spinner } from "react-bootstrap";
import { getParticipantById, updateParticipant, deleteParticipant } from "../../api/tournamentApi";
import { ParticipantDto, ParticipantStatus } from "../../types";

const ParticipantEdit = () => {
  const { tournamentid, id } = useParams();
  const navigate = useNavigate();
  const [participant, setParticipant] = useState<ParticipantDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const loadParticipant = async () => {
      if (!id) return;
      if (!tournamentid) return;
      try {
        const data = await getParticipantById(id, tournamentid);
        setParticipant(data);
      } catch (err) {
        setError("Ошибка загрузки участника:");
      } finally {
        setLoading(false);
      }
    };

    loadParticipant();
  }, [id, tournamentid]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (!participant) return;
    const { name, value } = e.target;
    setParticipant({
      ...participant,
      [name]: name === "points" ? +value : value,
    });
  };

  const handleStatusChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    if (!participant) return;
    const { name, value } = e.target;
    setParticipant({
      ...participant,
      [name]: name === "points" ? +value : value,
    });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!participant || !id || !tournamentid) return;
    try {
      await updateParticipant(id, tournamentid, participant);
      navigate(-1);
    } catch (err) {
      setError("Ошибка обновления участника:");
    }
  };

  const handleDelete = async () => {
    if (!id || !tournamentid) return;
    try {
      await deleteParticipant(id, tournamentid);
      navigate(-1);
    } catch (err) {
      setError("Ошибка удаления участника:");
    }
  };

  if (loading) return <Spinner animation="border" />;

  return (
    <Container>
      <Card>
        <Card.Body>
          <Card.Title>Редактирование участника</Card.Title>
          <Form onSubmit={handleSubmit}>
            <Form.Group className="mb-3">
              <Form.Label>Имя</Form.Label>
              <Form.Control
                type="text"
                name="name"
                value={participant?.name || ""}
                onChange={handleChange}
              />
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>Очки</Form.Label>
              <Form.Control
                type="number"
                name="points"
                value={participant?.points || 0}
                onChange={handleChange}
              />
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>Статус</Form.Label>
              <Form.Select
                name="status"
                value={participant?.status.toString() || ""}
                onChange={handleStatusChange}
              >
                {ParticipantStatus.map((key) => (
                  <option key={key.id} value={key.id}>{key.name}</option>
                ))}
              </Form.Select>
            </Form.Group>
            {error && <Alert variant="danger">{error}</Alert>}
            <Button variant="primary" type="submit" className="me-2">Сохранить</Button>
            <Button variant="danger" onClick={handleDelete}>Удалить</Button>
          </Form>
        </Card.Body>
      </Card>
    </Container>
  );
};

export default ParticipantEdit;
