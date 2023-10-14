namespace Seeker;

public static class Phrases
{
    private static readonly Dictionary<int, string> numberToWord =
        new()
        {
            { 0, "midnight" },
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

    private static IEnumerable<string> AppendAM(string timePhrase)
    {
        return new List<string>
        {
            $"At {timePhrase}am",
            $"At {timePhrase}a.m.",
            $"At {timePhrase} am",
            $"At {timePhrase} a.m.",
            $"At {timePhrase}  am",
            $"At {timePhrase}  a.m."
        };
    }

    private static IEnumerable<string> AppendPM(string timePhrase)
    {
        return new List<string>
        {
            $"At {timePhrase}pm",
            $"At {timePhrase}p.m.",
            $"At {timePhrase} pm",
            $"At {timePhrase} p.m.",
            $"At {timePhrase}  pm",
            $"At {timePhrase}  p.m."
        };
    }

    public static Dictionary<string, List<string>> GeneratePhrases()
    {
        var timePhrasesOneOf = new Dictionary<string, List<string>>();

        var startOfDay = DateTime.Now.Date;
        var endOfDay = startOfDay.Date.AddDays(1).AddTicks(-1);

        while (startOfDay < endOfDay)
        {
            var hourWord = numberToWord[startOfDay.Hour];
            var minuteWord = numberToWord[startOfDay.Minute];
            var minutePlural = startOfDay.Minute == 1 ? "minute" : "minutes";

            var currentTimePhrasesOneOf = new List<string>();

            // AM
            if (startOfDay.Hour < 12)
            {
                switch (startOfDay.Minute)
                {
                    case 0:
                        currentTimePhrasesOneOf.AddRange(AppendAM($"At {hourWord}"));
                        break;
                    case < 4:
                        currentTimePhrasesOneOf.AddRange(AppendAM($"just after {startOfDay.Hour}"));
                        currentTimePhrasesOneOf.AddRange(AppendAM($"just after {hourWord}"));
                        break;
                }

                currentTimePhrasesOneOf.AddRange(AppendAM($"{startOfDay:h:mm}"));
            }

            if (startOfDay is { Hour: < 12, Minute: > 0 })
            {
                // twelve minute(s) past nineteen
                currentTimePhrasesOneOf.AddRange(
                    AppendAM($"{minuteWord} {minutePlural} past {hourWord}")
                );

                // twelve minute(s) after nineteen
                currentTimePhrasesOneOf.AddRange(
                    AppendAM($"{minuteWord} {minutePlural} after {hourWord}")
                );

                // twelve past nineteen
                currentTimePhrasesOneOf.AddRange(AppendAM($"{minuteWord} past {hourWord}"));

                // twelve after nineteen
                currentTimePhrasesOneOf.AddRange(AppendAM($"{minuteWord} after {hourWord}"));
            }

            // PM
            if (startOfDay.Hour >= 12)
            {
                switch (startOfDay.Minute)
                {
                    case 0:
                        currentTimePhrasesOneOf.AddRange(AppendPM($"At {hourWord}"));
                        break;
                    case < 4:
                        currentTimePhrasesOneOf.AddRange(AppendPM($"just after {startOfDay.Hour}"));
                        currentTimePhrasesOneOf.AddRange(AppendAM($"just after {hourWord}"));
                        break;
                }

                currentTimePhrasesOneOf.AddRange(AppendPM($"{startOfDay:h:mm}"));
            }

            if (startOfDay is { Hour: >= 12, Minute: > 0 })
            {
                // twelve minute(s) past nineteen
                currentTimePhrasesOneOf.AddRange(
                    AppendPM($"{minuteWord} {minutePlural} past {hourWord}")
                );

                // twelve minute(s) after nineteen
                currentTimePhrasesOneOf.AddRange(
                    AppendPM($"{minuteWord} {minutePlural} after {hourWord}")
                );

                // twelve past nineteen
                currentTimePhrasesOneOf.AddRange(AppendPM($"{minuteWord} past {hourWord}"));

                // twelve after nineteen
                currentTimePhrasesOneOf.AddRange(AppendPM($"{minuteWord} after {hourWord}"));
            }

            // Generic
            // if (startOfDay.Minute > 0)
            // {
            //     // twelve minute(s) past nineteen
            //     currentTimePhrasesOneOf.Add($"{minuteWord} {minutePlural} past {hourWord}");

            //     // twelve minute(s) after nineteen
            //     currentTimePhrasesOneOf.Add($"{minuteWord} {minutePlural} after {hourWord}");

            //     // twelve past nineteen
            //     currentTimePhrasesOneOf.Add($"{minuteWord} past {hourWord}");

            //     // twelve after nineteen
            //     currentTimePhrasesOneOf.Add($"{minuteWord} after {hourWord}");
            // }

            // AM
            if (startOfDay is { Hour: < 12, Minute: > 0 })
            {
                var toHourWord = numberToWord[startOfDay.Hour + 1];
                var toMinute = 60 - startOfDay.Minute;
                var toMinuteWord = numberToWord[toMinute];
                var nearlyMinute = 60 - startOfDay.Minute;

                // twelve minute(s) to nineteen
                currentTimePhrasesOneOf.AddRange(
                    AppendAM($"{toMinuteWord} {minutePlural} to {toHourWord}")
                );

                switch (startOfDay.Minute)
                {
                    case 15:
                        currentTimePhrasesOneOf.AddRange(AppendAM($"quarter past {hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendAM($"quarter-past {hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendAM($"quarter after {hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendAM($"quarter-after {hourWord}"));
                        break;
                    case 30:
                        currentTimePhrasesOneOf.AddRange(AppendAM($"half past {hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendAM($"half-past {hourWord}"));
                        break;
                    case 45:
                        currentTimePhrasesOneOf.AddRange(AppendAM($"quarter to {toHourWord}"));
                        break;
                }

                if (nearlyMinute < 4)
                {
                    currentTimePhrasesOneOf.AddRange(AppendAM($"nearly {toHourWord} o'clock"));
                    currentTimePhrasesOneOf.AddRange(AppendAM($"nearly {toHourWord} o’clock"));

                    currentTimePhrasesOneOf.AddRange(AppendAM($"about {toHourWord} o'clock"));
                    currentTimePhrasesOneOf.AddRange(AppendAM($"about {toHourWord} o’clock"));
                }
                else if (startOfDay.Minute < 4)
                {
                    currentTimePhrasesOneOf.AddRange(AppendAM($"about {toHourWord} o'clock"));
                    currentTimePhrasesOneOf.AddRange(AppendAM($"about {toHourWord} o’clock"));
                }
            }

            // PM
            if (startOfDay.Hour is >= 12 and <= 22 && startOfDay.Minute > 0)
            {
                var toHourWord = numberToWord[startOfDay.Hour + 1];
                var toMinute = 60 - startOfDay.Minute;
                var toMinuteWord = numberToWord[toMinute];
                var nearlyMinute = 60 - startOfDay.Minute;

                // twelve minute(s) to nineteen
                currentTimePhrasesOneOf.AddRange(
                    AppendPM($"{toMinuteWord} {minutePlural} to {toHourWord}")
                );

                switch (startOfDay.Minute)
                {
                    case 15:
                        currentTimePhrasesOneOf.AddRange(AppendPM($"quarter past {hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendPM($"quarter-past {hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendPM($"quarter after {hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendPM($"quarter-after {hourWord}"));
                        break;
                    case 30:
                        currentTimePhrasesOneOf.AddRange(AppendPM($"half past {hourWord}"));
                        currentTimePhrasesOneOf.AddRange(AppendPM($"half-past {hourWord}"));
                        break;
                    case 45:
                        currentTimePhrasesOneOf.AddRange(AppendPM($"quarter to {toHourWord}"));
                        break;
                }

                if (nearlyMinute < 4)
                {
                    currentTimePhrasesOneOf.AddRange(AppendPM($"nearly {toHourWord} o'clock"));
                    currentTimePhrasesOneOf.AddRange(AppendPM($"nearly {toHourWord} o’clock"));

                    currentTimePhrasesOneOf.AddRange(AppendPM($"about {toHourWord} o'clock"));
                    currentTimePhrasesOneOf.AddRange(AppendPM($"about {toHourWord} o’clock"));
                }
                else if (startOfDay.Minute < 4)
                {
                    currentTimePhrasesOneOf.AddRange(AppendPM($"about {toHourWord} o'clock"));
                    currentTimePhrasesOneOf.AddRange(AppendPM($"about {toHourWord} o’clock"));
                }
            }

            // Generic
            // if (startOfDay.Minute > 0)
            // {
            //     var toHourWord = numberToWord[startOfDay.Hour + 1];
            //     var toMinute = 60 - startOfDay.Minute;
            //     var toMinuteWord = numberToWord[toMinute];
            //     var nearlyMinute = 60 - startOfDay.Minute;

            //     // twelve minute(s) to nineteen
            //     currentTimePhrasesOneOf.Add($"{toMinuteWord} {minutePlural} to {toHourWord}");

            //     if (startOfDay.Minute == 15)
            //     {
            //         currentTimePhrasesOneOf.Add($"quarter past {hourWord}");
            //         currentTimePhrasesOneOf.Add($"quarter-past {hourWord}");
            //         currentTimePhrasesOneOf.Add($"quarter after {hourWord}");
            //         currentTimePhrasesOneOf.Add($"quarter-after {hourWord}");
            //     }

            //     if (startOfDay.Minute == 30)
            //     {
            //         currentTimePhrasesOneOf.Add($"half past {hourWord}");
            //         currentTimePhrasesOneOf.Add($"half-past {hourWord}");
            //     }

            //     if (startOfDay.Minute == 45)
            //     {
            //         currentTimePhrasesOneOf.Add($"quarter to {toHourWord}");
            //     }

            //     if (nearlyMinute < 4)
            //     {
            //         currentTimePhrasesOneOf.Add($"nearly {toHourWord} o'clock");
            //         currentTimePhrasesOneOf.Add($"nearly {toHourWord} o’clock");

            //         currentTimePhrasesOneOf.Add($"about {toHourWord} o'clock");
            //         currentTimePhrasesOneOf.Add($"about {toHourWord} o’clock");
            //     }
            //     else if (startOfDay.Minute < 4)
            //     {
            //         currentTimePhrasesOneOf.Add($"about {toHourWord} o'clock");
            //         currentTimePhrasesOneOf.Add($"about {toHourWord} o’clock");
            //     }
            // }

            // AM
            if (startOfDay is { Hour: < 12, Minute: 0 })
            {
                currentTimePhrasesOneOf.AddRange(AppendAM($"{hourWord} o'clock"));
                currentTimePhrasesOneOf.AddRange(AppendAM($"{hourWord} o’clock"));

                currentTimePhrasesOneOf.AddRange(AppendAM($"It struck {hourWord}"));
            }

            // PM
            if (startOfDay is { Hour: >= 12, Minute: 0 })
            {
                currentTimePhrasesOneOf.AddRange(AppendPM($"{hourWord} o'clock"));
                currentTimePhrasesOneOf.AddRange(AppendPM($"{hourWord} o’clock"));

                currentTimePhrasesOneOf.AddRange(AppendPM($"It struck {hourWord}"));
            }

            // AM
            if (startOfDay is { Hour: < 12, Minute: > 57 })
            {
                var toHourWord = numberToWord[startOfDay.Hour + 1];
                currentTimePhrasesOneOf.Add($"almost at {toHourWord} in the morning");
            }

            if (startOfDay is { Hour: < 12, Minute: 0 })
            {
                currentTimePhrasesOneOf.Add($"at {hourWord} o’clock in the morning");
                currentTimePhrasesOneOf.Add($"{hourWord} o’clock in the morning");
                currentTimePhrasesOneOf.Add($"{hourWord} o’clock in the morn");

                currentTimePhrasesOneOf.Add($"at {hourWord} o'clock in the morning");
                currentTimePhrasesOneOf.Add($"{hourWord} o'clock in the morning");
                currentTimePhrasesOneOf.Add($"{hourWord} o'clock in the morn");

                currentTimePhrasesOneOf.Add($"at {hourWord} in the morning");
                currentTimePhrasesOneOf.Add($"{hourWord} in the morning");
                currentTimePhrasesOneOf.Add($"{hourWord} in the morn");
            }

            // PM
            if (startOfDay is { Hour: >= 12, Minute: > 57 })
            {
                var toHourWord = numberToWord[startOfDay.Hour + 1];
                currentTimePhrasesOneOf.Add($"almost at {toHourWord} in the afternoon");
            }

            if (startOfDay is { Hour: >= 12, Minute: 0 })
            {
                currentTimePhrasesOneOf.Add($"at {hourWord} o’clock in the afternoon");
                currentTimePhrasesOneOf.Add($"{hourWord} o’clock in the afternoon");

                currentTimePhrasesOneOf.Add($"at {hourWord} o'clock in the afternoon");
                currentTimePhrasesOneOf.Add($"{hourWord} o'clock in the afternoon");

                currentTimePhrasesOneOf.Add($"at {hourWord} in the afternoon");
                currentTimePhrasesOneOf.Add($"{hourWord} in the afternoon");
            }

            // Generic
            // if (startOfDay.Minute == 0)
            // {
            //     currentTimePhrasesOneOf.Add($"{hourWord} o'clock");
            //     currentTimePhrasesOneOf.Add($"{hourWord} o’clock");

            //     currentTimePhrasesOneOf.Add($"It struck {hourWord}");
            // }

            // currentTimePhrasesOneOf.Add(startOfDay.ToString("HH:mm"));

            var key = startOfDay.ToString("HH:mm");
            timePhrasesOneOf.TryAdd(key, currentTimePhrasesOneOf);

            startOfDay = startOfDay.AddMinutes(1);
        }

        return timePhrasesOneOf;
    }
}
