import { MatchDto, MatchList } from "../types";
import { getToken } from "./AuthHook";

const API_BASE = "http://localhost:5083/api";
//const API_BASE = "http://match-service:5083/api";


interface matchess{
  matches: MatchList[],
  total: number 
}
// Централизованный fetch с обработкой ошибок и редиректом при 401
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

// Получение списка матчей по фильтру
export async function fetchMatches(search: string): Promise<matchess> {
  const url = `${API_BASE}/matches/filter?${search}`;
  return apiRequest<matchess>(url, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
    mode: "cors",
  });
}

// Получение матча по ID
export async function getMatchById(id: string): Promise<MatchDto> {
  const url = `${API_BASE}/matches/${id}`;
  return apiRequest<MatchDto>(url, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
  });
}

// Обновление матча
export async function updateMatch(id: string, data: MatchDto): Promise<void> {
  const token = getToken();
  if (!token) throw new Error("Токен отсутствует");

  const url = `${API_BASE}/matches/${id}`;
  await apiRequest<MatchDto>(url, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(data),
  });
}

// Удаление матча
export async function deleteMatch(id: string): Promise<void> {
  const token = getToken();
  if (!token) throw new Error("Токен отсутствует");

  const url = `${API_BASE}/matches/${id}`;
  await apiRequest<void>(url, {
    method: "DELETE",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
  });
}
