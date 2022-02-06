# ChroMapper-CameraMovement
[CameraPlus](https://github.com/Snow1226/CameraPlus)�p��[MovementScript](https://github.com/Snow1226/CameraPlus/wiki/MovementScript)��ǂݍ���ŁA�안�c�[����[ChroMapper](https://github.com/Caeden117/ChroMapper)�ŃJ�������[�N�̍Č�������ChroMapper�p�v���O�C���ł��B
���[Script Mapper](https://github.com/hibit-at/Scriptmapper)���g���āA�J�����X�N���v�g���쐬�̂�O��Ƃ������ɂȂ��Ă��܂��B

# �C���X�g�[�����@
1. [�����[�X�y�[�W](https://github.com/rynan4818/ChroMapper-CameraMovement/releases)����A�ŐV�ł̃v���O�C����zip�t�@�C�����_�E�����[�h���ĉ������B

2. ChroMapper�̃C���X�g�[���t�H���_�ɂ���`Plugins`�t�H���_�ɁA�_�E�����[�h����zip�t�@�C�����𓀂���`ChroMapper-CameraMovement.dll`��`Newtonsoft.Json.dll`���R�s�[���ĉ������B

3. [Script Mapper](https://github.com/hibit-at/Scriptmapper)���_�E�����[�h���āAChroMapper�̃C���X�g�[���t�H���_(ChroMapper.exe������t�H���_)��`scriptmapper.exe`���R�s�[���܂��B

# �g�p�@
���ʂ�ǂݍ���ŃG�f�B�^��ʂ��o���ĉ������BTab�L�[�������ƉE���ɃA�C�R���p�l�����o�܂��̂ŁA���F�̃J�����A�C�R���������� CameraMovement�̐ݒ�p�l�����J���܂��B

* Movement Enable �F �J�����X�N���v�g�ɍ��킹�ăJ�������ړ����܂��B
* UI Hidden �F �안�p�̃O���b�hUI�Ȃǂ������܂��B(�܂����Ȃǈꕔ�������܂���)
* Turn To Head �F �J�����X�N���v�g��TurnToHeadUseCameraSetting�̐ݒ肪true�̎��ɁA�J�������A�o�^�[�̕����������Ō����܂��B(CameraPlus��TurnToHead�p�����[�^�ɑ������܂��B)
* Avatar �F �A�o�^�[��3D�I�u�W�F�N�g��\�����܂��B
* Head Hight �F �A�o�^�[�̓��̍����i���̒��S�j[�P�� m]
* Head Size �F �A�o�^�[�̓��̑傫��(���̒��a) [�P�� m]
* Arm Size �F �A�o�^�[�̗���̒��� [�P�� m]
* Script File �F ���ʃt�H���_�ɂ���ǂݍ��ރJ�����X�N���v�g�t�@�C����
* Cam Pos Rot �F ���݂̃J�����ʒu (�ǂݎ���p)
* Reload �F �ݒ�Ȃǂ��蓮�œǂݍ��ݒ���
* Setting Save : ��L�ݒ��ۑ�����
* Script Mapper Run �F ���ʃf�[�^��ۑ����āAScript Mapper�Ńu�b�N�}�[�N���J�����X�N���v�g�ɕϊ����܂��B

## �⑫
ChroMapper�̃u�b�N�}�[�N��MMA2�Ɠ���"B"�L�[�ł����A���̃^�C�����C������N���b�N����ƍŕҏW�ł��܂��B�܂��u�b�N�}�[�N�̍폜�́A�^�C�����C����̃u�b�N�}�[�N���}�E�X���N���b�N�ł��B

# ToDo
* UI Hidden�ŕs�v��UI��S������(�Q�[���̃v���C��ʂɋ߂�����)
* �u�b�N�}�[�N�̕\����MMA2�̗l�ɏ��ɕ\�����������A�N���b�N���čĕҏW
* Script Mapper�̃R�}���h�����j���[�őI�����ău�b�N�}�[�N����
* ���݂̃J�����ʒu��input.csv�ɏo�͂���{�^��
* VRM�Ƃ��ǂݍ���ŃA�o�^�[�\�����������Ǔ�����B

# �J���ҏ��
���̃v���W�F�N�g���r���h����ɂ́AChroMapper�̃C���X�g�[���p�X���w�肷�� ChroMapper-CameraMovement\ChroMapper-CameraMovement.csproj.user �t�@�C�����쐬����K�v������܂��B

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="Current" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <ChroMapperDir>C:\TOOL\ChroMapper\chromapper</ChroMapperDir>
  </PropertyGroup>
</Project>
```

## �v���O�C������̎Q�l
`CameraMovement.cs`�̑唼�́A���́[���񐻍��CameraPlus��[CameraMovement.cs](https://github.com/Snow1226/CameraPlus/blob/master/CameraPlus/Behaviours/CameraMovement.cs)���R�s�[���č쐬���Ă��܂��B�J�����ړ������͑S�������ł��B

`UI.cs`�̑唼��Kival Evan���񐻍��[Lolighter](https://github.com/KivalEvan/ChroMapper-Lolighter)��[UI.cs](https://github.com/KivalEvan/ChroMapper-Lolighter/blob/main/UI/UI.cs)���R�s�[���č쐬���Ă��܂��B