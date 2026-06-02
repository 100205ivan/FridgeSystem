-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- 主機： 127.0.0.1
-- 產生時間： 2026-06-02 19:30:04
-- 伺服器版本： 10.4.32-MariaDB
-- PHP 版本： 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- 資料庫： `fridgesystem`
--

-- --------------------------------------------------------

--
-- 資料表結構 `foods`
--

CREATE TABLE `foods` (
  `Id` int(11) NOT NULL,
  `UserId` int(11) NOT NULL,
  `Name` longtext NOT NULL,
  `Category` longtext NOT NULL,
  `Quantity` longtext NOT NULL,
  `PutDate` datetime(6) NOT NULL,
  `StoragePlace` longtext DEFAULT NULL,
  `ExpireDate` datetime(6) DEFAULT NULL,
  `Note` longtext DEFAULT NULL,
  `EnglishName` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- 傾印資料表的資料 `foods`
--

INSERT INTO `foods` (`Id`, `UserId`, `Name`, `Category`, `Quantity`, `PutDate`, `StoragePlace`, `ExpireDate`, `Note`, `EnglishName`) VALUES
(1, 1, '牛肉', '肉類', '1 盒', '2026-06-02 00:00:00.000000', '冷藏', '2026-06-07 00:00:00.000000', '福利雄 雄福利', NULL),
(2, 2, '牛奶', '乳製品', '1 瓶', '2026-06-02 00:00:00.000000', '冷藏', '2026-06-04 00:00:00.000000', '我都喝光泉', NULL),
(3, 2, '雞蛋', '蛋類', '10 顆', '2026-06-02 00:00:00.000000', '冷藏', '2026-06-14 00:00:00.000000', '不知道蛋可以放多久', 'egg'),
(4, 2, '高麗菜', '蔬菜', '1 顆', '2026-06-02 00:00:00.000000', '冷藏', '2026-06-07 00:00:00.000000', '爛爛的好吃', 'cabbage'),
(5, 2, '豬肉', '肉類', '1 盒', '2026-06-03 00:00:00.000000', '冷凍', '2026-06-06 00:00:00.000000', '佩佩珠', NULL),
(6, 2, '雞胸肉', '肉類', '1 盒', '2026-06-03 00:00:00.000000', '冷凍', '2026-06-07 00:00:00.000000', '好吃', NULL);

-- --------------------------------------------------------

--
-- 資料表結構 `users`
--

CREATE TABLE `users` (
  `Id` int(11) NOT NULL,
  `Username` longtext NOT NULL,
  `PasswordHash` longtext NOT NULL,
  `CreatedAt` datetime(6) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- 傾印資料表的資料 `users`
--

INSERT INTO `users` (`Id`, `Username`, `PasswordHash`, `CreatedAt`) VALUES
(1, 'user ', '1234', '2026-06-02 01:38:22.840969'),
(2, 'admin', '1234', '2026-06-02 01:48:33.820497');

-- --------------------------------------------------------

--
-- 資料表結構 `__efmigrationshistory`
--

CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- 傾印資料表的資料 `__efmigrationshistory`
--

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`) VALUES
('20260601171019_InitialCreate', '9.0.8'),
('20260601174631_AddFoodUserRelation', '9.0.8'),
('20260602071249_AddEnglishName', '9.0.8');

--
-- 已傾印資料表的索引
--

--
-- 資料表索引 `foods`
--
ALTER TABLE `foods`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_Foods_UserId` (`UserId`);

--
-- 資料表索引 `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`Id`);

--
-- 資料表索引 `__efmigrationshistory`
--
ALTER TABLE `__efmigrationshistory`
  ADD PRIMARY KEY (`MigrationId`);

--
-- 在傾印的資料表使用自動遞增(AUTO_INCREMENT)
--

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `foods`
--
ALTER TABLE `foods`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- 使用資料表自動遞增(AUTO_INCREMENT) `users`
--
ALTER TABLE `users`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- 已傾印資料表的限制式
--

--
-- 資料表的限制式 `foods`
--
ALTER TABLE `foods`
  ADD CONSTRAINT `FK_Foods_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
