using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Aplica automaticamente as migrations pendentes ao inicializar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
    db.Database.Migrate();
}

app.UseCors();

// Se a requisição for para a raiz e não solicitar HTML explicitamente (ex: Bruno/Postman), retorna o status JSON da API
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/" && !context.Request.Headers.Accept.ToString().Contains("text/html"))
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        await context.Response.WriteAsJsonAsync(new { mensagem = "A Doceria Gourmet API está no ar!" });
        return;
    }
    await next();
});

// Serve os arquivos estáticos do frontend da pasta wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

// Endpoint de status da API
app.MapGet("/api", () => Results.Ok(new { mensagem = "A Doceria Gourmet API está no ar!" }));

// CRUD da coleção Doces com persistência no banco SQLite via EF Core
app.MapGet("/api/doces", async (ApiDbContext db) =>
{
    var doces = await db.Doces.AsNoTracking().ToListAsync();
    var resposta = doces.Select(doce => ToDto(doce));
    return Results.Ok(resposta);
});

app.MapGet("/api/doces/{id:int}", async (int id, ApiDbContext db) =>
{
    var doce = await db.Doces.FindAsync(id);
    return doce is null
        ? Results.NotFound(new { mensagem = "Doce não encontrado." })
        : Results.Ok(ToDto(doce));
});

app.MapPost("/api/doces", async (DoceInputDto entrada, ApiDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(entrada.Nome))
        return Results.BadRequest(new { mensagem = "O nome do doce é obrigatório." });

    if (string.IsNullOrWhiteSpace(entrada.Categoria))
        return Results.BadRequest(new { mensagem = "A categoria é obrigatória." });

    if (entrada.Preco <= 0)
        return Results.BadRequest(new { mensagem = "O preço deve ser maior que zero." });

    if (string.IsNullOrWhiteSpace(entrada.Descricao))
        return Results.BadRequest(new { mensagem = "A descrição é obrigatória." });

    var doce = new Doce
    {
        Nome = entrada.Nome.Trim(),
        Categoria = entrada.Categoria.Trim(),
        Sabor = string.IsNullOrWhiteSpace(entrada.Sabor) ? null : entrada.Sabor.Trim(),
        Preco = entrada.Preco,
        Descricao = entrada.Descricao.Trim(),
        ImagemUrl = string.IsNullOrWhiteSpace(entrada.ImagemUrl) ? null : entrada.ImagemUrl.Trim()
    };

    db.Doces.Add(doce);
    await db.SaveChangesAsync();

    return Results.Created($"/api/doces/{doce.Id}", ToDto(doce));
});

app.MapPut("/api/doces/{id:int}", async (int id, DoceInputDto entrada, ApiDbContext db) =>
{
    var doce = await db.Doces.FindAsync(id);
    if (doce is null)
        return Results.NotFound(new { mensagem = "Doce não encontrado." });

    if (string.IsNullOrWhiteSpace(entrada.Nome))
        return Results.BadRequest(new { mensagem = "O nome do doce é obrigatório." });

    if (string.IsNullOrWhiteSpace(entrada.Categoria))
        return Results.BadRequest(new { mensagem = "A categoria é obrigatória." });

    if (entrada.Preco <= 0)
        return Results.BadRequest(new { mensagem = "O preço deve ser maior que zero." });

    if (string.IsNullOrWhiteSpace(entrada.Descricao))
        return Results.BadRequest(new { mensagem = "A descrição é obrigatória." });

    doce.Nome = entrada.Nome.Trim();
    doce.Categoria = entrada.Categoria.Trim();
    doce.Sabor = string.IsNullOrWhiteSpace(entrada.Sabor) ? null : entrada.Sabor.Trim();
    doce.Preco = entrada.Preco;
    doce.Descricao = entrada.Descricao.Trim();

    if (!string.IsNullOrWhiteSpace(entrada.ImagemUrl))
    {
        doce.ImagemUrl = entrada.ImagemUrl.Trim();
    }

    await db.SaveChangesAsync();

    return Results.Ok(ToDto(doce));
});

