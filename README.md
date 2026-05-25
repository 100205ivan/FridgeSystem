# FridgeSystem 智慧冰箱管理系統

## 專案介紹

這是一個使用 ASP.NET MVC 開發的智慧冰箱管理系統。

使用者可以管理冰箱內的食材資訊，包含：

- 新增食材
- 修改食材
- 刪除食材
- 查詢食材
- 顯示所有食材
- 食材狀態管理

系統主要目的是協助使用者管理冰箱內的食材與保存狀態。

---

# 目前完成進度

## 已完成功能

### 基本 CRUD 功能

- 新增食材
- 修改食材
- 刪除食材
- 查詢食材

### 食材欄位

目前包含：

- 食材名稱
- 分類
- 數量
- 保存位置
- 放入日期
- 有效期限
- 備註

### 狀態判斷

- 顯示即將到期食材
- 顯示已過期食材

### 前端畫面

- MVC Razor 頁面
- Bootstrap 基本排版
- 表格顯示食材資訊

---

# 專案架構

```bash
Controllers/
Models/
Views/
Services/
wwwroot/
```

## 架構說明

### Controllers

負責控制系統流程與資料處理。

### Models

資料模型、ViewModel 與資料欄位定義。

### Views

前端畫面，使用 MVC Razor 頁面。

### Services

目前放置使用紀錄相關邏輯。

### wwwroot

CSS、JavaScript、圖片等靜態資源。

---

# 開發環境

## 使用技術

- ASP.NET MVC
- C#
- .NET
- Bootstrap
- SQLite

---

# 前後端協作說明

目前專案是 ASP.NET MVC 架構，不是前後端完全分離的 API 專案。

目前資料尚未正式接資料庫，主要先用記憶體中的假資料讓前端畫面與流程可以正常展示。這些假資料已在程式碼旁加上 `TODO` 註解，後端串接時可以改成資料庫讀寫。

## 分工建議

### 前端負責

- Views 畫面
- 表單欄位
- 使用者操作流程
- Bootstrap 樣式
- 資料顯示方式

### 後端負責

- 資料庫設計
- Entity / Model 調整
- Controller 資料讀寫
- 登入驗證
- 使用紀錄儲存
- 食譜與推薦資料

---

# 目前假資料位置

## 食材假資料

位置：

```text
Controllers/FoodController.cs
```

目前資料來源：

```csharp
FoodController.Foods
```

用途：

- 食材列表
- 新增食材
- 編輯食材
- 刪除食材
- 食材詳情
- 食材月曆
- 首頁統計
- 食譜推薦

後端串接時，建議改成資料庫的 `Foods` table。

## 食譜假資料

位置：

```text
Controllers/RecipeController.cs
```

目前資料來源：

```csharp
RecipeController.Recipes
```

用途：

- 食譜推薦
- 根據冰箱現有食材比對食譜材料

後端串接時，建議改成資料庫的 `Recipes` 與 `RecipeIngredients` table。

## 登入假帳號

位置：

```text
Controllers/AccountController.cs
```

目前資料來源：

```csharp
AccountController.Users
```

目前測試帳號：

```text
admin / 1234
user / 1234
```

這只是測試登入用。正式後端應改成 `Users` table，並使用密碼雜湊，不要儲存明文密碼。

## 使用紀錄假資料

位置：

```text
Services/UsageLog.cs
```

目前資料來源：

```csharp
UsageLog.Records
```

用途：

- 登入紀錄
- 登出紀錄
- 新增食材紀錄
- 編輯食材紀錄
- 刪除食材紀錄
- 首頁最近紀錄
- 使用紀錄頁面

後端串接時，建議改成資料庫的 `UsageRecords` table。

## 食材分類規則假資料

位置：

```text
Models/FoodRule.cs
```

目前資料來源：

```csharp
FoodRule.Rules
```

用途：

- 食材分類下拉選單
- 預設保存位置
- 預設到期天數

後端串接時，建議改成資料庫的 `FoodRules` table。

---

# 核心資料結構

## FoodItem 食材

這是目前最重要的資料，食材 CRUD、首頁統計、月曆與食譜推薦都會用到。

```csharp
public class FoodItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public string Quantity { get; set; }
    public DateTime PutDate { get; set; }
    public string? StoragePlace { get; set; }
    public DateTime? ExpireDate { get; set; }
    public string? Note { get; set; }
}
```

前端表單會送出的欄位：

```text
Name
Category
Quantity
PutDate
StoragePlace
ExpireDate
Note
```

`Status` 是由程式根據日期計算出來的，不一定要存進資料庫。

狀態邏輯：

