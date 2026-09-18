#!/usr/bin/env bash
set -euo pipefail

sqlcmd=/opt/mssql-tools18/bin/sqlcmd
connection=(-S "${SQLSERVER_HOST}" -U sa -P "${MSSQL_SA_PASSWORD}" -C -b)

"${sqlcmd}" "${connection[@]}" -d master -Q \
  "IF DB_ID(N'HomeLibrary') IS NULL CREATE DATABASE [HomeLibrary];"

"${sqlcmd}" "${connection[@]}" -d HomeLibrary -Q \
  "IF OBJECT_ID(N'dbo.SchemaMigrations', N'U') IS NULL
   CREATE TABLE dbo.SchemaMigrations
   (
       [Version] NVARCHAR(255) NOT NULL CONSTRAINT PK_SchemaMigrations PRIMARY KEY,
       AppliedAt DATETIME2 NOT NULL
   );"

for migration in /database/migrations/*.sql; do
  migration_version="${migration##*/}"
  applied=$("${sqlcmd}" "${connection[@]}" -d HomeLibrary -h -1 -W \
    -Q "SET NOCOUNT ON; SELECT COUNT(1) FROM dbo.SchemaMigrations WHERE [Version] = N'\$(MigrationVersion)';" \
    -v "MigrationVersion=${migration_version}" | tr -d '\r[:space:]')

  if [[ "${applied}" == "0" ]]; then
    echo "Applying ${migration_version}"
    "${sqlcmd}" "${connection[@]}" -d HomeLibrary -i "${migration}"
    "${sqlcmd}" "${connection[@]}" -d HomeLibrary \
      -Q "INSERT INTO dbo.SchemaMigrations ([Version], AppliedAt) VALUES (N'\$(MigrationVersion)', SYSUTCDATETIME());" \
      -v "MigrationVersion=${migration_version}"
  else
    echo "Skipping ${migration_version}; already applied"
  fi
done
