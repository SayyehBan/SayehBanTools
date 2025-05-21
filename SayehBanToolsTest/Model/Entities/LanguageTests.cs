using SayehBanTools.Model.Entities;

namespace SayehBanToolsTest.Model.Entities;

public class LanguageTests
{
    [Fact]
    public void LanguagesCodeGetAll_Properties_SetCorrectly()
    {
        // Arrange
        var model = new Language
        {
            LanguageID = 1,
            LanguageCode = "fa",
            LanguageName = "فارسی",
            LanguageNameFarsi = "فارسی",
            TextDirection = "RTL"
        };

        // Assert
        Assert.Equal(1, model.LanguageID);
        Assert.Equal("fa", model.LanguageCode);
        Assert.Equal("فارسی", model.LanguageName);
        Assert.Equal("فارسی", model.LanguageNameFarsi);
        Assert.Equal("RTL", model.TextDirection);
    }
}
