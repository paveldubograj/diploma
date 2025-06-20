import React, { useEffect, useState } from "react";
import { Button, Card } from "react-bootstrap";
import { useNavigate, useParams } from "react-router-dom";
import { getUserById, deleteUserById } from "../../api/userApi";
import { UserCleanDto } from "../../types";

const UserDetailsAdmin: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [user, setUser] = useState<UserCleanDto | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchUser = async () => {
      if (id) {
        const data = await getUserById(id);
        setUser(data);
      }
    };
    fetchUser();
  }, [id]);

  const handleDelete = async () => {
    if (id && window.confirm("Вы уверены, что хотите удалить пользователя?")) {
      await deleteUserById(id);
      navigate("/admin/users");
    }
  };

  if (!user) return <div>Загрузка...</div>;

  return (
    <div className="flex-grow-1">
    <Card className="container mt-4 p-4">
      <h3>{user.userName}</h3>
      <p><strong>Email:</strong> {user.email}</p>
      <Button variant="danger" onClick={handleDelete}>
        Удалить пользователя
      </Button>
    </Card>
    </div>
  );
};

export default UserDetailsAdmin;
