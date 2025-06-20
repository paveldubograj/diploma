import React, { useEffect, useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { fetchPieceOfNews, fetchTags as fetchTaggs, saveNewsChanges, deletePieceOfNews, addTagToPieceOfNews, updateVisibility } from '../../api/newsApi';
import { DetailNews, Tag } from '../../types';
import { hasRole } from '../../utils/auth';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import Alert from 'react-bootstrap/Alert';
import NewsImageUploader from '../../components/NewsComponents/NewsImageUploader';
import { Badge, ButtonGroup, Card, Col, Container, Image, Row, Stack } from 'react-bootstrap';
import { generateSVGPlaceholder } from '../../utils/PlaceholdGenerator';

const NewsDetailPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [news, setNews] = useState<DetailNews | null>(null);
  const [isEditing, setIsEditing] = useState(false);
  const [form, setForm] = useState({ title: '', content: '', categoryId: '', imagePath: '' });
  const [tags, setTags] = useState<Tag[]>([]);
  const [selectedTag, setSelectedTag] = useState<string>('');
  const [error, setError] = useState<string>('');
  const [cacheBuster, setCacheBuster] = useState(Date.now());

  useEffect(() => {
    const fetchNews = async () => {
      const response = await fetchPieceOfNews(id);
      if (response) {
        const data: DetailNews = response;
        setNews(data);
        setForm({ title: data.title, content: data.content, categoryId: data.categoryId ,imagePath: data.imagePath || '' });
      }
    };

    const fetchTags = async () => {
      const response = await fetchTaggs();
      if (response) {
        const data: Tag[] = response;
        setTags(data);
      }
    };

    fetchNews();
    fetchTags();
  }, [id]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSave = async () => {
    const response = await saveNewsChanges(id, JSON.stringify(form))

    if (response) {
      const updated = response;
      setNews(updated);
      setIsEditing(false);
    } else {
      setError('Ошибка при сохранении');
    }
  };

  const handleDelete = async () => {
    const confirmDelete = window.confirm('Удалить эту новость?');
    if (!confirmDelete) return;

    const response = await deletePieceOfNews(id);

    if (response) {
      navigate('/news');
    } else {
      setError('Не удалось удалить новость');
    }
  };

  const handleAddTag = async () => {
    if (!selectedTag) return;

    const response = await addTagToPieceOfNews(id, selectedTag);

    if (response) {
      const updated = response;
      setNews(updated);
      setSelectedTag('');
    } else {
      setError('Ошибка при добавлении тега');
    }
  };

  const handleChangeVisibility = async () => {
    const response = await updateVisibility(news?.id, !news?.visibility);

    if (response) {
      setNews(response);
    } else {
      setError('Ошибка при обновлении видимости');
    }

  }

  if (!news) return <p>Загрузка...</p>;

  return (
    <Container className="my-4">
      {/* Карточка новости */}
      <Card className="mb-4">
        {/* Изображение */}
        <Card.Img
          variant="top"
          src={news.imagePath
            ? `http://localhost:5149/${news.imagePath}?t=${cacheBuster}`
            : generateSVGPlaceholder(800, 400, 'Нет изображения')
          }
          style={{ height: '300px', objectFit: 'cover' }}
          onError={(e) => {
            const target = e.target as HTMLImageElement;
            target.src = generateSVGPlaceholder(800, 400, 'Ошибка загрузки');
          }}
        />

        <Card.Body>
          {/* Заголовок и метаданные */}
          <Stack direction="horizontal" className="justify-content-between mb-3">
            <Card.Subtitle className="text-muted">
              Автор: <Link to={`/profile/${news.authorId}`}>{news.authorName}</Link>
            </Card.Subtitle>
            <Card.Subtitle className="text-muted">
              {new Date(news.publishingDate).toLocaleString()}
            </Card.Subtitle>
          </Stack>

          {isEditing ? (
            <Form>
              <Form.Group className="mb-3">
                <Form.Label>Заголовок</Form.Label>
                <Form.Control
                  type="text"
                  name="title"
                  value={form.title}
                  onChange={handleChange}
                />
              </Form.Group>

              <Form.Group className="mb-3">
                <Form.Label>Содержание</Form.Label>
                <Form.Control
                  as="textarea"
                  rows={5}
                  name="content"
                  value={form.content}
                  onChange={handleChange}
                />
              </Form.Group>

              <NewsImageUploader
                newsId={news.id}
                currentImageUrl={news.imagePath}
                onImageChange={(updatedNews) => {
                  setNews(updatedNews);
                  setForm(prev => ({ ...prev, imagePath: updatedNews.imagePath }));
                }}
              />

              <ButtonGroup className="mt-3">
                <Button variant="success" onClick={handleSave}>Сохранить</Button>
                <Button variant="outline-secondary" onClick={() => setIsEditing(false)}>Отмена</Button>
              </ButtonGroup>
            </Form>
          ) : (
            <>
              <Card.Title>{news.title}</Card.Title>
              <Card.Text className="mt-3" style={{ whiteSpace: 'pre-line' }}>
                {news.content}
              </Card.Text>
            </>
          )}
        </Card.Body>

        {/* Теги */}
        {news.tags.length > 0 && (
          <Card.Footer>
            <Stack direction="horizontal" gap={2}>
              <span className="fw-bold">Теги:</span>
              {news.tags.map(tag => (
                <Badge key={tag.id} bg="primary" pill>
                  {tag.name}
                </Badge>
              ))}
            </Stack>
          </Card.Footer>
        )}
      </Card>

    {(hasRole('newsTeller') || hasRole('admin')) && 
      (<Card className="mb-4">
        <Card.Body>
          <Stack gap={3}>
            {hasRole('newsTeller') && (
              <>
                {!isEditing ? (
                  <Button onClick={() => setIsEditing(true)}>Редактировать</Button>
                ) : null}

                <div>
                  <h5>Добавить тег</h5>
                  <Stack direction="horizontal" gap={2}>
                    <Form.Select 
                      value={selectedTag} 
                      onChange={(e) => setSelectedTag(e.target.value)}
                      style={{ width: 'auto' }}
                    >
                      <option value="">Выберите тег</option>
                      {tags.map(tag => (
                        <option key={tag.id} value={tag.id}>
                          {tag.name}
                        </option>
                      ))}
                    </Form.Select>
                    <Button onClick={handleAddTag}>Добавить</Button>
                  </Stack>
                </div>
              </>
            )}

            {hasRole('admin') && (
              <>
                <Stack direction="horizontal" gap={2} className="align-items-center">
                  <span>Видимость:</span>
                  <Badge bg={news.visibility ? "success" : "secondary"}>
                    {news.visibility ? "Видна" : "Скрыта"}
                  </Badge>
                  <Button 
                    variant={news.visibility ? "outline-danger" : "outline-success"}
                    size="sm"
                    onClick={handleChangeVisibility}
                  >
                    {news.visibility ? "Скрыть" : "Показать"}
                  </Button>
                </Stack>

                <Button 
                  variant="outline-danger" 
                  onClick={handleDelete}
                >
                  Удалить новость
                </Button>
              </>
            )}
          </Stack>
        </Card.Body>
      </Card>
      )}

      {/* Ошибки */}
      {error && (
        <Alert variant="danger" dismissible onClose={() => setError('')}>
          {error}
        </Alert>
      )}
    </Container>
  );
};

export default NewsDetailPage;
