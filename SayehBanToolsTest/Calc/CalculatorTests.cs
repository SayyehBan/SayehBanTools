using SayehBanTools.Calc;

namespace SayehBanToolsTest.Calc;
/// <summary>
/// تست محاسبات ریاضی
/// </summary>
public class CalculatorTests
{
    // Tests for Multiply
    /// <summary>
    /// بررسی می‌کند که ضرب اعداد معتبر به درستی انجام شود و نتیجه صحیح برگرداند.
    /// </summary>
    [Fact]
    public void Multiply_ValidNumbers_ReturnsCorrectProduct()
    {
        double[] numbers = { 2.5, 3, 4 };
        double result = Calculator.Multiply(numbers);
        Assert.Equal(30, result);
    }

    /// <summary>
    /// بررسی می‌کند که وقتی هیچ عددی وارد نشود، متد ضرب مقدار 1 را برگرداند.
    /// </summary>
    [Fact]
    public void Multiply_NoNumbers_ReturnsOne()
    {
        double[] numbers = { };
        double result = Calculator.Multiply(numbers);
        Assert.Equal(1, result);
    }

    /// <summary>
    /// بررسی می‌کند که وقتی یکی از اعداد صفر باشد، نتیجه ضرب صفر برگردد.
    /// </summary>
    [Fact]
    public void Multiply_WithZero_ReturnsZero()
    {
        double[] numbers = { 5, 0, 3 };
        double result = Calculator.Multiply(numbers);
        Assert.Equal(0, result);
    }

