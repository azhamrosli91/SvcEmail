using libMasterLibaryApi.ContextRepo;
using libMasterLibaryApi.Helpers;
using libMasterLibaryApi.Interface;
using SvcEmail.Interface;
using SvcEmail.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//libary Master Api required -- start
builder.Services.AddHttpClient();
builder.Services.AddCors();
builder.Services.AddScoped<IJwtToken, JWTToken>();
builder.Services.AddScoped<IWebApiCalling, WebApiCalling>();
builder.Services.AddScoped<IImageConvert, ImageConverter>();
builder.Services.AddScoped<IDbService, DBContext>();
builder.Services.AddScoped<IApiURL, ApiURLRepo>();
//libary Master Api required -- end
builder.Services.AddScoped<IEmail, EmailSendGridRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(builder => builder
                 .AllowAnyHeader()
                 .AllowAnyMethod()
                 .SetIsOriginAllowed((host) => true)
                 .AllowCredentials()
                 .WithOrigins("https://localhost")
             //.WithOrigins("https://localhost:32777") //SvcAccount   Docker
             //.WithOrigins("https://localhost:32779") //SvcEmail     Docker
             //.WithOrigins("https://localhost:32781") //EpsilonSigma Docker
             //.WithOrigins("https://localhost:44358") //SvcAccount   IIS
             //.WithOrigins("https://localhost:44301") //SvcEmail     IIS
             //.WithOrigins("https://localhost:44326") //EpsilonSigma IIS
             );
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
