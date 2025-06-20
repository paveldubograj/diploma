import React, { useState, useEffect } from "react";
import { Form, Button, Table, Alert } from "react-bootstrap";
import { searchUsersByName } from "../../api/userApi";
import { useNavigate } from "react-router-dom";
import { UserCleanDto } from "../../types";

const pageSize = 10;

const UserList = () => {
  const [users, setUsers] = useState<UserCleanDto[]>([]);
  const [searchName, setSearchName] = useState("");
  const [inpstr, setInp] = useState("");
  const [page, setPage] = useState(1);
  const [error, setError] = useState<string | null>(null);
  const [total, setTotal] = useState(0);
  const navigate = useNavigate();

  useEffect(() => {
    const loadUsers = async () => {
    try {
      const result = await searchUsersByName(searchName, page, pageSize);
      setUsers(result.users);
      setTotal(result.total);
    } catch (err) {
      console.error("Ошибка при поиске пользователей", err);
      setError("Ошибка при поиске пользователей");
    }
  };
  loadUsers();
  }, [searchName, page]);

  return (
    <div className="flex-grow-1 mt-4">
      <h2>Список пользователей</h2>
      <Form className="d-flex mb-3">
        <Form.Control
          type="text"
          placeholder="Поиск по имени"
          value={searchName}
          onChange={(e) => setInp(e.target.value)}
        />
        <Button className="ms-2" onClick={() => {setPage(1);setSearchName(inpstr);}}>
          Поиск
        </Button>
      </Form>

      {error && <Alert variant="danger">{error}</Alert>}

      <Table striped bordered hover>
        <thead>
          <tr>
            <th>Имя</th>
            <th>Email</th>
            <th>Действия</th>
          </tr>
        </thead>
        <tbody>
          {users.map((user) => (
            <tr key={user.id}>
              <td>{user.userName}</td>
              <td>{user.email}</td>
              <td>
                <Button
                  variant="info"
                  onClick={() => navigate(`/admin/users/${user.id}`)}
                >
                  Подробнее
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>
      <div className="d-flex justify-content-between align-items-center my-3">
        <Button disabled={page === 1} onClick={() => setPage(page - 1)}>
          Назад
        </Button>
        <span>Страница {page} из {Math.ceil(total / pageSize)}</span>
        <Button
          disabled={page >= Math.ceil(total / pageSize)}
          onClick={() => setPage(page + 1)}
        >
          Вперёд
        </Button>
      </div>
    </div>
  );
};

export default UserList;
