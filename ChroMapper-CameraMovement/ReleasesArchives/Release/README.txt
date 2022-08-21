■ChroMapper-CameraMovement　インストール方法

・ChroMapperのインストールフォルダにあるPluginsフォルダに ChroMapper-CameraMovement.dll をコピーします。

・Script Mapper(https://github.com/hibit-at/Scriptmapper)をダウンロードして、ChroMapperのインストールフォルダ(ChroMapper.exeがあるフォルダ)にscriptmapper.exeをコピーします。

■ChroMapper-CameraMovementのライセンスについて

MITライセンスで配布します。著作権表記・詳細は以下の通りです。
https://github.com/rynan4818/ChroMapper-CameraMovement/blob/main/LICENSE

■使用ライブラリ等の著作権、ライセンスについて

○ChroMapper-CameraMovement.dllには UniVRM (https://github.com/vrm-c/UniVRM) の
  UniVRM-0.96.2_b978.unitypackage でビルドした以下のDLLファイルをILMergeによって内包しています。

        FastSpringBone.dll
        MToon.dll
        netstandard.dll
        UniGLTF.dll
        UniHumanoid.dll
        VRM.dll
        VRMShaders.GLTF.IO.Runtime.dll
        VRMShaders.GLTF.UniUnlit.Runtime.dll
        VRMShaders.VRM.IO.Runtime.dll
        
  また、以下のシェーダーをアセットバンドルで vrmavatar.shaders にまとめ、DLLに埋め込んでいます。
  Assets\VRMShaders\GLTF\UniUnlit\Resources\UniGLTF\UniUnlit.shader
  Assets\VRMShaders\VRM\MToon\MToon\Resources\Shaders\MToon.shader
  Assets\VRMShaders\GLTF\IO\Resources\UniGLTF\NormalMapExporter.shader
  Assets\VRMShaders\GLTF\IO\Resources\UniGLTF\StandardMapExporter.shader
  Assets\VRMShaders\GLTF\IO\Resources\UniGLTF\StandardMapImporter.shader

  ※ChroMapperはURP（Universal Render Pipeline）を使用していますが、現時点ではMToonはURPに対応していなため
    UniVRM内でUniUnlitに変換されて表示しています。

UniVRMの著作権表記・ライセンスは以下の通りです。
https://github.com/vrm-c/UniVRM/blob/master/LICENSE.txt

○Json.NET(https://www.newtonsoft.com/json)ライブラリの Newtonsoft.Json.dll をILMergeによって内包しています。

Json.NETの著作権表記・ライセンスは以下の通りです。
https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md

○SimpleJSON

設定ファイルのJSONパースにSimpleJSON(https://github.com/Bunny83/SimpleJSON)を使用しています。

SimpleJSONの著作権表記・ライセンスは以下の通りです。
https://github.com/Bunny83/SimpleJSON/blob/master/LICENSE

○アイコン素材について

ICONIONで作成しました。
http://iconion.com/ja/

○ソースコードの参考、コピー

カメラスクリプトの処理はCameraPlusを流用しています、著作権表記・ライセンスは以下の通りです。
https://github.com/Snow1226/CameraPlus/blob/master/LICENSE

UIの参考としてLolighterを流用しています、著作権表記・ライセンスは以下の通りです。
https://github.com/KivalEvan/ChroMapper-Lolighter/blob/main/LICENSE

BlendShapeの参考としてVirtualMotionCaptureを流用しています、著作権表記・ライセンスは以下の通りです。
https://github.com/sh-akira/VirtualMotionCapture/blob/master/LICENSE
