import { Discipline } from "../types";
import { getToken } from "./AuthHook";

const API_BASE = "http://localhost:5215/api";
//const API_BASE = "http://discipline-service:5215/api";


async function apiRequest<T>(url: string, options: RequestInit): Promise<T> {
  try {
    const response = await fetch(url, options);

    if (response.status === 401) {
      window.location.href = "/login";
      throw new Error("Unauthorized");
    }

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`Ошибка HTTP: ${response.status}, ${errorText}`);
    }

    return await response.json();
  } catch (error) {
    console.error("Произошла сетевая ошибка:", error);
    throw error;
  }
}

// Получить все дисциплины
export async function fetchDisciplines(): Promise<Discipline[]> {
  const url = `${API_BASE}/disciplines`;
  return apiRequest<Discipline[]>(url, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
  });
}

export async function fetchDisciplinesAdmin(): Promise<Discipline[]> {
  const token = getToken();
  if (!token) throw new Error("Токен отсутствует");

  const url = `${API_BASE}/disciplines/admin`;
  return apiRequest<Discipline[]>(url, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
  });
}

// Получить дисциплину по ID
export async function fetchDisciplineById(id: string): Promise<Discipline> {
  const url = `${API_BASE}/disciplines/${id}`;
  return apiRequest<Discipline>(url, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
  });
}

// Обновить дисциплину
export async function UpdateDiscipline(discipline: Discipline): Promise<Discipline> {
  const token = getToken();
  if (!token) throw new Error("Токен отсутствует");

  const url = `${API_BASE}/disciplines/${discipline.id}`;
  return apiRequest<Discipline>(url, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(discipline),
  });
}

// Создать новую дисциплину
export async function CreateDiscipline(
  data: Pick<Discipline, 'name' | 'description'>
): Promise<Discipline> {
  const token = getToken();
  if (!token) throw new Error("Токен отсутствует");

  console.log(data.name + "  " + data.description);

  var d = {name: data.name, description: data.description}

  const url = `${API_BASE}/disciplines`;
  return apiRequest<Discipline>(url, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(d),
  });
}

// Удалить дисциплину
export async function DeleteDiscipline(id: string): Promise<Discipline> {
  const token = getToken();
  if (!token) throw new Error("Токен отсутствует");

  const url = `${API_BASE}/disciplines/${id}`;
  return apiRequest<Discipline>(url, {
    method: "DELETE",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
  });
}
