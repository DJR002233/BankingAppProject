
using BankingAppProjectWebAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.InitializeServices(builder.Configuration);

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();//.RequireAuthorization();

app.Run();

/*
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "yourapp",
            ValidAudience = "yourapp",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("supersecretkey"))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();  // <-- Adds the auth middleware
app.UseAuthorization();   // <-- Adds the authorization middleware
app.MapControllers().RequireAuthorization();

app.Run();


Here’s what happens:

AddAuthentication("Bearer") → registers an authentication scheme called "Bearer".

AddJwtBearer("Bearer", ...) → tells ASP.NET Core: "when the scheme is Bearer, expect a JWT token in the Authorization header".

UseAuthentication() → puts a middleware in the pipeline that:

Reads the request

Checks Authorization: Bearer <token> header

If valid → builds a ClaimsPrincipal (the logged-in user) and attaches it to HttpContext.User.

UseAuthorization() → runs after authentication. It checks:

Does this endpoint require [Authorize] (or .RequireAuthorization())?

If yes → is there a User in HttpContext with valid claims?

If no → returns 401 Unauthorized or 403 Forbidden.

*/


/*
2. The Request Flow

Say you call:

GET /api/secure/profile
Authorization: Bearer eyJhbGciOiJI...


Request comes in.

UseAuthentication() sees the Authorization header → matches "Bearer" scheme → runs the JWT handler.

JWT handler parses the token, validates it against the secret/signing key, expiration, issuer, audience, etc.

If valid → sets HttpContext.User = ClaimsPrincipal (user is now “logged in”).

Next, UseAuthorization() checks if this endpoint requires [Authorize].

If yes and User.Identity.IsAuthenticated == true → allow request to proceed into the controller.

Otherwise → short-circuit with 401/403.
*/


/*Why Does .RequireAuthorization() Work?

When you add .RequireAuthorization(), ASP.NET Core attaches an endpoint metadata tag that says: "This endpoint requires authorization".

When the request pipeline reaches UseAuthorization(), it checks that metadata and runs the corresponding auth checks.

How does it know which middleware to run? → Because in AddAuthentication("Bearer"), you set the default scheme as "Bearer".

So whenever [Authorize] is applied, it uses the default authentication scheme (Bearer) to try to authenticate the request.

If you had multiple schemes (e.g. Cookies + JWT), you could configure [Authorize(AuthenticationSchemes = "Bearer")] to force which one to use.*/
