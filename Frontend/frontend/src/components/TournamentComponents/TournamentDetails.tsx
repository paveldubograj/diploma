import React, { useEffect, useState } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
import { fetchTournamentById, updateTournament, deleteTournament, startTournament, endTournament, setNextRound, generateBracket } from "../../api/tournamentApi";
import { MatchList, ParticipantStatus, TournamentDto, TournamentFormat, TournamentStatus } from "../../types";
import { Button, Card, Container, Row, Col, Form, Pagination, Table, Nav, Alert } from "react-bootstrap";
import { fetchMatches } from "../../api/matchApi";

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
  const [matchHasMore, setMatchHasMore] = useState(false);
  const [participantHasMore, setParticipantHasMore] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
try {
      if (id) {
        fetchTournamentById(id).then(setTournament);
      }
} catch (error) {
  setError("Ошибка получения турнира");
}
  }, [id]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    if (!formData) return;
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleDateChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (!formData) return;
    setFormData({ ...formData, [e.target.name]: new Date(e.target.value) });
  };

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
          setMatches(result.items || result); // если API возвращает { items, total }
          //setTotalMatches(result.total || result.length); // если API возвращает total
        }
      } catch (err) {
        console.error("Ошибка загрузки матчей:", err);
      } finally {
        setLoadingMatches(false);
      }
    };

    loadMatches();
  }, [id, matchPage, matchPageSize]);

  const totalPages = Math.ceil(totalMatches / matchPageSize);

  if (!tournament) return <div>Загрузка...</div>;
  //if (!formData) return <div>Загрузка...</div>;

  const renderField = (label: string, value: any, name: string, type = "text") => (
    <Form.Group as={Row} className="mb-2">
      <Form.Label column sm={4}>{label}:</Form.Label>
      <Col sm={8}>
        {editMode ? (
          <Form.Control
            type={type}
            name={name}
            value={formData ? (type === "date" ? (name === "startDate" ? new Date(formData.startDate).toISOString().split("T")[0] : new Date(formData.endDate).toISOString().split("T")[0]) : value) : ""}
            onChange={type === "date" ? handleDateChange : handleChange}
          />
        ) : (
          <Form.Control plaintext readOnly defaultValue={value} />
        )}
      </Col>
    </Form.Group>
  );


  return (
    <Container>
      <h2>{tournament.name}</h2>
      <Card className="mb-4">
        <Card.Body>
          <Form>
            {renderField("Формат", TournamentFormat[tournament.format].name, "format")}
            {renderField("Статус", TournamentStatus[tournament.status].name, "status")}
            {renderField("Количество раундов", tournament.rounds, "rounds")}
            {renderField("Макс. участников", tournament.maxParticipants, "maxParticipants")}
            {renderField("Дата начала", tournament.startDate, "startDate", "date")}
            {renderField("Дата конца", tournament.endDate, "endDate", "date")}
            {renderField("Организатор", tournament.ownerId, "ownerId")}
            {renderField("Победитель", tournament.winnerId || "–", "winnerId")}
            {error && <Alert variant="danger">{error}</Alert>}

            {editMode ? (
              <Button onClick={handleSave} className="me-2">Сохранить</Button>
            ) : (
              <Button onClick={() => {
                setEditMode(true);
                setFormData(tournament);
              }} className="me-2">Редактировать</Button>
            )}
            <Button variant="danger" onClick={handleDelete} className="me-2">Удалить</Button>
          </Form>
        </Card.Body>
      </Card>

      <Row className="mb-4">
        <Col>
          <Button onClick={() => handleAction(() => startTournament(tournament.id))}>Запустить</Button>{" "}
          <Button onClick={() => handleAction(() => setNextRound(tournament.id))}>След. раунд</Button>{" "}
          <Button onClick={() => handleAction(() => endTournament(tournament.id))}>Завершить</Button>{" "}
          <Button onClick={() => handleAction(() => generateBracket(tournament.id))}>Сгенерировать сетку</Button>
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
            {matches.map((match) => (
              <div key={match.id} className="border p-2 mb-2">
                <Col md={6} lg={4} key={match.id} className="mb-3">
                  <Link to={`/matches/${match.id}`} style={{ textDecoration: "none" }}>
                    <Card className="h-100">
                      <Card.Body>
                        <Card.Title>{match.round} (#{match.matchOrder})</Card.Title>
                        <Card.Text>
                          Участники:<br />
                          {match.participant1Id}<br />
                          {match.participant2Id}
                          {match.status === 2 && (
                            <>
                              <br />
                              Счёт: {match.winScore} - {match.looseScore}
                            </>
                          )}
                        </Card.Text>
                      </Card.Body>
                    </Card>
                  </Link>
                </Col>
              </div>
            ))}
            <Pagination>
              <Pagination.Prev
                onClick={() => setMatchPage((p) => Math.max(p - 1, 1))}
                disabled={matchPage === 1}
              />
              <Pagination.Item active>{matchPage}</Pagination.Item>
              <Pagination.Next
                onClick={() => matchHasMore && setMatchPage((p) => p + 1)}
                disabled={!matchHasMore}
              />
            </Pagination>
          </>
        )}

        {activeTab === "participants" && (
          <>
            <Link to={`/tournaments/${id}/add-participant`}>
              <Button variant="success" className="mb-3">Добавить участника</Button>
            </Link>
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
                      <a href={`/participants/${p.id}/edit`} className="btn" role="button">Обновить</a>
                    </td>
                  </tr>
                ))}
              </tbody>
            </Table>
          </>
        )}
      </div>
    </Container>
  );
};

export default TournamentDetails;
