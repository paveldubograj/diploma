import React, { useState, useEffect } from "react";
import { Form, Button, Table } from "react-bootstrap";
import { searchUsersByName } from "../../api/userApi";
import { useNavigate } from "react-router-dom";
import { UserCleanDto } from "../../types";

const UserList: React.FC = () => {
  const [users, setUsers] = useState<UserCleanDto[]>([]);
  const [searchName, setSearchName] = useState("");
  const navigate = useNavigate();

  const handleSearch = async () => {
    try {
      const result = await searchUsersByName(searchName);
      setUsers(result);
    } catch (err) {
      console.error("Ошибка при поиске пользователей", err);
    }
  };

  return (
    <div className="flex-grow-1 mt-4">
      <h2>Список пользователей</h2>
      <Form className="d-flex mb-3">
        <Form.Control
          type="text"
          placeholder="Поиск по имени"
          value={searchName}
          onChange={(e) => setSearchName(e.target.value)}
        />
        <Button className="ms-2" onClick={handleSearch}>
          Поиск
        </Button>
      </Form>

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
    </div>
  );
};

export default UserList;
