using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using Newtonsoft.Json;

namespace ChatBot
{
    public class Bot : AbsBot
    {
        static string userName; // имя пользователя
        string path; // путь к файлу с историей сообщений
        List<string> history = new List<string>(); // хранение истории

        List<Regex> regecies = new List<Regex>
            {
            new Regex(@"привет|здраствуй"),
            new Regex(@"(?:который час\??|сколько времени\??)|время\??"),
            new Regex(@"(?:какое сегодня число\??|число\??)"),
            new Regex(@"как дела\??"),
            new Regex(@"(?:спасибо|благодарю)"),
            new Regex(@"(?:умножь(\s)?(\d+)(\s)?на(\s)?(\d+))"),
            new Regex(@"(?:раздели(\s)?(\d+)(\s)?на(\s)?(\d+))"),
            new Regex(@"(?:сложи(\s)?(\d+)(\s)?и(\s)?(\d+))"),
            new Regex(@"(?:вычти(\s)?(\d+)(\s)?из(\s)?(\d+))"),
            new Regex(@"какая погода\??|погода\??"),
            new Regex(@"курс валют\??|курс\??|какой курс\??")
            };

        Func<string, string> funcBuf; //буфер

        List<Func<string, string>> func = new List<Func<string, string>>
            {
                HelloBot,
                HowTime,
                HowDate,
                HowAreYou,
                ThankYou,
                MulPls,
                DivPls,
                PlusPls,
                SubPls,
                WeatherPls,
                HowCourse,

            };

        public Bot()
        {

        }

        public Bot(string filename)
        {
            LoadHistory(filename);
        }

        public string UserName
        {
            get
            {
                return userName;
            }
        }
        public string Path
        {
            get
            {
                return path;
            }
        }
        public List<string> History
        {
            get
            {
                return history;
            }
        }
        static string HelloBot(string question)
        {
            return "Привет, " + userName + "!";
        }

        static string HowTime(string question)
        {
            return "Сейчас: " + DateTime.Now.ToString("HH:mm");
        }

        static string HowDate(string question)
        {
            return "Сегодня: " + DateTime.Now.ToString("dd.MM.yy");
        }

        static string HowAreYou(string question)
        {
            Random rnd = new Random();
            int value = rnd.Next();
            if (value % 2 == 0)
                return "Все хорошо!)";
            else
            {
                return "Чувствую себя неплохо )";
            }
        }

        static string ThankYou(string question)
        {
            Random rnd = new Random();
            int value = rnd.Next();
            if (value % 2 == 0)
                return "Рад был помочь!";
            else
            {
                return "Не за что, обращайтесь ещё )";
            }
        }

        static string MulPls(string question)
        {
            question = question.Replace(" ", "");
            question = question.Substring(question.LastIndexOf('ь') + 1);
            string[] words = question.Split(new char[] { 'н', 'а' },
            StringSplitOptions.RemoveEmptyEntries);
            try
            {
                int num1 = Convert.ToInt32(words[0]);
                int num2 = Convert.ToInt32(words[1]);
                return (num1 * num2).ToString();
            }
            catch
            {
                return "Извините, не могу разобрать. Повторите, пожалуйста, ввод.";
            }
        }

        static string DivPls(string question)
        {
            question = question.Replace(" ", "");
            question = question.Substring(question.LastIndexOf('и') + 1);
            string[] words = question.Split(new char[] { 'н', 'а' },
            StringSplitOptions.RemoveEmptyEntries);
            try
            {
                float num1 = Convert.ToInt32(words[0]);
                float num2 = Convert.ToInt32(words[1]);
                return (num1 / num2).ToString();
            }
            catch
            {
                return "Извините, не могу разобрать. Повторите, пожалуйста, ввод.";
            }
        }

        static string PlusPls(string question)
        {
            question = question.Replace(" ", "");
            question = question.Substring(question.LastIndexOf('ж') + 2);
            string[] words = question.Split(new char[] { 'и' },
            StringSplitOptions.RemoveEmptyEntries);
            try
            {
                int num1 = Convert.ToInt32(words[0]);
                int num2 = Convert.ToInt32(words[1]);
                return (num1 + num2).ToString();
            }
            catch
            {
                return "Извините, не могу разобрать. Повторите, пожалуйста, ввод.";
            }
        }

