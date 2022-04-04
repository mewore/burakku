# Burakku

~~Yes, I used the age-old tactic of butchering a foreign word so that I don't have to deal with the fact that the name "
Blacklight" is already taken.~~

## TODO

- [x] 💚 MVP idea
    - **Base genre:** puzzle, platformer
    - Basically Fireboy and Watergirl, but with one catch - ~~Fireboy~~ Blacklight radiates ultraviolet light and
      ~~Watergirl~~ Vamp is a vampire, vulnerable to ~~sunlight~~ UV radiation

### MVP

- [x] 💙 Player (blacklight) moving around with [A]/[D]
- [x] 💙 Jumping with [W]
- [x] 💙 Light coming from the first player (this is going to be a hard one...)
- [x] 💙 Other player (vampire) moving around with [left arrow]/[right arrow] and jumping with [up]
- [x] 💙 The other player gets hurt by the first player if there is a direct line of sight between them
    - When the other player's HP turns 0, restart the level ([Lose!])
- [x] 💙 Instead of immediately restarting the level, turn the other player into particles that fade in slowly, float
  upwards and fade out
- [x] 💙 Button that opens a door
- [x] 💙 Stationary blacklights
- [x] 💙 Button that disables a blacklight
- [x] 💙 Door where if a character goes, they exit the level and can reappear with [Jump]
- [x] 💙 If both players are in the door, [Win!]
- [x] 💚 Second level
- [x] 💜 Use polygons to show the collision shapes
- [x] 💙 Add some in-game instructions
- [x] 💟 Publish `0.1.0`

### Basic features

- [x] 💜 Simple blacklight sprite
- [x] 💜 Simple vampire sprite
- [x] 💜 Simple blacklight character sprite
- [x] 💜 Simple environment textures
- [x] 💟 Publish `0.1.1`
- [x] 💙💜 Better-looking fire and lighting
- [x] 💙 Main menu (just "Play")
    - [x] 💙 Save progress and start from the last level or the win screen
    - [x] 💙 Pause menu
    - [x] 💙 Sound settings
        - [x] 💙💛 Main menu music
        - [x] 💙💛 Level music
        - [x] 💙💛 Jump sound ([Source](https://www.zapsplat.com/music/bendy-stick-whoosh-through-air-fast-3/))
        - [x] 💙💛 Land sound ([Source](https://www.zapsplat.com/music/footsteps-in-sandals-flip-flops-on-slightly-gritty-garage-floor-single-step-3/))
        - [x] 💙💛 Clear level sound ([Source](https://www.zapsplat.com/music/game-sound-bright-and-warm-synth-complete-success-tone-1/))
        - [x] 💙💛 Lose level sound ([Source](https://www.zapsplat.com/music/game-sound-hit-thud-good-for-success-win-or-finish-level/))
        - [x] 💙💛 Vampire death sound ([Source](https://www.zapsplat.com/music/medium-fireball-close/))
        - [x] 💙💛 Vampire damage sound ([Source](https://www.zapsplat.com/music/fire-small-flame-close/))
- [x] 💟 Publish `0.1.2`
- [x] 💙 Button that activates a platform
- [x] 💙💛 Button click sound
- [x] 💙💛 Button un-click sound
- [ ] 💚 Some proper levels
- [ ] 💟 Publish `0.1.3`
- [ ] 💜 Environment art
- [ ] 💜 Banner art
- [ ] 💜 Icon
- [ ] 💜 Logo
- [ ] 💜 Main menu art
- [ ] 💟 Publish `0.2.0`

### Advanced features

- [ ] 💙💜 Enemies that move around like goombas and are killed almost instantly by the blacklight
- [ ] 💙 The enemies kill the vampire immediately upon contact
- [ ] 💙 Particle effects for damaging the enemies
- [ ] 💙 Settings for the graphics quality; enable lights
- [ ] 💙💛 More dynamic-sounding level music
- [ ] 💙💜 Gradient in the blacklight polygons (the challenge is going to be to reposition the polygon texture properly)
- [ ] 💜 Main menu art
- [ ] 💙 Be able to start from a specific level
- [ ] 💜 Prettier blacklight sprite and animation
- [ ] 💜 Prettier vampire sprite and animation
- [ ] 💜 Prettier enemy sprite
- [ ] 💜 Prettier environment
- [ ] 💟 Publish `0.3.0`

### Expert features
- [ ] 💙💛 Menu item hover sound
- [ ] 💙💛 Menu item press sound
- [ ] 💙💜💚 Boxes, which only the vampire can pick up and throw
- [ ] 💙💜💚 Puzzles with water and floating boxes
- [ ] 💚 Even more levels
- [ ] 💙 Vampire monologue functionality (at the start of a level)
- [ ] 💜 Vampire monologue avatar
- [ ] 💚 Vampire monologue
- [ ] 💛 Monologue sound
- [ ] 💟 Publish `0.3.1`
- [ ] 💙💜💚 Story
- [ ] 💟 Publish `0.4.0`

---

#### Legend

- 💙 Code/Godot
- 💜 Art
- 💚 Design
- 💛 Audio
- 💟 Special
