import { MatchDto } from "../types";
import { getToken } from "./AuthHook";

const API_BASE = "http://localhost:5083/api";

export async function fetchMatches(search: string) {
try {
    const response = await fetch(`${API_BASE}/matches/filter?${search}`, {
      method: "GET",
      headers: { "Content-Type": "application/json" },
      mode: "cors",
    });
    return response.json();
} catch (error) {
  console.error("Произошла ошибка при загрузке матчей:", error);
  setError("Не удалось загрузить матчи");
}
  //else throw new Error("Ошибка при получении");
}

export const getMatchById = async (id: string) => {
try {
    const response = await fetch(`${API_BASE}/matches/${id}`);
    return response.json();
} catch (error) {
  console.error("Произошла ошибка при загрузке матча:", error);
  setError("Не удалось загрузить матч");
}
};

export const updateMatch = async (id: string, data: MatchDto) => {
try {
    const token = getToken();
    if(!token) return null;
    const response = await fetch(`${API_BASE}/match/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`, 
              },
      body: JSON.stringify(data),
    });
} catch (error) {
  console.error("Произошла ошибка при обновлении матча:", error);
  setError("Не удалось обновить матч");
}
};

export const deleteMatch = async (id: string) => {
try {
    const token = getToken();
    if(!token) return null;
    const response = await fetch(`${API_BASE}/match/${id}`, {
      method: "DELETE",
      headers: { "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`, 
      },
    });
} catch (error) {
  console.error("Произошла ошибка при удалении матча:", error);
  setError("Не удалось удалить матч");
}
};

function setError(arg0: string) {
  throw new Error(arg0);
}
