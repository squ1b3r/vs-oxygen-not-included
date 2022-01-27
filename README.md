# Oxygen Not Included
Vintage Story Mod that adds a simple breathing system to the game

## Features

- Adds an air bar
- Adds suffocation damage when air bar is depleted
- Air depletes while being underwater
- Air depletes while being buried
- Air depletes when player receives damage

## Available Config Options
```
"MaxAir": 100.0,
"AirDepletionRate": 0.05,               // percent of max air per second
"AirRegenerationRate": 0.1              // percent of max air per second

"AirDepletesOnDamageReceived": true     // lose air when damaged?

"AirBarHorizontalAlignmentRight": true  // set to false to align the air bar to the left
"AirBarVerticalAlignmentOffset": -10.0f // lesser number moves the bar up
```

## Todo (at some point)

- Integrate with [xlib](https://www.vintagestory.at/forums/topic/1720-xlib-and-xskills/) to add skills
