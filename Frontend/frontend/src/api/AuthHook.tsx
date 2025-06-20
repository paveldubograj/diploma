import React, { createContext, useContext, useState, useEffect } from "react";
import { UserCleanDto } from "../types";

interface AuthContextType {
  user: UserCleanDto | null;
  token: string | null;
  login: (user: UserCleanDto, token: string) => void;
  logout: () => void;
}

export const AuthContext = createContext<AuthContextType>({
  user: null,
  token: null,
  login: () => {},
  logout: () => {},
});

function parseJwt(token: string): { exp: number } {
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(atob(base64).split('').map(c => 
      '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)
    ).join(''));

    return JSON.parse(jsonPayload);
  } catch (e) {
    console.error("Ошибка парсинга токена", e);
    return { exp: 0 };
  }
}

function isTokenExpired(token: string): boolean {
  if (!token) return true;

  try {
    const { exp } = parseJwt(token);
    const expirationTime = exp * 1000; 
    return Date.now() >= expirationTime;
  } catch {
    return true;
  }
}

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [user, setUser] = useState<UserCleanDto | null>(null);
  const [token, setToken] = useState<string | null>(null);

  useEffect(() => {
    const storedUser = localStorage.getItem("user");
    const storedToken = localStorage.getItem("token");

    if (storedToken && isTokenExpired(storedToken)) {
      localStorage.removeItem("user");
      localStorage.removeItem("token");
      return;
    }

    if (storedUser) setUser(JSON.parse(storedUser));
    if (storedToken) setToken(storedToken);
  }, []);

  const login = (userData: UserCleanDto, accessToken: string) => {
    if (isTokenExpired(accessToken)) {
      console.error("Нельзя войти с просроченным токеном");
      return;
    }

    setUser(userData);
    setToken(accessToken);
    localStorage.setItem("user", JSON.stringify(userData));
    localStorage.setItem("token", accessToken);
  };

  const logout = () => {
    setUser(null);
    setToken(null);
    localStorage.removeItem("user");
    localStorage.removeItem("token");
  };

  return (
    <AuthContext.Provider value={{ user, token, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);

export const getToken = (): string | null => {
  const token = localStorage.getItem('token');

  if (!token && window.location.pathname !== '/login') {
    return null; 
  }

  return token;
};

export const getUser = (): UserCleanDto | null => {
  let k = localStorage.getItem("user");
  if(k)
    return JSON.parse(k);
  return null;
}
