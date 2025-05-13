import React, { useEffect, useState } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
import { fetchTournamentById, updateTournament, deleteTournament, startTournament, endTournament, setNextRound, generateBracket, registerForTournament } from "../../api/tournamentApi";
import { MatchList, ParticipantStatus, TournamentDto, TournamentFormat, TournamentStatus } from "../../types";
import { Button, Card, Container, Row, Col, Form, Table, Nav, Alert, Modal } from "react-bootstrap";
import { fetchMatches } from "../../api/matchApi";
import { hasRole } from "../../utils/auth";

const TournamentDetails = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState<"matches" | "participants">("matches");
  const [tournament, setTournament] = useState<TournamentDto | null>(null);
  const [editMode, setEditMode] = useState(false);
  const [formData, setFormData] = useState<TournamentDto | null>(null);
  const [matches, setMatches] = useState<MatchList[]>([]);
  const [loadingMatches, setLoadingMatches] = useState(true);
  const [matchPage, setMatchPage] = useState(1);
  const [matchPageSize] = useState(20);
  const [totalMatches, setTotalMatches] = useState(0);
  const [error, setError] = useState<string | null>(null);
  const [showRegisterModal, setShowRegisterModal] = useState(false);
  const [registerName, setRegisterName] = useState("");


  useEffect(() => {
    try {
      if (id) {
        fetchTournamentById(id).then(setTournament);
      }
    } catch (error) {
      setError("Ошибка получения турнира");
    }
  }, [id]);

  const handleSave = async () => {
    try {
      if (!formData || !id) return;
      const updated = await updateTournament(id, formData);
      setTournament(updated);
      setEditMode(false);
    } catch (error) {
      setError("Ошибка обновления турнира");
    }
  };

  const handleDelete = async () => {
    if (!id) return;
    await deleteTournament(id);
    navigate("/tournaments");
  };

  const handleAction = async (action: () => Promise<any>) => {
    await action();
    const updated = await fetchTournamentById(id!);
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

  const openRegisterModal = () => setShowRegisterModal(true);
  const closeRegisterModal = () => setShowRegisterModal(false);


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

    loadMatches();
  }, [id, matchPage, matchPageSize]);

  if (!tournament) return <div>Загрузка...</div>;

  return (
    <Container>
      <h2>{tournament.name}</h2>
      <Card className="mb-4">
        <Card.Body>

          {!editMode ? (
            <Card>
              <Card.Body>
                <p><strong>Формат:</strong> {TournamentFormat.find(f => f.id === tournament.format)?.name}</p>
                <p><strong>Статус:</strong> {TournamentStatus[tournament.status].name}</p>
                <p><strong>Макс. участников:</strong> {tournament.maxParticipants}</p>
                <p><strong>Раунды:</strong> {tournament.rounds}</p>
                <p><strong>Дата начала:</strong> {tournament.startDate?.split("T")[0]}</p>
                <p><strong>Дата окончания:</strong> {tournament.endDate?.split("T")[0]}</p>
                <p><strong>Регистрация:</strong> {tournament.isRegistrationAllowed ? "Да" : "Нет"}</p>
                <p><strong>Победитель:</strong> {tournament.winnerId}</p>
              </Card.Body>
            </Card>
          ) : (<Form onSubmit={handleSave}>
            <Form.Group className="mb-3">
              <Form.Label>Название</Form.Label>
              <Form.Control
                type="text"
                value={tournament.name}
                onChange={(e) => setTournament({ ...tournament, name: e.target.value })}
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Формат</Form.Label>
              <Form.Select
                value={tournament.format}
                onChange={(e) => setTournament({ ...tournament, format: parseInt(e.target.value) })}
              >
                {TournamentFormat.map((f) => (
                  <option key={f.id} value={f.id}>
                    {f.name}
                  </option>
                ))}
              </Form.Select>
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Статус</Form.Label>
              <Form.Select
                value={tournament.status}
                onChange={(e) =>
                  setTournament({ ...tournament, status: parseInt(e.target.value) })
                }
              >
                {TournamentStatus.map((f) => (
                  <option key={f.id} value={f.id}>
                    {f.name}
                  </option>
                ))}
              </Form.Select>
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Макс. участников</Form.Label>
              <Form.Control
                type="number"
                value={tournament.maxParticipants}
                onChange={(e) => setTournament({ ...tournament, maxParticipants: parseInt(e.target.value) })}
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Дата начала</Form.Label>
              <Form.Control
                type="date"
                value={tournament.startDate?.split('T')[0] ?? ""}
                onChange={(e) => setTournament({ ...tournament, startDate: e.target.value })}
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Дата окончания</Form.Label>
              <Form.Control
                type="date"
                value={tournament.endDate?.split('T')[0] ?? ""}
                onChange={(e) => setTournament({ ...tournament, endDate: e.target.value })}
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Регистрация</Form.Label>
              <Form.Check
                type="checkbox"
                label="Разрешена"
                checked={tournament.isRegistrationAllowed}
                onChange={(e) =>
                  setTournament({ ...tournament, isRegistrationAllowed: e.target.checked })
                }
              />
            </Form.Group>

            {(editMode && hasRole("organizer")) ? (
              (<Button onClick={handleSave} className="me-2">Сохранить</Button>)
            ) : (
              <Button onClick={() => {
                setEditMode(true);
                setFormData(tournament);
              }} className="me-2">Редактировать</Button>
            )}
            {hasRole("organizer") && (<Button variant="danger" onClick={handleDelete} className="me-2">Удалить</Button>)}
          </Form>)}
        </Card.Body>
      </Card>

      <Row className="mb-4">
        <Col>
          {tournament.status === 0 && (<Button onClick={() => handleAction(() => startTournament(tournament.id))}>Запустить</Button>)}
          {((tournament.format === 1 || tournament.format === 3) && tournament.status !== 2) && (<Button onClick={() => handleAction(() => setNextRound(tournament.id))}>След. раунд</Button>)}
          {tournament.status !== 2 && (<Button onClick={() => handleAction(() => endTournament(tournament.id))}>Завершить</Button>)}
          {tournament.status !== 2 && (<Button onClick={() => handleAction(() => generateBracket(tournament.id))}>Сгенерировать сетку</Button>)}
          {tournament.isRegistrationAllowed && (
            <Button variant="success" className="ms-3" onClick={openRegisterModal}>
              Зарегистрироваться
            </Button>
          )}
        </Col>
      </Row>

      <Nav variant="tabs" activeKey={activeTab} onSelect={(k) => setActiveTab(k as "matches" | "participants")}>
        <Nav.Item>
          <Nav.Link eventKey="matches">Матчи</Nav.Link>
        </Nav.Item>
        <Nav.Item>
          <Nav.Link eventKey="participants">Участники</Nav.Link>
        </Nav.Item>
      </Nav>

      <div className="mt-3">
        {activeTab === "matches" && (
          <>
            {matches.map((match) => {
              const isCompleted = match.status === 2;
              const winnerColor = {
                [match.participant1Id]: match.winnerId === match.participant1Id ? "green" : "red",
                [match.participant2Id]: match.winnerId === match.participant2Id ? "green" : "red"
              };

              return (
                <Col md={6} lg={4} key={match.id} className="mb-3">
                  <Link to={`/matches/${match.id}`} style={{ textDecoration: "none" }}>
                    <Card>
                      <Card.Body>
                        <Card.Title>{match.round} (#{match.matchOrder})</Card.Title>
                        <Card.Text>
                          Участники: <br />
                          {match.participant1Name}<br />
                          {match.participant2Name}
                          {isCompleted && (
                            <>
                              <br />
                              <span>Счет:</span>
                              <span style={{ color: winnerColor[match.participant1Id] }}> {match.winScore}</span> -
                              <span style={{ color: winnerColor[match.participant2Id] }}> {match.looseScore}</span>
                            </>
                          )}
                        </Card.Text>
                      </Card.Body>
                    </Card>
                  </Link>
                </Col>
              );
            })}
            <div className="d-flex justify-content-between align-items-center my-3">
              <Button disabled={matchPage === 1} onClick={() => setMatchPage(matchPage - 1)}>
                Назад
              </Button>
              <span>
                Страница {matchPage} из {Math.ceil(totalMatches / matchPageSize)}
              </span>
              <Button
                disabled={matchPage >= Math.ceil(totalMatches / matchPageSize)}
                onClick={() => setMatchPage(matchPage + 1)}
              >
                Вперёд
              </Button>
            </div>
          </>
        )}

        {activeTab === "participants" && (
          <>
            {hasRole("organizer") && <Link to={`/tournaments/${id}/add-participant`}>
              <Button variant="success" className="mb-3">Добавить участника</Button>
            </Link>}
            <Table striped bordered hover>
              <thead>
                <tr>
                  <th>Имя</th>
                  <th>Очки</th>
                  <th>Статус</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {tournament.participants.map(p => (
                  <tr key={p.id}>
                    <td>{p.name}</td>
                    <td>{p.points ?? "—"}</td>
                    <td>{ParticipantStatus[p.status].name ?? "—"}</td>
                    <td>
                      {hasRole("organizer") && <a href={`/participants/${p.id}/edit`} className="btn" role="button">Обновить</a>}
                    </td>
                  </tr>
                ))}
              </tbody>
            </Table>
          </>
        )}
      </div>
      <Modal show={showRegisterModal} onHide={closeRegisterModal} centered>
        <Modal.Header closeButton>
          <Modal.Title>Регистрация на турнир</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            <Form.Group controlId="registerName">
              <Form.Label>Имя участника</Form.Label>
              <Form.Control
                type="text"
                value={registerName}
                onChange={(e) => setRegisterName(e.target.value)}
                placeholder="Введите имя"
              />
            </Form.Group>
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={closeRegisterModal}>
            Отмена
          </Button>
          <Button variant="primary" onClick={handleRegister} disabled={!registerName.trim()}>
            Подтвердить
          </Button>
        </Modal.Footer>
      </Modal>
    </Container>
  );
};

export default TournamentDetails;
