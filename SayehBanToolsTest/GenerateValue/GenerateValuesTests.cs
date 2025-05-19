
using SayehBanTools.GenerateValue;

namespace SayehBanToolsTest.GenerateValue;
/// <summary>
/// این تست برای  تولید مقادیر تصادفی استفاده میشود
/// </summary>
public class GenerateValuesTests
{
    // تست تولید عدد تصادفی 8 رقمی
    [Fact]
    public void Generate_Unique_Number_ReturnsEightDigitNumber()
    {
        // توضیح: بررسی می‌کند که آیا متد Generate_Unique_Number یک رشته 8 رقمی عددی تولید می‌کند
        string result = GenerateValues.Generate_Unique_Number();
        Assert.Equal(8, result.Length);
        Assert.Matches(@"^\d{8}$", result); // فقط شامل ارقام و دقیقاً 8 کاراکتر
    }

    // تست منحصربه‌فرد بودن اعداد تولیدشده
    [Fact]
    public void Generate_Unique_Number_GeneratesDifferentNumbers()
    {
        // توضیح: بررسی می‌کند که آیا متد Generate_Unique_Number در چند اجرا، اعداد متفاوتی تولید می‌کند
        string result1 = GenerateValues.Generate_Unique_Number();
        string result2 = GenerateValues.Generate_Unique_Number();
        Assert.NotEqual(result1, result2); // احتمال برابر بودن بسیار کم است
    }

    // تست تولید رشته تصادفی 16 کاراکتری
    [Fact]
    public void Generate16ValueRandome_ReturnsSixteenCharacterString()
    {
        // توضیح: بررسی می‌کند که آیا متد Generate16ValueRandome یک رشته 16 کاراکتری شامل حروف و اعداد تولید می‌کند
        string result = GenerateValues.Generate16ValueRandome();
        Assert.Equal(16, result.Length);
        Assert.Matches(@"^[a-z0-9]{16}$", result); // فقط شامل حروف کوچک a-z و اعداد 0-9
    }

    // تست وجود حروف و اعداد در رشته تصادفی
    [Fact]
    public void Generate16ValueRandome_ContainsLettersAndNumbers()
    {
        // توضیح: بررسی می‌کند که آیا رشته تولیدشده توسط Generate16ValueRandome حداقل یک حرف و یک عدد دارد
        string result = GenerateValues.Generate16ValueRandome();
        Assert.True(result.Any(char.IsLetter), "رشته باید حداقل یک حرف داشته باشد");
        Assert.True(result.Any(char.IsDigit), "رشته باید حداقل یک عدد داشته باشد");
    }

    // تست تصادفی بودن رشته‌های تولیدشده
    [Fact]
    public void Generate16ValueRandome_GeneratesDifferentStrings()
    {
        // توضیح: بررسی می‌کند که آیا متد Generate16ValueRandome در چند اجرا، رشته‌های متفاوتی تولید می‌کند
        string result1 = GenerateValues.Generate16ValueRandome();
        string result2 = GenerateValues.Generate16ValueRandome();
        Assert.NotEqual(result1, result2); // احتمال برابر بودن بسیار کم است
    }
}