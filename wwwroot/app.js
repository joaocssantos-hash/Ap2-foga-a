/**
 * Doceria Gourmet API - Frontend Integrado ao Backend ASP.NET Core & EF Core SQLite
 * Todas as operações (Listar, Buscar por ID, Criar, Atualizar, Excluir)
 * são enviadas diretamente aos endpoints reais da API (/api/doces).
 */

const API_BASE_URL = "/api/doces";

// Imagem padrão caso o doce não tenha foto
const IMAGEM_PADRAO = "https://images.unsplash.com/photo-1578985545062-69928b1d9587?auto=format&fit=crop&w=500&q=80";

// Estado dos dados carregados da API
let doces = [];
let editandoId = null;
let timerFeedback = null;

// Elementos da interface
const form = document.getElementById("doce-form");
const formHeading = document.getElementById("form-heading");
const formSubheading = document.getElementById("form-subheading");
const btnSubmit = document.getElementById("btn-submit");
const btnCancel = document.getElementById("btn-cancel");
const listContainer = document.getElementById("doce-list");
const emptyState = document.getElementById("empty-state");
const totalCount = document.getElementById("total-count");
const filterInput = document.getElementById("filter-input");
const feedbackMessage = document.getElementById("feedback-message");

// Campos do formulário
const inputId = document.getElementById("doce-id");
const inputIdDisplay = document.getElementById("doce-id-display");
const groupIdDisplay = document.getElementById("group-id-display");
const inputNome = document.getElementById("doce-nome");
const selectCategoria = document.getElementById("doce-categoria");
const inputSabor = document.getElementById("doce-sabor");
const inputPreco = document.getElementById("doce-preco");
const inputImagem = document.getElementById("doce-imagem");
const inputDescricao = document.getElementById("doce-descricao");

// Elementos de erro
const errorNome = document.getElementById("error-nome");
const errorCategoria = document.getElementById("error-categoria");
const errorPreco = document.getElementById("error-preco");
const errorDescricao = document.getElementById("error-descricao");

/**
 * Formata um valor numérico para o padrão de moeda brasileiro (R$ 0,00)
 */
function formatarMoeda(valor) {
    return Number(valor).toLocaleString("pt-BR", {
        style: "currency",
        currency: "BRL"
    });
}

/**
 * Retorna classe CSS para o badge da categoria
 */
function getCategoryClass(categoria) {
    if (!categoria) return "cat-default";
    const normalizado = categoria
        .toLowerCase()
        .normalize("NFD")
        .replace(/[\u0300-\u036f]/g, "")
        .replace(/\s+/g, "-");
    return `cat-${normalizado}`;
}

/**
 * Exibe notificação de feedback para o usuário
 */
function exibirFeedback(texto, tipo = "info") {
    if (timerFeedback) {
        clearTimeout(timerFeedback);
    }
    feedbackMessage.className = `feedback-message ${tipo}`;
    feedbackMessage.innerHTML = `<span>${texto}</span>`;

    // Reseta para aviso padrão após 5 segundos
    timerFeedback = setTimeout(() => {
        feedbackMessage.className = "feedback-message info";
        feedbackMessage.innerHTML = "<span>Doceria conectada à API com persistência em banco de dados SQLite.</span>";
    }, 5000);
}

/**
 * Limpa mensagens de erro de validação
 */
function limparErros() {
    errorNome.textContent = "";
    errorCategoria.textContent = "";
    errorPreco.textContent = "";
    errorDescricao.textContent = "";
}

/**
 * Busca a lista de Doces da API (GET /api/doces)
 */
async function carregarDoces() {
    try {
        const resposta = await fetch(API_BASE_URL, {
            headers: {
                "Accept": "application/json"
            }
        });

        if (!resposta.ok) {
            throw new Error(`Falha ao carregar cardápio da API (HTTP ${resposta.status})`);
        }

        doces = await resposta.json();
        renderizarLista();
    } catch (erro) {
        console.error("Erro ao carregar doces:", erro);
        exibirFeedback(`Erro ao conectar à API: ${erro.message}`, "error");
    }
}

/**
 * Renderiza os cards de doces na tela
 */
