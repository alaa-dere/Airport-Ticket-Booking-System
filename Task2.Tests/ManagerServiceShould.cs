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
    private static readonly DateTime TestToday = DateTime.Today;

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
        // Arrange
        var nonExistentFilePath = $"nonexistentfile.csv";

        // Act
        var result = _managerService.BatchUploadFlights(nonExistentFilePath);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Equal("The specified file does not exist.", result.Errors.First().ErrorMessage);
    }

    [Fact]
    public void BatchUploadFlights_ReturnsSuccess_WhenLineIsValid()
    {
        // Arrange
        var filePath = CreateCsvFile(ValidFlightLine());
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        // Act
        var result = _managerService.BatchUploadFlights(filePath);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);

        _mockFlightService.Verify(
            service => service.Add(It.Is<ICollection<Flight>>(flights => flights.Count == 1)),
            Times.Once);

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void BatchUploadFlights_IgnoresLines_WhenLinesAreEmpty()
    {
        // Arrange
        var filePath = CreateCsvFile("", "   ");
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        // Act
        var result = _managerService.BatchUploadFlights(filePath);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void BatchUploadFlights_ReturnsError_WhenRowFormatIsInvalid()
    {
        // Arrange
        var filePath = CreateCsvFile("invalid flight data");
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        // Act
        var result = _managerService.BatchUploadFlights(filePath);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.FieldName == "Row");

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void BatchUploadFlights_ReturnsError_WhenFlightIdIsInvalid()
    {
        // Arrange
        var line = $"abc,Palestine,Jordan,Airport A,Airport B,{FutureDate()},100";
        var filePath = CreateCsvFile(line);
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        // Act
        var result = _managerService.BatchUploadFlights(filePath);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.FieldName == "Id");

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void BatchUploadFlights_ReturnsError_WhenFlightIdAlreadyExists()
    {
        // Arrange
        var filePath = CreateCsvFile(ValidFlightLine());
        _mockFlightService
            .Setup(service => service.GetAll())
            .Returns(new List<Flight> { CreateFlight(1) });

        // Act
        var result = _managerService.BatchUploadFlights(filePath);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.ErrorMessage.Contains("already exists"));

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void BatchUploadFlights_ReturnsErrors_WhenRequiredFieldsAreEmpty()
    {
        // Arrange
        var line = $"1,,,,,{FutureDate()},100";
        var filePath = CreateCsvFile(line);
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        // Act
        var result = _managerService.BatchUploadFlights(filePath);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.FieldName == nameof(Flight.DepartureCountry));
        Assert.Contains(result.Errors, error => error.FieldName == nameof(Flight.DestinationCountry));
        Assert.Contains(result.Errors, error => error.FieldName == nameof(Flight.DepartureAirport));
        Assert.Contains(result.Errors, error => error.FieldName == nameof(Flight.ArrivalAirport));

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void BatchUploadFlights_ReturnsError_WhenDepartureTimeFormatIsInvalid()
    {
        // Arrange
        var line = "1,Palestine,Jordan,Airport A,Airport B,wrong-date,100";
        var filePath = CreateCsvFile(line);
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        // Act
        var result = _managerService.BatchUploadFlights(filePath);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.FieldName == "DepartureTime");

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void BatchUploadFlights_ReturnsError_WhenDepartureTimeIsInPast()
    {
        // Arrange
        var pastDate = TestToday.AddDays(-1).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        var line = $"1,Palestine,Jordan,Airport A,Airport B,{pastDate},100";
        var filePath = CreateCsvFile(line);
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        // Act
        var result = _managerService.BatchUploadFlights(filePath);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.FieldName == nameof(Flight.DepartureTime));

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void BatchUploadFlights_ReturnsError_WhenBasePriceIsInvalid()
    {
        // Arrange
        var line = $"1,Palestine,Jordan,Airport A,Airport B,{FutureDate()},abc";
        var filePath = CreateCsvFile(line);
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        // Act
        var result = _managerService.BatchUploadFlights(filePath);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.FieldName == "BasePrice");

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void BatchUploadFlights_ReturnsError_WhenBasePriceIsZero()
    {
        // Arrange
        var line = $"1,Palestine,Jordan,Airport A,Airport B,{FutureDate()},0";
        var filePath = CreateCsvFile(line);
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        // Act
        var result = _managerService.BatchUploadFlights(filePath);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, error => error.FieldName == nameof(Flight.BasePrice));

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void BatchUploadFlights_ReturnsError_WhenFlightIdIsDuplicatedInFile()
    {
        // Arrange
        var filePath = CreateCsvFile(ValidFlightLine(), ValidFlightLine());
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        // Act
        var result = _managerService.BatchUploadFlights(filePath);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Contains("duplicated", result.Errors.First().ErrorMessage);

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void ValidateImportedFlightData_ReturnsError_WhenFileDoesNotExist()
    {
        // Arrange
        var nonExistentFilePath = "nonexistentfile.csv";

        // Act
        var errors = _managerService.ValidateImportedFlightData(nonExistentFilePath);

        // Assert
        Assert.Single(errors);
        Assert.Equal("The specified file does not exist.", errors.First().ErrorMessage);
    }

    [Fact]
    public void ValidateImportedFlightData_ReturnsNoErrors_WhenLineIsValid()
    {
        // Arrange
        var filePath = CreateCsvFile(ValidFlightLine());
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        // Act
        var errors = _managerService.ValidateImportedFlightData(filePath);

        // Assert   
        Assert.Empty(errors);

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void ValidateImportedFlightData_ReturnsError_WhenLineIsInvalid()
    {
        // Arrange
        var line = $"1,Palestine,Jordan,Airport A,Airport B,{FutureDate()},invalid-price";
        var filePath = CreateCsvFile(line);
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        // Act
        var errors = _managerService.ValidateImportedFlightData(filePath);

        // Assert
        Assert.Contains(errors, error => error.FieldName == "BasePrice");

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void ValidateImportedFlightData_ReturnsError_WhenFlightIdIsDuplicated()
    {
        // Arrange
        var filePath = CreateCsvFile(ValidFlightLine(), ValidFlightLine());
        _mockFlightService.Setup(service => service.GetAll()).Returns(new List<Flight>());

        // Act
        var errors = _managerService.ValidateImportedFlightData(filePath);

        // Assert
        Assert.Single(errors);
        Assert.Contains("duplicated", errors.First().ErrorMessage);

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void GetFlightValidationDetails_ReturnsDetailsForFlightFields_WhenCalled()
    {
        // Act
        var details = _managerService.GetFlightValidationDetails();

        // Assert
        Assert.Contains(details, field => field.FieldName == nameof(Flight.Id));
        Assert.Contains(details, field => field.FieldName == nameof(Flight.DepartureCountry));
        Assert.Contains(details, field => field.FieldName == nameof(Flight.DepartureTime));
        Assert.Contains(details, field => field.FieldName == nameof(Flight.BasePrice));
    }

    [Fact]
    public void GetFlightValidationDetails_ReturnsCorrectDetailsForId_WhenCalled()
    {
        // Act
        var details = _managerService.GetFlightValidationDetails();
        var idDetails = details.First(field => field.FieldName == nameof(Flight.Id));

        // Assert
        Assert.Equal("Integer", idDetails.Type);
        Assert.Contains("Required", idDetails.Constraints);
        Assert.Contains("Unique", idDetails.Constraints);
    }

    [Fact]
    public void GetAll_ReturnsFlightsFromFlightService_WhenCalled()
    {
        // Arrange
        var expectedFlights = new List<Flight> { CreateFlight(1) };
        _mockFlightService.Setup(service => service.GetAll()).Returns(expectedFlights);

        // Act
        var actualFlights = _managerService.GetAll();

        // Assert
        Assert.Equal(expectedFlights, actualFlights);
    }

    [Fact]
    public void FilterBookings_ReturnsBookingsFromBookingService_WhenFilterIsProvided()
    {
        // Arrange
        var filter = new BookingFilter { FlightId = 1 };
        var expectedBookings = new List<Booking>();
        _mockBookingService
            .Setup(service => service.FilterBookings(filter))
            .Returns(expectedBookings);

        // Act
        var actualBookings = _managerService.FilterBookings(filter);

        // Assert
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
        return TestToday.AddDays(10).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
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
}
