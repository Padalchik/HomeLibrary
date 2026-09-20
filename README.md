# HomeLibrary

ASP.NET Core MVC приложение «Домашняя библиотека» с SQL Server, Dapper и stored procedures.

## Prerequisites

- Docker Desktop с Docker Compose

Для локальной сборки без Docker также нужен .NET SDK 10.

## Запуск

```sh
docker compose up --build
```

Команда автоматически:

- запускает SQL Server;
- создаёт и подготавливает базу данных;
- применяет database migrations;
- запускает web-приложение.

Приложение будет доступно по адресу <http://localhost:18080>. Проверка состояния: <http://localhost:18080/health>.

## Тесты

```sh
dotnet test HomeLibrary.sln --configuration Release
```

## Локальная отладка

Перед отладкой поднимите SQL Server и примените миграции:

```sh
docker compose up -d sqlserver migrations
```

Затем выберите профиль `HomeLibrary.Web` в IDE и запустите проект в режиме Debug. Приложение использует окружение `Development`, а браузер автоматически откроет <http://localhost:5080/>. HTTPS также доступен по адресу <https://localhost:7080/>.

## Остановка

```sh
docker compose down
```
