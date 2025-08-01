﻿using SayehBanTools.Model.Entities;

namespace SayehBanToolsTest.Model.Entities;

public class VM_LanguageTests
{
    /// <summary>
    /// ویو مدل دریافت کامل اطلاعات زبان
    /// </summary>
    [Fact]
    public void LanguagesCodeGetAll_Properties_SetCorrectly()
    {
        // Arrange
        var model = new VM_Language.LanguageCodeList
        {
            LanguageID = 1,
            LanguageCode = "fa",
            LanguageCodeRegion = "fa-IR",
            LocalizedLanguageNames = "فارسی",
            TextDirection = "RTL"
        };

        // Assert
        Assert.Equal(1, model.LanguageID);
        Assert.Equal("fa", model.LanguageCode);
        Assert.Equal("fa-IR", model.LanguageCodeRegion);
        Assert.Equal("فارسی", model.LocalizedLanguageNames);
        Assert.Equal("RTL", model.TextDirection);
    }
}