function renderizarLista() {
    const termo = filterInput.value.trim().toLowerCase();
    const filtrados = doces.filter(d => {
        const nomeMatch = d.nome.toLowerCase().includes(termo);
        const categoriaMatch = d.categoria.toLowerCase().includes(termo);
        const saborMatch = d.sabor ? d.sabor.toLowerCase().includes(termo) : false;
        return nomeMatch || categoriaMatch || saborMatch;
    });

    totalCount.textContent = filtrados.length;
    listContainer.innerHTML = "";

    if (filtrados.length === 0) {
        emptyState.style.display = "block";
        return;
    }

    emptyState.style.display = "none";

    filtrados.forEach(doce => {
        const card = document.createElement("article");
        card.className = "doce-card";
        card.setAttribute("aria-label", `Doce ${doce.nome}`);

        const saborBadge = doce.sabor 
            ? `<span class="flavor-badge">${doce.sabor}</span>` 
            : "";

        const imgUrl = doce.imagemUrl || IMAGEM_PADRAO;

        card.innerHTML = `
            <div>
                <div class="card-top">
                    <span class="card-id">#${String(doce.id).padStart(3, "0")}</span>
                    <span class="card-price">${formatarMoeda(doce.preco)}</span>
                </div>
                <div class="card-image-container">
                    <img src="${imgUrl}" 
                         alt="Foto de ${doce.nome}" 
                         class="doce-image" 
                         loading="lazy"
                         onerror="this.onerror=null; this.src='${IMAGEM_PADRAO}';">
                </div>
                <h3 class="card-name">${doce.nome}</h3>
                <div class="card-tags">
                    <span class="category-badge ${getCategoryClass(doce.categoria)}">${doce.categoria}</span>
                    ${saborBadge}
                </div>
                <p class="card-desc">${doce.descricao}</p>
            </div>
            <div class="card-actions">
                <button type="button" class="btn btn-edit" data-id="${doce.id}" aria-label="Visualizar e editar ${doce.nome}">
                    Visualizar / Editar
                </button>
                <button type="button" class="btn btn-delete" data-id="${doce.id}" aria-label="Excluir ${doce.nome}">
                    Excluir
                </button>
            </div>
        `;

        // Eventos dos botões do card
        const btnEdit = card.querySelector(".btn-edit");
        btnEdit.addEventListener("click", () => carregarParaEdicao(doce.id));

        const btnDel = card.querySelector(".btn-delete");
        btnDel.addEventListener("click", () => excluirDoce(doce.id));

        listContainer.appendChild(card);
    });
}

/**
 * Busca os dados de um Doce por ID na API (GET /api/doces/{id}) e preenche o formulário
 */
async function carregarParaEdicao(id) {
    try {
        const resposta = await fetch(`${API_BASE_URL}/${id}`, {
            headers: {
                "Accept": "application/json"
            }
        });

        if (!resposta.ok) {
            throw new Error(`Doce não encontrado no banco (HTTP ${resposta.status})`);
        }

        const doce = await resposta.json();

        limparErros();
        editandoId = doce.id;

        inputId.value = doce.id;
        inputIdDisplay.value = `#${doce.id}`;
        groupIdDisplay.style.display = "flex";

        inputNome.value = doce.nome;
        selectCategoria.value = doce.categoria;
        inputSabor.value = doce.sabor || "";
        inputPreco.value = doce.preco;
        inputImagem.value = doce.imagemUrl || "";
        inputDescricao.value = doce.descricao;

        formHeading.textContent = `Visualizar / Editar Doce #${doce.id}`;
        formSubheading.textContent = "Altere os campos e clique em Salvar Alterações para persistir no banco de dados.";
        btnSubmit.textContent = "Salvar Alterações";
        btnSubmit.className = "btn btn-primary";

        inputNome.focus();
        exibirFeedback(`Doce "${doce.nome}" (#${doce.id}) carregado da API para edição.`, "info");
    } catch (erro) {
        console.error("Erro ao carregar doce para edição:", erro);
        exibirFeedback(erro.message, "error");
    }
}

/**
 * Reseta o formulário para o modo de adição
 */
function resetarFormulario() {
    limparErros();
    editandoId = null;
    form.reset();
    inputId.value = "";
    inputIdDisplay.value = "";
    groupIdDisplay.style.display = "none";
    inputImagem.value = "";

    formHeading.textContent = "Adicionar Doce ao Cardápio";
    formSubheading.textContent = "Preencha os campos para cadastrar um novo item no cardápio da doceria.";
    btnSubmit.textContent = "Cadastrar no Cardápio";
    btnSubmit.className = "btn btn-primary";
}

/**
 * Valida os campos obrigatórios
 */
