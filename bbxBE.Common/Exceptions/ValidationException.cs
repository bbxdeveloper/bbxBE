using bbxBE.Common.Consts;
using FluentValidation.Results;
using System;
using System.Collections.Generic;

namespace bbxBE.Common.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException() : base( bbxBEConsts.ERR_VALIDATION)
        {
            Errors = new List<string>();
        }

        public List<string> Errors { get; }

        public ValidationException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            foreach (var failure in failures)
            {
                Errors.Add(failure.ErrorMessage);
            }
        }

        public ValidationException(string message) : base(message)
        {
        }

        public ValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}