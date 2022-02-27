# ChroMapper-CameraMovement
[CameraPlus](https://github.com/Snow1226/CameraPlus)用の[MovementScript](https://github.com/Snow1226/CameraPlus/wiki/MovementScript)を読み込んで、作譜ツールの[ChroMapper](https://github.com/Caeden117/ChroMapper)でカメラワークの再現をするChroMapper用プラグインです。
主に[Script Mapper](https://github.com/hibit-at/Scriptmapper)を使ったカメラスクリプトの作成を前提とした作りになっています。

![image](https://user-images.githubusercontent.com/14249877/155873272-56ef7f1e-cfe5-428a-9092-3185ce923a8b.png)

# インストール方法
1. [リリースページ](https://github.com/rynan4818/ChroMapper-CameraMovement/releases)から、最新版のプラグインのzipファイルをダウンロードして下さい。

2. ダウンロードしたzipファイルを解凍して以下の通りファイルをコピーして下さい。
    - `ChroMapperのインストールフォルダ`(ChroMapper.exeがあるフォルダ ※CML.exeでは無いです)に以下の８つのライブラリファイルをコピーします。
        - netstandard.dll
        - Newtonsoft.Json.dll ・・・ (以前はPluginsフォルダに入れていましたが変更しました)
        - UniGLTF.dll
        - UniHumanoid.dll
        - VRM.dll
        - VRMShaders.GLTF.IO.Runtime.dll
        - VRMShaders.GLTF.UniUnlit.Runtime.dll
        - vrmavatar.shaders
    - `Plugins` フォルダに本プラグインをコピーします。
        - ChroMapper-CameraMovement.dll

    ライブラリのDLLファイルは`ChroMapper_Data\Managed`フォルダでも動作しますが、`vrmavatar.shaders`は ChroMapper.exe があるフォルダに必ずコピーして下さい。

3. [Script Mapper](https://github.com/hibit-at/Scriptmapper)をダウンロードして、ChroMapperのインストールフォルダ(ChroMapper.exeがあるフォルダ)に`scriptmapper.exe`をコピーします。

# アバターについて
[カスタムアバター](https://modelsaber.com/Avatars/) および VRM の3Dアバターを読み込んで表示できます。

本プラグイン用のカスタムアバターの作成は[CameraMovementAvatarExporterパッケージ](https://github.com/rynan4818/CameraMovementAvatarExporter)を使用すると簡単です。

VRM対応の副産物として[ChroMapper-VRMAvatar](https://github.com/rynan4818/ChroMapper-VRMAvatar)を作りました。VRMアバターをChroMapperに読み込む単独で動くプラグインです。

# 使用法
譜面を読み込んでエディタ画面を出して下さい。Tabキーを押すと右側にアイコンパネルが出ますので、水色のカメラアイコンを押すと下の画像 CameraMovementの設定パネルが開きます。

![image](https://user-images.githubusercontent.com/14249877/155874695-cdf4c42f-1620-4020-9855-f19a1c18963d.png)

* Movement Enable ： カメラスクリプトに合わせてカメラが移動します。
* UI Hidden ： 作譜用のグリッドUI、ライティングブロック、音声波形などを消します。
* Turn To Head ： カメラスクリプトのTurnToHeadUseCameraSettingの設定がtrueの時に、カメラがアバターの方向を自動で向きます。(CameraPlusのTurnToHeadパラメータに相当します。)
* Avatar ： アバターを表示します。
* Bookmark Lines ： ブックマークの内容をノーツブロックレーンの右側に表示します。
* Sub Camera ： 常にスクリプトに合わせて動くサブカメラを表示します。
* Bookmark Edit ： ブックマークの編集パネルを下に表示します。
* Camera Contral ： カメラコントロールのパネルを下に表示します。
* More Seting ： 詳細設定パネルを表示します。
* Reload ： 設定やスクリプトを読み込み直します
* Setting Save : 設定パネルの内容およびブックマーク編集パネルのコマンドボタンの内容を設定ファイルに保存します。
* Script Mapper Run ： 譜面データを保存して、Script Mapperでブックマークをカメラスクリプトに変換します。

![image](https://user-images.githubusercontent.com/14249877/155874730-eca4ba00-9d26-43e6-a7d8-ae9b97b44f52.png)

* Custom or VRM Avatar ： 読み込んだカスタムアバター・VRMアバターモデルを表示します。
* Avatar File ： `*.vrm` VRMファイルもしくは、`*.avatar` [カスタムアバターのファイル](https://modelsaber.com/Avatars/)を`Select`ボタンで選択します。デフォルト設定はChroMapper.exeと同じフォルダにある[Sour Miku Black v2](https://modelsaber.com/Avatars/?id=1564625718&pc)になっています。
* Reload ： アバターファイルを変更した場合の再読み込みボタンです。
* Select ： アバターファイルを選択します。
* Avatar Scale ： アバターのサイズ調整用です。実際のアバターのサイズはBeatSaber内でキャリブレーション後に何らかの方法で測定する必要があります。(アバターの半身が映るぐらいの画角で、ゲーム内でスクリプトを動かした録画ファイルと、本プラグインで同じサイズになるように調整するのが一番楽だと思います)
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
* Bookmark Import ： ブックマークをCSVファイルから取り込みます。(未実装)
* Sub Rect X ： サブカメラの左下座標の画面横軸位置です。（0が左端、1が右端)
* Sub Rect Y ： サブカメラの左下座標の画面縦軸位置です。（0が下端、1が上端)
* Sub Rect W ： サブカメラの画面の高さです。(1がメイン画面高さ)
* Sub Rect H ： サブカメラの画面の幅です。(1がメイン画面幅)
* Script File ： 譜面フォルダにある読み込むカメラスクリプトファイル名
* Setting Save ： 設定パネルの内容およびブックマーク編集パネルのコマンドボタンの内容を設定ファイルに保存します。(メイン設定の同名ボタンと機能は同じ)
* Close ： More Settingパネルを閉じます。

![image](https://user-images.githubusercontent.com/14249877/154273925-45361056-d4bd-4249-b738-0d9b2085548c.png)

* Current Bookmark No.- ： 現在の編集対象のブックマークNo.です。
* `center3` ： 設定されたブックマークの内容が表示されます。
* `center`～`random` ： よく使用するコマンドを登録して呼び出せます。
* Set ： チェックしてから上の６箇所のボタンを押すと`Empty` の部分に入力した内容が登録できます。
* Copy to Edit ： 現在のブックマーク内容を`Empty` の部分にコピーします。
* New ： `Empty` の内容をブックマークとして新規登録します。
* Change ： 現在のブックマークの内容を修正します。
* Delete ： 現在のブックマークを削除します。

![image](https://user-images.githubusercontent.com/14249877/155875168-a39690fd-3c1f-4152-a6ce-1cec1d1a7826.png)

* Pos X ： メインカメラのX軸(横方向)位置です[単位m]
* Pos Y ： メインカメラのY軸(高さ方向)位置です[単位m]
* Pos Z ： メインカメラのZ軸(奥行き方向)位置です[単位m]
* Rot X ： メインカメラのX軸(画面上下方向)の角度です。[単位°]
* Rot Y ： メインカメラのY軸(画面左右方向)の角度です。[単位°]
* Rot Z ： メインカメラのZ軸(画面奥行き方向)の角度です[単位°]
* FOV ： メインカメラの視野角です。[単位°]
* Dist ： アバターとメインカメラまでの距離です[単位m]
* Move Speed ： メインカメラの移動速度です。
* Look At ： メインカメラの角度をアバターの方向に向けます。
* Paste : Script Mapperのinput.csv形式(タブ区切り)で、クリップボードからカメラ位置・角度・FOVを設定します。
* Copy ： Script Mapperのinput.csv形式(タブ区切り)で、カメラ位置・角度・FOVをクリップボードにコピーします。

上記項目はカメラを移動すると現在値を表示します。値を入力するとその状態にカメラが移動します。

## 補足
ChroMapperのブックマークはMMA2と同じ"B"キーですが、下のタイムライン上をクリックすると最編集できます。またブックマークの削除は、タイムライン上のブックマークをマウス中クリックです。

メイン画面のカメラ位置はF5～F8に登録して呼び出せます。(MMA2のBackSpaceの代わり)　登録はCtrl + F5～F8です。Shift+で更に登録できるので、合計8箇所登録できます。

# ToDo
* ~~UI Hiddenで不要なUIを全部消す(ゲームのプレイ画面に近くする)~~
* ~~ブックマークの表示をMMA2の様に床に表示させたい、クリックして再編集~~
* ~~Script Mapperのコマンドをメニューで選択してブックマーク入力~~
* 現在のカメラ位置をinput.csvに出力するボタン
* ~~VRMとか読み込んでアバター表示したいけど難しそう。 (カスタムアバターには対応しました！)~~
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
# 添付ライブラリについて

### UniVRM
以下のファイルは [UniVRM](https://github.com/vrm-c/UniVRM) の `UniVRM-0.95.1_6465.unitypackage`でビルドしたDLLファイルを使用しています。`vrmavatar.shaders`は同パッケージのアセットから `UniUnlit.shader`をアセットバンドルで出力した物です。
- UniGLTF.dll
- UniHumanoid.dll
- VRM.dll
- VRMShaders.GLTF.IO.Runtime.dll
- VRMShaders.GLTF.UniUnlit.Runtime.dll
- vrmavatar.shaders
- netstandard.dll

ライセンス : https://github.com/vrm-c/UniVRM/blob/master/LICENSE.txt

### Newtonsoft.Json
Movementスクリプトの読込に使用しています。
Newtonsoft.Json.dll の配布先・ライセンスは以下です。

配布先 : https://www.newtonsoft.com/json

ライセンス : https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md

### SimpleJSON
設定ファイルのJSONパースに[SimpleJSON](https://github.com/Bunny83/SimpleJSON)を使用しています。

ライセンス： https://github.com/Bunny83/SimpleJSON/blob/master/LICENSE

## プラグイン製作の参考
`CameraMovement.cs`の大半は、すのーさん製作のCameraPlusの[CameraMovement.cs](https://github.com/Snow1226/CameraPlus/blob/master/CameraPlus/Behaviours/CameraMovement.cs)をコピーして作成しています。カメラ移動部分は全く同じです。

`UI.cs`の大半はKival Evanさん製作の[Lolighter](https://github.com/KivalEvan/ChroMapper-Lolighter)の[UI.cs](https://github.com/KivalEvan/ChroMapper-Lolighter/blob/main/UI/UI.cs)をコピーして作成しています。
