using System.Collections.Immutable;
using System.Globalization;

namespace seeker.literaturetime;

internal static class Phrases
{
    private static readonly Dictionary<int, string> NumberToWord = new()
    {
        { 0, "twelve" },
        { 1, "one" },
        { 2, "two" },
        { 3, "three" },
        { 4, "four" },
        { 5, "five" },
        { 6, "six" },
        { 7, "seven" },
        { 8, "eight" },
        { 9, "nine" },
        { 10, "ten" },
        { 11, "eleven" },
        { 12, "twelve" },
        { 13, "thirteen" },
        { 14, "fourteen" },
        { 15, "fifteen" },
        { 16, "sixteen" },
        { 17, "seventeen" },
        { 18, "eighteen" },
        { 19, "nineteen" },
        { 20, "twenty" },
        { 21, "twenty-one" },
        { 22, "twenty-two" },
        { 23, "twenty-three" },
        { 24, "twenty-four" },
        { 25, "twenty-five" },
        { 26, "twenty-six" },
        { 27, "twenty-seven" },
        { 28, "twenty-eight" },
        { 29, "twenty-nine" },
        { 30, "thirty" },
        { 31, "thirty-one" },
        { 32, "thirty-two" },
        { 33, "thirty-three" },
        { 34, "thirty-four" },
        { 35, "thirty-five" },
        { 36, "thirty-six" },
        { 37, "thirty-seven" },
        { 38, "thirty-eight" },
        { 39, "thirty-nine" },
        { 40, "forty" },
        { 41, "forty-one" },
        { 42, "forty-two" },
        { 43, "forty-three" },
        { 44, "forty-four" },
        { 45, "forty-five" },
        { 46, "forty-six" },
        { 47, "forty-seven" },
        { 48, "forty-eight" },
        { 49, "forty-nine" },
        { 50, "fifty" },
        { 51, "fifty-one" },
        { 52, "fifty-two" },
        { 53, "fifty-three" },
        { 54, "fifty-four" },
        { 55, "fifty-five" },
        { 56, "fifty-six" },
        { 57, "fifty-seven" },
        { 58, "fifty-eight" },
        { 59, "fifty-nine" },
        { 60, "sixty" },
    };

    private static IEnumerable<string> AppendAm(string timePhrase) =>
        Am.Select(v => $"{timePhrase}{v}");

    private static IEnumerable<string> AppendPm(string timePhrase) =>
        Pm.Select(v => $"{timePhrase}{v}");

    private static List<string> Am =>
        ["am", "a.m.", "a. m.", " am", " a.m.", " a. m.", "  am", "  a.m.", "  a. m."];

    private static List<string> Pm =>
        ["pm", "p.m.", "p. m.", " pm", " p.m.", " p. m.", "  pm", "  p.m.", "  p. m."];

    private static List<string> ALittleAfter =>
        ["a little after ", "just after ", "about ", "shortly after "];

    private static List<string> AlmostAt => ["almost at ", "nearly ", "about ", "nearing "];

    private static List<string> OClock =>
        [" o'clock", " o’clock", " of the clock", " o' the clock", " o’ the clock", " o the clock"];

    private static List<string> Half => ["half past ", "half-past ", "half after ", "half-after "];

    private static List<string> QuarterPast =>
        [
            "quarter past ",
            "quarter-past ",
            "quarter after ",
            "quarter-after ",
            "a quarter past ",
            "a quarter after ",
        ];

    private static List<string> QuarterTo => ["quarter to ", "quarter-to ", "a quarter to "];

    private static List<string> Noon => ["noon", "midday", "mid-day", "noontide"];

    private static List<string> Struck =>
        [
            "it struck ",
            "it chimed ",
            "it tolled ",
            "it rang ",
            "the clock struck ",
            "the clock chimed ",
            "the clock tolled ",
            "the clock rang ",
        ];

    private static List<string> StrokeOf => ["stroke of ", "chime of ", "toll of "];

    private static List<string> InTheMorning =>
        [" in the morning", " in the morn", " that morning", " that morn"];

    private static List<string> InTheAfterNoon => [" in the afternoon", " that afternoon"];

    private static List<string> InTheEvening => [" in the evening", " that evening"];

    private static List<string> GetMinuteWords(int minute)
    {
        var words = new List<string> { NumberToWord[minute] };
        if (minute is > 20 and < 60 && minute % 10 != 0)
        {
            var tens = (minute / 10) * 10;
            words.Add($"{NumberToWord[minute % 10]} and {NumberToWord[tens]}");
        }
        return words;
    }

    private static List<string> Combine(
        List<string> listA,
        List<string> listB,
        List<string> listC,
        List<string> listD
    )
    {
        List<string> result = [];
        foreach (var a in listA)
        {
            foreach (var b in listB)
            {
                foreach (var c in listC)
                {
                    foreach (var d in listD)
                    {
                        var r = $"{a}{b}{c}{d}";
                        result.Add(r);
                    }
                }
            }
        }

        return result;
    }

