using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Seeker;

var jsonSerializerOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
};

const string outputDirectory = "../quotes.literaturetime.temp";

const string gutPath = "/Users/blomma/Downloads/gutenberg";

Directory.CreateDirectory(outputDirectory);

var files = Directory.EnumerateFiles(gutPath, "*.txt", SearchOption.AllDirectories).ToList();
var (timePhrasesOneOf, timePhrasesGenericOneOf, timePhrasesSuperGenericOneOf) =
    Phrases.GeneratePhrases();

var timePhrasesOneOfJson = JsonSerializer.Serialize(timePhrasesOneOf, jsonSerializerOptions);
File.WriteAllText($"{outputDirectory}/timePhrasesOneOf.json", timePhrasesOneOfJson);

var timePhrasesGenericOneOfJson = JsonSerializer.Serialize(
    timePhrasesGenericOneOf,
    jsonSerializerOptions
);
File.WriteAllText($"{outputDirectory}/timePhrasesGenericOneOf.json", timePhrasesGenericOneOfJson);

var timePhrasesSuperGenericOneOfJson = JsonSerializer.Serialize(
    timePhrasesSuperGenericOneOf,
    jsonSerializerOptions
);
File.WriteAllText(
    $"{outputDirectory}/timePhrasesSuperGenericOneOf.json",
    timePhrasesSuperGenericOneOfJson
);

var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 3 };

