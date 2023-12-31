using Get_IPlayer_Wrapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddLogging();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IRecording, Recording>();

var app = builder.Build();


// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();