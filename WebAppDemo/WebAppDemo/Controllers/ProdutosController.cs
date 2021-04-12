using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppDemo.Context;
using WebAppDemo.Models;

namespace WebAppDemo.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly AppDbContext context;

        public ProdutosController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet("/first")]
        public ActionResult<Produto> GetFirst()
        {
            //Desabilitar o rastreamento das consultas (aumenta o desempenho da api)
            return this.context.Produtos.FirstOrDefault();
        }

        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            //Desabilitar o rastreamento das consultas (aumenta o desempenho da api)
            return this.context.Produtos.AsNoTracking().ToList();
        }

        [HttpGet("{id}", Name = "ObterProduto")]
        public ActionResult<Produto> Get(int id)
        {
            var produto = this.context.Produtos.AsNoTracking().FirstOrDefault(p => p.ProdutoId == id);
            if (produto == null)
                return NotFound();
            return produto;
        }

        [HttpPost]
        public ActionResult Post([FromBody] Produto produto)
        {
            produto.DataCadastro = DateTime.Now;
            this.context.Add(produto);
            this.context.SaveChanges();
            return new CreatedAtRouteResult("ObterProduto", new {id = produto.ProdutoId}, produto);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Produto produto)
        {
            //altera o estado da entidade oara modified
            this.context.Entry(produto).State = EntityState.Modified;
            //aplica as alterações no banco
            this.context.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult<Produto> Delete(int id)
        {
            //busca direto no banco de dados
            var produto = this.context.Produtos.FirstOrDefault(p => p.ProdutoId == id);

            //1º busca em memória, se não achar busca no banco
            //var produto = this.context.Produtos.Find(id);

            if (produto == null)
                return NotFound();

            this.context.Produtos.Remove(produto);
            this.context.SaveChanges();
            return produto;
        }
    }
}