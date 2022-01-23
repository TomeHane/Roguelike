# Roguelike
Unityプロジェクトで使用したスクリプトをまとめて管理するリポジトリ

# 主なスクリプトの説明

## AspectController.cs
カメラの縦横比を固定するスクリプト。ウインドウを常に監視しており、縦横比が変化したときに動く。

## DontDestroyManager.cs
プレイヤーやUIなど、シーンをまたいで使用したいオブジェクトをDontDestroyOnLoad状態にするスクリプト。
DestroyAll()でまとめて削除可能。

## GameController.cs
ゲーム全体を管理するスクリプト。現在の階数を記憶させ、シーン遷移時に階数に応じた処理を行う。
後述するMusicPlayer.csの関数を用いて、BGM・ME・一部SEを鳴らす役割も担っている。

## GameClearController.cs
ゲームクリア時の処理を管理するスクリプト。UIの表示などを行っている。

## GameOverController.cs
ゲームオーバー時の処理を管理するスクリプト。UIの表示などを行っている。

## PlayerController.cs
プレイヤーオブジェクトの動きを制御する。

## SearchArea.cs, SearchArea_wizard.cs
プレイヤーがエネミーの索敵範囲(トリガー)へ出入りしたときに動くスクリプト。
これによりエネミーの状態を"通常時"から"追跡時"に切り替えることができる。

## Sensor.cs,  Sensor_wizard.cs
エネミーが壁又は別のエネミーにぶつかったときに、進行方向(目的地)を変更する。

## SystemManager.cs
マップの生成とオブジェクトの生成・設置を行うスクリプト。グリッドの概念を用いている。

まず、ランダムな地点に、ランダムな大きさの部屋を、ランダムな数作る。
そして、道の集合点を決め、各部屋から道の集合点に向かって道を掘ることでマップが生成される。

周囲8マス壁ではないマスを"スポーン可能エリア"とし、そこにオブジェクトを配置している。

## VCameraController.cs
CinemachineVirtualCameraの動きを制御する。
