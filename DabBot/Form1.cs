 //by Vlasov Andrey (andrew.vlasof@yandex.ru)
 //Version 0.0.1
using System;
using System.Windows.Forms;
using System.Speech.Synthesis;
using Microsoft.Speech.Recognition;
using System.Globalization;
using System.Diagnostics;


namespace DabBot
{
    public partial class Form1 : Form
    {
     public   SpeechSynthesizer ss = new SpeechSynthesizer();  //синтез речи
       public SpeechRecognitionEngine sre; //распознавание речи
       public bool done = false;
      public bool speechOn = true; //Включение и отключение распознавания речи
        public NotifyIcon NI;
        public Form1()
        {
            InitializeComponent();
          
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                NI = new NotifyIcon();
       
                NI.Visible = true;
                NI.Icon = this.Icon;
                ss.SetOutputToDefaultAudioDevice();

                ss.Speak("Система запущена");

                CultureInfo ci = new CultureInfo("ru-ru"); //язык распознавания
                sre = new SpeechRecognitionEngine(ci);
                sre.SetInputToDefaultAudioDevice();
                sre.SpeechRecognized += sre_SpeechRecognized;

                Choices ch_StartStopCommands = new Choices();              //словарь главных команд
                ch_StartStopCommands.Add("Даб");
                ch_StartStopCommands.Add("Отключись");
                ch_StartStopCommands.Add("Очистить");
                ch_StartStopCommands.Add("Трей");
               
                GrammarBuilder gb_StartStop = new GrammarBuilder();
                gb_StartStop.Append(ch_StartStopCommands);
                Grammar g_StartStop = new Grammar(gb_StartStop);

                Choices ch_Numbers = new Choices();
                for (int i = 0; i < 100; i++)
                {
                    ch_Numbers.Add(i.ToString());
                }
                Choices ch_Func = new Choices();    //словарь функции калькулятора
                ch_Func.Add("плюс");
                ch_Func.Add("минус");
                ch_Func.Add("делить");
                ch_Func.Add("умножить");
                GrammarBuilder gb_WhatIsXplusY = new GrammarBuilder();

                gb_WhatIsXplusY.Append(ch_Numbers);
                gb_WhatIsXplusY.Append(ch_Func);
                gb_WhatIsXplusY.Append(ch_Numbers);
                Grammar g_WhatIsXplusY = new Grammar(gb_WhatIsXplusY);


                Choices ch_words = new Choices(); //словарь запускаемых программ
                ch_words.Add("Анрил");
                ch_words.Add("Юнити");
                ch_words.Add("Блендер");
                ch_words.Add("Браузер");
                ch_words.Add("Стим");

                GrammarBuilder gb_words = new GrammarBuilder();
                gb_words.Append("Запусти");
                gb_words.Append(ch_words);
                Grammar g_words = new Grammar(gb_words);


                sre.LoadGrammarAsync(g_StartStop);
                sre.LoadGrammarAsync(g_WhatIsXplusY);
                sre.LoadGrammarAsync(g_words);
                ss.Speak("Даб готов к работе");
                sre.RecognizeAsync(RecognizeMode.Multiple);




            }
            catch (Exception ex)    
            {
                ss.Speak("Ошибка загрузки " + ex.Message);
                listBox1.Items.Add("Bot->" + ex.Message);

            }
        }


    void sre_SpeechRecognized(object sender,                  //распознавание
    SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;                        //обработка команд
            float confidence = e.Result.Confidence;
            if(checkBox1.Checked==true)
           listBox1.Items.Add("Recognize->" + txt);

            if (confidence < 0.60) return;
            if (txt.IndexOf("Даб") >= 0)
            {


                listBox1.Items.Add("Bot->Speech ON");
                ss.SpeakAsync("Да");
                speechOn = true;
                if(listBox1.Items.Count>=10) listBox1.Items.Clear();
            }
            if (txt.IndexOf("Отключись") >= 0)
            {
                listBox1.Items.Add("Bot->Speech is now OFF");
                ss.SpeakAsync("Жду");
                speechOn = false;
            }
            if (speechOn == false) return;
          
            if (txt.IndexOf("Очистить") >= 0)
            {
                listBox1.Items.Clear();
                ss.SpeakAsync("Готово");
              
            }
            if (txt.IndexOf("Трей") >= 0)
            {
                listBox1.Items.Add("Bot->Трей");

                if (NI.Visible == false)
                {
                    NI.Visible = true;

                    Hide();
                } else
                {
                    NI.Visible = false;
                    WindowState = FormWindowState.Normal;
                    Show();
                    
                }
                ss.SpeakAsync("Готово");
                
               
            }

            if (txt.IndexOf("плюс") >= 0 | txt.IndexOf("минус") >= 0 | txt.IndexOf("разделить") >= 0 | txt.IndexOf("умножить") >= 0)
            {
                string[] words = txt.Split(' ');
                int num1 = int.Parse(words[0]);
                int num2 = int.Parse(words[2]);
                int sum=0;
                if (txt.IndexOf("плюс") >= 0)
                {
                     sum = num1 + num2;
                }
                else if(txt.IndexOf("минус") >= 0)
                {
                     sum = num1 - num2;

                }  else if (txt.IndexOf("умножить") >= 0)
                {
                     sum = num1 * num2;
                }
                else if (txt.IndexOf("делить") >= 0)
                {
                    if (num2 != 0)
                        sum = num1 / num2;
                    else ss.SpeakAsync("На ноль нельзя делить");
                }

                listBox1.Items.Add("Bot->"+ sum );
                ss.SpeakAsync(sum.ToString());
            }

            if (txt.IndexOf("Запусти") >= 0 )
            {
                string[] words = txt.Split(' ');
                string num2 = words[1];
                
               
                if (num2== "Анрил")
                {
                    Process.Start(@"C:\Program Files (x86)\Epic Games\Launcher\Portal\Binaries\Win64\EpicGamesLauncher.exe");
                }
                else if (num2 == "Юнити")
                {
                    Process.Start(@"C:\Program Files\Unity\Editor\Unity.exe");

                }
                else if (num2== "Блендер")
                {
                    Process.Start(@"C:\Program Files\Blender Foundation\Blender\blender.exe");
                }
                else if (num2== "Браузер")
                {
                    Process.Start(@"C:\Program Files (x86)\Mozilla Firefox\firefox.exe");
                }
                else if (num2 == "Стим")
                {
                    Process.Start(@"C:\Program Files (x86)\Steam\Steam.exe");
                }

                listBox1.Items.Add("Bot->Запустил " + num2);
                ss.SpeakAsync("Запустил");
            }
        } // sre_SpeechRecognized

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }
      public  string s = "0";
  
    }
}
