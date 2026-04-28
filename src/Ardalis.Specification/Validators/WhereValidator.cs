namespace Ardalis.Specification;

public class WhereValidator : IValidator
{
    private WhereValidator() { }
    public static WhereValidator Instance { get; } = new WhereValidator();

    public bool IsValid<T>(T entity, ISpecification<T> specification)
    {
        if (specification is not Specification<T> spec)
        {
            return specification.WhereExpressions.All(whereExpr => whereExpr.FilterFunc(entity));
        }

        if (spec.OneOrManyWhereExpressions.IsEmpty) return true;

        if (spec.OneOrManyWhereExpressions.SingleOrDefault is { } whereExpression)
        {
            return whereExpression.FilterFunc(entity);
        }

        return spec.OneOrManyWhereExpressions.List.All(whereExpr => whereExpr.FilterFunc(entity));
    }
}
