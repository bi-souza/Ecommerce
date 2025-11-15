CREATE DATABASE BDEcommerce
GO

USE BDEcommerce
GO

CREATE TABLE Pessoas
(
    IdPessoa    int             primary key  identity,
    Nome        varchar(700)    not null,
    Cpf         varchar(14)     not null     unique,
    Email       varchar(100)    not null     unique,
    Telefone    varchar(14)     not null,
    DataNasc    date                null,
    SenhaHash   nvarchar(255)   not null
);

CREATE TABLE Clientes
(
    IdCliente     int           not null     primary key,    
    
    foreign key (IdCliente) references Pessoas(IdPessoa) 
);

CREATE TABLE Administradores
(
    IdAdmin       int           not null     primary key,    
    
    foreign key (IdAdmin) references Pessoas(IdPessoa)
);

CREATE TABLE Categorias
(
    IdCategoria   int          not null    primary key  identity,
    NomeCategoria varchar(400) not null      
)


CREATE TABLE Produtos
(
    IdProduto   int            not null     primary key  identity,
    CategoriaId int            not null,
    NomeProduto varchar(200)   not null     unique,
    Descricao   varchar(400)   not null,
    Preco       decimal(7,2)   not null,     
    Estoque     int            not null     default 0,
    ImagemUrl   varchar(500)   not null,   
    Destaque    int            not null     check (Destaque in (0, 1)) default 0,         
    foreign key (CategoriaId)  references  Categorias(IdCategoria)  
)


CREATE TABLE Pedidos
(
    IdPedido     int            not null     primary key  identity,
    ClienteId    int            not null, 
    DataPedido   datetime       not null     default getdate(),
    ValorTotal   decimal(7,2)   not null,
    StatusPedido varchar(100)   not null     default 'Em andamento'
    check (StatusPedido in ('Aguardando pagamento', 'Em andamento', 'Concluído', 'Cancelado')),  
    foreign key  (ClienteId)    references  Clientes(IdCliente)    
)


CREATE TABLE Pagamentos
(
    IdPagamento     int             not null     primary key  identity,
    PedidoId        int             not null,
    ValorPago       decimal(7,2)    not null,
    DataPagamento   datetime        not null     default getdate(),
    TipoPagamento   varchar(50)     not null     check (TipoPagamento in ('Pix', 'Débito', 'Crédito', 'Dinheiro')),        
    foreign key     (PedidoId)      references Pedidos(IdPedido)   
)

CREATE TABLE Avaliacoes
(	
	ClienteId      int              not null,
    ProdutoId      int              not null,
    DataAvaliacao   datetime         not null    default getdate(),
    Comentario     varchar(500)         null,
    Nota           int              not null    check (Nota between 1 and 5),    
    primary key    (ClienteId, ProdutoId),
    foreign key    (ClienteId)      references Clientes(IdCliente),
    foreign key    (ProdutoId)      references Produtos(IdProduto)
)

CREATE TABLE ItensPedido
(	
	PedidoId       int              not null,
    ProdutoId      int              not null,
    Quantidade     int              not null,
    PrecoUnit      decimal(7,2)     not null,
    ValorItem      decimal(7,2)     not null,    
    primary key    (PedidoId, ProdutoId),
    foreign key    (PedidoId)       references Pedidos(IdPedido),
    foreign key    (ProdutoId)      references Produtos(IdProduto)
)


INSERT INTO Categorias (NomeCategoria) VALUES ('Chás e Infusões');
INSERT INTO Categorias (NomeCategoria) VALUES ('Temperos e Especiarias');
INSERT INTO Categorias (NomeCategoria) VALUES ('Suplementos Naturais');
INSERT INTO Categorias (NomeCategoria) VALUES ('Queijos e Laticínios');
INSERT INTO Categorias (NomeCategoria) VALUES ('Vinhos e Bebidas');
INSERT INTO Categorias (NomeCategoria) VALUES ('Grãos e Cereais');

INSERT INTO Produtos (NomeProduto, Descricao, Preco, Estoque, ImagemUrl, Destaque, CategoriaId) 
VALUES ('Chá de Camomila', 'Flores de camomila secas para infusão. Pacote 30g.', 14.50, 50, '/images/produtos/cha-de-camomila.jpg', 0, 1);