    /// <summary>
    /// بررسی می‌کند که وقتی آرایه ورودی null باشد، متد ضرب استثنای ArgumentNullException پرتاب کند.
    /// </summary>
    [Fact]
    public void Multiply_NullNumbers_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Calculator.Multiply(null!));
    }

    // Tests for Add
    /// <summary>
    /// بررسی می‌کند که جمع اعداد معتبر به درستی انجام شود و نتیجه صحیح برگرداند.
    /// </summary>
    [Fact]
    public void Add_ValidNumbers_ReturnsCorrectSum()
    {
        double[] numbers = { 1.5, 2.5, 3 };
        double result = Calculator.Add(numbers);
        Assert.Equal(7, result);
    }

    /// <summary>
    /// بررسی می‌کند که وقتی هیچ عددی وارد نشود، متد جمع مقدار 0 را برگرداند.
    /// </summary>
    [Fact]
    public void Add_NoNumbers_ReturnsZero()
    {
        double[] numbers = { };
        double result = Calculator.Add(numbers);
        Assert.Equal(0, result);
    }

    /// <summary>
    /// بررسی می‌کند که جمع اعداد شامل اعداد منفی به درستی انجام شود.
    /// </summary>
    [Fact]
    public void Add_NegativeNumbers_ReturnsCorrectSum()
    {
        double[] numbers = { -1, -2, 3 };
        double result = Calculator.Add(numbers);
        Assert.Equal(0, result);
    }

    /// <summary>
    /// بررسی می‌کند که وقتی آرایه ورودی null باشد، متد جمع استثنای ArgumentNullException پرتاب کند.
    /// </summary>
    [Fact]
    public void Add_NullNumbers_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Calculator.Add(null!));
    }

    // Tests for Subtract
    /// <summary>
    /// بررسی می‌کند که تفریق اعداد معتبر به درستی انجام شود و نتیجه صحیح برگرداند.
    /// </summary>
    [Fact]
    public void Subtract_ValidNumbers_ReturnsCorrectDifference()
    {
        double[] numbers = { 10, 3, 2 };
        double result = Calculator.Subtract(numbers);
        Assert.Equal(5, result);
    }

    /// <summary>
    /// بررسی می‌کند که وقتی تنها یک عدد وارد شود، متد تفریق استثنای ArgumentException پرتاب کند.
    /// </summary>
    [Fact]
    public void Subtract_SingleNumber_ThrowsArgumentException()
    {
        double[] numbers = { 5 };
        Assert.Throws<ArgumentException>(() => Calculator.Subtract(numbers));
    }

    /// <summary>
    /// بررسی می‌کند که وقتی هیچ عددی وارد نشود، متد تفریق استثنای ArgumentException پرتاب کند.
    /// </summary>
    [Fact]
    public void Subtract_NoNumbers_ThrowsArgumentException()
    {
        double[] numbers = { };
        Assert.Throws<ArgumentException>(() => Calculator.Subtract(numbers));
    }

    /// <summary>
    /// بررسی می‌کند که وقتی آرایه ورودی null باشد، متد تفریق استثنای ArgumentNullException پرتاب کند.
    /// </summary>
    [Fact]
    public void Subtract_NullNumbers_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Calculator.Subtract(null!));
    }

    // Tests for Divide
    /// <summary>
    /// بررسی می‌کند که تقسیم اعداد معتبر به درستی انجام شود و نتیجه صحیح برگرداند.
    /// </summary>
    [Fact]
    public void Divide_ValidNumbers_ReturnsCorrectQuotient()
    {
        double[] numbers = { 20, 2, 2 };
        double result = Calculator.Divide(numbers);
        Assert.Equal(5, result);
    }

    /// <summary>
    /// بررسی می‌کند که وقتی یکی از مقسوم‌علیه‌ها صفر باشد، متد تقسیم استثنای DivideByZeroException پرتاب کند.
    /// </summary>
    [Fact]
    public void Divide_DivisionByZero_ThrowsDivideByZeroException()
    {
        double[] numbers = { 10, 0, 2 };
        Assert.Throws<DivideByZeroException>(() => Calculator.Divide(numbers));
    }

    /// <summary>
    /// بررسی می‌کند که وقتی کمتر از دو عدد وارد شود، متد تقسیم استثنای ArgumentException پرتاب کند.
    /// </summary>
    [Fact]
    public void Divide_LessThanTwoNumbers_ThrowsArgumentException()
    {
        double[] numbers = { 5 };
        Assert.Throws<ArgumentException>(() => Calculator.Divide(numbers));
    }

    /// <summary>
    /// بررسی می‌کند که وقتی آرایه ورودی null باشد، متد تقسیم استثنای ArgumentNullException پرتاب کند.
    /// </summary>
    [Fact]
    public void Divide_NullNumbers_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Calculator.Divide(null!));
    }

    // Tests for Discount
    /// <summary>
    /// بررسی می‌کند که محاسبه تخفیف با مقدار و درصد معتبر به درستی انجام شود و مبلغ تخفیف‌خورده صحیح برگرداند.
    /// </summary>
    [Fact]
    public void Discount_ValidAmountAndPercent_ReturnsCorrectDiscountedAmount()
    {
        decimal amount = 100;
        short percent = 20;
        decimal result = Calculator.Discount(amount, percent);
        Assert.Equal(80, result);
    }

    /// <summary>
    /// بررسی می‌کند که وقتی درصد تخفیف صفر باشد، مبلغ اولیه بدون تغییر برگردد.
    /// </summary>
    [Fact]
    public void Discount_ZeroPercent_ReturnsOriginalAmount()
    {
        decimal amount = 100;
        short percent = 0;
        decimal result = Calculator.Discount(amount, percent);
        Assert.Equal(100, result);
    }

    /// <summary>
    /// بررسی می‌کند که وقتی درصد تخفیف منفی باشد، متد تخفیف استثنای ArgumentException پرتاب کند.
    /// </summary>
    [Fact]
    public void Discount_NegativePercent_ThrowsArgumentException()
    {
        decimal amount = 100;
        short percent = -10;
        Assert.Throws<ArgumentException>(() => Calculator.Discount(amount, percent));
    }

    /// <summary>
    /// بررسی می‌کند که وقتی مبلغ منفی باشد، متد تخفیف استثنای ArgumentException پرتاب کند.
    /// </summary>
    [Fact]
    public void Discount_NegativeAmount_ThrowsArgumentException()
    {
        decimal amount = -100;
        short percent = 10;
        Assert.Throws<ArgumentException>(() => Calculator.Discount(amount, percent));
    }

    // Tests for GetPercent
    /// <summary>
    /// بررسی می‌کند که محاسبه درصد با مقدار و قیمت معتبر به درستی انجام شود و درصد صحیح برگرداند.
    /// </summary>
    [Fact]
    public void GetPercent_ValidAmountAndPrice_ReturnsCorrectPercent()
    {
        decimal amount = 20;
        decimal price = 100;
        decimal result = Calculator.GetPercent(amount, price);
        Assert.Equal(20, result);
    }

    /// <summary>
    /// بررسی می‌کند که وقتی قیمت صفر یا منفی باشد، متد درصد استثنای ArgumentException پرتاب کند.
    /// </summary>
    [Fact]
    public void GetPercent_ZeroOrNegativePrice_ThrowsArgumentException()
    {
        decimal amount = 20;
        decimal price = 0;
        Assert.Throws<ArgumentException>(() => Calculator.GetPercent(amount, price));
    }

    /// <summary>
    /// بررسی می‌کند که وقتی قیمت منفی باشد، متد درصد استثنای ArgumentException پرتاب کند.
    /// </summary>
    [Fact]
    public void GetPercent_NegativePrice_ThrowsArgumentException()
    {
        decimal amount = 20;
        decimal price = -100;
        Assert.Throws<ArgumentException>(() => Calculator.GetPercent(amount, price));
    }

    // Tests for Taxation
    /// <summary>
    /// بررسی می‌کند که محاسبه مالیات با مقدار و درصد معتبر به درستی انجام شود و مبلغ با مالیات صحیح برگرداند.
    /// </summary>
    [Fact]
    public void Taxation_ValidAmountAndPercent_ReturnsCorrectTaxedAmount()
    {
        decimal amount = 100;
        short percent = 10;
        decimal result = Calculator.Taxation(amount, percent);
        Assert.Equal(110, result);
    }

    /// <summary>
    /// بررسی می‌کند که وقتی درصد مالیات صفر باشد، مبلغ اولیه بدون تغییر برگردد.
    /// </summary>
    [Fact]
    public void Taxation_ZeroPercent_ReturnsOriginalAmount()
    {
        decimal amount = 100;
        short percent = 0;
        decimal result = Calculator.Taxation(amount, percent);
        Assert.Equal(100, result);
    }

    /// <summary>
    /// بررسی می‌کند که وقتی درصد مالیات منفی باشد، متد مالیات استثنای ArgumentException پرتاب کند.
    /// </summary>
    [Fact]
    public void Taxation_NegativePercent_ThrowsArgumentException()
    {
        decimal amount = 100;
        short percent = -10;
        Assert.Throws<ArgumentException>(() => Calculator.Taxation(amount, percent));
    }

    /// <summary>
    /// بررسی می‌کند که وقتی مبلغ منفی باشد، متد مالیات استثنای ArgumentException پرتاب کند.
    /// </summary>
    [Fact]
    public void Taxation_NegativeAmount_ThrowsArgumentException()
    {
        decimal amount = -100;
        short percent = 10;
        Assert.Throws<ArgumentException>(() => Calculator.Taxation(amount, percent));
    }
}