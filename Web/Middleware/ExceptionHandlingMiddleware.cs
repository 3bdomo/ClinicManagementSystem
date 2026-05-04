namespace Web.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)   
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception - Path: {Path} | User: {User} | Method: {Method}",
                context.Request.Path,
                context.User.Identity?.Name ?? "Anonymous",
                context.Request.Method);

            if (context.Response.HasStarted) return;

            context.Response.Clear();
            context.Response.Redirect("/Home/Error");


        }
    }
}