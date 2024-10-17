using System.Text;
using System.Text.Json.Serialization;

using ccsflowserver.Controllers;
using ccsflowserver.Data;
using ccsflowserver.Model;
using ccsflowserver.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

var connectionString = BuildConnectionString(builder);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseNpgsql(connectionString);

});


builder.Services.AddScoped<IPasswordManager>(x => new PasswordManager());
builder.Services.AddScoped<IClaimsTranslator, ClaimsTranslator>();
builder.Services.AddScoped<IModelService<TagBlogpostMapping>>(IModelService => new TagBlopostMappingService(IModelService.GetRequiredService<AppDbContext>()));
builder.Services.AddScoped<IModelService<Tag>>(IModelService => new TagService(IModelService.GetRequiredService<AppDbContext>()));
builder.Services.AddScoped<IModelService<Category>>(IModelService => new CategoryService(IModelService.GetRequiredService<AppDbContext>()));
builder.Services.AddScoped<IAuthservice>(IAuthservice => new AuthService(IAuthservice.GetRequiredService<AppDbContext>(), IAuthservice.GetRequiredService<IPasswordManager>()));
builder.Services.AddScoped<IModelService<BlogPost>>(IModelService => new BlogPostService(IModelService.GetRequiredService<AppDbContext>()));
builder.Services.AddScoped<IModelService<User>>(IModelService => new UserService(IModelService.GetRequiredService<AppDbContext>()));
builder.Services.AddScoped<SlugCreator>();

builder.Services.AddControllers();
var jtwKey = Environment.GetEnvironmentVariable("JWT_KEY");
if (jtwKey == null)
{
	throw new ArgumentNullException(nameof(jtwKey), "Provide a json web token key as env variable");
}
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	  .AddJwtBearer(options =>
	  {
		  options.TokenValidationParameters = new TokenValidationParameters
		  {
			  ValidateIssuer = true,
			  ValidateAudience = true,
			  ValidateLifetime = true,
			  ValidateIssuerSigningKey = true,
			  ValidIssuer = config["Jwt:Issuer"],
			  ValidAudience = config["Jwt:Audience"],
			  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jtwKey))
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
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
	ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.MapControllers();


//app.UseHttpsRedirection();


app.Run();

static string BuildConnectionString(WebApplicationBuilder builder)
{
	const string _databaseHost = "DATABASE_HOST_PW";
	const string _databaseName = "DATABASE_PW";
	const string _databaseUser = "DATABASE_USER_PW";
	const string _databasePassword = "DATABASE_PASS_PW";

	List<string> strings = new()
	{
	_databaseHost,
	_databaseName,
	_databaseUser,
	_databasePassword
};
	string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

	foreach (var str in strings)
	{
		var env = Environment.GetEnvironmentVariable(str);
		if (env is null)
		{
			throw new Exception($"Environment variable {str} is not set");
		}
		else
		{
			connectionString = connectionString.Replace($"[{str}]", env);
		}
	}

	return connectionString;
}