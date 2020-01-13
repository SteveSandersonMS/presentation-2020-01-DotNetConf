dotnet publish -c Release --self-contained -r osx.10.13-x64

rm -rf bin/FlightFinder.app
mkdir bin/FlightFinder.app
mkdir bin/FlightFinder.app/Contents

cat > bin/FlightFinder.app/Contents/Info.plist <<- EOM
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple Computer//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
  <key>CFBundleGetInfoString</key>
  <string>FlightFinder</string>
  <key>CFBundleExecutable</key>
  <string>FlightFinder.Desktop</string>
  <key>CFBundleIdentifier</key>
  <string>com.your-company-name.www</string>
  <key>CFBundleName</key>
  <string>FlightFinder</string>
  <key>CFBundleIconFile</key>
  <string>mac-app-icon.icns</string>
  <key>CFBundleShortVersionString</key>
  <string>0.01</string>
  <key>CFBundleInfoDictionaryVersion</key>
  <string>6.0</string>
  <key>CFBundlePackageType</key>
  <string>APPL</string>
  <key>IFMajorVersion</key>
  <integer>0</integer>
  <key>IFMinorVersion</key>
  <integer>1</integer>
  <key>NSHighResolutionCapable</key>
  <string>true</string>
</dict>
</plist>
EOM


mkdir bin/FlightFinder.app/Contents/Resources
cp mac-app-icon.icns bin/FlightFinder.app/Contents/Resources

mkdir bin/FlightFinder.app/Contents/macOS
cp -R bin/Release/netcoreapp3.1/osx.10.13-x64/* bin/FlightFinder.app/Contents/macOS

rm -rf ~/Desktop/FlightFinder.app
mv bin/FlightFinder.app ~/Desktop
touch ~/Desktop/FlightFinder.app
