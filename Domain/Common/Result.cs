using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Domain.Common
{
    public class Result 
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }

        public Result()
        {

        }
        public Result(bool succeeded, string message, IEnumerable<string> errors)
        {
            Succeeded = succeeded;
            Message = message;
            Errors = errors?.ToList() ?? new List<string>();
        }

        // Helper: Trả về thành công
        public static Result Success()
        {
            return new Result(true, string.Empty, null);
        }

        public static Result Success(string message)
        {
            return new Result(true, message, null);
        }

        // Helper: Trả về thất bại
        public static Result Failure(string error)
        {
            return new Result(false, null, new List<string> { error });
        }

        public static Result Failure(IEnumerable<string> errors = null)
        {
            return new Result(false, null, errors);
        }
    }

    public class Result<T> : Result
    {
        public T Data { get; private set; }
        public Result(bool succeeded, string message, IEnumerable<string> errors, T data)
            : base(succeeded, message, errors)
        {
            Data = data;
        }
        // Helper: Trả về thành công với dữ liệu
        public static Result<T> Success(T data)
        {
            return new Result<T>(true, string.Empty, null, data);
        }
        public static Result<T> Success(string message, T data)
        {
            return new Result<T>(true, message, null, data);
        }
        // Helper: Trả về thất bại
        public new static Result<T> Failure(string error)
        {
            return new Result<T>(false, null, new List<string> { error }, default);
        }
        public new static Result<T> Failure(IEnumerable<string> errors = null)
        {
            return new Result<T>(false, null, errors, default);
        }
    }
}
