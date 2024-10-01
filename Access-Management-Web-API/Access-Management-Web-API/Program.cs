using Access_Management_Web_API.Container;
using Access_Management_Web_API.Helper;
using Access_Management_Web_API.Model;
using Access_Management_Web_API.Repos;
using Access_Management_Web_API.Repos.Models;
using Access_Management_Web_API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<IRefereseHandeler, RefereseHandeler>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IUserRoleServices,UserRoleService>();
builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddDbContext<LarnDataContext>(o => o
.UseSqlServer(builder.Configuration.GetConnectionString("apicon")));
//builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandeler>("BasicAuthentication", null);
var _authKey = builder.Configuration.GetValue<string>("JwtSettings:SequrityKey");
builder.Services.AddAuthentication(item =>
{
    item.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
    item.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(item =>
{
    item.RequireHttpsMetadata = true;
    item.SaveToken = true;
    item.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authKey)),
        ValidateIssuer=false,
        ValidateAudience=false,
        ClockSkew = TimeSpan.Zero

    };
});

//var automapper = new MapperConfiguration(item => item.AddProfile(new AutoMapperHandelar()));
//IMapper mapper = automapper.CreateMapper();
//builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(typeof(AutoMapperHandelar));

builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));
builder.Services.AddCors(p => p.AddPolicy("corspolicy1", build =>
{
    build.WithOrigins("https://localhost:7249").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddCors(p => p.AddDefaultPolicy(build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddRateLimiter(_ => _.AddFixedWindowLimiter(policyName: "fixedwindow", options =>
{
    options.Window = TimeSpan.FromSeconds(10);
    options.PermitLimit = 1;
    options.QueueLimit = 0;
    options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
}).RejectionStatusCode = 401);

string logPath = builder.Configuration.GetSection("Logging:LogPath").Value;
var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.File(logPath)
    .CreateLogger();
builder.Logging.AddSerilog(logger);

var _jwtsettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(_jwtsettings);

var app = builder.Build();

app.MapGet("/minimalapi", () => "Firoz Ahmed");

app.MapGet("/getchallen", (string challename) => "Welcome to "+ challename).WithOpenApi(opt =>
{
    var parameter = opt.Parameters[0];
    parameter.Description = "EnterChallen Name";
    return opt;
});

app.MapGet("/GetAllCustomer", async (LarnDataContext db) =>
{
    return await db.TblCustomers.ToListAsync();

});

app.MapGet("/GetByCode/{code}", async (LarnDataContext db, string code) =>
{
    return await db.TblCustomers.FindAsync(code);

});

app.MapPost("/CreateCustomer", async (LarnDataContext db, TblCustomer customer) =>
{
     await db.TblCustomers.AddAsync(customer);

     await db.SaveChangesAsync();


});


app.MapPut("/UpdateCustomer/{code}", async (LarnDataContext db, TblCustomer customer, string code) =>
{
    var existdata = await db.TblCustomers.FindAsync(code);

      if (existdata != null)
        {
            existdata.Name= customer.Name;
            existdata.Email= customer.Email;
        }

    await db.SaveChangesAsync();


});


app.MapDelete("/DeleteCustomer/{code}", async (LarnDataContext db,  string code) =>
{
    var existdata = await db.TblCustomers.FindAsync(code);

    if (existdata != null)
    {
        db.TblCustomers.Remove(existdata);
    }

    await db.SaveChangesAsync();


});




app.UseRateLimiter();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseCors();

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();



