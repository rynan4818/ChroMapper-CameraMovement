■ChroMapper-CameraMovement　インストール方法

ChroMapperのインストールフォルダにあるPluginsフォルダに ChroMapper-CameraMovement.dll をコピーします。

※Ver1.4.0～1.4.1 では以下のファイルをChroMapperのインストールフォルダにコピーしていましたが、
  Ver1.4.2 よりChroMapper-CameraMovement.dllに統合したため不要になりました。
  Ver1.4.0～1.4.1 をお使いの方は、不要なため削除して下さい。

        netstandard.dll
        Newtonsoft.Json.dll    ... 以前はPluginsフォルダに入れていましたが変更しました。
        UniGLTF.dll
        UniHumanoid.dll
        VRM.dll
        VRMShaders.GLTF.IO.Runtime.dll
        VRMShaders.GLTF.UniUnlit.Runtime.dll
        vrmavatar.shaders

■ChroMapper-CameraMovementのライセンスについて

MITライセンスで配布します。著作権表記・詳細は以下の通りです。
https://github.com/rynan4818/ChroMapper-CameraMovement/blob/main/LICENSE

■使用ライブラリの著作権、ライセンスについて

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
    UniVRM内でUniUnlitに変換されてい表示しています。

UniVRMの著作権表記・ライセンスは以下の通りです。
https://github.com/vrm-c/UniVRM/blob/master/LICENSE.txt

○Json.NET ライブラリの Newtonsoft.Json.dll をILMergeによって内包しています。

Json.NET配布先
https://www.newtonsoft.com/json
Json.NETの著作権表記・ライセンスは以下の通りです。
https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md

○アイコン素材について

ICONIONで作成しました。
http://iconion.com/ja/

○ソースコードの参考、コピー

CameraPlusフォルダの参考、CameraPlusの著作権表記・ライセンスは以下の通りです。
https://github.com/Snow1226/CameraPlus/blob/master/LICENSE

UI.cs の参考、Lolighterの著作権表記・ライセンスは以下の通りです。
https://github.com/KivalEvan/ChroMapper-Lolighter/blob/main/LICENSE
