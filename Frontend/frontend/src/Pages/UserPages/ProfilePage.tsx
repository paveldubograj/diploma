import React, { useEffect, useState } from "react";
import { Button, Card, Form, Spinner, Alert, Row } from "react-bootstrap";
import { getProfile, updateProfile, deleteProfile } from "../../api/userApi";
import { useNavigate } from "react-router-dom";
import { ListNews, TournamentCleanDto, UserCleanDto } from "../../types";
import { hasRole } from "../../utils/auth";
import NewsCard from "../../components/NewsComponents/NewsCard";
import { fetchNewsByUser } from "../../api/newsApi";
import { getUser } from "../../api/AuthHook";
import TournamentCard from "../../components/TournamentComponents/TournamentCard";
import { fetchTournamentsByUser } from "../../api/tournamentApi";

const pageSize = 10;

const ProfilePage = () => {
  const [profile, setProfile] = useState<UserCleanDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [editing, setEditing] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [newsPage, setNewsPage] = useState<number>(1);
  const [newsTotal, setNewsTotal] = useState<number>(0);
  const [news, setNews] = useState<ListNews[]>([]);
  const [tournamentPage, setTournamentPage] = useState<number>(1);
  const [tournamentTotal, setTournamentTotal] = useState<number>(0);
  const [tournament, setTournament] = useState<TournamentCleanDto[]>([]);
  const navigate = useNavigate();

  useEffect(() => {
    getProfile()
      .then(setProfile)
      .catch(() => setError("Не удалось загрузить профиль"))
      .finally(() => setLoading(false));
  }, []);

  useEffect(() => {
    var u = getUser();
    if (hasRole("newsTeller")) {
      const loadNews = async () => {
        try {
          var result = [];
          if (u !== null) result = await fetchNewsByUser(u.id, newsPage, pageSize);
          setNews(result.news);
          setNewsTotal(result.total);
        } catch (err) {
          setError("Ошибка загрузки новостей");
        }
      };

      loadNews();
    }
  }, [newsPage]);

  useEffect(() => {
    var u = getUser();
    if (hasRole("organizer")) {
      const loadTournaments = async () => {
        try {
          var result = [];
          if (u !== null) result = await fetchTournamentsByUser(u.id, newsPage, pageSize);
          setTournament(result.tournaments);
          setTournamentTotal(result.total);
        } catch (err) {
          setError("Ошибка загрузки турниров");
        }
      };

      loadTournaments();
    }
  }, [newsPage]);

  const handleUpdate = async () => {
    if (!profile) return;
    try {
      await updateProfile(profile);
      setEditing(false);
    } catch {
      setError("Ошибка при обновлении профиля");
    }
  };

  const handleDelete = async () => {
    if (!window.confirm("Вы уверены, что хотите удалить свой профиль?")) return;
    try {
      await deleteProfile();
      localStorage.removeItem("token");
      navigate("/login");
    } catch {
      setError("Ошибка при удалении профиля");
    }
  };

  if (loading) return <Spinner animation="border" />;
  if (!profile) return <Alert variant="danger">Профиль не найден</Alert>;

  return (
    <div className="flex-grow-1">
      <Card className="p-4">
        <h2>Профиль</h2>
        {error && <Alert variant="danger">{error}</Alert>}
        <Form>
          <Form.Group className="mb-3">
            <Form.Label>Имя пользователя</Form.Label>
            <Form.Control
              type="text"
              value={profile.userName}
              onChange={(e) => setProfile({ ...profile, userName: e.target.value })}
              disabled={!editing}
            />
          </Form.Group>
          <Form.Group className="mb-3">
            <Form.Label>Email</Form.Label>
            <Form.Control
              type="email"
              value={profile.email}
              onChange={(e) => setProfile({ ...profile, email: e.target.value })}
              disabled={!editing}
            />
          </Form.Group>
          <Form.Group className="mb-3">
            <Form.Label>Bio</Form.Label>
            <Form.Control
              type="text"
              value={profile.bio}
              onChange={(e) => setProfile({ ...profile, bio: e.target.value })}
              disabled={!editing}
            />
          </Form.Group>

          {editing ? (
            <>
              <Button variant="success" onClick={handleUpdate} className="me-2">
                Сохранить
              </Button>
              <Button variant="secondary" onClick={() => setEditing(false)}>
                Отмена
              </Button>
            </>
          ) : (
            <Button variant="primary" onClick={() => setEditing(true)} className="me-2">
              Редактировать
            </Button>
          )}
          {hasRole("newsTeller") && (
            <>
              <h2>Новости, опубликованные вами</h2>
              <Row xs={1} md={2} className="g-4">
                {news.map((n) => (
                  <NewsCard id={n.id} title={n.title} authorId={n.authorId} authorName={n.authorName} publishingDate={n.publishingDate} categoryId={n.categoryId} imagePath={n.imagePath}></NewsCard>
                ))}
              </Row>
              <div className="d-flex justify-content-between align-items-center my-3">
                <Button disabled={newsPage === 1} onClick={() => setNewsPage(newsPage - 1)}>
                  Назад
                </Button>
                <span>
                  Страница {newsPage} из {Math.ceil(newsTotal / pageSize)}
                </span>
                <Button
                  disabled={newsPage >= Math.ceil(newsTotal / pageSize)}
                  onClick={() => setNewsPage(newsPage + 1)}
                >
                  Вперёд
                </Button>
              </div>
            </>
          )}

          {hasRole("organizer") && (
            <>
              <h2>Турниры, проводимые Вами</h2>
              <Row xs={1} md={2} className="g-4">
                {tournament.map((t) => (
                  <TournamentCard id={t.id} name={t.name} disciplineId={t.disciplineId} status={t.status} format={t.format} rounds={t.rounds} maxParticipants={t.maxParticipants} ownerId={t.ownerId} imagePath={t.imagePath}></TournamentCard>
                ))}
              </Row>
              <div className="d-flex justify-content-between align-items-center my-3">
                <Button disabled={tournamentPage === 1} onClick={() => setTournamentPage(tournamentPage - 1)}>
                  Назад
                </Button>
                <span>
                  Страница {tournamentPage} из {Math.ceil(tournamentTotal / pageSize)}
                </span>
                <Button
                  disabled={tournamentPage >= Math.ceil(tournamentTotal / pageSize)}
                  onClick={() => setTournamentPage(tournamentPage + 1)}
                >
                  Вперёд
                </Button>
              </div>
            </>
          )}

          <Button variant="danger" onClick={handleDelete} className="ms-2">
            Удалить профиль
          </Button>
        </Form>

      </Card>
    </div>
  );
};

export default ProfilePage;
