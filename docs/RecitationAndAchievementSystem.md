# 背诵系统和成就系统实体说明

本文档说明了新增的背诵系统（基于艾宾浩斯遗忘曲线）和成就系统的实体设计。

## 一、背诵系统（艾宾浩斯遗忘曲线）

### 1.1 实体结构

#### UserRecitation（用户背诵记录）
记录用户开始背诵某首诗文的基本信息。

**主要字段：**
- `UserId`：用户ID
- `PoemId`：诗文ID
- `Status`：背诵状态（学习中、已掌握、需要复习、已放弃）
- `FirstRecitationTime`：首次背诵时间
- `LastReviewTime`：最后一次复习时间
- `ReviewCount`：复习次数
- `Proficiency`：熟练度（0-100）
- `NextReviewTime`：下次复习时间（根据艾宾浩斯曲线计算）

#### RecitationReview（复习记录）
基于艾宾浩斯遗忘曲线的复习计划和完成情况。

**主要字段：**
- `UserRecitationId`：关联的背诵记录ID
- `ScheduledTime`：计划复习时间
- `ActualReviewTime`：实际复习时间
- `Status`：复习状态（待复习、已完成、已跳过、已过期）
- `ReviewRound`：复习轮次（第几次复习）
- `QualityRating`：复习质量评分（1-5）
  - 1 = 完全忘记
  - 2 = 模糊
  - 3 = 能想起
  - 4 = 熟练
  - 5 = 完美
- `DurationSeconds`：复习耗时（秒）
- `ReminderSent`：是否发送了提醒

### 1.2 艾宾浩斯遗忘曲线复习间隔

根据艾宾浩斯遗忘曲线理论，复习间隔为：
- 第1次复习：1天后
- 第2次复习：2天后
- 第3次复习：4天后
- 第4次复习：7天后
- 第5次复习：15天后
- 第6次复习：30天后
- 第7次及以后：60天后

### 1.3 EbbinghausHelper 辅助类

提供了以下核心方法：

```csharp
// 获取复习间隔天数
int interval = EbbinghausHelper.GetReviewInterval(reviewRound);

// 计算下次复习时间（标准间隔）
DateTime nextTime = EbbinghausHelper.CalculateNextReviewTime(lastReviewTime, nextReviewRound);

// 根据复习质量调整下次复习时间
DateTime adjustedTime = EbbinghausHelper.CalculateNextReviewTimeWithQuality(
    lastReviewTime, 
    nextReviewRound, 
    qualityRating);

// 计算熟练度
int proficiency = EbbinghausHelper.CalculateProficiency(reviewCount, recentQualityRatings);

// 判断是否应该发送提醒
bool shouldRemind = EbbinghausHelper.ShouldSendReminder(scheduledTime, DateTime.Now);

// 判断复习是否过期
bool expired = EbbinghausHelper.IsReviewExpired(scheduledTime, DateTime.Now);
```

### 1.4 使用流程

1. **用户开始背诵**：创建 `UserRecitation` 记录
2. **首次背诵完成**：创建第一个 `RecitationReview` 记录，计划第1次复习（1天后）
3. **用户完成复习**：
   - 更新当前 `RecitationReview` 的状态、实际时间、质量评分
   - 根据质量评分计算下次复习时间
   - 创建下一个 `RecitationReview` 记录
   - 更新 `UserRecitation` 的熟练度和状态
4. **定时任务**：每小时检查是否有需要发送提醒的复习计划
5. **过期处理**：超过计划时间48小时未完成的复习标记为过期

---

## 二、成就系统

### 2.1 实体结构

#### Achievement（成就定义）
定义所有可能的成就。

**主要字段：**
- `Name`：成就名称
- `Description`：成就描述
- `Type`：成就类型（连续签到、背诵诗文、收藏诗文等）
- `IconUrl`：成就图标
- `TargetValue`：达成条件值（如：连续签到7天，这里就是7）
- `RewardPoints`：奖励积分
- `Level`：成就等级（用于同类成就的不同级别，如：铜、银、金）
- `IsHidden`：是否隐藏（隐藏成就在达成前不显示）
- `IsActive`：是否启用

#### UserAchievement（用户成就记录）
记录用户获得的成就。

**主要字段：**
- `UserId`：用户ID
- `AchievementId`：成就ID
- `AchievedAt`：获得成就的时间
- `CurrentValue`：当前进度值（用于显示进度条）
- `RewardClaimed`：是否已领取奖励
- `RewardClaimedAt`：奖励领取时间

### 2.2 成就类型（AchievementType）

