import React, { useEffect, useState } from "react";
import { Alert, Button, Card, Spinner, Image, Form, Row, Col, Modal } from "react-bootstrap";
import { useNavigate, useParams } from "react-router-dom";
import { getUserById, deleteUserById, getUserRolesById, updateProfile, addUserRole, deleteUserRole } from "../../api/userApi";
import { ListNews, RoleDto, TournamentCleanDto, UserProfileDto, UserUpdatedto } from "../../types";
import { getUser } from "../../api/AuthHook";
import { hasRole } from "../../utils/auth";
import { fetchTournamentsByIds, fetchTournamentsByUser } from "../../api/tournamentApi";
import { fetchNewsByUser } from "../../api/newsApi";
import TournamentCard from "../../components/TournamentComponents/TournamentCard";
import NewsCard from "../../components/NewsComponents/NewsCard";
import { generateSVGPlaceholder } from "../../utils/PlaceholdGenerator";

const pageSize = 10; 

const allRoles = ["admin", "organizer", "newsTeller"];

const UserDetailsAdmin: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [user, setUser] = useState<UserProfileDto | null>(null);
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
    const fetchUser = async () => {
      if (id) {
        const data = await getUserById(id);
        setUser(data);
      }
    };
    fetchUser();
    setLoading(false);
  }, [id]);

  const handleDelete = async () => {
    if (id && window.confirm("Вы уверены, что хотите удалить пользователя?")) {
      await deleteUserById(id);
      navigate("/admin/users");
    }
  };

  useEffect(() => {
    if (user !== null && (getUser()?.id === user.id || hasRole("admin"))) {
      getUserRolesById(user.id).then(setRoles)
        .catch(() => setRolesError("Не удалось получить роли"));
    }
  }, [user]);

  useEffect(() => {
    if (user !== null && user.tournaments){
      fetchTournamentsByIds(user.tournaments).then(setPTournaments)
        .catch(() => setTournamentError("Не удалось получить турниры"));
    }
  })

  useEffect(() => {
    var u = getUser();
    if (roles.find(n => n.name === "newsTeller") !== undefined) {
      const loadNews = async () => {
        try {
          if (u !== null) {
            var result = await fetchNewsByUser(user!.id, newsPage, pageSize);
          setNews(result.news);
          setNewsTotal(result.total);
        }
        } catch (err) {
          setNewsError("Ошибка загрузки новостей");
        }
      };

      loadNews();
    }
  }, [newsPage, roles]);

  useEffect(() => {
    var u = getUser();
    if (roles.find(n => n.name === "organizer") !== undefined) {
      const loadTournaments = async () => {
        try {
          if (u !== null){
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
  }, [roles, tournamentPage]);

  const handleUpdate = async () => {
    if (!user) return;
    if (!user.tournaments) user.tournaments = [];
    var dto: UserUpdatedto = user;
    try {
      await updateProfile(dto);
      setEditing(false);
    } catch {
      setError("Ошибка при обновлении профиля");
    }
  };

  if (loading) return <Spinner animation="border" />;
  if (!user) return <Alert variant="danger">Профиль не найден</Alert>;

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
      await addUserRole(user.id, selectedRole);
    } else {
      await deleteUserRole(user.id, selectedRole);
    }
  };

  const placeholderWidth = 300;
  const placeholderHeight = 400;

  if (!user) return <div>Загрузка...</div>;

  return (
    <div className="flex-grow-1">
      <Card className="p-4 mb-4">
        <h2>Профиль</h2>
        <Image src={user.image ? "http://localhost:5153/" + user.image :
                  generateSVGPlaceholder(placeholderWidth, placeholderHeight, 'Нет изображения')}
        className="mb-3" style={{ width: "300px", height: "400px" }} />
        {error && <Alert variant="danger">{error}</Alert>}
        <Form>
          <Form.Group className="mb-3">
            <Form.Label>Имя пользователя</Form.Label>
            <Form.Control
              type="text"
              value={user.userName}
              onChange={(e) => setUser({ ...user, userName: e.target.value })}
              disabled={!editing}
            />
          </Form.Group>
          <Form.Group className="mb-3">
            <Form.Label>Email</Form.Label>
            <Form.Control
              type="email"
              value={user.email}
              onChange={(e) => setUser({ ...user, email: e.target.value })}
              disabled={!editing}
            />
          </Form.Group>
          <Form.Group className="mb-3">
            <Form.Label>О себе</Form.Label>
            <Form.Control
              type="text"
              value={user.bio}
              onChange={(e) => setUser({ ...user, bio: e.target.value })}
              disabled={!editing}
            />
          </Form.Group>

          {(getUser()?.id === user.id || hasRole("admin")) && (
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

          {getUser()?.id === user.id && (editing ? (
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
          {(user.tournaments && hasRole("admin")) && (
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
          {roles.find(n => n.name === "organizer") !== undefined && (
            <>
              <h2>Турниры, проводимые пользователем</h2>
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
          {roles.find(n => n.name === "newsTeller") !== undefined && (
            <>
              <h2>Новости, опубликованные пользователем</h2>
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


          {(getUser()?.id === user.id || hasRole("admin")) && <Button variant="danger" onClick={handleDelete} className="ms-2">
            Удалить пользователя
          </Button>}

        </Form>

      </Card>

    </div>
  );
};

export default UserDetailsAdmin;



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
