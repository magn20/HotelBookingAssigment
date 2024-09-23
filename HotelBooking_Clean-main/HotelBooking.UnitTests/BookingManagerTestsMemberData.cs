using System;
using System.Collections.Generic;
using HotelBooking.Core;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.VisualStudio.TestPlatform.Common.ExtensionFramework;

namespace HotelBooking.UnitTests;

public class BookingManagerTestsMemberData
{

    #region CreateBooking

    public static IEnumerable<object[]> getCreateBookingCanBookTrueData()
    {
        var data = new List<object[]>
        {

            new object[]
                { new Booking { StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(10) }, true },
        };
        return data;
    }
    
    public static IEnumerable<object[]> GetCreateBookingNoRoomAvailableFalseData()
    {
        var data = new List<object[]>
        {

            new object[]
                { new Booking { StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(10) }, false },
        };
        return data;
    }
    
    #endregion

    #region FullyOccupiedDates

    public static IEnumerable<object[]> GetFullyOccupiedDatesStartDateIsAfterEndDateThrowsArgumentExceptionData()
    {
        var data = new List<object[]>
        {

            new object[]
                { DateTime.Today.AddDays(2),DateTime.Today.AddDays(1) },
        };
        return data;
    }
    
    public static IEnumerable<object[]> GetFullyOccupiedDatesStartDateAndEndDateData()
    {
        var data = new List<object[]>
        {

            new object[]
                { DateTime.Today.AddDays(1),DateTime.Today.AddDays(2) },
        };
        return data;
    }
    
    public static IEnumerable<object[]> GetFullyOccupiedDatesFullBookedDateTimeTodayPlusOneDay()
    {
        var data = new List<object[]>
        {

            new object[]
            {
                DateTime.Today.AddDays(1),DateTime.Today.AddDays(2),
                new List<Booking>
                {
                    new Booking {IsActive = true,RoomId = 1,StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(1)},
                    new Booking {IsActive = true,RoomId = 2,StartDate = DateTime.Today.AddDays(1), EndDate = DateTime.Today.AddDays(1)},
                },
                new List<DateTime> {DateTime.Today.AddDays(1)} 
            },
        };
        return data;
    }
    
    
    public static IEnumerable<object[]> GetFullyOccupiedDatesStopsAtEndDateEmptyListDatetime()
    {
        var data = new List<object[]>
        {

            new object[]
            {
                DateTime.Today.AddDays(1),DateTime.Today.AddDays(2),
                new List<Booking>
                {
                    new Booking {IsActive = true,RoomId = 1,StartDate = DateTime.Today.AddDays(3), EndDate = DateTime.Today.AddDays(3)},
                    new Booking {IsActive = true,RoomId = 2,StartDate = DateTime.Today.AddDays(3), EndDate = DateTime.Today.AddDays(3)},
                }
            },
        };
        return data;
    }
    
    #endregion
}