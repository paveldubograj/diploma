import { Link } from "react-router-dom";
import {
    TournamentCleanDto,
    TournamentFormat,
    TournamentStatus,
} from "../../types";
import { Card, Col } from "react-bootstrap";
import { generateSVGPlaceholder } from "../../utils/PlaceholdGenerator"

const TournamentCard = (t: TournamentCleanDto) =>{
        const placeholderWidth = 200;
    const placeholderHeight = 100;
    return <Col md={6} lg={4} key={t.id} className="mb-3">
        <Link to={`/tournaments/${t.id}`} style={{ textDecoration: "none" }}>
            <Card>
                <Card.Body>
                    <Card.Img variant="top" src={t.imagePath
                            ? `http://localhost:5276/${t.imagePath}`
                            : generateSVGPlaceholder(placeholderWidth, placeholderHeight, 'Нет изображения')} 
                            style={{ maxWidth: "400px", height: "200px" }}></Card.Img>
                    <Card.Title>{t.name}</Card.Title>
                    <Card.Text>
                        Статус: {TournamentStatus[t.status].name}<br />
                        Формат: {TournamentFormat[t.format].name}<br />
                        Макс. участников: {t.maxParticipants}
                    </Card.Text>
                </Card.Body>
            </Card>
            </Link>
        </Col>
    
}

export default TournamentCard;
