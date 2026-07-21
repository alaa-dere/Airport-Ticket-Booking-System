using System.ComponentModel.DataAnnotations;
using Moq;
using TASK2.Models;
using TASK2.Services.Bookings;
using TASK2.Services.Flights;
using TASK2.Services.Passengers;

namespace Task2.Tests;

public class PassengerServiceShould
{
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
    public void SearchFlights_ReturnsFlightsFromFlightService()
    {
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

        var result = _passengerService.SearchFlights(filter);

        Assert.Equal(expectedFlights, result);
    }

    [Fact]
    public void Book_ThrowsValidationException_WhenPassengerDataIsEmpty()
    {
        var request = new BookingRequest
        {
            FlightId = 1,
            PassengerEmail = "",
            PassengerName = "",
            PassengerPhone = "",
            SelectedClass = FlightClass.Economy
        };

        var exception = Assert.Throws<ValidationException>(() =>
            _passengerService.Book(request));

        Assert.Contains("required", exception.Message);
    }

    [Fact]
    public void Book_ThrowsKeyNotFoundException_WhenFlightDoesNotExist()
    {
        var request = CreateBookingRequest();

        _mockFlightService
            .Setup(service => service.GetAll())
            .Returns(new List<Flight>());

        var exception = Assert.Throws<KeyNotFoundException>(() =>
            _passengerService.Book(request));

        Assert.Equal("Flight not found.", exception.Message);
    }

    [Fact]
    public void Book_ThrowsInvalidOperationException_WhenPassengerAlreadyBookedFlight()
    {
        var request = CreateBookingRequest();
        var existingBooking = CreateBooking(1, 1, "passenger@gmail.com");

        _mockFlightService
            .Setup(service => service.GetAll())
            .Returns(new List<Flight> { CreateFlight(1) });
        _mockBookingService
            .Setup(service => service.GetAll())
            .Returns(new List<Booking> { existingBooking });

        var exception = Assert.Throws<InvalidOperationException>(() =>
            _passengerService.Book(request));

        Assert.Equal("Passenger already booked this flight.", exception.Message);
    }

    [Fact]
    public void Book_ReturnsBooking_WhenRequestIsValid()
    {
        var request = CreateBookingRequest();
        var expectedBooking = CreateBooking(1, 1, "passenger@gmail.com");

        _mockFlightService
            .Setup(service => service.GetAll())
            .Returns(new List<Flight> { CreateFlight(1) });
        _mockBookingService
            .Setup(service => service.GetAll())
            .Returns(new List<Booking>());
        _mockBookingService
            .Setup(service => service.Add(It.IsAny<Booking>()))
            .Returns(expectedBooking);

        var result = _passengerService.Book(request);

        Assert.Equal(expectedBooking, result);
        _mockBookingService.Verify(service => service.Add(It.IsAny<Booking>()), Times.Once);
    }

    [Fact]
    public void Cancel_DeletesBooking()
    {
        _passengerService.Cancel(1, "passenger@example.com");

        _mockBookingService.Verify(service => service.Delete(1), Times.Once);
    }

    [Fact]
    public void Modify_ReturnsFalse_WhenBookingDoesNotExist()
    {
        var request = CreateModifyRequest();

        _mockBookingService
            .Setup(service => service.GetAll())
            .Returns(new List<Booking>());

        var result = _passengerService.Modify(request);

        Assert.False(result);
    }

    [Fact]
    public void Modify_ReturnsFalse_WhenNewFlightDoesNotExist()
    {
        var request = CreateModifyRequest();

        _mockBookingService
            .Setup(service => service.GetAll())
            .Returns(new List<Booking>
            {
                CreateBooking(1, 1, "passenger@gmail.com")
            });
        _mockFlightService
            .Setup(service => service.GetAll())
            .Returns(new List<Flight>());

        var result = _passengerService.Modify(request);

        Assert.False(result);
    }

    [Fact]
    public void Modify_ReturnsFalse_WhenPassengerAlreadyBookedNewFlight()
    {
        var request = CreateModifyRequest();
        var currentBooking = CreateBooking(1, 1, "passenger@gmail.com");
        var duplicateBooking = CreateBooking(2, 2, "passenger@gmail.com");

        _mockBookingService
            .Setup(service => service.GetAll())
            .Returns(new List<Booking> { currentBooking, duplicateBooking });
        _mockFlightService
            .Setup(service => service.GetAll())
            .Returns(new List<Flight> { CreateFlight(2) });

        var result = _passengerService.Modify(request);

        Assert.False(result);
    }

    [Fact]
    public void Modify_ReturnsTrue_WhenRequestIsValid()
    {
        var request = CreateModifyRequest();
        var currentBooking = CreateBooking(1, 1, "passenger@gmail.com");

        _mockBookingService
            .Setup(service => service.GetAll())
            .Returns(new List<Booking> { currentBooking });
        _mockFlightService
            .Setup(service => service.GetAll())
            .Returns(new List<Flight> { CreateFlight(2) });

        var result = _passengerService.Modify(request);

        Assert.True(result);
        Assert.Equal(2, currentBooking.FlightId);
        Assert.Equal(FlightClass.Business, currentBooking.SelectedClass);
        Assert.Equal(150, currentBooking.PricePaid);
    }

    [Fact]
    public void GetMyBookings_ReturnsOnlyPassengerBookings()
    {
        var passengerBooking = CreateBooking(1, 1, "passenger@gmail.com");
        var anotherBooking = CreateBooking(2, 2, "another@gmail.com");

        _mockBookingService
            .Setup(service => service.GetAll())
            .Returns(new List<Booking> { passengerBooking, anotherBooking });

        var result = _passengerService.GetMyBookings("PASSENGER@gmail.com");

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
            DepartureTime = DateTime.Today.AddDays(1),
            BasePrice = 100
        };
    }

    private static BookingRequest CreateBookingRequest()
    {
        return new BookingRequest
        {
            FlightId = 1,
            PassengerEmail = "passenger@gmail.com",
            PassengerName = "Test Passenger",
            PassengerPhone = "05997659598",
            SelectedClass = FlightClass.Economy
        };
    }

    private static ModifyBookingRequest CreateModifyRequest()
    {
        return new ModifyBookingRequest
        {
            BookingId = 1,
            PassengerEmail = "passenger@gmail.com",
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
