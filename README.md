# Aries MobileAgent Xamarin

This repository is the home of Aries MobileAgent Xamarin, an open source mobile agent for achieving self sovereign identity (SSI). This project was formerly known as Osma (Open Source Mobile Agent). Commits previous can be found in the [archived original repository](https://github.com/mattrglobal/osma).

This original commit in this repository contains all contributions from the original repo, including contributions from @tplooker, @Liam-Tait, @burdettadam, @tmarkovski, @sukalpomitra, and @TelegramSam.

Note: Not all references to osma have been changed. Please change them as you find them, PRs accepted.

The primary goals of this project is to provide a common project to progress emerging community standards around mobile agents.

This repository contains a cross platform mobile app (iOS/Android) built using the Xamarin framework in C#. More specifically the two platform specific projects share a common UI through the use of Xamarin.Forms.

## Background

### SSI

SSI is a term coined by Christoper Allen in 2016 with this [article](http://www.lifewithalacrity.com/2016/04/the-path-to-self-soverereign-identity.html), it describes a new paradigm of digital identity. Its premise rests on 10 principles described in the article. In short SSI is about giving a user digital self sovereignty by inverting current approaches to digital identity. Under SSI users are given access and control of their own data and a means in which to use it in a capacity that enables and protects their digital selves.  

### Agents

Agents are essentially software processes that act on behalf of a user and facilitate the usage of their digital identity.

### Standards

There are several key standards in the SSI space but arguably the most important are that of the [DID](https://w3c-ccg.github.io/did-primer/) (as well as other associated specs) and the [Verifiable Credentials](https://w3c.github.io/vc-data-model/) specs. 

## Project Affiliation

### AgentFramework

This mobile apps primary dependency is upon the open source project [AgentFramework](https://github.com/streetcred-id/agent-framework). This framework provides the baseline components for realizing agents, AMA-X extends this framework in the context of a mobile app to realize a mobile agent.

### Indy

Much of the emerging standards AMA-X and AgentFramework implement are born out of the [Indy-Agent]() community.

## Getting started
1. Clone it locally,
2. Run `git lfs pull` in order to pull the dependent native libraries with LFS. If you do not have this installed please refer to [here](docs/development.md)  
3. Open osma-mobile-app.sln and build!

For more information on the development practices featured in this repository please refer to [here](docs/development.md)

## A Quick Demo

The following demo describes how you can connect with another agent.

1. Clone [AgentFramework](https://github.com/streetcred-id/agent-framework)
2. From the `/scripts` folder in the repository run `./start-web-agents.sh` - Note this shell script relies on [ngrok](https://ngrok.com/) to run the agents on a publicly accessible addresses. Please ensure that your ngrok version is 2.3.28 or higher. You may get an error if the port stated is protected or in use - curl: (7) Failed to connect to localhost port <port>: Connection refused. If this happens simply scan for an open port and change the port number for web_addr in `/scripts/web-agents-ngrok-config.yaml` and also on line number 6 of `/scripts/start-web-agents.sh`. For Macbook open network utility to scan ports.
3. Note the public URL's that are outputed by the script with the following text `Starting Web Agents with public urls http://... http://...`
4. Browse to one of the urls noted above.
5. In the rendered UI, click `Connections`->`Create Invitation`, a QR code should be displayed.
6. In the osma mobile app, deployed to a mobile phone, click the connect button in the top right corner of the connections tab.
7. Scan the QR code rendered in the browser with the osma mobile app and click connect in the rendered UI.
8. Congrats, you should now be redirected in the mobile app back to the connections page showing a new connection with the AgentFramework web agent! 


