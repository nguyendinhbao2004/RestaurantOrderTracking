using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Application.Common.Behavior
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // 1. Nếu không có validator nào cho command này thì bỏ qua
            if (!_validators.Any())
                return await next();
            // 2. Chạy tất cả các rules validation
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(_validators.Select(v
                                                        => v.ValidateAsync(context, cancellationToken)));

            // 3. Lấy tất cả lỗi (nếu có)
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();
            // 4. Nếu có lỗi -> Ném ra Exception (Middleware ở ngoài sẽ bắt cái này)
            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
            // 5. Nếu không lỗi -> Đi tiếp vào Handler
            return await next();
        }
    }
}
