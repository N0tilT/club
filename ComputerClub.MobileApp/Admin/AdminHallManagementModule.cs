using ComputerClub.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputerClub.Admin
{
    // ========== СХЕМА ЗАЛА И УПРАВЛЕНИЕ СЕССИЯМИ ==========
    public class AdminHallManagementModule
    {
        private List<Computer> computers = new List<Computer>();
        private List<Session> sessions = new List<Session>();
        private List<User> users = new List<User>();

        public AdminHallManagementModule()
        {
            // Инициализация тестовых данных
            computers.Add(new Computer { Id = 1, Name = "ПК №1", Row = 1, Place = 1, Status = ComputerStatus.Available, PricePerHour = 150 });
            computers.Add(new Computer { Id = 2, Name = "ПК №2", Row = 1, Place = 2, Status = ComputerStatus.Occupied, PricePerHour = 150 });
            computers.Add(new Computer { Id = 3, Name = "ПК №3", Row = 1, Place = 3, Status = ComputerStatus.Available, PricePerHour = 200 });
            computers.Add(new Computer { Id = 4, Name = "ПК №4", Row = 2, Place = 1, Status = ComputerStatus.Maintenance, PricePerHour = 150 });
            computers.Add(new Computer { Id = 5, Name = "ПК №5", Row = 2, Place = 2, Status = ComputerStatus.Reserved, PricePerHour = 200 });
            
            users.Add(new User { Id = 1, Name = "Иван Петров", Balance = 1000 });
            users.Add(new User { Id = 2, Name = "Анна Смирнова", Balance = 500 });
            
            sessions.Add(new Session
            {
                Id = 1,
                UserId = 2,
                ComputerId = 2,
                StartTime = DateTime.Now.AddHours(-2),
                Status = SessionStatus.Active
            });
        }

        // Функция 1: Получение схемы зала
        public HallScheme GetHallScheme()
        {
            var scheme = new HallScheme();
            
            // Группируем по рядам
            var computersByRow = computers
                .GroupBy(c => c.Row)
                .OrderBy(g => g.Key);

            foreach (var row in computersByRow)
            {
                var rowComputers = row.OrderBy(c => c.Place).Select(c => new HallComputer
                {
                    Id = c.Id,
                    Name = c.Name,
                    Status = c.Status,
                    StatusDisplay = GetStatusDisplay(c.Status),
                    StatusColor = GetStatusColor(c.Status),
                    CurrentUser = sessions
                        .Where(s => s.ComputerId == c.Id && s.Status == SessionStatus.Active)
                        .Select(s => users.FirstOrDefault(u => u.Id == s.UserId)?.Name)
                        .FirstOrDefault()
                }).ToList();

                scheme.Rows.Add(new HallRow
                {
                    RowNumber = row.Key,
                    Computers = rowComputers
                });
            }

            // Считаем статистику
            scheme.TotalComputers = computers.Count;
            scheme.AvailableComputers = computers.Count(c => c.Status == ComputerStatus.Available);
            scheme.OccupiedComputers = computers.Count(c => c.Status == ComputerStatus.Occupied);
            scheme.ReservedComputers = computers.Count(c => c.Status == ComputerStatus.Reserved);
            scheme.MaintenanceComputers = computers.Count(c => c.Status == ComputerStatus.Maintenance);

            return scheme;
        }

        // Функция 2: Запуск сессии для пользователя
        public string StartUserSession(int userId, int computerId, string adminName)
        {
            var computer = computers.FirstOrDefault(c => c.Id == computerId);
            if (computer == null) return "Ошибка: компьютер не найден";
            
            if (computer.Status != ComputerStatus.Available)
                return $"Ошибка: компьютер {computer.Status}";

            var user = users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return "Ошибка: пользователь не найден";

            var session = new Session
            {
                Id = sessions.Count + 1,
                UserId = userId,
                ComputerId = computerId,
                StartTime = DateTime.Now,
                Status = SessionStatus.Active,
                StartedByAdmin = adminName
            };

            sessions.Add(session);
            computer.Status = ComputerStatus.Occupied;

            return $"Сессия запущена для {user.Name} на {computer.Name}";
        }

        // Функция 3: Завершение сессии
        public string EndUserSession(int computerId)
        {
            var computer = computers.FirstOrDefault(c => c.Id == computerId);
            if (computer == null) return "Ошибка: компьютер не найден";

            var activeSession = sessions.FirstOrDefault(s => 
                s.ComputerId == computerId && s.Status == SessionStatus.Active);
            
            if (activeSession == null)
                return "Ошибка: нет активной сессии";

            // Завершаем сессию
            activeSession.EndTime = DateTime.Now;
            activeSession.Status = SessionStatus.Completed;
            
            // Рассчитываем стоимость
            var hours = (decimal)(activeSession.EndTime.Value - activeSession.StartTime).TotalHours;
            activeSession.TotalPrice = Math.Ceiling(hours * 2) / 2 * computer.PricePerHour;

            // Освобождаем компьютер
            computer.Status = ComputerStatus.Available;

            return $"Сессия завершена. Время: {hours:F1} ч. Сумма: {activeSession.TotalPrice} руб.";
        }

        // Функция 4: Изменение статуса компьютера
        public string ChangeComputerStatus(int computerId, ComputerStatus newStatus, string reason)
        {
            var computer = computers.FirstOrDefault(c => c.Id == computerId);
            if (computer == null) return "Ошибка: компьютер не найден";

            var oldStatus = computer.Status;
            computer.Status = newStatus;
            computer.Notes = $"[{DateTime.Now}] Статус изменен с {oldStatus} на {newStatus}. Причина: {reason}";

            return $"Статус компьютера {computer.Name} изменен на {newStatus}";
        }

        // Функция 5: Продление сессии
        public string ExtendUserSession(int computerId, int additionalMinutes)
        {
            var activeSession = sessions.FirstOrDefault(s => 
                s.ComputerId == computerId && s.Status == SessionStatus.Active);
            
            if (activeSession == null)
                return "Ошибка: нет активной сессии";

            activeSession.Notes += $" | Продлено на {additionalMinutes} мин.";
            
            return $"Сессия продлена на {additionalMinutes} минут";
        }

        // Вспомогательные функции
        private string GetStatusDisplay(ComputerStatus status)
        {
            switch (status)
            {
                case ComputerStatus.Available: return "Свободен";
                case ComputerStatus.Occupied: return "Занят";
                case ComputerStatus.Reserved: return "Забронирован";
                case ComputerStatus.Maintenance: return "Обслуживание";
                case ComputerStatus.Broken: return "Сломан";
                default: return status.ToString();
            }
        }

        private string GetStatusColor(ComputerStatus status)
        {
            switch (status)
            {
                case ComputerStatus.Available: return "Green";
                case ComputerStatus.Occupied: return "Red";
                case ComputerStatus.Reserved: return "Orange";
                case ComputerStatus.Maintenance: return "Gray";
                case ComputerStatus.Broken: return "Black";
                default: return "Gray";
            }
        }
    }

    // Классы для схемы зала
    public class HallScheme
    {
        public List<HallRow> Rows { get; set; } = new List<HallRow>();
        public int TotalComputers { get; set; }
        public int AvailableComputers { get; set; }
        public int OccupiedComputers { get; set; }
        public int ReservedComputers { get; set; }
        public int MaintenanceComputers { get; set; }
    }

    public class HallRow
    {
        public int RowNumber { get; set; }
        public List<HallComputer> Computers { get; set; } = new List<HallComputer>();
    }

    public class HallComputer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ComputerStatus Status { get; set; }
        public string StatusDisplay { get; set; }
        public string StatusColor { get; set; }
        public string CurrentUser { get; set; }
    }
}