namespace redditJsonTool.Types;

public class ErrorMessage(string message)
{
    public string Message { get; } = message;

}
public class Error()
{
}
public class SuccessMessage(string message)
{
    public string Message { get; } = message;
}
public class Success()
{
}