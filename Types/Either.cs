namespace redditJsonTool.Types;

public abstract class Either<TLeft, TRight>
{
    private Either() { }

    public sealed class Left : Either<TLeft, TRight>
    {
        public TLeft Value { get; }
        public Left(TLeft value) => Value = value;
    }

    public sealed class Right : Either<TLeft, TRight>
    {
        public TRight Value { get; }
        public Right(TRight value) => Value = value;
    }

    public static Either<TLeft, TRight> FromLeft(TLeft value) => new Left(value);
    public static Either<TLeft, TRight> FromRight(TRight value) => new Right(value);
    
    
    

    // implictly indicate to compiler to wrap returned either type methods with the correct either type
    public static implicit operator Either<TLeft, TRight>(TLeft left) => new Left(left);
    public static implicit operator Either<TLeft, TRight>(TRight right) => new Right(right);
}