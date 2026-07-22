using System.ComponentModel.DataAnnotations;
using Moq;
using TASK2.Models;
using TASK2.Services.Bookings;
using TASK2.Services.Flights;
using TASK2.Services.Passengers;

namespace Task2.Tests;

public class PassengerServiceShould
{
    private const string PassengerEmail = "passenger@gmail.com";
    private const string AnotherPassengerEmail = "another@gmail.com";
    private const string PassengerName = "Test Passenger";
    private const string PassengerPhone = "05999999999";
    private static readonly DateTime TestToday = DateTime.Today;

    private readonly Mock<IFlightService> _mockFlightService;
    private readonly Mock<IBookingService> _mockBookingService;
    private readonly PassengerService _passengerService;

    public PassengerServiceShould()
    {
        _mockFlightService = new Mock<IFlightService>();
        _mockBookingService = new Mock<IBookingService>();

        _passengerService = new PassengerService(
            _mockFlightService.Object,
            _mockBookingService.Object);
    }

    [Fact]
    public void SearchFlights_ReturnsFlightsFromFlightService_WhenFilterIsProvided()
    {
        // Arrange
        var filter = new FlightFilter
        {
            DepartureCountry = "Palestine"
        };
        var expectedFlights = new List<Flight>
        {
            CreateFlight(1)
        };

        _mockFlightService
            .Setup(service => service.SearchFlights(filter))
            .Returns(expectedFlights);

        // Act
        var result = _passengerService.SearchFlights(filter);

        // Assert
        Assert.Equal(expectedFlights, result);
    }

    [Fact]
    public void Book_ThrowsValidationException_WhenPassengerDataIsEmpty()
    {
        // Arrange
        var request = new BookingRequest
        {
            FlightId = 1,
            PassengerEmail = "",
            PassengerName = "",
            PassengerPhone = "",
            SelectedClass = FlightClass.Economy
        };

        // Act
        var exception = Assert.Throws<ValidationException>(() =>
            _passengerService.Book(request));

        // Assert
        Assert.Contains("required", exception.Message);
    }

    [Fact]
    public void Book_ThrowsKeyNotFoundException_WhenFlightDoesNotExist()
    {
        // Arrange
        var request = CreateBookingRequest();

        _mockFlightService
            .Setup(service => service.GetAll())
            .Returns(new List<Flight>());

        // Act
        var exception = Assert.Throws<KeyNotFoundException>(() =>
            _passengerService.Book(request));

        // Assert
        Assert.Equal("Flight not found.", exception.Message);
    }

    [Fact]
    public void Book_ThrowsInvalidOperationException_WhenPassengerAlreadyBookedFlight()
    {
        // Arrange
        var request = CreateBookingRequest();
        var existingBooking = CreateBooking(1, 1, PassengerEmail);

        _mockFlightService
            .Setup(service => service.GetAll())
            .Returns(new List<Flight> { CreateFlight(1) });
        _mockBookingService
            .Setup(service => service.GetAll())
            .Returns(new List<Booking> { existingBooking });

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _passengerService.Book(request));

