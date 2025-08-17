# Parsing Specifications
## Level: `Level` as `CLASS_NAME`
A level is a JSON object that contains the level's _template_, _playerPosition_, _npcs_, _rawPointers_,
and _objectiveDisplayGroups_.
```
{
    "template":"CLASS_NAME",
    "playerPosition":[0,0],
    "npcs":[...]
    rawPointers: {
        "pointer1": {...},
        "pointer2": {...},
    },
    "objectiveDisplayGroups":[
        {...},
        {...}
    ]
}
```

## Regular NPC: `Npc`
A regular NPC that can be interacted with, initiating a speech. If an NPC is interactable, 
_interactable_ must be true and the _speech_ must be defined. <br/>
```
{
    "type": "regular",
    "x": 0,
    "y": 0,
    "identity": {
        "speaker": "Layman",
        "name": "FOO",
    }
    "interactable": true,
    "speech": [{...}]
}
```

## Path-determinant NPC: `PathDeterminantNpc`
A path-determinant NPC that moves along a specified _movement_ path. <br/>
_repeatMotion_ indicates whether the NPC should repeat the movement path indefinitely. <br/>
_isRelativePath_ indicates whether the movement path is relative to the NPC's current position.
Otherwise, the movement will be interpreted as absolute position. <br/>
The _movement_ is a list of movements, where each movement is a list of three numbers: [x, y, duration]. <br/>
```
{
    "type": "path-determinant",
    "movement": [[0,0,5],[4,0,5],[0,0,5],[-4,0,5],[0,0,5]],
    "x": 0,
    "y": 0,
    "repeatMotion": true,
    "isRelativePath": true,
    "speaker": "Archer",
    "name": "FOO",
    "interactable": false
}
```

## Speech Line: `SpeechLine`
A speech line that sequences _content_.<br/>
Each speech line can have a _nextKey_ that points to the next speech line in the sequence. <br/>
Each speech line can also have _options_ that allow the player to make a choice. 
```
{
  "content": "START_SPEECH_CONTENT",
  "nextKey": "a",
  
  "a": {
	"content": "A_CONTENT",
	"options": [
	    ["option 1","optionContent"],
	    ["option 2","optionContent"],
	    ["option 3","optionContent"]
    ],
	"nextKey": "b"
  },
  
  "b": {
	"content": "B_CONTENT"
  },
  
  "optionContent": {
	"content": "OPTION_CONTENT"
  }
}
```

## Objective Display Group
A group of _objectives_ that are displayed together and completed together before proceeding. <br/>
_hiddenObjectives_ are not visible and are not required to be completed to proceed, but are still active. 
_hiddenObjectives_ are useful for triggering `NegativeObjective` objectives. 
```
{
    "hiddenObjectives": [
        {...},
        {...}
    ],
    "objectives": [
        {...},
        {...}
    ]
}
```

## Objective
An objective that can be displayed in the objective display group, with a specified _description_.
```
{
    "type": "objective",
    "description": "OBJECTIVE_DESCRIPTION"
}
```

## IHasObjective

### Touch: `TouchObjective`
An objective that requires the player to touch a specified _position_.
```
{
  "type":"touch",
  "position": [0,0],
  "objective": {...}
}
```

### Finish Chat: `FinishChatObjective`
An objective that requires the player to finish a chat with a specified _npc_.
```
{
  "type": "finishChat",
  "npc": "1",
  "objective": {...}
},
```

### Negative: `NegativeObjective`
An objective that requires the player to not complete specified _negativeObjectives_.
```
{
  "type": "negative",
  "negativeObjectives": [
    "OBJECTIVE_POINTER_1",
    "OBJECTIVE_POINTER_2"
  ],
  "objective": {...}
},
```

### Option: `OptionObjective`
An objective requiring the player to select a _correct option_ and avoid _incorrect options_.
```
{
  "type": "option",
  "correctOption": "CORRECT_OPTION",
  "incorrectOptions":[
    "INCORRECT_OPTION_1","INCORRECT_OPTION_2"
  ],
  "objective": {...}
},
```

### Start Chat: `StartChatObjective`
An objective requiring the player to start a chat with a _npc_.
```
{
  "type": "startChat",
  "npc": "npc-pointer",
  "objective": {...}
}
```

### Target Speech: `TargetSpeechObjective`
An objective requiring the player to reach a _target_ speech line in a chat.
```
{
    "type": "targetSpeech",
    "target": "TARGET_LINE",
    "objective": {...}
}
```

### Cutscene: `SimpleCutsceneObjective`
Starts a cutscene.
```
{
  "type": "cutscene",
  "actionGroups": [
    {...},
    [...]
  ]
}
```

## Cutscene Action Groups
A group of `ICutsceneAction` executed simultaneously. <br/>
Attribute _speechAction_ is optional. If _speechAction_ is present, the actions described will execute after the
`SpeechAction` is completed. <br/>
Otherwise, the actions will execute immediately.

```
{
  "speechAction": "SPEECH_ACTION_POINTER",
  "actions": [{...},{...}]
},
```

## ICutsceneAction

### Speech Action: `SpeechAction`
A `SpeechLine` that can be used in a cutscene as an action. This is usually specified 
in the _speechAction_ attribute of a Cutscene Action Group.
```
{
  "type": "speechAction",
  "speaker": "Archer",
  "name": "NAME",
  "speech": "SPEECH_POINTER"
}
```

### Cutscene Movement: `MovementCutsceneAction`
Moves the specified _npc_ through the specified path, _moves_.
```
{
  "type": "cmovement",
  "npc": "NPC_POINTER",
  "moves": [[0,-6],[-6,6,3],[0,0]]
}
```

### Pan: `PanCutsceneAction`
Pans the center of the camera to the specified _position_.
```
{
    "type": "pan",
    "position": [0,0]
},
```

### Inline: `InlineCutsceneAction`
Creates a _pointer_ to be late initialized during `_Ready()`, ie: 
```
var inline = (InlineCutsceneAction)builder.parser.filteredPointers["inline-pointer"];
inline.action = () => {...};
```

```
{
    "type": "inline",
    "pointer": "inline-pointer"
},
```

### Flash
Performs the specified flash _queue_ in sequence.
```
{
  "type": "flash",
  "queue": [
    {...},
    {...}
  ]
}
```

### Wait
Waits for the specified duration.
```
["wait", 5]
```

## Flash Actions

### Set
Sets the flash color to the specified color.
```
{"type": "set","color": "Black"}
```

### Dissolve
Fades the alpha value to 0 over the specified duration.
```
{"type": "dissolve","duration": 1},
```

### To Color
Fades to the specified color over the specified duration.
```
{"type": "toColor","color": "red","duration":0.3},
```

### StayStagnant
Keeps the color unchanged for the specified duration.
```
{"type": "stayStagnant","duration": 1},
```

### To Solid
Fades the alpha value to 1 over the specified duration.
```
{"type": "toSolid","duration": 1},
```

### Initiate
Starts the flash.
```
{"type": "initiate"}
```

### Queue Reset
Resets the color after all other flash effects are processed. 
```
{"type": "queueReset"}
```

### Hard Reset
Resets the flash color instantly to transparent white. 
```
{"type": "hardReset"}
```