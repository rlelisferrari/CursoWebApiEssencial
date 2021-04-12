using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppDemo.Migrations
{
    public partial class populaDb : Migration
    {
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("Insert into Categorias(Nome,ImagemUrl) Values ('Frutos do Mar','C:\\Users\\Ferrari\\Pictures\\fotos\\2.jpg')");
            mb.Sql("Insert into Categorias(Nome,ImagemUrl) Values ('Carnes Bovina','C:\\Users\\Ferrari\\Pictures\\fotos\\bike.jpg')");

            var categoriaCarnesId = mb.Sql("Select CategoriaId from Categorias where Nome = 'Carnes Bovina'");
            mb.Sql("Insert into Produtos(Nome,Descricao,Preco,ImagemUrl,Estoque,DataCadastro, CategoriaId) Values ('Camarão','1 kg camarão limpo', 50.00,'imagem',10,now(),(Select CategoriaId from Categorias where Nome = 'Frutos do Mar'))");

            mb.Sql("Insert into Produtos(Nome,Descricao,Preco,ImagemUrl,Estoque,DataCadastro, CategoriaId) Values ('Picanha','1 kg picanha', 39.00,'imagem',10,now(),(Select CategoriaId from Categorias where Nome = 'Carnes Bovina'))");
        }

        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("Delete from Categorias");
            mb.Sql("Delete from Produtos");
        }
    }
}
