# ChroMapper-CameraMovement
[CameraPlus](https://github.com/Snow1226/CameraPlus)用の[MovementScript](https://github.com/Snow1226/CameraPlus/wiki/MovementScript)を読み込んで、作譜ツールの[ChroMapper](https://github.com/Caeden117/ChroMapper)でカメラワークの再現をするChroMapper用プラグインです。
主に[Script Mapper](https://github.com/hibit-at/Scriptmapper)を使ったカメラスクリプトの作成を前提とした作りになっています。

![image](https://user-images.githubusercontent.com/14249877/158151048-4d7dbe2e-0df6-4a9d-812a-b977847721b7.png)

# インストール方法
1. [リリースページ](https://github.com/rynan4818/ChroMapper-CameraMovement/releases)から、最新版のプラグインのzipファイルをダウンロードして下さい。

2. ダウンロードしたzipファイルを解凍してChroMapperのインストールフォルダにある`Plugins`フォルダに`ChroMapper-CameraMovement.dll`をコピーします。

- Ver1.4.0～1.4.1 では以下のファイルをChroMapperのインストールフォルダにコピーしていましたが、Ver1.4.2 よりChroMapper-CameraMovement.dllに統合したため不要になりました。Ver1.4.0～1.4.1をお使いの方は、不要なため削除して下さい。
    - netstandard.dll
    - Newtonsoft.Json.dll
    - UniGLTF.dll
    - UniHumanoid.dll
    - VRM.dll
    - VRMShaders.GLTF.IO.Runtime.dll
    - VRMShaders.GLTF.UniUnlit.Runtime.dll
    - vrmavatar.shaders

3. [Script Mapper](https://github.com/hibit-at/Scriptmapper)をダウンロードして、ChroMapperのインストールフォルダ(ChroMapper.exeがあるフォルダ)に`scriptmapper.exe`をコピーします。

# アバターについて
[カスタムアバター](https://modelsaber.com/Avatars/) および VRM の3Dアバターを読み込んで表示できます。

本プラグイン用のカスタムアバターの作成は[CameraMovementAvatarExporterパッケージ](https://github.com/rynan4818/CameraMovementAvatarExporter)を使用すると簡単です。

VRM対応の副産物として[ChroMapper-VRMAvatar](https://github.com/rynan4818/ChroMapper-VRMAvatar)を作りました。VRMアバターをChroMapperに読み込む単独で動くプラグインです。

# 使用法

本ツールとScriptMapperを使った記事を書いてみました。

**[BeatSaberでカメラを動かしてみよう](https://note.com/rynan/n/n2830a21add26)**

## キーボードショートカット
- F4 でプレビュー表示のON/OFF
- F3 でScriptMapperの実行
- 各設定パネルは、Shiftを押しながら左ドラッグで移動
- 注視点カメラ移動モード
    - ALT + マウス右ドラッグでアバター中心にカメラ移動
    - ALT + マウス左ドラッグで注視点のオフセット
    - ALT + マウス左右同時クリックで注視点オフセットのリセット
    - ALT + マウスホイールで注視点との距離の変更
    - ALT + CTRL + マウスホイールでFOVの変更
    - ALT + CTRL + マウス左右同時クリックでFOVのリセット

※上記キーバインドは[設定ファイル](https://github.com/rynan4818/ChroMapper-CameraMovement#%E8%A8%AD%E5%AE%9A%E3%83%95%E3%82%A1%E3%82%A4%E3%83%AB%E3%81%AB%E3%81%A4%E3%81%84%E3%81%A6)の直接変更で変更可能

## 各パネル説明
譜面を読み込んでエディタ画面を出して下さい。**Tabキー**を押すと右側にアイコンパネルが出ますので、水色のカメラアイコンを押すと下の画像 CameraMovementの設定パネルが開きます。

![image](https://user-images.githubusercontent.com/14249877/158151205-a13f2038-ec85-40e8-bc2d-374c5a0cb4f1.png)

* Movement Enable ： カメラスクリプトに合わせてカメラが移動します。
* UI Hidden ： 作譜用のグリッドUI、ライティングブロック、音声波形などを消します。
* NJS Editor Scale ： エディタスケールをNJSを反映(ゲーム内と同じ)にします。ChroMapper設定のAccurate Editor Scaleと同じです。
* Turn To Head ： カメラスクリプトのTurnToHeadUseCameraSettingの設定がtrueの時に、カメラがアバターの方向を自動で向きます。(CameraPlusのTurnToHeadパラメータに相当します。)
* Avatar ： アバターを表示します。
* Bookmark Lines ： ブックマークの内容をノーツブロックレーンの右側に表示します。
* Sub Camera ： 常にスクリプトに合わせて動くサブカメラを表示します。
* Bookmark Edit ： ブックマークの編集パネルを下に表示します。
* Camera Contral ： カメラコントロールのパネルを下に表示します。
* More Seting ： 詳細設定パネルを表示します。
* Reload ： 設定やスクリプトを読み込み直します
* Setting Save : 設定パネルの内容およびブックマーク編集パネルのコマンドボタンの内容を設定ファイルに保存します。
* Script Mapper Run [F3] ： 譜面データを保存して、Script Mapperでブックマークをカメラスクリプトに変換します。F3キーでショートカット。

![image](https://user-images.githubusercontent.com/14249877/158151330-93cfb73b-0ca2-4d88-8c05-edb11b8e49b0.png)

* Custom or VRM Avatar ： 読み込んだカスタムアバター・VRMアバターモデルを表示します。
* Avatar File ： `*.vrm` VRMファイルもしくは、`*.avatar` [カスタムアバターのファイル](https://modelsaber.com/Avatars/)を`Select`ボタンで選択します。デフォルト設定はChroMapper.exeと同じフォルダにある[Sour Miku Black v2](https://modelsaber.com/Avatars/?id=1564625718&pc)になっています。
* Reload ： アバターファイルを変更した場合の再読み込みボタンです。
* Select ： アバターファイルを選択します。
* Avatar Scale ： アバターのサイズ調整用です。実際のアバターのサイズはBeatSaber内でキャリブレーション後に何らかの方法で測定する必要があります。(ゲーム内と本プラグインで同じカメラ設定にして、表示サイズが同じになるように調整するのが一番楽だと思います)
* Avatar Y offset ： アバターのY位置調整用です。
* VRM Blinker ： VRMファイルを読み込んだ場合に、まばたきを自動で行います。
* VRM LookAT ： VRMファイルを読み込んだ場合に、カメラ目線になります。
* VRM Animation ： 通常は使用しません。不具合がある場合にチェックを外して見て下さい。
* Simple Avatar: ピンクの棒人間アバターを表示します。本プラグインで読み込んだカスタムアバターのサイズ測定に使えます。(Head Hight + Head Size/2 = 身長[m]です。) ※Sour Miku Black v2は1.72m
* Head Hight Lookat point ： アバターの頭の高さ（球の中心）です。[単位 m] **カメラコントロールのLookAtボタンや、スクリプトのアバターにカメラを自動で向ける場合の位置になります**
* Head Size ： アバターの頭の大きさ(球の直径) [単位 m]
* Arm Size ： アバターの両手の長さ [単位 m]
* Bookmark Width ： タイムラインのブックマークの表示幅を調整します。(デフォルトは10)
* Bookmark Area ： Bookmark Linesで表示するために開けるスペース。
* Bookmark Export ： ブックマークをCSVファイルに出力します。
* Bookmark Import(未実装) ： ブックマークをCSVファイルから取り込みます。
* Sub X ： サブカメラの左下座標の画面横軸位置です。（0が左端、1が右端)
* Sub Y ： サブカメラの左下座標の画面縦軸位置です。（0が下端、1が上端)
* Cursor Key Move ： 押すとカーソルキーでSub X,Yを0.01単位で調整できます。Shiftを押しながらだと0.1単位。
* Sub W ： サブカメラの画面の高さです。(1がメイン画面高さ)
* Sub H ： サブカメラの画面の幅です。(1がメイン画面幅)
* Cursor Key Size ： 押すとカーソルキーでSub W,Hを0.01単位で調整できます。Shiftを押しながらだと0.1単位。
* Script File ： 譜面フォルダにある読み込むカメラスクリプトファイル名
* Mapping Disable ： Camera Movementの機能有効中は譜面作成機能を抑制して、ノーツ等の誤編集を防止します。
* Setting Save ： 設定パネルの内容およびブックマーク編集パネルのコマンドボタンの内容を設定ファイルに保存します。(メイン設定の同名ボタンと機能は同じ)
* Close ： More Settingパネルを閉じます。

![image](https://user-images.githubusercontent.com/14249877/158151436-430f0a38-d5b5-47fd-992c-7d5b8c860910.png)

* `No.8[18]` ： 現在の編集対象のブックマークNo.と拍子位置です。
* `center1,next` ： 設定されたブックマークの内容が表示されます。
* `center`～`random` ： よく使用するコマンドを登録して呼び出せます。
* Set ： チェックしてから上の６箇所のボタンを押すと`Empty` の部分に入力した内容が登録できます。
* Copy to Edit ： 現在のブックマーク内容を`Empty` の部分にコピーします。
* Clear ： ブックマーク入力部`Empty`の入力内容を消します。
* New ： `Empty` の内容をブックマークとして新規登録します。
* Change ： 現在のブックマークの内容を修正します。
* Delete ： 現在のブックマークを削除します。
* Script Mapper Run [F3] ： ScriptMapperを実行します。メインメニューのそれと同じです。

![image](https://user-images.githubusercontent.com/14249877/158151664-16964d70-680c-4625-ba72-133fed53bf96.png)

* Pos X ： メインカメラのX軸(横方向)位置です[単位m]
* Pos Y ： メインカメラのY軸(高さ方向)位置です[単位m]
* Pos Z ： メインカメラのZ軸(奥行き方向)位置です[単位m]
* Rot X ： メインカメラのX軸(画面上下方向)の角度です。[単位°]
* Rot Y ： メインカメラのY軸(画面左右方向)の角度です。[単位°]
* Rot Z ： メインカメラのZ軸(画面奥行き方向)の角度です[単位°]
* FOV ： メインカメラの視野角です。[単位°]
* Dist ： アバターとメインカメラまでの距離です[単位m]
* Move Speed ： メインカメラの右クリック+WASDでの移動速度です。ChroMapper設定のVisualsのCamera Speedと同じです。[5以下も可能]
* Look At ： メインカメラの角度をアバターの方向に向けます。
* Sub : コントロール対象をサブカメラにします。
* Preview[F4] : ゲーム内を正確に再現したプレビューモードにします、F4がショートカットキーで戻る時はF4です。
* Paste : Script Mapperのqコマンド形式または、input.csv形式(タブ区切り)で、クリップボードからカメラ位置・角度・FOVを設定します。
* Copy ： Script Mapperのinput.csv形式(タブ区切り)で、カメラ位置・角度・FOVをクリップボードにコピーします。
* q format ： Copy時にScriptMapperのqコマンド形式でコピーします。

上記項目はカメラを移動すると現在値を表示します。値を入力するとその状態にカメラが移動します。

## 設定ファイルについて
設定ファイルはChroMapperの設定ファイルと同じフォルダ`ユーザ設定フォルダ(Users)\ユーザ名\AppData\LocalLow\BinaryElement\ChroMapper`の`cameramovement.json`に保存されます。

プラグインのUIで設定できない項目の説明は以下です。

| 設定項目 | デフォルト値 | 説明 |
|:---|:---|:---|
| scriptMapperExe | scriptmapper.exe | ScriptMapperの実行ファイル名です。フォルダパスはChroMapper.exeの場所です |
| scriptMapperLog | log_latest.txt | ScriptMapperのログファイル名です。フォルダパスは編集中の譜面フォルダです |
| bookmarkLinesShowOnTop | False | Trueにすると、ブックマーク床面表示を最前面にします |
| avatarCameraScale | 1.5 | ChroMapper内のUnity単位をBeatSaberと合わせるための倍率です |
| originMatchOffsetY | -0.5 | ChroMapper内のY座標原点をBeatSaberと合わせるためのオフセット値です[avatarCameraScale反映前] |
| originMatchOffsetZ | -1.5 | ChroMapper内のZ座標原点をBeatSaberと合わせるためのオフセット値です[avatarCameraScale反映前] |
| originXoffset | 0 | 原点座標調整用のXオフセット値です[avatarCameraScale反映後] |
| originYoffset | 0 | 原点座標調整用のYオフセット値です[avatarCameraScale,originMatchOffsetY反映後] |
| originZoffset | 0 | 原点座標調整用のZオフセット値です[avatarCameraScale,originMatchOffsetZ反映後] |
| previewKeyBinding | ＜Keyboard＞/f4 |プレビューのショートカットキーバインドです |
| scriptMapperKeyBinding | ＜Keyboard＞/f3 | Script Mapper Runのショートカットキーバインドです |
| dragWindowKeyBinding | ＜Keyboard＞/shift | パネルドラッグ時の押すキーバインドです |
| orbitDefaultFOV | 60 | 注視点カメラ移動のデフォルトFOVです |
| orbitRotSensitivity | 0.5 | 注視点カメラ移動のカメラ回転感度です |
| orbitZoomSensitivity | 0.001 | 注視点カメラ移動のカメラズーム感度です |
| orbitOffsetSensitivity | 0.01 | 注視点カメラ移動のオフセット感度です |
| orbitFovSensitivity | 0.005 | 注視点カメラ移動のFOV変更感度です |
| orbitMinDistance | 0.2 | 注視点カメラ移動のズーム最接近距離です |
| orbitMaxDistance | 100 | 注視点カメラ移動のズーム最遠方距離です |
| orbitActiveKeyBinding | ＜Keyboard＞/alt | 注視点カメラ移動のキーバンドです |
| orbitSubActiveKeyBinding | ＜Keyboard＞/ctrl | 注視点カメラ移動のFOV変更キーバンドです |
| orbitMoveActiveKeyBinding | ＜Mouse＞/leftButton | 注視点カメラ移動のオフセット移動キーバンドです |
| orbitRotActiveKeyBinding | ＜Mouse＞/rightButton | 注視点カメラ移動のカメラ回転キーバンドです |

※キーバインドはUnityのInputSystem形式で設定してください。

## 補足
ChroMapperのブックマークはMMA2と同じ"B"キーですが、下のタイムライン上をクリックすると再編集できます。またブックマークの削除は、タイムライン上のブックマークをマウス中クリックです。

メイン画面のカメラ位置はF5～F8に登録して呼び出せます。(MMA2のBackSpaceの代わり)　登録はCtrl + F5～F8です。Shift+で更に登録できるので、合計8箇所登録できます。

# ToDo
※実生活がちょっと忙しくなってきているので、ゆっくり対応です。
## プレリリース中
* マルチディスプレイ表示モードの追加(プレビュー表示用、カメラレイアウト検討用）
* サブカメラだけマッピング用のUI表示を消す
* CameraPlus相当のマウス操作でのカメラ移動モードの追加
* マルチディスプレイでのデフォルトカメラ操作

## 実装中
* duration取込み誤差分をオフセットして、ブックマーク位置でスタート画面になるようにする
* プラグインのON/OFF機能の追加(通常作譜作業用にプラグインをワンタッチでON/OFFできるように)
* アバター非表示コマンドの対応
* カメラコントロールでサブカメラ対象の時に、カメラ操作もサブカメラを対象にする
* サブカメラにオブジェクトを割り当てて、カメラ移動が見えるモードの追加

## 検討中
* CameraMovementController.csが機能詰め込みすぎてるので整理する。
* ブックマークデータ単体の入出力（譜面.datとブックマークの分離)

## 保留
* ブックマークもノーツブロックのようにコピペ出来たら良いけど、だいぶムズい。

## 完了
* ~~カメラ移動をWASDではなく、CADみたいに対象物(アバター)中心で回転操作で動かすモード~~
* ~~現在のカメラ位置をinput.csvに出力するボタン（コピペは実装済み）~~ ※qコマンドが有用なのでこれ以上実装しない
* ~~ノーツや壁のスピードをゲームと一致させる~~
* ~~作譜用のショートカットキーが数値やコマンド入力の障害になるので、無効化チェックボックス~~
* ~~UI Hiddenで不要なUIを全部消す(ゲームのプレイ画面に近くする)~~
* ~~ブックマークの表示をMMA2の様に床に表示させたい、クリックして再編集~~
* ~~Script Mapperのコマンドをメニューで選択してブックマーク入力~~
* ~~VRMとか読み込んでアバター表示したいけど難しそう。 (カスタムアバターには対応しました！)~~

# 開発者情報
このプロジェクトをビルドするには、ChroMapperのインストールパスを指定する ChroMapper-CameraMovement\ChroMapper-CameraMovement.csproj.user ファイルを作成する必要があります。

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="Current" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <ChroMapperDir>C:\TOOL\ChroMapper\chromapper</ChroMapperDir>
  </PropertyGroup>
</Project>
```
## 添付ライブラリについて

### UniVRM
ChroMapper-CameraMovement.dllには[UniVRM](https://github.com/vrm-c/UniVRM) の `UniVRM-0.96.2_b978.unitypackage`でビルドしたDLLファイルをILMergeによって内包しています。
- FastSpringBone.dll
- MToon.dll
- UniGLTF.dll
- UniHumanoid.dll
- VRM.dll
- VRMShaders.GLTF.IO.Runtime.dll
- VRMShaders.GLTF.UniUnlit.Runtime.dll
- VRMShaders.VRM.IO.Runtime.dll
- netstandard.dll

また、以下のシェーダーをアセットバンドルで`vrmavatar.shaders`にまとめ、DLLに埋め込んでいます。
- Assets\VRMShaders\GLTF\UniUnlit\Resources\UniGLTF\UniUnlit.shader
- Assets\VRMShaders\VRM\MToon\MToon\Resources\Shaders\MToon.shader
- Assets\VRMShaders\GLTF\IO\Resources\UniGLTF\NormalMapExporter.shader
- Assets\VRMShaders\GLTF\IO\Resources\UniGLTF\StandardMapExporter.shader
- Assets\VRMShaders\GLTF\IO\Resources\UniGLTF\StandardMapImporter.shader

UniVRMの著作権表記・ライセンスは以下の通りです。
- https://github.com/vrm-c/UniVRM/blob/master/LICENSE.txt

### Json.NET
Movementスクリプトの読込に[Json.NET](https://www.newtonsoft.com/json)使用しています。`Newtonsoft.Json.dll`をILMergeによって内包しています。

Json.NETの著作権表記・ライセンスは以下の通りです。
- https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md

### SimpleJSON
設定ファイルのJSONパースに[SimpleJSON](https://github.com/Bunny83/SimpleJSON)を使用しています。

SimpleJSONの著作権表記・ライセンスは以下の通りです。
- https://github.com/Bunny83/SimpleJSON/blob/master/LICENSE

## アイコン素材
- ICONION
- http://iconion.com/ja/

## プラグイン製作の参考
`CameraPlus`フォルダのファイルは、すのーさんの[CameraPlus](https://github.com/Snow1226/CameraPlus)のソースコードをコピー・修正して使用しています。カメラ移動部分の処理は全く同じです。

CameraPlusの著作権表記・ライセンスは以下の通りです。
- https://github.com/Snow1226/CameraPlus/blob/master/LICENSE

`UI.cs`はKival Evanさん製作の[Lolighter](https://github.com/KivalEvan/ChroMapper-Lolighter)のソースコードをコピー・修正して使用しています。

Lolighterの著作権表記・ライセンスは以下の通りです。
- https://github.com/KivalEvan/ChroMapper-Lolighter/blob/main/LICENSE
