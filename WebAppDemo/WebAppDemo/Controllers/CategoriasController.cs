using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppDemo.Context;
using WebAppDemo.Models;

namespace WebAppDemo.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext context;

        public CategoriasController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet("produtos")]
        public ActionResult<IEnumerable<Categoria>> GetAll()
        {
            return this.context.Categorias.Include(item => item.Produtos).ToList();
        }

        [HttpGet]
        public ActionResult<IEnumerable<Categoria>> Get()
        {
            try
            {
                //Desabilitar o rastreamento das consultas (aumenta o desempenho da api)
                return this.context.Categorias.AsNoTracking().ToList();
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro ao tentar obter as categorias do banco de dados");
            }
        }

        [HttpGet("{id}", Name = "ObterCategoria")]
        public ActionResult<Categoria> Get(int id)
        {
            try
            {
                var categoria = this.context.Categorias.AsNoTracking().FirstOrDefault(p => p.CategoriaId == id);
                if (categoria == null)
                    return NotFound($"A categoria com id={id} não foi encontrada");
                return categoria;
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro ao tentar obter as categorias do banco de dados");
            }
        }

        [HttpPost]
        public ActionResult Post([FromBody] Categoria categoria)
        {
            this.context.Add(categoria);
            this.context.SaveChanges();
            return new CreatedAtRouteResult("ObterCategoria", new {id = categoria.CategoriaId}, categoria);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Categoria categoria)
        {
            //altera o estado da entidade oara modified
            this.context.Entry(categoria).State = EntityState.Modified;
            //aplica as alterações no banco
            this.context.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult<Categoria> Delete(int id)
        {
            //busca direto no banco de dados
            var categoria = this.context.Categorias.FirstOrDefault(p => p.CategoriaId == id);

            //1º busca em memória, se não achar busca no banco
            //var categoria = this.context.Categorias.Find(id);

            if (categoria == null)
                return NotFound();

            this.context.Categorias.Remove(categoria);
            this.context.SaveChanges();
            return categoria;
        }
    }
}