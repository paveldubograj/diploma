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

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [user, setUser] = useState<UserCleanDto | null>(null);
  const [token, setToken] = useState<string | null>(null);

  useEffect(() => {
    const stored = localStorage.getItem("user");
    if (stored) setUser(JSON.parse(stored));
    const tokenst = localStorage.getItem("token");
    if (tokenst) setToken(tokenst);
  }, []);

  const login = (userData: UserCleanDto, token: string) => {
    setUser(userData);
    setToken(token);
    localStorage.setItem("user", JSON.stringify(userData));
    localStorage.setItem("token", token)
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
  return localStorage.getItem("token");
};
