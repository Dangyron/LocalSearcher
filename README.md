# LocalSearcher - Local Document Search Application

![LocalSearcher](https://img.shields.io/badge/version-0.1.0-blue.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)
![React](https://img.shields.io/badge/React-19.x-blue.svg)
![TypeScript](https://img.shields.io/badge/TypeScript-4.x-blue.svg)
![Redis](https://img.shields.io/badge/Redis-7.x-red.svg)

LocalSearcher is a self-hosted document search application that allows you to quickly search through your local files using TF-IDF algorithms and Redis caching.

## Features

- **Fast Search**: TF-IDF based search algorithm for relevant results
- **Multi-format Support**: Search through `.txt`, `.md`. More file types will be supported later.
- **Real-time Indexing**: Automatic indexing of new and modified files
- **Debounced Search**: Intelligent search that waits for you to stop typing
- **Redis Caching**: Fast performance with Redis-backed index storage
- **Cross-platform**: Runs on Windows, macOS, and Linux

## Project Structure

```
LocalSearcher/
├── backend/                 # ASP.NET Core Web API
│   ├── Services/            # Business logic services
│   ├── Controllers/         # API controllers
│   ├── Models/              # Data models
│   ├── appsettings.json     # Configuration
│   └── Program.cs           # Application entry point
├── frontend/                # React TypeScript application
│   ├── src/
│   │   ├── api/             # API communication
│   │   ├── components/      # React components
│   │   ├── pages/           # Page components
│   │   ├── store/           # Zustand state management
│   │   ├── styles/          # SCSS stylesheets
│   │   └── types/           # TypeScript definitions
│   ├── package.json         # NPM dependencies
│   └── tsconfig.json        # TypeScript configuration
├── docker-compose.yml       # Docker container orchestration
└── README.md                # This file
```

## Prerequisites

Before running LocalSearcher, ensure you have the following installed:

- **.NET 8 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js** (v18 or higher) - [Download here](https://nodejs.org/)
- **Docker Desktop** - [Download here](https://www.docker.com/products/docker-desktop)
- **Redis** (can run via Docker)

## Quick Start

### Option 1: Docker Compose (Recommended)

1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd LocalSearcher
   ```  

2. Configure environment variables:
   ```bash
   cp .env.example .env
   # Edit .env with your preferred settings
   ```

3. Start the application with Docker:
   ```bash
   docker-compose up -d
   ```

4. Open your browser and navigate to `http://localhost:3000`

### Option 2: Manual Setup

#### Backend Setup

1. Navigate to the backend directory:
   ```bash
   cd backend
   ```

2. Configure the application:
    - Update `appsettings.json` with your Redis connection string and other settings.
    
   Mandatory settings
    ```json
   "ConnectionStrings": {
      "Redis": "localhost:6379"
    },
    "Config": {
      "SearchDirectory": "path/to/data/folder",
      "LogsFolder": "path/to/logs/folder",
      "FileWatchingEnabled": true
    }
   ```
3. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

4. Run the application:
   ```bash
   dotnet run
   ```

   The API will be available at `http://localhost:5000`

#### Frontend Setup

1. Navigate to the frontend directory:
   ```bash
   cd frontend
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Configure environment variables:
   ```bash
   # Edit .env to point to your backend API
   REACT_APP_API_BASE_URL=http://localhost:8090/api
   ```

4. Start the development server:
   ```bash
   npm start
   ```

5. Open your browser and navigate to `http://localhost:3000`

## API Endpoints

| Method | Endpoint | Description                                     |
|--------|----------|-------------------------------------------------|
| GET | `/api/search` | Search documents with query parameters          |
| GET | `/api/files` | Get content of a specific file (In development) |

### Search Parameters

```http
GET /api/search?query=aspnet+core&top=20
```

- `query`: Search terms (required)
- `top`: Number of results to return (optional, default: 10)

### Logs

Check the application logs for detailed error information:

- Backend logs are written to the console and can be configured in `appsettings.json`
- Frontend errors appear in the browser console

## Contributing

We welcome contributions to LocalSearcher! Please follow these steps:

1. Fork the repository
2. Create a feature branch: `git checkout -b feature-name`
3. Make your changes
4. Add tests if applicable
5. Commit your changes: `git commit -m 'Add some feature'`
6. Push to the branch: `git push origin feature-name`
7. Submit a pull request

### Development Guidelines

- Follow the existing code style and patterns
- Add TypeScript types for new features
- Update documentation for new features

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet) for the backend framework
- [React](https://reactjs.org/) for the frontend library
- [Redis](https://redis.io/) for caching and performance
- [Zustand](https://github.com/pmndrs/zustand) for state management

## Version History

- **v0.1.0** (Current)
    - Initial release with basic search functionality
    - Redis-backed indexing and caching
    - React frontend with TypeScript
    - Docker support

---

**Note**: This application is designed for local use. Ensure you have appropriate permissions to access and index the documents in the specified directory.