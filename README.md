
# Technical assignment at MESI

Create a .NET application that functions as both an HTTP server and client. This application will include a drag-and-drop interface for setting configuration parameters (such as inbound and outbound address/port). Position and values of parameters on drag-and-drop canvas should remain the same after the user closes the application.

*This application was developed with WPF, so it is only available on Windows platform.

## Prerequisites

Before getting started, ensure the following tools and frameworks are installed:

1.  **.NET SDK**
     - Download and install the .NET SDK from the [official .NET website](https://dotnet.microsoft.com/download).
     - Make sure the installed SDK version aligns with the target framework of your project (.NET 8).
   To verify the installation, run the following command in your terminal:
   `dotnet --version`

2.  **Visual studio 2022** (recommended)
    - Download and install the .NET SDK from the [official .NET website](https://visualstudio.microsoft.com/downloads/) and select Desktop development.

## Building instructions

First, you have to clone the repository to your machine. You can do that buy running the following command in your terminal or command prompt:

`git clone https://github.com/nacezupancic/MESI-APP.git`

When the repository is cloned to your machine, navigate to the `MESI-APP` folder.

###  Building/Running with Visual Studio
- Open the solution with Visual Studio (`MESI-APP.sln`)
- In toolbar, click **Build** -> **Buil solution (CTRL+SHIFT+B)**
- When the build succeedds, clic **Debug** -> **Start Debugging (F5)**


### Building/Running with CMD/PowerShell
- Restore dependencies:
`dotnet restore`
- Build the project:
`dotnet build`
- Navigate to the folder containing the `.csproj` file
- Run the application with this command:
`dotnet run`

## Features

- Function as HTTP Client and Server at the same time,
- Drag-and-drop UI elements across the canvas,
- Save settings (values and element positions) to File/SQLite/etc.,
- Values and Positions of canvas elements remain the same after app restart,
- Discard settings,
- Configure Server and Client (URL, Port),
- Configure HTTP request body and headers,
- Control buttons (Start/Stop server, Send request, Save settings, Reset settings)


## How it works

This application works as Windows desktop application (WPF platform). It only has one view, which is separated on two parts - _Control_ part and _Canvas_.

### Control part

This part of view contains one checkbox and 5 buttons, which are used to control the application/services.
- **Start server**: Button for starting the selfhosted HTTP server on specified url/port (fields on canvas). When server is running, this button is disabled.
- **Stop server**: Button for simply stopping the HTTP server.
- **Save settings**: Button for manually saving app state (canvas elements positions and values). On next app start, those settings will be retrieved and elements will show on previous location.
- **Reset settings**: Button which resets the values/position of the canvas elements to the initial state.
- **Send request**: Button which sends request with HTTP client (it takes data from canvas elements - url, port, body data, headers).
- **Auto save**: This is a checkbox. If it is checked, the settings are automatically saved after each element move. If unchecked, you have to manually save settings before closing the application.


### Canvas

A simple white Canvas, which contains 8 elements which are used to control the services (or display specific data about the user work). All of those elements are draggable across the canvas. You can drag it anywhere, but you can't drag it outside of canvas. If there is any previous saved settings/location, the elements will be positioned at that location.

- **Server inbound URL and Port**: URL and Port at which the server is listening for requests (e.g. url: http://localhost, Port: 65432).

- **Server inbound URL and Port**: URL and Port to which client will send the request (e.g. url: http://localhost, Port: 65432; must be set the same as the server, if you would like your request to reach the server)
- **Message Body**: If you would like to send some data to the server, you can insert it here. It should be in JSON format.
- **HTTP Headers**: If you would like to set any special HTTP headers to your request, you can add its Key and Value here.
- **Incoming requests**: Table with all requests which reached this server. For each request you can see its Method, Message, Timestamp and Headers.
- **Logs**: A simple table with some events that happend during the use of the application (when you started the server, any problem details, etc). For each of them you can see Type (Info, Error), Timestamp and message.
## Good to know

- The application was built with .NET 8 and WPF, so it is only available for Windows Platform
- You can test the HTTP Server from any other source (e.g. curl)
`curl -X POST http://localhost:65432 -d "Hello, world!"`
- For testing purposes, the Client has disabled SSL verification in case you don't have a valid certificate but would still like to test https. You can generate a development certiifcate in command line:
`netsh http add sslcert ipport=0.0.0.0:<port> certhash=ffffffffffffffffffffffffffffffffffffffff appid={00112233-4455-6677-8899-AABBCCDDEEFF}`


