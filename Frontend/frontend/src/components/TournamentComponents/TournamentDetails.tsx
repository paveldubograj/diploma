import React, { useEffect, useState } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
import { fetchTournamentById, deleteTournament, startTournament, endTournament, setNextRound, generateBracket, registerForTournament, getParticipantById } from "../../api/tournamentApi";
import { MatchList, ParticipantStatus, TournamentDto, TournamentFormat, TournamentStatus } from "../../types";
import { Button, Card, Container, Row, Col, Form, Table, Nav, Alert, Modal, Image, Stack, ButtonGroup, Badge, Pagination, Spinner } from "react-bootstrap";
import { fetchMatches } from "../../api/matchApi";
import { hasRole } from "../../utils/auth";
import MatchCard from "../MatchComponents/MatchCard";
import { generateSVGPlaceholder } from "../../utils/PlaceholdGenerator";

const TournamentDetails = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState<"matches" | "participants">("matches");
  const [tournament, setTournament] = useState<TournamentDto | null>(null);
  const [matches, setMatches] = useState<MatchList[]>([]);
  const [loadingMatches, setLoadingMatches] = useState(true);
  const [matchPage, setMatchPage] = useState(1);
  const [matchPageSize] = useState(20);
  const [totalMatches, setTotalMatches] = useState(0);
  const [error, setError] = useState<string | null>(null);
  const [showRegisterModal, setShowRegisterModal] = useState(false);
  const [registerName, setRegisterName] = useState("");
  const [loading, setLoading] = useState(true);
  const [cacheBuster, setCacheBuster] = useState(Date.now());
  const [winnerName, setWinnerName] = useState('');


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
    console.log('Current tournament:', tournament);
  }, [tournament]);

  useEffect(() => {
    const loadWinner = async () => {
      if(!tournament) return;
      if (!tournament?.winnerId) return;

      try {
        const winner = await getParticipantById(tournament.winnerId, tournament.id);
        setWinnerName(winner.name);
      } catch (err) {
        console.error("Ошибка загрузки победителя:", err);
        setWinnerName("Неизвестно");
      }
    };

    loadWinner();
  }, [tournament?.winnerId, tournament]);

  const handleDelete = async () => {
    if (!id) return;
    await deleteTournament(id);
    navigate("/tournaments");
  };

  const handleAction = async (action: () => Promise<any>) => {
    if (!id) return;
    await action();
    const updated = await fetchTournamentById(id);
    setTournament(updated);
  };

  const handleRegister = async () => {
    try {
      if (!tournament) return;
      await registerForTournament(tournament.id, registerName);
      setError(null);
    } catch (err) {
      setError("Ошибка при регистрации на турнир.");
    }
  };

  useEffect(() => {
    const loadMatches = async () => {
      if (!id) return;

      const params = new URLSearchParams({
        page: matchPage.toString(),
        pageSize: matchPageSize.toString(),
        TournamentId: id
      });

      try {
        const result = await fetchMatches(params.toString());
        if (result) {
          setMatches(result.matches);
          setTotalMatches(result.total);
        }
      } catch (err) {
        console.error("Ошибка загрузки матчей:", err);
      } finally {
        setLoadingMatches(false);
      }
    };

    if(tournament) {loadMatches();}
  }, [id, matchPage, matchPageSize, tournament]);

  if (loading) {
    return (
      <Container className="d-flex justify-content-center align-items-center" style={{ minHeight: "50vh" }}>
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

  const canEdit = hasRole("organizer");

  return <Container className="my-4">
      <Row className="mb-4">
        <Col md={4}>
        {tournament && (
          <Image 
            src={tournament.imagePath 
              ? `http://localhost:5276/${tournament.imagePath}?t=${cacheBuster}`
              : generateSVGPlaceholder(400, 400)
            }
            alt={tournament.name}
            fluid
            rounded
            className="shadow-sm"
            style={{ maxHeight: '300px', objectFit: 'cover' }}
            onError={(e) => {
              const target = e.target as HTMLImageElement;
              target.src = generateSVGPlaceholder(400, 400);
            }}
          />)}
        </Col>
        <Col md={8}>
          <Stack gap={3}>
          {tournament && (
            <div className="d-flex justify-content-between align-items-start">
              <h1>{tournament.name}</h1>
              {canEdit && (
                <ButtonGroup>
                  <Button 
                    variant={"primary"} 
                    size="sm"
                    onClick={() => navigate(`/tournaments/update/${tournament.id}`)}
                  >
                    {"Редактировать"}
                  </Button>
                  <Button 
                    variant="outline-danger" 
                    size="sm"
                    onClick={handleDelete}
                  >
                    Удалить
                  </Button>
                </ButtonGroup>
              )}
            </div>
            )}

            {tournament && (
            <div className="d-flex gap-2">
              <Badge bg={"primary"}>{TournamentFormat.find(f => f.id === tournament.format)?.name}</Badge>
              <Badge bg={"success"}>{TournamentStatus[tournament.status].name}</Badge>
              {tournament.winnerId && (
                <Badge bg="success">Победитель: {winnerName}</Badge>
              )}
            </div>
            )}
            {tournament && (
            <Row>
              <Col md={6}>
                <Card>
                  <Card.Body>
                    <Card.Title>Основная информация</Card.Title>
                    <Stack gap={2}>
                      <div>
                        <strong>Дата начала:</strong> {new Date(tournament.startDate).toLocaleDateString()}
                      </div>
                      <div>
                        <strong>Дата окончания:</strong> {new Date(tournament.endDate).toLocaleDateString()}
                      </div>
                      <div>
                        <strong>Макс. участников:</strong> {tournament.maxParticipants}
                      </div>
                      <div>
                        <strong>Раундов:</strong> {tournament.rounds}
                      </div>
                      <div>
                        <strong>Регистрация:</strong>{" "}
                        <Badge bg={tournament.isRegistrationAllowed ? "success" : "secondary"}>
                          {tournament.isRegistrationAllowed ? "Открыта" : "Закрыта"}
                        </Badge>
                      </div>
                    </Stack>
                  </Card.Body>
                </Card>
              </Col>
              <Col md={6}>
                <Card>
                  <Card.Body>
                    <Card.Title>Действия</Card.Title>
                    <Stack gap={2}>
                      {(tournament.status === 0 || tournament.status === 4) && (
                        <Button 
                          variant="outline-primary"
                          onClick={() => handleAction(() => startTournament(tournament.id))}
                        >
                          Запустить турнир
                        </Button>
                      )}
                      {((tournament.format === 1 || tournament.format === 3) && tournament.status !== 2) && (
                        <Button 
                          variant="outline-primary"
                          onClick={() => handleAction(() => setNextRound(tournament.id))}
                        >
                          Следующий раунд
                        </Button>
                      )}
                      {(tournament.status !== 2 && tournament.status !== 3) && (
                        <Button 
                          variant="outline-danger"
                          onClick={() => handleAction(() => endTournament(tournament.id))}
                        >
                          Завершить турнир
                        </Button>
                      )}
                      {(tournament.status === 0 || ((tournament.format === 1 || tournament.format === 3) && tournament.status === 1)) && 
                        <Button 
                          variant="outline-success"
                          onClick={() => handleAction(() => generateBracket(tournament.id))}
                        >
                          Сгенерировать сетку
                        </Button>
                      }
                      {(tournament.isRegistrationAllowed && tournament.status !== 2) && (
                        <Button 
                          variant="success"
                          onClick={() => setShowRegisterModal(true)}
                        >
                          Зарегистрироваться
                        </Button>
                      )}
                    </Stack>
                  </Card.Body>
                </Card>
              </Col>
            </Row>
            )}
          </Stack>
        </Col>
      </Row>

      {error && (
        <Alert variant="danger" dismissible onClose={() => setError(null)}>
          {error}
        </Alert>
      )}

      <Nav variant="tabs" activeKey={activeTab} onSelect={(k) => setActiveTab(k as "matches" | "participants")}>
        <Nav.Item>
          <Nav.Link eventKey="matches">Матчи ({matches.length})</Nav.Link>
        </Nav.Item>
        <Nav.Item>
        {tournament && (<Nav.Link eventKey="participants">Участники ({tournament.participants.length})</Nav.Link>)}
        </Nav.Item>
      </Nav>

      <div className="mt-3">
        {activeTab === "matches" ? (
          <>
            <Row xs={1} md={2} lg={3} className="g-4">
              {matches.map((match) => (
                <Col key={match.id}>
                  <MatchCard {...match} />
                </Col>
              ))}
            </Row>
            
            {matches.length === 0 && (
              <Alert variant="info" className="text-center">
                Матчи не найдены
              </Alert>
            )}

            {totalMatches > matchPageSize && (
              <div className="d-flex justify-content-center mt-4">
                <Pagination>
                  <Pagination.Prev 
                    disabled={matchPage === 1} 
                    onClick={() => setMatchPage(matchPage - 1)} 
                  />
                  {Array.from({ length: Math.ceil(totalMatches / matchPageSize + 1) }).map((_, i) => (
                    <Pagination.Item
                      key={i + 1}
                      active={i + 1 === matchPage}
                      onClick={() => setMatchPage(i + 1)}
                    >
                      {i + 1}
                    </Pagination.Item>
                  ))}
                  <Pagination.Next
                    disabled={matchPage >= Math.ceil(totalMatches / matchPageSize + 1)}
                    onClick={() => setMatchPage(matchPage + 1)}
                  />
                </Pagination>
              </div>
            )}
          </>
        ) : (
          <>
            {canEdit && tournament && (
              <Link to={`/tournaments/${id}/add-participant`}>
                <Button variant="success" className="mb-3">
                  Добавить участника
                </Button>
              </Link>
            )}
            
            <Table striped bordered hover responsive>
              <thead>
                <tr>
                  <th>#</th>
                  <th>Имя</th>
                  <th>Очки</th>
                  <th>Статус</th>
                  {canEdit && <th>Действия</th>}
                </tr>
              </thead>
              <tbody>
                {tournament.participants.map((p, index) => (
                  <tr key={p.id}>
                    <td>{index + 1}</td>
                    <td>
                      <Link to={`/users/${p.userId}`} className="text-decoration-none">
                        {p.name}
                      </Link>
                    </td>
                    <td>{p.points ?? "—"}</td>
                    <td>
                      <Badge bg="info">
                        {ParticipantStatus[p.status].name}
                      </Badge>
                    </td>
                    {canEdit && (
                      <td>
                        <Link 
                          to={`/tournaments/${tournament.id}/edit-participant/${p.id}`}
                        >
                          Редактировать
                        </Link>
                      </td>
                    )}
                  </tr>
                ))}
              </tbody>
            </Table>
          </>
        )}
      </div>

      <Modal show={showRegisterModal} onHide={() => setShowRegisterModal(false)} centered>
        <Modal.Header closeButton>
          <Modal.Title>Регистрация на турнир</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            <Form.Group className="mb-3">
              <Form.Label>Ваше имя</Form.Label>
              <Form.Control
                type="text"
                value={registerName}
                onChange={(e) => setRegisterName(e.target.value)}
                placeholder="Введите имя для регистрации"
              />
            </Form.Group>
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowRegisterModal(false)}>
            Отмена
          </Button>
          <Button 
            variant="primary" 
            onClick={handleRegister} 
            disabled={!registerName.trim()}
          >
            Зарегистрироваться
          </Button>
        </Modal.Footer>
      </Modal>
    </Container>
};

export default TournamentDetails;
