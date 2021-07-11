# Aries MobileAgent Xamarin

This repository is the home of Aries MobileAgent Xamarin, an open source mobile agent for achieving self sovereign identity (SSI).

The primary goals of this project is to provide a common project to progress emerging community standards around mobile agents. In addition, a basic mediator agent in ASP.NET Core is included in this repo. This mediator agent can be run separately and can be configured as public access point for the mobile application. Please check the instructions below or watch the introduction demo video.

This repository contains a cross platform mobile app (iOS/Android) built using the Xamarin framework in C#. More specifically the two platform specific projects share a common UI through the use of Xamarin.Forms.

## Getting started
1. Make sure you have [indy-sdk installed](https://github.com/hyperledger/indy-sdk#installing-the-sdk)
2. Clone this repo
3. [Download](https://hyperledger-org.bintray.com/aries/) and extract static libraries required for iOS and Android projects in the `libs` folder
4. Run the mediator agent inside `mediator` folder by running `dotnet run` in terminal
5. Open aries-max-app.sln and build

For more information on the development practices featured in this repository please refer to [here](docs/development.md)

### Working with public endpoint

To work with public endpoints, use Ngrok and start it to listen on port 5000. Copy the assigned URL to the following locations
- In `mediator/Startup.cs` replace the `EndpointUri` to instruct the mediator to use this address in configuration.
- In `src/Hyperledger.Aries.Max/App.xaml.cs` replace the `EndpointUri` to configure the mobile app to use this public endpoint as mediator service

> You may have to clear previously created wallets in `~/.indy_client/wallet` for the changes to the mediator to work. Check if the mediator is configured with this address by opening it in a browser

## Demo

Watch this [Getting started with AMA-X](https://www.loom.com/share/5c52c185673046b688bdd1ef7d280185) video to learn how to run and configure the mobile agent with a publicly acessible mediator service

## Background

### SSI

SSI is a term coined by Christoper Allen in 2016 with this [article](http://www.lifewithalacrity.com/2016/04/the-path-to-self-soverereign-identity.html), it describes a new paradigm of digital identity. Its premise rests on 10 principles described in the article. In short SSI is about giving a user digital self sovereignty by inverting current approaches to digital identity. Under SSI users are given access and control of their own data and a means in which to use it in a capacity that enables and protects their digital selves.  

### Agents

Agents are essentially software processes that act on behalf of a user and facilitate the usage of their digital identity.

### Standards

There are several key standards in the SSI space but arguably the most important are that of the [DID](https://w3c-ccg.github.io/did-primer/) (as well as other associated specs) and the [Verifiable Credentials](https://w3c.github.io/vc-data-model/) specs. 

## Project Affiliation

### AgentFramework

This mobile apps primary dependency is upon the open source project [Aries Framework for .NET](https://github.com/hyperledger/aries-framework-dotnet). This framework provides the baseline components for realizing agents, AMA-X extends this framework in the context of a mobile app to realize a mobile agent.

### Indy

Much of the emerging standards AMA-X and Aries Framework implement are born out of the [Indy-Agent]() community.

## Credits

This original commit in this repository contains all contributions from the original repo, including contributions from @tplooker, @Liam-Tait, @burdettadam, @tmarkovski, @sukalpomitra, and @TelegramSam.
 This project was formerly known as Osma (Open Source Mobile Agent). Commits previous can be found in the [archived original repository](https://github.com/mattrglobal/osma).
