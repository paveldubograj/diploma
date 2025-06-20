import { DetailNews, ListNews, Tag } from "../types";
import { getToken } from "./AuthHook";

const API_BASE = "http://localhost:5149/api";
//const API_BASE = "http://news-service:5149/api";

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

    return await response.json();
  } catch (error) {
    console.error('Произошла сетевая ошибка:', error);
    throw error;
  }
}
interface newss{
  news: ListNews[],
  total: number 
}
// Получение новостей с фильтром
export async function fetchNews(query: string): Promise<newss> {
  const url = `${API_BASE}/news/filter?${query}`;
  return apiRequest<newss>(url, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
    },
    mode: 'cors',
  });
}

// Получение новостей по пользователю
export async function fetchNewsByUser(id: string, page: number, pageSize: number): Promise<newss> {
  const params = new URLSearchParams({ page: page.toString(), pageSize: pageSize.toString() });
  const url = `${API_BASE}/news/${id}/list?${params.toString()}`;
  return apiRequest<newss>(url, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
    },
    mode: 'cors',
  });
}

// Получение всех тегов
export async function fetchTags(): Promise<Tag[]> {
  const url = `${API_BASE}/tags/list/`;
  return apiRequest<Tag[]>(url, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
    },
    mode: 'cors',
  });
}

// Получение одной новости
export async function fetchPieceOfNews(id: string | undefined): Promise<DetailNews> {
  const url = `${API_BASE}/news/${id}`;
  return apiRequest<DetailNews>(url, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
    },
    mode: 'cors',
  });
}

// Добавление новости
export async function addNews(news: any): Promise<DetailNews> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/news`;
  return apiRequest<DetailNews>(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(news),
  });
}

// Обновление новости
export async function saveNewsChanges(id: string | undefined, saving: string): Promise<DetailNews> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');
  console.log(saving);

  const url = `${API_BASE}/news/${id}`;
  return apiRequest<DetailNews>(url, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: saving,
  });
}

// Удаление новости
export async function deletePieceOfNews(id: string | undefined): Promise<DetailNews> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/news/${id}`;
  return apiRequest<DetailNews>(url, {
    method: 'DELETE',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
  });
}

// Добавление тега к новости
export async function addTagToPieceOfNews(id: string | undefined, selectedTag: string): Promise<DetailNews> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/news/${id}/tags/${selectedTag}`;
  return apiRequest<DetailNews>(url, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
  });
}

export async function removeTagFromPieceOfNews(id: string | undefined, selectedTag: string): Promise<DetailNews> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/news/${id}/tags/${selectedTag}`;
  return apiRequest<DetailNews>(url, {
    method: 'DELETE',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
  });
}

// Добавление новой новости (альтернативная функция)
export async function addPieceOfNews(saving: string): Promise<DetailNews> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/news`;
  return apiRequest<DetailNews>(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: saving,
  });
}

// Добавление нового тега
export async function addTag(saving: string): Promise<Tag> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/tags/`;
  return apiRequest<Tag>(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(saving),
  });
}

export async function updateVisibility(id?: string, visibility?: boolean): Promise<DetailNews> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/news/${id}/visibility?visibility=${visibility}`;
  return apiRequest<DetailNews>(url, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
  });
}

export async function addNewsImage(id: string, image: File): Promise<DetailNews> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const formData = new FormData();
  formData.append('image', image, image.name);

  const response = await fetch(`${API_BASE}/news/image/${id}`, {
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
export async function deleteNewsImage(id: string): Promise<DetailNews> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const response = await fetch(`${API_BASE}/news/image/${id}`, {
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
export async function updateNewsImage(id: string, image: File): Promise<DetailNews> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');


  const formData = new FormData();
  formData.append('image', image, image.name);

  const response = await fetch(`${API_BASE}/news/image/${id}`, {
    method: 'PUT',
    headers: {
      Authorization: `Bearer ${token}`,
      Accept: '*/*',
    },
    body: formData,
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`Ошибка при обновлении изображения: ${errorText}`);
  }

  return await response.json();
}