var titlesExclusion = new List<string>
{
    "A Bible Hand-Book",
    "A Bible School Manual: Studies in the Book of Revelation",
    "A Biblical and Theological Dictionary",
    "A Bird's-Eye View of the Bible",
    "A Brief Account of the Rise and Progress of the People Called Quakers",
    "A Brief Bible History: A Survey of the Old and New Testaments",
    "A Brief Commentary on the Apocalypse",
    "A Class-Book of Biblical History and Geography",
    "A Class-Book of New Testament History",
    "A Class-Book of Old Testament History",
    "A Coal From The Altar, To Kindle The Holy Fire of Zeale",
    "A Discourse for the Time, delivered January 4, 1852 in the First Congregational Unitarian Church",
    "A Farewell Sermon",
    "A Farmer's Wife: The Story of Ruth",
    "A Few Remarks on the Scripture History of Saul and the Witch of Endor",
    "A Greek Primer: For Beginners in New Testament Greek",
    "A Greek-English Lexicon to the New Testament",
    "A Handful of Stars: Texts That Have Moved Great Minds",
    "A Harmony of the Gospels for Students of the Life of Christ",
    "A History of American Christianity",
    "A History of the Moravian Church",
    "A Legacy to the Friends of Free Discussion",
    "A letter to a country clergyman, occasioned by his address to Lord Teignmouth",
    "A Letter to the Right Hon. Lord Bexley",
    "A Little Catechism; With Little Verses and Little Sayings for Little Children",
    "A Plain Introduction to the Criticism of the New Testament, Vol. I.",
    "A Plain Introduction to the Criticism of the New Testament, Vol. II.",
    "A Practical Discourse on Some Principles of Hymn-Singing",
    "A Ribband of Blue, and Other Bible Studies",
    "A sermon preach'd before the Right Honourable the Lord-Mayor : the aldermen and citizens of London",
    "A Sermon preached at Christ Church, Kensington, on May 1, 1859",
    "A Sermon Preached at the Quaker's Meeting House, in Gracechurch-Street, London, Eighth Month 12th, 1694.",
    "A Sermon Preached in York Minister, on St. Bartholomew's Day, Friday, August 24, 1877",
    "A Sermon Preached on the Anniversary of the Boston Female Asylum for Destitute Orphans, September 25, 1835",
    "A Short Method of Prayer",
    "A Translation of the New Testament from the original Greek",
    "A Treatise on Good Works",
    "A True Interpretation of the Witch of Endor",
    "A Voice from the Fire",
    "A Voice of Warning",
    "A Young Folks' History of the Church of Jesus Christ of Latter-day Saints",
    "About The Holy Bible: A Lecture",
    "Addresses on the Revised Version of Holy Scripture",
    "Aids to Reflection; and, The Confessions of an Inquiring Spirit",
    "All Four Gospels for Readers",
    "All Saints' Day and Other Sermons",
    "An Address to Lord Teignmouth, president of the British and Foreign Bible Society, occasioned by his address to the clergy of the Church of England",
    "An Amicable Controversy with a Jewish Rabbi, on The Messiah's Coming",
    "An Appeal to the People in Behalf of Their Rights as Authorized Interpreters of the Bible",
    "An Examination of the Testimony of the Four Evangelists, by the Rules of Evidence Administered in Courts of Justice",
    "An Exhortation to Peace and Unity",
    "An Explanation of Luther's Small Catechism",
    "An Exposition of the Last Psalme",
    "An Open Letter on Translating",
    "Archæology and the Bible",
    "As Others Saw Him: A Retrospect, A.D. 54",
    "Ben-Hur: A tale of the Christ",
    "Beside the Still Waters",
    "Best Stories from the Best Book: An Illustrated Bible Companion for the Home",
    "Bibelen, Det nye Testamente",
    "Bibeln, Gamla och Nya Testamentet",
    "Bible Animals;",
    "Bible Atlas: A Manual of Biblical Geography and History",
    "Bible Characters",
    "Bible Emblems",
    "Bible history and brief outline of church history",
    "Bible Myths and their Parallels in other Religions",
    "Bible Pictures and Stories in Large Print",
    "Bible Readings for the Home Circle",
    "Bible Romances, First Series",
    "Bible Stories and Pictures. From the Old and New Testaments",
    "Bible Stories and Religious Classics",
    "Bible Stories",
    "Bible Studies: Essays on Phallic Worship and Other Curious Rites and Customs",
    "Bible-Burning",
    "Biblia Sacra Vulgata - Psalmi XXII",
    "Biblical Extracts; Or, The Holy Scriptures Analyzed;",
    "Biblical Geography and History",
    "Biblical Revision, its duties and conditions",
    "Biblical Revision",
    "Biology versus Theology. The Bible: irreconcilable with Science, Experience, and even its own statements",
    "Book of Esther",
    "Book of James",
    "Book of Jude",
    "Book of Judith",
    "Book of Philemon",
    "Buried Cities and Bible Countries",
    "Captivating Bible Stories for Young People, Written in Simple Language",
    "Catholic Problems in Western Canada",
    "Chapters of Bible Study",
    "Child's Story of the Bible",
    "Children of the Old Testament",
    "Children's Edition of Touching Incidents and Remarkable Answers to Prayer",
    "Chimes of Mission Bells; an historical sketch of California and her missions",
    "Christ Going Up to Heaven",
    "Christ in the Storm",
    "Christ: The Way, the Truth, and the Life",
    "Christian Literature",
    "Christianity and Islam",
    "Christology of the Old Testament: And a Commentary on the Messianic Predictions, Vol. 1",
    "Christology of the Old Testament: And a Commentary on the Messianic Predictions. Vol. 2",
    "Church Reform",
    "Codex Junius 11",
    "Commentary on Genesis, Vol. 1: Luther on the Creation",
    "Commentary on Genesis, Vol. 2: Luther on Sin and the Flood",
    "Commentary on the Epistle to the Galatians",
    "Companion to the Bible",
    "Concerning Christian Liberty; with Letter of Martin Luther to Pope Leo X.",
    "Confessions of an Inquiring Spirit and Some Miscellaneous Pieces",
    "Consecrated Womanhood",
    "Cousin Hatty's Hymns and Twilight Stories",
    "David the Shepherd Boy",
    "Deaconesses in Europe and their Lessons for America",
    "Dead Men Tell Tales",
    "Det Gamle Testamente af 1931",
    "Deuterocanonical Books of the Bible",
    "Discipline and Other Sermons",
    "Disputation of Doctor Martin Luther on the Power and Efficacy of Indulgences",
    "Divine Songs",
    "Doctrina Christiana",
    "Dr. Martin Luther's Deutsche Geistliche Lieder",
    "Ecclesiastes",
    "Episcopal Fidelity",
    "Epistle of Jude",
    "Epistle Sermons, Vol. 2: Epiphany, Easter and Pentecost",
    "Epistle Sermons, Vol. 3: Trinity Sunday to Advent",
    "Eve's Diary, Complete",
    "Eve's Diary, Part 1",
    "Eve's Diary, Part 2",
    "Eve's Diary, Part 3",
    "Eve's Diary",
    "Evolution",
    "Experimental Investigation of the Spirit Manifestations",
    "Expositions of Holy Scripture : St. Matthew Chaps. IX to XXVIII",
    "Expositions of Holy Scripture: Genesis, Exodus, Leviticus and Numbers",
    "Expositions of Holy Scripture: Isaiah and Jeremiah",
    "Expositions of Holy Scripture: Psalms",
    "Expositions of Holy Scripture: Romans Corinthians (To II Corinthians, Chap. V)",
    "Expositions of Holy Scripture: St. John Chaps. XV to XXI",
    "Expositions of Holy Scripture: St. John Chapters I to XIV",
    "Expositions of Holy Scripture: St. Luke",
    "Expositions of Holy Scripture: St. Mark",
    "Expositions of Holy Scripture: the Acts",
    "Expositions of Holy Scripture",
    "Expositor's Bible: The Book of Ecclesiastes",
    "Expositor's Bible: The Book of Jeremiah, Chapters XXI.-LII.",
    "Expositor's Bible: The Book of Job",
    "Expositor's Bible: The Epistles of St. John",
    "Expositor's Bible: The Gospel of Matthew",
    "Expositor's Bible: The Gospel of St Luke",
    "Extracts from Adam's Diary, translated from the original ms.",
    "Extracts from Adam's Diary",
    "Fair to Look Upon",
    "Fanny, the Flower-Girl; or, Honesty Rewarded. To Which are Added Other Tales",
    "Female Scripture Biography, Volume I",
    "Female Scripture Biography, Volume II",
    "Five Sermons",
    "Five Young Men: Messages of Yesterday for the Young Men of To-day",
    "Four Psalms XXIII. XXXVI. LII. CXXI.",
    "Fresh Light from the Ancient Monuments",
    "Fugitive Slave Law",
    "Galatians",
    "Geology and Revelation",
    "George Borrow",
    "Grace Abounding to the Chief of Sinners",
    "Half Hours in Bible Lands, Volume 2",
    "Hasisadra's Adventure",
    "Hebrew Heroes: A Tale Founded on Jewish History",
    "Hebrew Life and Times",
    "Herein is Love",
    "Heretics",
    "Heroes of Israel",
    "His Glorious Appearing: An Exposition of Matthew Twenty-Four",
    "History and Ecclesiastical Relations of the Churches of the Presbyterial Order at Amoy, China",
    "History of the Catholic Church from the Renaissance to the French Revolution — Volume 1",
    "History of the Catholic Church from the Renaissance to the French Revolution — Volume 2",
    "History of the missions of the American Board Of Commissioners For Foreign Missions to the oriental churches, Volume I.",
    "History of the missions of the American Board Of Commissioners For Foreign Missions to the oriental churches, Volume II.",
    "History of the transmission of ancient books to modern times",
    "Hosanna",
    "How the Bible was Invented",
    "How to become like Christ",
    "How to Live a Holy Life",
    "How to Master the English Bible",
    "How to Teach Religion",
    "Human Nature, and Other Sermons",
    "Hurlbut's Bible Lessons for Boys and Girls",
    "Hymni ecclesiae",
    "Hymns and Spiritual Songs",
    "Hymns for Christian Devotion",
    "Hymns from the East",
    "Hymns of the Greek Church",
    "Hymns, Songs, and Fables, for Young People",
    "Illustrations of The Book of Job",
    "In Arapahoe: Matthew 9, 1-8.",
    "In Naaman's House",
    "Inspiration and Interpretation",
    "Inspiration: Its Nature and Extent",
    "Introduction to Robert Browning",
    "Introduction to the Old Testament",
    "Is the Bible Indictable?",
    "Israël en Égypte: Étude sur un oratorio de G.F. Hændel",
    "Jeremiah : Being The Baird Lecture for 1922",
    "Jesus the Christ",
    "Jesus, The Messiah; or, the Old Testament Prophecies Fulfilled in the New Testament Scriptures, by a Lady",
    "Job and Solomon: Or, The Wisdom of the Old Testament",
    "John the Baptist",
    "Joseph and His Brethren",
    "Joseph Smith as Scientist: A Contribution to Mormon Philosophy",
    "Joseph Smith the Prophet-Teacher: A Discourse",
    "Joseph the Dreamer",
    "Judith, a Play in Three Acts; Founded on the Apocryphal Book of Judith",
    "Key to the Science of Theology",
    "Kingless Folk, and Other Addresses on Bible Animals",
    "Latin Vulgate, Bible Book Titles and Names",
    "Latin Vulgate, Daniel: Prophetia Danielis",
    "Latin Vulgate, Esther: Liber Esther",
    "Lectures on Bible Revision",
    "Lectures on Evolution",
    "Legends of Babylon and Egypt in Relation to Hebrew Tradition",
    "Legends of Old Testament characters, from the Talmud and other sources",
    "Legends of the Patriarchs and Prophets",
    "Letters of George Borrow to the British and Foreign Bible Society",
    "Life of Heber C. Kimball, an Apostle",
    "Light On the Child's Path",
    "Light, Life, and Love: Selections from the German Mystics of the Middle Ages",
    "Little Folded Hands",
    "Little Frida: A Tale of the Black Forest",
    "Little Gidding and its inmates in the Time of King Charles I.",
    "Lourdes",
    "Love to the Uttermost",
    "Luther's Little Instruction Book: The Small Catechism of Martin Luther",
    "Manual of the Mother Church",
    "Marriage with a deceased wife's sister",
    "Martin Luther's Large Catechism, translated by Bente and Dau",
    "Mary Magdalen: A Chronicle",
    "Matthew",
    "Medica Sacra",
    "Men of the Bible; Some Lesser-Known Characters",
    "Men of the Bible",
    "Messages from the Epistle to the Hebrews",
    "Miscellaneous Pieces",
    "Miscellaneous Writings, 1883-1896",
    "Misread Passages of Scriptures",
    "Moses and Aaron: Civil and Ecclesiastical Rites, Used by the Ancient Hebrews",
    "Moses, not Darwin",
    "Mother Stories from the New Testament",
    "Mother Stories from the Old Testament",
    "Mr. Gladstone and Genesis",
    "New Tabernacle Sermons",
    "No and Yes",
    "Not Paul, But Jesus",
    "Notable Women of Olden Time",
    "Notes on the Apocalypse",
    "Notes on the Book of Deuteronomy, Volume I",
    "Notes on the Book of Deuteronomy, Volume II",
    "Notes on the book of Exodus",
    "Notes on the Book of Genesis",
    "Notes on the Book of Leviticus",
    "Notes on the New Testament, Explanatory and Practical: Revelation",
    "Obil, Keeper of Camels",
    "Observations on an Anonymous Pamphlet, Which Has Been Distributed in Lowestoft, and Its Neighbourhood, Entitled Reasons Why a Churchman May with Great Justice Refuse to Subscribe to the British and Foreign Bible Society",
    "Observations upon the Prophecies of Daniel, and the Apocalypse of St. John",
    "Old Groans and New Songs",
    "Omphalos: An Attempt to Untie the Geological Knot",
    "On Singing and Music",
    "On the Method of Zadig",
    "Orthodoxy",
    "Outline Studies in the New Testament for Bible Teachers",
    "Outline Studies in the Old Testament for Bible Teachers",
    "Paradise Lost",
    "Paradise Regained",
    "Parochial and Plain Sermons, Vol. VII (of 8)",
    "Parochial and Plain Sermons, Vol. VIII (of 8)",
    "Paula the Waldensian",
    "Philippian Studies",
    "Pleasure & Profit in Bible Study",
    "Poems",
    "Prayer and praying men",
    "Prayers Written At Vailima, and A Lowden Sabbath Morn",
    "Precepts in Practice; or, Stories Illustrating the Proverbs",
    "Prolegomena to the History of Israel",
    "Prospects of the Church of England",
    "Psalms - Selections from the World English Bible Translation",
    "Pulpit and Press (6th Edition)",
    "Pulpit and Press",
    "Quiet Talks about Jesus",
    "Quiet Talks on John's Gospel",
    "Quiet Talks on Prayer",
    "Quiet Talks on Service",
    "Quiet Talks on the Crowned Christ of Revelation",
    "Reasons why a Churchman may with Great Justice Refuse to Subscribe to the British and Foreign Bible Society",
    "Reina Valera New Testament of the Bible 1602, Book of Matthew",
    "Reina Valera New Testament of the Bible 1858",
    "Reina Valera New Testament of the Bible 1862",
    "Reina Valera New Testament of the Bible 1865",
    "Reina Valera New Testament of the Bible 1909",
    "Religion and Theology: A Sermon for the Times",
    "Report of Commemorative Services with the Sermons and Addresses at the Seabury Centenary, 1883-1885.",
    "Report of the Cromer Ladies' Bible Association, 1838",
    "Rescue the Perishing: Personal Work Made Easy",
    "Retrospection and Introspection",
    "Rome and Turkey in Connexion with the Second Advent",
    "Rome, Turkey and Jerusalem",
    "Rome, Turkey, and Jerusalem",
    "Rudimental Divine Science",
    "Science and Health With Key to The Scriptures",
    "Science and Health, with Key to the Scriptures",
    "Scripture Histories; from the Creation of the World, to the Death of Jesus Christ",
    "Select Masterpieces of Biblical Literature",
    "Selections from the Table Talk of Martin Luther",
    "Separation and Service; or, Thoughts on Numbers VI, VII.",
    "Serious Hours of a Young Lady",
    "Sermons at Rugby",
    "Sermons for the Times",
    "Sermons on Biblical Characters",
    "Sermons on Evil-Speaking",
    "Sermons on National Subjects",
    "Sermons on the Card, and Other Discourses",
    "Sermons on Various Important Subjects",
    "Sermons Preached at Brighton",
    "Sermons to the Natural Man",
    "Shorter Bible Plays",
    "Some Remains (hitherto unpublished) of Joseph Butler, LL.D.",
    "Sources of the Synoptic Gospels",
    "Spiritual Torrents",
    "St. Paul's Epistle to the Ephesians: A Practical Exposition",
    "St. Paul's Epistle to the Romans: A Practical Exposition. Vol. I",
    "St. Paul's Epistle to the Romans: A Practical Exposition. Vol. II",
    "St. Paul's Epistles to the Colossians and Philemon",
    "Stories from the olden time: Teacher's text book, course IV, part I",
    "Stories of the Bible, Volume 1: The People of the Chosen Land",
    "Stories of the Prophets (Before the Exile)",
    "Stories of the Wars of the Jews",
    "Story of the Bible Animals",
    "Strong Souls",
    "Studies in Old Testament History",
    "Studies in Prophecy",
    "Studies in the Epistle of James",
    "Studies in the Scriptures, Volume 7: The Finished Mystery",
    "Studies in Zechariah",
    "Substance of a Sermon on the Bible Society",
    "Succession in the Presidency of The Church of Jesus Christ of Latter-Day Saints",
    "Summa Theologica, Part I (Prima Pars)",
    "Summa Theologica, Part I-II (Pars Prima Secundae)",
    "Summa Theologica, Part II-II (Secunda Secundae)",
    "Summa Theologica, Part III (Tertia Pars)",
    "Supernatural Religion, Vol. 1 (of 3)",
    "Supernatural Religion, Vol. 2 (of 3)",
    "Supernatural Religion, Vol. 3 (of 3)",
    "The Adopted Son: The Story of Moses",
    "The American Woman's Home",
    "The Analogy of Religion to the Constitution and Course of Nature",
    "The Ancient Church: Its History, Doctrine, Worship, and Constitution",
    "The Angels' Song",
    "The Antiquities of the Jews",
    "The Apology of the Augsburg Confession",
    "The Apology of the Church of England",
    "The Assyrian and Hebrew Hymns of Praise",
    "The Astronomy of the Bible",
    "The Autobiography of Methuselah",
    "The Babe in the Bulrushes",
    "The Baptism of the Prince: A Sermon",
    "The Believer Not Ashamed of the Gospel",
    "The Bible and Life",
    "The Bible and Polygamy: Does the Bible Sanction Polygamy?",
    "The Bible Book by Book",
    "The Bible for Young People",
    "The Bible in its Making: The most Wonderful Book in the World",
    "The Bible Period by Period",
    "The Bible Story",
    "The Bible Unveiled",
    "The Bible, Douay-Rheims, Book 01: Genesis",
    "The Bible, Douay-Rheims, Book 02: Exodus",
    "The Bible, Douay-Rheims, Book 03: Leviticus",
    "The Bible, Douay-Rheims, Book 04: Numbers",
    "The Bible, Douay-Rheims, Book 05: Deuteronomy",
    "The Bible, Douay-Rheims, Book 06: Josue",
    "The Bible, Douay-Rheims, Book 07: Judges",
    "The Bible, Douay-Rheims, Book 08: Ruth",
    "The Bible, Douay-Rheims, Book 09: 1 Kings",
    "The Bible, Douay-Rheims, Book 10: 2 Kings",
    "The Bible, Douay-Rheims, Book 11: 3 Kings",
    "The Bible, Douay-Rheims, Book 12: 4 Kings",
    "The Bible, Douay-Rheims, Book 13: 1 Paralipomenon",
    "The Bible, Douay-Rheims, Book 14: 2 Paralipomenon",
    "The Bible, Douay-Rheims, Book 15: 1 Esdras",
    "The Bible, Douay-Rheims, Book 16: 2 Esdras",
    "The Bible, Douay-Rheims, Book 17: Tobias",
    "The Bible, Douay-Rheims, Book 18: Judith",
    "The Bible, Douay-Rheims, Book 19: Esther",
    "The Bible, Douay-Rheims, Book 20: Job",
    "The Bible, Douay-Rheims, Book 21: Psalms",
    "The Bible, Douay-Rheims, Book 22: Proverbs",
    "The Bible, Douay-Rheims, Book 23: Ecclesiastes",
    "The Bible, Douay-Rheims, Book 24: Canticle of Canticles",
    "The Bible, Douay-Rheims, Book 25: Wisdom",
    "The Bible, Douay-Rheims, Book 26: Ecclesiasticus",
    "The Bible, Douay-Rheims, Book 27: Isaias",
    "The Bible, Douay-Rheims, Book 28: Jeremias",
    "The Bible, Douay-Rheims, Book 29: Lamentations of Jeremias",
    "The Bible, Douay-Rheims, Book 30: Baruch",
    "The Bible, Douay-Rheims, Book 31: Ezechiel",
    "The Bible, Douay-Rheims, Book 32: Daniel",
    "The Bible, Douay-Rheims, Book 33: Osee",
    "The Bible, Douay-Rheims, Book 34: Joel",
    "The Bible, Douay-Rheims, Book 35: Amos",
    "The Bible, Douay-Rheims, Book 36: Abdias",
    "The Bible, Douay-Rheims, Book 37: Jonas",
    "The Bible, Douay-Rheims, Book 38: Micheas",
    "The Bible, Douay-Rheims, Book 39: Nahum",
    "The Bible, Douay-Rheims, Book 40: Habacuc",
    "The Bible, Douay-Rheims, Book 41: Sophonias",
    "The Bible, Douay-Rheims, Book 42: Aggeus",
    "The Bible, Douay-Rheims, Book 43: Zacharias",
    "The Bible, Douay-Rheims, Book 44: Malachias",
    "The Bible, Douay-Rheims, Book 45: 1 Machabees",
    "The Bible, Douay-Rheims, Book 46: 2 Machabees",
    "The Bible, Douay-Rheims, Book 47: Matthew",
    "The Bible, Douay-Rheims, Book 48: Mark",
    "The Bible, Douay-Rheims, Book 49: Luke",
    "The Bible, Douay-Rheims, Book 50: John",
    "The Bible, Douay-Rheims, Book 51: Acts",
    "The Bible, Douay-Rheims, Book 52: Romans",
    "The Bible, Douay-Rheims, Book 53: 1 Corinthians",
    "The Bible, Douay-Rheims, Book 54: 2 Corinthians",
    "The Bible, Douay-Rheims, Book 55: Galatians",
    "The Bible, Douay-Rheims, Book 56: Ephesians",
    "The Bible, Douay-Rheims, Book 57: Philippians",
    "The Bible, Douay-Rheims, Book 58: Colossians",
    "The Bible, Douay-Rheims, Book 59: 1 Thessalonians",
    "The Bible, Douay-Rheims, Book 60: 2 Thessalonians",
    "The Bible, Douay-Rheims, Book 61: 1 Timothy",
    "The Bible, Douay-Rheims, Book 62: 2 Timothy",
    "The Bible, Douay-Rheims, Book 63: Titus",
    "The Bible, Douay-Rheims, Book 64: Philemon",
    "The Bible, Douay-Rheims, Book 65: Hebrews",
    "The Bible, Douay-Rheims, Book 66: James",
    "The Bible, Douay-Rheims, Book 67: 1 Peter",
    "The Bible, Douay-Rheims, Book 68: 2 Peter",
    "The Bible, Douay-Rheims, Book 69: 1 John",
    "The Bible, Douay-Rheims, Book 70: 2 John",
    "The Bible, Douay-Rheims, Book 71: 3 John",
    "The Bible, Douay-Rheims, Book 72: Jude",
    "The Bible, Douay-Rheims, Book 73: Apocalypse",
    "The Bible, Douay-Rheims, Complete",
    "The Bible, Douay-Rheims, New Testament",
    "The Bible, Douay-Rheims, Old Testament — Part 1",
    "The Bible, Douay-Rheims, Old Testament — Part 2",
    "The Bible, Douay-Rheims, Old Testament--Part 2",
    "The Bible, Douay-Rheims, Old Testament--Part I",
    "The Bible, King James version, Book 1: Genesis",
    "The Bible, King James version, Book 10: 2 Samuel",
    "The Bible, King James version, Book 11: 1 Kings",
    "The Bible, King James version, Book 12: 2 Kings",
    "The Bible, King James version, Book 13: 1 Chronicles",
    "The Bible, King James version, Book 14: 2 Chronicles",
    "The Bible, King James version, Book 15: Ezra",
    "The Bible, King James version, Book 16: Nehemiah",
    "The Bible, King James version, Book 17: Esther",
    "The Bible, King James version, Book 18: Job",
    "The Bible, King James version, Book 19: Psalms",
    "The Bible, King James version, Book 2: Exodus",
    "The Bible, King James version, Book 20: Proverbs",
    "The Bible, King James version, Book 21: Ecclesiastes",
    "The Bible, King James version, Book 22: Song of Solomon",
    "The Bible, King James version, Book 23: Isaiah",
    "The Bible, King James version, Book 24: Jeremiah",
    "The Bible, King James version, Book 25: Lamentations",
    "The Bible, King James version, Book 26: Ezekiel",
    "The Bible, King James version, Book 27: Daniel",
    "The Bible, King James version, Book 28: Hosea",
    "The Bible, King James version, Book 29: Joel",
    "The Bible, King James version, Book 3: Leviticus",
    "The Bible, King James version, Book 30: Amos",
    "The Bible, King James version, Book 31: Obadiah",
    "The Bible, King James version, Book 32: Jonah",
    "The Bible, King James version, Book 33: Micah",
    "The Bible, King James version, Book 34: Nahum",
    "The Bible, King James version, Book 35: Habakkuk",
    "The Bible, King James version, Book 36: Zephaniah",
    "The Bible, King James version, Book 37: Haggai",
    "The Bible, King James version, Book 38: Zechariah",
    "The Bible, King James version, Book 39: Malachi",
    "The Bible, King James version, Book 4: Numbers",
    "The Bible, King James version, Book 40: Matthew",
    "The Bible, King James version, Book 41: Mark",
    "The Bible, King James version, Book 42: Luke",
    "The Bible, King James version, Book 43: John",
    "The Bible, King James version, Book 44: Acts",
    "The Bible, King James version, Book 45: Romans",
    "The Bible, King James version, Book 46: 1 Corinthians",
    "The Bible, King James version, Book 47: 2 Corinthians",
    "The Bible, King James version, Book 48: Galatians",
    "The Bible, King James version, Book 49: Ephesians",
    "The Bible, King James version, Book 5: Deuteronomy",
    "The Bible, King James version, Book 50: Philippians",
    "The Bible, King James version, Book 51: Colossians",
    "The Bible, King James version, Book 52: 1 Thessalonians",
    "The Bible, King James version, Book 53: 2 Thessalonians",
    "The Bible, King James version, Book 54: 1 Timothy",
    "The Bible, King James version, Book 55: 2 Timothy",
    "The Bible, King James version, Book 56: Titus",
    "The Bible, King James version, Book 57: Philemon",
    "The Bible, King James version, Book 58: Hebrews",
    "The Bible, King James version, Book 59: James",
    "The Bible, King James version, Book 6: Joshua",
    "The Bible, King James version, Book 60: 1 Peter",
    "The Bible, King James version, Book 61: 2 Peter",
    "The Bible, King James version, Book 62: 1 John",
    "The Bible, King James version, Book 63: 2 John",
    "The Bible, King James version, Book 64: 3 John",
    "The Bible, King James version, Book 65: Jude",
    "The Bible, King James version, Book 66: Revelation",
    "The Bible, King James version, Book 7: Judges",
    "The Bible, King James version, Book 8: Ruth",
    "The Bible, King James version, Book 9: 1 Samuel",
    "The Bible, King James Version, Complete Contents",
    "The Bible, King James Version, Complete",
    "The Bible, King James Version",
    "The Bible: I. Authenticity II. Credibility III. Morality",
    "The Bible: What It Is!",
    "The Black Man, the Father of Civilization, Proven by Biblical History",
    "The Blind Beggar of Jericho",
    "The Book of Daniel Unlocked",
    "The Book of God : In the Light of the Higher Criticism",
    "The Book of Job",
    "The Book of Jonah",
    "The Book of Light in the Hand of Love: A plea for the British and Foreign Bible Society",
    "The Book of Mormon",
    "The Book Of Mormon",
    "The books of Chronicles",
    "The Books of the New Testament",
    "The Boyhood of Jesus",
    "The Breadth, Freeness, and Yet Exclusiveness of the Gospel",
    "The Brook Kerith: A Syrian story",
    "The Canon of the Bible",
    "The Cause and Cure of the Cattle Plague: A Plain Sermon",
    "The Causes of the Corruption of the Traditional Text of the Holy Gospels",
    "The Cell of Self-Knowledge : seven early English mystical treatises printed by Henry Pepwell in 1521",
    "The Centurion's Story",
    "The Chaldean Account of Genesis",
    "The Character of the Jew Books",
    "The Child Who Died and Lived Again",
    "The Child's Book About Moses",
    "The Children's Bible",
    "The Children's Tabernacle; Or, Hand-Work and Heart-Work",
    "The Chosen People: A Compendium of Sacred and Church History for School-Children",
    "The Christ Myth",
    "The Christ of Paul; Or, The Enigmas of Christianity",
    "The Christian Use of the Psalter",
    "The Christian View of the Old Testament",
    "The Chronology of Ancient Kingdoms Amended",
    "The Church and the Empire",
    "The Cities of Refuge: or, The Name of Jesus",
    "The Common Edition: New Testament",
    "The Confessions of St. Augustine",
    "The Confutatio Pontificia",
    "The Covenants And The Covenanters",
    "The Creation of God",
    "The Criticism of the Fourth Gospel",
    "The Curtezan unmasked; or, The Whoredomes of Jezebel Painted to the Life",
    "The Dance of Death",
    "The Declaration of Independence",
    "The Deluge in the Light of Modern Science: A Discourse",
    "The Divine Office: A Study of the Roman Breviary",
    "The Doré Bible Gallery, Complete",
    "The Doré Bible Gallery, Volume 1",
    "The Doré Bible Gallery, Volume 2",
    "The Doré Bible Gallery, Volume 3",
    "The Doré Bible Gallery, Volume 4",
    "The Doré Bible Gallery, Volume 5",
    "The Doré Bible Gallery, Volume 6",
    "The Doré Bible Gallery, Volume 7",
    "The Doré Bible Gallery, Volume 8",
    "The Doré Bible Gallery, Volume 9",
    "The Doubts of Infidels",
    "The Elder Son Explained, and the Romish Church Exposed",
    "The Epistle of Paul the Apostle to the Colossians",
    "The Epistle of Paul the Apostle to the Ephesians",
    "The Epistle of Paul the Apostle to the Galatians",
    "The Epistle of Paul the Apostle to the Philippians",
    "The Epistle of Paul the Apostle to the Romans",
    "The Epistle of Paul to Titus",
    "The Epistle of Philemon",
    "The Epistle to the Hebrews",
    "The Epistles of St. Peter and St. Jude Preached and Explained",
    "The Evolution of Old Testament Religion",
    "The Evolution of Theology: an Anthropological Study",
    "The Existence and Attributes of God, Volumes 1 and 2",
    "The Expositor's Bible: Ezra, Nehemiah, and Esther",
    "The Expositor's Bible: Index",
    "The Expositor's Bible: Judges and Ruth",
    "The Expositor's Bible: The Acts of the Apostles, Vol. 1",
    "The Expositor's Bible: The Acts of the Apostles, Vol. 2",
    "The Expositor's Bible: The Book of Daniel",
    "The Expositor's Bible: The Book of Deuteronomy",
    "The Expositor's Bible: The Book of Exodus",
    "The Expositor's Bible: The Book of Ezekiel",
    "The Expositor's Bible: The Book of Genesis",
    "The Expositor's Bible: The Book of Isaiah, Volume 1 (of 2)",
    "The Expositor's Bible: The Book of Isaiah, Volume 2 (of 2)",
    "The Expositor's Bible: The Book of Joshua",
    "The Expositor's Bible: The Book of Leviticus",
    "The Expositor's Bible: The Book of Numbers",
    "The Expositor's Bible: The Book of Proverbs",
    "The Expositor's Bible: The Book of Revelation",
    "The Expositor's Bible: The Book of the Twelve Prophets, Vol. 1",
    "The Expositor's Bible: The Book of the Twelve Prophets, Vol. 2",
    "The Expositor's Bible: The Books of Chronicles",
    "The Expositor's Bible: The Epistle of St Paul to the Romans",
    "The Expositor's Bible: The Epistle to the Ephesians",
    "The Expositor's Bible: The Epistle to the Galatians",
    "The Expositor's Bible: The Epistle to the Hebrews",
    "The Expositor's Bible: The Epistle to the Philippians",
    "The Expositor's Bible: The Epistles of St. Paul to the Colossians and Philemon",
    "The Expositor's Bible: The Epistles of St. Peter",
    "The Expositor's Bible: The Epistles to the Thessalonians",
    "The Expositor's Bible: The First Book of Kings",
    "The Expositor's Bible: The First Book of Samuel",
    "The Expositor's Bible: The First Epistle to the Corinthians",
    "The Expositor's Bible: The General Epistles of St. James and St. Jude",
    "The Expositor's Bible: The Gospel According to St. Mark",
    "The Expositor's Bible: The Gospel of St. John, Vol. I",
    "The Expositor's Bible: The Gospel of St. John, Vol. II",
    "The Expositor's Bible: The Pastoral Epistles",
    "The Expositor's Bible: The Prophecies of Jeremiah",
    "The Expositor's Bible: The Psalms, Vol. 1",
    "The Expositor's Bible: The Psalms, Vol. 2",
    "The Expositor's Bible: The Psalms, Vol. 3",
    "The Expositor's Bible: The Second Book of Kings",
    "The Expositor's Bible: The Second Book of Samuel",
    "The Expositor's Bible: The Second Epistle to the Corinthians",
    "The Expositor's Bible: The Song of Solomon and the Lamentations of Jeremiah",
    "The Famous Missions of California",
    "The Farmer Boy: The Story of Jacob",
    "The First Boke of Moses called Genesis",
    "The First Book of Adam and Eve",
    "The First Epistle General of Peter",
    "The First Epistle of John",
    "The First Epistle of Paul the Apostle to the Corinthians",
    "The First Epistle of Paul the Apostle to Timothy",
    "The First Epistle of Paul to the Thessalonians",
    "The first New Testament printed in English",
    "The Flood",
    "The Four-Faced Visitors of Ezekiel",
    "The Garden of Eden: Stories from the first nine books of the Old Testament",
    "The General Epistle of James",
    "The Glad Tidings",
    "The Gospel According to Peter: A Study",
    "The Gospel According to Saint John",
    "The Gospel According to Saint Mark",
    "The Gospel According to St. Matthew",
    "The Gospel of John for Readers",
    "The Gospel of Luke for Readers",
    "The Gospel of Luke, an exposition",
    "The Gospel of Mark for Readers",
    "The Gospel of Matthew for Readers",
    "The Gospel of St. John: A Series of Discourses.",
    "The Gospel of St. John",
    "The Gospel of the Pentateuch: A Set of Parish Sermons",
    "The Gospels in Four Part Harmony",
    "The Gospels in the Second Century",
    "The Great Apostasy, Considered in the Light of Scriptural and Secular History",
    "The Great Doctrines of the Bible",
    "The Great Painters' Gospel",
    "The Hart and the Water-Brooks: a practical exposition of the forty-second Psalm.",
    "The Historical Evidence for the Virgin Birth",
    "The Hymns of Prudentius",
    "The Imitation of Christ",
    "The Influence of the Bible on Civilisation",
    "The Interpreters of Genesis and the Interpreters of Nature",
    "The Jerusalem Sinner Saved; or, Good News for the Vilest of Men",
    "The Jesus of History",
    "The Juvenile Bible: Being a brief concordance of the Holy Scriptures, in verse.",
    "The Key to Peace",
    "The King James Bible, Complete",
    "The King James Bible",
    "The King James Version of the Bible",
    "The Kingdom of Promise and Prophecy",
    "The Last Twelve Verses of the Gospel According to S. Mark",
    "The Lay-Man's Sermon upon the Late Storm",
    "The Legends of the Jews — Volume 1",
    "The Legends of the Jews — Volume 2",
    "The Legends of the Jews — Volume 3",
    "The Legends of the Jews — Volume 4",
    "The Life of David: As Reflected in His Psalms",
    "The Life of Duty, v. 2",
    "The Lights of the Church and the Light of Science",
    "The Literature and History of New Testament Times",
    "The Literature of the Old Testament",
    "The Little Child's Book of Divinity",
    "The Little Maid of Israel",
    "The Lost Faith, and Difficulties of the Bible, as Tested by the Laws of Evidence",
    "The Lost Gospel and Its Contents",
    "The Makers and Teachers of Judaism",
    "The Making of the New Testament",
    "The Man Who Did Not Die: The Story of Elijah",
    "The Messiah in Moses and the Prophets",
    "The Morning of Spiritual Youth Improved, in the Prospect of Old Age and Its Infirmities",
    "The Mosaic History of the Creation of the World",
    "The National Preacher, Vol. 2 No. 7 Dec. 1827",
    "The National Preacher, Vol. 2. No. 6., Nov. 1827",
    "The Neptunian, or Water Theory of Creation",
    "The New Testament of our Lord and Savior Jesus Christ.",
    "The New Testament",
    "The Old Franciscan Missions Of California",
    "The Old Testament in the Light of the Historical Records and Legends of Assyria and Babylonia",
    "The Opening Heavens",
    "The Origin and Permanent Value of the Old Testament",
    "The Origin of Paul's Religion",
    "The Origin of the World According to Revelation and Science",
    "The Other Side of Evolution: Its Effects and Fallacy",
    "The Otterbein Hymnal",
    "The Parables of the Saviour",
    "The Patriarchs",
    "The Peep of Day",
    "The Penance of Magdalena and Other Tales of the California Missions",
    "The Pentateuch, in Its Progressive Revelations of God to Men",
    "The People's Idea of God: Its Effect On Health And Christianity",
    "The Pilgrim's Progress from this world to that which is to come",
    "The Practice of the Presence of God the Best Rule of a Holy Life",
    "The Prayer Book Explained",
    "The Prayers of St. Paul",
    "The Preacher's Complete Homiletic Commentary of the Books of the Bible: Volume 29 (of 32)",
    "The Preacher's Complete Homiletic Commentary on the Books of the Bible, Volume 13 (of 32)",
    "The Preacher's Complete Homiletic Commentary on the Books of the Bible, Volume 15 (of 32)",
    "The Prince of the House of David",
    "The Prophet Ezekiel: An Analytical Exposition",
    "The prophete Ionas with an introduccion",
    "The Prose Works of Jonathan Swift, D.D. — Volume 03",
    "The Prose Works of Jonathan Swift, D.D. — Volume 04",
    "The Psalms of David",
    "The Pulpit Of The Reformation, Nos. 1, 2, 3 and 4.",
    "The Religion of Ancient Palestine in the Second Millenium B.C.",
    "The Religion of Geology and Its Connected Sciences",
    "The religious and loyal subject's duty considered: with regard to the present Government and the Revolution",
    "The Religious Spirit of the Slavs",
    "The Revelation Explained",
    "The Revelation of Saint John",
    "The Revelation of St. John the Divine",
    "The Revision Revised",
    "The Right and Wrong Uses of the Bible",
    "The Rise and Progress of Palaeontology",
    "The Ritual Movement",
    "The Roman Empire in the Light of Prophecy",
    "The Sceptics of the Old Testament: Job - Koheleth - Agur",
    "The Scripture Club of Valley Rest; or, Sketches of Everybody's Neighbours",
    "The Scriptures Able to Make Us Wise Unto Salvation",
    "The Second Epistle General of Peter",
    "The Second Epistle of John",
    "The Second Epistle of Paul the Apostle to the Corinthians",
    "The Second Epistle of Paul the Apostle to Timothy",
    "The Second Epistle of Paul to the Thessalonians",
    "The Secret of the Creation",
    "The Shepherd Of My Soul",
    "The Shepherd Psalm: A Meditation",
    "The Smalcald Articles",
    "The Song of our Syrian Guest",
    "The Song of Songs",
    "The Source and Mode of Solar Energy Throughout the Universe",
    "The Spiritual Improvement of the Census",
    "The St. Gregory Hymnal and Catholic Choir Book",
    "The Standard Oratorios: Their Stories, Their Music, And Their Composers",
    "The Story of a Soul (L'Histoire d'une Âme): The Autobiography of St. Thérèse of Lisieux",
    "The Story of Creation as Told By Theology and By Science",
    "The Story of Genesis and Exodus: An Early English Song, about 1250 A.D.",
    "The Story of the Hymns and Tunes",
    "The Story of the Prophet Jonas",
    "The Story Of The Prophet Jonas",
    "The Supernatural in the New Testament, Possible, Credible, and Historical",
    "The Testimony of the Bible Concerning the Assumptions of Destructive Criticism",
    "The Testimony of the Rocks",
    "The Things Which Remain",
    "The Third Epistle of John",
    "The Three Additions to Daniel, a Study",
    "The Threshold Grace: Meditations in the Psalms",
    "The Time of the End",
    "The Traditional Text of the Holy Gospels",
    "The True Ministers of Christ Accredited by the Holy Spirit: A Sermon",
    "The Truth about Jesus : Is He a Myth?",
    "The Two Treaties; or, Hope for Jerusalem",
    "The Undying Fire: A contemporary novel",
    "The United States in the Light of Prophecy",
    "The Vailan or annular theory: A synopsis of Prof. I. N. Vail's argument in support of the claim that this Earth once possessed a Saturn-like system of rings",
    "The Village in the Mountains; Conversion of Peter Bayssière; and History of a Bible",
    "The Village Pulpit, Volume II. Trinity to Advent",
    "The Water of Life, and Other Sermons",
    "The Wave of Scepticism and the Rock of Truth",
    "The Witch Hypnotizer",
    "The Witness of the Stars",
    "The Woman's Bible",
    "The Wonder Book of Bible Stories",
    "The wonders of prayer",
    "The World English Bible (WEB), Complete",
    "The World English Bible (WEB): 1 Chronicles",
    "The World English Bible (WEB): 1 Corinthians",
    "The World English Bible (WEB): 1 John",
    "The World English Bible (WEB): 1 Kings",
    "The World English Bible (WEB): 1 Peter",
    "The World English Bible (WEB): 1 Samuel",
    "The World English Bible (WEB): 1 Thessalonians",
    "The World English Bible (WEB): 1 Timothy",
    "The World English Bible (WEB): 2 Chronicles",
    "The World English Bible (WEB): 2 Corinthians",
    "The World English Bible (WEB): 2 John",
    "The World English Bible (WEB): 2 Kings",
    "The World English Bible (WEB): 2 Peter",
    "The World English Bible (WEB): 2 Samuel",
    "The World English Bible (WEB): 2 Thessalonians",
    "The World English Bible (WEB): 2 Timothy",
    "The World English Bible (WEB): 3 John",
    "The World English Bible (WEB): Acts",
    "The World English Bible (WEB): Amos",
    "The World English Bible (WEB): Colossians",
    "The World English Bible (WEB): Daniel",
    "The World English Bible (WEB): Deuteronomy",
    "The World English Bible (WEB): Ecclesiastes",
    "The World English Bible (WEB): Ephesians",
    "The World English Bible (WEB): Esther",
    "The World English Bible (WEB): Exodus",
    "The World English Bible (WEB): Ezekiel",
    "The World English Bible (WEB): Ezra",
    "The World English Bible (WEB): Galatians",
    "The World English Bible (WEB): Genesis",
    "The World English Bible (WEB): Habakkuk",
    "The World English Bible (WEB): Haggai",
    "The World English Bible (WEB): Hebrews",
    "The World English Bible (WEB): Hosea",
    "The World English Bible (WEB): Isaiah",
    "The World English Bible (WEB): James",
    "The World English Bible (WEB): Jeremiah",
    "The World English Bible (WEB): Job",
    "The World English Bible (WEB): Joel",
    "The World English Bible (WEB): John",
    "The World English Bible (WEB): Jonah",
    "The World English Bible (WEB): Joshua",
    "The World English Bible (WEB): Jude",
    "The World English Bible (WEB): Judges",
    "The World English Bible (WEB): Lamentations",
    "The World English Bible (WEB): Leviticus",
    "The World English Bible (WEB): Luke",
    "The World English Bible (WEB): Malachi",
    "The World English Bible (WEB): Mark",
    "The World English Bible (WEB): Matthew",
    "The World English Bible (WEB): Micah",
    "The World English Bible (WEB): Nahum",
    "The World English Bible (WEB): Nehemiah",
    "The World English Bible (WEB): Numbers",
    "The World English Bible (WEB): Obadiah",
    "The World English Bible (WEB): Philemon",
    "The World English Bible (WEB): Philippians",
    "The World English Bible (WEB): Proverbs",
    "The World English Bible (WEB): Psalms",
    "The World English Bible (WEB): Revelation",
    "The World English Bible (WEB): Romans",
    "The World English Bible (WEB): Ruth",
    "The World English Bible (WEB): Song of Solomon",
    "The World English Bible (WEB): Titus",
    "The World English Bible (WEB): Zechariah",
    "The World English Bible (WEB): Zephaniah",
    "The World's Great Sermons, Volume 01: Basil to Calvin",
    "The World's Great Sermons, Volume 02: Hooker to South",
    "The World's Great Sermons, Volume 03: Massillon to Mason",
    "The World's Great Sermons, Volume 08: Talmage to Knox Little",
    "The World's Great Sermons, Volume 10: Drummond to Jowett, and General Index",
    "They Call Me Carpenter",
    "Three Prayers and Sermons",
    "Three Sermons, Three Prayers",
    "Toleration and other essays",
    "Town and Country Sermons",
    "Training the Teacher",
    "True Christianity",
    "Twenty-Five Village Sermons",
    "Twenty-Four Short Sermons On The Doctrine Of Universal Salvation",
    "Twenty-Seven Drawings by William Blake",
    "Understanding the Scriptures",
    "Union and Communion; or, Thoughts on the Song of Solomon",
    "Unity of Good",
    "Walks and Words of Jesus: A Paragraph Harmony of the Four Evangelists",
    "Wee Ones' Bible Stories",
    "Welcome to the ransomed; or, Duties of the colored inhabitants of the District of Columbia",
    "Welsh Nationality, and How Alone It is to Be Saved: A Sermon",
    "Westminster Sermons",
    "Weymouth New Testament in Modern Speech, 1 Corinthians",
    "Weymouth New Testament in Modern Speech, 1 John",
    "Weymouth New Testament in Modern Speech, 1 Peter",
    "Weymouth New Testament in Modern Speech, 1 Thessalonians",
    "Weymouth New Testament in Modern Speech, 1 Timothy",
    "Weymouth New Testament in Modern Speech, 2 Corinthians",
    "Weymouth New Testament in Modern Speech, 2 John",
    "Weymouth New Testament in Modern Speech, 2 Peter",
    "Weymouth New Testament in Modern Speech, 2 Thessalonians",
    "Weymouth New Testament in Modern Speech, 2 Timothy",
    "Weymouth New Testament in Modern Speech, 3 John",
    "Weymouth New Testament in Modern Speech, Acts",
    "Weymouth New Testament in Modern Speech, Colossians",
    "Weymouth New Testament in Modern Speech, Ephesians",
    "Weymouth New Testament in Modern Speech, Galatians",
    "Weymouth New Testament in Modern Speech, Hebrews",
    "Weymouth New Testament in Modern Speech, James",
    "Weymouth New Testament in Modern Speech, John",
    "Weymouth New Testament in Modern Speech, Jude",
    "Weymouth New Testament in Modern Speech, Luke",
    "Weymouth New Testament in Modern Speech, Mark",
    "Weymouth New Testament in Modern Speech, Matthew",
    "Weymouth New Testament in Modern Speech, Philemon",
    "Weymouth New Testament in Modern Speech, Philippians",
    "Weymouth New Testament in Modern Speech, Preface and Introductions",
    "Weymouth New Testament in Modern Speech, Revelation",
    "Weymouth New Testament in Modern Speech, Romans",
    "Weymouth New Testament in Modern Speech, Titus",
    "When Were Our Gospels Written?",
    "Who Wrote the Bible? : a Book for the People",
    "Wit and Humor of the Bible: A Literary Study",
    "Woman in Sacred History",
    "Women in white raiment",
    "Works of Martin Luther",
    "Young Folks' Bible in Words of Easy Reading",
    "Systematic Theology (Volume 3 of 3)",
    "Systematic Theology (Volume 2 of 3)",
    "Systematic Theology (Volume 1 of 3)",
    "Education",
    "Armor and Arms",
    "The Mormon Doctrine of Deity",
    "The Fundamental Doctrines of the Christian faith"
};

