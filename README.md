# OpenID Connect auth using Bot Framework v4

This repository contains a quickly hacked-together demonstration of authenticating
against a OpenID Connect ([IdentityServer4](https://github.com/IdentityServer/IdentityServer4))
from Microsoft Teams via a bot written using [Bot Builder v4](https://github.com/Microsoft/BotBuilder).

**Disclaimer:** This has been produced during a hack week and not been prepared for
production release - so use it as inspiration only.

## Works in MS Teams

To test the code, you will need to deploy it to Teams - just write a manifest file (_consider
using the app designer_) and upload it as a custom app to your MS Team (_remember to allow
developer features in your Team_).

This will **not work in the _bot framework emulator_** as it handles the `signin` activity
type differently than Teams does, it requires the client to initiate a authentication flow
using `microsoftTeams.authentication.authenticate` which Teams does when it handles that 
activity type, but the emulator does not, it just opens it as if it was a `openUrl` type.

## How to configure

There are three variables in `appsettings.json` that you will need to configure:
* `LoginStartUrl` - This page initiates the authentiaction flow against your OpenID server,
    and the URL should point to where the bot is hosted
* `BotClientId` - This is the _client id_ configured in your OpenID server which should
    be configured with allowing redirects to where the bot is hosted
* `BotAuthority` - This is the base URL part for where your OpenID server is hosted 