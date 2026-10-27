using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DoceriaApi.Migrations
{
    /// <inheritdoc />
    public partial class BancoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Doces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Categoria = table.Column<string>(type: "TEXT", nullable: false),
                    Sabor = table.Column<string>(type: "TEXT", nullable: true),
                    Preco = table.Column<decimal>(type: "TEXT", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false),
                    ImagemUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doces", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Doces",
                columns: new[] { "Id", "Categoria", "Descricao", "ImagemUrl", "Nome", "Preco", "Sabor" },
                values: new object[,]
                {
                    { 1, "Bolos", "Massa fofinha de cenoura mesclada com uma generosa cobertura quente de brigadeiro artesanal.", "https://images.unsplash.com/photo-1578985545062-69928b1d9587?auto=format&fit=crop&w=500&q=80", "Bolo de Cenoura com Brigadeiro", 18.50m, "Chocolate" },
                    { 2, "Brigadeiros", "Clássico brigadeiro enrolado feito com chocolate nobre belga Callebaut e confeitos crocantes.", "https://images.unsplash.com/photo-1541781774459-bb2af2f05b55?auto=format&fit=crop&w=500&q=80", "Brigadeiro Gourmet Belga", 4.50m, "Chocolate Belga" },
                    { 3, "Tortas", "Base crocante de biscoito, creme suave de queijo e deliciosa calda artesanal de morango, amora e mirtilo.", "https://images.unsplash.com/photo-1533134242443-d4fd215305ad?auto=format&fit=crop&w=500&q=80", "Cheesecake de Frutas Vermelhas", 22.00m, "Frutas Vermelhas" },
                    { 4, "Sobremesas", "Pudim lisinho, super cremoso e sem furinhos, banhado em calda de caramelo dourado.", "https://images.unsplash.com/photo-1551024709-8f23befc6f87?auto=format&fit=crop&w=500&q=80", "Pudim de Leite Condensado", 14.00m, "Tradicional" },
                    { 5, "Bolos", "Brownie com casquinha crocante e interior denso e úmido, finalizado com pedaços de nozes e Nutella.", "https://images.unsplash.com/photo-1606313564200-e75d5e30476c?auto=format&fit=crop&w=500&q=80", "Brownie com Nozes e Nutella", 16.50m, "Nutella" },
                    { 6, "Tortas", "Creme holandês aveludado, cerco de biscoitos Calypso e cobertura espelhada de ganache meio amargo.", "https://images.unsplash.com/photo-1565958011703-44f9829ba187?auto=format&fit=crop&w=500&q=80", "Torta Holandesa Especial", 19.90m, "Baunilha e Chocolate" },
                    { 7, "Donuts", "Massa leve e frita na hora, coberta com glacê rosa de morango e confeitos coloridos.", "https://images.unsplash.com/photo-1527515637462-cff94eecc1ac?auto=format&fit=crop&w=500&q=80", "Donut Glaceado com Frutas", 11.00m, "Morango" },
                    { 8, "Doces no Pote", "Camadas intercaladas de bolo branco úmido, creme de leite Ninho cremoso e morangos frescos picados.", "https://images.unsplash.com/photo-1587314168485-3236d6710814?auto=format&fit=crop&w=500&q=80", "Bolo no Pote Ninho com Morango", 15.00m, "Ninho e Morango" },
                    { 9, "Sobremesas", "Taça recheada com camadas de suspiros artesanais, geleia de morango, chantilly fresco e morangos inteiros.", "https://images.unsplash.com/photo-1488477181946-6428a0291777?auto=format&fit=crop&w=500&q=80", "Taça Gourmet de Morango e Chantilly", 24.50m, "Morango" },
                    { 10, "Cookies", "Cookie vermelho aveludado com gotas de chocolate branco e recheio cremoso de cream cheese.", "https://images.unsplash.com/photo-1499636136210-6f4ee915583e?auto=format&fit=crop&w=500&q=80", "Cookie Recheado Red Velvet", 12.00m, "Red Velvet" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Doces");
        }
    }
}
