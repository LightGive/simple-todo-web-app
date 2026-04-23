# API設計書

## 共通仕様

### 認証方式

ASP.NET Core Identity の Cookie 認証を使用する。
ログイン後に発行された認証 Cookie をリクエストに付与することで認証済みと判定する。

### APIの種類

本システムのエンドポイントは以下の2種類に分類される。

| 種別 | 対象エンドポイント | リクエスト形式 | レスポンス形式 |
| ---- | ------------------ | -------------- | -------------- |
| ページ（MVC） | `/account/*`、`/setup`、`/home` | フォーム（`application/x-www-form-urlencoded`） | リダイレクト または HTML |
| REST API | `/api/*` | JSON（`application/json`） | JSON |

### 共通エラーレスポンス

| HTTPステータス | 条件 |
| -------------- | ---- |
| 400 Bad Request | バリデーションエラー |
| 401 Unauthorized | 認証が必要なエンドポイントへの未認証アクセス |
| 403 Forbidden | 他ユーザーのリソースへのアクセス |
| 500 Internal Server Error | サーバー内部エラー・DB接続失敗 |

### CSRF対策

フォーム送信・状態変更を伴う POST エンドポイントには ASP.NET Core の AntiForgery トークンを使用する。

---

## 認証・ユーザー管理

### POST /account/login

**概要**

メールアドレス・パスワードで認証し、Cookie セッションを発行する。

**認証**：不要

**リクエスト（フォーム）**

| パラメーター | 型 | 必須 | 説明 |
| ------------ | -- | ---- | ---- |
| Email | string | ○ | メールアドレス |
| Password | string | ○ | パスワード |

**レスポンス**

| 条件 | 結果 |
| ---- | ---- |
| 認証成功・IsInit = true | SCR008（/home）へリダイレクト |
| 認証成功・IsInit = false | SCR007（/setup）へリダイレクト |
| 認証失敗・バリデーションエラー | SCR001 を再表示（エラーメッセージ付き） |

**使用テーブル**

- `AspNetUsers`（メールアドレス・パスワードハッシュの照合、IsInit の参照）

---

### POST /account/logout

**概要**

Cookie 認証セッションを無効化し、ログアウトする。

**認証**：要

**リクエスト**：なし

**レスポンス**

| 条件 | 結果 |
| ---- | ---- |
| 常時 | SCR001（/account/login）へリダイレクト |

---

### POST /account/register

**概要**

メールアドレス・パスワードで新規ユーザーを登録し、確認メールを送信する。
登録成功時、確認メールを送信する。

**認証**：不要

**リクエスト（フォーム）**

| パラメーター | 型 | 必須 | 説明 |
| ------------ | -- | ---- | ---- |
| Email | string | ○ | メールアドレス（256文字以内） |
| Password | string | ○ | パスワード（8文字以上、英小文字・数字を各1文字以上含む、同一文字のみ不可） |
| ConfirmPassword | string | ○ | パスワード（確認） |

**レスポンス**

| 条件 | 結果 |
| ---- | ---- |
| 登録成功 | SCR003（/account/register-confirmation）へリダイレクト |
| バリデーションエラー・メールアドレス重複 | SCR002 を再表示（エラーメッセージ付き） |

**使用テーブル**

- `AspNetUsers`（ユーザー作成）

---

### GET /account/confirm-email

**概要**

メール内の確認リンクからアクセスし、メールアドレスの認証を完了する。

**認証**：不要（URLパラメーターによる本人確認）

**リクエスト（クエリパラメーター）**

| パラメーター | 型 | 必須 | 説明 |
| ------------ | -- | ---- | ---- |
| userId | string | ○ | ユーザーID |
| token | string | ○ | メール認証トークン |

**レスポンス**

| 条件 | 結果 |
| ---- | ---- |
| 認証成功 | SCR001（/account/login）へリダイレクト（成功メッセージ付き） |
| パラメーター不正・ユーザー不存在・トークン無効 | SCR001（/account/login）へリダイレクト（エラーメッセージ付き） |

**使用テーブル**

- `AspNetUsers`（メールアドレス認証状態の更新）

---

### POST /account/forgot-password

**概要**

入力されたメールアドレスに対してパスワード再設定リンクをメール送信する。
未登録のメールアドレスの場合も同じ完了画面へ遷移する（ユーザー列挙攻撃対策）。

**認証**：不要

**リクエスト（フォーム）**

| パラメーター | 型 | 必須 | 説明 |
| ------------ | -- | ---- | ---- |
| Email | string | ○ | メールアドレス |

**レスポンス**

