using Entities;

namespace XUnitTesting;

public sealed class CompanyTests
{
    // ---------------------------
    // Happy Path
    // ---------------------------

    [Fact]
    public void Build_WithAllPropertiesSet_ReturnsCorrectValues()
    {
        const string mcNumber = "ABCDEFGHIJ";
        const string companyName = "Acme Industries";

        var builder = new Company.Builder()
            .SetMcNumber(mcNumber)
            .SetCompanyName(companyName);

        Company company = builder.Build();

        Assert.NotNull(company);
        Assert.Equal(mcNumber, company.McNumber);
        Assert.Equal(companyName, company.CompanyName);
    }

    [Fact]
    public void Build_WithoutSettingAnything_UsesDefaultValues()
    {
        Company company = new Company.Builder().Build();

        Assert.NotNull(company);
        Assert.Equal("DEFAULTVAL", company.McNumber);
        Assert.Equal("Default Name", company.CompanyName);
    }

    [Fact]
    public void Builder_IsFluent_AndAllowsMethodChaining()
    {
        var builder = new Company.Builder();
        const string mcNumber = "1234567890";
        const string companyName = "Chain Corp";

        var returnedBuilder = builder
            .SetMcNumber(mcNumber)
            .SetCompanyName(companyName);

        Company company = returnedBuilder.Build();

        Assert.Same(builder, returnedBuilder);
        Assert.Equal(mcNumber, company.McNumber);
        Assert.Equal(companyName, company.CompanyName);
    }

    [Fact]
    public void Build_IsIdempotent_WhenBuilderIsReused()
    {
        var builder = new Company.Builder();
        Company first = builder.Build(); // defaults

        Company second = builder
            .SetMcNumber("1111111111")
            .SetCompanyName("New Name")
            .Build();

        Assert.Equal("DEFAULTVAL", first.McNumber);
        Assert.Equal("Default Name", first.CompanyName);

        Assert.Equal("1111111111", second.McNumber);
        Assert.Equal("New Name", second.CompanyName);
    }

    // ---------------------------
    // Validation – McNumber
    // ---------------------------

    //----------------------------
    // Unhappy path
    //----------------------------

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void SetMcNumber_WithNullOrEmpty_ThrowsArgumentException(string invalidMcNumber)
    {
        var builder = new Company.Builder();

        var ex = Assert.Throws<ArgumentException>(() => builder.SetMcNumber(invalidMcNumber));
        Assert.Contains("McNumber cannot be null or empty", ex.Message);
    }

    [Theory]
    [InlineData("123456789")]     // 9 chars
    [InlineData("12345678901")]   // 11 chars
    public void SetMcNumber_WithWrongLength_ThrowsArgumentException(string invalidMcNumber)
    {
        var builder = new Company.Builder();

        var ex = Assert.Throws<ArgumentException>(() => builder.SetMcNumber(invalidMcNumber));
        Assert.Contains("McNumber must be 10 characters long", ex.Message);
    }

    [Fact]
    public void SetMcNumber_WithExactly10Chars_Succeeds()
    {
        var builder = new Company.Builder();
        const string valid = "ABCDEFGHIJ";

        var resultBuilder = builder.SetMcNumber(valid);

        Assert.Same(builder, resultBuilder);
        Assert.Equal(valid, resultBuilder.Build().McNumber);
    }

    // ---------------------------
    // Validation – CompanyName
    // ---------------------------

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void SetCompanyName_WithNullOrEmpty_ThrowsArgumentException(string invalidName)
    {
        var builder = new Company.Builder();

        var ex = Assert.Throws<ArgumentException>(() => builder.SetCompanyName(invalidName));
        Assert.Contains("CompanyName cannot be null or empty", ex.Message);
    }

    [Fact]
    public void SetCompanyName_WithNonEmptyString_Succeeds()
    {
        var builder = new Company.Builder();
        const string name = "Some Corp";

        var resultBuilder = builder.SetCompanyName(name);

        Assert.Same(builder, resultBuilder);
        Assert.Equal(name, resultBuilder.Build().CompanyName);
    }

    // ---------------------------
    // Immutability
    // ---------------------------

    [Fact]
    public void Company_IsImmutable_AfterBuild()
    {
        var builder = new Company.Builder()
            .SetMcNumber("1234567890")
            .SetCompanyName("Immutable Inc");

        var company = builder.Build();

        var mcNumberProp = typeof(Company).GetProperty(nameof(Company.McNumber));
        var companyNameProp = typeof(Company).GetProperty(nameof(Company.CompanyName));

        Assert.NotNull(mcNumberProp);
        Assert.NotNull(companyNameProp);

        Assert.False(mcNumberProp!.SetMethod!.IsPublic);
        Assert.False(companyNameProp!.SetMethod!.IsPublic);
    }

}