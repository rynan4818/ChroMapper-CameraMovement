# ChroMapper-CameraMovement
[CameraPlus](https://github.com/Snow1226/CameraPlus)用の[MovementScript](https://github.com/Snow1226/CameraPlus/wiki/MovementScript)を読み込んで、作譜ツールの[ChroMapper](https://github.com/Caeden117/ChroMapper)でカメラワークの再現をするChroMapper用プラグインです。
主に[Script Mapper](https://github.com/hibit-at/Scriptmapper)を使ったカメラスクリプトの作成を前提とした作りになっています。

![image](https://user-images.githubusercontent.com/14249877/154273423-3d6bdcfe-a859-472b-9381-a7bf6e9a300b.png)

# インストール方法
1. [リリースページ](https://github.com/rynan4818/ChroMapper-CameraMovement/releases)から、最新版のプラグインのzipファイルをダウンロードして下さい。

2. ChroMapperのインストールフォルダにある`Plugins`フォルダに、ダウンロードしたzipファイルを解凍して`ChroMapper-CameraMovement.dll`と`Newtonsoft.Json.dll`をコピーして下さい。

3. [Script Mapper](https://github.com/hibit-at/Scriptmapper)をダウンロードして、ChroMapperのインストールフォルダ(ChroMapper.exeがあるフォルダ)に`scriptmapper.exe`をコピーします。

# アバターについて
[カスタムアバター](https://modelsaber.com/Avatars/)の3Dアバターを読み込んで表示できます。

VRM、VRChat用のUnityプロジェクト・パッケージなど他のアバター形式を読み込ませるには[CameraMovementAvatarExporterパッケージ](https://github.com/rynan4818/CameraMovementAvatarExporter)を使用してUnityからCameraMovement専用のカスタムアバターを出力して下さい。

[VRMを直接読み込む検討](https://github.com/rynan4818/ChroMapper-VRMAvatar)はしましたが断念しました。

# 使用法
譜面を読み込んでエディタ画面を出して下さい。Tabキーを押すと右側にアイコンパネルが出ますので、水色のカメラアイコンを押すと下の画像 CameraMovementの設定パネルが開きます。

![image](https://user-images.githubusercontent.com/14249877/154273595-d9aaa0c6-3608-4a70-ba07-d250c50b0c1f.png)

* Movement Enable ： カメラスクリプトに合わせてカメラが移動します。
* UI Hidden ： 作譜用のグリッドUI、ライティングブロック、音声波形などを消します。
* Turn To Head ： カメラスクリプトのTurnToHeadUseCameraSettingの設定がtrueの時に、カメラがアバターの方向を自動で向きます。(CameraPlusのTurnToHeadパラメータに相当します。)
* Avatar ： アバターを表示します。
* Bookmark Lines ： ブックマークの内容をノーツブロックレーンの右側に表示します。
* Sub Camera ： 常にスクリプトに合わせて動くサブカメラを表示します。
* Bookmark Edit ： ブックマークの編集パネルを下に表示します。
* Cam Pos Rot ： 現在のカメラ位置 (読み取り専用)
* More Seting ： 詳細設定パネルを表示します。
* Reload ： 設定やスクリプトを読み込み直します
* Setting Save : 設定パネルの内容およびブックマーク編集パネルのコマンドボタンの内容を設定ファイルに保存します。
* Script Mapper Run ： 譜面データを保存して、Script Mapperでブックマークをカメラスクリプトに変換します。

![image](https://user-images.githubusercontent.com/14249877/154273697-8b92c442-e352-4206-85b0-706886192d78.png)

* Custom Avator ： 読み込んだカスタムアバターモデルを表示します。
* Avatar File ： [*.avatar] [カスタムアバターのファイル](https://modelsaber.com/Avatars/)をChroMapper.exeと同じフォルダに置いてファイル名を設定して下さい。デフォルト設定は[Sour Miku Black v2](https://modelsaber.com/Avatars/?id=1564625718&pc)になっています
* Avatar Scale ： アバターのサイズ調整用です。実際のアバターのサイズはBeatSaber内でキャリブレーション後に何らかの方法で測定する必要があります。(例えばCameraPlus等で三人称カメラを自分の頭上丁度にミラーなど見ながら配置することで、カメラY座標から判断するなど)
* Avatar Y offset ： アバターのY位置調整用です。
* Simple Avatar: 従来のピンクの棒人間アバターを表示します。本プラグインで読み込んだカスタムアバターのサイズ測定に使えます。(Head Hight + Head Size/2 = 身長[m]です。) ※Sour Miku Black v2は1.72m
* Head Hight ： アバターの頭の高さ（球の中心）[単位 m]
* Head Size ： アバターの頭の大きさ(球の直径) [単位 m]
* Arm Size ： アバターの両手の長さ [単位 m]
* Bookmark Width ： タイムラインのブックマークの表示幅を調整します。(デフォルトは10)
* Offset Y ： アバターとカメラの原点Y位置をオフセットします。(ステージ中央は-0.5)
* Offset Z ： アバターとカメラの原点Z位置をオフセットします。(ステージ中央は-1.5)
* Bookmark Area ： Bookmark Linesで表示するために開けるスペース。
* Sub Rect X ： サブカメラの左下座標の画面横軸位置です。（0が左端、1が右端)
* Sub Rect Y ： サブカメラの左下座標の画面縦軸位置です。（0が下端、1が上端)
* Sub Rect W ： サブカメラの画面の高さです。(1がメイン画面高さ)
* Sub Rect H ： サブカメラの画面の幅です。(1がメイン画面幅)
* Script File ： 譜面フォルダにある読み込むカメラスクリプトファイル名
* Setting Save ： 設定パネルの内容およびブックマーク編集パネルのコマンドボタンの内容を設定ファイルに保存します。(メイン設定の同名ボタンと機能は同じ)

![image](https://user-images.githubusercontent.com/14249877/154273925-45361056-d4bd-4249-b738-0d9b2085548c.png)

* Current Bookmark No.- ： 現在の編集対象のブックマークNo.です。
* `center3` ： 設定されたブックマークの内容が表示されます。
* `center`～`random` ： よく使用するコマンドを登録して呼び出せます。
* Set ： チェックしてから上の６箇所のボタンを押すと`Empty` の部分に入力した内容が登録できます。
* Copy to Edit ： 現在のブックマーク内容を`Empty` の部分にコピーします。
* New ： `Empty` の内容をブックマークとして新規登録します。
* Change ： 現在のブックマークの内容を修正します。
* Delete ： 現在のブックマークを削除します。

## 補足
ChroMapperのブックマークはMMA2と同じ"B"キーですが、下のタイムライン上をクリックすると最編集できます。またブックマークの削除は、タイムライン上のブックマークをマウス中クリックです。

メイン画面のカメラ位置はF5～F8に登録して呼び出せます。(MMA2のBackSpaceの代わり)　登録はCtrl + F5～F8です。Shift+で更に登録できるので、合計8箇所登録できます。

# ToDo
* ~~UI Hiddenで不要なUIを全部消す(ゲームのプレイ画面に近くする)~~
* ~~ブックマークの表示をMMA2の様に床に表示させたい、クリックして再編集~~
* ~~Script Mapperのコマンドをメニューで選択してブックマーク入力~~
* 現在のカメラ位置をinput.csvに出力するボタン
* VRMとか読み込んでアバター表示したいけど難しそう。 (カスタムアバターには対応しました！)
* ブックマークデータ単体の入出力（譜面.datとブックマークの分離)
* ブックマークもノーツブロックのようにコピペ出来たら良いけど、だいぶムズい。

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
