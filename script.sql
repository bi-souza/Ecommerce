CREATE DATABASE BDEcommerce
GO

USE BDEcommerce
GO

CREATE TABLE Categoria
(
    IdCategoria   int          not null    primary key  identity,
    NomeCategoria varchar(400) not null      
)


CREATE TABLE Produto
(
    IdProduto   int            not null     primary key  identity,
    NomeProduto varchar(200)   not null     unique,
    Descricao   varchar(400)   not null,
    Preco       decimal(7,2)   not null,     
    Estoque     int            not null     default 0,
    ImagemUrl   varchar(500)   not null,   
    Destaque    int            not null     check (Destaque in (0, 1)) default 0, 
    CategoriaId int            not null,    
    foreign key (CategoriaId)  references  Categoria(IdCategoria)  
)


CREATE TABLE Cliente
(
    IdCliente   int             primary key  identity,
    NomeCliente varchar(700)    not null,
    Cpf         varchar(14)     not null     unique,
    Email       varchar(100)    not null,
    Telefone    varchar(14)     not null,
    DataNasc    date                null   
)


CREATE TABLE Pedido
(
    IdPedido     int            not null     primary key  identity,
    DataPedido   datetime       not null     default getdate(),
    ValorTotal   decimal(7,2)   not null,
    StatusPedido varchar(100)   not null     default 'Em andamento'
    check (StatusPedido in ('Aguardando pagamento', 'Em andamento', 'Concluído', 'Cancelado')),    
    ClienteId    int            not null,    
    foreign key  (ClienteId)    references  Cliente(IdCliente)    
)


CREATE TABLE Pagamento
(
    IdPagamento     int             not null     primary key  identity,
    ValorPago       decimal(7,2)    not null,
    DataPagamento   datetime        not null     default getdate(),
    TipoPagamento   varchar(50)     not null     check (TipoPagamento in ('Pix', 'Débito', 'Crédito', 'Dinheiro')),
    PedidoId        int             not null,    
    foreign key     (PedidoId)      references Pedido(IdPedido)   
)

CREATE TABLE Avalicao
(
    DataAvalicao   datetime         not null    default getdate(),
    Comentario     varchar(500)         null,
    Nota           int              not null    check (Nota between 1 and 5),
    ClienteId      int              not null,
    ProdutoId      int              not null,
    primary key    (ClienteId, ProdutoId),
    foreign key    (ClienteId)      references Cliente(IdCliente),
    foreign key    (ProdutoId)      references Produto(IdProduto)
)

CREATE TABLE ItensPedido
(
    Quantidade     int              not null,
    PrecoUnit      decimal(7,2)     not null,
    ValorItem      decimal(7,2)     not null,
    PedidoId       int              not null,
    ProdutoId      int              not null,
    primary key    (PedidoId, ProdutoId),
    foreign key    (PedidoId)       references Pedido(IdPedido),
    foreign key    (ProdutoId)      references Produto(IdProduto)
)

INSERT INTO Categoria (NomeCategoria) VALUES ('Chás e Infusões');
INSERT INTO Categoria (NomeCategoria) VALUES ('Temperos e Especiarias');
INSERT INTO Categoria (NomeCategoria) VALUES ('Suplementos Naturais');
INSERT INTO Categoria (NomeCategoria) VALUES ('Queijos e Laticínios');
INSERT INTO Categoria (NomeCategoria) VALUES ('Vinhos e Bebidas');
INSERT INTO Categoria (NomeCategoria) VALUES ('Grãos e Cereais');

INSERT INTO Produto (NomeProduto, Descricao, Preco, Estoque, ImagemUrl, Destaque, CategoriaId) 
VALUES ('Chá de Camomila', 'Flores de camomila secas para infusão. Pacote 30g.', 14.50, 50, '/images/produtos/cha-de-camomila.jpg', 0, 1);

INSERT INTO Produto (NomeProduto, Descricao, Preco, Estoque, ImagemUrl, Destaque, CategoriaId) 
VALUES ('Chá Verde Tostado (Hojicha)', 'Chá verde japonês com baixo teor de cafeína. Pacote 50g.', 22.90, 30, '/images/produtos/cha-verde-tostado.jpg', 0, 1);

INSERT INTO Produto (NomeProduto, Descricao, Preco, Estoque, ImagemUrl, Destaque, CategoriaId) 
VALUES ('Cúrcuma Pura (Açafrão-da-terra)', 'Raiz de cúrcuma moída pura. Pacote 100g.', 9.80, 100, '/images/produtos/curcuma.jpg', 0, 2);

INSERT INTO Produto (NomeProduto, Descricao, Preco, Estoque, ImagemUrl, Destaque, CategoriaId) 
VALUES ('Páprica Defumada', 'Páprica doce defumada (Pimentón). Pacote 75g.', 18.00, 45, '/images/produtos/paprica-defumada.jpg', 1, 2);

INSERT INTO Produto (NomeProduto, Descricao, Preco, Estoque, ImagemUrl, Destaque, CategoriaId) 
VALUES ('Spirulina em Cápsulas', 'Suplemento de microalga Spirulina. 60 cápsulas de 500mg.', 49.90, 25, '/images/produtos/spirulina-capsulas.jpg', 0, 3);

INSERT INTO Produto (NomeProduto, Descricao, Preco, Estoque, ImagemUrl, Destaque, CategoriaId) 
VALUES ('Cloreto de Magnésio', 'Cloreto de Magnésio P.A. em pó. Embalagem 100g.', 15.00, 60, '/images/produtos/cloreto-magnesio.jpg', 0, 3);

INSERT INTO Produto (NomeProduto, Descricao, Preco, Estoque, ImagemUrl, Destaque, CategoriaId) 
VALUES ('Queijo Minas Artesanal', 'Queijo curado artesanal da região do Serro-MG. Peça aprox. 500g.', 38.00, 20, '/images/produtos/queijo-minas.jpg', 1, 4);

INSERT INTO Produto (NomeProduto, Descricao, Preco, Estoque, ImagemUrl, Destaque, CategoriaId) 
VALUES ('Vinho Tinto', 'Vinho tinto seco orgânico nacional. Garrafa 750ml.', 72.00, 15, '/images/produtos/vinho-tinto.jpg', 0, 5);

INSERT INTO Produto (NomeProduto, Descricao, Preco, Estoque, ImagemUrl, Destaque, CategoriaId) 
VALUES ('Quinoa Real em Grãos', 'Grãos de Quinoa Real orgânica. Pacote 250g.', 19.90, 40, '/images/produtos/quinoa-graos.jpg', 1, 6);

