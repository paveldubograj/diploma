import { ParticipantCreateDto, ParticipantDto, TournamentCleanDto, TournamentCreateDto, TournamentDto, TournamentFilter } from "../types";
import { getToken } from "./AuthHook";

const API_BASE = 'http://localhost:5276/api/tournaments'
//const API_BASE = "http://tournament-service:5276/api";

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

// Получение списка турниров
export async function fetchTournaments(filter: string): Promise<{ tournaments: TournamentCleanDto[]; total: number }> {
  const url = `${API_BASE}/list/?${filter}`;
  return apiRequest<{ tournaments: TournamentCleanDto[]; total: number }>(url, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
    },
    mode: 'cors',
  });
}

// Получение турниров по пользователю
export async function fetchTournamentsByUser(
  userId: string,
  page: number,
  pageSize: number
): Promise<{ tournaments: TournamentCleanDto[]; total: number }> {
  const params = new URLSearchParams({ page: page.toString(), pageSize: pageSize.toString() });
  const url = `${API_BASE}/${userId}/list?${params}`;
  return apiRequest<{ tournaments: TournamentCleanDto[]; total: number }>(url, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
    },
    mode: 'cors',
  });
}

// Получение турнира по ID
export async function fetchTournamentById(id: string): Promise<TournamentDto> {
  const url = `${API_BASE}/${id}`;
  return apiRequest<TournamentDto>(url, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
    },
    mode: 'cors',
  });
}

// Получение турниров по списку ID
export async function fetchTournamentsByIds(ids: string[]): Promise<TournamentCleanDto[]> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}`;
  return apiRequest<TournamentCleanDto[]>(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(ids),
    mode: 'cors',
  });
}

// Обновление турнира
export async function updateTournament(id: string, data: TournamentDto): Promise<TournamentDto> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/${id}`;
  return apiRequest<TournamentDto>(url, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(data),
    mode: 'cors',
  });
}

// Удаление турнира
export async function deleteTournament(id: string): Promise<TournamentDto> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/${id}`;
  return apiRequest<TournamentDto>(url, {
    method: 'DELETE',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    mode: 'cors',
  });
}

// Начать турнир
export async function startTournament(id: string): Promise<void> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/${id}/start`;
  return apiRequest<void>(url, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    mode: 'cors',
  });
}

// Завершить турнир
export async function endTournament(id: string): Promise<void> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/${id}/end`;
  return apiRequest<void>(url, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    mode: 'cors',
  });
}

// Следующий раунд
export async function setNextRound(id: string): Promise<void> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/${id}/round`;
  return apiRequest<void>(url, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    mode: 'cors',
  });
}

// Генерация сетки
export async function generateBracket(id: string): Promise<void> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/${id}/bracket`;
  return apiRequest<void>(url, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    mode: 'cors',
  });
}

// Добавление участника
export async function addParticipant(tournamentId: string, participant: ParticipantCreateDto): Promise<ParticipantDto> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/${tournamentId}/participants/add`;
  return apiRequest<ParticipantDto>(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(participant),
  });
}

// Получение участника по ID
export async function getParticipantById(id: string, tournamentId: string): Promise<ParticipantDto> {
  const url = `${API_BASE}/${tournamentId}/participants/${id}`;
  return apiRequest<ParticipantDto>(url, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
    },
  });
}

// Обновление участника
export async function updateParticipant(id: string, tournamentId: string, dto: ParticipantDto): Promise<void> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/${tournamentId}/participants/${id}`;
  return apiRequest<void>(url, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(dto),
  });
}

// Удаление участника
export async function deleteParticipant(id: string, tournamentId: string): Promise<void> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/${tournamentId}/participants/${id}`;
  return apiRequest<void>(url, {
    method: 'DELETE',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
  });
}

// Создание турнира
export async function createTournament(dto: TournamentCreateDto): Promise<TournamentDto> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/`;
  return apiRequest<TournamentDto>(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(dto),
  });
}

// Назначение победителя матча
export async function handleSetWinner(
  tid: string,
  mid: string,
  winnerId: string,
  looserId: string,
  winPoints: number,
  loosePoints: number
): Promise<void> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/${tid}/${mid}/result`;
  return apiRequest<void>(url, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({ winnerId, looserId, winPoints, loosePoints }),
  });
}

// Регистрация на турнир
export async function registerForTournament(tid: string, userName: string): Promise<void> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const url = `${API_BASE}/${tid}/participants/register`;
  return apiRequest<void>(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: `{"name": "${userName}"}`,
  });
}

// Получение активных участников
export async function fetchPlayingParticipants(id: string): Promise<ParticipantDto[]> {
  const url = `${API_BASE}/${id}/participants/plays`;
  return apiRequest<ParticipantDto[]>(url, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
    },
    mode: 'cors',
  });
}

export async function addTournamentImage(id: string, image: File): Promise<TournamentDto> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const formData = new FormData();
  formData.append('image', image, image.name);

  const response = await fetch(`${API_BASE}/${id}/image`, {
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
export async function deleteTournamentImage(id: string): Promise<TournamentDto> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const response = await fetch(`${API_BASE}/${id}/image`, {
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
export async function updateTournamentImage(id: string, image: File): Promise<TournamentDto> {
  const token = getToken();
  if (!token) throw new Error('Токен отсутствует');

  const formData = new FormData();
  formData.append('image', image, image.name);

  const response = await fetch(`${API_BASE}/${id}/image`, {
    method: 'PUT',
    headers: {
      Authorization: `Bearer ${token}`,
    },
    body: formData,
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`Ошибка при обновлении изображения: ${errorText}`);
  }

  return await response.json();
}
