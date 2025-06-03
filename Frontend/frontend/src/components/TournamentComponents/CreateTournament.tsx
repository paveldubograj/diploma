import React, { useEffect, useState } from "react";
import { Form, Button, Container } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import { TournamentCreateDto, TournamentFormat } from "../../types";
import { fetchDisciplines } from "../../api/disciplineApi";
import { createTournament } from "../../api/tournamentApi";

const CreateTournament: React.FC = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState<TournamentCreateDto>({
    name: "",
    disciplineId: "",
    format: 0,
    maxParticipants: 0,
    isRegistrationAllowed: false,
    startDate: "",
    endDate: ""
  });

  const [disciplines, setDisciplines] = useState<{ id: string; name: string }[]>([]);

  useEffect(() => {
    fetchDisciplines().then(setDisciplines).catch(console.error);
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: name === "maxParticipants" || name === "format" ? Number(value) : value,
    }));
  };
  

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await createTournament(formData);
      navigate("/tournaments");
    } catch (err) {
      console.error("Ошибка при создании турнира", err);
    }
  };

  return (
    <Container className="mt-4">
      <h2>Создание турнира</h2>
      <Form onSubmit={handleSubmit}>
        <Form.Group>
          <Form.Label>Название</Form.Label>
          <Form.Control name="name" value={formData.name} onChange={handleChange} required />
        </Form.Group>

        <Form.Group>
          <Form.Label>Дисциплина</Form.Label>
          <Form.Select name="disciplineId" value={formData.disciplineId} onChange={handleChange} required>
            <option value="">Выберите дисциплину</option>
            {disciplines.map((d) => (
              <option key={d.id} value={d.id}>{d.name}</option>
            ))}
          </Form.Select>
        </Form.Group>

        <Form.Group>
          <Form.Label>Формат</Form.Label>
          <Form.Select name="format" value={formData.format} onChange={handleChange} required>
            {TournamentFormat.map((f) => (
              <option key={f.id} value={f.id}>{f.name}</option>
            ))}
          </Form.Select>
        </Form.Group>

        <Form.Group>
          <Form.Label>Максимум участников</Form.Label>
          <Form.Control type="number" name="maxParticipants" value={formData.maxParticipants} onChange={handleChange} required />
        </Form.Group>

        <Form.Group>
          <Form.Label>Дата начала</Form.Label>
          <Form.Control type="date" name="startDate" value={formData.startDate} onChange={handleChange} required />
        </Form.Group>

        <Form.Group>
          <Form.Label>Дата окончания</Form.Label>
          <Form.Control type="date" name="endDate" value={formData.endDate} onChange={handleChange} required />
        </Form.Group>

        <Button className="mt-3" type="submit">Создать</Button>
      </Form>
    </Container>
  );
};

export default CreateTournament;
