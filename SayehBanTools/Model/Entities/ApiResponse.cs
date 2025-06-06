using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayehBanTools.Model.Entities;

/// <summary>
/// مدل پاسخ استاندارد API
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// نتیجه عملیات
    /// </summary>
    public bool Success { get; set; }
    /// <summary>
    /// پشغام
    /// </summary>
    public string? Message { get; set; }
}

/// <summary>
/// مدل پاسخ استاندارد API با داده
/// </summary>
/// <typeparam name="T">نوع داده</typeparam>
public class ApiResponse<T> : ApiResponse
{
    /// <summary>
    /// 
    /// </summary>
    public T? Data { get; set; }
}
