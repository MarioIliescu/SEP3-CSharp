using ApiContracts.Enums;
using Entities;

namespace XUnitTesting;

public sealed class DriverTests
{
    // ---------------------------
    // Happy Path
    // ---------------------------

    [Fact]
    public void Build_WithAllPropertiesSet_ReturnsCorrectValues()
    {
        var builder = new Driver.Builder()
            .SetId(1)
            .SetFirstName("John")
            .SetLastName("Doe")
            .SetEmail("john.doe@example.com")
            .SetPhoneNumber("+4512345678")
            .SetPassword("Password1")
            .SetPhotoUrl("https://example.com/photo.jpg")
            .SetMcNumber("ABCDEFGHIJ")
            .SetStatus(DriverStatus.Busy)
            .SetTrailerType(TrailerType.Reefer)
            .SetLocationState("CA")
            .SetLocationZip(90210)
            .SetCompanyRole(DriverCompanyRole.OwnerOperator);

        var driver = builder.Build();

        Assert.NotNull(driver);
        Assert.Equal(1, driver.Id);
        Assert.Equal("John", driver.FirstName);
        Assert.Equal("Doe", driver.LastName);
        Assert.Equal("john.doe@example.com", driver.Email);
        Assert.Equal("+4512345678", driver.PhoneNumber);
        Assert.Equal("Password1", driver.Password);
        Assert.Equal("https://example.com/photo.jpg", driver.PhotoUrl);
        Assert.Equal(UserRole.Driver, driver.Role); // inherited default
        Assert.Equal("ABCDEFGHIJ", driver.McNumber);
        Assert.Equal(DriverStatus.Busy, driver.Status);
        Assert.Equal(TrailerType.Reefer, driver.Trailer_type);
        Assert.Equal("CA", driver.Location_State);
        Assert.Equal(90210, driver.Location_Zip_Code);
        Assert.Equal(DriverCompanyRole.OwnerOperator, driver.CompanyRole);
    }

    [Fact]
    public void Build_WithoutSettingAnything_UsesDefaults()
    {
        var driver = new Driver.Builder().Build();

        Assert.Equal(0, driver.Id);
        Assert.Equal("First", driver.FirstName);
        Assert.Equal("Last", driver.LastName);
        Assert.Equal("first.last@gmail.com", driver.Email);
        Assert.Equal("+4511119111", driver.PhoneNumber);
        Assert.Equal("VXe6FQmH2*UAQu9U7&wTnD1x7ERS@w*RahW*", driver.Password);
        Assert.Equal("", driver.PhotoUrl);
        Assert.Equal(UserRole.Driver, driver.Role);
        Assert.Equal("Stuffstuff", driver.McNumber);
        Assert.Equal(DriverStatus.Available, driver.Status);
        Assert.Equal(TrailerType.Dry_van, driver.Trailer_type);
        Assert.Equal("AL", driver.Location_State);
        Assert.Equal(35010, driver.Location_Zip_Code);
        Assert.Equal(DriverCompanyRole.Driver, driver.CompanyRole);
    }

    // ---------------------------
    // Validation – Location Zip
    // ---------------------------

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(100000)]
    public void SetLocationZip_Invalid_Throws(int invalidZip)
    {
        var builder = new Driver.Builder();
        var ex = Assert.Throws<ArgumentException>(() => builder.SetLocationZip(invalidZip));
        Assert.Contains("ZIP code must be between 1 and 99999", ex.Message);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(99999)]
    [InlineData(90210)]
    public void SetLocationZip_Valid_Succeeds(int validZip)
    {
        var driver = new Driver.Builder().SetLocationZip(validZip).Build();
        Assert.Equal(validZip, driver.Location_Zip_Code);
    }

    // ---------------------------
    // Validation – Location State
    // ---------------------------

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void SetLocationState_NullOrEmpty_Throws(string invalid)
    {
        var builder = new Driver.Builder();
        var ex = Assert.Throws<ArgumentException>(() => builder.SetLocationState(invalid));
        Assert.Contains("State cannot be empty", ex.Message);
    }

    [Fact]
    public void SetLocationState_Valid_Succeeds()
    {
        var driver = new Driver.Builder().SetLocationState("ny").Build();
        Assert.Equal("NY", driver.Location_State);
    }

    // ---------------------------
    // Validation – McNumber
    // ---------------------------

    [Fact]
    public void SetMcNumber_Succeeds()
    {
        var driver = new Driver.Builder().SetMcNumber("ABCDEFGHIJ").Build();
        Assert.Equal("ABCDEFGHIJ", driver.McNumber);
    }

    // ---------------------------
    // Validation – Status
    // ---------------------------

    [Theory]
    [InlineData(DriverStatus.Available)]
    [InlineData(DriverStatus.Busy)]
    [InlineData(DriverStatus.Off_duty)]
    public void SetStatus_Succeeds(DriverStatus status)
    {
        var driver = new Driver.Builder().SetStatus(status).Build();
        Assert.Equal(status, driver.Status);
    }

    // ---------------------------
    // Validation – TrailerType
    // ---------------------------

    [Theory]
    [InlineData(TrailerType.Dry_van)]
    [InlineData(TrailerType.Flatbed)]
    [InlineData(TrailerType.Reefer)]
    public void SetTrailerType_Succeeds(TrailerType type)
    {
        var driver = new Driver.Builder().SetTrailerType(type).Build();
        Assert.Equal(type, driver.Trailer_type);
    }

    // ---------------------------
    // Validation – CompanyRole
    // ---------------------------

    [Theory]
    [InlineData(DriverCompanyRole.Driver)]
    [InlineData(DriverCompanyRole.OwnerOperator)]
    public void SetCompanyRole_Succeeds(DriverCompanyRole role)
    {
        var driver = new Driver.Builder().SetCompanyRole(role).Build();
        Assert.Equal(role, driver.CompanyRole);
    }

    // ---------------------------
    // Inherited User properties
    // ---------------------------

    [Fact]
    public void Driver_InheritsUserDefaults()
    {
        var driver = new Driver.Builder().Build();

        Assert.Equal(0, driver.Id);
        Assert.Equal("First", driver.FirstName);
        Assert.Equal("Last", driver.LastName);
        Assert.Equal("first.last@gmail.com", driver.Email);
        Assert.Equal("+4511119111", driver.PhoneNumber);
        Assert.Equal("VXe6FQmH2*UAQu9U7&wTnD1x7ERS@w*RahW*", driver.Password);
        Assert.Equal("", driver.PhotoUrl);
        Assert.Equal(UserRole.Driver, driver.Role);
    }
}