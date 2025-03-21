namespace TournamentService.Shared.Enums;

public enum TournamentStatus
{
    Pending,    // Ожидание старта
    Ongoing,    // Турнир идет
    Completed,  // Завершен
    Cancelled,  // Отменен
    Paused      // Приостановлен
}
