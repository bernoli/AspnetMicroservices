using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace Ordering.Application.Exceptions
{
    public class ValidationException : ApplicationException
    {
        public Dictionary<string, string[]> Errors { get; set; }

        public ValidationException() : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures) : this()
        {
            Errors = failures
                .GroupBy(failure => failure.PropertyName, e => e.ErrorMessage)
                .ToDictionary(grouping => grouping.Key, grouping => grouping.ToArray());
        }
    }
}
