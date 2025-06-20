import React, { useState, useEffect } from "react";
import { Form, Button, Table, Alert, Pagination } from "react-bootstrap";
import { getUserRolesById, searchUsersByName } from "../../api/userApi";
import { useNavigate } from "react-router-dom";
import { RoleDto, UserCleanDto } from "../../types";

const pageSize = 10;

const UserList = () => {
  const [users, setUsers] = useState<UserCleanDto[]>([]);
  const [searchName, setSearchName] = useState("");
  const [inpstr, setInp] = useState("");
  const [page, setPage] = useState(1);
  const [error, setError] = useState<string | null>(null);
  const [total, setTotal] = useState(0);
  const [userRoles, setUserRoles] = useState<Record<string, RoleDto[]>>({});
  const navigate = useNavigate();
  const [loading, setLoading] = useState<boolean>(false);

  useEffect(() => {
    const loadUsers = async () => {
      setLoading(true);
      try {
        const result = await searchUsersByName(searchName, page, pageSize);
        setUsers(result.users);
        setTotal(result.total);

        const errorDto: RoleDto = { name: 'Ошибка загрузки ролей' }
        const rolesMap: Record<string, RoleDto[]> = {};
        for (const user of result.users) {
          try {
            const roles = await getUserRolesById(user.id);
            rolesMap[user.id] = roles;
          } catch (e) {
            rolesMap[user.id] = [errorDto];
          }
        }
        setUserRoles(rolesMap);
      } catch (err) {
        console.error("Ошибка при поиске пользователей", err);
        setError("Ошибка при поиске пользователей");
      } finally {
        setLoading(false);
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
          value={inpstr}
          onChange={(e) => setInp(e.target.value)}
        />
        <Button className="ms-2" onClick={() => { setPage(1); setSearchName(inpstr); }}>
          Поиск
        </Button>
      </Form>

      <Button
        className="mt-2"
        onClick={() => navigate(`/admin/disciplines`)}
      >
        Дисциплины
      </Button>

      {error && <Alert variant="danger">{error}</Alert>}

      <Table striped bordered hover>
        <thead>
          <tr>
            <th>Имя</th>
            <th>Email</th>
            <th>Роли</th>
            <th>Действия</th>
          </tr>
        </thead>
        <tbody>
          {users.map((user) => (
            <tr key={user.id}>
              <td>{user.userName}</td>
              <td>{user.email}</td>
              <td>{userRoles[user.id]?.map(r => r.name + ' ') || "Загрузка..."}</td>
              <td>
                <Button
                  variant="info"
                  onClick={() => navigate(`/users/${user.id}`)}
                >
                  Подробнее
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>
      <Pagination className="justify-content-center mt-4">
      <Pagination.Prev
      onClick={() => setPage(page - 1)}
      disabled={page === 1 || loading}
      >
      Назад
      </Pagination.Prev>

      {Array.from({ length: (total / pageSize + 1) }, (_, i) => (
        <Pagination.Item
        key={i + 1}
        active={i + 1 === page}
        onClick={() => setPage(i + 1)}
        disabled={loading}
        >
        {i + 1}
        </Pagination.Item>
      ))}

      <Pagination.Next
      onClick={() => setPage(page + 1)}
      disabled={page >= (total / pageSize) || loading}
      >
      Вперёд
      </Pagination.Next>
      </Pagination>
    </div>
  );
};

export default UserList;
