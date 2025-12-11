using Entities;
using Xunit;

namespace XUnitTesting
{
    public class RecruitDriverTests
    {
        [Fact]
        public void RecruitDriver_CanSetAndGetProperties()
        {
            // Arrange
            var driver = new Driver { Id = 1, FirstName = "John Doe" };
            var dispatcher = new Dispatcher { Id = 2, FirstName = "Jane Smith" };

            // Act
            var recruit = new RecruitDriver
            {
                Driver = driver,
                Dispatcher = dispatcher
            };

            // Assert
            Assert.Equal(driver, recruit.Driver);
            Assert.Equal(dispatcher, recruit.Dispatcher);
            Assert.Equal("John Doe", recruit.Driver.FirstName);
            Assert.Equal("Jane Smith", recruit.Dispatcher.FirstName);
        }

        [Fact]
        public void RecruitDriver_DefaultPropertiesAreNull()
        {
            var recruit = new RecruitDriver();
            Assert.Null(recruit.Driver);
            Assert.Null(recruit.Dispatcher);
        }
    }
}