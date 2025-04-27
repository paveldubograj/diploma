import React, { useEffect, useState } from "react";
import { Button, Card, Form, Spinner, Alert } from "react-bootstrap";
import { getProfile, updateProfile, deleteProfile } from "../../api/userApi";
import { useNavigate } from "react-router-dom";
import { UserCleanDto } from "../../types";

const ProfilePage = () => {
  const [profile, setProfile] = useState<UserCleanDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [editing, setEditing] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    getProfile()
      .then(setProfile)
      .catch(() => setError("Не удалось загрузить профиль"))
      .finally(() => setLoading(false));
  }, []);

  const handleUpdate = async () => {
    if (!profile) return;
    try {
      await updateProfile(profile);
      setEditing(false);
    } catch {
      setError("Ошибка при обновлении профиля");
    }
  };

  const handleDelete = async () => {
    if (!window.confirm("Вы уверены, что хотите удалить свой профиль?")) return;
    try {
      await deleteProfile();
      localStorage.removeItem("token");
      navigate("/login");
    } catch {
      setError("Ошибка при удалении профиля");
    }
  };

  if (loading) return <Spinner animation="border" />;
  if (!profile) return <Alert variant="danger">Профиль не найден</Alert>;

  return (
    <div className="flex-grow-1">
    <Card className="p-4">
      <h2>Профиль</h2>
      {error && <Alert variant="danger">{error}</Alert>}
      <Form>
        <Form.Group className="mb-3">
          <Form.Label>Имя пользователя</Form.Label>
          <Form.Control
            type="text"
            value={profile.userName}
            onChange={(e) => setProfile({ ...profile, userName: e.target.value })}
            disabled={!editing}
          />
        </Form.Group>
        <Form.Group className="mb-3">
          <Form.Label>Email</Form.Label>
          <Form.Control
            type="email"
            value={profile.email}
            onChange={(e) => setProfile({ ...profile, email: e.target.value })}
            disabled={!editing}
          />
        </Form.Group>

        {editing ? (
          <>
            <Button variant="success" onClick={handleUpdate} className="me-2">
              Сохранить
            </Button>
            <Button variant="secondary" onClick={() => setEditing(false)}>
              Отмена
            </Button>
          </>
        ) : (
          <Button variant="primary" onClick={() => setEditing(true)} className="me-2">
            Редактировать
          </Button>
        )}

        <Button variant="danger" onClick={handleDelete} className="ms-2">
          Удалить профиль
        </Button>
      </Form>
    </Card>
    </div>
  );
};

export default ProfilePage;