        static string SubPls(string question)
        {
            question = question.Replace(" ", "");
            question = question.Substring(question.LastIndexOf('т') + 2);
            string[] words = question.Split(new char[] { 'и', 'з' },
            StringSplitOptions.RemoveEmptyEntries);
            try
            {
                int num1 = Convert.ToInt32(words[0]);
                int num2 = Convert.ToInt32(words[1]);
                return (num2 - num1).ToString();
            }
            catch
            {
                return "Извините, не могу разобрать. Повторите, пожалуйста, ввод.";
            }
        }

        static string WeatherPls(string question)
        {
            String[] infoWeather = Weather.FindOutWeather(); // возвращает массив строк с информацией о погоде
            if (infoWeather[0] != null) // если null, значит отсутствет подключение к интернету
            {
                return "Температура в городе " + infoWeather[0] + " " + infoWeather[1] + " °C"
                    + ", ветер " + infoWeather[2] + " м/c";
            }
            else return "Проверьте подключение к интернету!";
        }
        static string HowCourse(string question)
        {
            String[] info = FindCourse(); // возвращает массив строк с информацией о курсах валют
            if (info[0] != null) // если null, значит отсутствет подключение к интернету
                return info[0] + ": " + info[1] + "; " +  // USD name + value
                    info[2] + ": " + info[3] + "; " +     // EUR name + value
                    info[4] + ": " + info[5];             // CNY name + value
            else return "Проверьте подключение к интернету!";
        }
        static private String[] FindCourse()
        {
            string url = "https://www.cbr-xml-daily.ru/daily_json.js";

            // инициализация WebRequest
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            try
            {
                // возврат ответа от интернет-ресурса
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                string response;
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }

                // конвертируем тип JSON в .NET тип
                DailyCourse cbr = JsonConvert.DeserializeObject<DailyCourse>(response);

                String[] info = { cbr.Valute.USD.Name, cbr.Valute.USD.Value.ToString(),
                                    cbr.Valute.EUR.Name, cbr.Valute.EUR.Value.ToString(),
                                    cbr.Valute.CNY.Name, cbr.Valute.CNY.Value.ToString() };
                return info;
            }
            catch
            {
                // в случае возникновения ошибки возвращаем пустой массив
                String[] error = { null };
                return error;
            }
        }

        public void LoadHistory(string user)
        {
            userName = user;
            path = userName + ".txt"; // запись пути

            try
            {
                //попытка загрузки существующей базы
                history.AddRange(File.ReadAllLines(path, Encoding.GetEncoding(1251)));

                // Если файл был изменен не сегодня, то записываем новую дату
                // переписки
                if (File.GetLastWriteTime(path).ToString("dd.MM.yy") !=
                    DateTime.Now.ToString("dd.MM.yy"))
                {
                    String[] date = new String[] {"\n" + "Переписка от " +
                        DateTime.Now.ToString("dd.MM.yy"+ "\n")};
                    AddToHistory(date);
                }
            }
            catch
            {
                // если файл не существует, создаем его
                File.WriteAllLines(path, history.ToArray(), Encoding.GetEncoding(1251));
                // отмечаем дату начала переписки
                String[] date = new String[] {"Переписка от " +
                        DateTime.Now.ToString("dd.MM.yy") + "\n"};
                AddToHistory(date);

            }
        }

        public void AddToHistory(string[] answer)
        {
            history.AddRange(answer);
            File.WriteAllLines(path, history.ToArray(), Encoding.GetEncoding(1251));
        }

        // проверка сообщения от пользователя и возвращения ответа
        public override string Ans(string Que)
        {
            Que = Que.ToLower(); // переводим в нижний регистр
            for (int i = 0; i < regecies.Count; i++)
            {
                if (regecies[i].IsMatch(Que))
                {
                    funcBuf = func[i];
                    return funcBuf(Que);
                }

            }
            return "Извините, я вас не понимаю ,посмотрите справку";
        }

    }
}


