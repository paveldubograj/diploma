import React, { useEffect, useState } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
import { getMatchById, updateMatch, deleteMatch } from "../../api/matchApi";
import { MatchDto, MatchStatus, ParticipantSListDto } from "../../types";
import { Container, Form, Button, Row, Col, Card, Alert, Modal, Badge, Stack, ButtonGroup, Spinner } from "react-bootstrap";
import { fetchPlayingParticipants, handleSetWinner } from "../../api/tournamentApi"
import { toast } from "react-toastify";
import { getUser } from "../../api/AuthHook";


const MatchDetails = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [match, setMatch] = useState<MatchDto | null>(null);
  const [formData, setFormData] = useState<MatchDto | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [participants, setParticipants] = useState<ParticipantSListDto[]>([]);
  const [participant1Id, setParticipant1Id] = useState<string>("");
  const [participant2Id, setParticipant2Id] = useState<string>("");
  const [participant1Name, setParticipant1Name] = useState<string>("");
  const [participant2Name, setParticipant2Name] = useState<string>("");
  const [showModal, setShowModal] = useState(false);
  const [winnerId, setWinnerId] = useState<string>('');
  const [looserId, setLooserId] = useState('');
  const [winPoints, setWinPoints] = useState(0);
  const [loosePoints, setLoosePoints] = useState(0);
  const [editMode, setEditMode] = useState(false);
  const [isSettingWinner, setIsSettingWinner] = useState(false);


  useEffect(() => {
    if (id) {
      getMatchById(id)
        .then((data) => {
          setMatch(data);
          setFormData(data);
        })
        .catch((err) => setError("Ошибка при загрузке матча"));
    }
  }, [id]);

  useEffect(() => {
    const selected = participants.find(p => p.id === participant1Id);
    if (selected) {
      setParticipant1Name(selected.name);
    }
  }, [participant1Id, participants]);

  useEffect(() => {
    const selected = participants.find(p => p.id === participant2Id);
    if (selected) {
      setParticipant2Name(selected.name);
    }
  }, [participant2Id, participants]);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    if (!formData) return;

    setFormData((prev) => ({
      ...prev!,
      [name]: name === "winScore" || name === "looseScore" || name === "matchOrder"
        ? parseInt(value, 10)
        : value,
    }));
  };

  const handleSelectChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const { name, value } = e.target;
    if (!formData) return;

    setFormData((prev) => ({
      ...prev!,
      [name]: value,
    }));
  };

  const handleSubmit = async () => {
    if (formData && id) {
      try {
        setEditMode(false);
        await updateMatch(id, formData);
        alert("Матч обновлен");
      } catch (error) {
        console.error("Ошибка обновления матча:", error);
        setError("Не удалось обновить матч");
      }
    }
  };


  const handleDelete = async () => {
    if (id && window.confirm("Вы уверены, что хотите удалить этот матч?")) {
      try {
        await deleteMatch(id);
        navigate("/matches");
      } catch (error) {
        console.error("Ошибка удаления матча:", error);
        setError("Не удалось удалить матч");
      }
    }
  };

  useEffect(() => {
    const loadParticipants = async () => {
      try {
        if (match) await fetchPlayingParticipants(match.tournamentId).then(setParticipants);
      } catch (error) {
        console.error("Ошибка загрузки участников", error);
      }
    };
    loadParticipants();
  }, [match]);

  if (!match) return <p>Загрузка...</p>;

  if (!formData) {
    return <p>Загрузка...</p>;
  }

  const handleWinnerSet = async () => {
    if (!winnerId) {
      toast.error("Выберите победителя");
      return;
    }
    setIsSettingWinner(true);
    try {
      await handleSetWinner(
        match!.tournamentId, 
        match!.id, 
        winnerId, 
        looserId, 
        winPoints, 
        loosePoints
      );
      const updatedMatch = await getMatchById(id!);
      setMatch(updatedMatch);
      setFormData(updatedMatch);
      
      toast.success("Победитель успешно назначен");
      setShowModal(false);
    } catch (error) {
      console.error("Ошибка назначения победителя:", error);
      toast.error("Не удалось назначить победителя");
    } finally {setIsSettingWinner(false)}
  };

  if (!match || !formData) {
    return (
      <Container className="text-center my-5">
        <Alert variant="info">Загрузка данных матча...</Alert>
      </Container>
    );
  }

  const statusInfo = MatchStatus.find(f => f.id === formData.status);
  const isOwner = match.ownerId === getUser()?.id;
  const isCompleted = match.status === 2;

  return (
    <Container className="my-4">
      {error && <Alert variant="danger" dismissible onClose={() => setError(null)}>{error}</Alert>}
      
      <Card className="mb-4">
        <Card.Header className="d-flex justify-content-between align-items-center">
          <h4 className="mb-0">Матч #{id}</h4>
          <Badge bg={"secondary"}>
            {statusInfo?.name || "Неизвестный статус"}
          </Badge>
        </Card.Header>
        
        <Card.Body>
          {!editMode ? (
            <>
              <Stack gap={3}>
                <Row>
                  <Col md={6}>
                    <h5>Основная информация</h5>
                    <div className="mb-3">
                      <strong>Турнир:</strong>{" "}
                      <Link to={`/tournaments/${match.tournamentId}`}>
                        {match.tournamentName}
                      </Link>
                    </div>
                    <div className="mb-3">
                      <strong>Раунд:</strong> {formData.round}
                    </div>
                    <div className="mb-3">
                      <strong>Очередность:</strong> {formData.matchOrder}
                    </div>
                  </Col>
                  <Col md={6}>
                    <h5>Временные параметры</h5>
                    <div className="mb-3">
                      <strong>Начало:</strong>{" "}
                      {new Date(formData.startTime).toLocaleString()}
                    </div>
                    <div className="mb-3">
                      <strong>Окончание:</strong>{" "}
                      {new Date(formData.endTime).toLocaleString()}
                    </div>
                  </Col>
                </Row>

                <Row>
                  <Col md={6}>
                    <Card className="mb-3">
                      <Card.Header>Участник 1</Card.Header>
                      <Card.Body>
                        <div className="d-flex justify-content-between">
                          <Link to={`/users/${match.participant1Id}`}>
                            {formData.participant1Name}
                          </Link>
                          {formData.winnerId === match.participant1Id && (
                            <Badge bg="success">Победитель</Badge>
                          )}
                        </div>
                      </Card.Body>
                    </Card>
                  </Col>
                  <Col md={6}>
                    <Card className="mb-3">
                      <Card.Header>Участник 2</Card.Header>
                      <Card.Body>
                        <div className="d-flex justify-content-between">
                          <Link to={`/users/${match.participant2Id}`}>
                            {formData.participant2Name}
                          </Link>
                          {formData.winnerId === match.participant2Id && (
                            <Badge bg="success">Победитель</Badge>
                          )}
                        </div>
                      </Card.Body>
                    </Card>
                  </Col>
                </Row>

                {formData.winnerId && (
                  <Row>
                    <Col md={6}>
                      <div className="mb-3">
                        <strong>Очки победителя:</strong> {formData.winScore}
                      </div>
                    </Col>
                    <Col md={6}>
                      <div className="mb-3">
                        <strong>Очки проигравшего:</strong> {formData.looseScore}
                      </div>
                    </Col>
                  </Row>
                )}

                {match.nextMatchId && (
                  <div className="mb-3">
                    <strong>Следующий матч:</strong>{" "}
                    <Link to={`/matches/${match.nextMatchId}`}>
                      Перейти к следующему матчу
                    </Link>
                  </div>
                )}
              </Stack>
            </>
          ) : (
            <Form>
              <Row className="mb-3">
                <Col md={6}>
                  <Form.Group>
                    <Form.Label>Раунд</Form.Label>
                    <Form.Control
                      name="round"
                      value={formData.round}
                      onChange={handleInputChange}
                    />
                  </Form.Group>
                </Col>
                <Col md={6}>
                  <Form.Group>
                    <Form.Label>Очередность</Form.Label>
                    <Form.Control
                      type="number"
                      name="matchOrder"
                      value={formData.matchOrder}
                      onChange={handleInputChange}
                    />
                  </Form.Group>
                </Col>
              </Row>

              <Row className="mb-3">
                <Col md={6}>
                  <Form.Group>
                    <Form.Label>Статус</Form.Label>
                    <Form.Select
                      name="status"
                      value={formData.status}
                      onChange={handleSelectChange}
                    >
                      {MatchStatus.map((status) => (
                        <option key={status.id} value={status.id}>
                          {status.name}
                        </option>
                      ))}
                    </Form.Select>
                  </Form.Group>
                </Col>
                <Col md={6}>
                  <Form.Group>
                    <Form.Label>Турнир</Form.Label>
                    <div className="form-control-plaintext">
                      <Link to={`/tournaments/${match.tournamentId}`}>
                        {match.tournamentName}
                      </Link>
                    </div>
                  </Form.Group>
                </Col>
              </Row>

              <Row className="mb-3">
                <Col md={6}>
                  <Form.Group>
                    <Form.Label>Дата начала</Form.Label>
                    <Form.Control
                      type="datetime-local"
                      name="startTime"
                      value={new Date(formData.startTime).toISOString().slice(0, 16)}
                      onChange={(e) =>
                        setFormData(prev => ({ ...prev!, startTime: new Date(e.target.value).toISOString() }))
                      }
                    />
                  </Form.Group>
                </Col>
                <Col md={6}>
                  <Form.Group>
                    <Form.Label>Дата окончания</Form.Label>
                    <Form.Control
                      type="datetime-local"
                      name="endTime"
                      value={new Date(formData.endTime).toISOString().slice(0, 16)}
                      onChange={(e) =>
                        setFormData(prev => ({ ...prev!, endTime: new Date(e.target.value).toISOString() }))
                      }
                    />
                  </Form.Group>
                </Col>
              </Row>

              <Row className="mb-3">
                <Col md={6}>
                  <Form.Group>
                    <Form.Label>Участник 1</Form.Label>
                    <Form.Select
                      name="participant1Id"
                      value={formData.participant1Id}
                      onChange={handleSelectChange}
                    >
                      <option value="">Не выбран</option>
                      {participants.map(p => (
                        <option key={p.id} value={p.id}>{p.name}</option>
                      ))}
                    </Form.Select>
                  </Form.Group>
                </Col>
                <Col md={6}>
                  <Form.Group>
                    <Form.Label>Участник 2</Form.Label>
                    <Form.Select
                      name="participant2Id"
                      value={formData.participant2Id}
                      onChange={handleSelectChange}
                    >
                      <option value="">Не выбран</option>
                      {participants.map(p => (
                        <option key={p.id} value={p.id}>{p.name}</option>
                      ))}
                    </Form.Select>
                  </Form.Group>
                </Col>
              </Row>

              {formData.participant1Id && formData.participant2Id && (
                <Row className="mb-3">
                  <Col md={6}>
                    <Form.Group>
                      <Form.Label>Победитель</Form.Label>
                      <Form.Select
                        name="winnerId"
                        value={formData.winnerId}
                        onChange={handleSelectChange}
                      >
                        <option value="">Не выбран</option>
                        <option value={formData.participant1Id}>{formData.participant1Name}</option>
                        <option value={formData.participant2Id}>{formData.participant2Name}</option>
                      </Form.Select>
                    </Form.Group>
                  </Col>
                  <Col md={3}>
                    <Form.Group>
                      <Form.Label>Очки победителя</Form.Label>
                      <Form.Control
                        type="number"
                        name="winScore"
                        value={formData.winScore}
                        onChange={handleInputChange}
                      />
                    </Form.Group>
                  </Col>
                  <Col md={3}>
                    <Form.Group>
                      <Form.Label>Очки проигравшего</Form.Label>
                      <Form.Control
                        type="number"
                        name="looseScore"
                        value={formData.looseScore}
                        onChange={handleInputChange}
                      />
                    </Form.Group>
                  </Col>
                </Row>
              )}

              {match.nextMatchId && (
                <div className="mb-3">
                  <Form.Label>Следующий матч</Form.Label>
                  <div className="form-control-plaintext">
                    <Link to={`/matches/${match.nextMatchId}`}>
                      {match.nextMatchId}
                    </Link>
                  </div>
                </div>
              )}
            </Form>
          )}

          <ButtonGroup className="mt-3">
            {editMode ? (
              <>
                <Button variant="primary" onClick={handleSubmit}>
                  Сохранить
                </Button>
                <Button variant="outline-secondary" onClick={() => setEditMode(false)}>
                  Отмена
                </Button>
              </>
            ) : (
              <Button variant="primary" onClick={() => setEditMode(true)}>
                Редактировать
              </Button>
            )}
            <Button variant="outline-danger" onClick={handleDelete}>
              Удалить
            </Button>
            {!isCompleted && isOwner && (
              <Button variant="success" onClick={() => setShowModal(true)}>
                Назначить победителя
              </Button>
            )}
          </ButtonGroup>
        </Card.Body>
      </Card>

      {/* Модальное окно назначения победителя */}
      <Modal show={showModal} onHide={() => setShowModal(false)} centered>
        <Modal.Header closeButton>
          <Modal.Title>Назначить победителя</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            <Form.Group className="mb-3">
              <Form.Label>Победитель</Form.Label>
              <Form.Select 
                value={winnerId} 
                onChange={(e) => {
                  setWinnerId(e.target.value);
                  setLooserId(
                    e.target.value === match.participant1Id 
                      ? match.participant2Id 
                      : match.participant1Id
                  );
                }}
              >
                <option value="">Выберите победителя</option>
                <option value={match.participant1Id}>
                  {match.participant1Name}
                </option>
                <option value={match.participant2Id}>
                  {match.participant2Name}
                </option>
              </Form.Select>
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Проигравший</Form.Label>
              <Form.Control 
                type="text" 
                value={
                  looserId === match.participant1Id 
                    ? match.participant1Name 
                    : match.participant2Name
                } 
                readOnly 
              />
            </Form.Group>

            <Row>
              <Col>
                <Form.Group>
                  <Form.Label>Очки победителя</Form.Label>
                  <Form.Control 
                    type="number" 
                    value={winPoints} 
                    onChange={(e) => setWinPoints(parseInt(e.target.value) || 0)} 
                  />
                </Form.Group>
              </Col>
              <Col>
                <Form.Group>
                  <Form.Label>Очки проигравшего</Form.Label>
                  <Form.Control 
                    type="number" 
                    value={loosePoints} 
                    onChange={(e) => setLoosePoints(parseInt(e.target.value) || 0)} 
                  />
                </Form.Group>
              </Col>
            </Row>
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowModal(false)}>
            Отмена
          </Button>
              <Button variant="primary" onClick={() => handleWinnerSet()}>
                Сохранить
              </Button>
        </Modal.Footer>
      </Modal>
    </Container>
  );
};

export default MatchDetails;
