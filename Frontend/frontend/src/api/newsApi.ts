import { getToken } from "./AuthHook";

const API_BASE = "http://localhost:5149/api";

export async function fetchNews(query: string) {
try {
    const response = await fetch(`${API_BASE}/news/filter?${query}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json"
      },
      mode: "cors"
      },);
    return response.json();
} catch (error) {
  console.error("Произошла ошибка при загрузке данных:", error);
  setError("Не удалось загрузить данные");
}
}

export async function fetchTags() {
try {
    const response = await fetch(`${API_BASE}/tags/list/`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json"
      },
      mode: "cors"
      },);
      return response.json();
} catch (error) {
  console.error("Произошла ошибка при загрузке тегов:", error);
  setError("Не удалось загрузить теги");
}
}

export async function addNews(news: any) {
try {
    const token = getToken();
    if(!token) return null;
    const response = await fetch(`${API_BASE}/news`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`,
      },
      body: JSON.stringify(news),
    });
    return response.json();
} catch (error) {
  console.error("Произошла ошибка при добавлении новостей:", error);
  setError("Не удалось добавить новость");
}
}

export async function fetchPieceOfNews(id: string | undefined) {
try {
    const response = await fetch(`${API_BASE}/news/${id}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json"
      },
      mode: "cors"
      },);
      return response.json();
} catch (error) {
  console.error("Произошла ошибка при загрузке новости:", error);
  setError("Не удалось загрузить новость");
}
}

export async function saveNewsChanges(id: string | undefined, saving: string){
try {
    const token = getToken();
    if(!token) return null;
    const response = await fetch(`${API_BASE}/news/${id}`,{
      method: 'PUT',
      headers: { 
        'Content-Type': 'application/json',
        "Authorization": `Bearer ${token}`,
       },
      body: saving,
    })
    return response.json();
} catch (error) {
  console.error("Произошла ошибка при обновлении новости:", error);
  setError("Не удалось обновить новость");
}
}

export async function deletePieceOfNews(id: string | undefined){
try {
    const token = getToken();
    if(!token) return null;
    const response = await fetch(`${API_BASE}/news/${id}`, {
      method: 'DELETE',
      headers: { 
        'Content-Type': 'application/json',
        "Authorization": `Bearer ${token}`,
       },
    });
    return response.json();
} catch (error) {
  console.error("Произошла ошибка при удалении новости:", error);
  setError("Не удалось удалить новость");
}
}


export async function addTagToPieceOfNews(id: string | undefined, selectedTag: string){
try {
    const token = getToken();
    if(!token) return null;
    const response = await fetch(`${API_BASE}/news/${id}/${selectedTag}`, {
      method: 'PUT',
      headers: { 
        'Content-Type': 'application/json',
        "Authorization": `Bearer ${token}`,
       },
    });
    return response.json();
} catch (error) {
  console.error("Произошла ошибка при добавлении тега:", error);
  setError("Не удалось добавить тег");
}
}


export async function addPieceOfNews(saving: string){
try {
    const token = getToken();
    if(!token) return null;
    const response = await fetch(`${API_BASE}/news`,{
      method: 'POST',
      headers: { 
        'Content-Type': 'application/json',
        "Authorization": `Bearer ${token}`,
       },
      body: saving,
    })
    return response.json();
} catch (error) {
  console.error("Произошла ошибка при добавлении новости:", error);
  setError("Не удалось добавить новостьы");
}
}


export async function addTag(saving: string){
try {
    const token = getToken();
    if(!token) return null;
    const response = await fetch(`${API_BASE}/tags/${saving}`,{
      method: 'POST',
      headers: { 
        'Content-Type': 'application/json',
        "Authorization": `Bearer ${token}`,
       },
    })
    return response.json();
} catch (error) {
  console.error("Произошла ошибка при добавлении тега:", error);
  setError("Не удалось добавить тег");
}
}


function setError(arg0: string) {
  throw new Error(arg0);
}

