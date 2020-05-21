# RemoteBotConnector
A tool that redirects incomming connections to open PhBot instances

This tool is meant to run in a VM, a second PC or a VPS/root server.
Setup Gateway.txt / Agent.txt to your current network settings.
Check your firewall rules, the host machine has to be able to connect to a fixed open port.

You have to copy phBot and the private silkroad game client on this machine too.
Startup phBot normally and setup your game but do not launch the gameclient. Just open phBot and select your private server and leave it.

Use edxLoader(or any other similar software) and redirect the gateway to your botmachine.

I used Net Framework 3.5 on purpose, because your can use the software easier with wine. But this tool will not work with linux, because "GetExtendedTcpTable" is not fully supported by wine. I'm not able to get the PID of the opened port.
