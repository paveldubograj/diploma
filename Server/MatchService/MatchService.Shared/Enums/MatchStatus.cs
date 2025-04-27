using System;

namespace MatchService.Shared.Enums;

public enum MatchStatus
{
    Scheduled,     // Запланирован
    InProgress,    // В процессе
    Completed,     // Завершен
    Cancelled,     // Отменен
    Postponed,     // Перенесен
}
