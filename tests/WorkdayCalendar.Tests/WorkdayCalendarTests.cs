using System;
using Xunit;
using WorkdayCalendar;

namespace WorkdayCalendar.Tests
{
    public class WorkdayCalendarTests
    {
        [Fact]
        public void AddWorkdays_WhenSubtracting5Point5DaysFromEvening_ReturnsExpectedDateTime()
        {
            // Arrange
            var calendar = CreateCalendar();
            var start = new DateTime(2004, 5, 24, 18, 5, 0); // May 24, 2004, 6:05 PM
            // Act
            var result = calendar.AddWorkdays(start, -5.5);

            // Assert
            var expected = new DateTime(2004, 5, 14, 12, 0, 0); // May 14, 2004, 12:00 PM
            
            Assert.Equal(expected.Date, result.Date);
            Assert.Equal(expected.Hour, result.Hour);
            Assert.Equal(expected.Minute, result.Minute);
        }

        [Fact]
        public void AddWorkdays_WhenAdding44Point723656DaysFromEvening_ReturnsExpectedDateTime()
        {
            // Arrange
            var calendar = CreateCalendar();
            var start = new DateTime(2004, 5, 24, 19, 3, 0); // May 24, 2004, 7:03 PM

            // Act
            var result = calendar.AddWorkdays(start, 44.723656);

            // Assert
            var expected = new DateTime(2004, 7, 27, 13, 47, 0); // July 27, 2004, 1:47 PM
            
            Assert.Equal(expected.Date, result.Date);
            Assert.Equal(expected.Hour, result.Hour);
            Assert.Equal(expected.Minute, result.Minute);
        }

        [Fact]
        public void AddWorkdays_WhenSubtracting6Point7470217DaysFromEvening_ReturnsExpectedDateTime()
        {
            // Arrange
            var calendar = CreateCalendar();
            var start = new DateTime(2004, 5, 24, 18, 3, 0); // May 24, 2004, 6:03 PM

            // Act
            var result = calendar.AddWorkdays(start, -6.7470217);

            Console.WriteLine($"Result: {result:yyyy-MM-dd HH:mm:ss.fffffff}");
            // Assert
            var expected = new DateTime(2004, 5, 13, 10, 2, 0); // May 13, 2004, 10:02 AM
            
            Assert.Equal(expected.Date, result.Date);
            Assert.Equal(expected.Hour, result.Hour);

            var difference = (result - expected).Duration();
            Assert.True(difference < TimeSpan.FromMinutes(1), "Difference was in minutes: " + difference.TotalMinutes);
        }

        [Fact]
        public void AddWorkdays_WhenAdding12Point782709DaysFromMorning_ReturnsExpectedDateTime()
        {
            // Arrange
            var calendar = CreateCalendar();
            var start = new DateTime(2004, 5, 24, 8, 3, 0); // May 24, 2004, 8:03 AM

            // Act
            var result = calendar.AddWorkdays(start, 12.782709);

            // Assert
            var expected = new DateTime(2004, 6, 10, 14, 18, 0); // June 10, 2004, 2:18 PM
            
            Assert.Equal(expected.Date, result.Date);
            Assert.Equal(expected.Hour, result.Hour);
            Assert.Equal(expected.Minute, result.Minute);
        }

        [Fact]
        public void AddWorkdays_WhenAdding8Point276628DaysStartingBeforeWorkday_ReturnsExpectedDateTime()
        {
            // Arrange
            var calendar = CreateCalendar();
            var start = new DateTime(2004, 5, 24, 7, 3, 0); // May 24, 2004, 7:03 AM

            // Act
            var result = calendar.AddWorkdays(start, 8.276628);

            // Assert
            var expected = new DateTime(2004, 6, 4, 10, 12, 0); // June 4, 2004, 10:12 AM
            
            Assert.Equal(expected.Date, result.Date);
            Assert.Equal(expected.Hour, result.Hour);
            Assert.Equal(expected.Minute, result.Minute);
        }

        private WorkdayCalendar CreateCalendar()
        {
            var settings = new WorkdaySettings(TimeSpan.FromHours(8), TimeSpan.FromHours(16));
            var holidays = new HolidayCalendar();

            holidays.AddRecurringHoliday(5, 17);

            holidays.AddHoliday(new DateTime(2004, 5, 27));

            return new WorkdayCalendar(settings, holidays);
        }
    }
}