var authorExclusion = new List<string>
{
    "Flavius Josephus",
    "Mary Baker Eddy",
    "J. Clontz",
    "John Bunyan",
    "Joseph Stump",
    "Rev. William Evans",
    "Henry F. Lutz",
    "E. B. Stewart",
    "Henry T. Sell",
    "Benedict of Spinoza",
    "Alexander von Humboldt",
    "Augustus Hopkins Strong",
    "Martin Luther"
};

List<LiteratureTime> literatureTimes = [];

List<string> fileDirectoryDone = [];

var lastSeekTime = DateTime.UnixEpoch;
if (File.Exists($"{outputDirectory}/fileDirectoryDone.json"))
{
    var content = File.ReadAllText($"{outputDirectory}/fileDirectoryDone.json");
    fileDirectoryDone = JsonSerializer.Deserialize<List<string>>(content) ?? [];

    content = File.ReadAllText($"{outputDirectory}/lastSeekTime");
    var unixTimeSeconds = long.Parse(content, CultureInfo.InvariantCulture);
    lastSeekTime = DateTimeOffset.FromUnixTimeSeconds(unixTimeSeconds).LocalDateTime;
}

Console.WriteLine(lastSeekTime);

Console.CancelKeyPress += (s, e) =>
{
    var fileDirectoryDoneJson = JsonSerializer.Serialize(fileDirectoryDone, jsonSerializerOptions);
    File.WriteAllText($"{outputDirectory}/fileDirectoryDone.json", fileDirectoryDoneJson);

    File.WriteAllText(
        $"{outputDirectory}/lastSeekTime",
        DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture)
    );
};

