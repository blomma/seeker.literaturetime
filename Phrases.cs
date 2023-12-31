using System.Globalization;

namespace Seeker;

public static class Phrases
{
    private static readonly Dictionary<int, string> NumberToWord =
        new()
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

    private static List<string> AppendAm(string timePhrase) =>
        [
            $"{timePhrase}am",
            $"{timePhrase}a.m.",
            $"{timePhrase} am",
            $"{timePhrase} a.m.",
            $"{timePhrase}  am",
            $"{timePhrase}  a.m."
        ];

    private static List<string> AppendPm(string timePhrase) =>
        [
            $"{timePhrase}pm",
            $"{timePhrase}p.m.",
            $"{timePhrase} pm",
            $"{timePhrase} p.m.",
            $"{timePhrase}  pm",
            $"{timePhrase}  p.m."
        ];

    private static List<string> Am => ["am", "a.m.", " am", " a.m.", "  am", "  a.m."];

    private static List<string> Pm => ["pm", "p.m.", " pm", " p.m.", "  pm", "  p.m."];

    private static List<string> Combine(
        List<string> listA,
        List<string> listB,
        List<string> listC,
        List<string> listD,
        List<string> listE
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
                        foreach (var e in listE)
                        {
                            var r = $"{a}{b}{c}{d}{e}";
                            result.Add(r);
                        }
                    }
                }
            }
        }

        return result;
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
        Dictionary<string, List<string>>,
        Dictionary<string, List<string>>,
        Dictionary<string, List<string>>
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
                switch (startOfDay.Minute)
                {
                    case 0:
                        var hour = startOfDay.Hour;
                        var hourWord = NumberToWord[hour];

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ["At "],
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                Am
                            )
                        );
                        break;
                }

                currentTimePhrasesOneOf.AddRange(AppendAm($"{startOfDay:h:mm}"));
            }

            if (startOfDay is { Hour: < 12, Minute: > 0 })
            {
                var hour = startOfDay.Hour;
                var hourWord = NumberToWord[hour];

                var minute = startOfDay.Minute;
                var minuteWord = NumberToWord[minute];

                // twelve minute(s) past nineteen
                currentTimePhrasesOneOf.AddRange(
                    AppendAm($"{minuteWord} {pastMinutePlural} past {hourWord}")
                );
                currentTimePhrasesOneOf.AddRange(
                    AppendAm($"{minute} {pastMinutePlural} past {hour}")
                );

                // twelve minute(s) after nineteen
                currentTimePhrasesOneOf.AddRange(
                    AppendAm($"{minuteWord} {pastMinutePlural} after {hourWord}")
                );
                currentTimePhrasesOneOf.AddRange(
                    AppendAm($"{minute} {pastMinutePlural} after {hour}")
                );

                // twelve past nineteen
                currentTimePhrasesOneOf.AddRange(AppendAm($"{minuteWord} past {hourWord}"));
                currentTimePhrasesOneOf.AddRange(AppendAm($"{minute} past {hour}"));

                // twelve after nineteen
                currentTimePhrasesOneOf.AddRange(AppendAm($"{minuteWord} after {hourWord}"));
                currentTimePhrasesOneOf.AddRange(AppendAm($"{minute} after {hour}"));

                if (startOfDay.Minute < 4)
                {
                    currentTimePhrasesOneOf.AddRange(
                        Combine(
                            ["a little after ", "just after ", "about "],
                            [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                            Am
                        )
                    );
                }
            }

            // PM
            if (startOfDay is { Hour: >= 12 and <= 23 })
            {
                switch (startOfDay.Minute)
                {
                    case 0:
                        var hour = startOfDay.Hour == 12 ? startOfDay.Hour : startOfDay.Hour - 12;
                        var hourWord = NumberToWord[hour];

                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ["At "],
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                Pm
                            )
                        );
                        break;
                }

                currentTimePhrasesOneOf.AddRange(AppendPm($"{startOfDay:h:mm}"));
            }

            if (startOfDay is { Hour: >= 12 and <= 23, Minute: > 0 })
            {
                var hour = startOfDay.Hour == 12 ? startOfDay.Hour : startOfDay.Hour - 12;
                var hourWord = NumberToWord[hour];

                var minute = startOfDay.Minute;
                var minuteWord = NumberToWord[minute];

                // twelve minute(s) past nineteen
                currentTimePhrasesOneOf.AddRange(
                    AppendPm($"{minuteWord} {pastMinutePlural} past {hourWord}")
                );
                currentTimePhrasesOneOf.AddRange(
                    AppendPm($"{minute} {pastMinutePlural} past {hour}")
                );

                // twelve minute(s) after nineteen
                currentTimePhrasesOneOf.AddRange(
                    AppendPm($"{minuteWord} {pastMinutePlural} after {hourWord}")
                );
                currentTimePhrasesOneOf.AddRange(
                    AppendPm($"{minute} {pastMinutePlural} after {hour}")
                );

                // twelve past nineteen
                currentTimePhrasesOneOf.AddRange(AppendPm($"{minuteWord} past {hourWord}"));
                currentTimePhrasesOneOf.AddRange(AppendPm($"{minute} past {hour}"));

                // twelve after nineteen
                currentTimePhrasesOneOf.AddRange(AppendPm($"{minuteWord} after {hourWord}"));
                currentTimePhrasesOneOf.AddRange(AppendPm($"{minute} after {hour}"));

                if (startOfDay.Minute < 4)
                {
                    currentTimePhrasesOneOf.AddRange(
                        Combine(
                            ["a little after ", "just after ", "about "],
                            [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                            Pm
                        )
                    );
                }
            }

            // AM
            if (startOfDay is { Hour: < 12, Minute: > 0 })
            {
                var hour = startOfDay.Hour;
                var hourWord = NumberToWord[hour];

                var toHour = hour + 1;
                var toHourWord = NumberToWord[toHour];
                var toMinute = 60 - startOfDay.Minute;
                var toMinuteWord = NumberToWord[toMinute];

                // twelve minute(s) to nineteen
                currentTimePhrasesOneOf.AddRange(
                    AppendAm($"{toMinuteWord} {toMinutePlural} to {toHourWord}")
                );
                currentTimePhrasesOneOf.AddRange(
                    AppendAm($"{toMinute} {toMinutePlural} to {toHour}")
                );

                switch (startOfDay.Minute)
                {
                    case < 4:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ["a little after ", "just after ", "about "],
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                [" o'clock", " o’clock"],
                                Am
                            )
                        );
                        break;
                    case 15:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                [
                                    "quarter past ",
                                    "quarter-past ",
                                    "quarter after ",
                                    "quarter-after "
                                ],
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                Am
                            )
                        );
                        break;
                    case 30:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ["half past ", "half-past "],
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                Am
                            )
                        );
                        break;
                    case 45:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ["quarter to ", "quarter-to "],
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                Am
                            )
                        );
                        break;
                    case >= 57
                    and < 60:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ["almost at ", "nearly ", "about "],
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                [" o'clock", " o’clock"],
                                Am
                            )
                        );
                        break;
                }
            }

            // PM
            if (startOfDay is { Hour: >= 12 and <= 23, Minute: > 0 })
            {
                var hour = startOfDay.Hour == 12 ? startOfDay.Hour : startOfDay.Hour - 12;
                var hourWord = NumberToWord[hour];

                var toHour = hour + 1;
                var toHourWord = NumberToWord[toHour];

                var toMinute = 60 - startOfDay.Minute;
                var toMinuteWord = NumberToWord[toMinute];

                // twelve minute(s) to nineteen
                currentTimePhrasesOneOf.AddRange(
                    AppendPm($"{toMinuteWord} {toMinutePlural} to {toHourWord}")
                );
                currentTimePhrasesOneOf.AddRange(
                    AppendPm($"{toMinute} {toMinutePlural} to {toHour}")
                );

                switch (startOfDay.Minute)
                {
                    case < 4:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ["a little after ", "just after ", "about "],
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                [" o'clock", " o’clock"],
                                Pm
                            )
                        );
                        break;

                    case 15:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                [
                                    "quarter past ",
                                    "quarter-past ",
                                    "quarter after ",
                                    "quarter-after "
                                ],
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                Pm
                            )
                        );
                        break;
                    case 30:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ["half past ", "half-past "],
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                Pm
                            )
                        );
                        break;
                    case 45:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ["quarter to ", "quarter-to "],
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                Pm
                            )
                        );
                        break;
                    case >= 57
                    and < 60:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ["almost at ", "nearly ", "about "],
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                [" o'clock", " o’clock"],
                                Pm
                            )
                        );
                        break;
                }
            }

            // Generic
            if (startOfDay.Hour is >= 0 and <= 23 && startOfDay.Minute > 0)
            {
                var hour = startOfDay.Hour > 12 ? startOfDay.Hour - 12 : startOfDay.Hour;
                var hourWord = NumberToWord[hour];

                var toHour = startOfDay.Hour > 12 ? startOfDay.Hour - 12 + 1 : startOfDay.Hour + 1;
                var toHourWord = NumberToWord[toHour];

                var minute = startOfDay.Minute;
                var minuteWord = NumberToWord[minute];

                var toMinute = 60 - startOfDay.Minute;
                var toMinuteWord = NumberToWord[toMinute];

                currentTimePhrasesGenericOneOf.Add(
                    $"{minuteWord} {pastMinutePlural} past {hourWord}"
                );
                currentTimePhrasesGenericOneOf.Add($"{minute} {pastMinutePlural} past {hour}");

                currentTimePhrasesGenericOneOf.Add(
                    $"{minuteWord} {pastMinutePlural} after {hourWord}"
                );
                currentTimePhrasesGenericOneOf.Add($"{minute} {pastMinutePlural} after {hour}");

                // currentTimePhrasesGenericOneOf.Add($"{minuteWord} past {hourWord}");
                // currentTimePhrasesGenericOneOf.Add($"{minute} past {hour}");

                // currentTimePhrasesGenericOneOf.Add($"{minuteWord} after {hourWord}");
                // currentTimePhrasesGenericOneOf.Add($"{minute} after {hour}");

                currentTimePhrasesGenericOneOf.Add(
                    $"{toMinuteWord} {toMinutePlural} to {toHourWord}"
                );
                currentTimePhrasesGenericOneOf.Add($"{toMinute} {toMinutePlural} to {toHour}");

                switch (startOfDay.Minute)
                {
                    case < 4:
                        currentTimePhrasesGenericOneOf.AddRange(
                            Combine(
                                ["a little after ", "just after ", "about "],
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                [" o'clock", " o’clock"]
                            )
                        );
                        break;
                    case 15:
                        currentTimePhrasesGenericOneOf.AddRange(
                            Combine(
                                [
                                    "quarter past ",
                                    "quarter-past ",
                                    "quarter after ",
                                    "quarter-after "
                                ],
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)]
                            )
                        );
                        break;
                    case 30:
                        currentTimePhrasesGenericOneOf.AddRange(
                            Combine(
                                ["half past ", "half-past "],
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)]
                            )
                        );
                        break;
                    case 45:
                        currentTimePhrasesGenericOneOf.AddRange(
                            Combine(
                                ["quarter to ", "quarter-to "],
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)]
                            )
                        );
                        break;
                    case >= 57
                    and < 60:
                        currentTimePhrasesGenericOneOf.AddRange(
                            Combine(
                                ["almost at ", "nearly ", "about "],
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                [" o'clock", " o’clock"]
                            )
                        );
                        break;
                }
            }

            // AM
            if (startOfDay is { Hour: < 12, Minute: 0 })
            {
                var hour = startOfDay.Hour;
                var hourWord = NumberToWord[hour];

                currentTimePhrasesOneOf.AddRange(AppendAm($"{hourWord} o'clock"));
                currentTimePhrasesOneOf.AddRange(AppendAm($"{hourWord} o’clock"));
                currentTimePhrasesOneOf.AddRange(AppendAm($"{hour} o'clock"));
                currentTimePhrasesOneOf.AddRange(AppendAm($"{hour} o’clock"));

                currentTimePhrasesOneOf.AddRange(AppendAm($"It struck {hourWord}"));
                currentTimePhrasesOneOf.AddRange(AppendAm($"It struck {hour}"));
                currentTimePhrasesOneOf.AddRange(AppendAm($"stroke of {hourWord}"));
                currentTimePhrasesOneOf.AddRange(AppendAm($"stroke of {hour}"));
            }

            // PM
            if (startOfDay is { Hour: >= 12 and <= 23, Minute: 0 })
            {
                var hour = startOfDay.Hour == 12 ? startOfDay.Hour : startOfDay.Hour - 12;
                var hourWord = NumberToWord[hour];

                currentTimePhrasesOneOf.AddRange(AppendPm($"{hourWord} o'clock"));
                currentTimePhrasesOneOf.AddRange(AppendPm($"{hourWord} o’clock"));
                currentTimePhrasesOneOf.AddRange(AppendPm($"{hour} o'clock"));
                currentTimePhrasesOneOf.AddRange(AppendPm($"{hour} o’clock"));

                currentTimePhrasesOneOf.AddRange(AppendPm($"It struck {hourWord}"));
                currentTimePhrasesOneOf.AddRange(AppendPm($"It struck {hour}"));
                currentTimePhrasesOneOf.AddRange(AppendPm($"stroke of {hourWord}"));
                currentTimePhrasesOneOf.AddRange(AppendPm($"stroke of {hour}"));
            }

            // AM
            if (startOfDay is { Hour: < 12, Minute: > 0 })
            {
                var hour = startOfDay.Hour;
                var hourWord = NumberToWord[hour];

                var toHour = startOfDay.Hour + 1;
                var toHourWord = NumberToWord[toHour];

                switch (startOfDay.Minute)
                {
                    case < 4:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ["a little after ", "just after ", "about "],
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                [
                                    " in the morning",
                                    " in the morn",
                                    " that morning",
                                    " that morn",
                                    " o’clock in the morning",
                                    " o’clock in the morn",
                                    " o'clock in the morning",
                                    " o'clock in the morn",
                                    " o’clock that morning",
                                    " o’clock that morn",
                                    " o'clock that morning",
                                    " o'clock that morn"
                                ]
                            )
                        );
                        break;
                    case 15:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ["quarter past ", "quarter-past "],
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                [
                                    " in the morning",
                                    " in the morn",
                                    " that morning",
                                    " that morn",
                                    " o’clock in the morning",
                                    " o’clock in the morn",
                                    " o'clock in the morning",
                                    " o'clock in the morn",
                                    " o’clock that morning",
                                    " o’clock that morn",
                                    " o'clock that morning",
                                    " o'clock that morn"
                                ]
                            )
                        );
                        break;

                    case 30:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ["half past ", "half-past "],
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                [
                                    " in the morning",
                                    " in the morn",
                                    " that morning",
                                    " that morn",
                                    " o’clock in the morning",
                                    " o’clock in the morn",
                                    " o'clock in the morning",
                                    " o'clock in the morn",
                                    " o’clock that morning",
                                    " o’clock that morn",
                                    " o'clock that morning",
                                    " o'clock that morn"
                                ]
                            )
                        );
                        break;

                    case 45:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ["quarter to ", "quarter-to "],
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                [
                                    " in the morning",
                                    " in the morn",
                                    " that morning",
                                    " that morn",
                                    " o’clock in the morning",
                                    " o’clock in the morn",
                                    " o'clock in the morning",
                                    " o'clock in the morn",
                                    " o’clock that morning",
                                    " o’clock that morn",
                                    " o'clock that morning",
                                    " o'clock that morn"
                                ]
                            )
                        );
                        break;

                    case >= 57
                    and < 60:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ["almost at ", "nearly ", "about "],
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                [
                                    " in the morning",
                                    " in the morn",
                                    " that morning",
                                    " that morn",
                                    " o’clock in the morning",
                                    " o’clock in the morn",
                                    " o'clock in the morning",
                                    " o'clock in the morn",
                                    " o’clock that morning",
                                    " o’clock that morn",
                                    " o'clock that morning",
                                    " o'clock that morn"
                                ]
                            )
                        );
                        break;
                }
            }

            if (startOfDay is { Hour: < 12, Minute: 0 })
            {
                var hour = startOfDay.Hour;
                var hourWord = NumberToWord[hour];

                currentTimePhrasesOneOf.AddRange(
                    Combine(
                        [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                        [
                            " in the morning",
                            " in the morn",
                            " that morning",
                            " that morn",
                            " o’clock in the morning",
                            " o’clock in the morn",
                            " o'clock in the morning",
                            " o'clock in the morn",
                            " o’clock that morning",
                            " o’clock that morn",
                            " o'clock that morning",
                            " o'clock that morn"
                        ]
                    )
                );
            }

            // PM
            if (startOfDay is { Hour: >= 12 and <= 23, Minute: > 0 })
            {
                var hour = startOfDay.Hour == 12 ? startOfDay.Hour : startOfDay.Hour - 12;
                var hourWord = NumberToWord[hour];

                var toHour = hour + 1;
                var toHourWord = NumberToWord[toHour];

                switch (startOfDay.Minute)
                {
                    case < 4:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ["a little after ", "just after ", "about "],
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                [
                                    " in the afternoon",
                                    " that afternoon",
                                    " in the evening",
                                    " that evening",
                                    " at night",
                                    " that night",
                                    " o’clock in the afternoon",
                                    " o'clock in the afternoon",
                                    " o’clock that afternoon",
                                    " o'clock that afternoon",
                                    " o’clock in the evening",
                                    " o'clock in the evening",
                                    " o’clock that evening",
                                    " o'clock that evening",
                                    " o’clock at night",
                                    " o'clock that night",
                                    " o’clock at night",
                                    " o'clock that night"
                                ]
                            )
                        );
                        break;
                    case 15:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ["quarter past ", "quarter-past "],
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                [
                                    " in the afternoon",
                                    " that afternoon",
                                    " in the evening",
                                    " that evening",
                                    " at night",
                                    " that night",
                                    " o’clock in the afternoon",
                                    " o'clock in the afternoon",
                                    " o’clock that afternoon",
                                    " o'clock that afternoon",
                                    " o’clock in the evening",
                                    " o'clock in the evening",
                                    " o’clock that evening",
                                    " o'clock that evening",
                                    " o’clock at night",
                                    " o'clock that night",
                                    " o’clock at night",
                                    " o'clock that night"
                                ]
                            )
                        );
                        break;
                    case 30:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ["half past ", "half-past "],
                                [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                                [
                                    " in the afternoon",
                                    " that afternoon",
                                    " in the evening",
                                    " that evening",
                                    " at night",
                                    " that night",
                                    " o’clock in the afternoon",
                                    " o'clock in the afternoon",
                                    " o’clock that afternoon",
                                    " o'clock that afternoon",
                                    " o’clock in the evening",
                                    " o'clock in the evening",
                                    " o’clock that evening",
                                    " o'clock that evening",
                                    " o’clock at night",
                                    " o'clock that night",
                                    " o’clock at night",
                                    " o'clock that night"
                                ]
                            )
                        );
                        break;
                    case 45:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ["quarter to ", "quarter-to "],
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                [
                                    " in the afternoon",
                                    " that afternoon",
                                    " in the evening",
                                    " that evening",
                                    " at night",
                                    " that night",
                                    " o’clock in the afternoon",
                                    " o'clock in the afternoon",
                                    " o’clock that afternoon",
                                    " o'clock that afternoon",
                                    " o’clock in the evening",
                                    " o'clock in the evening",
                                    " o’clock that evening",
                                    " o'clock that evening",
                                    " o’clock at night",
                                    " o'clock that night",
                                    " o’clock at night",
                                    " o'clock that night"
                                ]
                            )
                        );
                        break;
                    case >= 57
                    and < 60:
                        currentTimePhrasesOneOf.AddRange(
                            Combine(
                                ["almost at ", "nearly ", "about "],
                                [toHourWord, toHour.ToString(CultureInfo.InvariantCulture)],
                                [
                                    " in the afternoon",
                                    " that afternoon",
                                    " in the evening",
                                    " that evening",
                                    " at night",
                                    " that night",
                                    " o’clock in the afternoon",
                                    " o'clock in the afternoon",
                                    " o’clock that afternoon",
                                    " o'clock that afternoon",
                                    " o’clock in the evening",
                                    " o'clock in the evening",
                                    " o’clock that evening",
                                    " o'clock that evening",
                                    " o’clock at night",
                                    " o'clock that night",
                                    " o’clock at night",
                                    " o'clock that night"
                                ]
                            )
                        );
                        break;
                }
            }

            if (startOfDay is { Hour: >= 12 and <= 23, Minute: 0 })
            {
                var hour = startOfDay.Hour == 12 ? startOfDay.Hour : startOfDay.Hour - 12;
                var hourWord = NumberToWord[hour];

                currentTimePhrasesOneOf.AddRange(
                    Combine(
                        [hourWord, hour.ToString(CultureInfo.InvariantCulture)],
                        [
                            " in the afternoon",
                            " that afternoon",
                            " in the evening",
                            " that evening",
                            " at night",
                            " that night",
                            " o’clock in the afternoon",
                            " o'clock in the afternoon",
                            " o’clock that afternoon",
                            " o'clock that afternoon",
                            " o’clock in the evening",
                            " o'clock in the evening",
                            " o’clock that evening",
                            " o'clock that evening",
                            " o’clock at night",
                            " o'clock that night",
                            " o’clock at night",
                            " o'clock that night"
                        ]
                    )
                );
            }

            // Midnight
            if (startOfDay is { Hour: 0, Minute: > 0 })
            {
                var minuteWord = NumberToWord[startOfDay.Minute];

                // twelve minute(s) past nineteen
                currentTimePhrasesOneOf.Add($"{minuteWord} {pastMinutePlural} past midnight");

                // twelve minute(s) after nineteen
                currentTimePhrasesOneOf.Add($"{minuteWord} {pastMinutePlural} after midnight");

                // twelve past nineteen
                currentTimePhrasesOneOf.Add($"{minuteWord} past midnight");

                // twelve after nineteen
                currentTimePhrasesOneOf.Add($"{minuteWord} after midnight");

                switch (startOfDay.Minute)
                {
                    case < 4:
                        currentTimePhrasesOneOf.Add("a little after midnight");
                        currentTimePhrasesOneOf.Add("just after midnight");
                        currentTimePhrasesOneOf.Add("about midnight");
                        break;
                    case 15:
                        currentTimePhrasesOneOf.Add("quarter past midnight");
                        currentTimePhrasesOneOf.Add("quarter-past midnight");
                        currentTimePhrasesOneOf.Add("quarter after midnight");
                        currentTimePhrasesOneOf.Add("quarter-after midnight");
                        break;
                    case 30:
                        currentTimePhrasesOneOf.Add("half past midnight");
                        currentTimePhrasesOneOf.Add("half-past midnight");
                        break;
                }
            }

            if (startOfDay is { Hour: 0, Minute: 0 })
            {
                currentTimePhrasesOneOf.Add("At midnight");
                currentTimePhrasesOneOf.Add("It struck midnight");
                currentTimePhrasesOneOf.Add("stroke of midnight");
            }

            // Midnight
            if (startOfDay is { Hour: 23, Minute: > 0 })
            {
                var toMinute = 60 - startOfDay.Minute;
                var toMinuteWord = NumberToWord[toMinute];

                currentTimePhrasesOneOf.Add($"{toMinuteWord} {toMinutePlural} to midnight");

                switch (startOfDay.Minute)
                {
                    case 45:
                        currentTimePhrasesOneOf.Add("quarter to midnight");
                        currentTimePhrasesOneOf.Add("quarter-to midnight");
                        break;
                    case >= 57
                    and < 60:
                        currentTimePhrasesOneOf.Add($"almost at midnight");
                        currentTimePhrasesOneOf.Add($"nearly midnight");
                        currentTimePhrasesOneOf.Add($"about midnight");
                        break;
                }
            }

            currentTimePhrasesSuperGenericOneOf.Add($"{startOfDay:HH:mm}h");
            currentTimePhrasesSuperGenericOneOf.Add($"{startOfDay:HH:mm}");

            var key = startOfDay.ToString("HH:mm", CultureInfo.InvariantCulture);
            timePhrasesOneOf.TryAdd(key, currentTimePhrasesOneOf);
            timePhrasesGenericOneOf.TryAdd(key, currentTimePhrasesGenericOneOf);
            timePhrasesSuperGenericOneOf.TryAdd(key, currentTimePhrasesSuperGenericOneOf);

            startOfDay = startOfDay.AddMinutes(1);
        }

        return (timePhrasesOneOf, timePhrasesGenericOneOf, timePhrasesSuperGenericOneOf);
    }
}
