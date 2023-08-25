﻿namespace Domain.Models.Base;

/// <summary>
/// Стандартная модель ответа
/// </summary>
public class BaseResponse
{
    /// <summary>
    /// Пустой конструктор
    /// </summary>
    public BaseResponse()
    {
        
    }

    /// <summary>
    /// Конструктор с признаком успешности
    /// </summary>
    /// <param name="success"></param>
    public BaseResponse(bool success): this()
    {
        Success = success;        
    }

    /// <summary>
    /// Конструктор с ошибкой
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    public BaseResponse(bool success, BaseError error): this(success)
    {
        Error = error;
    }

    /// <summary>
    /// Признак успешности ответа
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Ошибка
    /// </summary>
    public BaseError? Error { get; set; }
}
