# UnityBotKun

## 概要

デバックやテストプレイで、同じ動作を何度も繰り返し行うのは非常に面倒では無いでしょうか？
本ツールは、Unity上で簡易スクリプトを使用して自動的にアプリケーションを制御する為のライブラリです。

## 本ライブラリで出来ること

- 独自のイベントスクリプトを通して、Unityで開発しているアプリケーションのTouchやキー入力といった[UnityEngine.Input](https://docs.unity3d.com/ja/current/ScriptReference/Input.html)を外部から操作する。　[^1]
- プレイ内容をイベントスクリプトへ出力することで、簡易的なリプレイ機能を再現する。[^2]

## 開発環境

### Unityのバージョン

- Unity2019.4.22f1

## 使用方法

1. 本リポジトリを使用したいUnityProjectのAsset以下に配置します。</br></br>
2. Scene上に、`Event System Bot` を配置します。元々Scene上にある`Event System`は無効化して下さい。
![7bb999acffa06c965befe08d2e0dfb32](https://user-images.githubusercontent.com/29646672/114997568-f414e000-9eda-11eb-9019-e399679cc537.gif)</br></br>
3. 簡易スクリプトを作成します。</br></br>
4. 簡易スクリプトをEventSystemBot->ScriptBot->Scriptsに登録します。
![4cc62410ddd69f7453220c85b54bae02](https://user-images.githubusercontent.com/29646672/115168940-9f9a7c00-a0f7-11eb-9f37-8630c06d885c.gif)</br></br>
5. Unity EditorをPlay Modeで実行し、任意のタイミングでScriptBiotのPlayボタンを押します。
![223d79121d8f60d04063952a468103fb](https://user-images.githubusercontent.com/29646672/115173162-9f9f7980-a101-11eb-9bc1-88bb9615ca79.gif)</br></br>

以上</br></br>

## Event System Bot

EventSystemBotはEventSystem,StandaloneInputModuleOverrider,ScriptBot,InputBot,InputRecorder,DontDestoryといいた複数のコンポーネントで構成されています。
### EventSystem

![img](https://user-images.githubusercontent.com/29646672/115169576-65ca7500-a0f9-11eb-95cf-c1f649bcf857.png)

入力、レイキャスト、イベント送信を処理します。
特に変更はありませんので、詳細に関しては[スクリプトリファレンス](https://docs.unity3d.com/ja/2018.4/ScriptReference/EventSystems.EventSystem.html)をご確認下さい。</br></br>

### Standalone Input Module Overrider

![img](https://user-images.githubusercontent.com/29646672/115170249-20a74280-a0fb-11eb-8830-cc0a4adc16f4.png)

Axisやボタンの名称を変更する場合はこちらで設定を行います。
詳細に関しては、[スクリプトリファレンス](https://docs.unity3d.com/ja/2018.4/ScriptReference/EventSystems.StandaloneInputModule.html)をご確認下さい。</br></br>

### Script Bot

![img](https://user-images.githubusercontent.com/29646672/115170588-f3a75f80-a0fb-11eb-8e37-0647a6b7389f.png)

イベントスクリプトの制御を行います。

- Script</br>実行するイベントスクリプトのテーブルです。
  - Size</br>登録するイベントスクリプトの個数を指定します。
  - Element</br>イベントスクリプトを登録します。
- isAutoPlay</br>有効な場合、実行直後にPlayScriptで指定されているイベントスクリプトを実行します。
- Play Scipt</br>実行する、イベントスクリプトを選択します。
- Play</br>イベントスクリプトを実行します。実行する為には、アプリケーションが実行中(Application.isPlay == true)である必要があります。
- Stop</br>実行中のイベントスクリプトを停止します。
</br></br>

### Input Bot

![img](https://user-images.githubusercontent.com/29646672/115174008-66680900-a103-11eb-966d-a2922eebb54e.png)

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

Sceneを跨いでEventSystemBotを使用する為のコンポーネントです。

![img](https://user-images.githubusercontent.com/29646672/115174476-44bb5180-a104-11eb-9dc0-43120e0f571a.png)

- Is Dont Destroy On Load
  Scene切り替え時のEventSystemBotを破棄したくない場合は有効にする必要がありまう。

## イベントスクリプト

イベントスクリプトでは、Touch,Mouse,Button,AxisRawを制御する為の命令に加えて、変数、四則演算、条件分岐、ジャンプ等のプログラマブルな記述を行うことが出来ます。
ゲーム業界でスクリプターの経験がある人には親しみやすい記述方式になっています。
詳しくは、イベントスクリプトリファレンスをご確認下さい。

## FAQ

- Q</br>InputRecorderで作成したイベントスクリプトを再生しても、全く同じ結果になりません。
- A</br>InpurRecorderはInputのみを記録している為、アクションゲーム等、結果に対して様々な要員があるアプリケーションでは同じ結果にならない場合があります。

- Q</br>イベントスクリプトを実行しても、Inputが反映されません。
- A</br>他のEventSystemが有効になっている可能性があります。実行時にEventSystemBot以外のEventSystemがScene上に存在しないか確認してみて下さい。また、Standalone Iput Module OverrideのForce Module Activeを有効にすることで改善する可能性があります。

[^1]:リリース済みのアプリケーションを制御出来る訳ではありません。）
[^2]:Input単体での再現の為、再現性の精度は低いです。
