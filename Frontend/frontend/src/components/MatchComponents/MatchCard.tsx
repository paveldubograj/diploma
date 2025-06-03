import { Card, Col } from "react-bootstrap";
import { MatchList } from "../../types"
import { Link } from "react-router-dom";

const MatchCard = (match: MatchList) => {
    const isCompleted = match.status === 2;
    const firstWinner = match.winnerId === match.participant1Id;

    return (
        <Col md={6} lg={4} key={match.id} className="mb-3">
            <Link to={`/matches/${match.id}`}>
            <Card>
                <Card.Body>
                <Card.Title>{match.round} (#{match.matchOrder})</Card.Title>
                <Card.Text>
                    Участники: <br />
                    <Link to={match.participant1Id}>{match.participant1Name}</Link> vs <Link to={match.participant2Id}>{match.participant2Name}</Link>
                    {isCompleted && firstWinner && (
                    <>
                        <br />
                        <span>Счет:</span>
                        <span style={{ color: "green" }}> {match.winScore}</span> - 
                        <span style={{ color: "red" }}> {match.looseScore}</span>
                    </>
                    )}
                    {isCompleted && !firstWinner && (
                    <>
                        <br />
                        <span>Счет:</span>
                        <span style={{ color: "red" }}> {match.looseScore}</span> - 
                        <span style={{ color: "green" }}> {match.winScore}</span>
                    </>
                    )}
                </Card.Text>
                </Card.Body>
            </Card>
            </Link>
        </Col>
    );
} 

export default MatchCard; 