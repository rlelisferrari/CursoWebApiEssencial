using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppDemo.Context;
using WebAppDemo.DTOs;
using WebAppDemo.Models;

namespace WebAppDemo.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;

        public CategoriasController(AppDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("produtos")]
        public ActionResult<IEnumerable<Categoria>> GetAll()
        {
            return this.context.Categorias.Include(item => item.Produtos).ToList();
        }

        [HttpGet]
        public ActionResult<IEnumerable<CategoriaDTO>> Get()
        {
            try
            {
                //AsNoTracking: Desabilitar o rastreamento das consultas (aumenta o desempenho da api)
                var categorias = context.Categorias.AsNoTracking().ToList();
                var categoriasDto = this.mapper.Map<List<CategoriaDTO>>(categorias);
                return categoriasDto;
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro ao tentar obter as categorias do banco de dados");
            }
        }

        [HttpGet("{id}", Name = "ObterCategoria")]
        public ActionResult<CategoriaDTO> Get(int id)
        {
            try
            {
                var categoria = this.context.Categorias.AsNoTracking().FirstOrDefault(p => p.CategoriaId == id);
                if (categoria == null)
                    return NotFound($"A categoria com id={id} não foi encontrada");
                var categoriaDto = this.mapper.Map<CategoriaDTO>(categoria);
                return categoriaDto;
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Erro ao tentar obter as categorias do banco de dados");
            }
        }

        [HttpPost]
        public ActionResult Post([FromBody] CategoriaDTO categoriaDto)
        {
            var categoria = this.mapper.Map<Categoria>(categoriaDto);

            this.context.Categorias.Add(categoria);
            this.context.SaveChanges();

            var categoriaDTO = this.mapper.Map<CategoriaDTO>(categoria);

            return new CreatedAtRouteResult(
                "ObterCategoria",
                new {id = categoria.CategoriaId},
                categoriaDTO);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] CategoriaDTO categoriaDto)
        {
            if (id != categoriaDto.CategoriaId)
            {
                return BadRequest();
            }

            var categoria = this.mapper.Map<Categoria>(categoriaDto);
            
            //altera o estado da entidade oara modified
            this.context.Entry(categoria).State = EntityState.Modified;

            //aplica as alterações no banco
            this.context.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult<CategoriaDTO> Delete(int id)
        {
            //busca direto no banco de dados
            var categoria = this.context.Categorias.FirstOrDefault(p => p.CategoriaId == id);

            //1º busca em memória, se não achar busca no banco
            //var categoria = this.context.Categorias.Find(id);

            if (categoria == null)
                return NotFound();

            this.context.Categorias.Remove(categoria);
            this.context.SaveChanges();
            var categoriaDto = this.mapper.Map<CategoriaDTO>(categoria);

            return categoriaDto;
        }
    }
}