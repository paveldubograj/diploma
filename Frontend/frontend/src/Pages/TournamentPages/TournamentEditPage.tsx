import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { 
  fetchTournamentById, 
  updateTournament, 
  deleteTournament 
} from '../../api/tournamentApi';
import { TournamentDto, TournamentStatus, TournamentFormat, Discipline } from '../../types';
import { 
  Button, 
  Card, 
  Container, 
  Form, 
  Alert, 
  Row, 
  Col,
  Spinner,
  Badge
} from 'react-bootstrap';
import TournamentImageUploader from '../../components/TournamentComponents/TournamentImageUploader';
import { fetchDisciplines } from '../../api/disciplineApi';

const TournamentEditPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [tournament, setTournament] = useState<TournamentDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isTournamentStarted, setIsTournamentStarted] = useState(false);
  const [disciplines, setDisciplines] = useState<Discipline[]>([]);

  useEffect(() => {
    const loadTournament = async () => {
      try {
        if (!id) {
          setError("ID турнира не указан");
          return;
        }
        
        setLoading(true);
        const data = await fetchTournamentById(id);
        setTournament(data);
        setIsTournamentStarted(data.status > TournamentStatus[0].id);
      } catch (err) {
        setError("Не удалось загрузить турнир");
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    loadTournament();
  }, [id]);

  useEffect(() => {
          try {
              fetchDisciplines().then(setDisciplines);
          } catch (error) {
              setError("Ошибка получения дисциплин");
          }
      }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!tournament || !id) return;

    if(!tournament.winnerId) tournament.winnerId = '';

    try {
      setLoading(true);
      await updateTournament(id, tournament);
      navigate(`/tournaments/${id}`);
    } catch (err) {
      setError("Не удалось обновить турнир");
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setTournament(prev => ({
      ...prev!,
      [name]: name === "maxParticipants" || name === "rounds" 
        ? parseInt(value, 10) 
        : value
    }));
  };

  const handleStatusChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const newStatus = parseInt(e.target.value, 10);
    setTournament(prev => ({
      ...prev!,
      status: newStatus
    }));
    setIsTournamentStarted(newStatus > TournamentStatus[0].id);
  };

  const handleDelete = async () => {
    if (!id || !window.confirm("Вы уверены, что хотите удалить этот турнир?")) return;
    try {
      await deleteTournament(id);
      navigate("/tournaments");
    } catch (error) {
      setError("Не удалось удалить турнир");
      console.error(error);
    }
  };

  if (loading && !tournament) {
    return (
      <Container className="d-flex justify-content-center mt-5">
        <Spinner animation="border" variant="primary" />
      </Container>
    );
  }

  if (error) {
    return (
      <Container className="my-4">
        <Alert variant="danger">
          {error}
          <Button variant="link" onClick={() => navigate("/tournaments")}>
            Вернуться к списку турниров
          </Button>
        </Alert>
      </Container>
    );
  }

  if (!tournament) {
    return (
      <Container className="my-4">
        <Alert variant="warning">
          Турнир не найден
          <Button variant="link" onClick={() => navigate("/tournaments")}>
            Вернуться к списку турниров
          </Button>
        </Alert>
      </Container>
    );
  }

  const currentStatus = TournamentStatus.find(s => s.id === tournament.status);

  return (
    <Container className="my-4">
      <Card>
        <Card.Header className="d-flex justify-content-between align-items-center">
          <h2>Редактирование турнира</h2>
          {currentStatus && (
            <Badge bg={
              tournament.status === 2 ? 'success' : 
              tournament.status === 3 ? 'danger' : 
              'primary'
            }>
              {currentStatus.name}
            </Badge>
          )}
        </Card.Header>
        <Card.Body>
          <Form onSubmit={handleSubmit}>
            <Row>
              <Col md={4}>
                <TournamentImageUploader
                  tournamentId={tournament.id}
                  currentImageUrl={tournament.imagePath 
                    ? `http://localhost:5149/${tournament.imagePath}`
                    : undefined
                  }
                  onImageChange={(updatedTournament) => {
                    setTournament(updatedTournament);
                  }}
                />
              </Col>
              <Col md={8}>
                <Form.Group className="mb-3">
                  <Form.Label>Название турнира</Form.Label>
                  <Form.Control
                    type="text"
                    name="name"
                    value={tournament.name}
                    onChange={handleChange}
                    required
                  />
                </Form.Group>

                <Form.Group className="mb-3">
                  <Form.Label>Дисциплина</Form.Label>
                  <Form.Select
                    name="disciplineId"
                    value={tournament.disciplineId}
                    onChange={handleChange}
                    required
                  >
                    {disciplines.map((format) => (
                          <option key={format.id} value={format.id}>
                            {format.name}
                          </option>
                        ))}
                    </Form.Select>
                </Form.Group>

                <Row>
                  <Col md={6}>
                    <Form.Group className="mb-3">
                      <Form.Label>Формат</Form.Label>
                      <Form.Select
                        name="format"
                        value={tournament.format}
                        onChange={handleChange}
                        disabled={isTournamentStarted}
                      >
                        {TournamentFormat.map((format) => (
                          <option key={format.id} value={format.id}>
                            {format.name}
                          </option>
                        ))}
                      </Form.Select>
                      {isTournamentStarted && (
                        <Form.Text className="text-muted">
                          Формат нельзя изменить после начала турнира
                        </Form.Text>
                      )}
                    </Form.Group>
                  </Col>
                  <Col md={6}>
                    <Form.Group className="mb-3">
                      <Form.Label>Статус</Form.Label>
                      <Form.Select
                        name="status"
                        value={tournament.status}
                        onChange={handleStatusChange}
                      >
                        {TournamentStatus.map((status) => (
                          <option key={status.id} value={status.id}>
                            {status.name}
                          </option>
                        ))}
                      </Form.Select>
                    </Form.Group>
                  </Col>
                </Row>

                <Row>
                  <Col md={6}>
                    <Form.Group className="mb-3">
                      <Form.Label>Дата начала</Form.Label>
                      <Form.Control
                        type="datetime-local"
                        name="startDate"
                        value={new Date(tournament.startDate).toISOString().slice(0, 16)}
                        onChange={(e) => 
                          setTournament(prev => ({
                            ...prev!,
                            startDate: new Date(e.target.value).toISOString()
                          }))
                        }
                        required
                      />
                    </Form.Group>
                  </Col>
                  <Col md={6}>
                    <Form.Group className="mb-3">
                      <Form.Label>Дата окончания</Form.Label>
                      <Form.Control
                        type="datetime-local"
                        name="endDate"
                        value={new Date(tournament.endDate).toISOString().slice(0, 16)}
                        onChange={(e) => 
                          setTournament(prev => ({
                            ...prev!,
                            endDate: new Date(e.target.value).toISOString()
                          }))
                        }
                        required
                      />
                    </Form.Group>
                  </Col>
                </Row>

                <Row>
                  <Col md={6}>
                    <Form.Group className="mb-3">
                      <Form.Label>Макс. участников</Form.Label>
                      <Form.Control
                        type="number"
                        name="maxParticipants"
                        min="2"
                        value={tournament.maxParticipants}
                        onChange={handleChange}
                        required
                      />
                    </Form.Group>
                  </Col>
                  <Col md={6}>
                    <Form.Group className="mb-3">
                      <Form.Label>Количество раундов</Form.Label>
                      <Form.Control
                        type="number"
                        name="rounds"
                        min="0"
                        value={tournament.rounds}
                        onChange={handleChange}
                        required
                      />
                    </Form.Group>
                  </Col>
                </Row>

                <Form.Group className="mb-3">
                  <Form.Check
                    type="switch"
                    id="registration-switch"
                    label="Разрешена регистрация"
                    checked={tournament.isRegistrationAllowed}
                    onChange={(e) => 
                      setTournament(prev => ({
                        ...prev!,
                        isRegistrationAllowed: e.target.checked
                      }))
                    }
                  />
                </Form.Group>

                <div className="d-flex justify-content-end gap-2 mt-4">
                  <Button 
                    variant="outline-danger" 
                    onClick={handleDelete}
                    disabled={loading}
                  >
                    Удалить турнир
                  </Button>
                  <Button 
                    variant="primary" 
                    type="submit" 
                    disabled={loading}
                  >
                    {loading ? 'Сохранение...' : 'Сохранить изменения'}
                  </Button>
                </div>
              </Col>
            </Row>
          </Form>
        </Card.Body>
      </Card>
    </Container>
  );
};

export default TournamentEditPage;
