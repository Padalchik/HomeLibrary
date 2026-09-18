# HomeLibrary

Каркас приложения «Домашняя библиотека» на ASP.NET Core MVC и SQL Server 2022.

## Prerequisites

- Docker Desktop с Docker Compose

Для локальной сборки без Docker также нужен .NET SDK 10.

## Запуск

```sh
docker compose up --build
```

Приложение будет доступно по адресу <http://localhost:8080>. Проверка состояния: <http://localhost:8080/health>.

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
