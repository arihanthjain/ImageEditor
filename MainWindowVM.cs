using MetadataExtractor;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ImageEditor
{
    class MainWindowVM : INotifyPropertyChanged
    {
        #region Properties
        public static ViewExifData ExifData { get; set; }
        public static StringBuilder ExifTagsDate { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Icommands
        public ICommand LoadCommand
        {
            get { return new MainWindowCommands(LoadImage, CanExecute); }
        }
        public ICommand FlipVerticalXCommand
        {
            get { return new MainWindowCommands(FlipImageVerticalXaxis, CanExecute); }
        }
        public ICommand FlipVerticalYCommand
        {
            get { return new MainWindowCommands(FlipImageVerticalYaxis, CanExecute); }
        }
        public ICommand CropSelection
        {
            get { return new MainWindowCommands(CropImageSelection, CanExecute); }
        }
        public ICommand ViewExifCommand
        {
            get { return new MainWindowCommands(Viewdetails, CanExecute); }
        }
        public ICommand CropImageCommand
        {
            get { return new MainWindowCommands(Crop, CanExecute); }
        }
        public ICommand SaveCommand
        {
            get { return new MainWindowCommands(SaveImage, CanExecute); }
        }


        #endregion

        #region Bindings

        private BitmapImage image_path;
        public BitmapImage ImagePath
        {
            get { return image_path; }
            set
            {
                if (value != null)
                {
                    image_path = value;
                    OnPropertyChanged("ImagePath");
                }
            }
        }
        private string Selected_FileName;
        public string SelectedFileName
        {
            get { return Selected_FileName; }
            set
            {
                if (value != null)
                {
                    Selected_FileName = value;
                    OnPropertyChanged("SelectedFileName");
                }
            }
        }
        private string Crop_Selected;
        public string CropSelected
        {
            get { return Crop_Selected; }
            set
            {
                if (value != null)
                {
                    Crop_Selected = value;
                    OnPropertyChanged("CropSelected");
                }
            }
        }

        #endregion

        #region Constructor
        public MainWindowVM()
        {
            CropSelected = "rbFirst";
            SelectedFileName = string.Empty;
        }
        #endregion

        #region Methods
        private void LoadImage(object obj)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = "c:\\";
            dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png"; ;
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string selectedfileExtension = new FileInfo(dlg.FileName).Extension;
                if (selectedfileExtension == ".jpg" || selectedfileExtension == ".jpeg" || selectedfileExtension == ".jpe" || selectedfileExtension == ".jfif" || selectedfileExtension == ".png")
                {
                    string selectedFileName = dlg.FileName;
                    // FileNameLabel.Content = selectedFileName;
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(selectedFileName);
                    bitmap.EndInit();
                    ImagePath = bitmap;
                    GetEXIFInfo(selectedFileName);
                    SelectedFileName = selectedFileName;
                }
                else
                {
                    System.Windows.MessageBox.Show("Please select an Image file(Supported Image Extesions:JPEG,JPE,JFIF,PNG", "Not Supported Type");
                }
            }
        }
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }
        private static void GetEXIFInfo(string filename)
        {
            ExifTagsDate = new StringBuilder();
            var directories = ImageMetadataReader.ReadMetadata(filename);

            foreach (var directory in directories)
            {
                foreach (var tag in directory.Tags)
                    ExifTagsDate.AppendLine(Convert.ToString(directory.Name + tag.Name + " : " + tag.Description));

                if (directory.HasError)
                {
                    foreach (var error in directory.Errors)
                        ExifTagsDate.AppendLine("ERROR:" + error);
                }
            }
        }
        private void Viewdetails(object obj)
        {
            if (ExifTagsDate != null)
            {
                ExifData = new ViewExifData(ExifTagsDate);
                ExifData.Show();
            }
        }
        private void Crop(object obj)
        {
            System.Windows.Controls.Image croppedImage = new System.Windows.Controls.Image();
            croppedImage.Width = 200;
            croppedImage.Margin = new Thickness(5);
            CroppedBitmap cb = new CroppedBitmap();

            if (CropSelected.Contains("First"))
               cb = new CroppedBitmap( ImagePath, new Int32Rect(0, 0, 50, 50));
            else if (CropSelected.Contains("Second"))
                cb = new CroppedBitmap(ImagePath, new Int32Rect(0, 0, 100, 100));
            else
                cb = new CroppedBitmap(ImagePath, new Int32Rect(0, 0, 200, 200));
            croppedImage.Source = cb;
            string path = Path.GetTempPath() + "temp.jpeg";
            saveCroppedBitmap(cb, path);
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path);
            bitmap.EndInit();
            ImagePath = bitmap;

        }
        public void FlipImageVerticalYaxis(object obj)
        {
            try
            {
                Bitmap temp = new Bitmap(SelectedFileName);
                temp.RotateFlip(RotateFlipType.Rotate180FlipY);
                string path = Path.GetTempPath() + "temp.jpeg";
                if (File.Exists(path))
                {
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    File.Delete(path);
                }
                temp.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path);
                bitmap.EndInit();
                ImagePath = bitmap;
            }
            catch(Exception)
            {
               System.Windows.MessageBox.Show("Uanble To Flip..Please Load the file again and try", "Not Supported Type");
            }

        }
        public void FlipImageVerticalXaxis(object obj)
        {
            try
            {
                Bitmap temp = new Bitmap(SelectedFileName);
                temp.RotateFlip(RotateFlipType.Rotate180FlipX);
                string path = Path.GetTempPath() + "temp.jpeg";
                if (File.Exists(path))
                {
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    File.Delete(path);
                }
                temp.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path);
                bitmap.EndInit();
                ImagePath = bitmap;
            }
            catch(Exception)
            {
               System.Windows.MessageBox.Show("Uanble To Flip..Please Load the file again and try", "Not Supported Type");
            }

        }

        private void CropImageSelection(object paramter)
        {
            CropSelected = (string)paramter;
        }
        private void SaveImage(object obj)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = "c:\\";
            saveFileDialog1.Title = "Save Image Files";
            saveFileDialog1.DefaultExt = "jpeg";
            saveFileDialog1.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var selectedfileExtension = Path.GetExtension(saveFileDialog1.FileName);
                if (selectedfileExtension == ".jpg" || selectedfileExtension == ".jpeg" || selectedfileExtension == ".jpe" || selectedfileExtension == ".jfif" || selectedfileExtension == ".png")
                     Save(ImagePath, saveFileDialog1.FileName);
                else
                    System.Windows.MessageBox.Show("Please Save as an Image file(Supported Image Extesions:JPEG,JPE,JFIF,PNG", "Not Supported Type");

            }
        }
        public  void Save(BitmapImage image, string filePath)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }
        void saveCroppedBitmap(CroppedBitmap image, string path)
        {
            if (File.Exists(path))
            {
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                File.Delete(path);
            }
            FileStream mStream = new FileStream(path, FileMode.Create);
            JpegBitmapEncoder jEncoder = new JpegBitmapEncoder();
            jEncoder.Frames.Add(BitmapFrame.Create(image));
            jEncoder.Save(mStream);
            mStream.Close();
        }
        #endregion
    }
}