| 条件 | 結果 |
| ---- | ---- |
| 常時（登録有無問わず） | SCR005（/account/forgot-password-confirmation）へリダイレクト |
| バリデーションエラー | SCR004 を再表示（エラーメッセージ付き） |

**使用テーブル**

- `AspNetUsers`（メールアドレスの存在確認）

---

### POST /account/reset-password

**概要**

URLクエリパラメーターのトークンとメールアドレスを使用して、パスワードを新しい値に更新する。
再設定成功後、自動ログインはしない。

**認証**：不要（トークンによる本人確認）

**リクエスト（フォーム）**

| パラメーター | 型 | 必須 | 説明 |
| ------------ | -- | ---- | ---- |
| Email | string | ○ | メールアドレス（hiddenフィールド） |
| Token | string | ○ | 再設定トークン（hiddenフィールド） |
| NewPassword | string | ○ | 新しいパスワード（8文字以上、英小文字・数字を各1文字以上含む、同一文字のみ不可） |
| ConfirmPassword | string | ○ | 新しいパスワード（確認） |

**レスポンス**

| 条件 | 結果 |
| ---- | ---- |
| 再設定成功 | SCR001（/account/login）へリダイレクト |
| バリデーションエラー | SCR006 を再表示（エラーメッセージ付き） |
| トークン無効・期限切れ | SCR006 を再表示（エラーメッセージ付き） |

**使用テーブル**

- `AspNetUsers`（パスワードハッシュの更新）

---

## 初期設定

### POST /home/initial-setup

**概要**

キャラクター名と3カテゴリのタスク名をトランザクション内で一括保存する。
完了後に `AspNetUsers.IsInit` を `true` に更新する。

**認証**：要

**リクエスト（フォーム）**

| パラメーター | 型 | 必須 | 桁数上限 | 説明 |
| ------------ | -- | ---- | -------- | ---- |
| DisplayName | string | ○ | 32文字 | キャラクター名 |
| ExerciseTaskName | string | ○ | 100文字 | 運動カテゴリのタスク名 |
| StudyTaskName | string | ○ | 100文字 | 勉強カテゴリのタスク名 |
| HouseworkTaskName | string | ○ | 100文字 | 家事カテゴリのタスク名 |

**レスポンス**

| 条件 | 結果 |
| ---- | ---- |
| 登録成功 | SCR008（/home）へリダイレクト |
| バリデーションエラー | SCR007 を再表示（エラーメッセージ付き） |

**使用テーブル**

- `AspNetUsers`（DisplayName・IsInit の更新）
- `Tasks`（タスク名で新規作成）
- `CharacterStats`（初期ステータスの作成）
- `UnallocatedPoints`（初期ポイントの作成）

---

## ホーム

### GET /home/home

**概要**

ホーム画面の表示に必要なデータをまとめて取得する。
レベルは `TaskCompletionLogs` の `COUNT(*)` で算出する。
タスクの完了判定はクエリ時に `LastCompletedDate = 本日` で行う（DBの更新は不要）。

**認証**：要

**リクエスト**：なし

**レスポンス（ページレンダリング用モデル）**

| フィールド | 型 | 説明 |
| ---------- | -- | ---- |
| DisplayName | string | キャラクター名 |
| Level | int | レベル（TaskCompletionLogs の COUNT(*)） |
| Tasks | Task[] | タスク一覧 |
| Tasks[].TaskId | int | タスクID |
| Tasks[].TaskName | string | タスク名 |
| Tasks[].Category | int | カテゴリ（0=運動 / 1=勉強 / 2=家事） |
| Tasks[].IsCompleted | bool | 完了状態（LastCompletedDate = 本日 で判定） |
| CharacterStats | object | キャラクターステータス |
| CharacterStats.HP | int | HP |
| CharacterStats.MP | int | MP |
| CharacterStats.ATK | int | 攻撃力 |
| CharacterStats.DEF | int | 防御力 |
| CharacterStats.SPD | int | 速度 |
| CharacterStats.MATK | int | 魔法攻撃力 |
| UnallocatedPoints | object | 未振り分けポイント |
| UnallocatedPoints.ExercisePoints | int | 運動ポイント |
| UnallocatedPoints.StudyPoints | int | 勉強ポイント |
| UnallocatedPoints.HouseworkPoints | int | 家事ポイント |

**使用テーブル**

- `AspNetUsers`（DisplayName の取得）
- `Tasks`（タスク一覧・完了状態の取得）
- `CharacterStats`（ステータスの取得）
- `UnallocatedPoints`（ポイントの取得）
- `TaskCompletionLogs`（レベル算出）

---

## タスク管理

### POST /api/tasks/{taskId}/complete

**概要**

指定タスクを完了状態にし、対応カテゴリのポイントを +1 する。
同一タスクの同日重複完了は不可（`TaskCompletionLogs` の UNIQUE 制約で防止）。

