// src/types.ts

export interface Tag {
  id: string;
  name: string;
}

export interface ListNews {
  id: string;
  title: string;
  authorId: string;
  publishingDate: Date;
  categoryId: string;
}

export interface DetailNews {
  id: string;
  title: string;
  authorId: string;
  publishingDate: Date;
  content: string;
  categoryId: string;
  tags: Tag[];
}

export interface Discipline {
  id: string;
  name: string;
  description: string;
  createdAt: string;
}

export interface UserCleanDto {
  id: string;
  userName: string;
  email: string;
}


export interface MatchList {
  id: string;
  round: string;
  startTime: string;
  status: number;
  matchOrder: number;
  winScore: number;
  looseScore: number;
  endTime: string;
  categoryId: string;
  winnerId: string;
  participant1Id: string;
  participant2Id: string;
  tournamentId: string;
}


export interface MatchDto {
  id: string;
  round: string;
  startTime: string;
  status: number;
  matchOrder: number;
  winScore: number;
  looseScore: number;
  endTime: string;
  categoryId: string;
  winnerId: string;
  participant1Id: string;
  participant2Id: string;
  tournamentId: string;
  nextMatchId: string | null;
  ownerId: string;
}

export const MatchStatus = [
  {id: 0, name:"Запланирован"},
  {id: 1, name:"Идёт"},
  {id: 2, name:"Завершен"},
  {id: 3, name:"Отменён"},
  {id: 4, name:"Перенесён"}
];

export interface TournamentCleanDto {
  id: string;
  name: string;
  disciplineId: string;
  status: number;
  format: number;
  rounds: number;
  maxParticipants: number;
  ownerId: string;
  winnerId?: string;
}

export interface TournamentFilter {
  SearchString?: string;
  CategoryId?: string;
  Status?: number;
  Format?: number;
  StartTime?: string; // ISO string format
  EndTime?: string;
}

export const TournamentStatus = [
  {id: 0, name:"Запланирован"},
  {id: 1, name:"Идёт"},
  {id: 2, name:"Завершен"},
  {id: 3, name:"Отменён"},
  {id: 4, name:"Остановлен"}
];

export const TournamentFormat = [
  {id: 0, name:"Single Elimination"},
  {id: 1, name:"Double Elimination"},
  {id: 2, name:"Round Robin"},
  {id: 3, name:"Swiss"},
];

export interface TournamentDto {
  id: string;
  name: string;
  disciplineId: string;
  status: number;
  format: number;
  rounds: number;
  maxParticipants: number;
  startDate: string;
  endDate: string;
  ownerId: string;
  winnerId: string;
  participants: ParticipantDto[];
}

export interface TournamentCreateDto {
  name: string;
  disciplineId: string;
  format: number;
  maxParticipants: number;
  startDate: string;
  endDate: string;
}

export interface ParticipantDto{
  id: string;
  name: string;
  userId: string | null;
  points: number;
  tournamentId: string;
  status: number;
}

export interface ParticipantCreateDto {
  name: string;
}


export const ParticipantStatus = [
  {id:0, name:"Участсвует"},
  {id:1, name:"Не участвует"},
];

export interface ParticipantDto {
  id: string;
  name: string;
  userId: string | null;
  points: number;
  tournamentId: string;
  status: number;
}
// MatchStatus.set(0, "Scheduled");
// MatchStatus.set(1, "InProgress");
// MatchStatus.set(2, "Completed");
// MatchStatus.set(3, "Cancelled");
// MatchStatus.set(4, "Postponed");
// [
//   [0, "Scheduled"],
//   [1, "InProgress"],
//   [2, "Completed"],
//   [3, "Cancelled"],
//   [4, "Postponed"]
// ]

// const MatchStatus = new Map<string, string>([
//   ["0", "Scheduled"],
//   ["1", "InProgress"],
//   ["2", "Completed"],
//   ["3", "Cancelled"],
//   ["4", "Postponed"]
// ]);