INSERT INTO Produtos (NomeProduto, Descricao, Preco, Estoque, ImagemUrl, Destaque, CategoriaId) 
VALUES ('Chá Verde Tostado (Hojicha)', 'Chá verde japonês com baixo teor de cafeína. Pacote 50g.', 22.90, 30, '/images/produtos/cha-verde-tostado.jpg', 0, 1);

INSERT INTO Produtos (NomeProduto, Descricao, Preco, Estoque, ImagemUrl, Destaque, CategoriaId) 
VALUES ('Cúrcuma Pura (Açafrão-da-terra)', 'Raiz de cúrcuma moída pura. Pacote 100g.', 9.80, 100, '/images/produtos/curcuma.jpg', 0, 2);

INSERT INTO Produtos (NomeProduto, Descricao, Preco, Estoque, ImagemUrl, Destaque, CategoriaId) 
VALUES ('Páprica Defumada', 'Páprica doce defumada (Pimentón). Pacote 75g.', 18.00, 45, '/images/produtos/paprica-defumada.jpg', 1, 2);

INSERT INTO Produtos (NomeProduto, Descricao, Preco, Estoque, ImagemUrl, Destaque, CategoriaId) 
VALUES ('Spirulina em Cápsulas', 'Suplemento de microalga Spirulina. 60 cápsulas de 500mg.', 49.90, 25, '/images/produtos/spirulina-capsulas.jpg', 0, 3);

INSERT INTO Produtos (NomeProduto, Descricao, Preco, Estoque, ImagemUrl, Destaque, CategoriaId) 
VALUES ('Cloreto de Magnésio', 'Cloreto de Magnésio P.A. em pó. Embalagem 100g.', 15.00, 60, '/images/produtos/cloreto-magnesio.jpg', 0, 3);

INSERT INTO Produtos (NomeProduto, Descricao, Preco, Estoque, ImagemUrl, Destaque, CategoriaId) 
VALUES ('Queijo Minas Artesanal', 'Queijo curado artesanal da região do Serro-MG. Peça aprox. 500g.', 38.00, 20, '/images/produtos/queijo-minas.jpg', 1, 4);

INSERT INTO Produtos (NomeProduto, Descricao, Preco, Estoque, ImagemUrl, Destaque, CategoriaId) 
VALUES ('Vinho Tinto', 'Vinho tinto seco orgânico nacional. Garrafa 750ml.', 72.00, 15, '/images/produtos/vinho-tinto.jpg', 0, 5);

INSERT INTO Produtos (NomeProduto, Descricao, Preco, Estoque, ImagemUrl, Destaque, CategoriaId) 
VALUES ('Quinoa Real em Grãos', 'Grãos de Quinoa Real orgânica. Pacote 250g.', 19.90, 40, '/images/produtos/quinoa-graos.jpg', 1, 6);


--INSERIR UM ADMIN
DECLARE @NovoAdminId INT;

-- 1. INSERIR NA TABELA BASE: PESSOA
INSERT INTO Pessoas 
(Nome, Cpf, Email, Telefone, DataNasc, SenhaHash)
VALUES
('Admin', '99988877788', 'admin@admin.com', '11999998888', '1985-05-15', 'admin456');

-- CAPTURAR O ID GERADO PARA A PESSOA
SET @NovoAdminId = SCOPE_IDENTITY();

-- 2. INSERIR NA TABELA DE ESPECIALIZAÇÃO: ADMINISTRADOR
INSERT INTO Administradores (IdAdmin)
VALUES (@NovoAdminId); 

-- VERIFICAÇÃO (OPCIONAL)
SELECT 'Administrador Inserido com sucesso!' AS Status, @NovoAdminId AS IdPessoa_IdAdmin;


--INSERIR UM CLIENTE
DECLARE @NovoClienteId INT;

-- 1. INSERIR NA TABELA BASE: PESSOA
INSERT INTO Pessoas 
(Nome, Cpf, Email, Telefone, DataNasc, SenhaHash)
VALUES
('Lúcia Helena', '11122233355', 'lucia@cliente.com', '11987654321', '1950-10-25', 'senha123');

-- CAPTURAR O ID GERADO PARA A PESSOA
SET @NovoClienteId = SCOPE_IDENTITY();

-- 2. INSERIR NA TABELA DE ESPECIALIZAÇÃO: CLIENTE
INSERT INTO Clientes (IdCliente)
VALUES (@NovoClienteId);

-- VERIFICAÇÃO (OPCIONAL)
SELECT 'Cliente Inserido com sucesso!' AS Status, @NovoClienteId AS IdPessoa_IdCliente;