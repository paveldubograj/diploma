import React, { useEffect, useState } from "react";
import { Form, Button, Alert } from "react-bootstrap";
import { fetchTags, addNews } from "../../api/newsApi";

const NewsForm: React.FC = () => {
  const [title, setTitle] = useState("");
  const [content, setContent] = useState("");
  const [tagId, setTagId] = useState("");
  const [tags, setTags] = useState<any[]>([]);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    try {
      fetchTags().then(setTags);
    } catch (error) {
      setError("Не удалось получить теги")
    }
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    try {
      e.preventDefault();
      await addNews({ title, content, tagId });
      setTitle("");
      setContent("");
    } catch (error) {
      setError("Не удалось обновить новость");
    }
  };

  return (
    <Form onSubmit={handleSubmit} className="mb-4">
      {error && <Alert variant="danger">{error}</Alert>}
      <Form.Group className="mb-2">
        <Form.Label>Заголовок</Form.Label>
        <Form.Control value={title} onChange={e => setTitle(e.target.value)} required />
      </Form.Group>
      <Form.Group className="mb-2">
        <Form.Label>Контент</Form.Label>
        <Form.Control as="textarea" value={content} onChange={e => setContent(e.target.value)} required />
      </Form.Group>
      <Form.Group className="mb-2">
        <Form.Label>Тег</Form.Label>
        <Form.Select value={tagId} onChange={e => setTagId(e.target.value)}>
          <option value="">Выберите тег</option>
          {tags.map(tag => (
            <option key={tag.id} value={tag.id}>{tag.name}</option>
          ))}
        </Form.Select>
      </Form.Group>
      <Button type="submit">Добавить новость</Button>
    </Form>
  );
};

export default NewsForm;
