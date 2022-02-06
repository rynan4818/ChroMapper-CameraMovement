# ChroMapper-CameraMovement
[CameraPlus](https://github.com/Snow1226/CameraPlus)用の[MovementScript](https://github.com/Snow1226/CameraPlus/wiki/MovementScript)を読み込んで、作譜ツールの[ChroMapper](https://github.com/Caeden117/ChroMapper)でカメラワークの再現をするChroMapper用プラグインです。
主に[Script Mapper](https://github.com/hibit-at/Scriptmapper)を使ったカメラスクリプトの作成を前提とした作りになっています。

![image](https://user-images.githubusercontent.com/14249877/152683121-afceb1e1-ef7b-497b-a30e-cd6e3230a784.png)

# インストール方法
1. [リリースページ](https://github.com/rynan4818/ChroMapper-CameraMovement/releases)から、最新版のプラグインのzipファイルをダウンロードして下さい。

2. ChroMapperのインストールフォルダにある`Plugins`フォルダに、ダウンロードしたzipファイルを解凍して`ChroMapper-CameraMovement.dll`と`Newtonsoft.Json.dll`をコピーして下さい。

3. [Script Mapper](https://github.com/hibit-at/Scriptmapper)をダウンロードして、ChroMapperのインストールフォルダ(ChroMapper.exeがあるフォルダ)に`scriptmapper.exe`をコピーします。

# 使用法
譜面を読み込んでエディタ画面を出して下さい。Tabキーを押すと右側にアイコンパネルが出ますので、水色のカメラアイコンを押すと下の画像 CameraMovementの設定パネルが開きます。

![image](https://user-images.githubusercontent.com/14249877/152683142-b7a21ccf-e509-487b-8b38-a075da027b1f.png)

* Movement Enable ： カメラスクリプトに合わせてカメラが移動します。
* UI Hidden ： 作譜用のグリッドUIなどを消します。(まだ床など一部が消えません)
* Turn To Head ： カメラスクリプトのTurnToHeadUseCameraSettingの設定がtrueの時に、カメラがアバターの方向を自動で向きます。(CameraPlusのTurnToHeadパラメータに相当します。)
* Avatar ： アバターの3Dオブジェクトを表示します。
* Head Hight ： アバターの頭の高さ（球の中心）[単位 m]
* Head Size ： アバターの頭の大きさ(球の直径) [単位 m]
* Arm Size ： アバターの両手の長さ [単位 m]
* Script File ： 譜面フォルダにある読み込むカメラスクリプトファイル名
* Cam Pos Rot ： 現在のカメラ位置 (読み取り専用)
* Reload ： 設定などを手動で読み込み直す
* Setting Save : 上記設定を保存する
* Script Mapper Run ： 譜面データを保存して、Script Mapperでブックマークをカメラスクリプトに変換します。

## 補足
ChroMapperのブックマークはMMA2と同じ"B"キーですが、下のタイムライン上をクリックすると最編集できます。またブックマークの削除は、タイムライン上のブックマークをマウス中クリックです。

# ToDo
* UI Hiddenで不要なUIを全部消す(ゲームのプレイ画面に近くする)
* ブックマークの表示をMMA2の様に床に表示させたい、クリックして再編集
* Script Mapperのコマンドをメニューで選択してブックマーク入力
* 現在のカメラ位置をinput.csvに出力するボタン
* VRMとか読み込んでアバター表示したいけど難しそう。

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

## プラグイン製作の参考
`CameraMovement.cs`の大半は、すのーさん製作のCameraPlusの[CameraMovement.cs](https://github.com/Snow1226/CameraPlus/blob/master/CameraPlus/Behaviours/CameraMovement.cs)をコピーして作成しています。カメラ移動部分は全く同じです。

`UI.cs`の大半はKival Evanさん製作の[Lolighter](https://github.com/KivalEvan/ChroMapper-Lolighter)の[UI.cs](https://github.com/KivalEvan/ChroMapper-Lolighter/blob/main/UI/UI.cs)をコピーして作成しています。
