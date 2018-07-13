namespace MalikP.GitHub.LabelSynchronizer.Parameters
{
    public abstract class Parameter<T> : IParameter
    {
        public Parameter(T value)
        {
            Value = value;
        }

        public T Value { get; private set; }

        object IParameter.Value => Value;
    }
}
