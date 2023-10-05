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

    public static (
        Dictionary<string, List<string>>,
        Dictionary<string, List<string>>
    ) GeneratePhrases()
    {
        var timePhrases = new Dictionary<string, List<string>>();
        var timePhrasesOneOf = new Dictionary<string, List<string>>();

        var startOfDay = DateTime.Now.Date;
        var endOfDay = startOfDay.Date.AddDays(1).AddTicks(-1);

        while (startOfDay < endOfDay)
        {
            var currentTimePhrasesOneOf = new List<string>();
            if (startOfDay.Hour < 13)
            {
                currentTimePhrasesOneOf.Add($"{startOfDay:h:mm}am");
                currentTimePhrasesOneOf.Add($"{startOfDay:h:mm}a.m.");
                currentTimePhrasesOneOf.Add($"{startOfDay:h:mm} am");
                currentTimePhrasesOneOf.Add($"{startOfDay:h:mm} a.m.");
                currentTimePhrasesOneOf.Add($"{startOfDay:h:mm}  am");
                currentTimePhrasesOneOf.Add($"{startOfDay:h:mm}  a.m.");
            }
            else
            {
                currentTimePhrasesOneOf.Add($"{startOfDay:h:mm}pm");
                currentTimePhrasesOneOf.Add($"{startOfDay:h:mm}p.m.");
                currentTimePhrasesOneOf.Add($"{startOfDay:h:mm} pm");
                currentTimePhrasesOneOf.Add($"{startOfDay:h:mm} p.m.");
                currentTimePhrasesOneOf.Add($"{startOfDay:h:mm}  pm");
                currentTimePhrasesOneOf.Add($"{startOfDay:h:mm}  p.m.");
            }
            // currentTimePhrasesOneOf.Add($"{startOfDay:h:mm}");

            var currentTimePhrases = new List<string>();
            currentTimePhrases.Add(startOfDay.ToString("HH:mm"));

            var hourWord = numberToWord[startOfDay.Hour];
            if (startOfDay.Minute > 0)
            {
                var minuteWord = numberToWord[startOfDay.Minute];

                var minutePlural = startOfDay.Minute == 1 ? "minute" : "minutes";

                // PAST AFTER
                // twelve minute(s) past nineteen
                currentTimePhrases.Add($"{minuteWord} {minutePlural} past {hourWord}");

                // twelve minute(s) after nineteen
                currentTimePhrases.Add($"{minuteWord} {minutePlural} after {hourWord}");

                // 12 minute(s) past nineteen
                // currentTimePhrases.Add($"{startOfDay.Minute} {minutePlural} past {hourWord}");

                // 12 minute(s) after nineteen
                // currentTimePhrases.Add($"{startOfDay.Minute} {minutePlural} after {hourWord}");

                // twelve minute(s) past 19
                // currentTimePhrases.Add($"{minuteWord} {minutePlural} past {startOfDay.Hour}");

                // twelve minute(s) after 19
                // currentTimePhrases.Add($"{minuteWord} {minutePlural} after {startOfDay.Hour}");

                // 12 minute(s) past 19
                // currentTimePhrases.Add(
                //     $"{startOfDay.Minute} {minutePlural} past {startOfDay.Hour}"
                // );

                // 12 minute(s) after 19
                // currentTimePhrases.Add(
                //     $"{startOfDay.Minute} {minutePlural} after {startOfDay.Hour}"
                // );

                // twelve past nineteen
                currentTimePhrases.Add($"{minuteWord} past {hourWord}");

                // twelve after nineteen
                currentTimePhrases.Add($"{minuteWord} after {hourWord}");

                // 12 past nineteen
                // currentTimePhrases.Add($"{startOfDay.Minute} past {hourWord}");

                // 12 after nineteen
                // currentTimePhrases.Add($"{startOfDay.Minute} after {hourWord}");

                // twelve past 19
                // currentTimePhrases.Add($"{minuteWord} past {startOfDay.Hour}");

                // twelve after 19
                // currentTimePhrases.Add($"{minuteWord} after {startOfDay.Hour}");

                // 12 past 19
                // currentTimePhrases.Add($"{startOfDay.Minute} past {startOfDay.Hour}");

                // 12 after 19
                // currentTimePhrases.Add($"{startOfDay.Minute} after {startOfDay.Hour}");

                // TO
                if (startOfDay.Hour < 22)
                {
                    var toHourWord = numberToWord[startOfDay.Hour + 1];
                    var toMinute = 60 - startOfDay.Minute;
                    var toMinuteWord = numberToWord[toMinute];

                    // twelve minute(s) to nineteen
                    currentTimePhrases.Add($"{toMinuteWord} {minutePlural} to {toHourWord}");

                    // 12 minute(s) to nineteen
                    // currentTimePhrases.Add($"{toMinute} {minutePlural} to {toHourWord}");

                    // twelve minute(s) to 19
                    // currentTimePhrases.Add(
                    //     $"{toMinuteWord} {minutePlural} to {startOfDay.Hour + 1}"
                    // );

                    // 12 minute(s) to 19
                    // currentTimePhrases.Add($"{toMinute} {minutePlural} to {startOfDay.Hour + 1}");

                    // twelve to nineteen
                    currentTimePhrases.Add($"{toMinuteWord} to {toHourWord}");

                    // 12 to nineteen
                    // currentTimePhrases.Add($"{toMinute} to {toHourWord}");

                    // twelve to 19
                    // currentTimePhrases.Add($"{toMinuteWord} to {startOfDay.Hour + 1}");

                    // 12 to 19
                    // currentTimePhrases.Add($"{toMinute} to {startOfDay.Hour + 1}");

                    if (startOfDay.Minute == 45)
                    {
                        currentTimePhrases.Add($"quarter to {toHourWord}");
                    }
                }

                if (startOfDay.Minute == 15)
                {
                    currentTimePhrases.Add($"quarter past {hourWord}");
                    currentTimePhrases.Add($"quarter-past {hourWord}");
                    currentTimePhrases.Add($"quarter after {hourWord}");
                    currentTimePhrases.Add($"quarter-after {hourWord}");
                }

                if (startOfDay.Minute == 30)
                {
                    currentTimePhrases.Add($"half past {hourWord}");
                    currentTimePhrases.Add($"half-past {hourWord}");
                }
            }
            else
            {
                currentTimePhrases.Add($"{hourWord} o'clock");
                currentTimePhrases.Add($"{hourWord} o’clock");
            }

            var key = startOfDay.ToString("HH:mm");
            if (!timePhrases.ContainsKey(key))
            {
                timePhrases.Add(key, currentTimePhrases);
                timePhrasesOneOf.Add(key, currentTimePhrasesOneOf);
            }

            startOfDay = startOfDay.AddMinutes(1);
        }

        return (timePhrases, timePhrasesOneOf);
    }
}
