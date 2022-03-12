# ChroMapper-CameraMovement
[CameraPlus](https://github.com/Snow1226/CameraPlus)用の[MovementScript](https://github.com/Snow1226/CameraPlus/wiki/MovementScript)を読み込んで、作譜ツールの[ChroMapper](https://github.com/Caeden117/ChroMapper)でカメラワークの再現をするChroMapper用プラグインです。
主に[Script Mapper](https://github.com/hibit-at/Scriptmapper)を使ったカメラスクリプトの作成を前提とした作りになっています。

![image](https://user-images.githubusercontent.com/14249877/155873272-56ef7f1e-cfe5-428a-9092-3185ce923a8b.png)

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
* Move Speed ： メインカメラの右クリック+WASDでの移動速度です。
* Look At ： メインカメラの角度をアバターの方向に向けます。
* Paste : Script Mapperのinput.csv形式(タブ区切り)で、クリップボードからカメラ位置・角度・FOVを設定します。
* Copy ： Script Mapperのinput.csv形式(タブ区切り)で、カメラ位置・角度・FOVをクリップボードにコピーします。

上記項目はカメラを移動すると現在値を表示します。値を入力するとその状態にカメラが移動します。

## 補足
ChroMapperのブックマークはMMA2と同じ"B"キーですが、下のタイムライン上をクリックすると再編集できます。またブックマークの削除は、タイムライン上のブックマークをマウス中クリックです。

メイン画面のカメラ位置はF5～F8に登録して呼び出せます。(MMA2のBackSpaceの代わり)　登録はCtrl + F5～F8です。Shift+で更に登録できるので、合計8箇所登録できます。

# ToDo
* 現在のカメラ位置をinput.csvに出力するボタン（コピペは実装済み）
* ブックマークデータ単体の入出力（譜面.datとブックマークの分離)
* ノーツや壁のスピードをゲームと一致させる
* ブックマークもノーツブロックのようにコピペ出来たら良いけど、だいぶムズい。
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
# 添付ライブラリについて

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