app.MapDelete("/api/doces/{id:int}", async (int id, ApiDbContext db) =>
{
    var doce = await db.Doces.FindAsync(id);
    if (doce is null)
        return Results.NotFound(new { mensagem = "Doce não encontrado." });

    db.Doces.Remove(doce);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();

// Funções e Modelos
static DoceDto ToDto(Doce doce) => new(
    doce.Id,
    doce.Nome,
    doce.Categoria,
    doce.Sabor,
    doce.Preco,
    doce.Descricao,
    string.IsNullOrWhiteSpace(doce.ImagemUrl)
        ? "https://images.unsplash.com/photo-1578985545062-69928b1d9587?auto=format&fit=crop&w=500&q=80"
        : doce.ImagemUrl);

public class Doce
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string? Sabor { get; set; }
    public decimal Preco { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? ImagemUrl { get; set; }
}

public record DoceDto(
    int Id,
    string Nome,
    string Categoria,
    string? Sabor,
    decimal Preco,
    string Descricao,
    string? ImagemUrl);

public record DoceInputDto(
    string Nome,
    string Categoria,
    string? Sabor,
    decimal Preco,
    string Descricao,
    string? ImagemUrl = null);

// DbContext do EF Core
public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {
    }

    public DbSet<Doce> Doces => Set<Doce>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Doce>().HasData(
            new Doce
            {
                Id = 1,
                Nome = "Bolo de Cenoura com Brigadeiro",
                Categoria = "Bolos",
                Sabor = "Chocolate",
                Preco = 18.50m,
                Descricao = "Massa fofinha de cenoura mesclada com uma generosa cobertura quente de brigadeiro artesanal.",
                ImagemUrl = "https://images.unsplash.com/photo-1578985545062-69928b1d9587?auto=format&fit=crop&w=500&q=80"
            },
            new Doce
            {
                Id = 2,
                Nome = "Brigadeiro Gourmet Belga",
                Categoria = "Brigadeiros",
                Sabor = "Chocolate Belga",
                Preco = 4.50m,
                Descricao = "Clássico brigadeiro enrolado feito com chocolate nobre belga Callebaut e confeitos crocantes.",
                ImagemUrl = "https://images.unsplash.com/photo-1541781774459-bb2af2f05b55?auto=format&fit=crop&w=500&q=80"
            },
            new Doce
            {
                Id = 3,
                Nome = "Cheesecake de Frutas Vermelhas",
                Categoria = "Tortas",
                Sabor = "Frutas Vermelhas",
                Preco = 22.00m,
                Descricao = "Base crocante de biscoito, creme suave de queijo e deliciosa calda artesanal de morango, amora e mirtilo.",
                ImagemUrl = "https://images.unsplash.com/photo-1533134242443-d4fd215305ad?auto=format&fit=crop&w=500&q=80"
            },
            new Doce
            {
                Id = 4,
                Nome = "Pudim de Leite Condensado",
                Categoria = "Sobremesas",
                Sabor = "Tradicional",
                Preco = 14.00m,
                Descricao = "Pudim lisinho, super cremoso e sem furinhos, banhado em calda de caramelo dourado.",
                ImagemUrl = "https://images.unsplash.com/photo-1551024709-8f23befc6f87?auto=format&fit=crop&w=500&q=80"
            },
            new Doce
            {
                Id = 5,
                Nome = "Brownie com Nozes e Nutella",
                Categoria = "Bolos",
                Sabor = "Nutella",
                Preco = 16.50m,
                Descricao = "Brownie com casquinha crocante e interior denso e úmido, finalizado com pedaços de nozes e Nutella.",
                ImagemUrl = "https://images.unsplash.com/photo-1606313564200-e75d5e30476c?auto=format&fit=crop&w=500&q=80"
            },
            new Doce
            {
                Id = 6,
                Nome = "Torta Holandesa Especial",
                Categoria = "Tortas",
                Sabor = "Baunilha e Chocolate",
                Preco = 19.90m,
                Descricao = "Creme holandês aveludado, cerco de biscoitos Calypso e cobertura espelhada de ganache meio amargo.",
                ImagemUrl = "https://images.unsplash.com/photo-1565958011703-44f9829ba187?auto=format&fit=crop&w=500&q=80"
            },
            new Doce
            {
                Id = 7,
                Nome = "Donut Glaceado com Frutas",
                Categoria = "Donuts",
                Sabor = "Morango",
                Preco = 11.00m,
                Descricao = "Massa leve e frita na hora, coberta com glacê rosa de morango e confeitos coloridos.",
                ImagemUrl = "https://images.unsplash.com/photo-1527515637462-cff94eecc1ac?auto=format&fit=crop&w=500&q=80"
            },
            new Doce
            {
                Id = 8,
                Nome = "Bolo no Pote Ninho com Morango",
                Categoria = "Doces no Pote",
                Sabor = "Ninho e Morango",
                Preco = 15.00m,
                Descricao = "Camadas intercaladas de bolo branco úmido, creme de leite Ninho cremoso e morangos frescos picados.",
                ImagemUrl = "https://images.unsplash.com/photo-1587314168485-3236d6710814?auto=format&fit=crop&w=500&q=80"
            },
            new Doce
            {
                Id = 9,
                Nome = "Taça Gourmet de Morango e Chantilly",
                Categoria = "Sobremesas",
                Sabor = "Morango",
                Preco = 24.50m,
                Descricao = "Taça recheada com camadas de suspiros artesanais, geleia de morango, chantilly fresco e morangos inteiros.",
                ImagemUrl = "https://images.unsplash.com/photo-1488477181946-6428a0291777?auto=format&fit=crop&w=500&q=80"
            },
            new Doce
            {
                Id = 10,
                Nome = "Cookie Recheado Red Velvet",
                Categoria = "Cookies",
                Sabor = "Red Velvet",
                Preco = 12.00m,
                Descricao = "Cookie vermelho aveludado com gotas de chocolate branco e recheio cremoso de cream cheese.",
                ImagemUrl = "https://images.unsplash.com/photo-1499636136210-6f4ee915583e?auto=format&fit=crop&w=500&q=80"
            }
        );
    }
}
