<h1 align="center">Roshan Timer</h1>
<p align="center">A fan-made Stream Deck plugin for Dota 2.</p>
<p align="center">
  <img src="RoshanTimer/images/pluginIcon@2x.png" width="128">
</p>

## Keep track of Roshan's respawn time and loot directly from your Stream Deck!
A Stream Deck plugin created using the [StreamDeckToolkit](https://github.com/FritzAndFriends/StreamDeckToolkit) template library.

# How to Use

| Action       | Result                                                      |
|--------------|-------------------------------------------------------------|
| Single Press | Start / Pause / Resume timer                                |
| Long Press   | Restart timer                                               |
| Double Press | Increase Roshan death count (Do this everytime Roshan dies) |

# Downloads
**IMPORTANT NOTE: Links coming soon!**
- Elgato Plugin Store (Recommended)
- [Direct Download](com.adrian-miasik.roshan-timer.streamDeckPlugin)

# Timer States
<img src="sources/roshan-timer-table.png" width="1280px">

## Default
<img src="RoshanTimer/images/actionDefaultImage.png" width="64">

- Timer has not been started. **Press on Roshan's first death to begin the timer.**

## Dead
> - **Dead** when the timer is less than 8 minutes.

<img src="RoshanTimer/images/dead0.png" width="64">

- First Death
- Roshan has previously dropped: `Aegis of the Immortal`
<br>
<br>

<img src="RoshanTimer/images/dead1.png" width="64">

- Second Death
- Roshan has previously dropped: `Aegis of the Immortal` + `Aghanims Shard` 
<br>
<br>

<img src="RoshanTimer/images/dead2.png" width="64">

- Third Death
- Roshan has previously dropped: `Aegis of the Immortal` + `Cheese` + (`Refresher Shard` OR `Aghanims Blessing`)
<br>
<br>

<img src="RoshanTimer/images/dead3.png" width="64">

- Fourth Death
- Roshan has previously dropped: `Aegis of the Immortal` + `Cheese` + `Aghanims Blessing` + `Refresher Shard`

## Maybe & Alive
> - **Maybe** when the timer is between 8-11 minutes.
> - **Alive** when the timer is 11 minutes or more.

<img src="RoshanTimer/images/maybe0.png" width="64">
<img src="RoshanTimer/images/alive0.png" width="64">

- First Death
- Roshan is going to drop: `Aegis of the Immortal` + `Aghanims Shard`
  <br>
  <br>

<img src="RoshanTimer/images/maybe1.png" width="64">
<img src="RoshanTimer/images/alive1.png" width="64">

- Second Death
- Roshan is going to drop: `Aegis of the Immortal` + `Cheese` + (`Refresher Shard` OR `Aghanims Blessing`)
  <br>
  <br>

<img src="RoshanTimer/images/maybe2.png" width="64">
<img src="RoshanTimer/images/alive2.png" width="64">

- Third Death
- Roshan is going to drop: `Aegis of the Immortal` + `Cheese` + `Aghanims Blessing` + `Refresher Shard`
  <br>
  <br>

<img src="RoshanTimer/images/maybe3.png" width="64">
<img src="RoshanTimer/images/alive3.png" width="64">

- Fourth Death
- Roshan is going to drop: `Aegis of the Immortal` + `Cheese` + `Aghanims Blessing` + `Refresher Shard`


# Contact Us / Support Line
- For inquires related to this specific plugin / repository: `roshan-timer@adrian-miasik.com`
- For inquries related to any of my stream deck plugins: `stream-deck-plugins@adrian-miasik.com`

# Legal
Copyrights and trademarks are the property of their respective owners.
- Adrian Miasik (Logo)
- Dota 2 (Logo)
- Roshan Spell Block (Skill Art)
- Aegis of the Immortal (Item Art)
- Cheese (Item Art)
- Aghanim's Shard (Item Art)
- Aghanim's Blessing (Item Art)
- Refresher Shard (Item Art)
