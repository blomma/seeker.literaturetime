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
        new()
        {
            $"{timePhrase}am",
            $"{timePhrase}a.m.",
            $"{timePhrase} am",
            $"{timePhrase} a.m.",
            $"{timePhrase}  am",
            $"{timePhrase}  a.m."
        };

    private static List<string> AppendPm(string timePhrase) =>
        new()
        {
            $"{timePhrase}pm",
            $"{timePhrase}p.m.",
            $"{timePhrase} pm",
            $"{timePhrase} p.m.",
            $"{timePhrase}  pm",
            $"{timePhrase}  p.m."
        };

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

                        currentTimePhrasesOneOf.AddRange(AppendAm($"At {hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"At {hour}"));
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
                    currentTimePhrasesOneOf.AddRange(AppendAm($"a little after {hourWord}"));
                    currentTimePhrasesOneOf.AddRange(AppendAm($"a little after {hour}"));

                    currentTimePhrasesOneOf.AddRange(AppendAm($"just after {hourWord}"));
                    currentTimePhrasesOneOf.AddRange(AppendAm($"just after {hour}"));

                    currentTimePhrasesOneOf.AddRange(AppendAm($"about {hourWord}"));
                    currentTimePhrasesOneOf.AddRange(AppendAm($"about {hour}"));
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

                        currentTimePhrasesOneOf.AddRange(AppendPm($"At {hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"At {hour}"));
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
                    currentTimePhrasesOneOf.AddRange(AppendPm($"a little after {hourWord}"));
                    currentTimePhrasesOneOf.AddRange(AppendPm($"a little after {hour}"));

                    currentTimePhrasesOneOf.AddRange(AppendPm($"just after {hourWord}"));
                    currentTimePhrasesOneOf.AddRange(AppendPm($"just after {hour}"));

                    currentTimePhrasesOneOf.AddRange(AppendPm($"about {hourWord}"));
                    currentTimePhrasesOneOf.AddRange(AppendPm($"about {hour}"));
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
                            AppendAm($"a little after {hourWord} o'clock")
                        );
                        currentTimePhrasesOneOf.AddRange(
                            AppendAm($"a little after {hourWord} o’clock")
                        );
                        currentTimePhrasesOneOf.AddRange(
                            AppendAm($"a little after {hour} o'clock")
                        );
                        currentTimePhrasesOneOf.AddRange(
                            AppendAm($"a little after {hour} o’clock")
                        );

                        currentTimePhrasesOneOf.AddRange(
                            AppendAm($"just after {hourWord} o'clock")
                        );
                        currentTimePhrasesOneOf.AddRange(
                            AppendAm($"just after {hourWord} o’clock")
                        );
                        currentTimePhrasesOneOf.AddRange(AppendAm($"just after {hour} o'clock"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"just after {hour} o’clock"));

                        currentTimePhrasesOneOf.AddRange(AppendAm($"about {hourWord} o'clock"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"about {hourWord} o’clock"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"about {hour} o'clock"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"about {hour} o’clock"));
                        break;
                    case 15:
                        currentTimePhrasesOneOf.AddRange(AppendAm($"quarter past {hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"quarter-past {hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"quarter after {hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"quarter-after {hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"quarter past {hour}"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"quarter-past {hour}"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"quarter after {hour}"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"quarter-after {hour}"));
                        break;
                    case 30:
                        currentTimePhrasesOneOf.AddRange(AppendAm($"half past {hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"half-past {hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"half past {hour}"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"half-past {hour}"));
                        break;
                    case 45:
                        currentTimePhrasesOneOf.AddRange(AppendAm($"quarter to {toHourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"quarter-to {toHourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"quarter to {toHour}"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"quarter-to {toHour}"));
                        break;
                    case >= 57
                    and < 60:
                        currentTimePhrasesOneOf.AddRange(
                            AppendAm($"almost at {toHourWord} o'clock")
                        );
                        currentTimePhrasesOneOf.AddRange(
                            AppendAm($"almost at {toHourWord} o’clock")
                        );
                        currentTimePhrasesOneOf.AddRange(AppendAm($"almost at {toHour} o'clock"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"almost at {toHour} o’clock"));

                        currentTimePhrasesOneOf.AddRange(AppendAm($"nearly {toHourWord} o'clock"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"nearly {toHourWord} o’clock"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"nearly {toHour} o'clock"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"nearly {toHour} o’clock"));

                        currentTimePhrasesOneOf.AddRange(AppendAm($"about {toHourWord} o'clock"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"about {toHourWord} o’clock"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"about {toHour} o'clock"));
                        currentTimePhrasesOneOf.AddRange(AppendAm($"about {toHour} o’clock"));
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
                            AppendPm($"a little after {hourWord} o'clock")
                        );
                        currentTimePhrasesOneOf.AddRange(
                            AppendPm($"a little after {hourWord} o’clock")
                        );
                        currentTimePhrasesOneOf.AddRange(
                            AppendPm($"a little after {hour} o'clock")
                        );
                        currentTimePhrasesOneOf.AddRange(
                            AppendPm($"a little after {hour} o’clock")
                        );

                        currentTimePhrasesOneOf.AddRange(
                            AppendPm($"just after {hourWord} o'clock")
                        );
                        currentTimePhrasesOneOf.AddRange(
                            AppendPm($"just after {hourWord} o’clock")
                        );
                        currentTimePhrasesOneOf.AddRange(AppendPm($"just after {hour} o'clock"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"just after {hour} o’clock"));

                        currentTimePhrasesOneOf.AddRange(AppendPm($"about {hourWord} o'clock"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"about {hourWord} o’clock"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"about {hour} o'clock"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"about {hour} o’clock"));
                        break;

                    case 15:
                        currentTimePhrasesOneOf.AddRange(AppendPm($"quarter past {hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"quarter-past {hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"quarter after {hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"quarter-after {hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"quarter past {hour}"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"quarter-past {hour}"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"quarter after {hour}"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"quarter-after {hour}"));
                        break;
                    case 30:
                        currentTimePhrasesOneOf.AddRange(AppendPm($"half past {hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"half-past {hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"half past {hour}"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"half-past {hour}"));
                        break;
                    case 45:
                        currentTimePhrasesOneOf.AddRange(AppendPm($"quarter to {toHourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"quarter-to {toHourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"quarter to {toHour}"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"quarter-to {toHour}"));
                        break;
                    case >= 57
                    and < 60:
                        currentTimePhrasesOneOf.AddRange(
                            AppendPm($"almost at {toHourWord} o'clock")
                        );
                        currentTimePhrasesOneOf.AddRange(
                            AppendPm($"almost at {toHourWord} o’clock")
                        );
                        currentTimePhrasesOneOf.AddRange(AppendPm($"almost at {toHour} o'clock"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"almost at {toHour} o’clock"));

                        currentTimePhrasesOneOf.AddRange(AppendPm($"nearly {toHourWord} o'clock"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"nearly {toHourWord} o’clock"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"nearly {toHour} o'clock"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"nearly {toHour} o’clock"));

                        currentTimePhrasesOneOf.AddRange(AppendPm($"about {toHourWord} o'clock"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"about {toHourWord} o’clock"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"about {toHour} o'clock"));
                        currentTimePhrasesOneOf.AddRange(AppendPm($"about {toHour} o’clock"));
                        break;
                }
            }

            // Generic
            if (startOfDay.Hour is > 0 and <= 23 && startOfDay.Minute > 0)
            {
                var hour = startOfDay.Hour > 12 ? startOfDay.Hour - 12 : startOfDay.Hour;
                var hourWord = NumberToWord[hour];

                var toHour = startOfDay.Hour > 12 ? startOfDay.Hour - 12 + 1 : startOfDay.Hour + 1;
                var toHourWord = NumberToWord[toHour];

                var toMinute = 60 - startOfDay.Minute;
                var toMinuteWord = NumberToWord[toMinute];

                // twelve minute(s) to nineteen
                currentTimePhrasesGenericOneOf.Add(
                    $"{toMinuteWord} {toMinutePlural} to {toHourWord}"
                );
                currentTimePhrasesGenericOneOf.Add($"{toMinute} {toMinutePlural} to {toHour}");

                switch (startOfDay.Minute)
                {
                    case < 4:
                        currentTimePhrasesGenericOneOf.Add($"a little after {hourWord} o'clock");
                        currentTimePhrasesGenericOneOf.Add($"a little after {hourWord} o’clock");
                        currentTimePhrasesGenericOneOf.Add($"a little after {hour} o'clock");
                        currentTimePhrasesGenericOneOf.Add($"a little after {hour} o’clock");

                        currentTimePhrasesGenericOneOf.Add($"just after {hourWord} o'clock");
                        currentTimePhrasesGenericOneOf.Add($"just after {hourWord} o’clock");
                        currentTimePhrasesGenericOneOf.Add($"just after {hour} o'clock");
                        currentTimePhrasesGenericOneOf.Add($"just after {hour} o’clock");

                        currentTimePhrasesGenericOneOf.Add($"about {hourWord} o'clock");
                        currentTimePhrasesGenericOneOf.Add($"about {hourWord} o’clock");
                        currentTimePhrasesGenericOneOf.Add($"about {hour} o'clock");
                        currentTimePhrasesGenericOneOf.Add($"about {hour} o’clock");
                        break;
                    case 15:
                        currentTimePhrasesGenericOneOf.Add($"quarter past {hourWord}");
                        currentTimePhrasesGenericOneOf.Add($"quarter-past {hourWord}");
                        currentTimePhrasesGenericOneOf.Add($"quarter after {hourWord}");
                        currentTimePhrasesGenericOneOf.Add($"quarter-after {hourWord}");
                        currentTimePhrasesGenericOneOf.Add($"quarter past {hour}");
                        currentTimePhrasesGenericOneOf.Add($"quarter-past {hour}");
                        currentTimePhrasesGenericOneOf.Add($"quarter after {hour}");
                        currentTimePhrasesGenericOneOf.Add($"quarter-after {hour}");
                        break;
                    case 30:
                        currentTimePhrasesGenericOneOf.Add($"half past {hourWord}");
                        currentTimePhrasesGenericOneOf.Add($"half-past {hourWord}");
                        currentTimePhrasesGenericOneOf.Add($"half past {hour}");
                        currentTimePhrasesGenericOneOf.Add($"half-past {hour}");
                        break;
                    case 45:
                        currentTimePhrasesGenericOneOf.Add($"quarter to {toHourWord}");
                        currentTimePhrasesGenericOneOf.Add($"quarter-to {toHourWord}");
                        currentTimePhrasesGenericOneOf.Add($"quarter to {toHour}");
                        currentTimePhrasesGenericOneOf.Add($"quarter-to {toHour}");
                        break;
                    case >= 57
                    and < 60:
                        currentTimePhrasesGenericOneOf.Add($"almost at {toHourWord} o'clock");
                        currentTimePhrasesGenericOneOf.Add($"almost at {toHourWord} o’clock");
                        currentTimePhrasesGenericOneOf.Add($"almost at {toHour} o'clock");
                        currentTimePhrasesGenericOneOf.Add($"almost at {toHour} o’clock");

                        currentTimePhrasesGenericOneOf.Add($"nearly {toHourWord} o'clock");
                        currentTimePhrasesGenericOneOf.Add($"nearly {toHourWord} o’clock");
                        currentTimePhrasesGenericOneOf.Add($"nearly {toHour} o'clock");
                        currentTimePhrasesGenericOneOf.Add($"nearly {toHour} o’clock");

                        currentTimePhrasesGenericOneOf.Add($"about {toHourWord} o'clock");
                        currentTimePhrasesGenericOneOf.Add($"about {toHourWord} o’clock");
                        currentTimePhrasesGenericOneOf.Add($"about {toHour} o'clock");
                        currentTimePhrasesGenericOneOf.Add($"about {toHour} o’clock");
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
                        currentTimePhrasesOneOf.Add($"a little after {hourWord} in the morning");
                        currentTimePhrasesOneOf.Add($"a little after {hourWord} in the morn");
                        currentTimePhrasesOneOf.Add($"a little after {hour} in the morning");
                        currentTimePhrasesOneOf.Add($"a little after {hour} in the morn");

                        currentTimePhrasesOneOf.Add($"just after {hourWord} in the morning");
                        currentTimePhrasesOneOf.Add($"just after {hourWord} in the morn");
                        currentTimePhrasesOneOf.Add($"just after {hour} in the morning");
                        currentTimePhrasesOneOf.Add($"just after {hour} in the morn");

                        currentTimePhrasesOneOf.Add($"about {hourWord} in the morning");
                        currentTimePhrasesOneOf.Add($"about {hourWord} in the morn");
                        currentTimePhrasesOneOf.Add($"about {hour} in the morning");
                        currentTimePhrasesOneOf.Add($"about {hour} in the morn");

                        currentTimePhrasesOneOf.Add($"about {hourWord} o’clock in the morning");
                        currentTimePhrasesOneOf.Add($"about {hourWord} o’clock in the morn");
                        currentTimePhrasesOneOf.Add($"about {hour} o’clock in the morning");
                        currentTimePhrasesOneOf.Add($"about {hour} o’clock in the morn");

                        currentTimePhrasesOneOf.Add($"about {hourWord} o'clock in the morning");
                        currentTimePhrasesOneOf.Add($"about {hourWord} o'clock in the morn");
                        currentTimePhrasesOneOf.Add($"about {hour} o'clock in the morning");
                        currentTimePhrasesOneOf.Add($"about {hour} o'clock in the morn");
                        break;
                    case 15:
                        currentTimePhrasesOneOf.Add($"quarter past {hourWord} in the morning");
                        currentTimePhrasesOneOf.Add($"quarter-past {hourWord} in the morning");
                        currentTimePhrasesOneOf.Add($"quarter past {hourWord} in the morn");
                        currentTimePhrasesOneOf.Add($"quarter-past {hourWord} in the morn");
                        currentTimePhrasesOneOf.Add($"quarter past {hour} in the morning");
                        currentTimePhrasesOneOf.Add($"quarter-past {hour} in the morning");
                        currentTimePhrasesOneOf.Add($"quarter past {hour} in the morn");
                        currentTimePhrasesOneOf.Add($"quarter-past {hour} in the morn");
                        break;

                    case 30:
                        currentTimePhrasesOneOf.Add($"half past {hourWord} in the morning");
                        currentTimePhrasesOneOf.Add($"half-past {hourWord} in the morning");
                        currentTimePhrasesOneOf.Add($"half past {hourWord} in the morn");
                        currentTimePhrasesOneOf.Add($"half-past {hourWord} in the morn");
                        currentTimePhrasesOneOf.Add($"half past {hour} in the morning");
                        currentTimePhrasesOneOf.Add($"half-past {hour} in the morning");
                        currentTimePhrasesOneOf.Add($"half past {hour} in the morn");
                        currentTimePhrasesOneOf.Add($"half-past {hour} in the morn");
                        break;

                    case 45:
                        currentTimePhrasesOneOf.Add($"quarter to {toHourWord} in the morning");
                        currentTimePhrasesOneOf.Add($"quarter to {toHourWord} in the morn");
                        currentTimePhrasesOneOf.Add($"quarter-to {toHourWord} in the morning");
                        currentTimePhrasesOneOf.Add($"quarter-to {toHourWord} in the morn");
                        currentTimePhrasesOneOf.Add($"quarter to {toHour} in the morning");
                        currentTimePhrasesOneOf.Add($"quarter to {toHour} in the morn");
                        currentTimePhrasesOneOf.Add($"quarter-to {toHour} in the morning");
                        currentTimePhrasesOneOf.Add($"quarter-to {toHour} in the morn");
                        break;

                    case >= 57
                    and < 60:
                        currentTimePhrasesOneOf.Add($"almost at {toHourWord} in the morning");
                        currentTimePhrasesOneOf.Add($"almost at {toHourWord} in the morn");
                        currentTimePhrasesOneOf.Add($"almost at {toHour} in the morning");
                        currentTimePhrasesOneOf.Add($"almost at {toHour} in the morn");

                        currentTimePhrasesOneOf.Add($"nearly {toHourWord} in the morning");
                        currentTimePhrasesOneOf.Add($"nearly {toHourWord} in the morn");
                        currentTimePhrasesOneOf.Add($"nearly {toHour} in the morning");
                        currentTimePhrasesOneOf.Add($"nearly {toHour} in the morn");

                        currentTimePhrasesOneOf.Add($"about {toHourWord} in the morning");
                        currentTimePhrasesOneOf.Add($"about {toHourWord} in the morn");
                        currentTimePhrasesOneOf.Add($"about {toHour} in the morning");
                        currentTimePhrasesOneOf.Add($"about {toHour} in the morn");

                        currentTimePhrasesOneOf.Add($"about {toHourWord} o’clock in the morning");
                        currentTimePhrasesOneOf.Add($"about {toHourWord} o’clock in the morn");
                        currentTimePhrasesOneOf.Add($"about {toHour} o’clock in the morning");
                        currentTimePhrasesOneOf.Add($"about {toHour} o’clock in the morn");

                        currentTimePhrasesOneOf.Add($"about {toHourWord} o'clock in the morning");
                        currentTimePhrasesOneOf.Add($"about {toHourWord} o'clock in the morn");
                        currentTimePhrasesOneOf.Add($"about {toHour} o'clock in the morning");
                        currentTimePhrasesOneOf.Add($"about {toHour} o'clock in the morn");
                        break;
                }
            }

            if (startOfDay is { Hour: < 12, Minute: 0 })
            {
                var hour = startOfDay.Hour;
                var hourWord = NumberToWord[hour];

                currentTimePhrasesOneOf.Add($"{hourWord} o’clock in the morning");
                currentTimePhrasesOneOf.Add($"{hourWord} o’clock in the morn");
                currentTimePhrasesOneOf.Add($"{hour} o’clock in the morning");
                currentTimePhrasesOneOf.Add($"{hour} o’clock in the morn");

                currentTimePhrasesOneOf.Add($"{hourWord} o'clock in the morning");
                currentTimePhrasesOneOf.Add($"{hourWord} o'clock in the morn");
                currentTimePhrasesOneOf.Add($"{hour} o'clock in the morning");
                currentTimePhrasesOneOf.Add($"{hour} o'clock in the morn");

                currentTimePhrasesOneOf.Add($"{hourWord} in the morning");
                currentTimePhrasesOneOf.Add($"{hourWord} in the morn");
                currentTimePhrasesOneOf.Add($"{hour} in the morning");
                currentTimePhrasesOneOf.Add($"{hour} in the morn");
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
                        currentTimePhrasesOneOf.Add($"a little after {hourWord} at night");
                        currentTimePhrasesOneOf.Add($"a little after {hour} at night");
                        currentTimePhrasesOneOf.Add($"a little after {hourWord} in the evening");
                        currentTimePhrasesOneOf.Add($"a little after {hour} in the evening");

                        currentTimePhrasesOneOf.Add($"just after {hourWord} at night");
                        currentTimePhrasesOneOf.Add($"just after {hour} at night");
                        currentTimePhrasesOneOf.Add($"just after {hourWord} in the evening");
                        currentTimePhrasesOneOf.Add($"just after {hour} in the evening");

                        currentTimePhrasesOneOf.Add($"about {hourWord} at night");
                        currentTimePhrasesOneOf.Add($"about {hour} at night");
                        currentTimePhrasesOneOf.Add($"about {hourWord} in the evening");
                        currentTimePhrasesOneOf.Add($"about {hour} in the evening");

                        currentTimePhrasesOneOf.Add($"about {hourWord} o’clock in the afternoon");
                        currentTimePhrasesOneOf.Add($"about {hourWord} o'clock in the afternoon");
                        currentTimePhrasesOneOf.Add($"about {hour} o’clock in the afternoon");
                        currentTimePhrasesOneOf.Add($"about {hour} o'clock in the afternoon");

                        currentTimePhrasesOneOf.Add($"about {hourWord} o’clock in the evening");
                        currentTimePhrasesOneOf.Add($"about {hourWord} o'clock in the evening");
                        currentTimePhrasesOneOf.Add($"about {hour} o’clock in the evening");
                        currentTimePhrasesOneOf.Add($"about {hour} o'clock in the evening");
                        break;
                    case 15:
                        currentTimePhrasesOneOf.Add($"quarter past {hourWord} in the afternoon");
                        currentTimePhrasesOneOf.Add($"quarter-past {hourWord} in the afternoon");
                        currentTimePhrasesOneOf.Add($"quarter past {hour} in the afternoon");
                        currentTimePhrasesOneOf.Add($"quarter-past {hour} in the afternoon");
                        currentTimePhrasesOneOf.Add($"quarter past {hourWord} in the evening");
                        currentTimePhrasesOneOf.Add($"quarter-past {hourWord} in the evening");
                        currentTimePhrasesOneOf.Add($"quarter past {hour} in the evening");
                        currentTimePhrasesOneOf.Add($"quarter-past {hour} in the evening");
                        break;
                    case 30:
                        currentTimePhrasesOneOf.Add($"half past {hourWord} in the afternoon");
                        currentTimePhrasesOneOf.Add($"half-past {hourWord} in the afternoon");
                        currentTimePhrasesOneOf.Add($"half past {hour} in the afternoon");
                        currentTimePhrasesOneOf.Add($"half-past {hour} in the afternoon");

                        currentTimePhrasesOneOf.Add($"half past {hourWord} in the evening");
                        currentTimePhrasesOneOf.Add($"half-past {hourWord} in the evening");
                        currentTimePhrasesOneOf.Add($"half past {hour} in the evening");
                        currentTimePhrasesOneOf.Add($"half-past {hour} in the evening");
                        break;
                    case 45:
                        currentTimePhrasesOneOf.Add($"quarter to {toHourWord} in the afternoon");
                        currentTimePhrasesOneOf.Add($"quarter to {toHour} in the afternoon");
                        currentTimePhrasesOneOf.Add($"quarter-to {toHourWord} in the afternoon");
                        currentTimePhrasesOneOf.Add($"quarter-to {toHour} in the afternoon");

                        currentTimePhrasesOneOf.Add($"quarter to {toHourWord} in the evening");
                        currentTimePhrasesOneOf.Add($"quarter to {toHour} in the evening");
                        currentTimePhrasesOneOf.Add($"quarter-to {toHourWord} in the evening");
                        currentTimePhrasesOneOf.Add($"quarter-to {toHour} in the evening");
                        break;
                    case >= 57
                    and < 60:
                        currentTimePhrasesOneOf.Add($"almost at {toHourWord} in the afternoon");
                        currentTimePhrasesOneOf.Add($"almost at {toHour} in the afternoon");
                        currentTimePhrasesOneOf.Add($"almost at {toHourWord} in the evening");
                        currentTimePhrasesOneOf.Add($"almost at {toHour} in the evening");

                        currentTimePhrasesOneOf.Add($"nearly {toHourWord} in the afternoon");
                        currentTimePhrasesOneOf.Add($"nearly {toHour} in the afternoon");
                        currentTimePhrasesOneOf.Add($"nearly {toHourWord} in the evening");
                        currentTimePhrasesOneOf.Add($"nearly {toHour} in the evening");

                        currentTimePhrasesOneOf.Add($"about {toHourWord} in the afternoon");
                        currentTimePhrasesOneOf.Add($"about {toHour} in the afternoon");
                        currentTimePhrasesOneOf.Add($"about {toHourWord} in the evening");
                        currentTimePhrasesOneOf.Add($"about {toHour} in the evening");

                        currentTimePhrasesOneOf.Add($"about {toHourWord} o’clock in the afternoon");
                        currentTimePhrasesOneOf.Add($"about {toHourWord} o'clock in the afternoon");
                        currentTimePhrasesOneOf.Add($"about {toHour} o’clock in the afternoon");
                        currentTimePhrasesOneOf.Add($"about {toHour} o'clock in the afternoon");

                        currentTimePhrasesOneOf.Add($"about {toHourWord} o’clock in the evening");
                        currentTimePhrasesOneOf.Add($"about {toHourWord} o'clock in the evening");
                        currentTimePhrasesOneOf.Add($"about {toHour} o’clock in the evening");
                        currentTimePhrasesOneOf.Add($"about {toHour} o'clock in the evening");
                        break;
                }
            }

            if (startOfDay is { Hour: >= 12 and <= 23, Minute: 0 })
            {
                var hour = startOfDay.Hour == 12 ? startOfDay.Hour : startOfDay.Hour - 12;
                var hourWord = NumberToWord[hour];

                currentTimePhrasesOneOf.Add($"{hourWord} o’clock in the afternoon");
                currentTimePhrasesOneOf.Add($"{hourWord} o'clock in the afternoon");
                currentTimePhrasesOneOf.Add($"{hour} o’clock in the afternoon");
                currentTimePhrasesOneOf.Add($"{hour} o'clock in the afternoon");
                currentTimePhrasesOneOf.Add($"{hourWord} in the afternoon");
                currentTimePhrasesOneOf.Add($"{hour} in the afternoon");

                currentTimePhrasesOneOf.Add($"{hourWord} o’clock in the evening");
                currentTimePhrasesOneOf.Add($"{hourWord} o'clock in the evening");
                currentTimePhrasesOneOf.Add($"{hour} o’clock in the evening");
                currentTimePhrasesOneOf.Add($"{hour} o'clock in the evening");
                currentTimePhrasesOneOf.Add($"{hourWord} in the evening");
                currentTimePhrasesOneOf.Add($"{hour} in the evening");
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

            var key = startOfDay.ToString("HH:mm");
            timePhrasesOneOf.TryAdd(key, currentTimePhrasesOneOf);
            timePhrasesGenericOneOf.TryAdd(key, currentTimePhrasesGenericOneOf);
            timePhrasesSuperGenericOneOf.TryAdd(key, currentTimePhrasesSuperGenericOneOf);

            startOfDay = startOfDay.AddMinutes(1);
        }

        return (timePhrasesOneOf, timePhrasesGenericOneOf, timePhrasesSuperGenericOneOf);
    }
}
