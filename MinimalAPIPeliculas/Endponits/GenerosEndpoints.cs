using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Entidades;
using MinimalAPIPeliculas.Repositorios;

namespace MinimalAPIPeliculas.Endponits
{
    public static class GenerosEndpoints
    {

        public static RouteGroupBuilder MapGeneros(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerGeneros).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("generos-get"));
            group.MapGet("/{id:int}", ObtenerGeneroPorId);
            group.MapPost("/", CrearGenero);
            group.MapPut("/{id:int}", ActualizarGenero);
            group.MapDelete("/{id:int}", BorrarGenero);
            return group;
        }

        static async Task<Ok<List<GeneroDTO>>> ObtenerGeneros(IRepositorioGeneros repositorio)
        {
            var generos = await repositorio.ObtenerTodos();
            //Se usa para que en swagger se pueda visualizar las respuestas del endpoint
            var generosDTO = generos.Select(x => new GeneroDTO { Id = x.Id, Nombre = x.Nombre }).ToList();
            return TypedResults.Ok(generosDTO);
        }

        static async Task<Results<Ok<GeneroDTO>, NotFound>> ObtenerGeneroPorId(IRepositorioGeneros repositorio, int id)
        {
            var genero = await repositorio.ObtenerPorId(id);
            if (genero is null)
            {
                return TypedResults.NotFound();
            }
            var generoDTO = new GeneroDTO
            {
                Id = id,
                Nombre = genero.Nombre
            };
            return TypedResults.Ok(generoDTO);
        }
        static async Task<Created<GeneroDTO>> CrearGenero(CrearGeneroDto crearGeneroDTO, IRepositorioGeneros repositorio, IOutputCacheStore outputCacheStore)
        {
            var genero = new Genero
            {
                Nombre = crearGeneroDTO.Nombre
            };

            var id = await repositorio.Crear(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);

            var generoDTO = new GeneroDTO
            {
                Id = id,
                Nombre = genero.Nombre
            };

            return TypedResults.Created($"/generos/{id}", generoDTO);
        }

        static async Task<Results<NoContent, NotFound>> ActualizarGenero(int id, CrearGeneroDto crearGeneroDTO, IRepositorioGeneros repositorio, IOutputCacheStore outputCacheStore)
        {
            var existe = await repositorio.Existe(id);

            if (!existe)
            {
                return TypedResults.NotFound();
            }

            var genero = new Genero
            { Id = id,
               Nombre = crearGeneroDTO.Nombre
            };

            await repositorio.Actualizar(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> BorrarGenero(int id, IRepositorioGeneros repositorio, IOutputCacheStore outputCacheStore)
        {
            var existe = await repositorio.Existe(id);
            if (!existe)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();
        }
    }
}
