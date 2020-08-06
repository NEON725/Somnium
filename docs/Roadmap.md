[Go Up](./)

# Road Map
A place to outline specific project requirements for release phase.

Each item should eventually be annotated with a Jira ticket. When all such tickets are marked as done, the next phase can begin.

This is not an exhaustive list of all things that will be accomplished in a phase; It is a list of minimum requirements before that phase can be released.

Bug fixes are not described here.

# MVP
Should feature a very basic environment, and a simple avatar can jump around in that environment. [MMO-15](https://neon725.atlassian.net/jira/software/projects/MMO/issues/MMO-15)

Code should implement the entity-component-system (ECS) pattern and update the project to utilize this framework for entity behavior. [MMO-21](https://neon725.atlassian.net/jira/software/projects/MMO/issues/MMO-21)

Project should implement multiplayer by means of a client-server model. [MMO-22](https://neon725.atlassian.net/jira/software/projects/MMO/issues/MMO-22)

Project should expand multiplayer to feature multiple "shards" that are responsible for different regions of the game world. [MMO-23](https://neon725.atlassian.net/jira/software/projects/MMO/issues/MMO-23)

Game should feature two zones controlled by different shard servers, one of which features basic enemies, and the other being a safe area.

Enemies should be able to damage and kill the players.

Game should implement a basic attack action which can damage and kill the enemies.

Development team should verify that all the above tasks have been completed in a way that presents a fully synchronized and deterministic world to the players. [MMO-24](https://neon725.atlassian.net/jira/software/projects/MMO/issues/MMO-24)