function validarCampos() {
    limparErros();
    let valido = true;

    if (!inputNome.value.trim()) {
        errorNome.textContent = "O nome do doce é obrigatório.";
        valido = false;
    }

    if (!selectCategoria.value) {
        errorCategoria.textContent = "Selecione a categoria do doce.";
        valido = false;
    }

    const precoNum = parseFloat(inputPreco.value);
    if (!inputPreco.value || isNaN(precoNum) || precoNum <= 0) {
        errorPreco.textContent = "Informe um preço válido maior que zero (Ex.: 12.50).";
        valido = false;
    }

    if (!inputDescricao.value.trim()) {
        errorDescricao.textContent = "A descrição do doce é obrigatória.";
        valido = false;
    }

    return valido;
}

/**
 * Processa a submissão do formulário:
 * - Modo Adição: POST /api/doces (HTTP 201 Created)
 * - Modo Edição: PUT /api/doces/{id} (HTTP 200 OK)
 */
form.addEventListener("submit", async (e) => {
    e.preventDefault();

    if (!validarCampos()) {
        exibirFeedback("Por favor, corrija os erros indicados no formulário.", "warning");
        return;
    }

    const dados = {
        nome: inputNome.value.trim(),
        categoria: selectCategoria.value,
        sabor: inputSabor.value.trim() || null,
        preco: parseFloat(inputPreco.value),
        descricao: inputDescricao.value.trim(),
        imagemUrl: inputImagem.value.trim() || null
    };

    try {
        if (editandoId !== null) {
            // Atualização via PUT
            const resposta = await fetch(`${API_BASE_URL}/${editandoId}`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Accept": "application/json"
                },
                body: JSON.stringify(dados)
            });

            if (!resposta.ok) {
                const erroJson = await resposta.json().catch(() => ({}));
                throw new Error(erroJson.mensagem || `Falha na atualização (HTTP ${resposta.status})`);
            }

            const doceAtualizado = await resposta.json();
            exibirFeedback(`Doce "${doceAtualizado.nome}" (#${doceAtualizado.id}) atualizado com sucesso no banco de dados!`, "success");
            resetarFormulario();
            await carregarDoces();
        } else {
            // Criação via POST
            const resposta = await fetch(API_BASE_URL, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Accept": "application/json"
                },
                body: JSON.stringify(dados)
            });

            if (!resposta.ok) {
                const erroJson = await resposta.json().catch(() => ({}));
                throw new Error(erroJson.mensagem || `Falha no cadastro (HTTP ${resposta.status})`);
            }

            const doceCriado = await resposta.json();
            exibirFeedback(`Doce "${doceCriado.nome}" cadastrado com sucesso com ID #${doceCriado.id}!`, "success");
            resetarFormulario();
            await carregarDoces();
        }
    } catch (erro) {
        console.error("Erro ao enviar dados do formulário:", erro);
        exibirFeedback(`Erro: ${erro.message}`, "error");
    }
});

/**
 * Exclusão de Doce via DELETE /api/doces/{id} (HTTP 204 No Content)
 */
async function excluirDoce(id) {
    const doce = doces.find(d => d.id === id);
    const nomeDoce = doce ? doce.nome : `#${id}`;

    const confirmacao = window.confirm(`Deseja realmente remover o doce "${nomeDoce}" (#${id}) do cardápio?`);
    if (!confirmacao) return;

    try {
        const resposta = await fetch(`${API_BASE_URL}/${id}`, {
            method: "DELETE"
        });

        if (!resposta.ok && resposta.status !== 204) {
            const erroJson = await resposta.json().catch(() => ({}));
            throw new Error(erroJson.mensagem || `Falha ao excluir (HTTP ${resposta.status})`);
        }

        // Se estava editando o item excluído, reseta o formulário
        if (editandoId === id) {
            resetarFormulario();
        }

        exibirFeedback(`Doce "${nomeDoce}" (#${id}) excluído com sucesso do banco de dados.`, "warning");
        await carregarDoces();
    } catch (erro) {
        console.error("Erro ao excluir doce:", erro);
        exibirFeedback(`Erro ao excluir: ${erro.message}`, "error");
    }
}

// Botão de cancelar / limpar
btnCancel.addEventListener("click", () => {
    resetarFormulario();
    exibirFeedback("Formulário limpo.", "info");
});

// Filtro de busca em tempo real
filterInput.addEventListener("input", renderizarLista);

// Inicialização da interface: carrega os doces da API
document.addEventListener("DOMContentLoaded", () => {
    carregarDoces();
});