try
{
    var totalFiles = files.Count;
    var processedFiles = 0;

    foreach (var file in files)
    {
        processedFiles += 1;

        var filePath = Path.GetDirectoryName(file);

        var fileDirectory = filePath?.Split(Path.DirectorySeparatorChar).LastOrDefault();
        if (fileDirectory == null)
        {
            continue;
        }

        fileDirectory = fileDirectory.ToLowerInvariant();
        if (fileDirectory == "old")
        {
            continue;
        }

        var fileToRead = Path.Combine(filePath!, $"{fileDirectory}.txt");

        // Prefer utf-8, files that end in -0
        // Otherwise prefer files that end in -8
        // else fallback to default
        var utf8File = Path.Combine(filePath!, $"{fileDirectory}-0.txt");
        var iso8859_1 = Path.Combine(filePath!, $"{fileDirectory}-8.txt");
        Encoding encoding = Encoding.ASCII;
        if (File.Exists(utf8File))
        {
            fileToRead = utf8File;
            encoding = Encoding.UTF8;
        }
        else if (File.Exists(iso8859_1))
        {
            fileToRead = iso8859_1;
            encoding = Encoding.Latin1;
        }

        if (!File.Exists(fileToRead))
        {
            // Console.WriteLine($"Skipping (wrong format) {file} - {processedFiles}:{totalFiles}");
            continue;
        }

        var fileToReadDate = File.GetLastWriteTimeUtc(fileToRead);
        if (fileDirectoryDone.Contains(fileDirectory))
        {
            if (fileToReadDate >= lastSeekTime)
            {
                Console.WriteLine($"Updating (modified) {file} - {processedFiles}:{totalFiles}");
            }
            else
            {
                // Console.WriteLine($"Skipping (directory done) {file} - {processedFiles}:{totalFiles}");
                continue;
            }
        }

        Console.WriteLine($"{fileToRead} - {processedFiles}:{totalFiles}");

        var lines = File.ReadAllLines(fileToRead, encoding);
        var title = "";
        var author = "";
        var language = "";

        var startIndex = -1;
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (line.StartsWith("Posting Date:", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (line.StartsWith("Release Date:", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (line.StartsWith("Language:", StringComparison.OrdinalIgnoreCase))
            {
                language = line.Replace(
                        "Language:",
                        "",
                        StringComparison.InvariantCultureIgnoreCase
                    )
                    .Trim();
                if (string.IsNullOrEmpty(language))
                {
                    break;
                }

                if (!language.Contains("English", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                continue;
            }

            if (line.StartsWith("[Most recently updated:", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (line.StartsWith("Title: ", StringComparison.OrdinalIgnoreCase))
            {
                title = line.Replace("Title:", "", StringComparison.InvariantCultureIgnoreCase)
                    .Trim();
                if (string.IsNullOrEmpty(title))
                {
                    break;
                }

                if (titlesExclusion.Contains(title))
                {
                    break;
                }

                continue;
            }

            if (line.StartsWith("Author: ", StringComparison.OrdinalIgnoreCase))
            {
                author = line.Replace("Author:", "", StringComparison.InvariantCultureIgnoreCase)
                    .Trim();
                if (string.IsNullOrEmpty(author))
                {
                    break;
                }

                if (authorExclusion.Contains(author))
                {
                    break;
                }

                continue;
            }

            if (
                !string.IsNullOrEmpty(title)
                && !string.IsNullOrEmpty(author)
                && !string.IsNullOrEmpty(language)
            )
            {
                startIndex = i;
                break;
            }
        }

        var matches = new ConcurrentDictionary<int, Match>();

        if (startIndex != -1)
        {
            Parallel.For(
                startIndex + 1,
                lines.Length,
                parallelOptions,
                index =>
                {
                    var line = lines[index];
                    var result = Matcher.FindMatches(
                        timePhrasesOneOf,
                        timePhrasesGenericOneOf,
                        timePhrasesSuperGenericOneOf,
                        line
                    );
                    if (result.Matches.Count > 0)
                    {
                        matches.TryAdd(index, result);
                    }
                }
            );
        }

        var literatureTimesFromMatches = Matcher.GenerateQuotesFromMatches(
            matches,
            lines,
            title,
            author,
            fileDirectory
        );

        literatureTimes.AddRange(literatureTimesFromMatches);

        var lookup = literatureTimesFromMatches.ToLookup(t => t.Time);
        foreach (var literatureTimesIndexGroup in lookup)
        {
            var jsonString = JsonSerializer.Serialize(
                literatureTimesIndexGroup.ToList(),
                jsonSerializerOptions
            );

            var directory =
                $"{outputDirectory}/{literatureTimesIndexGroup.Key.Replace(":", "_", StringComparison.InvariantCultureIgnoreCase)}";
            Directory.CreateDirectory(directory);

            File.WriteAllText($"{directory}/{fileDirectory}.json", jsonString);
        }

        fileDirectoryDone.Add(fileDirectory);
    }
}
finally
{
    var fileDirectoryDoneJson = JsonSerializer.Serialize(fileDirectoryDone, jsonSerializerOptions);
    File.WriteAllText($"{outputDirectory}/fileDirectoryDone.json", fileDirectoryDoneJson);

    File.WriteAllText(
        $"{outputDirectory}/lastSeekTime",
        DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture)
    );
}
