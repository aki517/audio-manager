# audio-manager
ゲームオーディオ管理クラス

### Unity Version
Unity 2019.1.0f2

### アセット作成手順  
1. Projectビューで右クリック > Create > Goisagi > Audio > Create AudioResource を作成  
![Screen Shot 2019-10-20 at 14 55 27](https://user-images.githubusercontent.com/6293424/67215367-48611000-f45c-11e9-9802-69dee81d7083.png)  

2. 作成した AudioResource に AudioClip を設定  
![Screen Shot 2019-10-20 at 15 00 19](https://user-images.githubusercontent.com/6293424/67215369-48611000-f45c-11e9-9c6a-2b54c1fe5026.png)  

### 使い方
1. Hierarchyビューの GameObject に AudioManager を追加
![Screen Shot 2019-10-20 at 14 56 18](https://user-images.githubusercontent.com/6293424/67215368-48611000-f45c-11e9-84cd-14d36f561d5b.png)  

2. 各カテゴリごとに以下の項目を設定する  

|名前|説明|  
|:---|:---|  
|Master Audio Mixer|AudioManagerで使用するAudioMixer|  
|Master Volume Name|AudioMixerで設定したマスターボリューム名|  
|Resource Max|AudioResourceの登録可能数|  
|Audio Pool Size|AudioSourceが接続されたGameObjectの上限数を設定|  
|Simultaneous Play Max|同時発音の上限数|  
|Volume Param Name|AudioMixerで設定したボリューム名|  

3. スクリプト制御
##### 一連の流れ
``` cs
// リソース読込.
AudioResource data = Resources.Load<AudioResource>( "bgm" );
// リソースを接続してコントローラを取得.
AudioController bgmCtrl = AudioManager.I.AttachBgmData( data );

// コントローラ経由で再生.
bgmCtrl.Play( 0 ); // AudioResourceに登録されているClipsの０番目を再生.

// 破棄.
AudioManager.I.DetachBgmData( ref bgmCtrl );
```
##### ループ音の制御
``` cs
// 再生時にハンドルを取得.
int handle = bgmCtrl.Play( 0 );
// ハンドルを引数にポーズ/停止を行う.
if( bgmCtrl.IsPlaying( handle )){
   bgmCtrl.Stop( handle );
   // bgmCtrl.Pause( handle );
}
```

### ライセンスについて
プロジェクト内のオーディオリソースは「ユニティちゃんライセンス条項」に準拠します。  
http://unity-chan.com/contents/license_jp/

![Light_Silhouette](https://user-images.githubusercontent.com/6293424/67156076-10bb7080-f355-11e9-8006-b0fa2eb7d1fe.jpg)