```csharp
public enum AchievementType
{
    ContinuousCheckIn,      // 连续签到
    RecitationCount,        // 背诵诗文
    FavoriteCount,          // 收藏诗文
    AudioUpload,            // 上传音频
    RatingReceived,         // 获得评分
    AnnotationCount,        // 添加标注
    PointsTotal,            // 积分累计
    UsageDays,              // 使用天数
    StudyDuration,          // 学习时长
    PerfectReview,          // 完美复习（连续多次复习评分5分）
    Special                 // 特殊成就
}
```

### 2.3 示例成就定义

```csharp
// 示例1：连续签到成就
new Achievement
{
    Name = "初来乍到",
    Description = "连续签到3天",
    Type = AchievementType.ContinuousCheckIn,
    TargetValue = 3,
    RewardPoints = 50,
    Level = 1
}

new Achievement
{
    Name = "持之以恒",
    Description = "连续签到7天",
    Type = AchievementType.ContinuousCheckIn,
    TargetValue = 7,
    RewardPoints = 100,
    Level = 2
}

// 示例2：背诵成就
new Achievement
{
    Name = "初窥门径",
    Description = "背诵10首诗文",
    Type = AchievementType.RecitationCount,
    TargetValue = 10,
    RewardPoints = 100,
    Level = 1
}

new Achievement
{
    Name = "诗词达人",
    Description = "背诵100首诗文",
    Type = AchievementType.RecitationCount,
    TargetValue = 100,
    RewardPoints = 1000,
    Level = 3
}

// 示例3：完美复习成就
new Achievement
{
    Name = "过目不忘",
    Description = "连续10次复习都获得5分评价",
    Type = AchievementType.PerfectReview,
    TargetValue = 10,
    RewardPoints = 500,
    Level = 1,
    IsHidden = true  // 隐藏成就
}
```

### 2.4 使用流程

1. **初始化成就**：在数据库中预定义所有成就
2. **检查成就达成**：
   - 用户执行某个操作时（背诵、签到等）
   - 检查相关成就的进度
   - 如果达成条件，创建 `UserAchievement` 记录
   - 发送通知给用户
3. **领取奖励**：
   - 用户点击领取奖励
   - 更新 `RewardClaimed` 和 `RewardClaimedAt`
   - 增加用户积分（创建 `PointsRecord`）
4. **显示成就**：
   - 显示已获得的成就
   - 显示进行中的成就及进度
   - 隐藏未达成的隐藏成就

---

## 三、数据库迁移

创建完实体后，需要生成并应用数据库迁移：

```bash
# 在 PoemApp.Infrastructure 目录下
dotnet ef migrations add AddRecitationAndAchievementSystem --startup-project ../PoemApp.API

# 应用迁移
dotnet ef database update --startup-project ../PoemApp.API
```

---

## 四、积分来源更新

在 `PointsSource` 枚举中新增了以下积分来源：
- `RecitationCompleted`：背诵诗文
- `ReviewCompleted`：完成复习
- `AchievementUnlocked`：获得成就

---

## 五、后续开发建议

### 5.1 API 接口

需要创建以下 Controller：
- `RecitationController`：管理背诵记录和复习
  - POST /api/recitations - 开始背诵
  - GET /api/recitations/my - 获取我的背诵列表
  - GET /api/recitations/{id}/reviews - 获取某个背诵的复习记录
  - POST /api/recitations/{id}/reviews - 完成一次复习
  - GET /api/recitations/due - 获取待复习列表
  
- `AchievementController`：管理成就
  - GET /api/achievements - 获取所有成就
  - GET /api/achievements/my - 获取我的成就
  - POST /api/achievements/{id}/claim - 领取成就奖励
  - GET /api/achievements/progress - 获取成就进度

### 5.2 后台任务

需要创建以下定时任务：
1. **复习提醒任务**（每小时执行）：
   - 查找需要复习的记录
   - 发送推送通知或邮件提醒
   
2. **过期复习处理任务**（每天执行）：
   - 将超过48小时未完成的复习标记为过期
   
3. **成就检查任务**（实时或定时）：
   - 检查用户是否达成新成就
   - 发送通知

### 5.3 前端页面

需要创建以下页面：
1. **我的背诵**：显示背诵列表和复习计划
2. **今日复习**：显示今天需要复习的内容
3. **成就大厅**：显示所有成就和个人进度
4. **成就详情**：显示某个成就的详细信息和获得者列表

### 5.4 推送通知

建议集成推送服务（如 Firebase、极光推送等）：
- 复习提醒
- 成就解锁通知
- 奖励领取提醒

---

## 六、参考资料

- [艾宾浩斯遗忘曲线](https://zh.wikipedia.org/wiki/%E9%81%97%E5%BF%98%E6%9B%B2%E7%BA%BF)
- 间隔重复学习法（Spaced Repetition）
- Anki 记忆算法

---

Created: 2025
Author: PoemApp Development Team
