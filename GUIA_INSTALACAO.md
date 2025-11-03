# Guia de Instalação Simplificado

## Passo 1: Compilar

Execute no prompt de comando (na pasta do projeto):

```bash
compilar_tudo.bat
```

Isso vai gerar:
- `bin\Release\net6.0\TransferFiles.exe` (serviço automático)
- `bin\Release\net6.0\Start.exe` (botão manual)

## Passo 2: Criar Pastas

Crie manualmente estas pastas:

```
C:\Transfer\
├── NAO MECHER\
├── A enviar\
└── Enviados\
```

## Passo 3: Copiar Executáveis

1. **TransferFiles.exe** → Copie para `C:\Transfer\NAO MECHER\`
2. **Start.exe** → Copie para a **Área de Trabalho**

## Passo 4: Configurar SFTP

1. Execute `C:\Transfer\NAO MECHER\TransferFiles.exe` uma vez
2. Isso cria o arquivo `C:\Transfer\NAO MECHER\config.json`
3. Edite o `config.json` com suas credenciais SFTP:

```json
{
    "sftp_host": "seu-servidor-sftp.com",
    "sftp_port": 22,
    "sftp_username": "seu_usuario",
    "sftp_password": "sua_senha",
    "sftp_remote_path": "/"
}
```

## Passo 5: Iniciar com Windows (Opcional)

Para o serviço iniciar automaticamente:

1. Pressione `Win + R`
2. Digite: `shell:startup`
3. Crie um **atalho** para: `C:\Transfer\NAO MECHER\TransferFiles.exe`

## Como Funciona

### Envio Automático (17h)
- `TransferFiles.exe` roda em segundo plano
- Todos os dias às **17:00** envia arquivos automaticamente
- Se o PC estiver desligado às 17h, envia no dia seguinte

### Envio Manual
- Clique duas vezes no **Start.exe** da área de trabalho
- Envia os arquivos imediatamente, sem esperar as 17h
- Não interfere com o envio automático

## Uso

1. Coloque arquivos em `C:\Transfer\A enviar\`
2. Aguarde as 17h (automático) ou clique em Start.exe (manual)
3. Arquivos enviados vão para `C:\Transfer\Enviados\`

## Verificar Logs

Logs ficam em: `C:\Transfer\transfer_log_YYYYMMDD.log`

