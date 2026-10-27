# Doceria Gourmet API — AP2

API REST desenvolvida em **ASP.NET Core Minimal API com .NET 10**, persistência em banco **SQLite via Entity Framework Core** e **frontend integrado** servido pela própria aplicação (estilo cardápio/delivery online de doceria tipo iFood).

---

## 📋 Sobre o Projeto

Este projeto é a evolução da **AP1**, adaptado para o tema de **Doceria Gourmet Online (estilo iFood)**, substituindo o armazenamento em memória pela persistência real em banco relacional SQLite via EF Core e integrando o frontend diretamente aos endpoints HTTP da API.

### Recursos implementados:
- **CRUD completo de Doces/Sobremesas** com validação de dados no backend;
- **Persistência relacional em SQLite** gerenciada via migrations do Entity Framework Core;
- **Aplicação automática de migrations e seed** de 10 doces artesanais ao iniciar a aplicação;
- **Respostas HTTP padronizadas**: `201 Created` (com header `Location`), `200 OK`, `204 No Content`, `400 Bad Request` e `404 Not Found`;
- **Frontend SPA (Single Page Application)** servido pela mesma origem da API (`wwwroot`), permitindo cadastrar, listar, filtrar, editar e excluir registros com feedback visual;
- **Collections de testes** prontas para **Bruno** e **Postman** cobrindo todos os fluxos e cenários de erro.

---

## 🛠️ Tecnologias

| Camada | Tecnologia |
|---|---|
| Backend | ASP.NET Core Minimal API (.NET 10) |
| ORM / Banco | Entity Framework Core 10 + SQLite |
| Frontend | HTML5 Semântico + CSS3 Moderno + JavaScript Vanilla |
| Testes de Contrato | Bruno Collection e Postman Collection |

---

