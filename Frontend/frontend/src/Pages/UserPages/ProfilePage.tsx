import React, { useEffect, useState } from "react";
import { Button, Card, Form, Spinner, Alert, Row, Image, Modal, Col } from "react-bootstrap";
import { getProfile, updateProfile, deleteProfile, getUserRolesById, addUserRole, deleteUserRole } from "../../api/userApi";
import { useNavigate } from "react-router-dom";
import { ListNews, RoleDto, TournamentCleanDto, UserProfileDto, UserUpdatedto } from "../../types";
import { hasRole } from "../../utils/auth";
import NewsCard from "../../components/NewsComponents/NewsCard";
import { fetchNewsByUser } from "../../api/newsApi";
import { getUser } from "../../api/AuthHook";
import TournamentCard from "../../components/TournamentComponents/TournamentCard";
import { fetchTournamentsByUser, fetchTournamentsByIds } from "../../api/tournamentApi";
import { generateSVGPlaceholder } from "../../utils/PlaceholdGenerator";
import ProfileImageUploader from "../../components/UserComponents/UserImagesUploader";

const pageSize = 10;

const allRoles = ["admin", "organizer", "newsTeller"];

const ProfilePage = () => {
  const [profile, setProfile] = useState<UserProfileDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [editing, setEditing] = useState(false);
  const [newsPage, setNewsPage] = useState<number>(1);
  const [newsTotal, setNewsTotal] = useState<number>(0);
  const [news, setNews] = useState<ListNews[]>([]);
  const [pTournaments, setPTournaments] = useState<TournamentCleanDto[]>([]);
  const [tournamentPage, setTournamentPage] = useState<number>(1);
  const [tournamentTotal, setTournamentTotal] = useState<number>(0);
  const [tournament, setTournament] = useState<TournamentCleanDto[]>([]);
  const [roles, setRoles] = useState<RoleDto[]>([]);
  const [showModal, setShowModal] = useState(false);
  const [modalAction, setModalAction] = useState<"add" | "delete">("add");
  const [modalRoles, setModalRoles] = useState<string[]>(["admin", "organizer", "newsTeller"]);
  const [selectedRole, setSelectedRole] = useState<string>("");
  const [error, setError] = useState<string | null>(null);
  const [tournamentError, setTournamentError] = useState<string | null>(null);
  const [newsError, setNewsError] = useState<string | null>(null);
  const [pTournamentError, setPTournamentError] = useState<string | null>(null);
  const [rolesError, setRolesError] = useState<string | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    getProfile()
      .then(setProfile).then(console.log)
      .catch(() => setError("Не удалось загрузить профиль"))
      .finally(() => setLoading(false));
  }, []);

  useEffect(() => {
    if (profile !== null && (getUser()?.id === profile.id || hasRole("admin"))) {
      getUserRolesById(profile.id).then(setRoles)
        .catch(() => setRolesError("Не удалось получить роли"));
    }
  }, [profile]);

  useEffect(() => {
    if (profile !== null && profile.tournaments){
      fetchTournamentsByIds(profile.tournaments).then(setPTournaments)
        .catch(() => setPTournamentError("Не удалось получить турниры"));
    }
  }, [profile])

  useEffect(() => {
    var u = getUser();
    if (hasRole("newsTeller")) {
      const loadNews = async () => {
        try {
          if (u !== null) {
          var result = await fetchNewsByUser(u.id, newsPage, pageSize);
          setNews(result.news);
          setNewsTotal(result.total);
        }
        } catch (err) {
          setNewsError("Ошибка загрузки новостей");
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
          if (u !== null) {
          var result = await fetchTournamentsByUser(u.id, tournamentPage, pageSize);
          setTournament(result.tournaments);
          setTournamentTotal(result.total);
        }
        } catch (err) {
          setTournamentError("Ошибка загрузки турниров");
        }
      };

      loadTournaments();
    }
  }, [tournamentPage]);

  const handleUpdate = async () => {
    if (!profile) return;
    if (!profile.tournaments) profile.tournaments = [];
    var dto: UserUpdatedto = profile;
    try {
      await updateProfile(dto);
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

  const handleOpenModal = (action: "add" | "delete") => {
    setModalAction(action);
    if(action === "delete"){
      setModalRoles(roles.map((r) => r.name));
      setSelectedRole(modalRoles[0]);
    }
    else{
      var res: string[] = [];
      var ur = roles.map((r) => r.name);
      allRoles.forEach(element => {
        if(!ur.includes(element)) res.push(element);
      });
      setModalRoles(res);
      setSelectedRole(modalRoles[0]);
    }
    setShowModal(true);
  };

  const handleRoleSubmit = async (role: string) => {
    if (modalAction === "add") {
      await addUserRole(profile.id, selectedRole);
    } else {
      await deleteUserRole(profile.id, selectedRole);
    }
  };
  const placeholderWidth = 300;
  const placeholderHeight = 400;

  return (
    <div className="flex-grow-1">
      <Card className="p-4 mb-4">
        <h2>Профиль</h2>
        <Image src={profile.image ? "http://localhost:5153/" + profile.image :
          generateSVGPlaceholder(placeholderWidth, placeholderHeight, 'Нет изображения')
        } className="mb-3" style={{ maxWidth: "300px", height: "400px" }} />
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
            <Form.Label>О себе</Form.Label>
            <Form.Control
              type="text"
              value={profile.bio}
              onChange={(e) => setProfile({ ...profile, bio: e.target.value })}
              disabled={!editing}
            />
          </Form.Group>

          {editing && (<ProfileImageUploader
                profileId={profile.id}
                currentImageUrl={profile.image}
                onImageChange={(profile) => {
                  setProfile(profile);
                }}
              />)}

          {(getUser()?.id === profile.id || hasRole("admin")) && (
          <div>
            <p>Роли: </p>
            {rolesError && <Alert variant="danger">{rolesError}</Alert>}
            <Row>
              {roles.map((r) => (
                <Col md={1} lg={2} key={r.name} className="mb-3">
                  <Card>
                    <Card.Body>{r.name}</Card.Body>
                  </Card>
                </Col>
              ))}
            </Row>
          </div>)}

          {hasRole("admin") && (
            <div className="d-flex gap-2 mt-2">
              <Button variant="success" onClick={() => handleOpenModal("add")}>
                Добавить роль
              </Button>
              <Button variant="danger" onClick={() => handleOpenModal("delete")}>
                Удалить роль
              </Button>
              <RoleModal
                show={showModal}
                onHide={() => setShowModal(false)}
                onSubmit={handleRoleSubmit}
                action={modalAction}
                roles={modalRoles}
                selectedRole={selectedRole}
                onChangeRole={setSelectedRole}
              />
            </div>
          )}

          {getUser()?.id === profile.id && (editing ? (
            <div className="d-flex gap-2 mt-3">
              <Button variant="success" onClick={handleUpdate} className="me-2">
                Сохранить
              </Button>
              <Button variant="secondary" onClick={() => setEditing(false)}>
                Отмена
              </Button>
            </div>
          ) : (
            <div className="d-flex gap-2 mt-3">
              <Button variant="primary" onClick={() => setEditing(true)} className="me-2">
                Редактировать
              </Button>
            </div>
          ))}
          {profile.tournaments && (
            <>
              <h2>Турниры, в которых вы участвуете</h2>
              {pTournamentError && <Alert variant="danger">{pTournamentError}</Alert>}
              <Row xs={1} md={2} className="g-4">
                {pTournaments.map((t) => (
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
          {hasRole("organizer") && (
            <>
              <h2>Турниры, проводимые Вами</h2>
              {tournamentError && <Alert variant="danger">{tournamentError}</Alert>}
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
          {hasRole("newsTeller") && (
            <>
              <h2>Новости, опубликованные вами</h2>
              {newsError && <Alert variant="danger">{newsError}</Alert>}
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


          {getUser()?.id === profile.id && <Button variant="danger" onClick={handleDelete} className="ms-2">
            Удалить профиль
          </Button>}

        </Form>

      </Card>

    </div>
  );
};

export default ProfilePage;

type RoleModalProps = {
  show: boolean;
  onHide: () => void;
  onSubmit: (role: string) => void;
  action: "add" | "delete";
  roles: string[];
  selectedRole: string;
  onChangeRole: (role: string) => void;
};

const RoleModal: React.FC<RoleModalProps> = ({ show, onHide, onSubmit, action, roles, selectedRole, onChangeRole }) => {

  const handleSubmit = () => {
    onSubmit(selectedRole);
    onHide();
  };

  return (
    <Modal show={show} onHide={onHide} centered>
      <Modal.Header closeButton>
        <Modal.Title>{action === "add" ? "Добавить роль" : "Удалить роль"}</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <Form.Group>
          <Form.Label>Выберите роль:</Form.Label>
          <Form.Control
            as="select"
            value={selectedRole}
            onChange={(e) => onChangeRole(e.target.value)}>
            {roles.map((role) => (
              <option key={role} value={role}>
                {role}
              </option>
            ))}
          </Form.Control>
        </Form.Group>
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={onHide}>
          Отмена
        </Button>
        <Button variant={action === "add" ? "success" : "danger"} onClick={handleSubmit}>
          {action === "add" ? "Добавить" : "Удалить"}
        </Button>
      </Modal.Footer>
    </Modal>
  );
};
