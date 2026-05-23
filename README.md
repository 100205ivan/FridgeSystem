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
wwwroot/
```

## 架構說明

### Controllers
負責控制系統流程與資料處理。

### Models
資料模型與資料庫欄位。

### Views
前端畫面。

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
