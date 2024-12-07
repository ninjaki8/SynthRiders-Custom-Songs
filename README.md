## SynthRiders - Custom Songs Downloader for Linux
This tool will check your CustomSongs folder in your Quest 3 device and compare it with the available beatmaps found in synthriderz.com, any missing entries found will be downloaded to your Quest 3 device (CustomSongs Folder).

### Requirements
- Quest 3 with Debug Mode Enabled
- Linux
- Android Tools
  
### Installation
- Install android-tools package for your linux distro, eg. **sudo pacman -S android-tools** (Arch)
- Give the file execute permissions with **chmod +x SynthMapUpdate**
- Run the file with **./SynthMapUpdate**

**Note: The first time you plugin the device to your PC you need you to accept the debug dialog through your headset in order for the adb server to detect it (this has to be done only once), after that you only plug in the usb cable and run the app. 

First Time Example
- Plug-in the device
- Run the app (it wont show the device yet but leave it running)
- Put the headset on and accept the popup debug dialog
- Remove Headset
- Close the app and run it again

