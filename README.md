# KNRAngler v. 7.3

KNRAngler is a front-end program to monitor, debug, and control robot systems in [KNR Okoń simulation](https://github.com/knr-auv/simulation)

# Features

- HUD - simple aviation inspired HUD containing: depth, attitude, battery, heading
- Livefeed - View RGB and stereo camera streams in real time
- Cloud - 3D visualization of the stereo camera point cloud, combined with the RGB frame
- Console - debugging and sending custom messages
- Keyboard Control - directly control robot with a keyboard
- Joystick Control (WIP) - directly control robot with a joystick
- Detected Objects (WIP) - visualise detected objects bounding boxes

# Usage

Open KNRAngler EXE file, by default simulation IP is `localhost` TCP ports `44210` WAPI
Optional settings in Setting panel: 
- enable video feed
- enable keyboard control
- yeet network logs to console
- show depth map (enabling will obscure HUD)
Video quality controlled by the simulation settings

IMPORTANT ensure KNRAngler version supports given simulation version 

# Instalation

Download the latest .exe release from the Releases page
(or build from source)
