# Quake Log Parser

## Descrição

Este projeto é uma solução para o desafio de parser de logs do Quake 3 Arena. Ele lê o arquivo `games.log` gerado pelo servidor do jogo, processa os dados de cada partida e expõe uma API RESTful para consulta dos relatórios de jogos, incluindo total de kills, lista de jogadores e kills por jogador, seguindo todas as regras do desafio.

---

## Funcionalidades

- **Parser de log:** Lê e interpreta o arquivo `games.log`, agrupando eventos por partida.
- **Regras de negócio:**  
  - Quando `<world>` mata um player, ele perde -1 kill.
  - `<world>` não aparece na lista de jogadores nem no dicionário de kills.
  - `total_kills` inclui todas as mortes, inclusive as causadas por `<world>`.
- **API RESTful:** Exposição dos relatórios de jogos em formato JSON, com endpoint documentado via Swagger.
- **Testes unitários:** Cobertura dos principais fluxos e regras de negócio.
- **Documentação automática:** Swagger UI com descrições detalhadas dos endpoints.

---

## Como rodar o projeto

### 1. Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- (Opcional) Visual Studio 2022+ ou VS Code

### 2. Clone o repositório

```sh
git clone https://github.com/seu-usuario/QuakeLogParser.git
cd QuakeLogParser
```

### 3. Coloque o arquivo de log

Coloque o arquivo `games.log` na raiz do projeto ou ajuste o caminho conforme necessário.

### 4. Build e execução

```sh
dotnet build
dotnet run --project QuakeLogParser
```

A aplicação estará disponível em `https://localhost:5001` ou `http://localhost:5000`.

---

## Como usar a API

### 1. Acesse o Swagger

Abra o navegador em:
```
https://localhost:5001/swagger
```
ou
```
http://localhost:5000/swagger
```

### 2. Endpoint principal

- **GET** `/api/games/parse?filePath={CAMINHO_COMPLETO_DO_LOG}`

**Exemplo de uso:**
```
GET /api/games/parse?filePath=C:\Users\SeuUsuario\source\repos\QuakeLogParser\games.log
```

**Resposta:**
```json
{
  "game_1": {
    "total_kills": 45,
    "players": ["Dono da bola", "Isgalamido", "Zeh"],
    "kills": {
      "Dono da bola": 5,
      "Isgalamido": 18,
      "Zeh": 20
    }
  },
  ...
}
```

---

## Estrutura do Projeto

- `QuakeLogParser.Core`: Lógica de parsing, modelos e serviços.
- `QuakeLogParser`: Projeto da API (controllers, configuração).
- `QuakeLogParser.Tests`: Testes unitários.

---

## Testes

Para rodar os testes:

```sh
dotnet test
```

Os testes cobrem parsing, regras de negócio e integração básica.

---

## Explicação da Solução

- O parser lê o arquivo linha a linha, identifica o início de cada jogo e processa eventos de kill.
- Usa injeção de dependência para facilitar testes e extensibilidade.
- A API retorna os dados no formato exato pedido no desafio, com chaves `game_1`, `game_2`, etc.
- O código é modular, com separação clara entre parsing, regras de negócio e exposição via API.
- A documentação dos endpoints é gerada automaticamente via Swagger, com comentários XML no código.

---

## Contribuição

Pull requests são bem-vindos!  
Sugestões, melhorias e correções são sempre apreciadas.

---

## Licença

Este projeto é apenas para fins de avaliação técnica. 