import React, { useEffect, useState } from "react";
import {
    fetchDisciplines,
    fetchDisciplineById,
} from "../../api/disciplineApi";
import { fetchTournaments } from "../../api/tournamentApi";
import {
    Discipline,
    TournamentCleanDto,
    TournamentFormat,
    TournamentSortOptions,
    TournamentStatus,
} from "../../types";
import {
    Container,
    Row,
    Col,
    Form,
    Button,
    Card,
    Alert,
    Pagination,
} from "react-bootstrap";
import { Link } from "react-router-dom";
import { hasRole } from "../../utils/auth";
import TournamentCard from "./TournamentCard";

const pageSize = 10;

const TournamentList = () => {
    const [tournaments, setTournaments] = useState<TournamentCleanDto[]>([]);
    const [disciplines, setDisciplines] = useState<Discipline[]>([]);
    const [selectedDisciplineId, setSelectedDisciplineId] = useState<string | null>(null);
    const [selectedDiscipline, setSelectedDiscipline] = useState<Discipline | null>(null);
    const [status, setStatus] = useState<string>("");
    const [format, setFormat] = useState<string>("");
    const [startDate, setStartDate] = useState<string>("");
    const [endDate, setEndDate] = useState<string>("");
    const [searchString, setSearchString] = useState<string>("");
    const [total, setTotal] = useState(1);
    const [page, setPage] = useState(1);
    const [error, setError] = useState<string | null>("");
    const [sortOption, setSortOption] = useState<string>("0");
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        try {
            fetchDisciplines().then(setDisciplines);
        } catch (error) {
            setError("Ошибка получения дисциплин");
        }
    }, []);

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
                setError("Ошибка загрузки дисциплины:");
                setSelectedDiscipline(null);
            }
        };

        loadDiscipline();
    }, [selectedDisciplineId]);

    useEffect(() => {
        const loadTournaments = async () => {
            setLoading(true);
            const filter = new URLSearchParams({
                page: page.toString(),
                pageSize: pageSize.toString(),
            });
            if (sortOption) {
                filter.append('options', sortOption);
            }

            if (searchString) {
                filter.append('SearchString', searchString);
            }

            if (selectedDisciplineId) {
                filter.append('CategoryId', selectedDisciplineId);
            }

            if (status) {
                filter.append('Status', status);
            }

            if (format) {
                filter.append('Format', format);
            }

            if (startDate) {
                filter.append('StartTime', startDate);
            }

            if (endDate) {
                filter.append('EndTime', endDate);
            }
            try {
                var result = await fetchTournaments(filter.toString())
                setTournaments(result.tournaments);
                setTotal(result.total);
            } catch (err) {
                setError("Ошибка загрузки турниров:");
            } finally {
                setLoading(false);
            }
        };

        loadTournaments();
    }, [selectedDisciplineId, status, format, startDate, endDate, searchString, page, sortOption]);

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
                        <Form.Select value={selectedDisciplineId ?? ""} onChange={(e) => setSelectedDisciplineId(e.target.value || null)}>
                            <option value="">Все дисциплины</option>
                            {disciplines.map((d) => (
                                <option key={d.id} value={d.id}>
                                    {d.name}
                                </option>
                            ))}
                        </Form.Select>
                    </Form.Group>
                </Col>
                <Col md={4}>
                    <Form.Group>
                        <Form.Label>Поиск по названию</Form.Label>
                        <Form.Control
                            type="text"
                            value={searchString}
                            onChange={(e) => setSearchString(e.target.value)}
                            placeholder="Введите название"
                        />
                    </Form.Group>
                </Col>
            </Row>

            <Row className="mb-3">
                <Col md={3}>
                    <Form.Label>Статус</Form.Label>
                    <Form.Select value={status} onChange={(e) => setStatus(e.target.value)}>
                        <option value="">Все</option>
                        {TournamentStatus.map((key) => (
                            <option key={key.id} value={key.id}>{key.name}</option>
                        ))}
                    </Form.Select>
                </Col>
                <Col md={3}>
                    <Form.Label>Формат</Form.Label>
                    <Form.Select value={format} onChange={(e) => setFormat(e.target.value)}>
                        <option value="">Все</option>
                        {TournamentFormat.map((key) => (
                            <option key={key.id} value={key.id}>{key.name}</option>
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
                {TournamentSortOptions.map((d) => (
                    <option value={d.id}>{d.name}</option>
                ))}
                </Form.Select>
            </Form.Group>
            </Col>
            {hasRole("organizer") && (
              <Link to={`/tournaments/create`}>
                <Button variant="success" className="mb-3">Создать турнир</Button>
              </Link>
            )}
            {error && <Alert variant="danger">{error}</Alert>}

            <Row xs={1} md={2} className="g-4 mt-2">
                {tournaments.map((t) => (
                    <TournamentCard id={t.id} name={t.name} disciplineId={t.disciplineId} status={t.status} format={t.format} rounds={t.rounds} maxParticipants={t.maxParticipants} ownerId={t.ownerId} imagePath={t.imagePath}></TournamentCard>
                ))}
            </Row>
            <Pagination className="justify-content-center mt-4">
        <Pagination.Prev
          onClick={() => setPage(page - 1)}
          disabled={page === 1 || loading}
        >
          Назад
        </Pagination.Prev>

        {Array.from({ length: (total / pageSize + 1) }, (_, i) => (
          <Pagination.Item
            key={i + 1}
            active={i + 1 === page}
            onClick={() => setPage(i + 1)}
            disabled={loading}
          >
            {i + 1}
          </Pagination.Item>
        ))}

        <Pagination.Next
          onClick={() => setPage(page + 1)}
          disabled={page >= (total / pageSize) || loading}
        >
          Вперёд
        </Pagination.Next>
      </Pagination>
        </Container>
    );
};

export default TournamentList;
