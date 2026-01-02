namespace redditJsonTool.Types;

public static class EitherHelpers
{
    /// <summary>
    /// determines if the Either is a Right value
    /// </summary>
    /// <param name="either"></param>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <returns></returns>
    public static bool IsRight<TLeft, TRight>(this Either<TLeft, TRight> either)
    {
        return either is Right<TLeft, TRight>;
    }

    /// <summary>
    /// Determines if the Either is a Left value
    /// </summary>
    /// <param name="either"></param>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <returns></returns>
    public static bool IsLeft<TLeft, TRight>(this Either<TLeft, TRight> either)
    {
        return either is Left<TLeft, TRight>;
    }

    /// <summary>
    /// tries to get the right value from an Either
    /// </summary>
    /// <param name="either"></param>
    /// <param name="right"></param>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <returns></returns>
    public static bool TryGetRight<TLeft, TRight>(this Either<TLeft, TRight> either, out TRight? right)
        where TRight : notnull
    {
        right = default;
        if (either is Right<TLeft, TRight> r)
        {
            right = r.FromRight();
            return true;
        }

        return false;
    }

    /// <summary>
    /// tries to get the left value from an Either
    /// </summary>
    /// <param name="either"></param>
    /// <param name="left"></param>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <returns></returns>
    public static bool TryGetLeft<TLeft, TRight>(this Either<TLeft, TRight> either, out TLeft? left)
    {
        if (either is Left<TLeft, TRight> l)
        {
            left = l;
            return true;
        }

        left = default;
        return false;
    }

    /// <summary>
    /// returns the left value from a Left Either
    /// </summary>
    /// <param name="left"></param>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <returns></returns>
    public static TLeft FromLeft<TLeft, TRight>(this Left<TLeft, TRight> left)
    {
        return (TLeft)left;
    }

    /// <summary>
    /// returns the right value from a right Either
    /// </summary>
    /// <param name="right"></param>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <returns></returns>
    public static TRight FromRight<TLeft, TRight>(this Right<TLeft, TRight> right)
    {
        return (TRight)right;

    }
}