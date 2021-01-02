# EasyTcp
<p>
  <img alt="MIT license" src="https://img.shields.io/badge/License-MIT-green.svg">
  <img alt="Nuget version" src="https://img.shields.io/nuget/v/EasyTcp">
  <img alt="Nuget downloads" src="https://img.shields.io/nuget/dt/EasyTcp">
  <img alt="GitHub starts" src="https://img.shields.io/github/stars/job79/EasyTcp">
</p>

EasyTcp is a simple and fast tcp library that handles all the repetative tasks that are normally done when working with tcp.  
It tries to make tcp simple without giving up on functionality or performance.

## Different packages

EasyTcp has multiple packages, this to keep the project maintainable and the dependencies small. Every package has his own goal and all packages are compatible with eachother.

| Package            | Functionality                     |
|--------------------|-----------------------------------|
| EasyTcp            | Base package with the networking functionality. <br> Contains support for serialisation, multiple types of framing, compression, logging, streaming and disconnect detection (with optional keep alive) |
| EasyTcp.Actions    | Support for EasyTcp to triggering specific functions with an attribute based on received data |
| EasyTcp.Encryption | Ssl support for EasyTcp and EasyTcp.Actions |

## Examples

Work in progress...
