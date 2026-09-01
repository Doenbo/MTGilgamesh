using System;
using System.Collections.Generic;
using System.Text;

namespace MTG.Core.Helper;

public static class ResultExtensions
{
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> mapper)
    {
        return result.IsSuccess
            ? Result<TOut>.Success(mapper(result.Value))
            : Result<TOut>.Failure(result.Error);
    }
}