using System.Data;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SayehBanTools.Converter;
/// <summary>
/// مدیریتی برای تاریخ و زمان
/// </summary>
public class ManageDateAndTime
{/// <summary>
 /// تبدیل تاریخ همیشه به میلادی
 /// </summary>
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private const string Format = "yyyy-MM-dd";
        /// <summary>
        /// خواندن فقط تاریخ
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return DateOnly.ParseExact(value!, Format, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// نوشتن فقط تاریخ
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
        }
    }
    /// <summary>
    /// هندل فقط تاریخ
    /// </summary>
    public class DateOnlyTypeHandler : Dapper.SqlMapper.TypeHandler<DateOnly>
    {
        /// <summary>
        /// قرار دادن مقدار
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        public override void SetValue(IDbDataParameter parameter, DateOnly value)
        {
            parameter.Value = value.ToDateTime(TimeOnly.MinValue);
        }
        /// <summary>
        /// تلاش برای مقدار
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override DateOnly Parse(object value)
        {
            if (value is DateTime dt)
                return DateOnly.FromDateTime(dt);

            return (DateOnly)value;
        }
    }
}
