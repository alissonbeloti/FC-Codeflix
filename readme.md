$env:ASPNETCORE_ENVIRONMENT='Migrations'

dotnet ef migrations add Videos -s src/FC.Codeflix.Catalog.Api/ -p src/FC.Codeflix.Catalog.Infra.Data.EF/

