// api.ts
import { getToken } from './AuthHook';

const API_BASE = "http://localhost:5153/api";

export const apiFetch = async (url: string, options: RequestInit = {}) => {
  const token = getToken();

  const headers: HeadersInit = {
    ...(options.headers as Record<string, string>),
    "Content-Type": "application/json",
  };

  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  return fetch(`${API_BASE}${url}`, {
    ...options,
    headers,
    mode: "cors"
  });
};
