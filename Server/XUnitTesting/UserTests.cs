using System;
using System.Text.RegularExpressions;
using Entities;
using ApiContracts.Enums;
using Xunit;

public sealed class UserTests
{
    // ---------------------------
    // Happy Path
    // ---------------------------

    [Fact]
    public void Build_WithAllPropertiesSet_ReturnsCorrectValues()
    {
        var builder = new User.Builder()
            .SetId(1)
            .SetFirstName("John")
            .SetLastName("Doe")
            .SetEmail("john.doe@example.com")
            .SetPhoneNumber("+4512345678")
            .SetPassword("Password1")
            .SetRole(UserRole.Driver)
            .SetPhotoUrl("https://example.com/photo.jpg");

        var user = builder.Build();

        Assert.NotNull(user);
        Assert.Equal(1, user.Id);
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.Equal("john.doe@example.com", user.Email);
        Assert.Equal("+4512345678", user.PhoneNumber);
        Assert.Equal("Password1", user.Password);
        Assert.Equal(UserRole.Driver, user.Role);
        Assert.Equal("https://example.com/photo.jpg", user.PhotoUrl);
    }

    [Fact]
    public void Build_WithoutSettingAnything_UsesDefaults()
    {
        var user = new User.Builder().Build();

        Assert.Equal(0, user.Id);
        Assert.Equal("First", user.FirstName);
        Assert.Equal("Last", user.LastName);
        Assert.Equal("first.last@gmail.com", user.Email);
        Assert.Equal("+4511119111", user.PhoneNumber);
        Assert.Equal("VXe6FQmH2*UAQu9U7&wTnD1x7ERS@w*RahW*", user.Password);
        Assert.Equal(UserRole.Driver, user.Role);
        Assert.Equal("", user.PhotoUrl);
    }

