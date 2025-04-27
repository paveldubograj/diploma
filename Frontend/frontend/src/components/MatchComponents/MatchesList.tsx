import React, { useEffect, useState } from "react";
import {
  fetchMatches,
} from "../../api/matchApi";
import {
  fetchDisciplines,
  fetchDisciplineById,
} from "../../api/disciplineApi"
import { MatchList, Discipline, MatchStatus } from "../../types";
import { Container, Row, Col, Form, Button, Card, Alert } from "react-bootstrap";
import { Link } from "react-router-dom";
import ErrorBoundary from "../ErrorBoundary";

const pageSize = 10;

// const MatchStatus = [
//     {id: 0, name:"Запланирован"},
//     {id: 1, name:"Идёт"},
//     {id: 2, name:"Завершен"},
//     {id: 3, name:"Отменён"},
//     {id: 4, name:"Перенесён"}
//   ];

const MatchesList = () => {
  const [matches, setMatches] = useState<MatchList[]>([]);
  const [total, setTotal] = useState(1);
  const [page, setPage] = useState(1);
  const [disciplines, setDisciplines] = useState<Discipline[]>([]);
  const [selectedDisciplineId, setSelectedDisciplineId] = useState<string | null>(null);
  const [selectedDiscipline, setSelectedDiscipline] = useState<Discipline | null>(null);
  const [status, setStatus] = useState<string>("");
  const [startDate, setStartDate] = useState<string>("");
  const [endDate, setEndDate] = useState<string>("");
  const [error, setError] = useState<string | null>(null);

  // Загрузка тегов и дисциплин при первом рендере
  useEffect(() => {
    try {
      fetchDisciplines().then(setDisciplines);
    } catch (error) {
      console.error("Ошибка загрузки дисциплин:", error);
      setError("Ошибка загрузки дисциплин");
    }
  }, []);

  // Загрузка конкретной дисциплины при выборе
  useEffect(() => {
    if (!selectedDisciplineId) {
      setSelectedDiscipline(null);
      return;
    }

    const loadDiscipline = async () => {
      try {
        const disc = await fetchDisciplineById(selectedDisciplineId);
        setSelectedDiscipline(disc);
      } catch (err) {
        console.error("Ошибка загрузки дисциплины:", err);
        setSelectedDiscipline(null);
      }
    };

    loadDiscipline();
  }, [selectedDisciplineId]);

  useEffect(() => {
    const loadNews = async () => {
      const filter = new URLSearchParams({
        page: page.toString(),
        pageSize: pageSize.toString(),
      });

      if (selectedDisciplineId) {
        filter.append('CategoryId', selectedDisciplineId);
      }

      if (startDate) {
        filter.append('StartDate', startDate);
      }

      if (endDate) {
        filter.append('EndDate', endDate);
      }

      if (status) {
        filter.append('Status', status);
      }

      try {
        const result = await fetchMatches(filter.toString());
        setMatches(result);
        //setTotal(result.total);
      } catch (err) {
        console.error("Ошибка загрузки матчей:", err);
        setError("Ошибка загрузки матчей");
      }
    };

    loadNews();
  }, [page, selectedDisciplineId, status, startDate, endDate]);

  const handleDisciplineChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setSelectedDisciplineId(e.target.value || null);
    setPage(1); // сброс страницы при смене дисциплины
  };

  return (
    <Container>
      {selectedDiscipline && (
        <Card className="mb-4">
          <Card.Body>
            <Card.Title>{selectedDiscipline.name}</Card.Title>
            <Card.Text>{selectedDiscipline.description}</Card.Text>
          </Card.Body>
        </Card>
      )}

      

      <Row className="mb-3">
        <Col md={4}>
          <Form.Group>
            <Form.Label>Дисциплина</Form.Label>
            <Form.Select value={selectedDisciplineId ?? ""} onChange={handleDisciplineChange}>
              <option value="">Все дисциплины</option>
              {disciplines.map((d) => (
                <option key={d.id} value={d.id}>
                  {d.name}
                </option>
              ))}
            </Form.Select>
          </Form.Group>
        </Col>
      </Row>

      <Row className="mb-3">
        <Col md={3}>
          <Form.Label>Статус</Form.Label>
          <Form.Select value={status} onChange={(e) => setStatus(e.target.value)}>
            <option value="">Все</option>
            {MatchStatus.map((statuse) => (
              <option value={statuse.id}>{statuse.name}</option>
            ))}
          </Form.Select>
        </Col>
        <Col md={3}>
          <Form.Label>Дата начала</Form.Label>
          <Form.Control type="date" value={startDate} onChange={(e) => setStartDate(e.target.value)} />
        </Col>
        <Col md={3}>
          <Form.Label>Дата окончания</Form.Label>
          <Form.Control type="date" value={endDate} onChange={(e) => setEndDate(e.target.value)} />
        </Col>
      </Row>

      {error && <Alert variant="danger">{error}</Alert>}

      <Row>
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
                      {match.participant1Id}<br />
                      {match.participant2Id}
                      {isCompleted && (
                        <>
                          <br />
                          <span style={{ color: winnerColor[match.participant1Id] }}>Счет: {match.winScore}</span> -
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
      </Row>
    </Container>
  );
};

export default MatchesList;