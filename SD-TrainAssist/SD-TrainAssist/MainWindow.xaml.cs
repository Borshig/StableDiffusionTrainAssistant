using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SD_TrainAssist
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string workDir = "";
        private int currentElement = -1;
        List<string> imgFiles = new List<string>();
        List<string> txtFiles = new List<string>();
        List<string> tokensInMemory = new List<string>();
        FileStream currentOpenedFile;

        public MainWindow()
        {
            InitializeComponent();
        }

        
        private void Button_Prev_Click(object sender, RoutedEventArgs e)
        {
            if (currentElement < 0) return;
            if (currentElement >= 0)
            {
                int prevElement = currentElement - 1;
                if (prevElement < 0) prevElement = imgFiles.Count - 1;

                imgView.Source = new BitmapImage(new Uri(imgFiles[prevElement]));
                currentElement = prevElement;
            }
        }

        private void Button_Next_Click(object sender, RoutedEventArgs e)
        {
            
            if (currentElement < 0) return;
            int nextElement = currentElement + 1;

            if (nextElement > imgFiles.Count - 1) nextElement = 0;

            imgView.Source = new BitmapImage(new Uri(imgFiles[nextElement]));
            currentElement = nextElement;
        }

        private void Button_ChooseWorkDir_Click(object sender, RoutedEventArgs e)
        {
            imgFiles.Clear();
            txtFiles.Clear();


            workDir = tbWorkDir.Text;
            if (!(Directory.Exists(workDir)))
            {
                MessageBox.Show("Not correct workdir");
                return;
            }


            string[] files = Directory.GetFiles(workDir);

            if(files.Length == 0)
            {
                MessageBox.Show("Choosen directory is empty");
                return;
            }

            

            foreach(string file in files)
            {
                if ((file.ToLower().EndsWith(".jpg")) 
                   || (file.ToLower().EndsWith(".jpeg"))
                   || (file.ToLower().EndsWith(".png"))
                ){
                    imgFiles.Add(file);
                }
                else if((file.EndsWith(".txt")))
                {
                    txtFiles.Add(file);
                }
            }
            if (imgFiles.Count == 0)
            {
                MessageBox.Show("There is no images");
                return;
            }

            imgView.Source = new BitmapImage(new Uri(imgFiles[0]));
            currentElement = 0;


            PreProccessFiles();

        }

        private void Button_AddToken_Click(object sender, RoutedEventArgs e)
        {
            string token = tbTokens.Text;
            if (searchTokensInMemory(token) == 0)
            {
                tokensInMemory.Add(token);
            }
            UpdateTokensCheckBoxes();
            tbTokens.Clear();
        }

        private void UpdateTokensCheckBoxes()
        {
            GridCheckboxes.Children.Clear();

            for (int i = 0; i < tokensInMemory.Count; i++)
            {
                CheckBox newCheckBox = new CheckBox();
                newCheckBox.Content = tokensInMemory[i];
                newCheckBox.Name = "Button" + i;
                newCheckBox.Height = 28;
                newCheckBox.Width = 120;
                newCheckBox.FontSize = 20;
                newCheckBox.VerticalContentAlignment = VerticalAlignment.Center;
                newCheckBox.HorizontalAlignment = HorizontalAlignment.Left;
                newCheckBox.VerticalAlignment = VerticalAlignment.Top;

                double y = newCheckBox.Height * i;
                int j = (int)(y / GridCheckboxes.ActualWidth);
                y = (double)(y % GridCheckboxes.ActualHeight);

                newCheckBox.Margin = new Thickness(newCheckBox.Width*j, y, 0.0, 0.0);


                //newBtn.Margin.Left = 0.0;
                //newBtn.Margin.Top = (double)i*15;

                GridCheckboxes.Children.Add(newCheckBox);

            }

            CheckBox chb = new CheckBox();


           

        }

        private void Button_SetForAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PreProccessFiles()
        {
            if(txtFiles.Count == 0)
            {
                foreach (string imgfile in imgFiles)
                {
                    string newTxtFile = System.IO.Path.GetFileNameWithoutExtension(imgfile) + ".txt";
                    txtFiles.Add ( newTxtFile  );



                    File.Create(workDir +"\\"+ newTxtFile).Dispose();


                    //TODO: Exceptions for File.Create!

                    //try { File.Create(newTxtFile); }
                    //catch(...) {}

                    //      T:System.UnauthorizedAccessException:
                    //     The caller does not have the required permission. -or- path specified a file
                    //     that is read-only. -or- path specified a file that is hidden.
                    //
                    //   T:System.ArgumentException:
                    //     .NET Framework and .NET Core versions older than 2.1: path is a zero-length string,
                    //     contains only white space, or contains one or more invalid characters. You can
                    //     query for invalid characters by using the System.IO.Path.GetInvalidPathChars
                    //     method.
                    //
                    //   T:System.ArgumentNullException:
                    //     path is null.
                    //
                    //   T:System.IO.PathTooLongException:
                    //     The specified path, file name, or both exceed the system-defined maximum length.
                    //
                    //   T:System.IO.DirectoryNotFoundException:
                    //     The specified path is invalid (for example, it is on an unmapped drive).
                    //
                    //   T:System.IO.IOException:
                    //     An I/O error occurred while creating the file.
                    //
                    //   T:System.NotSupportedException:
                    //     path is in an invalid format.




                }
            }

            else if(txtFiles.Count < imgFiles.Count)
            {
                foreach (string imgfile in imgFiles)
                {
                    string compTxtFile = System.IO.Path.GetFileNameWithoutExtension(imgfile) + ".txt";

                    if (!File.Exists(compTxtFile))
                    {
                        File.Create(workDir + "\\" + compTxtFile).Dispose();
                    }
                    else
                    {
                        //LoadTokensFromFile()
                    }
                    
                    txtFiles.Add(compTxtFile);
                       
                   
                    //TODO: Exceptions for File.Create!

                    //try { File.Create(newTxtFile); }
                    //catch(...) {}

                    //      T:System.UnauthorizedAccessException:
                    //     The caller does not have the required permission. -or- path specified a file
                    //     that is read-only. -or- path specified a file that is hidden.
                    //
                    //   T:System.ArgumentException:
                    //     .NET Framework and .NET Core versions older than 2.1: path is a zero-length string,
                    //     contains only white space, or contains one or more invalid characters. You can
                    //     query for invalid characters by using the System.IO.Path.GetInvalidPathChars
                    //     method.
                    //
                    //   T:System.ArgumentNullException:
                    //     path is null.
                    //
                    //   T:System.IO.PathTooLongException:
                    //     The specified path, file name, or both exceed the system-defined maximum length.
                    //
                    //   T:System.IO.DirectoryNotFoundException:
                    //     The specified path is invalid (for example, it is on an unmapped drive).
                    //
                    //   T:System.IO.IOException:
                    //     An I/O error occurred while creating the file.
                    //
                    //   T:System.NotSupportedException:
                    //     path is in an invalid format.

                }

                if(txtFiles.Count != imgFiles.Count)
                {
                    MessageBox.Show("Maybe extra .txt without image-pair");
                }


            }// else if




        }

        // return count of this token. should be 0 or 1; 
        private int searchTokensInMemory(string token)
        {
            int sum = 0;
            for (int i = 0; i < tokensInMemory.Count; i++)
            {
                if (tokensInMemory[i] == token) sum++;
            }
                 

            return sum;
        }

        // return count of this token. should be 0 or 1; 
        


    }
}
