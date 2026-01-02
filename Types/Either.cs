namespace redditJsonTool.Types;

/// <summary>
/// Stolen from another project, a simple Either implementation for wrapping either/or values.
/// not all that useful since this is a simple linear console app, but i wanted to try it out.
/// Wrapper around either/or values. Must be either a left (bad) or right (good) value.
/// </summary>
/// <typeparam name="TLeft"></typeparam>
/// <typeparam name="TRight"></typeparam>
public abstract class Either<TLeft, TRight>
{
    /// <summary>
    /// Operator that implicitly converts a left value into an Either.
    /// </summary>
    /// <param name="obj"></param>
    public static implicit operator Either<TLeft, TRight>(TLeft obj)
    {
        return new Left<TLeft, TRight>(obj);
    }

    /// <summary>
    /// Operator that implicitly converts a right value into an Either.
    /// </summary>
    /// <param name="obj"></param>
    public static implicit operator Either<TLeft, TRight>(TRight obj)
    {
        return new Right<TLeft, TRight>(obj);
    }
}

/// <summary>
/// Wrapper around left values; used with Eithers.
/// </summary>
/// <typeparam name="TLeft"></typeparam>
/// <typeparam name="TRight"></typeparam>
public class Left<TLeft, TRight> : Either<TLeft, TRight>
{
    private TLeft Content { get; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public Left(TLeft content)
    {
        Content = content;
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// Operator that unboxes left values.
    /// </summary>
    /// <param name="obj"></param>
    public static implicit operator TLeft(Left<TLeft, TRight> obj)
    {
        return obj.Content;
    }
}

/// <summary>
/// Wrapper around right values; used with Eithers.
/// </summary>
/// <typeparam name="TLeft"></typeparam>
/// <typeparam name="TRight"></typeparam>
public class Right<TLeft, TRight> : Either<TLeft, TRight>
{
    private TRight Content { get; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public Right(TRight content)
    {
        Content = content;
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// Operator that unboxes right values.
    /// </summary>
    /// <param name="obj"></param>
    public static implicit operator TRight(Right<TLeft, TRight> obj)
    {
        return obj.Content;
    }
}