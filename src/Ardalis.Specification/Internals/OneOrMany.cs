namespace Ardalis.Specification;

internal struct OneOrMany<T> where T : class
{
    private const int DEFAULT_CAPACITY = 2;
    private object? _value;

    public readonly bool IsEmpty => _value is null;
    public readonly bool HasSingleItem => _value is T;

    public void Add(T item)
    {
        switch (_value)
        {
            case null:
                _value = item;
                return;
            case List<T> list:
                list.Add(item);
                return;
            case T singleValue:
                _value = new List<T>(2) { singleValue, item };
                break;
        }
    }

    public void AddSorted(T item, IComparer<T> comparer)
    {
        if (_value is null)
        {
            _value = item;
            return;
        }

        if (comparer is null)
        {
            throw new ArgumentNullException(nameof(comparer), "Comparer cannot be null.");
        }

        switch (_value)
        {
            case List<T> list:
                {
                    var index = list.FindIndex(x => comparer.Compare(item, x) < 0);
                    if (index == -1)
                    {
                        list.Add(item);
                    }
                    else
                    {
                        list.Insert(index, item);
                    }

                    return;
                }
            case T singleValue when comparer.Compare(item, singleValue) < 0:
                _value = new List<T>(DEFAULT_CAPACITY) { item, singleValue };
                break;
            case T singleValue:
                _value = new List<T>(DEFAULT_CAPACITY) { singleValue, item };
                break;
        }
    }

    /// <summary>
    /// Gets the list value stored in the instance.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the value is Empty or Single.</exception>
    public readonly List<T> List
    {
        get
        {
            if (_value is List<T> list)
            {
                return list;
            }

            throw new InvalidOperationException("The value is not a list.");
        }
    }

    /// <summary>
    /// Gets the single value stored in the instance.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the value is Empty or Many.</exception>
    public readonly T Single
    {
        get
        {
            if (_value is T singleValue)
            {
                return singleValue;
            }

            throw new InvalidOperationException("The value is not a single item.");
        }
    }

    /// <summary>
    /// Gets the single value stored in the instance.
    /// If the value is Empty or Many, returns null.
    /// </summary>
    public readonly T? SingleOrDefault
    {
        get
        {
            if (_value is T singleValue)
            {
                return singleValue;
            }

            return null;
        }
    }

    public readonly IEnumerable<T> Values
    {
        get
        {
            return _value switch
            {
                null => Enumerable.Empty<T>(),
                List<T> list => list,
                T singleValue => new[] { singleValue },
                _ => throw new InvalidOperationException("The value is neither a single item nor a list of items.")
            };
        }
    }

    public readonly OneOrMany<T> Clone()
    {
        var clone = new OneOrMany<T>();

        clone._value = _value switch
        {
            T singleValue => singleValue,
            List<T> list => list.ToList(),
            _ => clone._value
        };

        return clone;
    }
}
