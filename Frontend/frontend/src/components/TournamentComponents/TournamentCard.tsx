import { Link } from "react-router-dom";
import {
    TournamentCleanDto,
    TournamentFormat,
    TournamentStatus,
} from "../../types";
import { Card, Col } from "react-bootstrap";

const TournamentCard = (t: TournamentCleanDto) =>{
    return <Link to={`/tournaments/${t.id}`} style={{ textDecoration: "none" }}>
        <Col md={6} lg={4} key={t.id} className="mb-3">
            <Card>
                <Card.Body>
                    {t.imagePath && <Card.Img variant="top" src={"http://localhost:/wwwroot/" + t.imagePath}></Card.Img>}
                    <Card.Title>{t.name}</Card.Title>
                    <Card.Text>
                        Статус: {TournamentStatus[t.status].name}<br />
                        Формат: {TournamentFormat[t.format].name}<br />
                        Раунды: {t.rounds}<br />
                        Макс. участников: {t.maxParticipants}
                    </Card.Text>
                </Card.Body>
            </Card>
        </Col>
    </Link>
}

export default TournamentCard;