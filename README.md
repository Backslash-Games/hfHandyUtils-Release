https://www.notion.so/halfhand870/README-365d086035d380dba5ccd3e100bdad5a

# Overview 05/19/2026

An open source Unity Engine 6 library that contains foundational scripts to assist in creating games.

# Features

- **General**
    - **IActivatable**: Interface that allows for logic when “activated”
- **Asynchronous Tools [Work in Progress]**
- **Data**
    - **Cache Value:** Value that can be set once then automatically locks until manually reset.
    - **Limitless Numeric:** Integer value that has no maximum limit
    - **Random Set:** Cached collection of random values. Allows for instanced seeded randomness.
- **Effects**
    - **Effect Component:** General scene definition for playing an effect
        - **Audio:** Audio specific definition
        - **Visual:** Visual specific definition
    - **Effect Library:** Enumerated collection of effects
    - **Effect Library Entry:** Effect entry in the library
    - **Effect Manager:** Required manager for playing effects
- **Graphical**
    - **Billboard:** Definition that forces an object to always look at the main camera
    - **Decal [Work in Progress]**
- **Math**
    - **Math H.:** Collection of math functions
    - **Random H.:** Collection of randomness functions
- **Mono**
    - **Monobehaviour Debug:** Pushes information from a Monobehaviour into a text container which exists in a scene.
    - **Monobehaviour Save [Work in Progress]**
- **Physics**
    - **Hitboxes:** General definition for a hitbox
        - **Cube**
        - **Sphere**
        - **Capsule [Work in Progress]**
- **Time**
    - **Time Control**: Generalized method of controlling unity’s time scale
    - **Cooldown:** Method that handles cooldowns