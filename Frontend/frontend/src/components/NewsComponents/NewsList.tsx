import React, { useEffect, useState } from "react";
import {
  fetchNews,
  fetchTags,
} from "../../api/newsApi";
import {
  fetchDisciplines,
  fetchDisciplineById,
} from "../../api/disciplineApi"
import { ListNews, Discipline, Tag, NewsSortOptions } from "../../types";
import { Container, Row, Col, Form, Button, Card, Alert, Spinner, Pagination } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import NewsCard from "./NewsCard";
import { hasRole } from "../../utils/auth";

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
  const [sortOption, setSortOption] = useState<string>("0");
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

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
        console.error("Ошибка загрузки дисциплины", err);
        setSelectedDiscipline(null);
      }
    };

    loadDiscipline();
  }, [selectedDisciplineId]);

  // Загрузка новостей
  useEffect(() => {
    const loadNews = async () => {
      setLoading(true);
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
      if (sortOption !== null) {
        filter.append("sortOptions", sortOption.toString());
      }

      try {
        const result = await fetchNews(filter.toString());
        setNews(result.news);
        setTotal(result.total);
      } catch (err) {
        setError("Ошибка загрузки новостей:");
      } finally {
        setLoading(false);
      }
    };

    loadNews();
  }, [page, searchString, selectedTags, selectedDisciplineId, sortOption]);

  const handleDisciplineChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setSelectedDisciplineId(e.target.value || null);
    setPage(1); // сброс страницы при смене дисциплины
  };

  return (
    <Container className="my-4">
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
            <Form.Label>Сортировка</Form.Label>
            <Form.Select
              value={sortOption}
              onChange={(e) => {
                setSortOption(e.target.value);
                setPage(1);
              }}
            >
              {NewsSortOptions.map((d) => (
                <option value={d.id}>{d.name}</option>
              ))}
            </Form.Select>
          </Form.Group>
        </Col>

        <Col md={16}>
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
      </Row>

      {hasRole('newsTeller') && (
        <Button className="mt-2 mb-2" onClick={() => navigate('/news/create')}>Добавить новость</Button>
      )}

      {/* Сообщения об ошибках или загрузке */}
      {error && <Alert variant="danger">{error}</Alert>}
      {loading && (
        <div className="text-center my-4">
          <Spinner animation="border" role="status">
            <span className="visually-hidden">Загрузка...</span>
          </Spinner>
        </div>
      )}

      <Row xs={1} md={2} className="g-4">
        {news.map((n) => (
          <NewsCard id={n.id} title={n.title} authorId={n.authorId} authorName={n.authorName} publishingDate={n.publishingDate} categoryId={n.categoryId} imagePath={n.imagePath}></NewsCard>
        ))}
      </Row>

      {news.length === 0 && !loading && (
        <p className="text-center mt-4">Новостей не найдено.</p>
      )}

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

export default NewsList;
