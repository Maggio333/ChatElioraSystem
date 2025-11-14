#!/bin/bash
# Setup script for ChatElioraSystem
# Bash script to help set up the development environment

echo "🚀 ChatElioraSystem - Setup Script"
echo ""

# Check if Docker is installed
echo "📦 Checking Docker installation..."
if command -v docker &> /dev/null; then
    DOCKER_VERSION=$(docker --version)
    echo "✅ Docker found: $DOCKER_VERSION"
else
    echo "❌ Docker not found. Please install Docker from https://www.docker.com/get-started"
    exit 1
fi

# Check if .NET SDK is installed
echo "📦 Checking .NET SDK installation..."
if command -v dotnet &> /dev/null; then
    DOTNET_VERSION=$(dotnet --version)
    echo "✅ .NET SDK found: $DOTNET_VERSION"
else
    echo "❌ .NET SDK not found. Please install .NET 8.0 SDK from https://dotnet.microsoft.com/download"
    exit 1
fi

# Start Qdrant with Docker Compose
echo ""
echo "🐳 Starting Qdrant with Docker Compose..."
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR/.."
docker-compose up -d

if [ $? -eq 0 ]; then
    echo "✅ Qdrant started successfully!"
    echo "   REST API: http://localhost:6333"
    echo "   Dashboard: http://localhost:6333/dashboard"
else
    echo "❌ Failed to start Qdrant"
    exit 1
fi

# Restore NuGet packages
echo ""
echo "📥 Restoring NuGet packages..."
dotnet restore

if [ $? -eq 0 ]; then
    echo "✅ Packages restored successfully!"
else
    echo "❌ Failed to restore packages"
    exit 1
fi

# Build solution
echo ""
echo "🔨 Building solution..."
dotnet build

if [ $? -eq 0 ]; then
    echo "✅ Build successful!"
else
    echo "❌ Build failed"
    exit 1
fi

# Run tests
echo ""
echo "🧪 Running tests..."
dotnet test

if [ $? -eq 0 ]; then
    echo "✅ All tests passed!"
else
    echo "⚠️  Some tests failed"
fi

echo ""
echo "✨ Setup complete!"
echo ""
echo "📝 Next steps:"
echo "   1. Install LM Studio from https://lmstudio.ai/"
echo "   2. Load a model in LM Studio"
echo "   3. Start LM Studio server on port 8123"
echo "   4. Run the application from your IDE or: dotnet run --project ChatElioraSystem"
echo ""

