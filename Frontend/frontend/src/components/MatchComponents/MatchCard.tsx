import { Card, Col, Row, Image } from "react-bootstrap";
import { MatchList, MatchStatus } from "../../types"
import { Link } from "react-router-dom";
import { generateSVGPlaceholder } from "../../utils/PlaceholdGenerator";


const placeholderWidth = 60;
const placeholderHeight = 60;

const getAvatarUrl = (userId?: string) =>
  userId ? `/avatars/${userId}.png` : generateSVGPlaceholder(placeholderWidth, placeholderHeight);

const MatchCard = (match: MatchList) => {
  const isCompleted = match.status === 2;
  const firstWinner = match.winnerId === match.participant1Id;

  // Форматирование даты
  const formatDate = (dateStr: string) => {
    if (!dateStr) return "";
    const date = new Date(dateStr);
    return date.toLocaleString("ru-RU", {
      day: "2-digit",
      month: "2-digit",
      year: "2-digit",
      hour: "2-digit",
      minute: "2-digit",
    });
  };

  return (
    <Col xs={12} className="mb-3">
      <Link to={`/matches/${match.id}`} style={{ textDecoration: "none" }}>
        <Card className="p-2" style={{ minHeight: 90 }}>
          <Row className="align-items-center">
            {/* Левый игрок */}
            <Col xs="auto" className="d-flex align-items-center">
              {match.participant1Id && (
                <Image
                  src={getAvatarUrl(match.participant1Id)}
                  roundedCircle
                  width={48}
                  height={48}
                  alt={match.participant1Name}
                  className="me-2"
                  onError={(e) => {
                            // Если основное изображение не загрузится, покажем плейсхолдер
                            const target = e.target as HTMLImageElement;
                            target.src = generateSVGPlaceholder(placeholderWidth, placeholderHeight);
                        }}
                />
              )}
              <div>
                <Link to={`/users/${match.participant1Id}`}>
                  <span style={{ fontWeight: 500 }}>{match.participant1Name}</span>
                </Link>
                {isCompleted && (
                  <div>
                    <span
                      style={{
                        color: firstWinner ? "green" : "red",
                        fontWeight: 600,
                        marginLeft: 4,
                      }}
                    >
                      {firstWinner ? match.winScore : match.looseScore}
                    </span>
                  </div>
                )}
              </div>
            </Col>

            {/* Центр: турнир, статус, дата */}
            <Col className="text-center">
              <div style={{ fontWeight: 600 }}>{match.tournamentName}</div>
              <div style={{ fontSize: 14, color: "#888" }}>
                {MatchStatus.find((s) => s.id === match.status)?.name || "Неизвестный статус"}
                {" • "}
                {isCompleted
                  ? `Завершён: ${formatDate(match.endTime)}`
                  : `Начало: ${formatDate(match.startTime)}`}
              </div>
            </Col>

            {/* Правый игрок */}
            <Col xs="auto" className="d-flex align-items-center flex-row-reverse">
              {match.participant2Id && (
                <Image
                  src={getAvatarUrl(match.participant2Id)}
                  roundedCircle
                  width={48}
                  height={48}
                  alt={match.participant2Name}
                  className="ms-2"
                  onError={(e) => {
                            // Если основное изображение не загрузится, покажем плейсхолдер
                            const target = e.target as HTMLImageElement;
                            target.src = generateSVGPlaceholder(placeholderWidth, placeholderHeight);
                        }}
                />
              )}
              <div className="text-end">
                <Link to={`/users/${match.participant2Id}`}>
                  <span style={{ fontWeight: 500 }}>{match.participant2Name}</span>
                </Link>
                {isCompleted && (
                  <div>
                    <span
                      style={{
                        color: !firstWinner ? "green" : "red",
                        fontWeight: 600,
                        marginRight: 4,
                      }}
                    >
                      {!firstWinner ? match.winScore : match.looseScore}
                    </span>
                  </div>
                )}
              </div>
            </Col>
          </Row>
        </Card>
      </Link>
    </Col>
  );
};

export default MatchCard; 