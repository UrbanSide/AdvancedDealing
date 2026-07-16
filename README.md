# AdvancedDealing Community

> Unofficial community-maintained fork of ManZune's AdvancedDealing.
> This branch adds Schedule I 0.4.5f2 compatibility, editable localization, separate product/cash dead drops, and legacy save migration. If no cash dead drop is selected, automatic delivery is disabled, cash stays on the dealer, and it is collected through the normal game interaction.

Russian build and migration guide: `README_COMMUNITY_RU.md`.


[![GitHub Release](https://img.shields.io/github/v/release/UrbanSide/AdvancedDealing-Community?include_prereleases&sort=semver)](https://github.com/UrbanSide/AdvancedDealing-Community/releases)

A MelonLoader mod for Schedule1 that changes your dealers behavior, let's you communicate with them via messages app, automates the cash collection process and makes your life (hopefully) a lot easier.

## Features

### Implemented

* Optional automatic cash delivery to a separately selected cash dead drop; without one, cash remains on the dealer for normal manual collection
* Communicate with dealers via messaging app
* Product pickup at a separately selected product dead drop
* ~~Loyality mode~~ (Temporary removed)
* Allow more customers per dealer
* Add item slots to your dealers
* Negotiate cut %
* Access dealer inventories from everywhere
* Change speed multiplier
* Fire your dealers
* Fully compatible with Mod Manager
* Multiplayer ready
* Il2Cpp & Mono source support
* Editable JSON localization with English fallback
* Legacy single-dead-drop save migration


### Planned

* Change dealer signing fee
* Customizable quality bonus
* More deal related actions and behavior for dealers and customers

<sub>Any ideas? Let me know!</sub>

# Bugs & Issues

If you're running into any bugs or issues using this mod, use the Issues section of the community fork. For upstream history, see the original ManZune repository.

# Set Up

### Manual Installation
1. Install [MelonLoader﻿](https://melonwiki.xyz) Version 0.7 or higher
2. Download the latest release of this mod
3. Unzip the archive and copy the *DLL file* from the archives **Mods** folder to the games **Mods** folder
4. Launch the game and enjoy

#### Which DLL file to choose?

 - For **Il2Cpp** (Default): ``AdvancedDealing.Il2Cpp.dll``
 - For **Mono** (Alternate): ``AdvancedDealing.Mono.dll``

### Configuration

The config file will be generated after the first launch after mod installation and can be found inside the UserData folder.
Savegame related settings can be applied ingame by using the messaging app on your phone.

### Multiplayer behavior

The followeing settings are restricted to change if you are in a multiplayer session:
* AccessInventory
* SettingsMenu
* NegotiationModifier
