import { ParticipantCreateDto, ParticipantDto, TournamentCleanDto, TournamentCreateDto, TournamentDto, TournamentFilter } from "../types";
import { getToken } from "./AuthHook";

const API_BASE = 'http://localhost:5276/api/tournaments'

export async function fetchTournaments(filter: string){
try {
    const response = await fetch(`${API_BASE}/list/?${filter}`, {
      method: "GET",
      headers: { "Content-Type": "application/json" },
      mode: "cors",
    });
    return response.json();
} catch (error) {
  console.error("Произошла ошибка при получении турниров:", error);
  setError("Не удалось получить турниры");
}
};

export async function fetchTournamentById(id: string){
try {
    const response = await fetch(`${API_BASE}/${id}`, {
      method: "GET",
      headers: { "Content-Type": "application/json" },
      mode: "cors",
    });
    return response.json();
} catch (error) {
  console.error("Произошла ошибка при получении турнира:", error);
  setError("Не удалось получить турнир");
}
}

export async function updateTournament(id: string, data: TournamentDto) {
try {
    const token = getToken();
    if(!token) return null;
    const response = await fetch(`${API_BASE}/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`,
       },
      body: JSON.stringify(data),
      mode: "cors",
    });
    return await response.json();
} catch (error) {
  console.error("Произошла ошибка при обновлении турнира:", error);
  setError("Не удалось обновить турнир");
}
}

export async function deleteTournament(id: string) {
try {
    const token = getToken();
    if(!token) return null;
    const response = await fetch(`${API_BASE}/${id}`, {
      method: "DELETE",
      headers: { "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`, },
      mode: "cors",
    });
    return await response.json();
} catch (error) {
  console.error("Произошла ошибка при удалении турнира:", error);
  setError("Не удалось удалить турнир");
}
}

export async function startTournament(id: string) {
try {
    const token = getToken();
    if(!token) return null;
    const response = await fetch(`${API_BASE}/${id}/start`, {
      method: "PUT",
      headers: { "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`, },
      mode: "cors",
    });
    return await response.json();
} catch (error) {
  console.error("Произошла ошибка при старте турнира:", error);
  setError("Не удалось начать турнир");
}
}

export async function endTournament(id: string) {
try {
    const token = getToken();
    if(!token) return null;
    const response = await fetch(`${API_BASE}/${id}/end`, {
      method: "PUT",
      headers: { "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`, },
      mode: "cors",
    });
    return await response.json();
} catch (error) {
  console.error("Произошла ошибка при окончании турнира:", error);
  setError("Не удалось закончить турнир");
}
}

export async function setNextRound(id: string) {
try {
    const token = getToken();
    if(!token) return null;
    const response = await fetch(`${API_BASE}/${id}/round`, {
      method: "PUT",
      headers: { "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`, },
      mode: "cors",
    });
    return await response.json();
} catch (error) {
  console.error("Произошла ошибка при инициализации раунда турнира:", error);
  setError("Не удалось начать раунд турнира");
}
}

export async function generateBracket(id: string) {
try {
    const token = getToken();
    if(!token) return null;
    const response = await fetch(`${API_BASE}/${id}/bracket`, {
      method: "PUT",
      headers: { "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`, },
      mode: "cors",
    });
    return await response.json();
} catch (error) {
  console.error("Произошла ошибка при генерации сетки турнира:", error);
  setError("Не удалось сгенерировать сетку для турнира");
}
}

export async function addParticipant(tournamentId: string, participant: ParticipantCreateDto) {
try {
    const token = getToken();
    if(!token) return null;
    const response = await fetch(`${API_BASE}/${tournamentId}/participants`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`,
      },
      body: JSON.stringify(participant),
    });
    return await response.json();
} catch (error) {
  console.error("Произошла ошибка при добавлении участника:", error);
  setError("Не удалось добавить участника");
}
}

export async function getParticipantById(id: string) {
try {
    const res = await fetch(`${API_BASE}/participants/${id}`);
    return res.json();
} catch (error) {
  console.error("Произошла ошибка при получении участников:", error);
  setError("Не удалось получить участников");
}
}

export async function updateParticipant(id: string, dto: ParticipantDto) {
try {
    const token = getToken();
    if(!token) return null;
    const res = await fetch(`${API_BASE}/participants/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`, },
      body: JSON.stringify(dto),
    });
} catch (error) {
  console.error("Произошла ошибка при обновлении участника:", error);
  setError("Не удалось обновить участника");
}
}

export async function deleteParticipant(id: string) {
try {
    const token = getToken();
    if(!token) return null;
    const res = await fetch(`${API_BASE}/participants/${id}`, {
      method: "DELETE",
      headers: { "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`, },
    });
} catch (error) {
  console.error("Произошла ошибка при удалении участника:", error);
  setError("Не удалось удалить участника");
}
}

export async function createTournament(dto: TournamentCreateDto) {
try {
    const token = getToken();
      if(!token) return null;
    const response = await fetch(`${API_BASE}/`, {
      method: "POST",
      headers: { "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`, },
      body: JSON.stringify(dto),
    });
    return await response.json();
} catch (error) {
  console.error("Произошла ошибка при удалении участника:", error);
  setError("Не удалось создать турнир");
}
}

export const handleSetWinner = async (tid: string, mid: string, winnerId: string, looserId: string, winPoints: number, loosePoints: number) => {
  try {
    const token = getToken();
    if(!token) return null;
    await fetch(`${API_BASE}/${tid}/${mid}/result`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify({
        winnerId,
        looserId,
        winPoints,
        loosePoints
      })
    });
  } catch (error) {
    console.error("Ошибка при назначении победителя:", error);
  }
};

export async function registerForTournament(tid: string, name: string) {
  try {
      const token = getToken();
      if(!token) return null;
      const res = await fetch(`${API_BASE}/${tid}/participants/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`, },
          body: JSON.stringify({
           name
          })
      });
  } catch (error) {
    console.error("Произошла ошибка при регистрации:", error);
    setError("Не удалось зарегистрироваться");
  }
  }


  export async function fetchPlayingParticipants(id: string){
    try {
        const response = await fetch(`${API_BASE}/${id}/participants/plays`, {
          method: "GET",
          headers: { "Content-Type": "application/json" },
          mode: "cors",
        });
        return response.json();
    } catch (error) {
      console.error("Произошла ошибка при получении матчей:", error);
      setError("Не удалось получить матчи");
    }
    }

function setError(arg0: string) {
  throw new Error(arg0);
}
