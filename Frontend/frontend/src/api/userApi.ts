import { UserCleanDto } from "../types";

const API_BASE = "http://localhost:5153/api";

export async function register(username: string, email: string, password: string) {
try {
        const response = await fetch(`${API_BASE}/user`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ username, email, password }),
        });
    
        if (!response.ok) {
            const err = await response.text();
            throw new Error(err);
        }
        return response.json();
} catch (error) {
    console.error("Произошла ошибка при регистрации:", error);
    setError("Не удалось зарегистрироваться");
}
}

export async function updateProfile(profile: UserCleanDto): Promise<void> {
    try {
        const res = await fetch(`${API_BASE}/user/profile`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${localStorage.getItem("jwt")}`,
            },
            body: JSON.stringify(profile),
        });
    } catch (error) {
        console.error("Произошла ошибка при обновлении профиля:", error);
        setError("Не удалось обновить профиль");
    }
 
}

export async function deleteProfile(): Promise<void> {
    try {
        const res = await fetch(`${API_BASE}/user/profile`, {
            method: "DELETE",
            headers: {
                Authorization: `Bearer ${localStorage.getItem("jwt")}`,
            },
        });
    } catch (error) {
        console.error("Произошла ошибка при удалении профиля:", error);
        setError("Не удалось удалить профиль");
    }
}

export async function getProfile() {
try {
        const res = await fetch(`${API_BASE}/user/profile`, {
            headers: { Authorization: `Bearer ${localStorage.getItem("jwt")}` },
        });
        return res.json();
} catch (error) {
    console.error("Произошла ошибка при получении профиля:", error);
    setError("Не удалось получить профиль");
}
}

export const searchUsersByName = async (userName: string, page: number, pageSize: number) => {
    try {
        const res = await fetch(`${API_BASE}/user?page=${page}&pageSize=${pageSize}&userName=${userName}`, {
          credentials: "include",
        });
        return await res.json();
    } catch (error) {
        console.error("Произошла ошибка при получении пользователей:", error);
        setError("Не удалось получить пользователей");
    }
  };
  
  export const getUserById = async (id: string) => {
    try {
        const res = await fetch(`${API_BASE}/user/${id}`, {
          credentials: "include",
        });
        return await res.json();
    } catch (error) {
        console.error("Произошла ошибка при получении пользователя:", error);
        setError("Не удалось получить пользователя");
    }
  };
  
  export const deleteUserById = async (id: string): Promise<void> => {
    try {
        await fetch(`${API_BASE}/user/${id}`, {
          method: "DELETE",
          credentials: "include",
        });
    } catch (error) {
        console.error("Произошла ошибка при удалении пользователя:", error);
        setError("Не удалось удалить пользователя");
    }
  };

function setError(arg0: string) {
    throw new Error(arg0);
}
