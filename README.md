# medical tr.AI.ning - Virtual Reality Runtime

<img width="2498" height="887" alt="banner" src="https://github.com/user-attachments/assets/0e34550f-a742-40e4-8212-94f8d3100ccf" />
<img width="2518" height="143" alt="contributors" src="https://github.com/user-attachments/assets/06d31904-d809-4182-b257-50dfe8cdb2d4" />



medical tr.AI.ning is a virtual reality training platform, designed for integration into the medical curriculum to enhance the clinical reasoning capabilities of future physicians.

The platform allows medical students to train clinical competencies from a first-person perspective with virtual, intelligent and interactive patients in an authentic simulated environment. Individual scenarios can be intuitively created by customizing specific parameters regarding patient, pathology and environment to support situated learning.

**This repository contains the virtual reality runtime application, used to experience medical scenarios with a virtual reality headset.**

[Read more about the project](https://medical-training-project.de/en/#project)

# Usage

## Requirements

To run the medical tr.AI.ning VR application the requirements are:
- System capable of running modern VR Applications
- VR-Headset and Controllers (application is targeted towards the Valve Index, but other HMDs can be used as well)
- SteamVR installed

## Setup

- Download the newest version from the [Releases](https://github.com/medical-tr-AI-ning/vr-runtime/releases) and extract the files.
- Once extracted, run `runtime.exe`.
- **If you are using a VR Headset which is not the Valve Index, you need to follow [this tutorial to rebind the controls in SteamVR](https://github.com/medical-tr-AI-ning/.github/wiki/How-to-use-other-VR-Headsets-(not-the-Valve-Index))**

# Development

To set up the Unity project and make contributions to the software yourself, these steps must be followed:

## Requirements

- Install `Unity Editor 2022.3.62f2` using the [Unity Hub Application](https://unity.com/download)
- In case you don't have it yet, acquire a (free) Unity license in the Unity Hub Application

## Cloning the repository

This project uses common assets which are stored in a [separate submodule](https://github.com/medical-tr-AI-ning/common-assets).
To ensure that all required assets are present in the project, you need to clone the reposity using the `--recurse-submodules` option, e.g. `
`git clone https://github.com/medical-tr-AI-ning/vr-runtime.git --recurse-submodules`

## Workflow

- Import project folder into Unity Hub using `Add project from disk` and open the project
- The main scene to run the application is located at `Scenes/General/Menu`
- The configurable medical environment is located at `Scenes/Scenarios/default`


<img width="3991" height="556" alt="sponsors" src="https://github.com/user-attachments/assets/c61efca6-6182-4ea9-9d36-694a2506dc79" />
