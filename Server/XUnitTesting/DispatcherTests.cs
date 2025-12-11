using ApiContracts.Enums;
using Entities;

namespace XUnitTesting;

public sealed class DispatcherTests
{
    // ---------------------------
    // Happy Path
    // ---------------------------

    [Fact]
    public void Build_WithAllPropertiesSet_ReturnsCorrectValues()
    {
        var builder = new Dispatcher.Builder()
            .SetId(1)
            .SetFirstName("Alice")
            .SetLastName("Smith")
            .SetEmail("alice.smith@example.com")
            .SetPhoneNumber("+4512345678")
            .SetPassword("Password1")
            .SetPhotoUrl("https://example.com/photo.jpg")
            .SetCurrentRate(12.5);

        var dispatcher = builder.Build();

        Assert.NotNull(dispatcher);
        Assert.Equal(1, dispatcher.Id);
        Assert.Equal("Alice", dispatcher.FirstName);
        Assert.Equal("Smith", dispatcher.LastName);
        Assert.Equal("alice.smith@example.com", dispatcher.Email);
        Assert.Equal("+4512345678", dispatcher.PhoneNumber);
        Assert.Equal("Password1", dispatcher.Password);
        Assert.Equal(UserRole.Dispatcher, dispatcher.Role); // always Dispatcher
        Assert.Equal("https://example.com/photo.jpg", dispatcher.PhotoUrl);
        Assert.Equal(12.5, dispatcher.Current_Rate);
    }

    [Fact]
    public void Build_WithoutSettingAnything_UsesDefaults()
    {
        var dispatcher = new Dispatcher.Builder().Build();

        Assert.Equal(0, dispatcher.Id);
        Assert.Equal("First", dispatcher.FirstName);
        Assert.Equal("Last", dispatcher.LastName);
        Assert.Equal("first.last@gmail.com", dispatcher.Email);
        Assert.Equal("+4511119111", dispatcher.PhoneNumber);
        Assert.Equal("VXe6FQmH2*UAQu9U7&wTnD1x7ERS@w*RahW*", dispatcher.Password);
        Assert.Equal(UserRole.Dispatcher, dispatcher.Role);
        Assert.Equal("", dispatcher.PhotoUrl);
        Assert.Equal(0, dispatcher.Current_Rate);
    }

    // ---------------------------
    // Validation – Current_Rate
    // ---------------------------

    [Theory]
    [InlineData(0)]
    [InlineData(50.5)]
    [InlineData(999.99)]
    public void SetCurrentRate_Valid_Succeeds(double rate)
    {
        var dispatcher = new Dispatcher.Builder()
            .SetCurrentRate(rate)
            .Build();

        Assert.Equal(rate, dispatcher.Current_Rate);
    }

    // ---------------------------
    // Inherited User Validations
    // ---------------------------

    [Fact]
    public void Dispatcher_InheritsRoleAsDispatcher()
    {
        var dispatcher = new Dispatcher.Builder()
            .SetRole(UserRole.Driver) // ignored, Build overrides
            .Build();

        Assert.Equal(UserRole.Dispatcher, dispatcher.Role);
    }

    [Fact]
    public void Dispatcher_SetId_Valid_Succeeds()
    {
        var dispatcher = new Dispatcher.Builder().SetId(10).Build();
        Assert.Equal(10, dispatcher.Id);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Dispatcher_SetId_Negative_Throws(int invalidId)
    {
        var builder = new Dispatcher.Builder();
        var ex = Assert.Throws<ArgumentException>(() => builder.SetId(invalidId));
        Assert.Contains("Id cannot be negative", ex.Message);
    }

    [Fact]
    public void Dispatcher_SetFirstName_Valid_Succeeds()
    {
        var dispatcher = new Dispatcher.Builder().SetFirstName("Alice").Build();
        Assert.Equal("Alice", dispatcher.FirstName);
    }

    [Theory]
    [InlineData("Jo")]
    [InlineData("A")]
    public void Dispatcher_SetFirstName_TooShort_Throws(string invalid)
    {
        var builder = new Dispatcher.Builder();
        var ex = Assert.Throws<ArgumentException>(() => builder.SetFirstName(invalid));
        Assert.Contains("First name must be at least 3 characters long", ex.Message);
    }

    [Fact]
    public void Dispatcher_SetLastName_Valid_Succeeds()
    {
        var dispatcher = new Dispatcher.Builder().SetLastName("Smith").Build();
        Assert.Equal("Smith", dispatcher.LastName);
    }

    [Fact]
    public void Dispatcher_SetEmail_Valid_Succeeds()
    {
        var dispatcher = new Dispatcher.Builder().SetEmail("user@example.com").Build();
        Assert.Equal("user@example.com", dispatcher.Email);
    }

    [Fact]
    public void Dispatcher_SetPhoneNumber_Valid_Succeeds()
    {
        var dispatcher = new Dispatcher.Builder().SetPhoneNumber("+45 12 34 56 78").Build();
        Assert.Equal("+45 12 34 56 78", dispatcher.PhoneNumber);
    }

    [Fact]
    public void Dispatcher_SetPassword_Valid_Succeeds()
    {
        var dispatcher = new Dispatcher.Builder().SetPassword("ValidPass1").Build();
        Assert.Equal("ValidPass1", dispatcher.Password);
    }

    [Fact]
    public void Dispatcher_SetPhotoUrl_Succeeds()
    {
        var dispatcher = new Dispatcher.Builder().SetPhotoUrl("https://example.com/photo.jpg").Build();
        Assert.Equal("https://example.com/photo.jpg", dispatcher.PhotoUrl);
    }
}