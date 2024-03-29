# UnityBotKun

![GitHub package.json version](https://img.shields.io/github/package-json/v/katsumasa/UnityBotKun)

## 概要

<img width="512" alt="BotKun" src="https://user-images.githubusercontent.com/29646672/117268534-533e9280-ae92-11eb-913a-81dcd2f3daf1.png">

</br></br>
簡単なイベントスクリプトを記述することで、アプリケーション上でその内容に沿った動作を実行します。

***

## 本ライブラリで出来ること

- 独自のイベントスクリプトを記述することで、その内容に沿った任意の操作をアプリケーション上で実行する。[^1]
- アプリケーション上の操作をイベントスクリプトに書き出す。[^2]

※アプリケーションのチュートリアルの実装や、アプリケーションのテスト等で使用することを想定しています。</br>

</br>

## イベントスクリプトで出来ること

- Input系の命令
- int型、float型、string型の変数の使用
- 四則演算、条件分岐、ジャンプ命令などプログラムチックな操作

例えば、ボタンを1秒間タッチして放す場合は、下記のように記述します。

```cs
::: Button1を1秒間押す
touch begin 0 "Button1"
wait sec 1.0
touch ended 0
```

条件分岐やジャンプ命令を使えば複雑な事も可能です。

```cs
::: Button1を0.1秒間隔で100回連打する
int i 0
# LOOP1
touch begin 0 "Button1"
wait sec 0.1
touch ended 0
add i i 1
ifls i 100 goto LOOP1
```

イベントスクリプトに関してはWiki上で詳細に説明していますので、[こちら](https://github.com/katsumasa/UnityBotKun/wiki)をご確認下さい。</br></br>

## 動作環境

### Unityのバージョン

- Unity2019.4.22f1

### 動作確認済み端末

#### Android

- Pixel4 XL
</br></br>

## 使用方法

- UnityEditorとUnityPlayer(実機)の両方で実行出来ます。
- 既存の`Event System`オブジェクトと`Event System Bot`オブジェクトを差し替える必要があります。
- 具体的な手順は下記の通りです。

### インストール方法

1. 本リポジトリを使用したいUnityProjectのAsset以下に配置します。</br></br>
2. Scene上に、`Event System Bot` を配置します。元々Scene上にある`EventSystem`は無効化して下さい。</br></br>
![7bb999acffa06c965befe08d2e0dfb32](https://user-images.githubusercontent.com/29646672/114997568-f414e000-9eda-11eb-9019-e399679cc537.gif)</br></br>
3. [イベントスクリプト]を作成します。イベントスクリプトの記述情報に関しては、[こちら](https://github.com/katsumasa/UnityBotKun/wiki/EventScript)をご確認下さい。また、プレイ中の入力を記録し、イベントスクリプトに書き出すことも可能です。</br></br>
4. イベントスクリプトを`Event System Bot`->`Event Script System`->`Scripts`に登録します。</br></br>
![4cc62410ddd69f7453220c85b54bae02](https://user-images.githubusercontent.com/29646672/115168940-9f9a7c00-a0f7-11eb-9f37-8630c06d885c.gif)</br></br>
5. プログラム中に[Input](https://docs.unity3d.com/ja/2018.4/ScriptReference/Input.html)を使用している箇所は`Input2`に置き換えて下さい。</br> example</br>

```c#
var horizontal = Input.GetAxsisRow("Horizontal");
```

```c#
using Utj.UnityBotKun;
...
var horizontal = Input2.GetAxsisRow("Horizontal");
```

### UnityEditorでの実行方法

Unity EditorをPlay Modeで実行し、任意のタイミングで`Event Script System`のPlayボタンを押します。</br></br>
![223d79121d8f60d04063952a468103fb](https://user-images.githubusercontent.com/29646672/115173162-9f9f7980-a101-11eb-9bc1-88bb9615ca79.gif)</br></br>

### UnityPlayer(実機)での実行方法

UnityBotKun Remote Clientを使ってApplicationをUnityEditorからリモートで制御し、イベントスクリプトの実行、記録を行います。UnityBotKun RemoteClientは`Window->UnityBotKun->RemoteClient`で起動します。</br>

### 注意時効

アプリケーションをビルドする際にDevelopmentBuildを有効にしてビルドを行って下さい。
</br></br>

## Component

ここでは、`Event System Bot`　にAddされているコンポーネントを説明します。

### Event System Bot

`Event System Bot`はEventSystem,StandaloneInputModuleOverrider,ScriptBot,InputBot,InputRecorder,DontDestory等の複数のコンポーネントで構成されています。

### Event System

![img](https://user-images.githubusercontent.com/29646672/115169576-65ca7500-a0f9-11eb-95cf-c1f649bcf857.png)

入力、レイキャスト、イベント送信を処理します。
特に変更はありませんので、詳細に関しては[スクリプトリファレンス](https://docs.unity3d.com/ja/2018.4/ScriptReference/EventSystems.EventSystem.html)をご確認下さい。</br></br>

### Standalone Input Module Overrider

![img](https://user-images.githubusercontent.com/29646672/115170249-20a74280-a0fb-11eb-8830-cc0a4adc16f4.png)

Axisやボタンの名称を変更する場合はこちらで設定を行います。
詳細に関しては、[スクリプトリファレンス](https://docs.unity3d.com/ja/2018.4/ScriptReference/EventSystems.StandaloneInputModule.html)をご確認下さい。</br></br>

### Event Script System

![img](https://user-images.githubusercontent.com/29646672/115514564-31081a80-a2bf-11eb-9ca6-991f5ed9b4e2.png)

イベントスクリプトの制御を行います。

- Script</br>実行するイベントスクリプトのテーブルです。
  - Size</br>登録するイベントスクリプトの個数を指定します。
  - Element</br>イベントスクリプトを登録します。
- isAutoPlay</br>有効な場合、実行直後にPlayScriptで指定されているイベントスクリプトを実行します。
- Play Scipt</br>実行する、イベントスクリプトを選択します。
- Play</br>イベントスクリプトを実行します。実行する為には、アプリケーションが実行中(Application.isPlay == true)である必要があります。
- Stop</br>実行中のイベントスクリプトを停止します。
- PC</br>イベントスクリプトの実行位置(行)を表示します。
</br></br>

### BaseInputOverride

![img](https://user-images.githubusercontent.com/29646672/115514881-8a704980-a2bf-11eb-87e6-11e84ba400ef.png)

InputをHackするコンポーネントです。

- Is Override Input</br>有効な場合、InputをHackします。イベントスクリプトが実行中は常に有効となります。
- Is Enable Touch Simulation</br>有効な場合、MouseをTouchとしてシュミレーションします。この時、Mouseは無効となります。
- Input Infomation</br>Inputの情報を表示します。デバック用の機能です。
</br></br>

### Input Recorder

![Input Recorder](https://user-images.githubusercontent.com/29646672/115172998-5e0ece80-a101-11eb-84bd-16e8a57b0e58.png)

実行中のアプリケーションのInputを記録して、イベントスクリプトを生成する為のコンポーネントです。

- Axis Names</br>イベントスクリプトに記録する軸の名称です。イベントスクリプトに記録する、軸の名前を変更したい場合、こちらを変更して下さい。
- Button Names</br>イベントスクリプトに記録するボタンの名称です。イベントスクリプトに記録する、ボタンの名前を変更したい場合、こちらを変更して下さい。
- Wait Type</br>イベントスクリプトに記録するwaitコマンドをframe/secから指定します。
- IsEnable Position To GameObject</br>Touchイベントが発生した場合、可能であれば、Touchイベントを座標からGameObject名に変換します。
- IsCompression</br>イベントスクリプトを可能な限り圧縮します。この設定が無効な場合、axisやMouseの情報が毎フレーム記録されます。この設定が有効な場合、axisやMouseの情報は変化があったフレームのみ記録されますが、精度が若干落ちます。
- Is Record Button</br>ボタンの情報を記録するか否かを指定します。
- Is Record Axis Raw</br>AxisRawの情報を記録するか否かを指定します。
- Is Record Mouse</br>Mouseの情報を記録するか否かを指定します。
- Is Record Touch</br>Touchの情報を記録するか否かを指定します。
- Script</br>記録中のイベントスクリプトの内容を表示します。
- Record</br>イベントスクリプトの記録を開始します。イベントスクリプトを記録する為にはアプリケーションが実行中(Application.isPlaying)である必要があります。
- Stop</br>イベントスクリプトの記録を停止します。
- Save</br>記録したイベントスクリプトをTextAssetとして保存します。
</br></br>

### Dont Destory

Sceneを跨いで`Event System Bot`を使用する為のコンポーネントです。

![img](https://user-images.githubusercontent.com/29646672/115174476-44bb5180-a104-11eb-9dc0-43120e0f571a.png)

- Is Dont Destroy On Load
  Scene切り替え時の`Event System Bot`を破棄したくない場合は有効にする必要があります。

### Remote Player

実機上で動作させる場合にUnityPlayer<->UnityEditor間の動作を行う為のコンポーネントです。

## UnityBotKun Remote Player

<img width="426" alt="RemoteClient" src="https://user-images.githubusercontent.com/29646672/116061453-d48d6c80-a6bd-11eb-93f5-2dcfc7384654.png">

UnityPlayer(実機)上の`Event System Bot`をUnityEditorから制御する為のWindowです。

① Refleshボタン。Player上のUnityBotKunの情報を取得します。</br>
② イベントスクリプト実行ボタン。④で指定されたイベントスクリプトを実行/停止を行います。
③ 録画ボタン。Player上で発生したInput情報の記録/停止を行います。記録内容は停止時にEditor上でTextAssetとして保存します。</br>
④ イベントスクリプト選択リスト。Player上で実行するイベントを選択します。</br>
⑤ イベントスクリプトオブジェクトフィールド。Player上に転送するイベントスクリプト。</br>
⑥ 追加ボタン。⑤で指定したイベントスクリプトをPlayerへ転送します。</br>

</br></br>

## イベントスクリプト

イベントスクリプトでは、`Touch`,`Mouse`,`Button`,`AxisRaw`を制御する為の命令に加えて、変数、四則演算、条件分岐、ジャンプ等のプログラマブルな記述を行うことが出来ます。
ゲーム業界でスクリプターの経験がある人には親しみやすい記述方式になっていると思います。
詳しくは、[イベントスクリプトリファレンス](https://github.com/katsumasa/UnityBotKun/wiki)をご確認下さい。
</br></br>

## FAQ

- Q</br>[New Input System](https://docs.unity3d.com/Manual/com.unity.inputsystem.html)に対応していますか？
- A</br>対応していません。

- Q</br>`InputRecorder`で作成したイベントスクリプトを再生しても、全く同じ結果になりません。
- A</br>`InpurRecorder`はInputのみを記録しています。処理落ちや乱数など、結果に対して様々な要員がある為、同じ結果にならない場合があります。

- Q</br>イベントスクリプトを実行しても、Inputが反映されません。
- A</br>他の`Event System`が有効になっている可能性があります。実行時に`Event System Bot`以外の`Event System`が`Scene`上に存在しないか確認してみて下さい。また、`Standalone Iput Module Override`の`Force Module Active`を有効にすることで改善する可能性があります。

- Q</br>`uGUI`にはタッチやマウスのクリックが反応しますが、3D等他のオブジェクトに反応しません。
- A</br>`Event System`を利用している為、`MonoBehaviour.OnMouseXXX`系のイベントは発生しません。[IPointerEnterHandler](https://docs.unity3d.com/ja/2018.4/ScriptReference/EventSystems.IPointerEnterHandler.html)を継承する等してイベントをキャッチして下さい。また、Cameraオブジェクトに[PhysicsRaycaster](https://docs.unity3d.com/ja/2018.4/ScriptReference/EventSystems.PhysicsRaycaster.html)や[Physics2DRaycaster](https://docs.unity3d.com/ja/2018.4/ScriptReference/EventSystems.Physics2DRaycaster.html)をAddすることもお忘れなく。

[^1]:リリース済みのアプリケーションを制御出来る訳ではありません。）
[^2]:Input単体での再現の為、再現性の精度は低いです。
