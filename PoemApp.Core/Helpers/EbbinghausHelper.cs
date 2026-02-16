namespace PoemApp.Core.Helpers;

/// <summary>
/// 艾宾浩斯遗忘曲线复习计划助手
/// 基于艾宾浩斯遗忘曲线理论，计算最佳复习时间点
/// </summary>
public static class EbbinghausHelper
{
    /// <summary>
    /// 艾宾浩斯遗忘曲线的标准复习间隔（天）
    /// 第1次复习：1天后
    /// 第2次复习：2天后
    /// 第3次复习：4天后
    /// 第4次复习：7天后
    /// 第5次复习：15天后
    /// 第6次复习：30天后
    /// 第7次及以后：60天后
    /// </summary>
    private static readonly int[] ReviewIntervals = { 1, 2, 4, 7, 15, 30, 60 };

    /// <summary>
    /// 根据复习轮次获取复习间隔天数
    /// </summary>
    /// <param name="reviewRound">复习轮次（从1开始）</param>
    /// <returns>距离上次复习的天数</returns>
    public static int GetReviewInterval(int reviewRound)
    {
        if (reviewRound <= 0)
            return 1;

        // 如果超过预定义的复习次数，使用最后一个间隔（60天）
        if (reviewRound > ReviewIntervals.Length)
            return ReviewIntervals[^1];

        return ReviewIntervals[reviewRound - 1];
    }

    /// <summary>
    /// 计算下次复习时间
    /// </summary>
    /// <param name="lastReviewTime">上次复习时间</param>
    /// <param name="nextReviewRound">下次复习轮次</param>
    /// <returns>下次复习时间</returns>
    public static DateTime CalculateNextReviewTime(DateTime lastReviewTime, int nextReviewRound)
    {
        int intervalDays = GetReviewInterval(nextReviewRound);
        return lastReviewTime.AddDays(intervalDays);
    }

    /// <summary>
    /// 根据复习质量调整下次复习间隔
    /// 如果复习质量差（评分低），缩短间隔；如果质量好，可以略微延长
    /// </summary>
    /// <param name="lastReviewTime">上次复习时间</param>
    /// <param name="nextReviewRound">下次复习轮次</param>
    /// <param name="qualityRating">复习质量评分（1-5）</param>
    /// <returns>调整后的下次复习时间</returns>
    public static DateTime CalculateNextReviewTimeWithQuality(
        DateTime lastReviewTime,
        int nextReviewRound,
        int qualityRating)
    {
        int baseInterval = GetReviewInterval(nextReviewRound);
        double adjustedInterval = baseInterval;

        // 根据质量评分调整间隔
        switch (qualityRating)
        {
            case 1: // 完全忘记 - 缩短到原来的30%
                adjustedInterval = baseInterval * 0.3;
                break;
            case 2: // 模糊 - 缩短到原来的50%
                adjustedInterval = baseInterval * 0.5;
                break;
            case 3: // 能想起 - 保持标准间隔
                adjustedInterval = baseInterval * 1.0;
                break;
            case 4: // 熟练 - 延长到原来的120%
                adjustedInterval = baseInterval * 1.2;
                break;
            case 5: // 完美 - 延长到原来的150%
                adjustedInterval = baseInterval * 1.5;
                break;
            default:
                adjustedInterval = baseInterval;
                break;
        }

        return lastReviewTime.AddDays(Math.Max(1, (int)adjustedInterval));
    }

    /// <summary>
    /// 计算熟练度（0-100）
    /// 基于复习次数和最近的复习质量
    /// </summary>
    /// <param name="reviewCount">总复习次数</param>
    /// <param name="recentQualityRatings">最近3次的复习质量评分</param>
    /// <returns>熟练度（0-100）</returns>
    public static int CalculateProficiency(int reviewCount, IEnumerable<int> recentQualityRatings)
    {
        var ratings = recentQualityRatings.ToList();
        
        // 基础熟练度：根据复习次数
        int baseProficiency = Math.Min(reviewCount * 10, 50);

        // 最近表现加成：根据最近的复习质量
        if (ratings.Count > 0)
        {
            double averageQuality = ratings.Average();
            int qualityBonus = (int)((averageQuality / 5.0) * 50);
            return Math.Min(baseProficiency + qualityBonus, 100);
        }

        return baseProficiency;
    }

    /// <summary>
    /// 判断是否应该发送复习提醒
    /// 在计划复习时间前1小时到后24小时内发送
    /// </summary>
    /// <param name="scheduledTime">计划复习时间</param>
    /// <param name="now">当前时间</param>
    /// <returns>是否应该发送提醒</returns>
    public static bool ShouldSendReminder(DateTime scheduledTime, DateTime now)
    {
        // 在计划时间前1小时到后24小时内
        var reminderWindowStart = scheduledTime.AddHours(-1);
        var reminderWindowEnd = scheduledTime.AddHours(24);
        
        return now >= reminderWindowStart && now <= reminderWindowEnd;
    }

    /// <summary>
    /// 判断复习是否过期
    /// 如果超过计划时间48小时仍未完成，标记为过期
    /// </summary>
    /// <param name="scheduledTime">计划复习时间</param>
    /// <param name="now">当前时间</param>
    /// <returns>是否过期</returns>
    public static bool IsReviewExpired(DateTime scheduledTime, DateTime now)
    {
        return now > scheduledTime.AddHours(48);
    }
}