    private static List<string> Combine(List<string> listA, List<string> listB, List<string> listC)
    {
        List<string> result = [];
        foreach (var a in listA)
        {
            foreach (var b in listB)
            {
                foreach (var c in listC)
                {
                    var r = $"{a}{b}{c}";
                    result.Add(r);
                }
            }
        }

        return result;
    }

    private static List<string> Combine(List<string> listA, List<string> listB)
    {
        List<string> result = [];
        foreach (var a in listA)
        {
            foreach (var b in listB)
            {
                var r = $"{a}{b}";
                result.Add(r);
            }
        }

        return result;
    }

    public static (
        ImmutableDictionary<string, List<string>>,
        ImmutableDictionary<string, List<string>>,
        ImmutableDictionary<string, List<string>>
    ) GeneratePhrases()
    {
        var timePhrasesOneOf = new Dictionary<string, List<string>>();
        var timePhrasesGenericOneOf = new Dictionary<string, List<string>>();
        var timePhrasesSuperGenericOneOf = new Dictionary<string, List<string>>();

        var startOfDay = DateTime.Now.Date;
        var endOfDay = startOfDay.Date.AddDays(1).AddTicks(-1);

        while (startOfDay < endOfDay)
        {
            var pastMinutePlural = startOfDay.Minute == 1 ? "minute" : "minutes";
            var toMinutePlural = startOfDay.Minute == 59 ? "minute" : "minutes";

            var currentTimePhrasesOneOf = new List<string>();
            var currentTimePhrasesGenericOneOf = new List<string>();
            var currentTimePhrasesSuperGenericOneOf = new List<string>();

            // AM
            if (startOfDay.Hour < 12)
            {
                var hour = startOfDay.Hour;
                var hourWord = NumberToWord[hour];

                if (startOfDay.Minute == 0)
                {
                    currentTimePhrasesOneOf.AddRange(
                        Combine(
                            ["at "],
                            [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                            Am
                        )
                    );

                    currentTimePhrasesOneOf.AddRange(
                        Combine(
                            ["at "],
                            [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                            OClock,
                            Am
                        )
                    );

                    currentTimePhrasesOneOf.AddRange(AppendAm($"{hourWord}"));
                    foreach (var oc in OClock)
                    {
                        currentTimePhrasesOneOf.AddRange(AppendAm($"{hourWord}{oc}"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"{hour}{oc}"));
                    }

                    foreach (var s in Struck)
                    {
                        currentTimePhrasesOneOf.AddRange(AppendAm($"{s}{hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"{s}{hour}"));
                    }
                    foreach (var s in StrokeOf)
                    {
                        currentTimePhrasesOneOf.AddRange(AppendAm($"{s}{hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"{s}{hour}"));
                    }

                    currentTimePhrasesOneOf.AddRange(AppendAm($"the hour of {hourWord}"));
                    currentTimePhrasesOneOf.AddRange(AppendAm($"the hour of {hour}"));
                }

                currentTimePhrasesSuperGenericOneOf.AddRange(AppendAm($"{startOfDay:h:mm}"));
                currentTimePhrasesSuperGenericOneOf.AddRange(AppendAm($"{startOfDay:h.mm}"));
            }

            // AM
            if (startOfDay is { Hour: < 12, Minute: > 0 })
            {
                var hour = startOfDay.Hour;
                var hourWord = NumberToWord[hour];

                var minute = startOfDay.Minute;
                var minuteWords = GetMinuteWords(minute);

                var toHour = hour + 1;
                var toHourWord = NumberToWord[toHour];

                var toMinute = 60 - minute;
                var toMinuteWords = GetMinuteWords(toMinute);

                // four thirty a.m.
                currentTimePhrasesOneOf.AddRange(AppendAm($"{hourWord} {NumberToWord[minute]}"));

                foreach (var mw in minuteWords)
                {
                    // twelve minute(s) past nineteen a.m.
                    currentTimePhrasesOneOf.AddRange(
                        AppendAm($"{mw} {pastMinutePlural} past {hourWord}")
                    );
                    // twelve minute(s) after nineteen a.m.
                    currentTimePhrasesOneOf.AddRange(
                        AppendAm($"{mw} {pastMinutePlural} after {hourWord}")
                    );
                    // twelve past nineteen a.m.
                    currentTimePhrasesOneOf.AddRange(AppendAm($"{mw} past {hourWord}"));
                    // twelve after nineteen a.m.
                    currentTimePhrasesOneOf.AddRange(AppendAm($"{mw} after {hourWord}"));
                }

                // 12 minute(s) past 19 a.m.
                currentTimePhrasesOneOf.AddRange(
                    AppendAm($"{minute} {pastMinutePlural} past {hour}")
                );
                // 12 minute(s) after 19 a.m.
                currentTimePhrasesOneOf.AddRange(
                    AppendAm($"{minute} {pastMinutePlural} after {hour}")
                );
                // 12 past 19 a.m.
                currentTimePhrasesOneOf.AddRange(AppendAm($"{minute} past {hour}"));
                // 12 after 19 a.m.
                currentTimePhrasesOneOf.AddRange(AppendAm($"{minute} after {hour}"));

                foreach (var tmw in toMinuteWords)
                {
                    // twelve minute(s) to nineteen a.m.
                    currentTimePhrasesOneOf.AddRange(
                        AppendAm($"{tmw} {toMinutePlural} to {toHourWord}")
                    );
                    // twelve to nineteen a.m.
                    currentTimePhrasesOneOf.AddRange(AppendAm($"{tmw} to {toHourWord}"));
                }

                // 12 minute(s) to 19 a.m.
                currentTimePhrasesOneOf.AddRange(
                    AppendAm($"{toMinute} {toMinutePlural} to {toHour}")
                );
                // 12 to 19 a.m.
                currentTimePhrasesOneOf.AddRange(AppendAm($"{toMinute} to {toHour}"));

                switch (startOfDay.Minute)
                {
                    case < 4:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ALittleAfter,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                Am
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ALittleAfter,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                Am
                            )
                        );
                        break;
                    case 15:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                QuarterPast,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                Am
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                QuarterPast,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                Am
                            )
                        );
                        break;
                    case 30:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                Half,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                Am
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                Half,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                Am
                            )
                        );
                        break;
                    case 45:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                QuarterTo,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                Am
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                QuarterTo,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                Am
                            )
                        );
                        break;
                    case >= 57 and < 60:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                AlmostAt,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                Am
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                AlmostAt,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                Am
                            )
                        );
                        break;
                }
            }

            // PM
            if (startOfDay is { Hour: >= 12 and <= 23 })
            {
                if (startOfDay.Minute == 0)
                {
                    var hour = startOfDay.Hour == 12 ? startOfDay.Hour : startOfDay.Hour - 12;
                    var hourWord = NumberToWord[hour];

                    currentTimePhrasesOneOf.AddRange(
                        Combine(
                            ["at "],
                            [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                            Pm
                        )
                    );

                    currentTimePhrasesOneOf.AddRange(
                        Combine(
                            ["at "],
                            [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                            OClock,
                            Pm
                        )
                    );

                    currentTimePhrasesOneOf.AddRange(AppendPm($"{hourWord}"));
                    foreach (var oc in OClock)
                    {
                        currentTimePhrasesOneOf.AddRange(AppendPm($"{hourWord}{oc}"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"{hour}{oc}"));
                    }

                    foreach (var s in Struck)
                    {
                        currentTimePhrasesOneOf.AddRange(AppendPm($"{s}{hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"{s}{hour}"));
                    }
                    foreach (var s in StrokeOf)
                    {
                        currentTimePhrasesOneOf.AddRange(AppendPm($"{s}{hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"{s}{hour}"));
                    }

                    currentTimePhrasesOneOf.AddRange(AppendPm($"the hour of {hourWord}"));
                    currentTimePhrasesOneOf.AddRange(AppendPm($"the hour of {hour}"));
                }

                currentTimePhrasesSuperGenericOneOf.AddRange(AppendPm($"{startOfDay:h:mm}"));
                currentTimePhrasesSuperGenericOneOf.AddRange(AppendPm($"{startOfDay:h.mm}"));
            }

            // PM
            if (startOfDay is { Hour: >= 12 and <= 23, Minute: > 0 })
            {
                var hour = startOfDay.Hour == 12 ? startOfDay.Hour : startOfDay.Hour - 12;
                var hourWord = NumberToWord[hour];

                var minute = startOfDay.Minute;
                var minuteWords = GetMinuteWords(minute);

                var toHour = hour + 1;
                var toHourWord = NumberToWord[toHour];

                var toMinute = 60 - minute;
                var toMinuteWords = GetMinuteWords(toMinute);

                // four thirty p.m.
                currentTimePhrasesOneOf.AddRange(AppendPm($"{hourWord} {NumberToWord[minute]}"));

                foreach (var mw in minuteWords)
                {
                    // twelve minute(s) past nineteen p.m.
                    currentTimePhrasesOneOf.AddRange(
                        AppendPm($"{mw} {pastMinutePlural} past {hourWord}")
                    );
                    // twelve minute(s) after nineteen p.m.
                    currentTimePhrasesOneOf.AddRange(
                        AppendPm($"{mw} {pastMinutePlural} after {hourWord}")
                    );
                    // twelve past nineteen p.m.
                    currentTimePhrasesOneOf.AddRange(AppendPm($"{mw} past {hourWord}"));
                    // twelve after nineteen p.m.
                    currentTimePhrasesOneOf.AddRange(AppendPm($"{mw} after {hourWord}"));
                }

                // 12 minute(s) past 19 p.m.
                currentTimePhrasesOneOf.AddRange(
                    AppendPm($"{minute} {pastMinutePlural} past {hour}")
                );
                // 12 minute(s) after 19 p.m.
                currentTimePhrasesOneOf.AddRange(
                    AppendPm($"{minute} {pastMinutePlural} after {hour}")
                );
                // 12 past 19 p.m.
                currentTimePhrasesOneOf.AddRange(AppendPm($"{minute} past {hour}"));
                // 12 after 19 p.m.
                currentTimePhrasesOneOf.AddRange(AppendPm($"{minute} after {hour}"));

                foreach (var tmw in toMinuteWords)
                {
                    // twelve minute(s) to nineteen p.m.
                    currentTimePhrasesOneOf.AddRange(
                        AppendPm($"{tmw} {toMinutePlural} to {toHourWord}")
                    );
                    // twelve to nineteen p.m.
                    currentTimePhrasesOneOf.AddRange(AppendPm($"{tmw} to {toHourWord}"));
                }

                // 12 minute(s) to 19 p.m.
                currentTimePhrasesOneOf.AddRange(
                    AppendPm($"{toMinute} {toMinutePlural} to {toHour}")
                );
                // 12 to 19 p.m.
                currentTimePhrasesOneOf.AddRange(AppendPm($"{toMinute} to {toHour}"));

                switch (startOfDay.Minute)
                {
                    case < 4:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ALittleAfter,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                Pm
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ALittleAfter,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                Pm
                            )
                        );
                        break;

                    case 15:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                QuarterPast,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                Pm
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                QuarterPast,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                Pm
                            )
                        );
                        break;
                    case 30:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                Half,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                Pm
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                Half,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                Pm
                            )
                        );
                        break;
                    case 45:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                QuarterTo,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                Pm
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                QuarterTo,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                Pm
                            )
                        );
                        break;
                    case >= 57 and < 60:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                AlmostAt,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                Pm
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                AlmostAt,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                Pm
                            )
                        );
                        break;
                }
            }

            // Generic
            if (startOfDay.Hour is >= 0 and <= 23)
            {
                if (startOfDay.Minute == 0)
                {
                    var hour = startOfDay.Hour > 12 ? startOfDay.Hour - 12 : startOfDay.Hour;
                    var hourWord = NumberToWord[hour];

                    currentTimePhrasesOneOf.AddRange(
                        Combine(
                            ["at "],
                            [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                            OClock
                        )
                    );

                    foreach (var oc in OClock)
                    {
                        currentTimePhrasesOneOf.Add($"{hourWord}{oc}");
                        currentTimePhrasesOneOf.Add($"{hour}{oc}");
                    }

                    foreach (var s in Struck)
                    {
                        currentTimePhrasesOneOf.Add($"{s}{hourWord}");
                        currentTimePhrasesOneOf.Add($"{s}{hour}");
                    }
                    foreach (var s in StrokeOf)
                    {
                        currentTimePhrasesOneOf.Add($"{s}{hourWord}");
                        currentTimePhrasesOneOf.Add($"{s}{hour}");
                    }

                    currentTimePhrasesOneOf.Add($"the hour of {hourWord}");
                    currentTimePhrasesOneOf.Add($"the hour of {hour}");
                }
            }

            if (startOfDay.Hour is >= 0 and <= 23 && startOfDay.Minute > 0)
            {
                var hour = startOfDay.Hour > 12 ? startOfDay.Hour - 12 : startOfDay.Hour;
                var hourWord = NumberToWord[hour];

                var toHour = startOfDay.Hour > 12 ? startOfDay.Hour - 12 + 1 : startOfDay.Hour + 1;
                var toHourWord = NumberToWord[toHour];

                var minute = startOfDay.Minute;
                var minuteWords = GetMinuteWords(minute);

                var toMinute = 60 - minute;
                var toMinuteWords = GetMinuteWords(toMinute);

                foreach (var mw in minuteWords)
                {
                    currentTimePhrasesGenericOneOf.Add($"{mw} {pastMinutePlural} past {hourWord}");
                    currentTimePhrasesGenericOneOf.Add($"{mw} {pastMinutePlural} after {hourWord}");
                    currentTimePhrasesGenericOneOf.Add($"{mw} past {hourWord}");
                    currentTimePhrasesGenericOneOf.Add($"{mw} after {hourWord}");
                }

                currentTimePhrasesGenericOneOf.Add($"{minute} {pastMinutePlural} past {hour}");
                currentTimePhrasesGenericOneOf.Add($"{minute} {pastMinutePlural} after {hour}");
                currentTimePhrasesGenericOneOf.Add($"{minute} past {hour}");
                currentTimePhrasesGenericOneOf.Add($"{minute} after {hour}");

                foreach (var tmw in toMinuteWords)
                {
                    currentTimePhrasesGenericOneOf.Add($"{tmw} {toMinutePlural} to {toHourWord}");
                    currentTimePhrasesGenericOneOf.Add($"{tmw} to {toHourWord}");
                }

                currentTimePhrasesGenericOneOf.Add($"{toMinute} {toMinutePlural} to {toHour}");
                currentTimePhrasesGenericOneOf.Add($"{toMinute} to {toHour}");

                switch (startOfDay.Minute)
                {
                    case < 4:
                        currentTimePhrasesGenericOneOf.AddRange(
                            Combine(
                                ALittleAfter,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                OClock
                            )
                        );

                        break;
                    case 15:
                        currentTimePhrasesGenericOneOf.AddRange(
                            Combine(
                                QuarterPast,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                OClock
                            )
                        );

                        currentTimePhrasesGenericOneOf.AddRange(
                            Combine(
                                QuarterPast,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)]
                            )
                        );
                        break;
                    case 30:
                        currentTimePhrasesGenericOneOf.AddRange(
                            Combine(
                                Half,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                OClock
                            )
                        );

                        currentTimePhrasesGenericOneOf.AddRange(
                            Combine(Half, [hourWord, hour.ToString(CultureInfo.InvariantCulture)])
                        );
                        break;
                    case 45:
                        currentTimePhrasesGenericOneOf.AddRange(
                            Combine(
                                QuarterTo,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                OClock
                            )
                        );

                        currentTimePhrasesGenericOneOf.AddRange(
                            Combine(
                                QuarterTo,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)]
                            )
                        );
                        break;
                    case >= 57 and < 60:
                        currentTimePhrasesGenericOneOf.AddRange(
                            Combine(
                                AlmostAt,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                OClock
                            )
                        );

                        break;
                }
            }

            // AM
            if (startOfDay is { Hour: >= 6 and <= 11, Minute: > 0 })
            {
                var hour = startOfDay.Hour;
                var hourWord = NumberToWord[hour];

                var minute = startOfDay.Minute;
                var minuteWords = GetMinuteWords(minute);

                var toHour = hour + 1;
                var toHourWord = NumberToWord[toHour];

                var toMinute = 60 - minute;
                var toMinuteWords = GetMinuteWords(toMinute);

                currentTimePhrasesOneOf.AddRange(
                    Combine(
                        minuteWords,
                        ["", $" {pastMinutePlural}"],
                        [$" past {hourWord}", $" after {hourWord}"],
                        InTheMorning
                    )
                );

                currentTimePhrasesOneOf.AddRange(
                    Combine(
                        [$"{minute}"],
                        ["", $" {pastMinutePlural}"],
                        [$" past {hour}", $" after {hour}"],
                        InTheMorning
                    )
                );

                currentTimePhrasesOneOf.AddRange(
                    Combine(
                        toMinuteWords,
                        ["", $" {toMinutePlural}"],
                        [$" to {toHourWord}"],
                        InTheMorning
                    )
                );

                currentTimePhrasesOneOf.AddRange(
                    Combine(
                        [$"{toMinute}"],
                        ["", $" {toMinutePlural}"],
                        [$" to {toHour}"],
                        InTheMorning
                    )
                );

                switch (startOfDay.Minute)
                {
                    case < 4:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ALittleAfter,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                InTheMorning
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ALittleAfter,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                InTheMorning
                            )
                        );

                        break;
                    case 15:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                QuarterPast,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                InTheMorning
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                QuarterPast,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                InTheMorning
                            )
                        );

                        break;

                    case 30:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                Half,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                InTheMorning
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                Half,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                InTheMorning
                            )
                        );

                        break;

                    case 45:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                QuarterTo,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                InTheMorning
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                QuarterTo,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                InTheMorning
                            )
                        );

                        break;

                    case >= 57
                    and < 60:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                AlmostAt,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                InTheMorning
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                AlmostAt,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                InTheMorning
                            )
                        );

                        break;
                }
            }

            if (startOfDay is { Hour: >= 6 and <= 12, Minute: 0 })
            {
                var hour = startOfDay.Hour;
                var hourWord = NumberToWord[hour];

                currentTimePhrasesOneOf.AddRange(
                    Combine([hourWord, hour.ToString(CultureInfo.InvariantCulture)], InTheMorning)
                );

                currentTimePhrasesOneOf.AddRange(
                    Combine(
                        [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                        OClock,
                        InTheMorning
                    )
                );
            }

            // PM
            if (startOfDay is { Hour: >= 12 and <= 17, Minute: > 0 })
            {
                var hour = startOfDay.Hour == 12 ? startOfDay.Hour : startOfDay.Hour - 12;
                var hourWord = NumberToWord[hour];

                var minute = startOfDay.Minute;
                var minuteWords = GetMinuteWords(minute);

                var toHour = hour + 1;
                var toHourWord = NumberToWord[toHour];

                var toMinute = 60 - minute;
                var toMinuteWords = GetMinuteWords(toMinute);

                currentTimePhrasesOneOf.AddRange(
                    Combine(
                        minuteWords,
                        ["", $" {pastMinutePlural}"],
                        [$" past {hourWord}", $" after {hourWord}"],
                        InTheAfterNoon
                    )
                );

                currentTimePhrasesOneOf.AddRange(
                    Combine(
                        [$"{minute}"],
                        ["", $" {pastMinutePlural}"],
                        [$" past {hour}", $" after {hour}"],
                        InTheAfterNoon
                    )
                );

                currentTimePhrasesOneOf.AddRange(
                    Combine(
                        toMinuteWords,
                        ["", $" {toMinutePlural}"],
                        [$" to {toHourWord}"],
                        InTheAfterNoon
                    )
                );

                currentTimePhrasesOneOf.AddRange(
                    Combine(
                        [$"{toMinute}"],
                        ["", $" {toMinutePlural}"],
                        [$" to {toHour}"],
                        InTheAfterNoon
                    )
                );

                switch (startOfDay.Minute)
                {
                    case < 4:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ALittleAfter,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                InTheAfterNoon
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ALittleAfter,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                InTheAfterNoon
                            )
                        );

                        break;
                    case 15:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                QuarterPast,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                InTheAfterNoon
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                QuarterPast,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                InTheAfterNoon
                            )
                        );

                        break;
                    case 30:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                Half,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                InTheAfterNoon
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                Half,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                InTheAfterNoon
                            )
                        );

                        break;
                    case 45:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                QuarterTo,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                InTheAfterNoon
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                QuarterTo,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                InTheAfterNoon
                            )
                        );

                        break;
                    case >= 57 and < 60:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                AlmostAt,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                InTheAfterNoon
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                AlmostAt,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                InTheAfterNoon
                            )
                        );

                        break;
                }
            }

            if (startOfDay is { Hour: >= 12 and <= 18, Minute: 0 })
            {
                var hour = startOfDay.Hour == 12 ? startOfDay.Hour : startOfDay.Hour - 12;
                var hourWord = NumberToWord[hour];

                currentTimePhrasesOneOf.AddRange(
                    Combine([hourWord, hour.ToString(CultureInfo.InvariantCulture)], InTheAfterNoon)
                );

                currentTimePhrasesOneOf.AddRange(
                    Combine(
                        [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                        OClock,
                        InTheAfterNoon
                    )
                );
            }

            if (startOfDay is { Hour: >= 18 and <= 20, Minute: > 0 })
            {
                var hour = startOfDay.Hour - 12;
                var hourWord = NumberToWord[hour];

                var minute = startOfDay.Minute;
                var minuteWords = GetMinuteWords(minute);

                var toHour = hour + 1;
                var toHourWord = NumberToWord[toHour];

                var toMinute = 60 - minute;
                var toMinuteWords = GetMinuteWords(toMinute);

                currentTimePhrasesOneOf.AddRange(
                    Combine(
                        minuteWords,
                        ["", $" {pastMinutePlural}"],
                        [$" past {hourWord}", $" after {hourWord}"],
                        InTheEvening
                    )
                );

                currentTimePhrasesOneOf.AddRange(
                    Combine(
                        [$"{minute}"],
                        ["", $" {pastMinutePlural}"],
                        [$" past {hour}", $" after {hour}"],
                        InTheEvening
                    )
                );

                currentTimePhrasesOneOf.AddRange(
                    Combine(
                        toMinuteWords,
                        ["", $" {toMinutePlural}"],
                        [$" to {toHourWord}"],
                        InTheEvening
                    )
                );

                currentTimePhrasesOneOf.AddRange(
                    Combine(
                        [$"{toMinute}"],
                        ["", $" {toMinutePlural}"],
                        [$" to {toHour}"],
                        InTheEvening
                    )
                );

                switch (startOfDay.Minute)
                {
                    case < 4:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ALittleAfter,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                InTheEvening
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ALittleAfter,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                InTheEvening
                            )
                        );

                        break;
                    case 15:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                QuarterPast,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                InTheEvening
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                QuarterPast,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                InTheEvening
                            )
                        );

                        break;
                    case 30:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                Half,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                InTheEvening
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                Half,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                InTheEvening
                            )
                        );

                        break;
                    case 45:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                QuarterTo,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                InTheEvening
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                QuarterTo,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                InTheEvening
                            )
                        );

                        break;
                    case >= 57 and < 60:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                AlmostAt,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                InTheEvening
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                AlmostAt,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                InTheEvening
                            )
                        );

                        break;
                }
            }

            if (startOfDay is { Hour: >= 18 and <= 21, Minute: 0 })
            {
                var hour = startOfDay.Hour - 12;
                var hourWord = NumberToWord[hour];

                currentTimePhrasesOneOf.AddRange(
                    Combine([hourWord, hour.ToString(CultureInfo.InvariantCulture)], InTheEvening)
                );

                currentTimePhrasesOneOf.AddRange(
                    Combine(
                        [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                        OClock,
                        InTheEvening
                    )
                );
            }

            if (startOfDay is { Hour: >= 21 and <= 23 or >= 0 and <= 5, Minute: > 0 })
            {
                var hour = startOfDay is { Hour: >= 21 and <= 23 }
                    ? startOfDay.Hour - 12
                    : startOfDay.Hour;

                var hourWord = NumberToWord[hour];

                var minute = startOfDay.Minute;
                var minuteWords = GetMinuteWords(minute);

                var toHour = hour + 1;
                var toHourWord = NumberToWord[toHour];

                var toMinute = 60 - minute;
                var toMinuteWords = GetMinuteWords(toMinute);

                currentTimePhrasesOneOf.AddRange(
                    Combine(
                        minuteWords,
                        ["", $" {pastMinutePlural}"],
                        [$" past {hourWord}", $" after {hourWord}"],
                        [" at night", " that night"]
                    )
                );

                currentTimePhrasesOneOf.AddRange(
                    Combine(
                        [$"{minute}"],
                        ["", $" {pastMinutePlural}"],
                        [$" past {hour}", $" after {hour}"],
                        [" at night", " that night"]
                    )
                );

                currentTimePhrasesOneOf.AddRange(
                    Combine(
                        toMinuteWords,
                        ["", $" {toMinutePlural}"],
                        [$" to {toHourWord}"],
                        [" at night", " that night"]
                    )
                );

                currentTimePhrasesOneOf.AddRange(
                    Combine(
                        [$"{toMinute}"],
                        ["", $" {toMinutePlural}"],
                        [$" to {toHour}"],
                        [" at night", " that night"]
                    )
                );

                switch (startOfDay.Minute)
                {
                    case < 4:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ALittleAfter,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                [" at night", " that night"]
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ALittleAfter,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                [" at night", " that night"]
                            )
                        );

                        break;
                    case 15:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                QuarterPast,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                [" at night", " that night"]
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                QuarterPast,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                [" at night", " that night"]
                            )
                        );

                        break;
                    case 30:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                Half,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                [" at night", " that night"]
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                Half,
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                [" at night", " that night"]
                            )
                        );

                        break;
                    case 45:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                QuarterTo,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                [" at night", " that night"]
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                QuarterTo,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                [" at night", " that night"]
                            )
                        );

                        break;
                    case >= 57 and < 60:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                AlmostAt,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                [" at night", " that night"]
                            )
                        );

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                AlmostAt,
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                OClock,
                                [" at night", " that night"]
                            )
                        );

                        break;
                }
            }

            if (startOfDay is { Hour: >= 21 and <= 23 or >= 0 and <= 6, Minute: 0 })
            {
                var hour = startOfDay is { Hour: >= 21 and <= 23 }
                    ? startOfDay.Hour - 12
                    : startOfDay.Hour;

                var hourWord = NumberToWord[hour];

                currentTimePhrasesOneOf.AddRange(
                    Combine(
                        [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                        [" at night", " that night"]
                    )
                );

                currentTimePhrasesOneOf.AddRange(
                    Combine(
                        [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                        OClock,
                        [" at night", " that night"]
                    )
                );
            }

            // Noon
            if (startOfDay is { Hour: 12, Minute: > 0 })
            {
                var minuteWords = GetMinuteWords(startOfDay.Minute);

                foreach (var mw in minuteWords)
                {
                    foreach (var n in Noon)
                    {
                        currentTimePhrasesOneOf.Add($"{mw} {pastMinutePlural} past {n}");
                        currentTimePhrasesOneOf.Add($"{mw} {pastMinutePlural} after {n}");
                        currentTimePhrasesOneOf.Add($"{mw} past {n}");
                        currentTimePhrasesOneOf.Add($"{mw} after {n}");
                    }
                }

                switch (startOfDay.Minute)
                {
                    case < 4:
                        foreach (var n in Noon)
                        {
                            currentTimePhrasesOneOf.Add($"a little after {n}");
                            currentTimePhrasesOneOf.Add($"just after {n}");
                            currentTimePhrasesOneOf.Add($"about {n}");
                        }
                        break;
                    case 15:
                        foreach (var n in Noon)
                        {
                            foreach (var qp in QuarterPast)
                            {
                                currentTimePhrasesOneOf.Add($"{qp}{n}");
                            }
                        }
                        break;
                    case 30:
                        foreach (var n in Noon)
                        {
                            foreach (var h in Half)
                            {
                                currentTimePhrasesOneOf.Add($"{h}{n}");
                            }
                        }
                        break;
                }
            }

            if (startOfDay is { Hour: 12, Minute: 0 })
            {
                foreach (var n in Noon)
                {
                    currentTimePhrasesOneOf.Add($"at {n}");
                    currentTimePhrasesOneOf.Add($"12 {n}");
                    currentTimePhrasesOneOf.Add($"12:00 {n}");
                    currentTimePhrasesOneOf.Add($"12.00 {n}");
                    foreach (var s in Struck)
                    {
                        currentTimePhrasesOneOf.Add($"{s}{n}");
                    }
                    foreach (var s in StrokeOf)
                    {
                        currentTimePhrasesOneOf.Add($"{s}{n}");
                    }
                }
            }

            if (startOfDay is { Hour: 12, Minute: > 0 })
            {
                var toMinute = 60 - startOfDay.Minute;
                var toMinuteWords = GetMinuteWords(toMinute);

                foreach (var tmw in toMinuteWords)
                {
                    foreach (var n in Noon)
                    {
                        currentTimePhrasesOneOf.Add($"{tmw} {toMinutePlural} to {n}");
                    }
                }

                switch (startOfDay.Minute)
                {
                    case 45:
                        foreach (var n in Noon)
                        {
                            foreach (var qt in QuarterTo)
                            {
                                currentTimePhrasesOneOf.Add($"{qt}{n}");
                            }
                        }
                        break;
                    case >= 57 and < 60:
                        foreach (var n in Noon)
                        {
                            currentTimePhrasesOneOf.Add($"almost at {n}");
                            currentTimePhrasesOneOf.Add($"nearly {n}");
                            currentTimePhrasesOneOf.Add($"about {n}");
                        }
                        break;
                }
            }

            // Midnight
            if (startOfDay is { Hour: 0, Minute: > 0 })
            {
                var minuteWords = GetMinuteWords(startOfDay.Minute);

                foreach (var mw in minuteWords)
                {
                    currentTimePhrasesOneOf.Add($"{mw} {pastMinutePlural} past midnight");
                    currentTimePhrasesOneOf.Add($"{mw} {pastMinutePlural} after midnight");
                    currentTimePhrasesOneOf.Add($"{mw} past midnight");
                    currentTimePhrasesOneOf.Add($"{mw} after midnight");
                }

                switch (startOfDay.Minute)
                {
                    case < 4:
                        currentTimePhrasesOneOf.Add("a little after midnight");
                        currentTimePhrasesOneOf.Add("just after midnight");
                        currentTimePhrasesOneOf.Add("about midnight");
                        break;
                    case 15:
                        foreach (var qp in QuarterPast)
                        {
                            currentTimePhrasesOneOf.Add($"{qp}midnight");
                        }
                        break;
                    case 30:
                        foreach (var h in Half)
                        {
                            currentTimePhrasesOneOf.Add($"{h}midnight");
                        }
                        break;
                }
            }

            if (startOfDay is { Hour: 0, Minute: 0 })
            {
                currentTimePhrasesOneOf.Add("at midnight");
                currentTimePhrasesOneOf.Add("12 midnight");
                currentTimePhrasesOneOf.Add("12:00 midnight");
                currentTimePhrasesOneOf.Add("12.00 midnight");
                foreach (var s in Struck)
                {
                    currentTimePhrasesOneOf.Add($"{s}midnight");
                }
                foreach (var s in StrokeOf)
                {
                    currentTimePhrasesOneOf.Add($"{s}midnight");
                }
            }

            if (startOfDay is { Hour: 23, Minute: > 0 })
            {
                var toMinute = 60 - startOfDay.Minute;
                var toMinuteWords = GetMinuteWords(toMinute);

                foreach (var tmw in toMinuteWords)
                {
                    currentTimePhrasesOneOf.Add($"{tmw} {toMinutePlural} to midnight");
                }

                switch (startOfDay.Minute)
                {
                    case 45:
                        foreach (var qt in QuarterTo)
                        {
                            currentTimePhrasesOneOf.Add($"{qt}midnight");
                        }
                        break;
                    case >= 57 and < 60:
                        currentTimePhrasesOneOf.Add("almost at midnight");
                        currentTimePhrasesOneOf.Add("nearly midnight");
                        currentTimePhrasesOneOf.Add("about midnight");
                        break;
                }
            }

            currentTimePhrasesSuperGenericOneOf.Add($"{startOfDay:HH:mm}h");
            currentTimePhrasesSuperGenericOneOf.Add($"{startOfDay:HHmm}h");

            var key = startOfDay.ToString("HH:mm", CultureInfo.InvariantCulture);

            currentTimePhrasesOneOf = [.. currentTimePhrasesOneOf.OrderByDescending(s => s.Length)];
            timePhrasesOneOf.TryAdd(key, currentTimePhrasesOneOf);

            currentTimePhrasesGenericOneOf =
            [
                .. currentTimePhrasesGenericOneOf.OrderByDescending(s => s.Length),
            ];
            timePhrasesGenericOneOf.TryAdd(key, currentTimePhrasesGenericOneOf);

            currentTimePhrasesSuperGenericOneOf =
            [
                .. currentTimePhrasesSuperGenericOneOf.OrderByDescending(s => s.Length),
            ];
            timePhrasesSuperGenericOneOf.TryAdd(key, currentTimePhrasesSuperGenericOneOf);

            startOfDay = startOfDay.AddMinutes(1);
        }

        return (
            timePhrasesOneOf.ToImmutableDictionary(),
            timePhrasesGenericOneOf.ToImmutableDictionary(),
            timePhrasesSuperGenericOneOf.ToImmutableDictionary()
        );
    }
}
