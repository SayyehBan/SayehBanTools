using SayehBanTools.Validations;

namespace SayehBanToolsTest.Validations;

public class HierarchyValidatorTests
{
    [Fact]
    public void IsValidHierarchyPath_EmptyString_ReturnsTrue()
    {
        // Arrange
        string path = "";

        // Act
        bool result = HierarchyValidator.IsValidHierarchyPath(path);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidHierarchyPath_NullString_ReturnsTrue()
    {
        // Arrange
        string? path = null;

        // Act
        bool result = HierarchyValidator.IsValidHierarchyPath(path??"");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidHierarchyPath_ValidRootPath_ReturnsTrue()
    {
        // Arrange
        string path = "/";

        // Act
        bool result = HierarchyValidator.IsValidHierarchyPath(path);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidHierarchyPath_ValidNumericPath_ReturnsTrue()
    {
        // Arrange
        string path = "/1/";

        // Act
        bool result = HierarchyValidator.IsValidHierarchyPath(path);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidHierarchyPath_MultipleNumericParts_ReturnsTrue()
    {
        // Arrange
        string path = "/1/2/3/";

        // Act
        bool result = HierarchyValidator.IsValidHierarchyPath(path);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidHierarchyPath_MissingStartSlash_ReturnsFalse()
    {
        // Arrange
        string path = "1/";

        // Act
        bool result = HierarchyValidator.IsValidHierarchyPath(path);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidHierarchyPath_MissingEndSlash_ReturnsFalse()
    {
        // Arrange
        string path = "/1";

        // Act
        bool result = HierarchyValidator.IsValidHierarchyPath(path);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidHierarchyPath_NonNumericPart_ReturnsFalse()
    {
        // Arrange
        string path = "/1/a/";

        // Act
        bool result = HierarchyValidator.IsValidHierarchyPath(path);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidHierarchyPath_NegativeNumber_ReturnsTrue()
    {
        // Arrange
        string path = "/-1/";

        // Act
        bool result = HierarchyValidator.IsValidHierarchyPath(path);

        // Assert
        Assert.True(result);
    }
}