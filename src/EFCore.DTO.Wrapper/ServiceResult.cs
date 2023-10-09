namespace EFCore.DTO.Wrapper;

public abstract record ServiceResult
{
    public static ServiceResult<T> Success<T>(T result) where T : class
        => new ServiceResult<T>(result, null);
    public static ServiceResult<T> Fail<T>(Exception error) where T : class
        => new ServiceResult<T>(null, error);
}

public record ServiceResult<T>(T? Result, Exception? Error) where T : class
{
    public bool IsSuccess => Error == null;
}
