// News dto


export interface Tag {
  id: string;
  name: string;
}

export interface ListNews {
  id: string;
  title: string;
  authorId: string;
  authorName: string;
  publishingDate: string;
  categoryId: string;
  imagePath: string;
}

export interface DetailNews {
  id: string;
  title: string;
  authorId: string;
  publishingDate: Date;
  content: string;
  categoryId: string;
  authorName: string;
  imagePath: string;
  tags: Tag[];
  visibility: boolean;
}

export const NewsSortOptions = [
  {id: 0, name:"По названию"},
  {id: 1, name:"По убыванию названия"},
  {id: 2, name:"По дате"},
  {id: 3, name:"По убыванию даты"},
];


// Discipline dto


export interface Discipline {
  id: string;
  name: string;
  description: string;
  createdAt: string;
}


// User dto


export interface UserCleanDto {
  id: string;
  userName: string;
  email: string;
}

export interface UserProfileDto {
  id: string;
  userName: string;
  email: string;
  bio: string;
  registeredAt: Date;
  image: string;
  tournaments: string[];
}

export interface RoleDto {
  name: string;
}

export interface UserUpdatedto {
  userName: string;
  email: string;
  bio: string;
}

// Match dto


export interface MatchList {
  id: string;
  round: string;
  startTime: string;
  status: number;
  matchOrder: number;
  winScore: number;
  looseScore: number;
  endTime: string;
  participant1Name: string;
  participant2Name: string;
  tournamentName: string;
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
  participant1Name: string;
  participant2Name: string;
  tournamentName: string;
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

export const MatchSortOptions = [
  {id: 0, name:"По раунду"},
  {id: 1, name:"По убыванию раунда"},
  {id: 2, name:"По очереди"},
  {id: 3, name:"По убыванию очереди"},
  {id: 4, name:"По дате"},
  {id: 5, name:"По убыванию даты"},
];


// Tournament dto


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
  imagePath: string;
}

export interface TournamentFilter {
  SearchString?: string;
  CategoryId?: string;
  Status?: number;
  Format?: number;
  StartTime?: string;
  EndTime?: string;
}

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
  isRegistrationAllowed: boolean;
  imagePath: string;
}

export interface TournamentCreateDto {
  name: string;
  disciplineId: string;
  format: number;
  maxParticipants: number;
  isRegistrationAllowed: boolean;
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

export interface ParticipantSListDto{   
  id: string;   
  name: string; 
}

export interface ParticipantCreateDto {
  name: string;
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

export const ParticipantStatus = [
  {id:0, name:"Участсвует"},
  {id:1, name:"Не участвует"},
];

export const TournamentSortOptions = [
  {id: 0, name:"По названию"},
  {id: 1, name:"По убыванию названия"},
  {id: 2, name:"По дате"},
  {id: 3, name:"По убыванию даты"},
  {id: 4, name:"По кол-ву участников"},
  {id: 5, name:"По убыванию кол-ва участников"},
];

export const ParticipantSortOptions = [
  {id: 0, name:"По имени"},
  {id: 1, name:"По убыванию имени"},
  {id: 2, name:"По очкам"},
  {id: 3, name:"По убыванию очков"},
];


