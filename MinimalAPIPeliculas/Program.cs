using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas;
using MinimalAPIPeliculas.Endponits;
using MinimalAPIPeliculas.Entidades;
using MinimalAPIPeliculas.Migrations;
using MinimalAPIPeliculas.Repositorios;

var builder = WebApplication.CreateBuilder(args);
//Origenes de Cors que vienen del appsentigs de desarrollo
var origenesPermitidos = builder.Configuration.GetValue<string>("origenesPermitidos")!;

//Inicio de area de servicios

builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
opciones.UseSqlServer("name=DefaultConnection"));

//Servicio de Configuracion de cors
builder.Services.AddCors(opciones =>
{
    //Politica por defecto
    opciones.AddDefaultPolicy(configuracion =>
    {
        //Permitir Todos los origenes, metodos y cabeceras
        //configuracion.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();

        //Permitir origenes establecidos, y sus cabeceras y metodos
        configuracion.WithOrigins(origenesPermitidos).AllowAnyHeader().AllowAnyMethod();
    });
    //Otra politica
    opciones.AddPolicy("libre", configuracion =>
    {
        configuracion.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddOutputCache();

//Permite que Swagger pueda explorar nuestros endpoinits
builder.Services.AddEndpointsApiExplorer();
//Configuracion de Swagger
builder.Services.AddSwaggerGen();

//Configurando nuestro propio servicio, permite utilizarlo en cualquier parte de la aplicacion (abstraccion)
builder.Services.AddScoped<IRepositorioGeneros, RepositorioGeneros>();

//Fin de area de servicios

var app = builder.Build();

//Inicio de area de los middleware

//Configuracion en un ambiente especifico
if (builder.Environment.IsDevelopment()) { }

//Swagger
app.UseSwagger();
//Interfaz Grafica de Swagger
app.UseSwaggerUI();

//Uso de cors en nuestra api
app.UseCors();

app.UseOutputCache();

//Este endpoint esta configurado con la politica libre de cors
app.MapGet("/", [EnableCors(policyName: "libre")] () => "¡Hola Mundo!");

app.MapGroup("/generos").MapGeneros();

//Fin del area de los middleware
app.Run();

