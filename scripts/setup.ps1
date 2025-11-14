# Setup script for ChatElioraSystem
# PowerShell script to help set up the development environment

Write-Host "🚀 ChatElioraSystem - Setup Script" -ForegroundColor Cyan
Write-Host ""

# Check if Docker is installed
Write-Host "📦 Checking Docker installation..." -ForegroundColor Yellow
try {
    $dockerVersion = docker --version
    Write-Host "✅ Docker found: $dockerVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker not found. Please install Docker Desktop from https://www.docker.com/products/docker-desktop" -ForegroundColor Red
    exit 1
}

# Check if .NET SDK is installed
Write-Host "📦 Checking .NET SDK installation..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "✅ .NET SDK found: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ .NET SDK not found. Please install .NET 8.0 SDK from https://dotnet.microsoft.com/download" -ForegroundColor Red
    exit 1
}

# Start Qdrant with Docker Compose
Write-Host ""
Write-Host "🐳 Starting Qdrant with Docker Compose..." -ForegroundColor Yellow
Set-Location $PSScriptRoot\..
docker-compose up -d

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Qdrant started successfully!" -ForegroundColor Green
    Write-Host "   REST API: http://localhost:6333" -ForegroundColor Cyan
    Write-Host "   Dashboard: http://localhost:6333/dashboard" -ForegroundColor Cyan
} else {
    Write-Host "❌ Failed to start Qdrant" -ForegroundColor Red
    exit 1
}

# Restore NuGet packages
Write-Host ""
Write-Host "📥 Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Packages restored successfully!" -ForegroundColor Green
} else {
    Write-Host "❌ Failed to restore packages" -ForegroundColor Red
    exit 1
}

# Build solution
Write-Host ""
Write-Host "🔨 Building solution..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Build successful!" -ForegroundColor Green
} else {
    Write-Host "❌ Build failed" -ForegroundColor Red
    exit 1
}

# Run tests
Write-Host ""
Write-Host "🧪 Running tests..." -ForegroundColor Yellow
dotnet test

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ All tests passed!" -ForegroundColor Green
} else {
    Write-Host "⚠️  Some tests failed" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "✨ Setup complete!" -ForegroundColor Green
Write-Host ""
Write-Host "📝 Next steps:" -ForegroundColor Cyan
Write-Host "   1. Install LM Studio from https://lmstudio.ai/" -ForegroundColor White
Write-Host "   2. Load a model in LM Studio" -ForegroundColor White
Write-Host "   3. Start LM Studio server on port 8123" -ForegroundColor White
Write-Host "   4. Run the application from Visual Studio or: dotnet run --project ChatElioraSystem" -ForegroundColor White
Write-Host ""

