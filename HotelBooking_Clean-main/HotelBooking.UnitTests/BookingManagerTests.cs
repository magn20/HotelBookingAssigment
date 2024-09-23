using System;
using System.Collections.Generic;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Xunit;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using HotelBooking.Infrastructure.Repositories;
using Moq;
using static HotelBooking.UnitTests.BookingManagerTestsMemberData;


namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {
        private IBookingManager bookingManager;
        Mock<IRepository<Booking>> mockBookingRepo;
        Mock<IRepository<Room>> mockRoomRepo;

        
        
        
        public BookingManagerTests(){
            
            //mock repo
            mockBookingRepo = new Mock<IRepository<Booking>>();
            mockRoomRepo = new Mock<IRepository<Room>>();

            
            //default fake data for Mocked repo
            var rooms = new List<Room>
            {
            new Room {Id = 1, Description = "1"},
            new Room {Id = 2, Description = "2"},
            };

            //setup for mockRepo's
            mockRoomRepo.Setup(x => x.GetAll()).Returns(rooms);

            mockRoomRepo.Setup(x => x.Get(1)).Returns(rooms[0]);
            
            bookingManager = new BookingManager(mockBookingRepo.Object, mockRoomRepo.Object);
        }

        #region GetCreateBooking
        
        /*
         * Rooms available for booking
         * returns true
         */
        [Theory]
        [MemberData(nameof(getCreateBookingCanBookTrueData), MemberType = typeof(BookingManagerTestsMemberData))]
        public void CreateBooking_BookingCanBook_True(Booking booking, Boolean expectedResult)
        {
            //Arrange
            
            //Act
            Boolean result = bookingManager.CreateBooking(booking);
            
            //Assert
            Assert.Equivalent(expectedResult, result, false);
        }
        /*
         * no rooms available for booking
         * returns false 
         */
        [Theory]
        [MemberData(nameof(GetCreateBookingNoRoomAvailableFalseData), MemberType = typeof(BookingManagerTestsMemberData))]
        public void CreateBooking_BookingNoRoomAvailable_False(Booking booking, Boolean expectedResult)
        {
            //Arrange
            mockRoomRepo.Setup(x => x.GetAll()).Returns([]);
            
            //Act
            Boolean result = bookingManager.CreateBooking(booking);
            
            //Assert
            Assert.Equivalent(expectedResult, result, false);
        }
        #endregion
        
        #region FindAvailableRoom
        
        /*
         * Startdate is today
         * return argumentException
         */
        [Fact]
        public void FindAvailableRoom_StartDateNotInTheFuture_ThrowsArgumentException()
        {
            // Arrange
            DateTime date = DateTime.Today;

            // Act
            Action act = () => bookingManager.FindAvailableRoom(date, date);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        /*
         * test finds an available room
         * returns !-1
         */
        [Fact]
        public void FindAvailableRoom_RoomAvailable_RoomIdNotMinusOne()
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            // Act
            int roomId = bookingManager.FindAvailableRoom(date, date);
            // Assert
            Assert.NotEqual(-1, roomId);
        }
        /*
         * Verify that FindAvailableRoom calls the Booking repo
         */
        [Fact]
        public void FindAvailableRoom_CallsBookingRepo_Once()
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            // Act
            bookingManager.FindAvailableRoom(date, date);
            // Assert
            mockBookingRepo.Verify(x => x.GetAll(), Times.Once);
        }
        /*
         * verify that FindAvailableRoom calls the Room Repo
         */
        [Fact]
        public void FindAvailableRoom_CallsRoomRepo_Once()
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            // Act
            bookingManager.FindAvailableRoom(date, date);
            // Assert
            mockRoomRepo.Verify(x => x.GetAll(), Times.Once);
        }
        
        
        [Fact]
        public void FindAvailableRoom_RoomAvailable_ReturnsAvailableRoom()
        {
            // This test was added to satisfy the following test design
            // principle: "Tests should have strong assertions".

            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            
            // Act
            int roomId = bookingManager.FindAvailableRoom(date, date);

            
            
            var bookingForReturnedRoomId = mockBookingRepo.Object.GetAll().Where(
                b => b.RoomId == roomId
                && b.StartDate <= date
                && b.EndDate >= date
                && b.IsActive);
            
            // Assert
            Assert.Empty(bookingForReturnedRoomId);
        }
        #endregion

        #region GetFullyOccupiedDates

        [Theory]
        [MemberData(nameof(GetFullyOccupiedDatesStartDateIsAfterEndDateThrowsArgumentExceptionData), MemberType = typeof(BookingManagerTestsMemberData))]
        public void GetFullyOccupiedDates_StartDateIsAfterEndDate_ThrowsArgumentException(DateTime startDate, DateTime endDate)
        {
            // Arrange

            // Act
            Action act = () => bookingManager.GetFullyOccupiedDates(startDate, endDate);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }
        
        /*
         * verify that GetFullyOccupiedDates calls the Room Repo
         */
        [Theory]
        [MemberData(nameof(GetFullyOccupiedDatesStartDateAndEndDateData), MemberType = typeof(BookingManagerTestsMemberData))]
        public void GetFullyOccupiedDates_CallsRoomRepo_Once(DateTime startDate, DateTime endDate)
        {
            // Arrange
            // Act
            bookingManager.GetFullyOccupiedDates(startDate, endDate);
            // Assert
            mockRoomRepo.Verify(x => x.GetAll(), Times.Once);
        }
        
        /*
         * Verify that FindAvailableRoom calls the Booking repo
         */
        [Theory]
        [MemberData(nameof(GetFullyOccupiedDatesStartDateAndEndDateData), MemberType = typeof(BookingManagerTestsMemberData))]
        public void GetFullyOccupiedDates_CallsBookingRepo_Once(DateTime startDate, DateTime endDate)
        {
            // Arrange
            // Act
            bookingManager.GetFullyOccupiedDates(startDate, endDate);
            // Assert
            mockBookingRepo.Verify(x => x.GetAll(), Times.Once);
        }
        
        /*
         * Test if no there isnt any bookings returns a empty list
         * returns List<Datetime>
         */
        [Theory]
        [MemberData(nameof(GetFullyOccupiedDatesStartDateAndEndDateData), MemberType = typeof(BookingManagerTestsMemberData))]
        public void GetFullyOccupiedDates_WhenNoBookings_EmptyDateTimeList(DateTime startDate, DateTime endDate)
        {
            // Arrange
            mockBookingRepo.Setup(x => x.GetAll()).Returns([]);
            // Act
            var result =bookingManager.GetFullyOccupiedDates(startDate, endDate);
            // Assert
            Assert.Empty(result);
        }
        
        
        /*
         * Test if all rooms are booked returns the Date
         * returns List<Datetime>
         */
        [Theory]
        [MemberData(nameof(GetFullyOccupiedDatesFullBookedDateTimeTodayPlusOneDay), MemberType = typeof(BookingManagerTestsMemberData))]
        public void GetFullyOccupiedDates_FullBooked_DateTimeTodayPlusOneDay(DateTime startDate, DateTime endDate, List<Booking> bookings, List<DateTime> expectedResult)
        {
            // Arrange
            mockBookingRepo.Setup(x => x.GetAll()).Returns(bookings);
            // Act
            var result =bookingManager.GetFullyOccupiedDates(startDate, endDate);
            // Assert
            Assert.Equivalent(expectedResult, result, false);
        }
        
        /*
         * Test GetFullyOccupiedDates stops At the endDate
         * returns List<Datetime>
         */
        [Theory]
        [MemberData(nameof(GetFullyOccupiedDatesStopsAtEndDateEmptyListDatetime), MemberType = typeof(BookingManagerTestsMemberData))]
        public void GetFullyOccupiedDates_StopsAtEndDate_EmptyListDatetime(DateTime startDate, DateTime endDate, List<Booking> bookings)
        {
            // Arrange
            mockBookingRepo.Setup(x => x.GetAll()).Returns(bookings);
            // Act
            var result =bookingManager.GetFullyOccupiedDates(startDate, endDate);
            // Assert
            Assert.Empty(result);
        }
        
        
        #endregion
    }
}
