using ComputerClub.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputerClub.Client
{
    // ========== БРОНИРОВАНИЕ ПК И ОПЛАТА ==========
    public class ClientBookingModule
    {
        private List<Booking> bookings = new List<Booking>();
        private List<Computer> computers = new List<Computer>();
        private List<User> users = new List<User>();

        public ClientBookingModule()
        {
            // Инициализация тестовых данных
            computers.Add(new Computer { Id = 1, Name = "ПК №1", Status = ComputerStatus.Available, PricePerHour = 150 });
            computers.Add(new Computer { Id = 2, Name = "ПК №2", Status = ComputerStatus.Available, PricePerHour = 150 });
            computers.Add(new Computer { Id = 3, Name = "ПК №3", Status = ComputerStatus.Available, PricePerHour = 200 });
            
            users.Add(new User { Id = 1, Name = "Иван Петров", Balance = 1000 });
        }

        // Функция 1: Просмотр доступных ПК
        public List<Computer> GetAvailableComputers()
        {
            return computers.Where(c => c.Status == ComputerStatus.Available).ToList();
        }

        // Функция 2: Бронирование ПК
        public string BookComputer(int userId, int computerId, DateTime startTime, int hours)
        {
            // Проверяем существование пользователя
            var user = users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return "Ошибка: пользователь не найден";

            // Проверяем существование компьютера
            var computer = computers.FirstOrDefault(c => c.Id == computerId);
            if (computer == null) return "Ошибка: компьютер не найден";

            // Проверяем доступность
            if (computer.Status != ComputerStatus.Available)
                return "Ошибка: компьютер недоступен";

            // Рассчитываем стоимость
            decimal totalPrice = computer.PricePerHour * hours;

            // Создаем бронирование
            var booking = new Booking
            {
                Id = bookings.Count + 1,
                UserId = userId,
                ComputerId = computerId,
                StartTime = startTime,
                EndTime = startTime.AddHours(hours),
                TotalPrice = totalPrice,
                Status = BookingStatus.Pending,
                PaymentStatus = PaymentStatus.Pending
            };

            bookings.Add(booking);
            computer.Status = ComputerStatus.Reserved;

            return $"Бронирование создано! К оплате: {totalPrice} руб. ID брони: {booking.Id}";
        }

        // Функция 3: Оплата бронирования
        public string PayForBooking(int bookingId, string paymentMethod)
        {
            var booking = bookings.FirstOrDefault(b => b.Id == bookingId);
            if (booking == null) return "Ошибка: бронирование не найдено";

            var user = users.FirstOrDefault(u => u.Id == booking.UserId);
            if (user == null) return "Ошибка: пользователь не найден";

            // Проверяем баланс если оплата с баланса
            if (paymentMethod == "balance")
            {
                if (user.Balance < booking.TotalPrice)
                    return "Ошибка: недостаточно средств на балансе";

                user.Balance -= booking.TotalPrice;
            }

            booking.PaymentStatus = PaymentStatus.Paid;
            booking.Status = BookingStatus.Confirmed;

            return $"Оплата прошла успешно! Бронирование {bookingId} подтверждено.";
        }

        // Функция 4: Просмотр истории бронирований
        public List<Booking> GetUserBookings(int userId)
        {
            return bookings.Where(b => b.UserId == userId).ToList();
        }
    }
}