```text
ExpireDate 沒有值 -> 未設定 / 正常
ExpireDate < 今天 -> 已過期
ExpireDate == 今天 -> 今天到期
ExpireDate 距離今天 <= 3 天 -> 即將到期
PutDate 距離今天 >= 30 天，且不是冷凍 -> 放太久
其他 -> 正常
```

建議資料表：

```sql
Foods
- Id int primary key
- Name nvarchar(100) not null
- Category nvarchar(50) not null
- Quantity nvarchar(50) not null
- PutDate date not null
- StoragePlace nvarchar(50) null
- ExpireDate date null
- Note nvarchar(500) null
- CreatedAt datetime
- UpdatedAt datetime
```

## FoodRule 食材分類規則

```csharp
public class FoodRule
{
    public string Category { get; set; }
    public string DefaultStoragePlace { get; set; }
    public int DefaultExpireDays { get; set; }
}
```

用途：

```text
新增食材時，如果 StoragePlace 沒有填，就自動帶入 DefaultStoragePlace。
新增食材時，如果 ExpireDate 沒有填，就自動用 PutDate + DefaultExpireDays。
```

建議資料表：

```sql
FoodRules
- Id int primary key
- Category nvarchar(50) not null
- DefaultStoragePlace nvarchar(50) not null
- DefaultExpireDays int not null
```

## Recipe 食譜

```csharp
public class Recipe
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<RecipeIngredient> Ingredients { get; set; }
}
```

## RecipeIngredient 食譜材料

```csharp
public class RecipeIngredient
{
    public string Name { get; set; }
}
```

建議資料表：

```sql
Recipes
- Id int primary key
- Name nvarchar(100) not null
```

```sql
RecipeIngredients
- Id int primary key
- RecipeId int not null
- Name nvarchar(100) not null
```

## UsageRecord 使用紀錄

```csharp
public class UsageRecord
{
    public DateTime Time { get; set; }
    public string UserName { get; set; }
    public string Action { get; set; }
    public string Target { get; set; }
    public string Description { get; set; }
}
```

建議資料表：

```sql
UsageRecords
- Id int primary key
- Time datetime not null
- UserName nvarchar(50) not null
- Action nvarchar(50) not null
- Target nvarchar(100) not null
- Description nvarchar(500) not null
```

## User 使用者

目前還沒有正式 User Model，只有測試帳號。

建議資料表：

```sql
Users
- Id int primary key
- UserName nvarchar(50) not null unique
- PasswordHash nvarchar(255) not null
- Role nvarchar(50) null
- CreatedAt datetime
```

---

# 後端串接建議

因為目前專案是 ASP.NET MVC，所以建議先保留目前的 Views 與 Controller action 名稱，由後端把 Controller 裡面的記憶體資料來源改成資料庫讀寫。

例如目前：

```csharp
Foods.Add(foodItem);
```

之後可以改成：

```csharp
_db.Foods.Add(foodItem);
_db.SaveChanges();
```

目前：

```csharp
Foods.FirstOrDefault(food => food.Id == id);
```

之後可以改成：

```csharp
_db.Foods.FirstOrDefault(food => food.Id == id);
```

這樣前端 View 幾乎不用大改。

## 後端優先處理順序

建議後端可以照這個順序處理：

1. 建立資料庫連線
2. 建立 `Foods` table
3. 把 `FoodController.Foods` 改成資料庫 CRUD
4. 建立 `Users` table，處理登入
5. 建立 `UsageRecords` table，儲存操作紀錄
6. 建立 `FoodRules` table，管理分類規則
7. 建立 `Recipes` / `RecipeIngredients` table
8. 最後調整首頁統計、月曆、食譜推薦

最重要的是先完成：

```text
Foods CRUD
Users login
UsageRecords
```

## 協作注意事項

目前不建議直接刪除假資料，因為刪掉後畫面會變空，展示與測試會比較不方便。

正確做法是：

```text
保留 mock data
加 TODO 註解
等後端接資料庫時替換掉資料來源
```

前端會盡量不要任意修改以下欄位名稱，避免影響後端 Model binding：

```text
Name
Category
Quantity
PutDate
StoragePlace
ExpireDate
Note
```

如果後端需要修改欄位名稱，請先一起確認，避免表單資料送不到 Controller。

---

# 如何執行專案

## 1. Clone 專案

```bash
git clone https://github.com/100205ivan/FridgeSystem.git
```

---

## 2. 進入專案資料夾

```bash
cd FridgeSystem
```

---

## 3. 執行專案

```bash
dotnet run
```

---

## 4. 開啟瀏覽器

通常會出現：

```bash
http://localhost:xxxx
```

將網址貼到瀏覽器即可。

---

# Git 協作方式

## 更新最新版本

```bash
git pull
```

---

## 上傳修改

```bash
git add .
git commit -m "修改內容"
git push
```

---
