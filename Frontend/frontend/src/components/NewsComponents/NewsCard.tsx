import { Link } from "react-router-dom";
import { ListNews } from "../../types";
import { Card, Col } from "react-bootstrap";
import { generateSVGPlaceholder } from "../../utils/PlaceholdGenerator";
import { useState } from "react";

const NewsCard = (n: ListNews) => {
    const [cacheBuster, setCacheBuster] = useState(Date.now());
    const placeholderWidth = 300;
    const placeholderHeight = 100;
    return <Col key={n.id} className="mb-3" xs={12} md={6} lg={6}>
        <Link to={`/news/${n.id}`} style={{ textDecoration: "none" }}>
            <Card>
                <Card.Body>
                    <Card.Img
                        variant="top"
                        src={n.imagePath
                            ? `http://localhost:5149/${n.imagePath}?t=${cacheBuster}`
                            : generateSVGPlaceholder(placeholderWidth, placeholderHeight, 'Нет изображения')
                        }
                        style={{
                            maxWidth: "600px",
                            height: "300px",
                            objectFit: "cover"
                        }}
                        onError={(e) => {
                            const target = e.target as HTMLImageElement;
                            target.src = generateSVGPlaceholder(placeholderWidth, placeholderHeight, 'Ошибка загрузки');
                        }}
                    />
                    <Card.Title>{n.title}</Card.Title>
                    <Card.Text>
                        <small>Автор: {<Link to={"/admin/users/" + n.authorId}>{n.authorName}</Link>}</small>
                    </Card.Text>
                    <Card.Footer>
                        <small className="text-muted">Загружено: {n.publishingDate}</small>
                    </Card.Footer>
                </Card.Body>
            </Card>
        </Link>
    </Col>
}

export default NewsCard;