        // Assert
        Assert.Equal("Passenger already booked this flight.", exception.Message);
    }

    [Fact]
    public void Book_ReturnsBooking_WhenRequestIsValid()
    {
        // Arrange
        var request = CreateBookingRequest();
        var expectedBooking = CreateBooking(1, 1, PassengerEmail);

        _mockFlightService
            .Setup(service => service.GetAll())
            .Returns(new List<Flight> { CreateFlight(1) });
        _mockBookingService
            .Setup(service => service.GetAll())
            .Returns(new List<Booking>());
        _mockBookingService
            .Setup(service => service.Add(It.IsAny<Booking>()))
            .Returns(expectedBooking);

        // Act
        var result = _passengerService.Book(request);

        // Assert
        Assert.Equal(expectedBooking, result);
        _mockBookingService.Verify(service => service.Add(It.IsAny<Booking>()), Times.Once);
    }

    [Fact]
    public void Cancel_DeletesBooking_WhenCalled()
    {
        // Arrange
        var bookingId = 1;
        var passengerEmail = PassengerEmail;

        // Act
        _passengerService.Cancel(bookingId, passengerEmail);

        // Assert
        _mockBookingService.Verify(service => service.Delete(bookingId), Times.Once);
    }

    [Fact]
    public void Modify_ReturnsFalse_WhenBookingDoesNotExist()
    {
        // Arrange
        var request = CreateModifyRequest();

        _mockBookingService
            .Setup(service => service.GetAll())
            .Returns(new List<Booking>());

        // Act
        var result = _passengerService.Modify(request);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Modify_ReturnsFalse_WhenNewFlightDoesNotExist()
    {
        // Arrange
        var request = CreateModifyRequest();

        _mockBookingService
            .Setup(service => service.GetAll())
            .Returns(new List<Booking>
            {
                CreateBooking(1, 1, PassengerEmail)
            });
        _mockFlightService
            .Setup(service => service.GetAll())
            .Returns(new List<Flight>());

        // Act
        var result = _passengerService.Modify(request);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Modify_ReturnsFalse_WhenPassengerAlreadyBookedNewFlight()
    {
        // Arrange
        var request = CreateModifyRequest();
        var currentBooking = CreateBooking(1, 1, PassengerEmail);
        var duplicateBooking = CreateBooking(2, 2, PassengerEmail);

        _mockBookingService
            .Setup(service => service.GetAll())
            .Returns(new List<Booking> { currentBooking, duplicateBooking });
        _mockFlightService
            .Setup(service => service.GetAll())
            .Returns(new List<Flight> { CreateFlight(2) });

        // Act
        var result = _passengerService.Modify(request);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Modify_ReturnsTrue_WhenRequestIsValid()
    {
        // Arrange
        var request = CreateModifyRequest();
        var currentBooking = CreateBooking(1, 1, PassengerEmail);

        _mockBookingService
            .Setup(service => service.GetAll())
            .Returns(new List<Booking> { currentBooking });
        _mockFlightService
            .Setup(service => service.GetAll())
            .Returns(new List<Flight> { CreateFlight(2) });

        // Act
        var result = _passengerService.Modify(request);

        // Assert
        Assert.True(result);
        Assert.Equal(2, currentBooking.FlightId);
        Assert.Equal(FlightClass.Business, currentBooking.SelectedClass);
        Assert.Equal(150, currentBooking.PricePaid);
    }

    [Fact]
    public void GetMyBookings_ReturnsOnlyPassengerBookings_WhenEmailIsProvided()
    {
        // Arrange
        var passengerBooking = CreateBooking(1, 1, PassengerEmail);
        var anotherBooking = CreateBooking(2, 2, AnotherPassengerEmail);

        _mockBookingService
            .Setup(service => service.GetAll())
            .Returns(new List<Booking> { passengerBooking, anotherBooking });

        // Act
        var result = _passengerService.GetMyBookings(PassengerEmail);

        // Assert
        Assert.Single(result);
        Assert.Equal(passengerBooking, result.First());
    }

    private static Flight CreateFlight(int id)
    {
        return new Flight
        {
            Id = id,
            DepartureCountry = "Palestine",
            DestinationCountry = "Jordan",
            DepartureAirport = "Airport A",
            ArrivalAirport = "Airport B",
            DepartureTime = TestToday.AddDays(1),
            BasePrice = 100
        };
    }

    private static BookingRequest CreateBookingRequest()
    {
        return new BookingRequest
        {
            FlightId = 1,
            PassengerEmail = PassengerEmail,
            PassengerName = PassengerName,
            PassengerPhone = PassengerPhone,
            SelectedClass = FlightClass.Economy
        };
    }

    private static ModifyBookingRequest CreateModifyRequest()
    {
        return new ModifyBookingRequest
        {
            BookingId = 1,
            PassengerEmail = PassengerEmail,
            NewFlightId = 2,
            NewClass = FlightClass.Business
        };
    }

    private static Booking CreateBooking(int id, int flightId, string email)
    {
        return new Booking
        {
            Id = id,
            FlightId = flightId,
            Passenger = new Passenger(
                new Email(email),
                "Test Passenger",
                "05997659598"),
            SelectedClass = FlightClass.Economy,
            PricePaid = 100
        };
    }
}
