import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { hasRole } from '../../utils/auth';
import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';
import Alert from 'react-bootstrap/Alert';
import ListGroup from 'react-bootstrap/ListGroup';
import { Tag } from '../../types';
import { addPieceOfNews, addTag, addTagToPieceOfNews, fetchTags } from '../../api/newsApi';

const NewsCreatePage: React.FC = () => {
  const navigate = useNavigate();
  const [form, setForm] = useState({ title: '', content: '' });
  const [error, setError] = useState<string | null>(null);
  const [tags, setTags] = useState<Tag[]>([]);
  const [selectedTagIds, setSelectedTagIds] = useState<string[]>([]);
  const [newTagName, setNewTagName] = useState('');
  const [newsId, setNewsId] = useState<string | null>(null);

  useEffect(() => {
    loadTags();
  }, []);

  const loadTags = async () => {
    const response = await fetchTags();
    if (response) {
      const data = await response;
      setTags(data);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleCreateNews = async () => {
    const response = await addPieceOfNews(JSON.stringify(form));

    if (response) {
      const data = await response;
      setNewsId(data.id);
    } else {
      setError('Ошибка при создании новости');
    }
  };

  const handleAddTagToNews = async (tagId: string) => {
    if (!newsId) return;
    const response = await addTagToPieceOfNews(newsId, tagId);
    if (response) {
      setSelectedTagIds((prev) => [...prev, tagId]);
    }
  };

  const handleCreateTag = async () => {
    const response = await addTag(JSON.stringify({ name: newTagName }));

    if (response) {
      setNewTagName('');
      await loadTags();
    }
  };

  const finishAndGoToDetails = () => {
    if (newsId) navigate(`/news/${newsId}`);
  };

  if (!hasRole('newsTeller')) {
    return <Alert variant="danger">У вас нет доступа к созданию новостей</Alert>;
  }

  return (
    <div className="flex-grow-1">
      <h2>Создать новость</h2>

      <Form className="mb-3">
        <Form.Group className="mb-2">
          <Form.Label>Заголовок</Form.Label>
          <Form.Control
            type="text"
            name="title"
            value={form.title}
            onChange={handleChange}
          />
        </Form.Group>
        <Form.Group className="mb-2">
          <Form.Label>Контент</Form.Label>
          <Form.Control
            as="textarea"
            name="content"
            rows={5}
            value={form.content}
            onChange={handleChange}
          />
        </Form.Group>
        <Button variant="primary" onClick={handleCreateNews} disabled={!!newsId}>
          {newsId ? 'Новость создана' : 'Создать новость'}
        </Button>
      </Form>

      {newsId && (
        <>
          <h4>Добавить теги к новости</h4>
          <ListGroup className="mb-3">
            {tags.map(tag => (
              <ListGroup.Item key={tag.id}>
                {tag.name}
                <Button
                  variant="outline-success"
                  size="sm"
                  className="ms-2"
                  disabled={selectedTagIds.includes(tag.id)}
                  onClick={() => handleAddTagToNews(tag.id)}
                >
                  Добавить
                </Button>
              </ListGroup.Item>
            ))}
          </ListGroup>

          <Form className="mb-3">
            <Form.Label>Создать новый тег</Form.Label>
            <div className="d-flex gap-2">
              <Form.Control
                value={newTagName}
                onChange={(e) => setNewTagName(e.target.value)}
              />
              <Button onClick={handleCreateTag}>Создать</Button>
            </div>
          </Form>

          <Button variant="success" onClick={finishAndGoToDetails}>
            Перейти к новости
          </Button>
        </>
      )}

      {error && <Alert className="mt-3" variant="danger">{error}</Alert>}
    </div>
  );
};

export default NewsCreatePage;
