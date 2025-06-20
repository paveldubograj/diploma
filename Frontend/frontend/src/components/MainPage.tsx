import { useEffect, useState } from "react";
import { Container, Row, Col, Card, Button, Pagination } from "react-bootstrap";
import NewsCard from "./NewsComponents/NewsCard";
import { fetchNews } from "../api/newsApi";
import { fetchTournaments } from "../api/tournamentApi";
import { ListNews, MatchStatus, TournamentCleanDto } from "../types";
import { Link } from "react-router-dom";

const PAGE_SIZE = 10;

const TournamentCard = ({
  id,
  name,
  discipline,
  status,
}: {
  id: string
  name: string;
  discipline: string;
  status: number;
}) => (
  <Link to={`/tournaments/${id}`} style={{ textDecoration: "none" }}>
    <Card className="mb-3">
      <Card.Body className="d-flex align-items-center">
        <div>
          <Card.Title className="mb-1">{name}</Card.Title>
          <Card.Text className="mb-0" style={{ fontSize: 15 }}>
            <strong>Дисциплина:</strong> {discipline}
          </Card.Text>
        </div>
        <span
          style={{
            minWidth: 90,
            fontWeight: 600,
            color: status === 1 ? "#198754" : "#6c757d",
            marginLeft: 16,
          }}
        >
          {MatchStatus[status]?.name || "Неизвестный статус"}
        </span>
      </Card.Body>
    </Card>
  </Link>
);

const MainPage = () => {
  const [newsList, setNewsList] = useState<ListNews[]>([]);
  const [tournaments, setTournaments] = useState<TournamentCleanDto[]>([]);
  const [page, setPage] = useState(1);
  const [newsTotal, setNewsTotal] = useState(0);
  const [loading, setLoading] = useState(false);

  // Загрузка новостей с пагинацией
  useEffect(() => {
    setLoading(true);
    const loadNews = (async () => {
      const res = await fetchNews(new URLSearchParams({
        page: page.toString(),
        pageSize: PAGE_SIZE.toString(),
      }).toString());
      setNewsList(res.news);
      setNewsTotal(res.total);
      setLoading(false);
    });
    loadNews();
  }, [page]);

  useEffect(() => {
    const loadTournaments = (async () => {
      const res = await fetchTournaments(new URLSearchParams({
        page: "1",
        pageSize: "10",
      }).toString());
      setTournaments(res.tournaments);
    })
    loadTournaments();
  }, []);

  const totalPages = Math.ceil(newsTotal / PAGE_SIZE);

  return (
    <Container fluid className="mt-4">
      <Row xs={2} md={2}>
        {/* Левый широкий столбец с новостями */}
        <Col md={8}>
          <h4 className="mb-3">Новости</h4>
          {loading && <div>Загрузка...</div>}
          {!loading && (
            <Row>
              {newsList.map((news) => (
                <NewsCard key={news.id} {...news} />
              ))}
            </Row>)}
          {/* Пагинация */}
          <Pagination className="justify-content-center mt-4">
            <Pagination.Prev
              onClick={() => setPage(page - 1)}
              disabled={page === 1 || loading}
            >
              Назад
            </Pagination.Prev>

            {Array.from({ length: (newsTotal / PAGE_SIZE + 1) }, (_, i) => (
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
              disabled={page >= (newsTotal / PAGE_SIZE) || loading}
            >
              Вперёд
            </Pagination.Next>
          </Pagination>
        </Col>
        {/* Правый узкий столбец с турнирами */}
        <Col md={4}>
          <h4 className="mb-3">Турниры</h4>
          {tournaments.map((t) => (
            <TournamentCard discipline={t.disciplineId} key={t.id} {...t} />
          ))}
        </Col>
      </Row>
    </Container>
  );
};

export default MainPage;