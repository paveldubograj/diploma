import React, { useEffect, useState } from "react";
import {
  fetchMatches,
} from "../../api/matchApi";
import {
  fetchDisciplines,
  fetchDisciplineById,
} from "../../api/disciplineApi"
import { MatchList, Discipline, MatchStatus, MatchSortOptions } from "../../types";
import { Container, Row, Col, Form, Button, Card, Alert } from "react-bootstrap";
import { Link } from "react-router-dom";
import ErrorBoundary from "../ErrorBoundary";
import MatchCard from "./MatchCard";

const pageSize = 10;

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
  const [sortOption, setSortOption] = useState<string>("0");

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

      if(sortOption){
        filter.append('options', sortOption)
      }

      if (selectedDisciplineId) {
        filter.append('CategoryId', selectedDisciplineId);
      }

      if (startDate) {
        filter.append('StartTime', startDate);
      }

      if (endDate) {
        filter.append('EndTime', endDate);
      }

      if (status) {
        filter.append('Status', status);
      }

      try {
        const result = await fetchMatches(filter.toString());
        setMatches(result.matches);
        setTotal(result.total);
      } catch (err) {
        console.error("Ошибка загрузки матчей:", err);
        setError("Ошибка загрузки матчей");
      }
    };

    loadNews();
  }, [page, selectedDisciplineId, status, startDate, endDate, sortOption]);

  const handleDisciplineChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setSelectedDisciplineId(e.target.value || null);
    setPage(1); 
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

      <Col md={4}>
          <Form.Group>
            <Form.Label>Сортировка</Form.Label>
            <Form.Select
              value={sortOption}
              onChange={(e) => {
                setSortOption(e.target.value);
                setPage(1);
              }}
            >
              {MatchSortOptions.map((d) => (
                <option value={d.id}>{d.name}</option>
              ))}
            </Form.Select>
          </Form.Group>
        </Col>

      {error && <Alert variant="danger">{error}</Alert>}

      <Row xs={1} md={2} className="g-4">
        {matches.map((match) => (
          <MatchCard id={match.id} round={match.round} startTime={match.startTime} status={match.status} matchOrder={match.matchOrder} winScore={match.winScore} looseScore={match.looseScore} endTime={match.endTime} participant1Name={match.participant1Name} participant2Name={match.participant2Name} tournamentName={match.tournamentName} categoryId={match.categoryId} winnerId={match.winnerId} participant1Id={match.participant1Id} participant2Id={match.participant2Id} tournamentId={match.tournamentId}></MatchCard>
        ))}
      </Row>
      <div className="d-flex justify-content-between align-items-center my-3">
        <Button disabled={page === 1} onClick={() => setPage(page - 1)}>
          Назад
        </Button>
        <span>
          Страница {page} из {Math.ceil(total / pageSize)}
        </span>
        <Button
          disabled={page >= Math.ceil(total / pageSize)}
          onClick={() => setPage(page + 1)}
        >
          Вперёд
        </Button>
      </div>
    </Container>
  );
};

export default MatchesList;