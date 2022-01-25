# Oxygen Not Included
Vintage Story Mod that adds a simple breathing system to the game

## Features

- Adds an air bar
- Add suffocation damage when air bar is depleted
- Air depletes while underwater
- Air depletes while being buried

## Available Config Options
```
"MaxAir": 100.0,
"AirDepletionRate": 0.05,               // percent of max air per second
"AirRegenerationRate": 0.1              // percent of max air per second
"AirBarVerticalAlignmentOffset": -10.0f // lesser number moves the bar up
```

## Todo (at some point)

- Deplete air `OnEntityReceiveDamage`
- Integrate with [xlib](https://www.vintagestory.at/forums/topic/1720-xlib-and-xskills/) to add skills
