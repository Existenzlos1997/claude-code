#!/bin/bash
# ============================================================
# EarthUnderFreelancer - Linux/macOS Build Script
# ============================================================

echo "============================================================"
echo "  EarthUnderFreelancer Build Script"
echo "============================================================"
echo ""

# Configuration
UNITY_PATH="/Applications/Unity/Hub/Editor/2022.3.0f1/Unity.app/Contents/MacOS/Unity"
PROJECT_PATH="$(dirname "$0")/.."
BUILD_OUTPUT="$PROJECT_PATH/Builds"

# Check Unity installation
if [ ! -f "$UNITY_PATH" ]; then
    echo "[ERROR] Unity not found at $UNITY_PATH"
    echo "Please update UNITY_PATH in this script"
    exit 1
fi

echo "[1/4] Building Windows Player..."
echo "------------------------------------------------------------"

"$UNITY_PATH" \
    -projectPath "$PROJECT_PATH" \
    -executeMethod BuildScript.BuildWindowsFromCommandLine \
    -batchmode \
    -quit \
    -logFile "$BUILD_OUTPUT/build_windows.log"

if [ $? -ne 0 ]; then
    echo "[ERROR] Windows build failed. Check build_windows.log"
    exit 1
fi

echo "[OK] Windows build complete!"
echo ""

echo "[2/4] Building macOS Player..."
echo "------------------------------------------------------------"

"$UNITY_PATH" \
    -projectPath "$PROJECT_PATH" \
    -executeMethod BuildScript.BuildMacOSFromCommandLine \
    -batchmode \
    -quit \
    -logFile "$BUILD_OUTPUT/build_macos.log"

if [ $? -ne 0 ]; then
    echo "[ERROR] macOS build failed. Check build_macos.log"
    exit 1
fi

echo "[OK] macOS build complete!"
echo ""

echo "[3/4] Building Android APK..."
echo "------------------------------------------------------------"

"$UNITY_PATH" \
    -projectPath "$PROJECT_PATH" \
    -executeMethod BuildScript.BuildAndroidFromCommandLine \
    -batchmode \
    -quit \
    -logFile "$BUILD_OUTPUT/build_android.log"

if [ $? -ne 0 ]; then
    echo "[ERROR] Android build failed. Check build_android.log"
    exit 1
fi

echo "[OK] Android build complete!"
echo ""

echo "[4/4] Creating Release Package..."
echo "------------------------------------------------------------"

RELEASE_DIR="$BUILD_OUTPUT/Release"
mkdir -p "$RELEASE_DIR"

# Create macOS DMG
if [ -d "$BUILD_OUTPUT/macOS/EarthUnderFreelancer.app" ]; then
    echo "Creating macOS DMG..."
    hdiutil create -volname "EarthUnderFreelancer" \
        -srcfolder "$BUILD_OUTPUT/macOS/EarthUnderFreelancer.app" \
        -ov -format UDZO \
        "$RELEASE_DIR/EarthUnderFreelancer.dmg"
fi

# Copy Windows build
if [ -d "$BUILD_OUTPUT/Windows" ]; then
    echo "Creating Windows ZIP..."
    cd "$BUILD_OUTPUT/Windows"
    zip -r "$RELEASE_DIR/EarthUnderFreelancer_Windows.zip" .
    cd -
fi

# Copy Android APK
if [ -f "$BUILD_OUTPUT/Android/EarthUnderFreelancer.apk" ]; then
    cp "$BUILD_OUTPUT/Android/EarthUnderFreelancer.apk" "$RELEASE_DIR/"
fi

# Create checksums
echo "Generating checksums..."
cd "$RELEASE_DIR"
shasum -a 256 * > checksums.sha256
cd -

echo "[OK] Release package created!"
echo ""

echo "============================================================"
echo "  BUILD COMPLETE!"
echo "============================================================"
echo ""
echo "Output files in: $RELEASE_DIR"
ls -la "$RELEASE_DIR"
echo ""
