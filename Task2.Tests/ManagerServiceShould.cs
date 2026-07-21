using System.Globalization;
using Moq;
using TASK2.File_Storage.Flights;
using TASK2.Models;
using TASK2.Services.Bookings;
using TASK2.Services.Flights;
using TASK2.Services.Manager;
using TASK2.Services.Validation;

namespace Task2.Tests;

public class ManagerServiceShould
{
    private readonly Mock<IFlightService> _mockFlightService;
    private readonly Mock<IBookingService> _mockBookingService;
    private readonly Mock<IFlightRepository> _mockFlightRepository;
    private readonly ManagerService _managerService;

    public ManagerServiceShould()
    {
        _mockFlightService = new Mock<IFlightService>();
        _mockBookingService = new Mock<IBookingService>();
        _mockFlightRepository = new Mock<IFlightRepository>();

        var validationService = new ValidationService(_mockFlightRepository.Object);

        _managerService = new ManagerService(
            _mockFlightService.Object,
            _mockBookingService.Object,
            validationService);
    }

    [Fact]
    public void BatchUploadFlights_ReturnsError_WhenFileDoesNotExist()
    {
        var nonExistentFilePath = $"{Guid.NewGuid()}.csv";

        var result = _managerService.BatchUploadFlights(nonExistentFilePath);

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Equal("The specified file does not exist.", result.Errors.First().ErrorMessage);
    }

    [Fact]
    public void BatchUploadFlights_ReturnsSuccess_WhenLineIsValid()
    {
        var filePath = CreateCsvFile(ValidFlightLine());
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        var result = _managerService.BatchUploadFlights(filePath);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
        _mockFlightService.Verify(
            service => service.Add(It.Is<ICollection<Flight>>(flights => flights.Count == 1)),
            Times.Once);

        File.Delete(filePath);
    }

    [Fact]
    public void BatchUploadFlights_IgnoresEmptyLines()
    {
        var filePath = CreateCsvFile("", "   ");
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        var result = _managerService.BatchUploadFlights(filePath);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);

