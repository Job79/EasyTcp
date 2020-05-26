# EasyTcp.Examples
This project contains example usage of EasyTcp & EasyTcp.Actions & EasyTcp.Encryption <br/>
All examples are well commented to explain what is happening. <br/>
Feel free to open an issue when something is unclear or needs to be better documented.

## Actions
Contains all examples for EasyTcp.Actions

## Basic
Contains basic examples for the EasyTcpClient & EasyTcpServer. <br/>
See this folder when starting with EasyTcp

## CustomPacket
Contains examples of how to create a custom packet. <br/>
Custom packets can be send with the `Send()` function, received with `message.ToPacket<{PacketType}>`, and can be compressed/encrypted. <br/>
This is usefull when needing something more efficient then standard serialization.

## Encryption
Contains all examples for EasyTcp.Encryption

## Files
Contains examples of a basic file client/server. Demostrates the usage of the `SendStream` and `ReceiveStream` functions.

## Protocols
Contains examples of how to use different protocols with EasyTcp. <br/>
Default protocol adds two bytes (message length) in front of every message.

## Readme
Contains the examples from the README

## SpeedTest
Contains speedtests for EasyTcp
