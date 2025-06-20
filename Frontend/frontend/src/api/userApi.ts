import { RoleDto, UserCleanDto, UserProfileDto, UserUpdatedto } from "../types";
import { getToken } from "./AuthHook";

const API_BASE = "http://localhost:5153/api";
//const API_BASE = "http://news-service:5153/api";

async function apiRequest<T>(url: string, options: RequestInit): Promise<T> {
  try {
    const response = await fetch(url, options);

    if (response.status === 401) {
      // Редирект на страницу авторизации
      window.location.href = '/login';
      throw new Error('Unauthorized');
    }

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`Ошибка HTTP: ${response.status}, ${errorText}`);
    }

    const contentType = response.headers.get('content-type');
    if (contentType && contentType.includes('application/json')) {
      return await response.json();
    } else {
      return void 0 as unknown as T;
    }
  } catch (error) {
    console.error('Произошла сетевая ошибка:', error);
    throw error;
  }
}

// Авторизация пользователя
export async function loginF(email: string, password: string): Promise<{ accessToken: string; id: string, userName: string, email: string }> {
  const url = `${API_BASE}/user/login`;
  return apiRequest<{ accessToken: string; id: string, userName: string, email: string}>(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ email, password }),
  });
}

// Регистрация пользователя
export async function register(username: string, email: string, password: string): Promise<{ token: string; user: UserCleanDto }> {
  const url = `${API_BASE}/user`;
  return apiRequest<{ token: string; user: UserCleanDto }>(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ username, email, password }),
  });
}

// Получение профиля текущего пользователя
export async function getProfile(): Promise<UserProfileDto> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/user/profile`;
  return apiRequest<UserProfileDto>(url, {
    method: 'GET',
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
}

// Обновление профиля текущего пользователя
export async function updateProfile(profile: UserUpdatedto): Promise<void> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/user/profile`;
  return apiRequest<void>(url, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(profile),
  });
}

// Удаление профиля текущего пользователя
export async function deleteProfile(): Promise<void> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/user/profile`;
  return apiRequest<void>(url, {
    method: 'DELETE',
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
}

// Поиск пользователей по имени
export async function searchUsersByName(userName: string, page: number, pageSize: number): Promise<{ users: UserCleanDto[]; total: number }> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/user?page=${page}&pageSize=${pageSize}&userName=${userName}`;
  return apiRequest<{ users: UserCleanDto[]; total: number }>(url, {
    method: 'GET',
    headers: {
      Authorization: `Bearer ${token}`,
    },
    credentials: 'include',
  });
}

// Получение пользователя по ID
export async function getUserById(id: string): Promise<UserProfileDto> {
  const url = `${API_BASE}/user/${id}`;
  return apiRequest<UserProfileDto>(url, {
    method: 'GET',
    credentials: 'include',
  });
}

// Получение ролей пользователя по ID
export async function getUserRolesById(id: string): Promise<RoleDto[]> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/roles/${id}`;
  return apiRequest<RoleDto[]>(url, {
    method: 'GET',
    headers: {
      Authorization: `Bearer ${token}`,
    },
    credentials: 'include',
  });
}

// Добавление роли пользователю
export async function addUserRole(id: string, role: string): Promise<void> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/roles/${id}/${role}`;
  return apiRequest<void>(url, {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
}

// Удаление роли у пользователя
export async function deleteUserRole(id: string, role: string): Promise<void> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/roles/${id}/${role}`;
  return apiRequest<void>(url, {
    method: 'DELETE',
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
}

// Удаление пользователя по ID
export async function deleteUserById(id: string): Promise<void> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/user/${id}`;
  return apiRequest<void>(url, {
    method: 'DELETE',
    headers: {
      Authorization: `Bearer ${token}`,
    },
    credentials: 'include',
  });
}

export async function addProfileImage(id: string, image: File): Promise<UserProfileDto> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const formData = new FormData();
  formData.append('image', image);

  const response = await fetch(`${API_BASE}/profile/image`, {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
    },
    body: formData,
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`Ошибка при добавлении изображения: ${errorText}`);
  }

  return await response.json();
}

// Удаление изображения у новости
export async function deleteProfileImage(id: string): Promise<UserProfileDto> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const response = await fetch(`${API_BASE}/profile/image`, {
    method: 'DELETE',
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`Ошибка при удалении изображения: ${errorText}`);
  }

  return await response.json();
}

// Обновление изображения у новости
export async function updateProfileImage(id: string, image: File): Promise<UserProfileDto> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const response = await fetch(`${API_BASE}/profile/image`, {
    method: 'PUT',
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`Ошибка при удалении изображения: ${errorText}`);
  }

  return await response.json();
}
