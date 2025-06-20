import { getToken } from "./../api/AuthHook";


export function getUserRoles(): string[] {
    const token = getToken();
    if (!token) return [];
  
    const payload = token.split('.')[1];
    const decoded = JSON.parse(atob(payload));
    return decoded["role"] ? (Array.isArray(decoded["role"]) ? decoded["role"] : [decoded["role"]]) : [];
  }
  
  export function hasRole(role: string): boolean {
    return getUserRoles().includes(role);
  }
  