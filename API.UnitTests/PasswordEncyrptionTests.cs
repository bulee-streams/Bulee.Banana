using Xunit;
using FluentAssertions;


namespace API.UnitTests
{
    public class PasswordEncyrptionTests
    {
        [Fact]
        public void PasswordEncyrption_Hashed_ReturnsCorrectWhenChecked()
        {
            // Arrange
            var password = "password01";
            var encrypt = new PasswordEncryption();

            // Act 
            var result = encrypt.HashReturnSalt(password);

            // Assert
            result.Item2.Should().Be(encrypt.Hash(password, result.Item1));

        }
    }
}
