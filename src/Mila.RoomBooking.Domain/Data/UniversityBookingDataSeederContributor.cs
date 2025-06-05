// UniversityBookingDataSeederContributor.cs
using System;
using System.Threading.Tasks;
using UniversityBooking.Days;
using UniversityBooking.Rooms;
using UniversityBooking.TimeSlots;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace UniversityBooking.Data
{
    public class UniversityBookingDataSeederContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Room, Guid> _roomRepository;
        private readonly IRepository<TimeSlot, Guid> _timeSlotRepository;
        private readonly IRepository<Day, Guid> _dayRepository;
        private readonly IGuidGenerator _guidGenerator;

        public UniversityBookingDataSeederContributor(
            IRepository<Room, Guid> roomRepository,
            IRepository<TimeSlot, Guid> timeSlotRepository,
            IRepository<Day, Guid> dayRepository,
            IGuidGenerator guidGenerator)
        {
            _roomRepository = roomRepository;
            _timeSlotRepository = timeSlotRepository;
            _dayRepository = dayRepository;
            _guidGenerator = guidGenerator;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            // Only insert one set of days (either English or Arabic)
            if (await _dayRepository.GetCountAsync() == 0)
            {
                await _dayRepository.InsertAsync(
                    new Day(_guidGenerator.Create(), "الأحد", DayOfWeek.Sunday));
                await _dayRepository.InsertAsync(
                    new Day(_guidGenerator.Create(), "الاثنين", DayOfWeek.Monday));
                await _dayRepository.InsertAsync(
                    new Day(_guidGenerator.Create(), "الثلاثاء", DayOfWeek.Tuesday));
                await _dayRepository.InsertAsync(
                    new Day(_guidGenerator.Create(), "الأربعاء", DayOfWeek.Wednesday));
                await _dayRepository.InsertAsync(
                    new Day(_guidGenerator.Create(), "الخميس", DayOfWeek.Thursday));
            }

            // Seed time slots (based on the image time slots)
            if (await _timeSlotRepository.GetCountAsync() == 0)
            {
                await _timeSlotRepository.InsertAsync(
                    new TimeSlot(_guidGenerator.Create(), "8-9", new TimeSpan(8, 0, 0), new TimeSpan(9, 0, 0)));
                await _timeSlotRepository.InsertAsync(
                    new TimeSlot(_guidGenerator.Create(), "9-10", new TimeSpan(9, 0, 0), new TimeSpan(10, 0, 0)));
                await _timeSlotRepository.InsertAsync(
                    new TimeSlot(_guidGenerator.Create(), "10-11", new TimeSpan(10, 0, 0), new TimeSpan(11, 0, 0)));
                await _timeSlotRepository.InsertAsync(
                    new TimeSlot(_guidGenerator.Create(), "11-12", new TimeSpan(11, 0, 0), new TimeSpan(12, 0, 0)));
                await _timeSlotRepository.InsertAsync(
                    new TimeSlot(_guidGenerator.Create(), "12-1", new TimeSpan(12, 0, 0), new TimeSpan(13, 0, 0)));
                await _timeSlotRepository.InsertAsync(
                    new TimeSlot(_guidGenerator.Create(), "1-2", new TimeSpan(13, 0, 0), new TimeSpan(14, 0, 0)));
                await _timeSlotRepository.InsertAsync(
                    new TimeSlot(_guidGenerator.Create(), "2-3", new TimeSpan(14, 0, 0), new TimeSpan(15, 0, 0)));
                await _timeSlotRepository.InsertAsync(
                    new TimeSlot(_guidGenerator.Create(), "3-4", new TimeSpan(15, 0, 0), new TimeSpan(16, 0, 0)));
                await _timeSlotRepository.InsertAsync(
                    new TimeSlot(_guidGenerator.Create(), "4-5", new TimeSpan(16, 0, 0), new TimeSpan(17, 0, 0)));
                await _timeSlotRepository.InsertAsync(
                    new TimeSlot(_guidGenerator.Create(), "5-6", new TimeSpan(17, 0, 0), new TimeSpan(18, 0, 0)));
                await _timeSlotRepository.InsertAsync(
                    new TimeSlot(_guidGenerator.Create(), "6-7", new TimeSpan(18, 0, 0), new TimeSpan(19, 0, 0)));
                await _timeSlotRepository.InsertAsync(
                    new TimeSlot(_guidGenerator.Create(), "7-8", new TimeSpan(19, 0, 0), new TimeSpan(20, 0, 0)));
                await _timeSlotRepository.InsertAsync(
                    new TimeSlot(_guidGenerator.Create(), "8-9", new TimeSpan(20, 0, 0), new TimeSpan(21, 0, 0)));
            }

            // Semester data seeding removed

            // Seed sample rooms
            if (await _roomRepository.GetCountAsync() == 0)
            {
                await _roomRepository.InsertAsync(
                    new Room(
                        _guidGenerator.Create(),
                        "88",
                        "Engineering",
                        "Ground",
                        30,
                        "Computer Lab for engineering students"

                    )
                );

                await _roomRepository.InsertAsync(
                    new Room(
                        _guidGenerator.Create(),
                        "101",
                        "Engineering",
                        "First",
                        60,

                        "Classroom for engineering lectures"
                          ,
                        RoomCategory.Lab
                    )
                );

                await _roomRepository.InsertAsync(
                    new Room(
                        _guidGenerator.Create(),
                        "201",
                        "Science",
                        "Second",
                        40,
                        "Physics Lab"
                    )
                );
            }
        }
    }
}
