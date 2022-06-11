# ChroMapper-CameraMovement
[CameraPlus](https://github.com/Snow1226/CameraPlus)用の[MovementScript](https://github.com/Snow1226/CameraPlus/wiki/MovementScript)を読み込んで、作譜ツールの[ChroMapper](https://github.com/Caeden117/ChroMapper)でカメラワークの再現をするChroMapper用プラグインです。
主に[Script Mapper](https://github.com/hibit-at/Scriptmapper)を使ったカメラスクリプトの作成を前提とした作りになっています。

![image](https://user-images.githubusercontent.com/14249877/170858452-37e4b916-7bd1-4a0e-adbd-f737b534b707.png)

# インストール方法
1. [リリースページ](https://github.com/rynan4818/ChroMapper-CameraMovement/releases)から、最新版のプラグインのzipファイルをダウンロードして下さい。

2. ダウンロードしたzipファイルを解凍してChroMapperのインストールフォルダにある`Plugins`フォルダに`ChroMapper-CameraMovement.dll`をコピーします。

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
    - ALT + SHIFT + マウスホイールでカメラZ角度回転
- CameraPlus相当のマウスによるカメラ移動モード
    - Zキー + マウス右ドラッグでカメラ回転
    - Zキー + 中ドラッグでカメラ移動
    - Zキー + マウスホイールで視点方向前後移動 （右クリックは不要 ※CameraPlusと違います)
    - Zキー + 左クリック + マウスホイールでZ角度回転
    - Zキー + 左クリック + 中クリックでZを0度に設定
    - Zキー + 右クリック + マウスホイールでFOVの変更
    - Zキー + 右クリック + 中クリックでFOVのリセット

※上記キーバインドや感度は[設定ファイル](https://github.com/rynan4818/ChroMapper-CameraMovement#%E8%A8%AD%E5%AE%9A%E3%83%95%E3%82%A1%E3%82%A4%E3%83%AB%E3%81%AB%E3%81%A4%E3%81%84%E3%81%A6)の直接変更で変更可能

## 各パネル説明
譜面を読み込んでエディタ画面を出して下さい。**Tabキー**を押すと右側にアイコンパネルが出ますので、水色のカメラアイコンを押すと下の画像 CameraMovementの設定パネルが開きます。

![image](https://user-images.githubusercontent.com/14249877/170858312-2cec4b44-fed5-4248-83b0-a2bbc6df1278.png)

* CameraMovement ： CameraMovementプラグインの有効／無効切り替え。（作譜時に無効にする場合など)
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
* Multi Display ： マルチディスプレイパネルを表示します。
* Script Mapper Run [F3] ： 譜面データを保存して、Script Mapperでブックマークをカメラスクリプトに変換します。F3キーでショートカット。
* Setting Save ： 設定パネルの内容およびブックマーク編集パネルのコマンドボタンの内容を設定ファイルに保存します。
* Close ： パネルを閉じます。

![image](https://user-images.githubusercontent.com/14249877/173173694-493c7e73-fe88-4fee-99ed-5a9b339ccc0a.png)

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
* Bookmark Size(%) ： Bookmark Linesで表示する文字のサイズ(デフォルトは100%)
* Bookmark Export ： ブックマークをCSVファイルに出力します。
* ~~Bookmark Import ： ブックマークをCSVファイルから取り込みます。~~(未実装)
* Sub X ： サブカメラの左下座標の画面横軸位置です。（0が左端、1が右端)
* Sub Y ： サブカメラの左下座標の画面縦軸位置です。（0が下端、1が上端)
* Cursor Key Move ： 押すとカーソルキーでSub X,Yを0.01単位で調整できます。Shiftを押しながらだと0.1単位。
* Sub W ： サブカメラの画面の高さです。(1がメイン画面高さ)
* Sub H ： サブカメラの画面の幅です。(1がメイン画面幅)
* Cursor Key Size ： 押すとカーソルキーでSub W,Hを0.01単位で調整できます。Shiftを押しながらだと0.1単位。
* Script File ： 譜面フォルダにある読み込むカメラスクリプトファイル名
* Mapping Disable ： Camera Movementの機能有効中は譜面作成機能を抑制して、ノーツ等の誤編集を防止します。ただし、OFFにするとカメラ移動機能が制限されます。
* bookmark Lines Top ： Bookmark Linesを全てのオブジェクトの最前面に表示します。
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

![image](https://user-images.githubusercontent.com/14249877/170858357-af821054-add0-4c39-9c10-494e0fafc2ac.png)

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
* Lay : マルチウィンドウ時にコントロール対象をレイアウト検討カメラにします。
* Obj : サブカメラ表示時に、カメラの3Dオブジェクトを表示します。
* Preview[F4] : ゲーム内を正確に再現したプレビューモードにします、F4がショートカットキーで戻る時はF4です。
* Paste : Script Mapperのqコマンド形式または、input.csv形式(タブ区切り)で、クリップボードからカメラ位置・角度・FOVを設定します。
* Copy ： Script Mapperのinput.csv形式(タブ区切り)で、カメラ位置・角度・FOVをクリップボードにコピーします。
* q fmt ： Copy時にScriptMapperのqコマンド形式でコピーします。

上記項目はカメラを移動すると現在値を表示します。値を入力するとその状態にカメラが移動します。

![image](https://user-images.githubusercontent.com/14249877/170858377-6005ae8a-3855-45df-bd36-77764418ef90.png)

- Display Counts = * ： 現在のPCのディスプレイ数です。2以上で本機能を利用できます。3以上だとサブ・レイアウト検討の両方ウィンドウ化できます。
- Sub Window ： サブカメラ用のウィンドウを表示します。
- Layout Window ： レイアウト検討カメラのウィンドウを表示します。
- Create Window ： 現在の設定でマルチディスプレイ機能を有効にします。
- Save Window Layout ： 現在のウィンドウレイアウトを保存し、次回Create Windowした時に再現します。
- Reset Window Layout ： ウィンドウレイアウトをリセットします。
- Setting Save ： 現在の設定を保存します。
- Close ： パネルを閉じます。

※一度作成したウィンドウはUnityの仕様でChroMapperを終了するまで閉じることができません。

※各ウィンドウでのカメラ操作は、カメラコントロールパネルの Sub 及び Lay チェック時に値で直接変更できる他、ウィンドウがアクティブ状態の場合は各モードでマウス・キーボードでコントロールできます。

※サブ・レイアウト検討ウィンドウはモニタのアスペクト比に依存するため、サイズ変更はアスペクト比固定になっています。

※マルチディスプレイモード時にメインウィンドウのサイズを変更すると、画面が崩れることがありますがウィンドウをドラッグで移動すると直ります。

※作譜作業用には[MultiDisplayWindow](https://github.com/rynan4818/ChroMapper-MultiDisplayWindow)として、マルチディスプレイウインドウ機能を別プラグインにしました。ただし、マルチディスプレイは本プラグインと同時使用はできないため、切り替えるときには一度ChroMapperを再起動して下さい。

## Duration取込み誤差について
カメラスクリプトはDuration(移動時間)の積算で表現していますが、CameraPlusやCameraMovementで取り込む際に有効桁数6~9桁(float)で丸められるため、ScriptMapperの出力内容によっては誤差が積算されてブックマークの位置に対してカメラ移動がズレる可能性があります。また、Durationは0.01秒未満の値は0.01秒に繰り上げるため、0.01秒未満のDurationがあるとズレが積算します。

特に遅れる方向にズレやすく、CameraMovementで表示する際に後ろの方になると、ブックマークの位置では一つ前の最終カメラ位置になってしまうことがあるため、サブカメラのプレビューでは内部でズレを補正しています。この補正の上限値はデフォルトで0.05秒(50ms)で、これよりもズレが大きくなると下記エラーメッセージが表示され、ズレていることを分かるようにするため補正されなくなります。

![image](https://user-images.githubusercontent.com/14249877/170859030-71a35442-9563-49e3-8af8-9ccf834c9328.png)

ズレが発生した場合の対処方法は、ズレ対策されたScriptMapperを利用する。ScriptMapperの環境コマンドのオフセット `#offset0.1`(例0.1秒早める)を利用するなどがあります。

取込み誤差は通常時にもログに表示していますので、どのくらいズレているか確認することも可能です。
- Duration total error : スクリプト末尾時点でのズレ時間
- Duration max error : スクリプトの最大ズレ時間
- Duration min error : スクリプトのマイナス方向の最大ズレ時間

ログ表示画面は通常ではChromapperのキーバインドでバッククオートになっていますが、日本語キーボードだと反応しないため、キーバンドのDebugのToggle Debug Consoleを変更して下さい。(F12など)

## 設定ファイルについて
設定ファイルはChroMapperの設定ファイルと同じフォルダ`ユーザ設定フォルダ(Users)\ユーザ名\AppData\LocalLow\BinaryElement\ChroMapper`の`cameramovement.json`に保存されます。

プラグインのUIで設定できない項目の説明は以下です。

| 設定項目 | デフォルト値 | 説明 |
|:---|:---|:---|
| scriptMapperExe | scriptmapper.exe | ScriptMapperの実行ファイル名です。フォルダパスはChroMapper.exeの場所です |
| scriptMapperLog | log_latest.txt | ScriptMapperのログファイル名です。フォルダパスは編集中の譜面フォルダです |
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
| orbitActiveKeyBinding | ＜Keyboard＞/alt | 注視点カメラ移動のキーバインドです |
| orbitSubActiveKeyBinding | ＜Keyboard＞/ctrl | 注視点カメラ移動のFOV変更キーバインドです |
| orbitZrotActiveKeyBinding | ＜Keyboard＞/shift | 注視点カメラ移動のZカメラ回転キーバインドです |
| orbitMoveActiveKeyBinding | ＜Mouse＞/leftButton | 注視点カメラ移動のオフセット移動キーバインドです |
| orbitRotActiveKeyBinding | ＜Mouse＞/rightButton | 注視点カメラ移動のXYカメラ回転キーバインドです |
| subCameraNoUI | True | TrueにするとサブカメラのUI表示を消します |
| layoutCameraNoUI | True | Trueにするとレイアウト検討カメラのUI表示を消します |
| multiDislayCreateDelay | 0.1 | マルチディスプレイ作成時のディレイ時間(秒)。作成時に異常動作する場合は値を増やして下さい |
| plusActiveKeyBinding | ＜Keyboard＞/z | CameraPlus互換カメラ移動の有効キーバインドです |
| plusZrotActiveKeyBinding | ＜Mouse＞/leftButton | CameraPlus互換カメラ移動のZカメラ回転キーバインドです |
| plusPosActiveKeyBinding | ＜Mouse＞/middleButton | CameraPlus互換カメラ移動のカメラ移動キーバインドです |
| plusRotActiveKeyBinding | ＜Mouse＞/rightButton | CameraPlus互換カメラ移動のXYカメラ回転キーバインドです |
| plusZrotSensitivity | 0.004165 | CameraPlus互換カメラ移動のZ回転感度です |
| plusPosSensitivity | 0.01 | CameraPlus互換カメラ移動のカメラ移動感度です |
| plusRotSensitivity | 0.03 | CameraPlus互換カメラ移動のXYカメラ回転感度です |
| plusZoomSensitivity | 0.003125 | CameraPlus互換カメラ移動の視点方向前後移動の感度です |
| plusFovSensitivity | 0.005 | CameraPlus互換カメラ移動のFOV変更感度です |
| plusDefaultFOV | 60 | CameraPlus互換カメラ移動のデフォルトFOVです |
| defaultCameraActiveKeyBinding | ＜Mouse＞/rightButton | サブ・レイアウト検討カメラ用デフォルトカメラ移動の有効キーバインドです |
| defaultCameraElevatePositiveKeyBinding | ＜Keyboard＞/space | サブ・レイアウト検討カメラ用デフォルトカメラ移動の上移動キーバインドです |
| defaultCameraElevateNegativeKeyBinding | ＜Keyboard＞/ctrl | サブ・レイアウト検討カメラ用デフォルトカメラ移動の下移動キーバインドです |
| defaultCameraMoveUpKeyBinding | ＜Keyboard＞/w | サブ・レイアウト検討カメラ用デフォルトカメラ移動の前移動キーバインドです |
| defaultCameraMoveLeftKeyBinding | ＜Keyboard＞/a | サブ・レイアウト検討カメラ用デフォルトカメラ移動の左移動キーバインドです |
| defaultCameraMoveDownKeyBinding | ＜Keyboard＞/s | サブ・レイアウト検討カメラ用デフォルトカメラ移動の後移動キーバインドです |
| defaultCameraMoveRightKeyBinding | ＜Keyboard＞/d | サブ・レイアウト検討カメラ用デフォルトカメラ移動の右移動キーバインドです |
| maxDurationErrorOffset | True | Trueにすると、サブカメラでDuration取込み誤差補正機能を有効にします |
| durationErrorWarning | 0.05 | Duration取込み誤差補正機能の上限値(秒)です。この値を超えると補正されず警告表示します |
| cameraKeyMouseControlSub | True | Trueにすると、非マルチディスプレイ時にサブカメラのキーボード・マウス移動対象にします |
| subCameraModelTrailTime | 4 | サブカメラの3Dオブジェクトのトレイル表示時間(秒)です |

※キーバインドはUnityのInputSystem形式で設定してください。

## 補足
ChroMapperのブックマークはMMA2と同じ"B"キーですが、下のタイムライン上をクリックすると再編集できます。またブックマークの削除は、タイムライン上のブックマークをマウス中クリックです。

メイン画面のカメラ位置はF5～F8に登録して呼び出せます。(MMA2のBackSpaceの代わり)　登録はCtrl + F5～F8です。Shift+で更に登録できるので、合計8箇所登録できます。

# ToDo
※実生活がちょっと忙しくなってきているので、ゆっくり対応です。

## 検討中
* CameraMovementController.csが機能詰め込みすぎてるので整理する。

## 保留
* ブックマークデータ単体の入出力（譜面.datとブックマークの分離) CSV出力は実装済み、入力はエディタ起動中は難しい。譜面データを直接扱う別ツールにしたほうがずっと簡単。
* ブックマークもノーツブロックのようにコピペ出来たら良いけど、だいぶムズい。
* カメラ移動パスを描いて、それ通りに動くようなブックマークを自動生成できたら便利だけど、ムズ～

## 完了
* ~~マルチディスプレイ表示モードの追加(プレビュー表示用、カメラレイアウト検討用）~~
* ~~サブカメラだけマッピング用のUI表示を消す~~
* ~~CameraPlus相当のマウス操作でのカメラ移動モードの追加~~
* ~~マルチディスプレイでのデフォルトカメラ操作~~
* ~~duration取込み誤差分をオフセットして、ブックマーク位置でスタート画面になるようにする~~
* ~~プラグインのON/OFF機能の追加(通常作譜作業用にプラグインをワンタッチでON/OFFできるように)~~
* ~~アバター非表示コマンドの対応~~
* ~~カメラコントロールでサブカメラ対象の時に、カメラ操作もサブカメラを対象にする~~
* ~~サブカメラにオブジェクトを割り当てて、カメラ移動が見えるモードの追加~~
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
- Google Fonts (Monochrome Photos)
- https://fonts.google.com/icons?selected=Material+Icons:monochrome_photos:

## プラグイン製作の参考
`CameraPlus`フォルダのファイルは、すのーさんの[CameraPlus](https://github.com/Snow1226/CameraPlus)のソースコードをコピー・修正して使用しています。カメラ移動部分の処理は全く同じです。

CameraPlusの著作権表記・ライセンスは以下の通りです。
- https://github.com/Snow1226/CameraPlus/blob/master/LICENSE

`UI.cs`はKival Evanさん製作の[Lolighter](https://github.com/KivalEvan/ChroMapper-Lolighter)のソースコードをコピー・修正して使用しています。

Lolighterの著作権表記・ライセンスは以下の通りです。
- https://github.com/KivalEvan/ChroMapper-Lolighter/blob/main/LICENSE
