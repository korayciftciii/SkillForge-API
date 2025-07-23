namespace SkillForge.API.Middlewares
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;

        public SecurityHeadersMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            // Basic security headers
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-Frame-Options"] = "DENY";
            context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
            context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            
            // HTTP Strict Transport Security
            context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload";
            
            // Cache control
            context.Response.Headers["Cache-Control"] = "no-store, max-age=0";
            
            // Content Security Policy
            // In development, allow more sources for debugging tools
            string cspValue;
            if (_env.IsDevelopment())
            {
                cspValue = "default-src 'self'; " +
                           "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                           "style-src 'self' 'unsafe-inline'; " +
                           "img-src 'self' data:; " +
                           "font-src 'self'; " +
                           "connect-src 'self'; " +
                           "frame-ancestors 'none'; " +
                           "form-action 'self';";
            }
            else
            {
                cspValue = "default-src 'self'; " +
                           "script-src 'self'; " +
                           "style-src 'self'; " +
                           "img-src 'self'; " +
                           "font-src 'self'; " +
                           "connect-src 'self'; " +
                           "frame-ancestors 'none'; " +
                           "form-action 'self';";
            }
            
            context.Response.Headers["Content-Security-Policy"] = cspValue;
            
            // Cross-Origin-Embedder-Policy
            context.Response.Headers["Cross-Origin-Embedder-Policy"] = "require-corp";
            
            // Cross-Origin-Opener-Policy
            context.Response.Headers["Cross-Origin-Opener-Policy"] = "same-origin";
            
            // Cross-Origin-Resource-Policy
            context.Response.Headers["Cross-Origin-Resource-Policy"] = "same-origin";
            
            // Permissions-Policy
            context.Response.Headers["Permissions-Policy"] = 
                "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()";
            
            await _next(context);
        }
    }
}
