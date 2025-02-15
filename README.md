# ScriptBot - Telegram Bot for Script Management

A Telegram bot built with .NET 8 that helps manage and upload scripts to servers. The bot supports role-based access control and provides various commands for script management and user administration.

## Features

- Role-based access control (Guest, User, Admin)
- Script generation and upload functionality
- User management and role assignment
- PostgreSQL database integration
- Secure SFTP file transfer

## Commands

### General Commands

| Command | Description             | Required Role | Parameters |
| ------- | ----------------------- | ------------- | ---------- |
| /start  | Register new user       | All roles     | None       |
| /help   | Show available commands | All roles     | None       |

### User Commands

| Command        | Description                | Required Role | Parameters                                           |
| -------------- | -------------------------- | ------------- | ---------------------------------------------------- |
| /upload_script | Upload script to server    | User, Admin   | `<appName> <appBundle> <host> <username> <password>` |
| /lastupluads   | Show last executed command | Admin         | `<count>`                                            |

### Admin Commands

| Command     | Description               | Required Role | Parameters        |
| ----------- | ------------------------- | ------------- | ----------------- |
| /users      | List all registered users | Admin         | None              |
| /assignrole | Assign role to user       | Admin         | `<chatId> <role>` |

## Command Parameters

### /upload_script

- `appName`: Name of the application
- `appBundle`: Application bundle ID (format: com.example.app)
- `host`: Server hostname
- `username`: Server username
- `password`: Server password

Example:

```bash
/upload_script MyApp com.example.myapp server.example.com user123 password123
```

### /assignrole

- `chatId`: Telegram chat ID of the target user
- `role`: Role to assign (Guest, User, or Admin)

Example:

```bash
/assignrole 123456789 Admin
```

## Role Hierarchy

1. **Guest**

   - Basic access
   - Can register using /start

2. **User**

   - All Guest permissions
   - Can upload scripts

3. **Admin**
   - All User permissions
   - Can view all users
   - Can assign roles to users

## Technology Stack

- .NET 8.0
- Entity Framework Core 9.0
- PostgreSQL (via Npgsql)
- Telegram.Bot API
- SSH.NET for SFTP operations
- ErrorOr for error handling

## Project Structure

- **ScriptBot.API**: Web API and bot configuration
- **ScriptBot.BLL**: Business logic and services
- **ScriptBot.DAL**: Data access layer and entities

## Setup

### Local Development Setup

1. Clone the repository:

```bash
git clone https://github.com/Vladipz/Telegram-Script-Bot.git
cd Telegram-Script-Bot
```

2. Create a new bot using the BotFather:

   - Open Telegram and message [@BotFather](https://t.me/BotFather)
   - Send `/newbot` command
   - Follow instructions to create bot
   - Save the API token provided

3. Configure the bot token (choose one method):

#### Using Secret Manager (Recommended)

```bash
dotnet user-secrets init --project ScriptBot.API
dotnet user-secrets set "TelegramBotToken" "YOUR_BOT_TOKEN_HERE" --project ScriptBot.API
```

#### Using appsettings.Development.json

Create or update `ScriptBot.API/appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=script_bot_db;Username=admin;Password=admin1234"
  },
  "TelegramBotToken": "YOUR_BOT_TOKEN_HERE"
}
```

4. Start required services:

```bash
docker-compose -f docker-compose.yml up -d postgres pgadmin sftp
```

5. Run the application:

```bash
cd ScriptBot.API
dotnet run
```

6. Setup webhook using localtunnel:

```bash
npm install -g localtunnel
lt --port 5076 --subdomain mytelegrambot
```

### Docker Deployment

1. Clone the repository:

```bash
git clone https://github.com/Vladipz/Telegram-Script-Bot.git
cd Telegram-Script-Bot
```

2. Create a new bot using BotFather (if not already done)

3. Create `.env` file from example:

```bash
cp .env.example .env
```

4. Update `.env` file with your bot token:

```env
# Telegram Configuration
TelegramBotToken=1234567890:ABCdefGHIjklMNOpqrsTUVwxyz

# Database Configuration
DB_HOST=postgres
DB_PORT=5432
DB_NAME=script_bot_db
DB_USER=admin
DB_PASSWORD=admin1234
```

5. Start all services:

```bash
docker-compose up -d
```

6. Setup webhook using localtunnel:

```bash
npm install -g localtunnel
lt --port 5076 --subdomain mytelegrambot
```

## TODO

- [x] Add last command
- [ ] Add help command
- [x] Add Logging
- [x] Add global error logging
- [ ] Add docker support
- [ ] Add readme documentation about the project
- [ ] Add env file with configuration
- [x] Add documentation to methods
- [ ] Add tests
- [ ] Think about using repository or context in services
