using SayehBanTools.Model.Entities;

namespace SayehBanToolsTest.Model.Entities;

public class ApiResponseTests
{
    [Fact]
    public void ApiResponse_Success_SetAndGet_ShouldWork()
    {
        // Arrange
        var apiResponse = new ApiResponse
        {
            Success = true
        };

        // Act & Assert
        Assert.True(apiResponse.Success);
    }
    // توضیح: 
    // این تست بررسی می‌کند که آیا ویژگی Success در کلاس ApiResponse به درستی کار می‌کند.
    // ما یک شیء از ApiResponse می‌سازیم و مقدار Success را true قرار می‌دهیم.
    // سپس بررسی می‌کنیم که آیا مقدار Success واقعاً true است یا خیر.
    // هدف: اطمینان از اینکه می‌توانیم مقدار Success را تنظیم کرده و به درستی بخوانیم.

    [Fact]
    public void ApiResponse_Message_SetAndGet_ShouldWork()
    {
        // Arrange
        var apiResponse = new ApiResponse
        {
            Message = "Operation completed successfully"
        };

        // Act & Assert
        Assert.Equal("Operation completed successfully", apiResponse.Message);
    }
    // توضیح:
    // این تست بررسی می‌کند که آیا ویژگی Message در کلاس ApiResponse به درستی کار می‌کند.
    // ما یک پیام متنی ("Operation completed successfully") به ویژگی Message اختصاص می‌دهیم.
    // سپس بررسی می‌کنیم که آیا مقدار Message همان چیزی است که تنظیم کردیم.
    // هدف: اطمینان از اینکه می‌توانیم یک پیام متنی را تنظیم کرده و به درستی بخوانیم.

    [Fact]
    public void ApiResponse_Message_Null_ShouldWork()
    {
        // Arrange
        var apiResponse = new ApiResponse
        {
            Message = null
        };

        // Act & Assert
        Assert.Null(apiResponse.Message);
    }
    // توضیح:
    // این تست بررسی می‌کند که آیا ویژگی Message می‌تواند مقدار null (تهی) داشته باشد.
    // ما مقدار Message را null قرار می‌دهیم و بررسی می‌کنیم که آیا واقعاً null است.
    // هدف: اطمینان از اینکه ویژگی Message به درستی با مقدار null کار می‌کند.

    [Fact]
    public void ApiResponseGeneric_Data_SetAndGet_ShouldWork()
    {
        // Arrange
        var apiResponse = new ApiResponse<string>
        {
            Data = "Test Data",
            Success = true,
            Message = "Data retrieved"
        };

        // Act & Assert
        Assert.Equal("Test Data", apiResponse.Data);
        Assert.True(apiResponse.Success);
        Assert.Equal("Data retrieved", apiResponse.Message);
    }
    // توضیح:
    // این تست بررسی می‌کند که آیا کلاس عمومی ApiResponse<T> با یک نوع داده ساده (مثل string) به درستی کار می‌کند.
    // ما یک شیء از ApiResponse<string> می‌سازیم و مقادیر Data، Success و Message را تنظیم می‌کنیم.
    // سپس بررسی می‌کنیم که آیا همه این مقادیر به درستی قابل دسترسی هستند.
    // هدف: اطمینان از اینکه ویژگی Data و سایر ویژگی‌ها در نسخه عمومی کلاس به درستی کار می‌کنند.

    [Fact]
    public void ApiResponseGeneric_Data_Null_ShouldWork()
    {
        // Arrange
        var apiResponse = new ApiResponse<string>
        {
            Data = null,
            Success = false,
            Message = "No data available"
        };

        // Act & Assert
        Assert.Null(apiResponse.Data);
        Assert.False(apiResponse.Success);
        Assert.Equal("No data available", apiResponse.Message);
    }
    // توضیح:
    // این تست بررسی می‌کند که آیا ویژگی Data در ApiResponse<T> می‌تواند مقدار null داشته باشد.
    // ما یک شیء از ApiResponse<string> می‌سازیم و Data را null، Success را false و Message را یک متن مشخص قرار می‌دهیم.
    // سپس بررسی می‌کنیم که آیا Data واقعاً null است و سایر ویژگی‌ها درست تنظیم شده‌اند.
    // هدف: اطمینان از اینکه کلاس عمومی با مقدار null برای Data به درستی کار می‌کند.

    [Fact]
    public void ApiResponseGeneric_WithComplexType_ShouldWork()
    {
        // Arrange
        var complexData = new { Id = 1, Name = "Test" };
        var apiResponse = new ApiResponse<object>
        {
            Data = complexData,
            Success = true
        };

        // Act & Assert
        Assert.Equal(complexData, apiResponse.Data);
        Assert.True(apiResponse.Success);
    }
    // توضیح:
    // این تست بررسی می‌کند که آیا کلاس ApiResponse<T> با یک نوع داده پیچیده (مثل یک شیء ناشناس) به درستی کار می‌کند.
    // ما یک شیء ناشناس با دو ویژگی Id و Name می‌سازیم و آن را به Data اختصاص می‌دهیم.
    // سپس بررسی می‌کنیم که آیا Data همان شیء ناشناس است و Success درست تنظیم شده است.
    // هدف: اطمینان از اینکه کلاس عمومی می‌تواند با نوع‌های داده پیچیده کار کند.
}