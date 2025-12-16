namespace SaiPublicity.Middleware
{
    public class MaintenanceMiddleware
    {
        private readonly RequestDelegate _next;

        public MaintenanceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //  WEBSITE DEADLINE
            var deadlineDate = new DateTime(2025, 12, 20);

            var path = context.Request.Path.Value?.ToLower();

            // Allow maintenance page itself
            if (path == "/maintenance")
            {
                await _next(context);
                return;
            }

            // Deadline reached or passed
            if (DateTime.Today >= deadlineDate.Date)
            {
                context.Response.Redirect("/maintenance");
                return;
            }

            await _next(context);
        }
    }
}
