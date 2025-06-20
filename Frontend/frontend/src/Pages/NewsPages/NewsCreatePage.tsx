import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { hasRole } from '../../utils/auth';
import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';
import Alert from 'react-bootstrap/Alert';
import ListGroup from 'react-bootstrap/ListGroup';
import { Discipline, Tag } from '../../types';
import { addPieceOfNews, addTag, addTagToPieceOfNews, fetchTags } from '../../api/newsApi';
import { fetchDisciplines } from '../../api/disciplineApi';

const NewsCreatePage: React.FC = () => {
  const navigate = useNavigate();
  const [form, setForm] = useState({ title: '', content: '', categoryId: '', imagePath: '' });
  const [error, setError] = useState<string | null>(null);
  const [tags, setTags] = useState<Tag[]>([]);
  const [selectedTagIds, setSelectedTagIds] = useState<string[]>([]);
  const [newTagName, setNewTagName] = useState('');
  const [newsId, setNewsId] = useState<string | null>(null);
  const [disciplines, setDisciplines] = useState<Discipline[]>([]);

  useEffect(() => {
    loadTags();
  }, []);

  const loadTags = async () => {
    const response = await fetchTags();
    if (response) {
      const data = await response;
      setTags(data);
    }
    fetchDisciplines().then(setDisciplines);
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleDisciplineChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setForm(prev => ({
      ...prev,
      categoryId: e.target.value
    }));
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
    const response = await addTag(newTagName);

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
    <div className="flex-grow-1 container py-4">
      <h2 className="mb-4">Создать новость</h2>

      <Form className="mb-4">
        <Form.Group className="mb-3">
          <Form.Label>Заголовок</Form.Label>
          <Form.Control
            type="text"
            name="title"
            value={form.title}
            onChange={handleChange}
            required
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Дисциплина</Form.Label>
          <Form.Select
            name="disciplineId"
            value={form.categoryId}
            onChange={handleDisciplineChange}
            required
          >
            <option value="">Выберите дисциплину</option>
            {disciplines.map(discipline => (
              <option key={discipline.id} value={discipline.id}>
                {discipline.name}
              </option>
            ))}
          </Form.Select>
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Контент</Form.Label>
          <Form.Control
            as="textarea"
            name="content"
            rows={5}
            value={form.content}
            onChange={handleChange}
            required
          />
        </Form.Group>

        <Button 
          variant="primary" 
          onClick={handleCreateNews} 
          disabled={!!newsId || !form.title || !form.content || !form.categoryId}
        >
          {newsId ? 'Новость создана' : 'Создать новость'}
        </Button>
      </Form>

      {newsId && (
        <div className="border-top pt-4">
          <h4 className="mb-3">Добавить теги к новости</h4>
          
          <ListGroup className="mb-4">
            {tags.map(tag => (
              <ListGroup.Item key={tag.id} className="d-flex justify-content-between align-items-center">
                {tag.name}
                <Button
                  variant={selectedTagIds.includes(tag.id) ? "success" : "outline-success"}
                  size="sm"
                  onClick={() => handleAddTagToNews(tag.id)}
                  disabled={selectedTagIds.includes(tag.id)}
                >
                  {selectedTagIds.includes(tag.id) ? 'Добавлен' : 'Добавить'}
                </Button>
              </ListGroup.Item>
            ))}
          </ListGroup>

          <Form className="mb-4">
            <Form.Label>Создать новый тег</Form.Label>
            <div className="d-flex gap-2">
              <Form.Control
                value={newTagName}
                onChange={(e) => setNewTagName(e.target.value)}
                placeholder="Введите название тега"
              />
              <Button 
                onClick={handleCreateTag}
                disabled={!newTagName.trim()}
              >
                Создать
              </Button>
            </div>
          </Form>

          <div className="d-flex justify-content-end">
            <Button 
              variant="success" 
              onClick={finishAndGoToDetails}
              className="px-4"
            >
              Перейти к новости
            </Button>
          </div>
        </div>
      )}

      {error && (
        <Alert 
          variant="danger" 
          className="mt-3"
          dismissible
          onClose={() => setError(null)}
        >
          {error}
        </Alert>
      )}
    </div>
  );
};

export default NewsCreatePage;
