import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { fetchPieceOfNews, fetchTags as fetchTaggs, saveNewsChanges, deletePieceOfNews, addTagToPieceOfNews} from '../../api/newsApi';
import { DetailNews, Tag } from '../../types';
import { hasRole } from '../../utils/auth';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import Alert from 'react-bootstrap/Alert';

const NewsDetailPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [news, setNews] = useState<DetailNews | null>(null);
  const [isEditing, setIsEditing] = useState(false);
  const [form, setForm] = useState({ title: '', content: '' });
  const [tags, setTags] = useState<Tag[]>([]);
  const [selectedTag, setSelectedTag] = useState<string>('');
  const [error, setError] = useState<string>('');

  useEffect(() => {
    const fetchNews = async () => {
      const response = await fetchPieceOfNews(id);
      if (response) {
        const data: DetailNews = response;
        setNews(data);
        setForm({ title: data.title, content: data.content });
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

  if (!news) return <p>Загрузка...</p>;

  return (
    <div className="flex-grow-1">
      <h2>{news.title}</h2>
      <p>{news.content}</p>

      {news.tags.length > 0 && (
        <div className="mt-3">
          <h5>Теги:</h5>
          {news.tags.map(tag => (
            <span key={tag.id} className="badge bg-primary me-2">
              {tag.name}
            </span>
          ))}
        </div>
      )}

      {hasRole('newsTeller') && (
        <>
          {!isEditing ? (
            <Button className="mt-3" onClick={() => setIsEditing(true)}>Редактировать</Button>
          ) : (
            <Form className="mt-3">
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
                  rows={4}
                  name="content"
                  value={form.content}
                  onChange={handleChange}
                />
              </Form.Group>
              <Button variant="success" onClick={handleSave}>Сохранить</Button>
              <Button variant="secondary" onClick={() => setIsEditing(false)}>Отмена</Button>
            </Form>
          )}
        </>
      )}

      {hasRole('newsTeller') && (
        <div className="mt-4">
          <h5>Добавить тег</h5>
          <Form.Select value={selectedTag} onChange={(e) => setSelectedTag(e.target.value)}>
            <option value="">Выберите тег</option>
            {tags.map(tag => (
              <option key={tag.id} value={tag.id}>
                {tag.name}
              </option>
            ))}
          </Form.Select>
          <Button className="mt-2" onClick={handleAddTag}>Добавить тег</Button>
        </div>
      )}

      {hasRole('admin') && (
        <Button variant="danger" className="mt-4" onClick={handleDelete}>
          Удалить новость
        </Button>
      )}

      {error && <Alert variant="danger" className="mt-3">{error}</Alert>}
    </div>
  );
};

export default NewsDetailPage;