        File.Delete(filePath);
    }

    [Fact]
    public void BatchUploadFlights_ReturnsError_WhenRowFormatIsInvalid()
    {
        var filePath = CreateCsvFile("invalid flight data");
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        var result = _managerService.BatchUploadFlights(filePath);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.FieldName == "Row");

        File.Delete(filePath);
    }

    [Fact]
    public void BatchUploadFlights_ReturnsError_WhenFlightIdIsInvalid()
    {
        var line = $"abc,Palestine,Jordan,Airport A,Airport B,{FutureDate()},100";
        var filePath = CreateCsvFile(line);
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        var result = _managerService.BatchUploadFlights(filePath);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.FieldName == "Id");

        File.Delete(filePath);
    }

    [Fact]
    public void BatchUploadFlights_ReturnsError_WhenFlightIdAlreadyExists()
    {
        var filePath = CreateCsvFile(ValidFlightLine());
        _mockFlightService
            .Setup(service => service.GetAll())
            .Returns(new List<Flight> { CreateFlight(1) });

        var result = _managerService.BatchUploadFlights(filePath);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.ErrorMessage.Contains("already exists"));

        File.Delete(filePath);
    }

    [Fact]
    public void BatchUploadFlights_ReturnsErrors_WhenRequiredFieldsAreEmpty()
    {
        var line = $"1,,,,,{FutureDate()},100";
        var filePath = CreateCsvFile(line);
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        var result = _managerService.BatchUploadFlights(filePath);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.FieldName == nameof(Flight.DepartureCountry));
        Assert.Contains(result.Errors, error => error.FieldName == nameof(Flight.DestinationCountry));
        Assert.Contains(result.Errors, error => error.FieldName == nameof(Flight.DepartureAirport));
        Assert.Contains(result.Errors, error => error.FieldName == nameof(Flight.ArrivalAirport));

        File.Delete(filePath);
    }

    [Fact]
    public void BatchUploadFlights_ReturnsError_WhenDepartureTimeFormatIsInvalid()
    {
        var line = "1,Palestine,Jordan,Airport A,Airport B,wrong-date,100";
        var filePath = CreateCsvFile(line);
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        var result = _managerService.BatchUploadFlights(filePath);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.FieldName == "DepartureTime");

        File.Delete(filePath);
    }

    [Fact]
    public void BatchUploadFlights_ReturnsError_WhenDepartureTimeIsInPast()
    {
        var pastDate = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        var line = $"1,Palestine,Jordan,Airport A,Airport B,{pastDate},100";
        var filePath = CreateCsvFile(line);
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        var result = _managerService.BatchUploadFlights(filePath);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.FieldName == nameof(Flight.DepartureTime));

        File.Delete(filePath);
    }

    [Fact]
    public void BatchUploadFlights_ReturnsError_WhenBasePriceIsInvalid()
    {
        var line = $"1,Palestine,Jordan,Airport A,Airport B,{FutureDate()},abc";
        var filePath = CreateCsvFile(line);
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        var result = _managerService.BatchUploadFlights(filePath);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.FieldName == "BasePrice");

        File.Delete(filePath);
    }

    [Fact]
    public void BatchUploadFlights_ReturnsError_WhenBasePriceIsZero()
    {
        var line = $"1,Palestine,Jordan,Airport A,Airport B,{FutureDate()},0";
        var filePath = CreateCsvFile(line);
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        var result = _managerService.BatchUploadFlights(filePath);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.FieldName == nameof(Flight.BasePrice));

        File.Delete(filePath);
    }

    [Fact]
    public void BatchUploadFlights_ReturnsError_WhenFlightIdIsDuplicatedInFile()
    {
        var filePath = CreateCsvFile(ValidFlightLine(), ValidFlightLine());
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        var result = _managerService.BatchUploadFlights(filePath);

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Contains("duplicated", result.Errors.First().ErrorMessage);

        File.Delete(filePath);
    }

    [Fact]
    public void ValidateImportedFlightData_ReturnsError_WhenFileDoesNotExist()
    {
        var nonExistentFilePath = $"{Guid.NewGuid()}.csv";

        var errors = _managerService.ValidateImportedFlightData(nonExistentFilePath);

        Assert.Single(errors);
        Assert.Equal("The specified file does not exist.", errors.First().ErrorMessage);
    }

    [Fact]
    public void ValidateImportedFlightData_ReturnsNoErrors_WhenLineIsValid()
    {
        var filePath = CreateCsvFile(ValidFlightLine());
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        var errors = _managerService.ValidateImportedFlightData(filePath);

        Assert.Empty(errors);

        File.Delete(filePath);
    }

    [Fact]
    public void ValidateImportedFlightData_ReturnsError_WhenLineIsInvalid()
    {
        var line = $"1,Palestine,Jordan,Airport A,Airport B,{FutureDate()},invalid-price";
        var filePath = CreateCsvFile(line);
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        var errors = _managerService.ValidateImportedFlightData(filePath);

        Assert.Contains(errors, error => error.FieldName == "BasePrice");

        File.Delete(filePath);
    }

    [Fact]
    public void ValidateImportedFlightData_ReturnsError_WhenFlightIdIsDuplicated()
    {
        var filePath = CreateCsvFile(ValidFlightLine(), ValidFlightLine());
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        var errors = _managerService.ValidateImportedFlightData(filePath);

        Assert.Single(errors);
        Assert.Contains("duplicated", errors.First().ErrorMessage);

        File.Delete(filePath);
    }

    [Fact]
    public void GetFlightValidationDetails_ReturnsDetailsForFlightFields()
    {
        var details = _managerService.GetFlightValidationDetails();

        Assert.Contains(details, field => field.FieldName == nameof(Flight.Id));
        Assert.Contains(details, field => field.FieldName == nameof(Flight.DepartureCountry));
        Assert.Contains(details, field => field.FieldName == nameof(Flight.DepartureTime));
        Assert.Contains(details, field => field.FieldName == nameof(Flight.BasePrice));
    }

    [Fact]
    public void GetFlightValidationDetails_ReturnsCorrectDetailsForId()
    {
        var details = _managerService.GetFlightValidationDetails();
        var idDetails = details.First(field => field.FieldName == nameof(Flight.Id));

        Assert.Equal("Integer", idDetails.Type);
        Assert.Contains("Required", idDetails.Constraints);
        Assert.Contains("Unique", idDetails.Constraints);
    }

    [Fact]
    public void GetAll_ReturnsFlightsFromFlightService()
    {
        var expectedFlights = new List<Flight> { CreateFlight(1) };
        _mockFlightService.Setup(service => service.GetAll()).Returns(expectedFlights);

        var actualFlights = _managerService.GetAll();

        Assert.Equal(expectedFlights, actualFlights);
    }

    [Fact]
    public void FilterBookings_ReturnsBookingsFromBookingService()
    {
        var filter = new BookingFilter { FlightId = 1 };
        var expectedBookings = new List<Booking>();
        _mockBookingService
            .Setup(service => service.FilterBookings(filter))
            .Returns(expectedBookings);

        var actualBookings = _managerService.FilterBookings(filter);

        Assert.Equal(expectedBookings, actualBookings);
    }

    private static string CreateCsvFile(params string[] lines)
    {
        var filePath = Path.GetTempFileName();
        File.WriteAllLines(filePath, ["Header", .. lines]);
        return filePath;
    }

    private static string ValidFlightLine()
    {
        return $"1,Palestine,Jordan,Airport A,Airport B,{FutureDate()},100";
    }

    private static string FutureDate()
    {
        return DateTime.Today.AddDays(10).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
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
}
