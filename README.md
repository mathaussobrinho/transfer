# Sistema de Transferência de Arquivos via SFTP (C#)

Sistema automatizado em C# para enviar arquivos para um servidor SFTP de forma agendada (17h) ou manual.

## Estrutura de Pastas

Você deve criar manualmente a seguinte estrutura:

```
C:\Transfer\
├── NAO MECHER\          # Pasta onde fica o programa
│   ├── TransferFiles.exe    # Executável do serviço automático
│   └── config.json          # Configurações SFTP
├── A enviar\            # Coloque arquivos aqui para serem enviados
└── Enviados\            # Arquivos movidos após envio
```

## Requisitos

- **.NET 6.0 SDK** ou superior ([Download](https://dotnet.microsoft.com/download))
- Windows (testado no Windows 10/11)

## Instalação

### 1. Compilar os executáveis

```bash
compilar_tudo.bat
```

Isso vai gerar dois executáveis:
- **TransferFiles.exe** - Serviço automático (roda às 17h)
- **Start.exe** - Botão manual para envio imediato

### 2. Configurar estrutura de pastas

Crie manualmente:
- `C:\Transfer\NAO MECHER\`
- `C:\Transfer\A enviar\`
- `C:\Transfer\Enviados\`

### 3. Copiar arquivos

Copie os executáveis para:
- `TransferFiles.exe` → `C:\Transfer\NAO MECHER\`
- `Start.exe` → Área de trabalho (criar atalho)

### 4. Configurar SFTP

Execute `TransferFiles.exe` uma vez para criar o `config.json`:

```
C:\Transfer\NAO MECHER\TransferFiles.exe
```

Depois edite `C:\Transfer\NAO MECHER\config.json`:

```json
{
    "sftp_host": "seu-servidor-sftp.com",
    "sftp_port": 22,
    "sftp_username": "seu_usuario",
    "sftp_password": "sua_senha",
    "sftp_remote_path": "/"
}
```

### 5. Configurar início automático

Para o serviço rodar automaticamente:

1. Pressione `Win + R` e digite: `shell:startup`
2. Crie um atalho para `C:\Transfer\NAO MECHER\TransferFiles.exe`

Ou use o Agendador de Tarefas do Windows:
1. Abra "Agendador de Tarefas"
2. Crie nova tarefa:
   - **Nome:** Transfer Files SFTP
   - **Gatilho:** Ao iniciar o computador
   - **Ação:** Iniciar programa
   - **Programa:** `C:\Transfer\NAO MECHER\TransferFiles.exe`

## Uso

### Envio Automático (17h)

O `TransferFiles.exe` roda em segundo plano e:
- Verifica todos os dias às **17:00** se há arquivos para enviar
- Se o computador estiver desligado às 17h, envia no dia seguinte às 17h
- Processa todos os arquivos da pasta `A enviar` e move para `Enviados`

**Para iniciar o serviço:**
- Execute `TransferFiles.exe` uma vez
- Ele ficará rodando em segundo plano

### Envio Manual (Botão Start)

O `Start.exe` permite enviar arquivos **imediatamente**, sem esperar as 17h:

1. Coloque o `Start.exe` na área de trabalho
2. Clique duas vezes quando quiser enviar os arquivos manualmente
3. O envio manual **não interfere** com o agendado às 17h

## Funcionamento

1. **Arquivos na pasta "A enviar"** são processados
2. **Envio via SFTP** para o servidor configurado
3. **Arquivos movidos** para "Enviados" (sucesso ou erro)
4. **Relatórios de erro** gerados em `C:\Transfer\` quando necessário

## Logs e Relatórios

- **Logs:** `C:\Transfer\transfer_log_YYYYMMDD.log`
- **Relatórios de Erro:** `C:\Transfer\erro_YYYYMMDD_HHMMSS.txt`

## Compilar como Executável Standalone

Para criar executáveis únicos (não precisam do .NET instalado):

```bash
dotnet publish TransferFiles.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
dotnet publish StartManual.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## Solução de Problemas

### O envio automático não está funcionando
- Verifique se `TransferFiles.exe` está rodando (Gerenciador de Tarefas)
- Verifique os logs em `C:\Transfer\transfer_log_*.log`
- Certifique-se de que o serviço está configurado para iniciar com Windows

### Arquivo não está sendo enviado
- Verifique os logs
- Confirme que o arquivo não está em uso
- Use o botão `Start.exe` para tentar envio manual

### Erro de conexão SFTP
- Verifique as credenciais em `config.json`
- Confirme que o servidor SFTP está acessível
- Verifique firewall e porta

## Dependências

- **SSH.NET** (2023.0.1) - Cliente SFTP
- **Newtonsoft.Json** (13.0.3) - Serialização JSON

## Notas de Segurança

⚠️ **Importante:** O arquivo `config.json` contém senhas em texto plano. Proteja este arquivo:
- Não compartilhe o arquivo
- Use permissões de arquivo adequadas
