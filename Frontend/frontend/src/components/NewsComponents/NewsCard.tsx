import { Link } from "react-router-dom";
import { ListNews } from "../../types";
import { Card, Col } from "react-bootstrap";

const NewsCard = (n : ListNews) => {
    return <Link to={`/tournaments/${n.id}`} style={{ textDecoration: "none" }}>
        <Col md={6} lg={4} key={n.id} className="mb-3">
            <Card>
                <Card.Body>
                    {n.imagePath && <Card.Img variant="top" src={"http://localhost:/wwwroot/" + n.imagePath}></Card.Img>}
                    <Card.Title>{n.title}</Card.Title>
                    <Card.Text>
                        <small>Автор: {<Link to={n.authorId}>{n.authorName}</Link>}</small>
                    </Card.Text>
                    <Card.Footer>
                        <small className="text-muted">Загружено: {n.publishingDate.toISOString()}</small>
                    </Card.Footer>
                </Card.Body>
            </Card>
        </Col>
    </Link>
}

export default NewsCard;