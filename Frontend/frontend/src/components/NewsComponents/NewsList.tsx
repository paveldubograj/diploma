import React, { useEffect, useState } from "react";
import {
  fetchNews,
  fetchTags,
} from "../../api/newsApi";
import {
  fetchDisciplines,
  fetchDisciplineById,
} from "../../api/disciplineApi"
import { ListNews, Discipline, Tag } from "../../types";
import { Container, Row, Col, Form, Button, Card, Alert } from "react-bootstrap";
import { Link } from "react-router-dom";

const pageSize = 10;

const NewsList = () => {
  const [news, setNews] = useState<ListNews[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [searchString, setSearchString] = useState("");
  const [tags, setTags] = useState<Tag[]>([]);
  const [selectedTags, setSelectedTags] = useState<string[]>([]);
  const [disciplines, setDisciplines] = useState<Discipline[]>([]);
  const [selectedDisciplineId, setSelectedDisciplineId] = useState<string | null>(null);
  const [selectedDiscipline, setSelectedDiscipline] = useState<Discipline | null>(null);
  const [error, setError] = useState<string | null>(null);

  // Загрузка тегов и дисциплин при первом рендере
  useEffect(() => {
    try {
      fetchTags().then(setTags);
    } catch (error) {
      setError("Не удалось получить теги");
    }
    try {
      fetchDisciplines().then(setDisciplines);
    } catch (error) {
      setError("Не удалось получить дисциплины");
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

  // Загрузка новостей
  useEffect(() => {
    const loadNews = async () => {
      const filter = new URLSearchParams({
        page: page.toString(),
        pageSize: pageSize.toString(),
      });

      if (searchString) {
        filter.append('SearchString', searchString);
      }
      selectedTags.forEach(tagId =>
        filter.append('Tags', tagId)
      );
      if (selectedDisciplineId) {
        filter.append('CategoryId', selectedDisciplineId);
      }

      try {
        const result = await fetchNews(filter.toString());
        setNews(result.news);
        setTotal(result.total);
      } catch (err) {
        setError("Ошибка загрузки новостей:");
      }
    };

    loadNews();
  }, [page, searchString, selectedTags, selectedDisciplineId]);

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

        <Col md={4}>
          <Form.Group>
            <Form.Label>Поиск</Form.Label>
            <Form.Control
              type="text"
              value={searchString}
              onChange={(e) => {
                setSearchString(e.target.value);
                setPage(1);
              }}
            />
          </Form.Group>
        </Col>

        <Col md={4}>
          <Form.Group>
            <Form.Label>Теги</Form.Label>
            <Form.Select
              multiple
              value={selectedTags}
              onChange={(e) => {
                const values = Array.from(e.target.selectedOptions, (option) => option.value);
                setSelectedTags(values);
                setPage(1);
              }}
            >
              {tags.map((tag) => (
                <option key={tag.id} value={tag.id}>
                  {tag.name}
                </option>
              ))}
            </Form.Select>
          </Form.Group>
        </Col>
      </Row>

      {error && <Alert variant="danger">{error}</Alert>}
      {news.map((n) => (
        <Card key={n.id} className="mb-3">
          <Card.Body>
            <Card.Title><Link to={`/news/${n.id}`}>{n.title}</Link></Card.Title>
          </Card.Body>
        </Card>
      ))}

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

export default NewsList;
