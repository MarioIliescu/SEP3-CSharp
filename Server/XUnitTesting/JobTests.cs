using System;
using ApiContracts.Enums;
using Entities;
using Xunit;

namespace XUnitTesting;

public sealed class JobTests
{
    // ---------------------------
    // Happy Path Tests
    // ---------------------------

    [Fact]
    public void Build_WithAllPropertiesSet_ReturnsCorrectValues()
    {
        var pickup = DateTime.Now.AddHours(1);
        var delivery = pickup.AddHours(5);

        var job = new Job.Builder()
            .SetId(1)
            .SetDispatcherId(2)
            .SetDriverId(3)
            .SetTitle("Test Job")
            .SetDescription("This is a test job")
            .SetLoadedMiles(200)
            .SetWeight(1500)
            .SetTrailerType(TrailerType.Reefer)
            .SetTotalPrice(5000)
            .SetCargoInfo("Test cargo")
            .SetPickupTime(pickup)
            .SetDeliveryTime(delivery)
            .SetPickupState("NY")
            .SetPickupZip(10001)
            .SetDropState("CA")
            .SetDropZip(90210)
            .SetStatus(JobStatus.Assigned)
            .Build();

        Assert.Equal(1, job.JobId);
        Assert.Equal(2, job.DispatcherId);
        Assert.Equal(3, job.DriverId);
        Assert.Equal("Test Job", job.Title);
        Assert.Equal("This is a test job", job.Description);
        Assert.Equal(200, job.Loaded_Miles);
        Assert.Equal(1500, job.Weight_Of_Cargo);
        Assert.Equal(TrailerType.Reefer, job.Type_Of_Trailer_Needed);
        Assert.Equal(5000, job.Total_Price);
        Assert.Equal("Test cargo", job.Cargo_Info);
        Assert.Equal(pickup, job.Pickup_Time);
        Assert.Equal(delivery, job.Delivery_Time);
        Assert.Equal("NY", job.Pickup_Location_State);
        Assert.Equal(10001, job.Pickup_Location_Zip);
        Assert.Equal("CA", job.Drop_Location_State);
        Assert.Equal(90210, job.Drop_Location_Zip);
        Assert.Equal(JobStatus.Assigned, job.Current_Status);
    }

    [Fact]
    public void Build_WithoutSettingAnything_UsesDefaults()
    {
        var job = new Job.Builder().SetDispatcherId(1).Build();

        Assert.Equal(0, job.JobId);
        Assert.Equal(1, job.DispatcherId);
        Assert.Equal(0, job.DriverId);
        Assert.Equal("Title", job.Title);
        Assert.Equal("Description", job.Description);
        Assert.Equal(100, job.Loaded_Miles);
        Assert.Equal(1000, job.Weight_Of_Cargo);
        Assert.Equal(TrailerType.Dry_van, job.Type_Of_Trailer_Needed);
        Assert.Equal(1000, job.Total_Price);
        Assert.Equal("Cargo Info", job.Cargo_Info);
        Assert.True(
            Math.Abs((job.Delivery_Time - job.Pickup_Time).TotalHours - 5) <
            0.1);
        Assert.Equal("AL", job.Pickup_Location_State);
        Assert.Equal(35010, job.Pickup_Location_Zip);
        Assert.Equal("AL", job.Drop_Location_State);
        Assert.Equal(35049, job.Drop_Location_Zip);
        Assert.Equal(JobStatus.Available, job.Current_Status);
    }

    // ---------------------------
    // Validation Tests
    // ---------------------------

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void SetDispatcherId_Invalid_Throws(int invalid)
    {
        var builder = new Job.Builder();
        var ex =
            Assert.Throws<ArgumentException>(() =>
                builder.SetDispatcherId(invalid));
        Assert.Contains("Dispatcher ID must be positive", ex.Message);
    }

    [Fact]
    public void SetDispatcherId_Valid_Succeeds() =>
        Assert.Equal(10,
            new Job.Builder().SetDispatcherId(10).Build().DispatcherId);

    [Fact]
    public void SetDriverId_Negative_Throws() =>
        Assert.Throws<ArgumentException>(() =>
            new Job.Builder().SetDriverId(-1));

    [Fact]
    public void SetDriverId_Valid_Succeeds() =>
        Assert.Equal(5, new Job.Builder().SetDriverId(5).Build().DriverId);

    [Fact]
    public void SetTitle_NullOrEmpty_Throws() =>
        Assert.Throws<ArgumentException>(() => new Job.Builder().SetTitle(""));

    [Fact]
    public void SetTitle_TooLong_Throws() =>
        Assert.Throws<ArgumentException>(() =>
            new Job.Builder().SetTitle(new string('A', 21)));

    [Fact]
    public void SetTitle_Valid_Succeeds() =>
        Assert.Equal("Short Title",
            new Job.Builder().SetTitle("Short Title").Build().Title);

