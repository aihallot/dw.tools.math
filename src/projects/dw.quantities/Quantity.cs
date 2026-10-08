namespace dw.quantities;

public readonly record struct Quantity(ExactRational Value, DimensionVector Dimension)
{
    public bool TryAdd(Quantity other, out Quantity result)
    {
        if (Dimension != other.Dimension)
        {
            result = default;
            return false;
        }

        result = new Quantity(Value + other.Value, Dimension);
        return true;
    }

    public Quantity Multiply(Quantity other) =>
        new(Value * other.Value, Dimension + other.Dimension);

    public Quantity Divide(Quantity other) =>
        new(Value / other.Value, Dimension - other.Dimension);
}
