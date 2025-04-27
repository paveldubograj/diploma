import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { getMatchById, updateMatch, deleteMatch } from "../../api/matchApi";
import { MatchDto, MatchStatus } from "../../types";
import { Container, Form, Button, Row, Col, Card, Alert, Modal } from "react-bootstrap";
import { handleSetWinner } from "../../api/tournamentApi"
import { toast } from "react-toastify";


const MatchDetails = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [match, setMatch] = useState<MatchDto | null>(null);
  const [formData, setFormData] = useState<MatchDto | null>(null);
  const [error, setError] = useState<string | null>(null);

  const [showModal, setShowModal] = useState(false);
  const [winnerId, setWinnerId] = useState<string>('');
  const [looserId, setLooserId] = useState('');
  const [winPoints, setWinPoints] = useState(0);
  const [loosePoints, setLoosePoints] = useState(0);


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

  if (!match) return <p>Загрузка...</p>;
  if (!formData) {
    return <p>Загрузка...</p>;
  }

  const handleWinnerSet = async (winnerId: string, looserId: string, winPoints: number, loosePoints: number) => {
    handleSetWinner(match.tournamentId, match.id, winnerId, looserId, winPoints, loosePoints);
    setShowModal(false);
  }

  return (
    <Container>
      {error && <Alert variant="danger">{error}</Alert>}
      <Card>
        <Card.Body>
          <Card.Title>Детали матча #{id}</Card.Title>
          <Form>
            <Row className="mb-3">
              <Col>
                <Form.Label>Раунд</Form.Label>
                <Form.Control
                  type="text"
                  name="round"
                  value={formData.round}
                  onChange={handleInputChange}
                />
              </Col>
              <Col>
                <Form.Label>Очередность</Form.Label>
                <Form.Control
                  type="number"
                  name="matchOrder"
                  value={formData.matchOrder}
                  onChange={handleInputChange}
                />
              </Col>
            </Row>

            <Row className="mb-3">
              <Col>
                <Form.Label>Статус</Form.Label>
                <Form.Select
                  name="status"
                  value={formData.status}
                  onChange={handleSelectChange}
                >
                  {MatchStatus.map((key) => (
                    <option key={key.id} value={key.id}>{key.name}</option>
                  ))}
                </Form.Select>
              </Col>
              <Col>
                <Form.Label>ID турнира</Form.Label>
                <Form.Control
                  type="text"
                  name="tournamentId"
                  value={formData.tournamentId}
                  disabled
                />
              </Col>
            </Row>

            <Row className="mb-3">
              <Col>
                <Form.Label>Дата начала</Form.Label>
                <Form.Control
                  type="datetime-local"
                  name="startTime"
                  value={new Date(formData.startTime).toISOString().slice(0, 16)}
                  onChange={(e) =>
                    setFormData((prev) => ({ ...prev!, startTime: new Date(e.target.value).toISOString() }))
                  }
                />
              </Col>
              <Col>
                <Form.Label>Дата окончания</Form.Label>
                <Form.Control
                  type="datetime-local"
                  name="endTime"
                  value={new Date(formData.endTime).toISOString().slice(0, 16)}
                  onChange={(e) =>
                    setFormData((prev) => ({ ...prev!, endTime: new Date(e.target.value).toISOString() }))
                  }
                />
              </Col>
            </Row>

            <Row className="mb-3">
              <Col>
                <Form.Label>ID участника 1</Form.Label>
                <Form.Control
                  type="text"
                  name="participant1Id"
                  value={formData.participant1Id}
                  onChange={handleInputChange}
                />
              </Col>
              <Col>
                <Form.Label>ID участника 2</Form.Label>
                <Form.Control
                  type="text"
                  name="participant2Id"
                  value={formData.participant2Id}
                  onChange={handleInputChange}
                />
              </Col>
            </Row>

            <Row className="mb-3">
              <Col>
                <Form.Label>ID победителя</Form.Label>
                <Form.Control
                  type="text"
                  name="winnerId"
                  value={formData.winnerId}
                  onChange={handleInputChange}
                />
              </Col>
              <Col>
                <Form.Label>ID следующего матча</Form.Label>
                <Form.Control
                  type="text"
                  name="nextMatchId"
                  value={formData.nextMatchId ?? ""}
                  onChange={handleInputChange}
                  disabled
                />
              </Col>
            </Row>

            <Row className="mb-3">
              <Col>
                <Form.Label>Очки победителя</Form.Label>
                <Form.Control
                  type="number"
                  name="winScore"
                  value={formData.winScore}
                  onChange={handleInputChange}
                />
              </Col>
              <Col>
                <Form.Label>Очки проигравшего</Form.Label>
                <Form.Control
                  type="number"
                  name="looseScore"
                  value={formData.looseScore}
                  onChange={handleInputChange}
                />
              </Col>
            </Row>

            <div className="d-flex gap-2">
              <Button variant="primary" onClick={handleSubmit}>
                Сохранить
              </Button>
              <Button variant="danger" onClick={handleDelete}>
                Удалить
              </Button>
            </div>
          </Form>
        </Card.Body>
      </Card>

      { (match.status !== 2) && (<Button variant="success" onClick={() => setShowModal(true)}>
        Назначить победителя
      </Button>)}

      <Modal show={showModal} onHide={() => setShowModal(false)}>
        <Modal.Header closeButton>
          <Modal.Title>Назначить победителя</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            <Form.Group controlId="winnerId">
              <Form.Label>Победитель</Form.Label>
              <Form.Select value={winnerId} onChange={(e) => setWinnerId(e.target.value)}>
                <option key={match.participant1Id} value={match.participant1Id}>{match.participant1Id}</option>
                <option key={match.participant2Id} value={match.participant2Id}>{match.participant2Id}</option>
              </Form.Select>
            </Form.Group>
            <Form.Group controlId="looserId" className="mt-3">
              <Form.Label>Проигравший</Form.Label>
              <Form.Select value={looserId} onChange={(e) => setLooserId(e.target.value)}>
                <option key={match.participant1Id} value={match.participant1Id}>{match.participant1Id}</option>
                <option key={match.participant2Id} value={match.participant2Id}>{match.participant2Id}</option>
              </Form.Select>
            </Form.Group>
            <Form.Group className="mt-3">
              <Form.Label>Очки победителя</Form.Label>
              <Form.Control type="number" value={winPoints} onChange={(e) => setWinPoints(parseInt(e.target.value))} />
            </Form.Group>
            <Form.Group className="mt-3">
              <Form.Label>Очки проигравшего</Form.Label>
              <Form.Control type="number" value={loosePoints} onChange={(e) => setLoosePoints(parseInt(e.target.value))} />
            </Form.Group>
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowModal(false)}>
            Отмена
          </Button>
          <Button variant="primary" onClick={() => {if(winnerId !== '')handleWinnerSet(winnerId, looserId, winPoints, loosePoints); else {toast.error("давай смени значение");}}}>
            Подтвердить
          </Button>
        </Modal.Footer>
      </Modal>
    </Container>
  );
};

export default MatchDetails;
