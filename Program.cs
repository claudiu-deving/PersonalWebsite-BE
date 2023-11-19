using System.Net;
using System.Text;
using System.Text.RegularExpressions;

using ccsflowserver.Data;
using ccsflowserver.Model;
using ccsflowserver.Services;

using MdToHtml;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<IPasswordManager>(x => new PasswordManager());
builder.Services.AddScoped<IAuthservice>(IAuthservice => new AuthService(IAuthservice.GetRequiredService<AppDbContext>(), IAuthservice.GetRequiredService<IPasswordManager>()));
builder.Services.AddScoped<IModelService<BlogPost>>(IModelService => new BlogPostService(IModelService.GetRequiredService<AppDbContext>()));

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(options =>
      {
          options.TokenValidationParameters=new TokenValidationParameters
          {
              ValidateIssuer=true,
              ValidateAudience=true,
              ValidateLifetime=true,
              ValidateIssuerSigningKey=true,
              ValidIssuer=config["Jwt:Issuer"],
              ValidAudience=config["Jwt:Audience"],
              IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]))
          };
      });

builder.Services.AddAuthorization();


builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .AllowAnyMethod()
            .AllowAnyOrigin()
            .AllowAnyHeader());
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders=ForwardedHeaders.XForwardedFor|ForwardedHeaders.XForwardedProto
});

app.MapControllers();


//app.UseHttpsRedirection();


app.Run();
partial class Program
{
    [GeneratedRegex("([a-z A-Z 1-9-_]*)\\.md")]
    private static partial Regex MyRegex();
}