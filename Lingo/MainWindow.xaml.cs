using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;


namespace Lingo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static bool Specific = true;
        string CoppiedText = "", CurrentText = "";
        IList<Word> Words = new List<Word>();
        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadFiles()
        {
            string Text = File.ReadAllText("C:\\Users\\Sheikh\\Downloads\\eng_arabic\\dict.xdxf");
            string[] Chars = new string[] { "<ar><k>", "</k>\n", "</ar>\n<ar><k>", "</ar>" };
            string[] Lines = Text.Split(Chars, StringSplitOptions.RemoveEmptyEntries);
            string T1, T2;
            for (int i = 0; i < Lines.Length; i += 2)
            {
                T1 = Lines[i];
                T2 = Lines[i + 1];
                Words.Add(new Word() { EN = Formatter(T1), AR = Formatter(T2) });
            }
            TXT_BOX.Content = Lines.Length;
        }

        private string Formatter(string Line)
        {
            string NewLine;
            NewLine = Line.Replace("::", ";");
            return NewLine;
        }

        private void SaveFiles()
        {
            XmlSerializer Serializer; TextWriter Writer;
            string FilePath = Directory.GetCurrentDirectory() + "\\test.xml";
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
            Serializer = new XmlSerializer(typeof(List<Word>));
            Writer = new StreamWriter(FilePath);
            Serializer.Serialize(Writer, Words);

        }

        private void LoadData()
        {
            XmlSerializer Serializer = new XmlSerializer(typeof(List<Word>));
            Words = (List<Word>)Serializer.Deserialize(new MemoryStream(Properties.Resources.Vocabulary));
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) this.DragMove();
            if (e.ClickCount == 2) this.WindowState = WindowState.Minimized;
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            CoppiedText = Clipboard.GetText();
            if (CurrentText != CoppiedText)
            {
                string Text = "", New = "";
                int Length = 0, Count = -1;
                IEnumerable<Word> Result;
                CurrentText = CoppiedText;
                if (Specific) Result = Words.Where(Word => Word.Means(CoppiedText));
                else Result = Words.Where(Word => Word.Contains(CoppiedText));
                foreach (Word NewWord in Result)
                {
                    New = NewWord.ToString();
                    if (New.Length > Length) Length = New.Length;
                    Text += New + "\n";
                    Count++;
                }
                if (Text != "")
                {
                    TXT_BOX.Content = Text;
                    this.Width = 20 + Length * 8;
                    this.Height = 40 + Count * 17.5;
                    this.Show();
                }
                else
                {
                    TXT_BOX.Content = "Not Found !";
                    this.Width = 110;
                    this.Height = 40;
                    this.Show();
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                Specific = !Specific;
                CurrentText = "";
            }
        }
    }

    public class Word
    {
        public string AR { get; set; }
        public string EN { get; set; }

        public bool Means(string Text)
        {
            string Left = Text.ToLower();
            string Right = EN.ToLower();
            return Left == Right;
        }

        public bool Contains(string Text)
        {
            string Left = Text.ToLower();
            string Right = EN.ToLower();
            if (Right.Contains(Left))
            {
                return Left == Right.Substring(0, Left.Length);
            }
            return false;
        }
        public override string ToString()
        {
            return EN + " : " + AR;
        }
    }
}
