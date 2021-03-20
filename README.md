# ThingsBoardDotNet

**ThingsBoardDotNet** is a .Net library designed to work with [ThingsBoard](https://thingsboard.io/) platform and allows you to monitor and control IoT devices. At that moment ThingsBoardDotNet works on the following .Net platforms : 
- .Net Core 3.1
- [.Net nanoFramework](https://nanoframework.net/)

**What is ThingsBoard ?**
ThingsBoard is an open-source IoT platform that enables rapid development, management, and scaling of IoT projects.
We recommend to review [what-is-thingsboard](https://thingsboard.io/) page and [getting-started guide](https://thingsboard.io/docs/getting-started-guides/helloworld/).

This sample application will allow you to control GPIO of your .Net device using ThingsBoard RPC widgets.

We will use ESP32 Dev Kit board with .Net nanoFramework and .Net Core console application that will connect to ThingsBoard server via MQTT and listen to RPC commands. Sample application code written in C# language which is quite simple and easy to understand.  All operations are visualized using a built-in customizable dashboard.

**Prerequisites**

Any hardware and software platform compatible with [.Net Core](https://docs.microsoft.com/en-us/dotnet/core/install/) and [.Net nanoFramework](https://github.com/nanoframework/nf-interpreter).

You will need to have ThingsBoard server up and running. The easiest way is to use [Live Demo  ](https://demo.thingsboard.io/signupserver). The alternative option is to install ThingsBoard using [Installation Guide](https://thingsboard.io/docs/user-guide/install/installation-options/).

Once you complete this sample/tutorial, you will see your sensor data on the following dashboard.