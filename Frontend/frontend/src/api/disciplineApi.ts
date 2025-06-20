import { Discipline } from "../types";
//import { getToken } from "./auth";

const API_BASE = "http://localhost:5215/api";

export const fetchDisciplines = async (): Promise<Discipline[]> => {
    const response = await fetch(`${API_BASE}/disciplines`);
    return await response.json();
  };
  
  export const fetchDisciplineById = async (id: string): Promise<Discipline> => {
    const response = await fetch(`${API_BASE}/disciplines/${id}`);
    return await response.json();
  };