    // ---------------------------
    // Validation – Id
    // ---------------------------

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void SetId_Negative_ThrowsArgumentException(int invalidId)
    {
        var builder = new User.Builder();
        var ex = Assert.Throws<ArgumentException>(() => builder.SetId(invalidId));
        Assert.Contains("Id cannot be negative", ex.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void SetId_PositiveOrZero_Succeeds(int validId)
    {
        var user = new User.Builder().SetId(validId).Build();
        Assert.Equal(validId, user.Id);
    }

    // ---------------------------
    // Validation – FirstName
    // ---------------------------

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void SetFirstName_NullOrEmpty_ThrowsArgumentException(string invalid)
    {
        var builder = new User.Builder();
        var ex = Assert.Throws<ArgumentException>(() => builder.SetFirstName(invalid));
        Assert.Contains("First name cannot be null or empty", ex.Message);
    }

    [Theory]
    [InlineData("Jo")]
    [InlineData("A")]
    public void SetFirstName_TooShort_ThrowsArgumentException(string invalid)
    {
        var builder = new User.Builder();
        var ex = Assert.Throws<ArgumentException>(() => builder.SetFirstName(invalid));
        Assert.Contains("First name must be at least 3 characters long", ex.Message);
    }

    [Theory]
    [InlineData("J@hn")]
    [InlineData("123")]
    [InlineData("John!")]
    public void SetFirstName_InvalidCharacters_ThrowsArgumentException(string invalid)
    {
        var builder = new User.Builder();
        var ex = Assert.Throws<ArgumentException>(() => builder.SetFirstName(invalid));
        Assert.Contains("First name contains invalid characters", ex.Message);
    }

    [Fact]
    public void SetFirstName_Valid_Succeeds()
    {
        var user = new User.Builder().SetFirstName("John").Build();
        Assert.Equal("John", user.FirstName);
    }

    // ---------------------------
    // Validation – LastName
    // ---------------------------

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void SetLastName_NullOrEmpty_ThrowsArgumentException(string invalid)
    {
        var builder = new User.Builder();
        var ex = Assert.Throws<ArgumentException>(() => builder.SetLastName(invalid));
        Assert.Contains("First name cannot be null or empty", ex.Message);
    }

    [Theory]
    [InlineData("Do")]
    [InlineData("A")]
    public void SetLastName_TooShort_ThrowsArgumentException(string invalid)
    {
        var builder = new User.Builder();
        var ex = Assert.Throws<ArgumentException>(() => builder.SetLastName(invalid));
        Assert.Contains("First name must be at least 3 characters long", ex.Message);
    }

    [Theory]
    [InlineData("D@e")]
    [InlineData("123")]
    [InlineData("Doe!")]
    public void SetLastName_InvalidCharacters_ThrowsArgumentException(string invalid)
    {
        var builder = new User.Builder();
        var ex = Assert.Throws<ArgumentException>(() => builder.SetLastName(invalid));
        Assert.Contains("First name contains invalid characters", ex.Message);
    }

    [Fact]
    public void SetLastName_Valid_Succeeds()
    {
        var user = new User.Builder().SetLastName("Doe").Build();
        Assert.Equal("Doe", user.LastName);
    }

    // ---------------------------
    // Validation – Email
    // ---------------------------

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void SetEmail_NullOrEmpty_ThrowsArgumentException(string invalid)
    {
        var builder = new User.Builder();
        var ex = Assert.Throws<ArgumentException>(() => builder.SetEmail(invalid));
        Assert.Contains("Email cannot be null or empty", ex.Message);
    }

    [Theory]
    [InlineData("plainaddress")]
    [InlineData("missing@domain")]
    [InlineData("@missinguser.com")]
    public void SetEmail_InvalidFormat_ThrowsArgumentException(string invalid)
    {
        var builder = new User.Builder();
        var ex = Assert.Throws<ArgumentException>(() => builder.SetEmail(invalid));
        Assert.Contains("Email format is invalid", ex.Message);
    }

    [Fact]
    public void SetEmail_Valid_Succeeds()
    {
        var user = new User.Builder().SetEmail("user@example.com").Build();
        Assert.Equal("user@example.com", user.Email);
    }

    // ---------------------------
    // Validation – PhoneNumber
    // ---------------------------

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void SetPhoneNumber_NullOrEmpty_ThrowsArgumentException(string invalid)
    {
        var builder = new User.Builder();
        var ex = Assert.Throws<ArgumentException>(() => builder.SetPhoneNumber(invalid));
        Assert.Contains("Phone number cannot be null or empty", ex.Message);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("abcd")]
    [InlineData("+12 34 56 xyz")]
    public void SetPhoneNumber_InvalidFormat_ThrowsArgumentException(string invalid)
    {
        var builder = new User.Builder();
        var ex = Assert.Throws<ArgumentException>(() => builder.SetPhoneNumber(invalid));
        Assert.Contains("Phone number format is invalid", ex.Message);
    }

    [Fact]
    public void SetPhoneNumber_Valid_Succeeds()
    {
        var user = new User.Builder().SetPhoneNumber("+45 12 34 56 78").Build();
        Assert.Equal("+45 12 34 56 78", user.PhoneNumber);
    }

    // ---------------------------
    // Validation – Password
    // ---------------------------

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void SetPassword_NullOrEmpty_ThrowsArgumentException(string invalid)
    {
        var builder = new User.Builder();
        var ex = Assert.Throws<ArgumentException>(() => builder.SetPassword(invalid));
        Assert.Contains("Password cannot be null or empty", ex.Message);
    }

    [Theory]
    [InlineData("Short1")]
    [InlineData("aB1")]
    public void SetPassword_TooShort_ThrowsArgumentException(string invalid)
    {
        var builder = new User.Builder();
        var ex = Assert.Throws<ArgumentException>(() => builder.SetPassword(invalid));
        Assert.Contains("Password must be at least 8 characters long", ex.Message);
    }

    [Fact]
    public void SetPassword_Valid_Succeeds()
    {
        var user = new User.Builder().SetPassword("ValidPass1").Build();
        Assert.Equal("ValidPass1", user.Password);
    }

    // ---------------------------
    // Validation – Role
    // ---------------------------

    [Fact]
    public void SetRole_Valid_Succeeds()
    {
        var userDriver = new User.Builder().SetRole(UserRole.Driver).Build();
        var userDispatcher = new User.Builder().SetRole(UserRole.Dispatcher).Build();

        Assert.Equal(UserRole.Driver, userDriver.Role);
        Assert.Equal(UserRole.Dispatcher, userDispatcher.Role);
    }

    [Fact]
    public void SetRole_Invalid_ThrowsArgumentException()
    {
        var builder = new User.Builder();
        var ex = Assert.Throws<ArgumentException>(() => builder.SetRole((UserRole)999));
        Assert.Contains("Invalid user role specified", ex.Message);
    }

    // ---------------------------
    // Validation – PhotoUrl
    // ---------------------------

    [Fact]
    public void SetPhotoUrl_Succeeds()
    {
        var user = new User.Builder().SetPhotoUrl("https://example.com/photo.jpg").Build();
        Assert.Equal("https://example.com/photo.jpg", user.PhotoUrl);
    }
}