**認証**：要

**パスパラメーター**

| パラメーター | 型 | 説明 |
| ------------ | -- | ---- |
| taskId | int | 完了するタスクのID |

**リクエストボディ**：なし

**レスポンス**

| 条件 | 結果 |
| ---- | ---- |
| 完了成功 | SCR008（/home/home）へリダイレクト |
| 本日既に完了済み・タスクが存在しない・ポイントデータ不整合 | SCR008（/home/home）へリダイレクト |
| 未認証 | SCR001（/account/login）へリダイレクト |

**使用テーブル**

- `Tasks`（LastCompletedDate の更新）
- `UnallocatedPoints`（ポイントの加算）
- `TaskCompletionLogs`（完了ログの INSERT）

---

### GET /api/tasks/history

> ⚠️ **未実装** ：このエンドポイントは現時点では実装されていない。

**概要**

ログインユーザーの過去のタスク完了履歴を取得する。草表示の描画に使用する。

**認証**：要

**リクエスト**：なし

**レスポンスボディ（JSON）**

```json
{
  "history": [
    {
      "completedDate": "2026-04-01",
      "categories": [0, 1]
    },
    {
      "completedDate": "2026-03-31",
      "categories": [0, 1, 2]
    }
  ]
}
```

| フィールド | 型 | 説明 |
| ---------- | -- | ---- |
| history | array | 完了履歴の配列 |
| history[].completedDate | string（date） | 完了日（YYYY-MM-DD） |
| history[].categories | int[] | 完了したカテゴリの配列（0=運動 / 1=勉強 / 2=家事） |

**使用テーブル**

- `TaskCompletionLogs`（完了履歴の集計）

---

## キャラクター管理

### POST /api/character/allocate

**概要**

未振り分けポイントを指定ステータスに振り分ける。
振り分けルールに従いカテゴリ外のステータスへの振り分ては不可。1ポイント消費で対象ステータス +1。

**振り分けルール**

| カテゴリ | 振り分け可能なステータス |
| -------- | ------------------------ |
| 運動     | HP、ATK                  |
| 勉強     | MP、MATK                 |
| 家事     | DEF、SPD                 |

**認証**：要

**リクエストボディ（JSON）**

```json
{
  "hp": 2,
  "mp": 0,
  "atk": 1,
  "def": 0,
  "spd": 0,
  "matk": 0
}
```

| フィールド | 型 | 必須 | 説明 |
| ---------- | -- | ---- | ---- |
| hp | int | ○ | HP に振り分けるポイント数（0以上）。運動ポイントから消費される |
| mp | int | ○ | MP に振り分けるポイント数（0以上）。勉強ポイントから消費される |
| atk | int | ○ | ATK に振り分けるポイント数（0以上）。運動ポイントから消費される |
| def | int | ○ | DEF に振り分けるポイント数（0以上）。家事ポイントから消費される |
| spd | int | ○ | SPD に振り分けるポイント数（0以上）。家事ポイントから消費される |
| matk | int | ○ | MATK に振り分けるポイント数（0以上）。勉強ポイントから消費される |

**レスポンスボディ**：なし（200 OK）

**エラーレスポンス**

| HTTPステータス | 条件 |
| -------------- | ---- |
| 400 | ポイントが不足している |
| 400 | リクエストのバリデーションエラー（負の値など） |
| 500 | DB保存エラー |

**使用テーブル**

- `UnallocatedPoints`（ポイントの減算）
- `CharacterStats`（ステータスの加算）

---

## エンドポイント一覧

| メソッド | エンドポイント | 処理名 | 認証 | 種別 |
| -------- | -------------- | ------ | ---- | ---- |
| POST | /account/login | ログイン認証 | 不要 | ページ |
| POST | /account/logout | ログアウト | 要 | ページ |
| POST | /account/register | ユーザー登録 | 不要 | ページ |
| GET | /account/confirm-email | メールアドレス認証 | 不要 | ページ |
| POST | /account/forgot-password | パスワード再設定メール送信 | 不要 | ページ |
| POST | /account/reset-password | パスワード再設定 | 不要 | ページ |
| GET | /home/initial-setup | 初期設定画面表示 | 要 | ページ |
| POST | /home/initial-setup | 初期設定登録 | 要 | ページ |
| GET | /home/home | ホーム情報取得 | 要 | ページ |
| POST | /api/tasks/{taskId}/complete | タスク完了 | 要 | ページ |
| GET | /api/tasks/history | タスク完了履歴取得（未実装） | 要 | REST API |
| POST | /api/character/allocate | ポイント振り分け | 要 | REST API |