## ⚙️ Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) instalado
- [Bruno](https://www.usebruno.com/) ou [Postman](https://www.postman.com/) (para executar a Collection de testes)
- Navegador web moderno (Chrome, Edge, Firefox)

> Não são necessários Docker, Node.js, npm ou instalação de SGBD externo.

---

## 🚀 Como Executar

### 1. Clonar ou abrir a pasta do repositório

```bash
git clone <URL_DO_REPOSITORIO>
cd <PASTA_DO_PROJETO>
```

### 2. Preparação do banco de dados (SQLite + EF Core)

As migrations do Entity Framework Core já estão versionadas no repositório na pasta `Migrations/`.

Ao iniciar a API, o método `db.Database.Migrate()` em `Program.cs` aplica as migrations e o seed inicial **automaticamente**. Não é obrigatório rodar nenhum comando manual.

Caso prefira aplicar as migrations manualmente antes de iniciar:

```bash
dotnet ef database update
```

> **Aviso:** O arquivo local do banco SQLite (`doceria.db`) e arquivos temporários (`doceria.db-wal`, `doceria.db-shm`) estão configurados no `.gitignore` para garantir que cada avaliador gere seu próprio banco limpo a partir das migrations.

### 3. Iniciar a API e o Frontend

Execute o comando no terminal:

```bash
dotnet run --urls http://localhost:5050
```

A API e o frontend ficarão disponíveis em:
👉 **`http://localhost:5050`**

---

## 🌐 Como Abrir a Interface (Frontend)

Com a API em execução, acesse pelo navegador:

```
http://localhost:5050
```

O frontend é servido automaticamente pela mesma origem da API através do middleware de arquivos estáticos (`UseDefaultFiles` e `UseStaticFiles`), localizado na pasta `wwwroot`.

### Funcionalidades do Frontend:
1. **Listagem e filtro**: Exibe os doces cadastrados em cards com fotos, categoria, sabor, descrição e preço formatado em reais (R$). Há filtro instantâneo por nome, categoria ou sabor.
2. **Cadastro (POST)**: Formulário para adicionar novos doces com validação imediata de campos obrigatórios e preço positivo.
3. **Edição (PUT)**: Ao clicar em "Visualizar / Editar", os dados do doce são carregados da API (`GET /api/doces/{id}`) para o formulário. Ao salvar, uma requisição `PUT` atualiza o registro no banco SQLite.
4. **Exclusão (DELETE)**: Botão para remover o item do banco com confirmação, atualizando a listagem em tempo real.

---

## 📡 Rotas da API

| Método | Rota | Descrição | Sucesso | Erro / Validação |
|---|---|---|---|---|
| `GET` | `/api` | Status da API | `200 OK` | — |
| `GET` | `/api/doces` | Lista todos os doces | `200 OK` | — |
| `GET` | `/api/doces/{id}` | Busca doce por ID | `200 OK` | `404 Not Found` |
| `POST` | `/api/doces` | Cadastra novo doce | `201 Created` | `400 Bad Request` |
| `PUT` | `/api/doces/{id}` | Atualiza doce existente | `200 OK` | `404 Not Found` / `400 Bad Request` |
| `DELETE` | `/api/doces/{id}` | Remove doce por ID | `204 No Content` | `404 Not Found` |

### Exemplo de corpo JSON (`POST` / `PUT`)

```json
{
  "nome": "Macaron Francês de Pistache",
  "categoria": "Sobremesas",
  "sabor": "Pistache",
  "preco": 9.50,
  "descricao": "Clássico doce francês crocante por fora e macio por dentro, com ganache de pistache.",
  "imagemUrl": "https://images.unsplash.com/photo-1569864358642-9d1684040f43?auto=format&fit=crop&w=500&q=80"
}
```

> **Nota:** O campo `imagemUrl` é opcional. Caso não seja informado, a API e o frontend atribuem uma foto padrão de alta qualidade de doceria gourmet.

---

## 🧪 Testes com Bruno e Postman

O repositório disponibiliza as collections prontas para validação dos endpoints e respostas HTTP:

### 1. Bruno (pasta `bruno/`)
1. Abra o **Bruno**;
2. Clique em **Open Collection** e selecione a pasta `bruno/` deste repositório;
3. No canto superior direito, selecione o ambiente **Local** (configurado para `http://localhost:5050`);
4. Execute as requisições na ordem numérica:
   - `00` — Status da API (`GET /api`)
   - `01` — Listar Doces (`GET /api/doces`) → `200 OK`
   - `02` — Buscar por ID (`GET /api/doces/1`) → `200 OK`
   - `03` — Criar Doce (`POST /api/doces`) → `201 Created`
   - `04` — Atualizar Doce (`PUT /api/doces/1`) → `200 OK`
   - `05` — Excluir Doce (`DELETE /api/doces/1`) → `204 No Content`
   - `06` — Buscar Doce Excluído (`GET /api/doces/1`) → `404 Not Found`
   - `07` — Buscar ID Inexistente (`GET /api/doces/9999`) → `404 Not Found`

### 2. Postman (arquivo `DoceriaApi.postman_collection.json`)
1. Abra o **Postman** e clique em **Import**;
2. Selecione o arquivo `DoceriaApi.postman_collection.json` presente na raiz do repositório;
3. Execute as requisições na ordem indicada para validar todo o contrato da API.

---

## 💾 Persistência e Prova de Reinicialização

- Todos os dados são gravados no banco SQLite local (`doceria.db`);
- Ao **reiniciar** a aplicação (parar com `Ctrl+C` e executar `dotnet run` novamente), todos os registros previamente criados ou editados continuam salvos e acessíveis tanto pela API quanto pela interface web;
- 10 doces artesanais são inseridos pelo seed da migration `BancoInicial` caso o banco seja criado do zero.

---

## 📁 Estrutura do Projeto

```
├── Migrations/                             # Migrations versionadas do Entity Framework Core
│   ├── 20261027211923_BancoInicial.cs
│   ├── 20261027211923_BancoInicial.Designer.cs
│   └── ApiDbContextModelSnapshot.cs
├── wwwroot/                                # Frontend SPA (servido pela própria API)
│   ├── index.html                          # Estrutura HTML5 semântica e acessível
│   ├── app.js                              # Lógica de integração e chamadas fetch aos endpoints
│   └── style.css                           # Estilização moderna e responsiva estilo delivery
├── bruno/                                  # Collection de testes do Bruno
│   ├── 00-status-api/
│   ├── 01-listar-doces/
│   ├── 02-buscar-doce-por-id/
│   ├── 03-criar-doce/
│   ├── 04-atualizar-doce/
│   ├── 05-excluir-doce/
│   ├── 06-buscar-doce-excluido/
│   ├── 07-buscar-doce-inexistente/
│   └── environments/Local.bru
├── DoceriaApi.postman_collection.json      # Collection de testes para o Postman
├── Program.cs                              # Configuração, Model (Doce), DbContext e Endpoints CRUD
├── DoceriaApi.csproj                       # Arquivo de projeto .NET 10
├── appsettings.json                        # Connection string do SQLite (doceria.db)
├── appsettings.Development.json
├── .gitignore                              # Ignora bin/, obj/, *.db, *.db-shm, *.db-wal
└── README.md                               # Documentação e link do vídeo da avaliação
```

---

## 🎬 Vídeo de Demonstração — AP2

> 🔗 **Link público do vídeo:**  
> **[https://youtu.be/qfxr5nxZGOU]**

### Roteiro apresentado no vídeo:
1. **Apresentação**: Tema da Doceria Online (tipo iFood), organização das pastas do backend, frontend e collections de testes.
2. **Inicialização**: Execução do `dotnet run`, aplicação das migrations automáticas e abertura da interface no navegador.
3. **Testes de Contrato (Bruno / Postman)**:
   - Execução do `POST` (retornando `201 Created` e cabeçalho `Location`);
   - Execução do `GET` da lista (retornando `200 OK`);
   - Execução do `GET` por ID (retornando `200 OK`);
   - Execução do `PUT` (retornando `200 OK`);
   - Execução do `DELETE` (retornando `204 No Content`);
   - Consulta ao ID excluído comprovando o retorno `404 Not Found`.
4. **Demonstração Integrada no Navegador**:
   - Cadastro de um doce real pela interface;
   - Atualização/edição do doce pelo formulário;
   - Exibição e filtro na listagem;
   - Exclusão do item pela tela;
   - Conferência de um dos dados via Bruno/Postman comprovando integração real de ponta a ponta.
5. **Comprovação de Persistência**:
   - Encerramento do processo da API (`Ctrl+C`);
   - Reinicialização da aplicação com `dotnet run`;
   - Recarregamento da página demonstrando que os registros persistem no banco SQLite sem perda de dados e sem dados fictícios em memória.
