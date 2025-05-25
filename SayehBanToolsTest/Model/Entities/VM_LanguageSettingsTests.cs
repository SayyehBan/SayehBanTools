using SayehBanTools.API.Language;
using SayehBanTools.Model.Entities;

namespace SayehBanToolsTest.Model.Entities;

public class VM_LanguageSettingsTests
{
    [Fact]
    public void LanguageSettingsGetAll_Properties_SetCorrectly()
    {
        //Arrange
        var model = new VM_LanguageSettings.LanguageSettingsGetAll()
        {
            LanguageSettingID = 1,
            LanguageCode = "fa",
            LanguageCodeRegion = "fa-IR",
            AnalyzerName = "persian_custom",
            Tokenizer = "standard",
            Filters = "[\"lowercase\", \"persian_stop\", \"persian_normalization\"]"
        };

        //Assert
        Assert.Equal(1, model.LanguageSettingID);
        Assert.Equal("fa", model.LanguageCode);
        Assert.Equal("fa-IR", model.LanguageCodeRegion);
        Assert.Equal("persian_custom", model.AnalyzerName);
        Assert.Equal("standard", model.Tokenizer);
        Assert.Equal("[\"lowercase\", \"persian_stop\", \"persian_normalization\"]", model.Filters);
    }
}
