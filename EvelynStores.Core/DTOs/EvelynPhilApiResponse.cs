using System;
using System.Collections.Generic;
using System.Text;

namespace EvelynStores.Core.DTOs
{ 
    /// <summary>
    /// Generic API response
    /// </summary>
    public class EvelynPhilApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public int StatusCode { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public static EvelynPhilApiResponse<T> SuccessResponse(T data, string message = "Operation successful", int statusCode = 200)
        {
            return new EvelynPhilApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = statusCode
            };
        }

        public static EvelynPhilApiResponse<T> ErrorResponse(string message, int statusCode = 400, List<string> errors = null)
        {
            return new EvelynPhilApiResponse<T>
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Errors = errors ?? new List<string>()
            };
        }
    }

    /// <summary>
    /// Generic non-generic API response
    /// </summary>
    public class EvelynPhilApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public static EvelynPhilApiResponse SuccessResponse(string message = "Operation successful", int statusCode = 200)
        {
            return new EvelynPhilApiResponse
            {
                Success = true,
                Message = message,
                StatusCode = statusCode
            };
        }

        public static EvelynPhilApiResponse ErrorResponse(string message, int statusCode = 400, List<string> errors = null)
        {
            return new EvelynPhilApiResponse
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Errors = errors ?? new List<string>()
            };
        }
    }
}
