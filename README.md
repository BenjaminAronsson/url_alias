# URL Alias Web App

This project provides a minimal ASP.NET Core backend with an Angular frontend to manage URL aliases.

## Backend

The backend exposes a small REST API and stores aliases in `aliases.json` in the project root. Routes are configured in `AliasEndpointExtensions.cs` and operate on an `AliasService` that manages the data file.

### Run

```bash
# from the `Backend` directory
dotnet run
```

The API will be available at `http://localhost:5000`.

## Frontend

The frontend is a basic Angular application that interacts with the API.

### Run

```bash
# from the `frontend` directory
npm install
npm start  # serves the app on http://localhost:4200
```

While developing locally, the `proxy.conf.json` routes `/api` calls to the backend.