    [Fact]
    public void SetDescription_TooLong_Throws() =>
        Assert.Throws<ArgumentException>(() =>
            new Job.Builder().SetDescription(new string('A', 301)));

    [Fact]
    public void SetDescription_Valid_Succeeds() =>
        Assert.Equal("Valid description",
            new Job.Builder().SetDescription("Valid description").Build()
                .Description);

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void SetLoadedMiles_Invalid_Throws(int invalid) =>
        Assert.Throws<ArgumentException>(() =>
            new Job.Builder().SetLoadedMiles(invalid));

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void SetWeight_Invalid_Throws(int invalid) =>
        Assert.Throws<ArgumentException>(() =>
            new Job.Builder().SetWeight(invalid));

    [Fact]
    public void SetLoadedMiles_Valid_Succeeds() =>
        Assert.Equal(200,
            new Job.Builder().SetLoadedMiles(200).Build().Loaded_Miles);

    [Fact]
    public void SetWeight_Valid_Succeeds() =>
        Assert.Equal(2000,
            new Job.Builder().SetWeight(2000).Build().Weight_Of_Cargo);

    [Fact]
    public void SetTotalPrice_Invalid_Throws() =>
        Assert.Throws<ArgumentException>(() =>
            new Job.Builder().SetTotalPrice(0));

    [Fact]
    public void SetTotalPrice_Valid_Succeeds() =>
        Assert.Equal(5000,
            new Job.Builder().SetTotalPrice(5000).Build().Total_Price);

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("A")]
    public void SetPickupState_Invalid_Throws(string invalid) =>
        Assert.Throws<ArgumentException>(() =>
            new Job.Builder().SetPickupState(invalid));

    [Theory]
    [InlineData(0)]
    [InlineData(100000)]
    public void SetPickupZip_Invalid_Throws(int invalid) =>
        Assert.Throws<ArgumentException>(() =>
            new Job.Builder().SetPickupZip(invalid));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("A")]
    public void SetDropState_Invalid_Throws(string invalid) =>
        Assert.Throws<ArgumentException>(() =>
            new Job.Builder().SetDropState(invalid));

    [Theory]
    [InlineData(0)]
    [InlineData(100000)]
    public void SetDropZip_Invalid_Throws(int invalid) =>
        Assert.Throws<ArgumentException>(() =>
            new Job.Builder().SetDropZip(invalid));

    [Fact]
    public void SetDeliveryTime_BeforePickup_Throws()
    {
        var pickup = DateTime.Now;
        var builder = new Job.Builder().SetPickupTime(pickup);
        Assert.Throws<ArgumentException>(() =>
            builder.SetDeliveryTime(pickup.AddMinutes(-1)));
    }

    [Fact]
    public void SetDeliveryTime_AfterPickup_Succeeds()
    {
        var pickup = DateTime.Now;
        var delivery = pickup.AddHours(2);
        var job = new Job.Builder().SetPickupTime(pickup)
            .SetDeliveryTime(delivery).Build();
        Assert.Equal(delivery, job.Delivery_Time);
    }

    // ---------------------------
    // Status Expiration Tests
    // ---------------------------

    [Fact]
    public void Job_Expires_WhenPickupTimeInPast_AndStatusAvailable() =>
        Assert.Equal(JobStatus.Expired, new Job.Builder()
            .SetDispatcherId(1)
            .SetPickupTime(DateTime.Now.AddHours(-1))
            .Build().Current_Status);

    [Fact]
    public void Job_Expires_WhenPickupTimeInPast_AndStatusAssigned() =>
        Assert.Equal(JobStatus.Expired, new Job.Builder()
            .SetDispatcherId(1)
            .SetStatus(JobStatus.Assigned)
            .SetPickupTime(DateTime.Now.AddHours(-1))
            .Build().Current_Status);

    [Fact]
    public void Job_DoesNotExpire_WhenPickupTimeInFuture()
    {
        var futurePickup = DateTime.Now.AddDays(1);
        var job = new Job.Builder()
            .SetDispatcherId(1)
            .SetPickupTime(futurePickup)
            .Build();

        Assert.Equal(JobStatus.Available, job.Current_Status);
    }

    [Fact]
    public void Job_Expires_WhenDeliveryTimeInPast() =>
        Assert.Equal(JobStatus.Expired, new Job.Builder()
            .SetDispatcherId(1)
            .SetPickupTime(DateTime.Now.AddHours(-5))
            .SetDeliveryTime(DateTime.Now.AddHours(-1))
            .Build().Current_Status);
    [Fact]
    public void Job_StatusRemains_WhenDeliveryTimeInFuture()
    {
        var pickup = DateTime.Now.AddDays(1); // future pickup
        var delivery = pickup.AddHours(5); // delivery after pickup
        var job = new Job.Builder()
            .SetDispatcherId(1)
            .SetPickupTime(pickup)
            .SetDeliveryTime(delivery)
            .Build();
        Assert.Equal(JobStatus.Available, job.Current_Status);
    }
}
