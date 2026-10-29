using System.Text;
using HappyChat.API.Middlewares;
using HappyChat.Application;
using HappyChat.Infrastructure;
using HappyChat.Infrastructure.Auth.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Lê e valida a configuração do JWT já na inicialização (falha cedo, com mensagem clara).
var jwtSettings = builder.Configuration
    .GetSection(JwtSettings.SectionName)
    .Get<JwtSettings>() ?? new JwtSettings();
jwtSettings.Validate();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Happy Chat API",
        Version = "v1",
        Description = "Backend happy chat"
    });

    // Botão "Authorize" no Swagger para testar rotas protegidas.
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Cole apenas o token JWT (sem o prefixo 'Bearer ')."
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
    
    var xmlFiles = new[] { "HappyChat.API.xml", "HappyChat.Application.xml" };
    foreach (var xml in xmlFiles)
    {
        var path = Path.Combine(AppContext.BaseDirectory, xml);
        if (File.Exists(path))
            c.IncludeXmlComments(path, includeControllerXmlComments: true);
    }
});
builder.Services.AddHealthChecks();

// CORS: só as origens listadas em "Cors:AllowedOrigins" podem chamar a API pelo navegador.
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(jwtSettings, builder.Configuration);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

// Antes do exception handler e da autenticação: assim o preflight (OPTIONS) passa
// e até as respostas de erro (401, 500) levam os headers de CORS.
app.UseCors("Frontend");

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();