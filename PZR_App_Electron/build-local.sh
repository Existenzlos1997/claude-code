#!/bin/bash

echo "🚀 Building PZR App Installers..."
echo ""

# Check if Node.js is installed
if ! command -v node &> /dev/null; then
    echo "❌ Node.js is not installed!"
    echo "Please install Node.js from https://nodejs.org/"
    exit 1
fi

# Check if npm is installed
if ! command -v npm &> /dev/null; then
    echo "❌ npm is not installed!"
    exit 1
fi

echo "✅ Node.js version: $(node --version)"
echo "✅ npm version: $(npm --version)"
echo ""

# Install dependencies
echo "📦 Installing dependencies..."
npm install

if [ $? -ne 0 ]; then
    echo "❌ Failed to install dependencies!"
    exit 1
fi

echo ""
echo "🔨 Building installers..."
echo ""

# Build for current platform
PLATFORM=$(uname)

if [[ "$PLATFORM" == "Darwin"* ]]; then
    echo "🍎 Building for macOS..."
    npm run build:mac
elif [[ "$PLATFORM" == "Linux"* ]]; then
    echo "🐧 Building for Linux..."
    npm run build:linux
elif [[ "$PLATFORM" == "MINGW"* ]] || [[ "$PLATFORM" == "MSYS"* ]] || [[ "$PLATFORM" == "CYGWIN"* ]]; then
    echo "🪟 Building for Windows..."
    npm run build:win
else
    echo "⚠️ Unknown platform: $PLATFORM"
    echo "Building for all platforms..."
    npm run build
fi

if [ $? -eq 0 ]; then
    echo ""
    echo "✅ Build successful!"
    echo ""
    echo "📦 Installers are in the 'dist/' folder:"
    ls -lh dist/
else
    echo ""
    echo "❌ Build failed!"
    exit 1
fi
