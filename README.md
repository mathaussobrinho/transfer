# Sistema de Transferência de Arquivos via SFTP (C#)

Sistema automatizado em C# para monitorar uma pasta local e enviar arquivos para um servidor SFTP.

## Funcionamento

O programa:
1. Cria automaticamente as pastas necessárias em `C:\Transfer`
2. Monitora a pasta `C:\Transfer\A enviar` por novos arquivos
3. Envia os arquivos encontrados para o servidor SFTP configurado
4. Move os arquivos (com sucesso ou erro) para `C:\Transfer\Enviados`
5. Gera relatórios de erro na pasta `C:\Transfer` quando necessário

## Requisitos

- **.NET 6.0 SDK** ou superior ([Download](https://dotnet.microsoft.com/download))
- Windows (testado no Windows 10/11)

## Instalação

### 1. Restaurar dependências

```bash
dotnet restore
```

### 2. Configurar credenciais SFTP

Execute o programa uma vez para criar o arquivo `config.json`:

```bash
dotnet run
```

Depois edite o arquivo `C:\Transfer\config.json` com suas credenciais:

```json
{
    "sftp_host": "seu-servidor-sftp.com",
    "sftp_port": 22,
    "sftp_username": "seu_usuario",
    "sftp_password": "sua_senha",
    "sftp_remote_path": "/"
}
```

### 3. Compilar o projeto

```bash
dotnet build -c Release
```

O executável será gerado em: `bin\Release\net6.0\TransferFiles.exe`

## Uso

### Executar manualmente:

**Opção 1 - Via dotnet run:**
```bash
dotnet run
```

**Opção 2 - Executar o executável compilado:**
```bash
.\bin\Release\net6.0\TransferFiles.exe
```

**Opção 3 - Duplo clique no arquivo:**
```
executar.bat
```

### Executar como serviço (Windows):

#### Opção 1: Usando Task Scheduler (Recomendado)

1. Abra o "Agendador de Tarefas" (Task Scheduler)
2. Crie uma nova tarefa:
   - **Nome:** "Transfer Files SFTP"
   - **Gatilho:** "Ao iniciar o computador" ou "Quando o usuário fizer logon"
   - **Ação:** Iniciar programa
   - **Programa:** `C:\caminho\para\TransferFiles.exe`
   - **Diretório inicial:** `C:\caminho\para\projeto`

#### Opção 2: Atalho na pasta de Inicialização

1. Pressione `Win + R`
2. Digite `shell:startup`
3. Crie um atalho para o executável `TransferFiles.exe`

## Estrutura de Pastas

```
C:\Transfer\
├── A enviar\          # Coloque arquivos aqui para serem enviados
├── Enviados\          # Arquivos movidos após envio (sucesso ou erro)
├── config.json        # Configurações do SFTP
└── transfer_log_*.log # Logs do sistema
```

## Estrutura do Projeto

```
transfer/
├── Program.cs                 # Ponto de entrada da aplicação
├── Models/
│   └── SftpConfig.cs         # Modelo de configuração SFTP
├── Services/
│   ├── DirectoryService.cs   # Gerenciamento de pastas
│   ├── ConfigurationService.cs # Carregamento de configuração
│   ├── SftpService.cs        # Cliente SFTP
│   ├── FileWatcherService.cs # Monitoramento de arquivos
│   ├── LoggingService.cs     # Sistema de logs
│   └── ErrorReportService.cs # Geração de relatórios de erro
├── TransferFiles.csproj      # Arquivo de projeto
└── README.md                 # Este arquivo
```

## Recursos

- ✅ Monitoramento automático em tempo real usando `FileSystemWatcher`
- ✅ Processamento de arquivos existentes ao iniciar
- ✅ Movimentação automática após envio
- ✅ Relatórios de erro detalhados
- ✅ Logs diários com timestamp
- ✅ Proteção contra processamento duplicado
- ✅ Tratamento de arquivos com mesmo nome (adiciona timestamp)
- ✅ Detecção de arquivos em uso antes de processar
- ✅ Conexão SFTP persistente e gerenciamento automático

## Logs e Relatórios

- **Logs:** Arquivos `transfer_log_YYYYMMDD.log` na pasta `C:\Transfer\`
- **Relatórios de Erro:** Arquivos `erro_YYYYMMDD_HHMMSS.txt` na pasta `C:\Transfer\`

Os logs são escritos tanto no console quanto em arquivo, facilitando o acompanhamento.

## Compilar como Executável Standalone

Para criar um executável único que não precisa do .NET instalado:

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

O executável será gerado em: `bin\Release\net6.0\win-x64\publish\TransferFiles.exe`

## Solução de Problemas

### Erro: "dotnet não é reconhecido"
- Instale o .NET 6.0 SDK ou superior
- Verifique se o PATH do sistema inclui a pasta do .NET SDK

### Erro de conexão SFTP:
- Verifique as credenciais em `config.json`
- Confirme que o servidor SFTP está acessível
- Verifique firewall e porta (padrão: 22)

### Arquivo não está sendo enviado:
- Verifique os logs em `C:\Transfer\transfer_log_*.log`
- Confirme que o arquivo não está em uso por outro programa
- Verifique se o arquivo não está corrompido

### Pastas não foram criadas:
- Execute o programa com permissões de administrador
- Verifique se a unidade C: está acessível

## Dependências

- **SSH.NET** (2023.0.1) - Cliente SFTP
- **Newtonsoft.Json** (13.0.3) - Serialização JSON para configuração

## Notas de Segurança

⚠️ **Importante:** O arquivo `config.json` contém senhas em texto plano. Proteja este arquivo:
- Não compartilhe o arquivo
- Use permissões de arquivo adequadas
- Considere usar variáveis de ambiente ou criptografia para produção

## Diferenças da Versão Python

- Executável nativo do Windows (não precisa Python instalado após compilação)
- Melhor desempenho e uso de memória
- Integração nativa com Windows
- Compilável como executável standalone
