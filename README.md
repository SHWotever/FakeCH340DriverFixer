# FakeCH340DriverFixer

Driver fix for fake or counterfeit CH340 chips

## Background

Recently (April 2023) a driver update delivered through windows update  (3.5.2019.1) is breaking some fake CH340G chips (this chip is used to emulate an USB serial port to communicate with the Arduino main MCU).

This triggers various symptoms as impossible to open the serial port or some crashs depending of the application. Some software still work with it as chip crashes seems to be related to the port settings.

The huge problem is that windows update will reinstall again and again the offending driver, requiring some deeper manipulations to block and keep an older driver.

### How to recognize a fake CH340G chip ?

- The device will appear as a CH340 in the device manager : 

![image](https://github.com/SHWotever/SimHub/assets/2207331/760ecf63-00ad-484f-92eb-ca77c5aa0b60)

- Most of the counterfeit chips are coming unlabeled (it's a rectangular 16 pins chip) : 

![image](https://github.com/SHWotever/SimHub/assets/2207331/287f3dc5-e567-43ee-9ed5-09a95045dd3d)

- Most of the counterfeit chips are showing as ```USB2.0-Ser!``` instead of ```USB2.0-Serial``` from the USB descriptor (used by this utility to determine if the chip is likely to be fake

## Requirements

- Have one or more CH340 chips actually plugged to the computer
- Make sure to close any application using a CH340G serial port
- Administrator privileges
- Windows driver update enabled

## What it does ?

- Enumerate CH340 based serial ports and give an estimate if the chip is likely to be fake or genuine.
- Install the last known to work driver with fake chips (3.5.2019.1).
- Uninstall the driver known not to work driver with fake chips (3.8.2023.02).
- Block **all** pending windows update for CH340 driver (Hide the update in the windows update semantic) : once the driver unistalled the drivers updates will automatically be pending again allowing to block it.  

## How to use 

Download and uncompress the latest release : https://github.com/SHWotever/FakeCH340DriverFixer/releases/latest

### Apply fix
- Exectute directly ```FakeCH340DriverFixer.exe``` to automatically fix the drivers.

![image](https://github.com/SHWotever/FakeCH340DriverFixer/assets/2207331/4482a1ed-2f09-40e2-8561-c21695770552)

### Revert fix
- Exectute ```FakeCH340DriverFixer.exe unblock``` to revert the windows update changes, then you can use windows update to reinstall the latest driver update.

![image](https://github.com/SHWotever/FakeCH340DriverFixer/assets/2207331/8da63dc3-6cca-4ada-ade1-292c0029d763)

## Limitations
- It's not possible to revert reliably the driver only for a specific device instance, windows update will blindly update all of them automatically.
- The fixer only targets a specific driver for uninstallation, if the driver gets a new update this fix will have to be updated.
- Manipulating windows update might make nervous some Antiviruses and trigger false positives, you've been warned ;) You have all the source code to check yourself what it does.
