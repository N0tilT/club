using ComputerClub.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputerClub.Client
{
    // ========== РЕГИСТРАЦИЯ НА ТУРНИРЫ ==========
    public class ClientTournamentModule
    {
        private List<Tournament> tournaments = new List<Tournament>();
        private List<TournamentRegistration> registrations = new List<TournamentRegistration>();

        public ClientTournamentModule()
        {
            // Инициализация тестовых турниров
            tournaments.Add(new Tournament
            {
                Id = 1,
                Name = "CS:GO Турнир",
                Game = "CS:GO",
                StartDate = DateTime.Now.AddDays(7),
                MaxParticipants = 16,
                CurrentParticipants = 8,
                EntryFee = 300,
                PrizePool = 5000,
                Status = TournamentStatus.Registration
            });

            tournaments.Add(new Tournament
            {
                Id = 2,
                Name = "Dota 2 Cup",
                Game = "Dota 2",
                StartDate = DateTime.Now.AddDays(14),
                MaxParticipants = 8,
                CurrentParticipants = 3,
                EntryFee = 500,
                PrizePool = 8000,
                Status = TournamentStatus.Registration
            });
        }

        // Функция 1: Просмотр доступных турниров
        public List<Tournament> GetAvailableTournaments()
        {
            return tournaments
                .Where(t => t.Status == TournamentStatus.Registration && 
                       t.StartDate > DateTime.Now)
                .OrderBy(t => t.StartDate)
                .ToList();
        }

        // Функция 2: Регистрация на турнир
        public string RegisterForTournament(int userId, int tournamentId)
        {
            var tournament = tournaments.FirstOrDefault(t => t.Id == tournamentId);
            if (tournament == null) return "Ошибка: турнир не найден";

            // Проверяем, есть ли уже регистрация
            var existingReg = registrations.FirstOrDefault(r => 
                r.UserId == userId && r.TournamentId == tournamentId);
            
            if (existingReg != null)
                return "Ошибка: вы уже зарегистрированы на этот турнир";

            // Проверяем количество мест
            if (tournament.CurrentParticipants >= tournament.MaxParticipants)
                return "Ошибка: нет свободных мест";

            // Создаем регистрацию
            var registration = new TournamentRegistration
            {
                Id = registrations.Count + 1,
                UserId = userId,
                TournamentId = tournamentId,
                RegistrationDate = DateTime.Now,
                Status = tournament.EntryFee > 0 ? 
                    RegistrationStatus.Pending : RegistrationStatus.Confirmed,
                PaymentStatus = tournament.EntryFee > 0 ? 
                    PaymentStatus.Pending : PaymentStatus.Paid
            };

            registrations.Add(registration);
            tournament.CurrentParticipants++;

            if (tournament.EntryFee == 0)
                return "Регистрация на турнир успешна!";
            else
                return $"Регистрация создана. Необходимо оплатить взнос: {tournament.EntryFee} руб.";
        }

        // Функция 3: Оплата взноса за турнир
        public string PayTournamentFee(int userId, int tournamentId)
        {
            var registration = registrations.FirstOrDefault(r => 
                r.UserId == userId && r.TournamentId == tournamentId);
            
            if (registration == null)
                return "Ошибка: регистрация не найдена";

            var tournament = tournaments.FirstOrDefault(t => t.Id == tournamentId);
            if (tournament == null)
                return "Ошибка: турнир не найден";

            if (registration.PaymentStatus == PaymentStatus.Paid)
                return "Взнос уже оплачен";

            // Здесь была бы логика оплаты
            registration.PaymentStatus = PaymentStatus.Paid;
            registration.Status = RegistrationStatus.Confirmed;

            return "Взнос оплачен! Вы зарегистрированы на турнир.";
        }

        // Функция 4: Мои турниры
        public List<Tournament> GetMyTournaments(int userId)
        {
            var myRegistrations = registrations
                .Where(r => r.UserId == userId)
                .Select(r => r.TournamentId)
                .ToList();

            return tournaments
                .Where(t => myRegistrations.Contains(t.Id))
                .OrderBy(t => t.StartDate)
                .ToList();
        }
    }
}