# DungeonAndSurvivors

## Git コマンドメモ

## ローカルリポジトリのセッティングは以下コマンドを参照
### カレントディレクトリにローカルリポジトリを作成する (Git管理したいフォルダで以下コマンドを実行する)
$ git init

### ローカルリポジトリとリモートリポジトリを紐づけする
$ git remote add origin https://github.com/ry-takahashi86/DungeonAndSurvivors.git

### リモートリポジトリからファイルをダウンロードするとき
$ git pull origin main

## リモートリポジトリをクローンしてローカルリポジトリを作成したいとき
$ git clone https://github.com/ry-takahashi86/DungeonAndSurvivors.git

## ファイルをアップロードしたいとき(以下の順番で実行)
### カレントディレクトリ内のファイルを全てステージング対象にする (「.」をファイル名とかにすると個別に指定できる)
$ git add .
### ローカルリポジトリに保存する
$ git commit -m "comment"
### リモートリポジトリにアップロードする
$ git push origin main