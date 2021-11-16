# UnityBotKun

![GitHub package.json version](https://img.shields.io/github/package-json/v/katsumasa/UnityBotKun)

## Summary

<img width="512" alt="BotKun" src="https://user-images.githubusercontent.com/29646672/117268534-533e9280-ae92-11eb-913a-81dcd2f3daf1.png">

</br></br>
Allows executing content you wish to operate on the application by writing a simple event script.

***

## What you can do with this Library

- By writing your own event script, you can execute arbitrary operations on the application according to the contents.[^1]
- Write the operations on the application to the event script.[^2]

※Intended to be used for implementing application tutorials and testing applications.</br>

</br>

## What you can do with the event script

- Input related command
- Use of int、float、and string variables
- Programmatic operations such as four arithmetic operations, conditional branching, and jump instructions

For example, in order to touch and release the button for a second, you'll need to write the following command:

```cs
::: Press Button1 for a seccond
touch begin 0 "Button1"
wait sec 1.0
touch ended 0
```

Complex commands such as conditional branching and jump instructions can be made as well:

```cs
::: Hit Button1 100 times at を0.1 second intervals
int i 0
# LOOP1
touch begin 0 "Button1"
wait sec 0.1
touch ended 0
add i i 1
ifls i 100 goto LOOP1
```

The event script is explained in detail on the Wiki. Please check [here](https://github.com/katsumasa/UnityBotKun/wiki) for more information.</br></br>

## Operating Environment

### Confirmed Unity version

- Unity2019.4.22f1

### Confirmed platform

#### Android

- Pixel4 XL
</br></br>

## How to use

- Can be execured on UnityEditor and UnityPlayer(actuaal device).
- You need to replace the existing EventSystem object with the EventSystemBot object.
- The procedures are shown as the following:

### How to install

1. Place this repository under Asset of Unity Project you wish to use。</br></br>
2. Place、`EventSystemBot` on the Scene. Disable the original `EventSystem` on the Scene.</br></br>
![7bb999acffa06c965befe08d2e0dfb32](https://user-images.githubusercontent.com/29646672/114997568-f414e000-9eda-11eb-9019-e399679cc537.gif)</br></br>
3. Create [event script]. Please check [here](https://github.com/katsumasa/UnityBotKun/wiki/EventScript) for more information. You can also record input during play and write it out to an event script.</br></br>
4. Register event script from をEventSystemBot->EventScriptSystem->Scripts.</br></br>
![4cc62410ddd69f7453220c85b54bae02](https://user-images.githubusercontent.com/29646672/115168940-9f9a7c00-a0f7-11eb-9f37-8630c06d885c.gif)</br></br>
5. Replace [Input](https://docs.unity3d.com/ja/2018.4/ScriptReference/Input.html) with`Input2`where it's used in the program </br> example</br>

```c#
var horizontal = Input.GetAxsisRow("Horizontal");
```

```c#
using Utj.UnityBotKun;
...
var horizontal = Input2.GetAxsisRow("Horizontal");
```

### Execution method from UnityEditor

Run the Unity Editor in Play Mode and press the Play button on the EventScriptSystem at any time.</br></br>
![223d79121d8f60d04063952a468103fb](https://user-images.githubusercontent.com/29646672/115173162-9f9f7980-a101-11eb-9bc1-88bb9615ca79.gif)</br></br>

### How to execute on UnityPlayer(actual device)

Use UnityBotKun Remote Client to remotely control Application from UnityEditor to execute and record event scripts. UnityBotKun RemoteClient can start with `Window->UnityBotKun->RemoteClient`.</br>

### Warning

Please enable DevelopmentBuild when building the application.
</br></br>

## Component

I nhere, the components added to`EventSystemBot` are explained.

### Event System Bot

EventSystemBot is composed of multiple components such as: EventSystem,StandaloneInputModuleOverrider,ScriptBot,InputBot,InputRecorder,DontDestory.

### EventSystem

![img](https://user-images.githubusercontent.com/29646672/115169576-65ca7500-a0f9-11eb-95cf-c1f649bcf857.png)

Handles input, raycast, and event transmission.
Do not have any change on this. If you wish to know more information, please check　[script reference](https://docs.unity3d.com/ja/2018.4/ScriptReference/EventSystems.EventSystem.html).</br></br>

### Standalone Input Module Overrider

![img](https://user-images.githubusercontent.com/29646672/115170249-20a74280-a0fb-11eb-8830-cc0a4adc16f4.png)

If you want to change the name of Axis or button, set it here.
Check、[script reference](https://docs.unity3d.com/ja/2018.4/ScriptReference/EventSystems.StandaloneInputModule.html) for more information.</br></br>

### EventScriptSystem

![img](https://user-images.githubusercontent.com/29646672/115514564-31081a80-a2bf-11eb-9ca6-991f5ed9b4e2.png)

Controls the event script.

- Script</br>Table that allows to execute event script.
  - Size</br>Specifies the numbers of event scripts to register.
  - Element</br>Register event script.
- isAutoPlay</br>If enabled, the event script specified in PlayScript will run immediately after execution.
- Play Scipt</br>Select the event script to be executed.
- Play</br>Execute event script. In order to execute, the application needs to be running(Application.isPlay == true).
- Stop</br>Stops the event script that are running.
- PC</br>Displays the execution position (line) of the event script.
</br></br>

### BaseInputOverride

![img](https://user-images.githubusercontent.com/29646672/115514881-8a704980-a2bf-11eb-87e6-11e84ba400ef.png)

Component that Hack Input.

- If Is Override Input</br> is valid,、it'll Hack the Input. It'll always be valid whenever the event script is running.
- If Is Enable Touch Simulation</br> is valid, Simulate Mouse as Touch (Mouse will be disabled).
- Input Infomation</br>Input information is displayed. This is a function for debug.
</br></br>

### Input Recorder

![Input Recorder](https://user-images.githubusercontent.com/29646672/115172998-5e0ece80-a101-11eb-84bd-16e8a57b0e58.png)

Component that'll record the Input of the running application and generates an event script.

- Axis Names</br>The name of the Axis to record in the event script. Please make an update to this if you wish to change the name of the axis recorded in the event script.
- Button Names</br>The name of the button to record in the event script. Please make an update to this if you want to change the name of the button recorded in the event script.
- Wait Type</br>Specify the wait command to be recorded in the event script from frame/sec.
- IsEnable Position To GameObject</br>When Touch event occurs, try to convert the Touch event from coordinates to a GameObject name.
- IsCompression</br>Compresses the event script as much as possible. If this setting is disabled, the axis and Mouse information will record every frame. When this setting is enabled, the axis and Mouse information will be recorded only for the changed frames, but the accuracy might get reduced slightly. 
- Is Record Button</br>Specifies whether to record button information or not.
- Is Record Axis Raw</br>Specifies whether to record AxisRaw information or not.
- Is Record Mouse</br>Specifies whether to record Mouse information or not.
- Is Record Touch</br>Specifies whether to record Touch information or not.
- Script</br>Displays the event scripts that are being recorded.
- Record</br>Start recording the event script. The application must be running (Application.isPlaying).
- Stop</br>Stops recording event script.
- Save</br>Save the recorded event script as a TextAsset.
</br></br>

### Dont Destory

Component for using EventSystemBot across the Scenes.

![img](https://user-images.githubusercontent.com/29646672/115174476-44bb5180-a104-11eb-9dc0-43120e0f571a.png)

- Is Dont Destroy On Load
  Needs to be enabled if you do not want to destroy the EventSystemBot when switching Scenes.

### Remote Player

Component used to operate between UnityPlayer to UnityEditor when running on an actual device. 

## UnityBotKun Remote Player

<img width="426" alt="RemoteClient" src="https://user-images.githubusercontent.com/29646672/116061453-d48d6c80-a6bd-11eb-93f5-2dcfc7384654.png">

Wwindow to control EventSystemBot on UnityPlayer (actual device) from UnityEditor.

① Refresh button: Gets the information of UnityBotKun on the Player.</br>
② Event script execution button: Executes/stops the event script specified in ④.</br>
③ Record button: Records / stops the Input information generated on the Player. The recorded content is saved as a Text Asset on the Editor when it stops.</br>
④ Event script selection list: Select the event to run on the Player.</br>
⑤ Event script object field: Sending Event script to Player.</br>
⑥ Add button: Transfer the event script specified in ⑤ to Player.</br>

</br></br>

## Event script

You're able to construct instuctions to control `Touch`,`Mouse`,`Button`, and `AxisRaw`. 
You could also make programmable description of variables, arithmetic operations, conditional branching, jumps and more can be performed.
The description method is familiar to those who have experience as scripters in the game industry.
Please check [Event script reference](https://github.com/katsumasa/UnityBotKun/wiki) for more information.
</br></br>

## FAQ

- Q</br>Does it support [New Input System](https://docs.unity3d.com/Manual/com.unity.inputsystem.html)?
- A</br>No.

- Q</br>Will playing the event script created with `InputRecorder` makes exact sam result?
- A</br>`InpurRecorder`only records Input. The same result may not be obtained because there are various cause being affected such as processing omissions and random numbers.

- Q</br>Input is not reflected even though the event script has been executed.
- A</br>There may be a possibility that other `EventSystem`s may be enabled. Check if there is`EventSystem`other than the`EventSystemBot`on the`Scene` during runtime. It may improve bu enabling`Force Module Active`in`Standalone Iput Module Override`.

- Q</br>The touch and mouse clicks responds to `uGUI`, but it does not respond to other objects such as 3D.
- A</br>Since`EventSystem`is used,`MonoBehaviour.OnMouseXXX`type events does not occur. Please catch the event by inheriting[IPointerEnterHandler](https://docs.unity3d.com/ja/2018.4/ScriptReference/EventSystems.IPointerEnterHandler.html). Also, don't forget to add [PhysicsRaycaster](https://docs.unity3d.com/ja/2018.4/ScriptReference/EventSystems.PhysicsRaycaster.html) and [Physics2DRaycaster](https://docs.unity3d.com/ja/2018.4/ScriptReference/EventSystems.Physics2DRaycaster.html) to the Camera object.

[^1]:You cannot control a released application.
[^2]:The accuracy of reproducibility is low since it's reproduced by Input itself.
