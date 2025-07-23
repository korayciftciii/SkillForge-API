using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillForge.Shared.Results
{
  
    public class Result
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = [];

        public static Result Ok(string? message = null) => new()
        {
            Success = true,
            Message = message
        };

        public static Result Fail(params string[] errors) => new()
        {
            Success = false,
            Errors = errors.ToList()
        };
    }

    public class Result<T> : Result
    {
        public T? Value { get; set; }

        public static Result<T> Ok(T value, string? message = null) => new()
        {
            Success = true,
            Value = value,
            Message = message
        };

        public static new Result<T> Fail(params string[] errors) => new()
        {
            Success = false,
            Errors = errors.ToList()
        };
    }


}
