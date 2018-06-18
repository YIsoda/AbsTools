# AbsTools

化学実験Aで吸光度の測定データを前処理するツール（群）です。

コマンドライン上で動きます

- 現時点で対応しているプラットフォーム：
  - Windows Command Line
  - （パッケージ化の方法がわからないけど暫定で）macOS Command Line
 
## 使い方
生データのテキストファイルから，波長を1 nmごと，300-800 nmの範囲に対応させた吸光度のデータのみを含むファイルを生成します。
### Windows Command Line版

[リリースページ](https://github.com/YIsoda/AbsTools/releases)から[AbsSimplifier.exe](https://github.com/YIsoda/AbsTools/releases/download/v0.1/AbsSimplifier.exe)をダウンロードして，適当な場所においてください。

引数に生データファイルの相対or絶対パスを指定して実行ください（複数のファイルを指定可能です）。

```
PS> AbsSimplifier.exe absData1.txt absData2.txt ...
  absData1.txt  ->  absData1-simplified.txt
  absData2.txt  ->  absData2-simplified.txt
  ...
```

※実行しようとするとウイルスバスターにブロックされるようです。［開く］を押すと実行できると思います。


### macOS Commamd Line版
zipを落として展開し，中のAbsConvertCoreを実行すると動くかも
