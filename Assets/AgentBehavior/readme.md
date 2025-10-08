## Interface
The Non-Verbal interaction with the agent is achieved through two interfaces - 
the EventSystem and the ArmatureHandles for procedural animations.

The ```AgentControler``` script attached to the Agent (e.g., Patient Node) is the entry point 
for both systems. Use AgentController's ```notify()``` method to enqueue events in that system. 
Use ```GetArmatureHandle``` to get a handle that controls one of the manipulatable body parts. 

## State
The agent's high-level and clothing states are exposed through the respective properties in 
the ```AgantController```. 

## Events
The events can be ```ActionRequest```s (s. Events/ActionRequest.cs for all available requests), 
```InterestingObjectEvent```s (s. Events/InterestingObjectEvent.cs), or ```SpeechEvent```s 
(s. Events/SpeechEvent.cs). In addition, an implementation of the ```IActionRequestStateObserver```
interface can be passed to the ```ActionRequest```, such that the sender receives notifications about 
the current processing state. Similarly, an ```IPerceptionEventStateObserver``` can be used to receive 
notifications about the state of a perception event (e.g., ```InterestingObjectEvent``` or ```SpeechEvent```).

## Armature Handles
An armature handle is basically a hidden scene object used as a target for the IK system when enabled. 
It remains attached to the respective body part when disabled. For instance, the armature handle for 
the key ```ArmatureHandleType.RightIndex``` is attached to the fingertip of the agent's right index finger. 
When the handle is activated, the fingertip will try to align with it. 

The particular implementation of each handle is abstracted behind the interface ```IArmatureHandle```
(s. Armature/IArmatureHandle.cs), and each handle has its dedicated trigger collider in the scene. 
This enables both direct and indirect selection and manipulation. In addition, the position and 
orientation of the handle can be set either manually, through the ```SetPosition``` and ```SetOrientation```
methods, or indirectly by forcing the handle to follow some other transform (s. ```IArmatureHandle.SetTarget```). 

## Example
- An example implementation of an event-sending script is available in Examples/TestEventSender.cs 
(attached to the Patient node).
- TODO: IndirectAnchor.cs demonstrates how indirect interaction with the agent can be implemented 
through the ```IArmatureHandle``` interface. 
- TODO: DirectAnchor.cs demonstrates how direct interaction with the agent can be implemented 
through the ```IArmatureHandle``` interface.

## Rules of Engagement
1. Don't edit, add, remove, or change anything in the AgentBehavior folder. 
If you find a bug or need some functionality - issue a ticket or write me an e-mail.
2. Don't put any code in the ```AgentBehavior``` namespace.
3. Don't change anything under [Agent-Name]/Rig nodes.
4. Don't down-cast or use any undocumented classes, functions, etc.

## Known problems/TODO
1. The Look-At behavior is only implemented for the head, not for the eyes.
2. Lip-sync is not implemented.
3. The agent can only walk in a straight line and stops without recovery if any obstacle is encountered. 
4. There is only a very simple/prototypic recovery in the action sequencing. 
