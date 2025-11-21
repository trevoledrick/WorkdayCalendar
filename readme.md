# Workday Calendar – Case Assignment

This repository contains an implementation of a Workday Calendar for the Element Logic case assignment.  
The solution calculates dates by adding or subtracting workdays while accounting for:

- Customizable workday hours (default 08:00–16:00)
- Weekends
- Fixed holidays
- Recurring holidays (e.g., 17th of May every year)
- Fractional workdays (e.g., 5.5 days)

The project is written in **C# (.NET 6)** and designed with clean architecture principles, testability, and SOLID in mind.

---

## Project Structure

/src
/WorkdayCalendar
- WorkdayCalendar.cs
- WorkdaySettings.cs
- HolidayCalendar.cs
- Interfaces (IWorkdayCalendar, IHolidayCalendar, IWorkdaySettings)

/tests
/WorkdayCalendar.Tests
- WorkdayCalendarTests.cs

---

## Key Concepts

### Workday Logic
A workday is defined by configurable start and end times (default 08:00–16:00).  
The calculator ensures:

- Start times outside the workday are normalized
- Time is added only within workday hours
- Moving through days skips weekends and holidays
- Fractional days are converted into time spans inside a workday

### Holiday Support
The solution supports:

- Single-date holidays (`AddHoliday(DateTime)`)
- Recurring annual holidays (`AddRecurringHoliday(month, day)`)

### Precision
Fractional days are calculated using ticks to avoid floating-point inaccuracies.

---

## Unit Tests

The test suite includes:

### Required case tests
- Adding and subtracting fractional workdays
- Handling time outside work hours
- Jumping over holidays
- Large fractional values

### Additional validation tests
- Starting on a weekend
- Starting on a holiday
- Adding zero workdays (normalization)

All tests pass at millisecond precision.

Run tests:

```bash
dotnet test

---

## Usage Example
var settings = new WorkdaySettings(TimeSpan.FromHours(8), TimeSpan.FromHours(16));
var holidays = new HolidayCalendar();
holidays.AddRecurringHoliday(5, 17);

var calendar = new WorkdayCalendar(settings, holidays);

var result = calendar.AddWorkdays(new DateTime(2004, 5, 24, 18, 05, 00), -5.5);

---

## Design Principles

SOLID

Separation of concerns

Dependency injection

Testability

Predictable domain logic

---

## Requirements

.NET 6 SDK

xUnit for tests

---

## Running the Project

Build:

dotnet build


Run tests:

dotnet test

---

## License

This project is developed as part of a technical case assignment and is not licensed for commercial use.